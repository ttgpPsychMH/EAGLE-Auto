using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChickenAutoEx.Startup
{
    // This client fetches public version metadata only. It cannot grant licenses,
    // change VIP status, install files, open a browser, or attach to the game.
    internal sealed class UpdateClient
    {
        internal async Task<UpdateResult> CheckAsync(UpdateOptions options, int currentVersion, CancellationToken cancellation)
        {
            if (options == null) return new UpdateResult(UpdateStatus.ConfigurationError, "InvalidOptions");
            using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellation))
            using (var handler = new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false })
            using (var client = new HttpClient(handler))
            {
                timeout.CancelAfter(options.TimeoutMilliseconds);
                client.Timeout = Timeout.InfiniteTimeSpan;
                try
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, options.Endpoint))
                    {
                        request.Headers.Accept.ParseAdd("text/plain");
                        using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                            timeout.Token).ConfigureAwait(false))
                        {
                            if (!response.IsSuccessStatusCode)
                                return new UpdateResult(UpdateStatus.Unavailable, "Http" + (int)response.StatusCode);
                            var type = response.Content.Headers.ContentType;
                            if (type == null || !string.Equals(type.MediaType, "text/plain", StringComparison.OrdinalIgnoreCase))
                                return new UpdateResult(UpdateStatus.InvalidMetadata, "UnexpectedContentType");
                            if (response.Content.Headers.ContentLength > UpdateOptions.MaximumMetadataBytes)
                                return new UpdateResult(UpdateStatus.InvalidMetadata, "MetadataTooLarge");

                            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                            using (var body = new MemoryStream())
                            // Disposing the stream on cancellation also bounds stalled body reads
                            // on .NET Framework streams which do not fully honor ReadAsync's token.
                            using (timeout.Token.Register(() => stream.Dispose()))
                            {
                                var buffer = new byte[4096];
                                int read;
                                while ((read = await stream.ReadAsync(buffer, 0, buffer.Length, timeout.Token).ConfigureAwait(false)) > 0)
                                {
                                    if (body.Length + read > UpdateOptions.MaximumMetadataBytes)
                                        return new UpdateResult(UpdateStatus.InvalidMetadata, "MetadataTooLarge");
                                    body.Write(buffer, 0, read);
                                }
                                timeout.Token.ThrowIfCancellationRequested();
                                var text = new UTF8Encoding(false, true).GetString(body.ToArray());
                                return UpdateMetadata.Parse(text, currentVersion);
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    return Cancelled(cancellation);
                }
                catch (HttpRequestException)
                {
                    return timeout.IsCancellationRequested ? Cancelled(cancellation)
                        : new UpdateResult(UpdateStatus.Unavailable, "TransportError");
                }
                catch (DecoderFallbackException)
                {
                    return new UpdateResult(UpdateStatus.InvalidMetadata, "InvalidUtf8");
                }
                catch (IOException)
                {
                    return timeout.IsCancellationRequested ? Cancelled(cancellation)
                        : new UpdateResult(UpdateStatus.Unavailable, "ReadError");
                }
                catch (ObjectDisposedException)
                {
                    if (!timeout.IsCancellationRequested) throw;
                    return Cancelled(cancellation);
                }
            }
        }

        private static UpdateResult Cancelled(CancellationToken caller)
        {
            return caller.IsCancellationRequested
                ? new UpdateResult(UpdateStatus.Cancelled, "Cancelled")
                : new UpdateResult(UpdateStatus.Unavailable, "Timeout");
        }
    }
}
