using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChickenAutoEx.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TinhKiemAuto.Models;
using Xunit;

namespace ChickenAutoEx.Startup.Tests
{
    public sealed class UpdateServiceTests
    {
        [Theory]
        [InlineData("Version = 107\n", (int)UpdateStatus.UpToDate, 107)]
        [InlineData("Version = 108\r\n", (int)UpdateStatus.UpdateAvailable, 108)]
        [InlineData("Version = 106\n", (int)UpdateStatus.UpToDate, 106)]
        [InlineData("\ufeff; comment\nVersion = 107\nFileCheckSum = unused\n", (int)UpdateStatus.UpToDate, 107)]
        public async Task FetchesValidIniOverRealHttp(string ini, int status, int version)
        {
            await using (var service = await FakeService.Start(async context =>
            {
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync(ini);
            }))
            {
                var result = await Check(service.Url);
                Assert.Equal((UpdateStatus)status, result.Status);
                Assert.Equal(version, result.RemoteVersion);
                Assert.Equal(1, service.RequestCount);
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("Other = 107")]
        [InlineData("Version = nope")]
        [InlineData("Version = -1")]
        [InlineData("Version = 0")]
        [InlineData("Version = 2147483648")]
        [InlineData("Version = 107\nVersion = 108")]
        [InlineData("Version = 107\nVersion = broken")]
        [InlineData("<html>Version = 107</html>")]
        [InlineData("Version = 107\nnot an ini line")]
        public async Task RejectsMalformedOrParkingContent(string body)
        {
            await using (var service = await FakeService.Start(async context =>
            {
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(body);
            }))
                Assert.Equal(UpdateStatus.InvalidMetadata, (await Check(service.Url)).Status);
        }

        [Fact]
        public async Task RejectsHtmlEvenIfItsBodyLooksLikeIni()
        {
            await using (var service = await FakeService.Start(async context =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync("Version = 107");
            }))
                Assert.Equal("UnexpectedContentType", (await Check(service.Url)).DiagnosticCode);
        }

        [Theory]
        [InlineData(401)]
        [InlineData(403)]
        [InlineData(404)]
        [InlineData(503)]
        public async Task ReportsHttpFailuresWithoutChangingEntitlements(int status)
        {
            await using (var service = await FakeService.Start(context =>
            {
                context.Response.StatusCode = status;
                return Task.CompletedTask;
            }))
            {
                var result = await Check(service.Url);
                Assert.Equal(UpdateStatus.Unavailable, result.Status);
                Assert.Equal("Http" + status, result.DiagnosticCode);
                Assert.Null(result.RemoteVersion);
            }
        }

