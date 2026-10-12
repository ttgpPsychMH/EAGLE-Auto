using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TinhKiemAuto.Hooks
{
    public enum ReaderObservation
    {
        NotRequested, RequestQueuedNoAck, DataObserved, RestoreFailed, RestoreWrittenNoAck
    }

    public enum HookEvent
    {
        WindowHookAccepted, WindowHookRejected, ReceiverQueuedNoAck, ReceiverQueueRejected,
        PrerequisiteMissing, RecvAddressMissing, RecvReadRejected, RecvReadShort,
        ReaderQueuedNoAck, ReaderQueueRejected, ExceptionObserved,
        CopyDataObserved, CopyDataUnexpected, CopyDataShapeRejected,
        RestoreRejected, RestoreShort, RestoreWrittenNoAck
    }

    // Only numeric API outcomes and allowlisted event names. Never retain packet/chat/account data.
    public sealed class HookDiagnostics
    {
        private readonly object sync = new object();
        private readonly Queue<string> history = new Queue<string>();
        private readonly Dictionary<HookEvent, long> counts = new Dictionary<HookEvent, long>();
        private string last;
        private bool windowHookAccepted, receiverQueued;
        private ReaderObservation reader;
        private long observedBytes;
        public const int HistoryLimit = 32;

        public ReaderObservation Reader { get { lock (sync) return reader; } }
        public bool CanRequestReader
        {
            get { lock (sync) return windowHookAccepted && receiverQueued
                && (reader == ReaderObservation.NotRequested || reader == ReaderObservation.RestoreWrittenNoAck); }
        }

        private void Record(HookEvent code, int value = 0, int error = 0)
        {
            long count;
            counts.TryGetValue(code, out count);
            counts[code] = count + 1;
            string row = code + " value=" + value.ToString(CultureInfo.InvariantCulture)
                + " code=" + error.ToString(CultureInfo.InvariantCulture);
            if (row == last) return; // Counters remain exact; repeated polling cannot grow the log.
            last = row;
            if (history.Count == HistoryLimit) history.Dequeue();
            history.Enqueue(DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture) + " " + row);
        }

        public void WindowHookResult(bool accepted)
        {
            lock (sync)
            {
                windowHookAccepted = accepted;
                receiverQueued = false;
                // Do not erase an outstanding recv request or a failed restore on reinitialization.
                Record(accepted ? HookEvent.WindowHookAccepted : HookEvent.WindowHookRejected);
            }
        }

        public void ReceiverResult(bool queued, int error)
        {
            lock (sync)
            {
                receiverQueued = queued;
                Record(queued ? HookEvent.ReceiverQueuedNoAck : HookEvent.ReceiverQueueRejected, 0, queued ? 0 : error);
            }
        }

        public bool ReadResult(bool addressFound, bool success, int bytesRead, int error)
        {
            lock (sync)
            {
                if (!CanRequestReader) { Record(HookEvent.PrerequisiteMissing); return false; }
                if (!addressFound) { Record(HookEvent.RecvAddressMissing); return false; }
                if (!success) { Record(HookEvent.RecvReadRejected, bytesRead, error); return false; }
                if (bytesRead != 10) { Record(HookEvent.RecvReadShort, bytesRead); return false; }
                return true;
            }
        }

        public bool ReaderQueueResult(bool queued, int error)
        {
            lock (sync)
            {
                if (!CanRequestReader) { Record(HookEvent.PrerequisiteMissing); return false; }
                if (queued) reader = ReaderObservation.RequestQueuedNoAck;
                Record(queued ? HookEvent.ReaderQueuedNoAck : HookEvent.ReaderQueueRejected, 0, queued ? 0 : error);
                return queued;
            }
        }

        public void ExceptionResult(int hresult)
        {
            lock (sync) Record(HookEvent.ExceptionObserved, 0, hresult);
        }

        public void ObserveCopyData(long tag, long senderLength, int copiedLength)
        {
            lock (sync)
            {
                // Native 107 uses dwData=0, wParam=n, cbData=n for 15<n<1000.
                // This is transport-shape evidence, not proof of origin or a complete TLBB packet.
                if (tag != 0 || senderLength != copiedLength || copiedLength <= 15 || copiedLength >= 1000)
                { Record(HookEvent.CopyDataShapeRejected, copiedLength); return; }
                if (reader != ReaderObservation.RequestQueuedNoAck && reader != ReaderObservation.DataObserved)
                { Record(HookEvent.CopyDataUnexpected, copiedLength); return; }
                reader = ReaderObservation.DataObserved;
                observedBytes += copiedLength - 4; // Four bytes are the PID prefix, not payload.
                Record(HookEvent.CopyDataObserved, copiedLength);
            }
        }

        public bool RestoreResult(bool success, int written, int error)
        {
            lock (sync)
            {
                if (!success || written != 10)
                {
                    reader = ReaderObservation.RestoreFailed;
                    Record(success ? HookEvent.RestoreShort : HookEvent.RestoreRejected, written, success ? 0 : error);
                    return false;
                }
                reader = ReaderObservation.RestoreWrittenNoAck;
                Record(HookEvent.RestoreWrittenNoAck, written);
                return true;
            }
        }

        public string Report(int processId)
        {
            lock (sync)
            {
                var text = new StringBuilder();
                text.AppendLine("PID: " + processId.ToString(CultureInfo.InvariantCulture));
                text.AppendLine("Window hook: " + windowHookAccepted);
                text.AppendLine("Receiver queued (no ACK): " + receiverQueued);
                text.AppendLine("Reader observation: " + reader);
                text.AppendLine("Observed payload bytes: " + observedBytes.ToString(CultureInfo.InvariantCulture));
                text.AppendLine("PostMessage thành công chỉ xác nhận vào hàng đợi. DLL cũ không trả ACK.");
                text.AppendLine("DataObserved chưa xác nhận nguồn gửi, framing, codec hoặc nội dung TLBB.");
                text.AppendLine("RestoreWrittenNoAck chỉ xác nhận API đã ghi 10 byte; chưa xác nhận lifecycle native.");
                foreach (HookEvent code in Enum.GetValues(typeof(HookEvent)))
                {
                    long count;
                    if (counts.TryGetValue(code, out count)) text.AppendLine(code + " count=" + count.ToString(CultureInfo.InvariantCulture));
                }
                text.AppendLine("Recent observations (UTC, max " + HistoryLimit + "):");
                foreach (string row in history) text.AppendLine(row);
                return text.ToString();
            }
        }
    }
}
