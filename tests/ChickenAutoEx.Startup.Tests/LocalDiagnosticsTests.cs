using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TinhKiemAuto.Diagnostics;
using Xunit;

namespace ChickenAutoEx.Startup.Tests
{
    [CollectionDefinition("Local diagnostic globals", DisableParallelization = true)]
    public sealed class LocalDiagnosticCollection { }

    [Collection("Local diagnostic globals")]
    public sealed class LocalDiagnosticsTests
    {
        private const string Secret = "password=SUPER_SECRET; token=hidden; character=PRIVATE_NAME; chat=私密; C:\\Users\\private\\game.cfg";
        private sealed class Temp : IDisposable
        {
            public string Folder = Path.Combine(Path.GetTempPath(), "eagle-diag-test-" + Guid.NewGuid().ToString("N"));
            public void Dispose() { if (Directory.Exists(Folder)) Directory.Delete(Folder, true); }
        }
        private sealed class MemoryStore : IDiagnosticStore
        {
            public readonly ConcurrentQueue<string> Rows = new ConcurrentQueue<string>();
            public bool FailWrite, FailDispose, Disposed;
            public int WritesAfterDispose;
            public void Write(string row) { if (Disposed) { WritesAfterDispose++; throw new ObjectDisposedException("store"); } if (FailWrite) throw new IOException(Secret); Rows.Enqueue(row); }
            public void Dispose() { Disposed = true; if (FailDispose) throw new IOException(Secret); }
        }
        private static string Row(DiagnosticEvent code = DiagnosticEvent.SessionStarted)
        { return DiagnosticSchema.Row(DateTime.UtcNow, "0.11", code, DiagnosticFeature.General); }

