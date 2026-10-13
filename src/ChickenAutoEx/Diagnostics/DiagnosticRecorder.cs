using System;
using System.Collections.Generic;
using System.Threading;

namespace TinhKiemAuto.Diagnostics
{
    public interface IDiagnosticStore : IDisposable { void Write(string row); }

    public sealed class DiagnosticRecorder : IDisposable
    {
        public const int QueueLimit = 256;
        private readonly object queueLock = new object(), writeLock = new object();
        private readonly Queue<string> queue = new Queue<string>();
        private readonly IDiagnosticStore store;
        private readonly AutoResetEvent signal = new AutoResetEvent(false);
        private readonly Thread worker;
        private volatile bool enabled = true, failed;
        private long dropped, exceptionWindow;
        private int exceptionsThisWindow;
        private bool disposed;
        public string Version { get; private set; }
        public bool Enabled { get { return enabled; } }
        public bool Failed { get { return failed; } }
        public long Dropped { get { lock (queueLock) return dropped; } }
        public DiagnosticRecorder(string version, IDiagnosticStore store, bool startWorker = true)
        {
            Version = DiagnosticSchema.Version(version); this.store = store ?? throw new ArgumentNullException("store");
            if (startWorker) { worker = new Thread(Work) { IsBackground = true, Name = "EAGLE local diagnostics" }; worker.Start(); }
        }
        public bool Record(string row)
        {
            // Caller must use the typed schema. Reject raw messages/multiline data at this boundary too.
            if (row == null || row.Length > 1024 || row.IndexOf('\n') >= 0 || row.IndexOf('\r') >= 0) return false;
            if (!DiagnosticRowValidator.Valid(row, Version)) return false;
            lock (queueLock)
            {
                if (!enabled) return false;
                if (queue.Count == QueueLimit) { dropped++; return false; }
                queue.Enqueue(row); signal.Set(); return true;
            }
        }
        public bool TakeExceptionBudget(long milliseconds)
        {
            lock (queueLock)
            {
                if (!enabled) return false;
                if (milliseconds - exceptionWindow >= 60000) { exceptionWindow = milliseconds; exceptionsThisWindow = 0; }
                if (exceptionsThisWindow >= 60) { dropped++; return false; }
                exceptionsThisWindow++; return true;
            }
        }
        public void Drain()
        {
            while (true)
            {
                string row;
                lock (queueLock) { if (!enabled || queue.Count == 0) return; row = queue.Dequeue(); }
                lock (writeLock)
                {
                    if (!enabled) return;
                    try { store.Write(row); }
                    catch { failed = true; enabled = false; lock (queueLock) queue.Clear(); return; }
                }
            }
        }
        private void Work()
        {
            while (enabled) { signal.WaitOne(500); Drain(); }
        }
        public void FlushUnhandled(string row)
        {
            if (!DiagnosticRowValidator.Valid(row, Version)) return;
            // A fatal event may not leave time for the background writer. Typed, bounded row only.
            lock (writeLock)
            {
                if (!enabled) return;
                try { store.Write(row); }
                catch { failed = true; enabled = false; }
            }
        }
        public void Dispose()
        {
            lock (writeLock)
            {
                if (disposed) return;
                disposed = true; enabled = false; lock (queueLock) queue.Clear(); signal.Set();
                try { store.Dispose(); } catch { failed = true; }
            }
            if (worker != null && worker != Thread.CurrentThread) worker.Join(1000);
            if (worker == null || !worker.IsAlive) signal.Dispose();
            // If an OS write is still unwinding, its handle is released by GC after the worker exits.
        }
    }
}
