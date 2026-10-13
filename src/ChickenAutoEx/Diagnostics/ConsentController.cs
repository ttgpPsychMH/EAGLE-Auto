using System;

namespace TinhKiemAuto.Diagnostics
{
    public sealed class ConsentController : IDisposable
    {
        private readonly ConsentSettings settings;
        private readonly Func<DiagnosticRecorder> create;
        public DiagnosticConsent Decision { get; private set; }
        public DiagnosticRecorder Recorder { get; private set; }
        public bool SaveSucceeded { get; private set; }
        public bool StartFailed { get; private set; }
        public ConsentController(ConsentSettings settings, Func<DiagnosticRecorder> create)
        { this.settings = settings; this.create = create; Decision = settings.Read(); SaveSucceeded = true; }
        public bool Initialize(Func<bool> prompt)
        {
            if (Decision == DiagnosticConsent.Unknown) return Request(prompt);
            return Decision == DiagnosticConsent.Accepted && Start();
        }
        public bool Request(Func<bool> prompt)
        {
            bool accepted = prompt != null && prompt();
            Decision = accepted ? DiagnosticConsent.Accepted : DiagnosticConsent.Declined;
            SaveSucceeded = settings.Write(Decision);
            if (!accepted) { Stop(); return false; }
            return Start();
        }
        private bool Start()
        {
            if (Recorder != null && Recorder.Enabled) return true;
            Stop(); StartFailed = false;
            try { Recorder = create(); return Recorder != null && Recorder.Enabled; }
            catch { StartFailed = true; return false; }
        }
        public void Revoke()
        {
            Decision = DiagnosticConsent.Declined; Stop(); SaveSucceeded = settings.Write(Decision);
        }
        private void Stop() { var old = Recorder; Recorder = null; if (old != null) old.Dispose(); }
        public void Dispose() { Stop(); }
    }
}