        [Fact] public void UnknownConsentDoesNotCreateFolderOrRecorderBeforePrompt()
        {
            using var temp = new Temp(); int starts = 0;
            using var controller = new ConsentController(new ConsentSettings(temp.Folder), () => { starts++; return new DiagnosticRecorder("0.11", new MemoryStore(), false); });
            Assert.Equal(DiagnosticConsent.Unknown, controller.Decision); Assert.False(Directory.Exists(temp.Folder)); Assert.Equal(0, starts);
            Assert.False(controller.Initialize(() => false)); Assert.Equal(0, starts);
            Assert.Single(Directory.GetFiles(temp.Folder)); Assert.Empty(Directory.GetFiles(temp.Folder, "*.csv"));
        }
        [Fact] public void AcceptedConsentRestartsAutomaticallyWithoutPrompt()
        {
            using var temp = new Temp(); var settings = new ConsentSettings(temp.Folder); Assert.True(settings.Write(DiagnosticConsent.Accepted));
            int starts = 0, prompts = 0;
            using var controller = new ConsentController(settings, () => { starts++; return new DiagnosticRecorder("0.11", new MemoryStore(), false); });
            Assert.True(controller.Initialize(() => { prompts++; return false; }));
            Assert.True(controller.Initialize(() => { prompts++; return false; })); Assert.Equal(0, prompts); Assert.Equal(1, starts);
        }
        [Fact] public void DeclinedConsentDoesNotStartOrAskAgain()
        {
            using var temp = new Temp(); var settings = new ConsentSettings(temp.Folder); settings.Write(DiagnosticConsent.Declined);
            using var controller = new ConsentController(settings, () => throw new Exception("Factory must not run"));
            Assert.False(controller.Initialize(() => throw new Exception("Prompt must not run"))); Assert.Null(controller.Recorder);
        }
        [Fact] public void ExplicitEnableCanChangeEarlierDecline()
        {
            using var temp = new Temp(); var settings = new ConsentSettings(temp.Folder); settings.Write(DiagnosticConsent.Declined);
            using var controller = new ConsentController(settings, () => new DiagnosticRecorder("0.11", new MemoryStore(), false));
            Assert.True(controller.Request(() => true)); Assert.Equal(DiagnosticConsent.Accepted, settings.Read());
            controller.Revoke(); Assert.Null(controller.Recorder); Assert.Equal(DiagnosticConsent.Declined, settings.Read());
        }
        [Theory]
        [InlineData("EAGLE-DIAGNOSTICS-POLICY-0\nAccepted")]
        [InlineData("EAGLE-DIAGNOSTICS-POLICY-1\nAccepted\nextra")]
        [InlineData("password=SUPER_SECRET")]
        [InlineData("")]
        public void InvalidOrOutdatedPolicyRequiresNewConsent(string content)
        {
            using var temp = new Temp(); Directory.CreateDirectory(temp.Folder); File.WriteAllText(Path.Combine(temp.Folder, "consent.txt"), content);
            Assert.Equal(DiagnosticConsent.Unknown, new ConsentSettings(temp.Folder).Read());
        }
        [Fact] public void OversizedConsentFileCannotEnableCollection()
        {
            using var temp = new Temp(); Directory.CreateDirectory(temp.Folder); File.WriteAllText(Path.Combine(temp.Folder, "consent.txt"), new string('a', 1024));
            Assert.Equal(DiagnosticConsent.Unknown, new ConsentSettings(temp.Folder).Read());
        }
        [Fact] public void ConsentStorageFailureDoesNotGrantConsentOrCrash()
        {
            using var temp = new Temp(); Directory.CreateDirectory(temp.Folder); string file = Path.Combine(temp.Folder, "not-folder"); File.WriteAllText(file, "x");
            var settings = new ConsentSettings(file); Assert.Equal(DiagnosticConsent.Unknown, settings.Read());
            using var controller = new ConsentController(settings, () => throw new Exception("Must not start"));
            Assert.False(controller.Initialize(() => false)); Assert.False(controller.SaveSucceeded);
        }
        [Fact] public void CollectorConstructionFailureLeavesApplicationUsable()
        {
            using var temp = new Temp(); using var controller = new ConsentController(new ConsentSettings(temp.Folder), () => throw new IOException(Secret));
            Assert.False(controller.Initialize(() => true)); Assert.True(controller.StartFailed); Assert.Null(controller.Recorder);
        }
        [Fact] public void TypedErrorsNeverSerializeExceptionMessagesStackDataOrCustomTypeNames()
        {
            var store = new MemoryStore(); using var recorder = new DiagnosticRecorder("0.11", store, false);
            LocalDiagnostics.Attach(recorder, typeof(LocalDiagnosticsTests).Assembly);
            try {
                Exception error; try { TinhKiemAuto.DiagnosticFixtures.Actions.UseSkillFailure(Secret); throw new Exception("fixture must throw"); } catch (InvalidOperationException ex) { error = ex; }
                error.Data["password"] = Secret; LocalDiagnostics.ObserveException(error); recorder.Drain();
                string row = Assert.Single(store.Rows); Assert.Contains("Skills", row); Assert.Contains("InvalidOperation", row);
                Assert.DoesNotContain("SUPER_SECRET", row); Assert.DoesNotContain("PRIVATE_NAME", row); Assert.DoesNotContain("private", row);
                Assert.DoesNotContain("UseSkillFailure", row); Assert.DoesNotContain("password", row);
                LocalDiagnostics.ObserveException(new VerySecretCustomException(Secret), true); recorder.Drain();
                Assert.Contains(store.Rows, r => r.Contains("UnhandledException") && r.Contains("Other"));
                Assert.DoesNotContain(store.Rows, r => r.Contains("VerySecret") || r.Contains("SUPER_SECRET"));
            } finally { LocalDiagnostics.Detach(); }
        }
        private sealed class VerySecretCustomException : Exception { public VerySecretCustomException(string message) : base(message) { } }
        [Fact] public void ExistingWarningsKeepOnlyFeatureAndFiniteReason()
        {
            var store = new MemoryStore(); using var recorder = new DiagnosticRecorder("0.11", store, false); LocalDiagnostics.Attach(recorder, GetType().Assembly);
            try {
                LocalDiagnostics.Warning("Ác Bá " + Secret, "hết thời gian chờ " + Secret, 3); recorder.Drain();
                string row = Assert.Single(store.Rows); Assert.Contains("Warning", row); Assert.Contains("WaitingTimeout", row);
                Assert.DoesNotContain("SUPER_SECRET", row); Assert.DoesNotContain("PRIVATE_NAME", row);
                LocalDiagnostics.Warning("Thông báo", Secret, 0); recorder.Drain(); Assert.Single(store.Rows);
            } finally { LocalDiagnostics.Detach(); }
        }
        [Fact] public void DisabledStaticObserverCreatesNoFilesOrRows()
        {
            LocalDiagnostics.Detach(); LocalDiagnostics.Warning("Ác Bá", Secret, 3); LocalDiagnostics.ObserveException(new Exception(Secret), true);
            Assert.False(LocalDiagnostics.Enabled);
        }
        [Theory]
        [InlineData("UseSkill", DiagnosticFeature.Skills)] [InlineData("CheDo", DiagnosticFeature.Crafting)]
        [InlineData("Đã dừng Trừng Ác", DiagnosticFeature.TrungAc)] [InlineData("Kỳ Cuộc", DiagnosticFeature.KyCuoc)]
        [InlineData("Thủy Lao", DiagnosticFeature.ThuyLao)] [InlineData("Ác Tặc", DiagnosticFeature.AcTac)]
        [InlineData("Ác Bá", DiagnosticFeature.AcBa)] [InlineData("BuffPet", DiagnosticFeature.Pet)]
        [InlineData("PickItem", DiagnosticFeature.Loot)] [InlineData("HoiSinh", DiagnosticFeature.Recovery)]
        [InlineData("GetCMDBYMD5", DiagnosticFeature.General)] [InlineData("Move", DiagnosticFeature.Travel)]
        [InlineData("Login", DiagnosticFeature.AutoLogin)] [InlineData("SetHook", DiagnosticFeature.Hook)]
        public void FeaturesAreMappedToFiniteLabels(string input, DiagnosticFeature expected) { Assert.Equal(expected, DiagnosticSchema.Feature(input)); }
        [Fact] public void NonAppErrorsAndDiagnosticInternalFailuresAreFiltered()
        {
            Exception ex; try { int.Parse("not-an-int"); throw new Exception(); } catch (FormatException error) { ex = error; }
            LocalDiagnostics.Classify(ex, GetType().Assembly, out _, out bool belongs); Assert.False(belongs);
            try { DiagnosticSchema.Version(Secret); throw new Exception(); } catch (ArgumentException error) { ex = error; }
            LocalDiagnostics.Classify(ex, GetType().Assembly, out _, out belongs); Assert.False(belongs);
        }
        [Fact] public void FrameworkErrorStormDoesNotHideTheNextOwnedException()
        {
            var store = new MemoryStore(); using var recorder = new DiagnosticRecorder("0.14", store, false);
            LocalDiagnostics.Attach(recorder, GetType().Assembly);
            try
            {
                for(int i=0;i<200;i++)
                {
                    try { int.Parse("not-an-int"); } catch(FormatException error) { LocalDiagnostics.ObserveException(error); }
                }
                try { TinhKiemAuto.DiagnosticFixtures.Actions.UseSkillFailure(Secret); }
                catch(InvalidOperationException error) { LocalDiagnostics.ObserveException(error); }
                recorder.Drain(); Assert.Contains("Skills",Assert.Single(store.Rows));Assert.Equal(0,recorder.Dropped);
            }
            finally { LocalDiagnostics.Detach(); }
        }
        [Fact] public void QueueDropsAndErrorBudgetDropsAreReportedSeparately()
        {
            var store = new MemoryStore(); using var recorder = new DiagnosticRecorder("0.14",store,false);
            for(int i=0;i<DiagnosticRecorder.QueueLimit+3;i++) recorder.Record(DiagnosticSchema.Row(DateTime.UtcNow,"0.14",DiagnosticEvent.TaskState,DiagnosticFeature.AcTac));
            for(int i=0;i<62;i++) recorder.TakeExceptionBudget(1);
            Assert.Equal(3,recorder.QueueDropped);Assert.Equal(2,recorder.ErrorDropped);Assert.Equal(5,recorder.Dropped);
        }
        [Theory]
        [InlineData("password=SUPER_SECRET")] [InlineData("0.11\nsecret")] [InlineData("=CMD()")] [InlineData("0,11")]
        public void UntrustedVersionCannotEnterCsv(string version) { Assert.Throws<ArgumentException>(() => DiagnosticSchema.Version(version)); }
        [Fact] public void InvalidEnumAndMetricValuesRejected()
        {
            Assert.Throws<ArgumentException>(() => DiagnosticSchema.Row(DateTime.UtcNow, "0.11", (DiagnosticEvent)999, DiagnosticFeature.General));
            Assert.Throws<ArgumentException>(() => DiagnosticSchema.Row(DateTime.UtcNow, "0.11", DiagnosticEvent.ProcessSample, DiagnosticFeature.General, cpu: double.NaN));
            Assert.Throws<ArgumentException>(() => DiagnosticSchema.Row(DateTime.UtcNow, "0.11", DiagnosticEvent.ProcessSample, DiagnosticFeature.General, working: -1));
            Assert.Throws<ArgumentException>(() => DiagnosticSchema.Row(DateTime.UtcNow, "0.11", DiagnosticEvent.ProcessSample, DiagnosticFeature.General, window: 10000));
        }
        [Fact] public void RawMessageCsvFormulaAndLineInjectionCannotReachStore()
        {
            var store = new MemoryStore(); using var recorder = new DiagnosticRecorder("0.11", store, false);
            Assert.False(recorder.Record(Secret)); Assert.False(recorder.Record(Row() + "\n" + Secret));
            string[] cells = Row().Split(','); cells[3] = "=cmd|'/c x'!A0"; Assert.False(recorder.Record(string.Join(",", cells)));
            cells[3] = "General"; cells[6] = "PRIVATE_NAME"; Assert.False(recorder.Record(string.Join(",", cells)));
            recorder.Drain(); Assert.Empty(store.Rows);
        }
        [Fact] public void CpuRamRowsUseAnonymousAliasAndInvariantNumbers()
        {
            var before = System.Globalization.CultureInfo.CurrentCulture;
            try { System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("vi-VN");
                string row = DiagnosticSchema.Row(DateTime.UtcNow, "0.11", DiagnosticEvent.ProcessSample, DiagnosticFeature.General,
                    window: 3, cpu: 1.25, working: 1048576, privateBytes: 2097152, handles: 50);
                Assert.Equal(17, row.Split(',').Length); Assert.Contains("window_0003,1.25,1.00,2.00,50", row);
            } finally { System.Globalization.CultureInfo.CurrentCulture = before; }
        }
        [Fact] public void QueueAndErrorStormHaveBoundedMemory()
        {
            var store = new MemoryStore(); using var recorder = new DiagnosticRecorder("0.11", store, false);
            for (int i = 0; i < 1000; i++) recorder.Record(Row()); recorder.Drain();
            Assert.Equal(DiagnosticRecorder.QueueLimit, store.Rows.Count); Assert.Equal(1000 - DiagnosticRecorder.QueueLimit, recorder.Dropped);
            for (int i = 0; i < 60; i++) Assert.True(recorder.TakeExceptionBudget(100));
            Assert.False(recorder.TakeExceptionBudget(100)); Assert.True(recorder.TakeExceptionBudget(60000));
        }
        [Fact] public void RevokingDiscardsQueuedRowsAndPreventsNewWrites()
        {
            var store = new MemoryStore(); var recorder = new DiagnosticRecorder("0.11", store, false);
            recorder.Record(Row()); recorder.Dispose(); recorder.Drain(); Assert.Empty(store.Rows);
            Assert.False(recorder.Record(Row())); Assert.True(store.Disposed); Assert.Equal(0, store.WritesAfterDispose);
        }
        [Fact] public void DiskWriteFailureStopsRecordingWithoutThrowingOrLeakingDetails()
        {
            var store = new MemoryStore { FailWrite = true }; using var recorder = new DiagnosticRecorder("0.11", store, false);
            recorder.Record(Row()); recorder.Drain(); Assert.True(recorder.Failed); Assert.False(recorder.Enabled);
            Assert.False(recorder.Record(Row())); Assert.Empty(store.Rows);
        }
        [Fact] public void DisposeFailureIsContained()
        {
            var recorder = new DiagnosticRecorder("0.11", new MemoryStore { FailDispose = true }, false);
            recorder.Dispose(); Assert.True(recorder.Failed); recorder.Dispose();
        }
        [Fact] public void BackgroundWriterAndConcurrentShutdownDoNotWriteAfterDisposal()
        {
            var store = new MemoryStore(); var recorder = new DiagnosticRecorder("0.11", store);
            Parallel.For(0, 300, _ => recorder.Record(Row())); Assert.True(SpinWait.SpinUntil(() => store.Rows.Count > 0, 3000));
            recorder.Dispose(); int count = store.Rows.Count;
            Parallel.For(0, 100, _ => Assert.False(recorder.Record(Row()))); recorder.Drain(); Assert.Equal(count, store.Rows.Count);
            Assert.Equal(0, store.WritesAfterDispose);
        }
        [Fact] public void FatalRowCanBeFlushedWithoutWaitingForWorker()
        {
            var store = new MemoryStore(); using var recorder = new DiagnosticRecorder("0.11", store, false);
            recorder.FlushUnhandled(Row(DiagnosticEvent.UnhandledException)); Assert.Single(store.Rows);
            recorder.Dispose(); recorder.FlushUnhandled(Row()); Assert.Single(store.Rows);
        }
        [Fact] public void RotationBoundsFilesAndLeavesUnrelatedUserFilesAlone()
        {
            using var temp = new Temp(); Directory.CreateDirectory(temp.Folder);
            File.WriteAllText(Path.Combine(temp.Folder, "keep.csv"), Secret);
            File.WriteAllText(Path.Combine(temp.Folder, "eagle-diagnostics-manual.csv"), Secret);
            using (var store = new RotatingDiagnosticStore(temp.Folder, 512, 4))
                for (int i = 0; i < 100; i++) store.Write(Row());
            var files = Directory.GetFiles(temp.Folder).Where(f => Path.GetFileName(f) != "keep.csv" && Path.GetFileName(f) != "eagle-diagnostics-manual.csv").ToArray();
            Assert.Equal(4, files.Length); Assert.All(files, f => { Assert.True(new FileInfo(f).Length <= 512); Assert.Equal(DiagnosticSchema.Header, File.ReadLines(f).First()); });
            Assert.Equal(Secret, File.ReadAllText(Path.Combine(temp.Folder, "keep.csv")));
            Assert.Equal(Secret, File.ReadAllText(Path.Combine(temp.Folder, "eagle-diagnostics-manual.csv")));
        }
        [Fact] public void SessionRestartRetainsAtMostConfiguredLogFiles()
        {
            using var temp = new Temp(); for (int i = 0; i < 8; i++) using (var store = new RotatingDiagnosticStore(temp.Folder, 512, 2)) store.Write(Row());
            Assert.Equal(2, Directory.GetFiles(temp.Folder, "*.csv").Length);
        }
        [Fact] public void InvalidLogDirectoryFailsOnlyDiagnostics()
        {
            using var temp = new Temp(); Directory.CreateDirectory(temp.Folder); string file = Path.Combine(temp.Folder, "file"); File.WriteAllText(file, "x");
            using var recorder = new DiagnosticRecorder("0.11", new RotatingDiagnosticStore(file), false);
            recorder.Record(Row()); recorder.Drain(); Assert.True(recorder.Failed); Assert.Equal("x", File.ReadAllText(file));
        }
    }
}

namespace TinhKiemAuto.DiagnosticFixtures
{
    public static class Actions
    {
        [MethodImpl(MethodImplOptions.NoInlining)] public static void UseSkillFailure(string text) { throw new InvalidOperationException(text); }
    }
}