        [Fact]
        public async Task DoesNotFollowRedirectsOrContactTheTarget()
        {
            await using (var target = await FakeService.Start(context => Task.CompletedTask))
            await using (var redirect = await FakeService.Start(context =>
            {
                context.Response.Redirect(target.Url.ToString());
                return Task.CompletedTask;
            }))
            {
                var result = await Check(redirect.Url);
                Assert.Equal("Http302", result.DiagnosticCode);
                Assert.Equal(0, target.RequestCount);
            }
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task BoundsOversizedMetadataWithAndWithoutContentLength(bool knownLength)
        {
            var bytes = Encoding.UTF8.GetBytes("Version = 107\nNote = " + new string('x', UpdateOptions.MaximumMetadataBytes));
            await using (var service = await FakeService.Start(async context =>
            {
                context.Response.ContentType = "text/plain";
                if (knownLength) context.Response.ContentLength = bytes.Length;
                await context.Response.Body.WriteAsync(bytes);
            }))
                Assert.Equal("MetadataTooLarge", (await Check(service.Url)).DiagnosticCode);
        }

        [Fact]
        public async Task RejectsInvalidUtf8()
        {
            await using (var service = await FakeService.Start(async context =>
            {
                context.Response.ContentType = "text/plain";
                await context.Response.Body.WriteAsync(new byte[] { 0xff, 0xfe, 0xff });
            }))
                Assert.Equal("InvalidUtf8", (await Check(service.Url)).DiagnosticCode);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task TimeoutCoversBothHeadersAndStalledBody(bool sendHeaders)
        {
            await using (var service = await FakeService.Start(async context =>
            {
                context.Response.ContentType = "text/plain";
                if (sendHeaders)
                {
                    await context.Response.WriteAsync("Version = ");
                    await context.Response.Body.FlushAsync();
                }
                try { await Task.Delay(10000, context.RequestAborted); }
                catch (OperationCanceledException) { }
            }))
            {
                var time = Stopwatch.StartNew();
                var result = await Check(service.Url, 250);
                Assert.Equal("Timeout", result.DiagnosticCode);
                Assert.True(time.Elapsed < TimeSpan.FromSeconds(3), "The deadline must include response-body reads.");
            }
        }

        [Fact]
        public async Task CallerCancellationStopsAnInFlightRequest()
        {
            var started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            await using (var service = await FakeService.Start(async context =>
            {
                started.TrySetResult(true);
                try { await Task.Delay(10000, context.RequestAborted); }
                catch (OperationCanceledException) { }
            }))
            using (var cancellation = new CancellationTokenSource())
            {
                var task = Check(service.Url, 5000, cancellation.Token);
                await started.Task.WaitAsync(TimeSpan.FromSeconds(3));
                cancellation.Cancel();
                Assert.Equal(UpdateStatus.Cancelled, (await task).Status);
            }
        }

        [Fact]
        public async Task RefusesAnUntrustedTlsCertificate()
        {
            await using (var service = await FakeService.Start(context => Task.CompletedTask, true))
            {
                var result = await Check(service.Url);
                Assert.Equal(UpdateStatus.Unavailable, result.Status);
                Assert.Equal("TransportError", result.DiagnosticCode);
                Assert.Equal(0, service.RequestCount);
            }
        }

        [Theory]
        [InlineData("http://update.chickenauto.com/PatchInfoEx.ini", "5000")]
        [InlineData("https://user:password@example.com/PatchInfoEx.ini", "5000")]
        [InlineData("https://example.com/PatchInfoEx.ini?token=secret", "5000")]
        [InlineData("https://example.com/PatchInfoEx.ini#fragment", "5000")]
        [InlineData("file:///tmp/PatchInfoEx.ini", "5000")]
        [InlineData("https://example.com/PatchInfoEx.ini", "0")]
        [InlineData("https://example.com/PatchInfoEx.ini", "30001")]
        [InlineData("https://example.com/PatchInfoEx.ini", "not a timeout")]
        public void RejectsUnsafeEndpointsAndUnboundedTimeouts(string endpoint, string timeout)
        {
            UpdateOptions options;
            Assert.False(UpdateOptions.TryCreate(endpoint, timeout, out options));
            Assert.Null(options);
        }

        [Fact]
        public void KeepsDiagnosticsFreeOfUrlsAndResponseBodies()
        {
            var message = StartupMessages.ForUpdate(new UpdateResult(UpdateStatus.Unavailable, "TransportError"));
            Assert.Contains("tiếp tục khởi động", message);
            Assert.DoesNotContain("http", message);
            Assert.DoesNotContain("token", message);
        }

        [Fact]
        public void ExistingTrainDataSchemaSurvivesJsonDependencyUpgrade()
        {
            const string original = "{\"Name\":\"Bãi thử\",\"PosX\":123,\"PosY\":45,\"Level\":40,\"MapID\":8,\"MapName\":\"Đôn Hoàng\"}";
            var item = JsonConvert.DeserializeObject<BaiTrain>(original);
            var roundtrip = JsonConvert.DeserializeObject<BaiTrain>(JsonConvert.SerializeObject(item));
            Assert.Equal("Bãi thử", roundtrip.Name);
            Assert.Equal(123, roundtrip.PosX);
            Assert.Equal(45, roundtrip.PosY);
            Assert.Equal(40, roundtrip.Level);
            Assert.Equal(8, roundtrip.MapID);
            Assert.Equal("Đôn Hoàng", roundtrip.MapName);
        }

        private static Task<UpdateResult> Check(Uri endpoint, int timeout = 2000, CancellationToken cancellation = default)
        {
            UpdateOptions options;
            Assert.True(UpdateOptions.TryCreate(endpoint.ToString(), timeout.ToString(CultureInfo.InvariantCulture), out options));
            return new UpdateClient().CheckAsync(options, 107, cancellation);
        }

        private sealed class FakeService : IAsyncDisposable
        {
            private readonly WebApplication application;
            private readonly X509Certificate2 certificate;
            private int requestCount;
            internal int RequestCount => Volatile.Read(ref requestCount);
            internal Uri Url { get; private set; }

            private FakeService(WebApplication application, X509Certificate2 certificate)
            {
                this.application = application;
                this.certificate = certificate;
            }

            internal static async Task<FakeService> Start(RequestDelegate handler, bool tls = false)
            {
                X509Certificate2 certificate = null;
                if (tls)
                {
                    using (var rsa = RSA.Create(2048))
                    {
                        var request = new CertificateRequest("CN=localhost", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                        var names = new SubjectAlternativeNameBuilder();
                        names.AddIpAddress(IPAddress.Loopback);
                        request.CertificateExtensions.Add(names.Build());
                        certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddHours(1));
                    }
                }
                var builder = WebApplication.CreateBuilder();
                builder.Logging.ClearProviders();
                builder.WebHost.ConfigureKestrel(server => server.Listen(IPAddress.Loopback, 0, listen =>
                {
                    if (tls) listen.UseHttps(certificate);
                }));
                var application = builder.Build();
                var service = new FakeService(application, certificate);
                application.Run(async context =>
                {
                    Interlocked.Increment(ref service.requestCount);
                    await handler(context);
                });
                await application.StartAsync();
                var addresses = application.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();
                service.Url = new Uri(addresses.Addresses.Single());
                return service;
            }

            public async ValueTask DisposeAsync()
            {
                await application.StopAsync();
                await application.DisposeAsync();
                certificate?.Dispose();
            }
        }
    }
}
