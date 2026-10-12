using System;
using System.Runtime.InteropServices;
using TinhKiemAuto.Hooks;

namespace TinhKiemAuto
{
    public partial class Game
    {
        private readonly HookDiagnostics hookDiagnostics = new HookDiagnostics();
        private readonly object receiveHookLock = new object();
        public string HookDiagnosticReport { get { return hookDiagnostics.Report(ProcessId); } }

        private void InstallWindowHookWithDiagnostics()
        {
            try { hookDiagnostics.WindowHookResult(SetHook(Handle) != IntPtr.Zero); }
            catch (Exception ex) { hookDiagnostics.WindowHookResult(false); hookDiagnostics.ExceptionResult(ex.HResult); throw; }
            // Keep the legacy Init/automation/licensing path; do not infer recv installation here.
        }

        private bool QueueHookControl(int value, int command)
        {
            int message = Global.HookMessage;
            if (Handle == IntPtr.Zero || message < 0xC000 || message > 0xFFFF) return false;
            bool queued = HookPostMessage(Handle, message, new IntPtr(value), new IntPtr(command));
            int error = queued ? 0 : Marshal.GetLastWin32Error();
            if (command == -5) hookDiagnostics.ReceiverResult(queued, error);
            else if (command == -10) return hookDiagnostics.ReaderQueueResult(queued, error);
            return queued;
        }

        private void QueueHookReceiver(int receiver)
        {
            try
            {
                if (Handle == IntPtr.Zero || Global.HookMessage < 0xC000 || Global.HookMessage > 0xFFFF)
                { hookDiagnostics.ReceiverResult(false, 0); return; }
                QueueHookControl(receiver, -5);
            }
            catch (Exception ex) { hookDiagnostics.ExceptionResult(ex.HResult); throw; }
        }

        private bool TryQueueReceiveHook()
        {
            lock (receiveHookLock)
            {
                if (IsHooked) return true; // No repeated -10 while the previous request is outstanding.
                if (!hookDiagnostics.CanRequestReader)
                { hookDiagnostics.ReadResult(false, false, 0, 0); return false; }
                try
                {
                    RecvAddress = (int)GetRemoteProcAddress(System.Diagnostics.Process.GetProcessById(ProcessId), "ws2_32.dll", "recv");
                    IntPtr bytes = IntPtr.Zero;
                    bool read = RecvAddress != 0 && HookReadMemory(Memory.Id, new IntPtr(RecvAddress), bufferRecv, new IntPtr(10), out bytes);
                    // Initialize separately for the missing-address short-circuit.
                    int count = RecvAddress == 0 ? 0 : checked((int)bytes.ToInt64());
                    int error = read || RecvAddress == 0 ? 0 : Marshal.GetLastWin32Error();
                    if (!hookDiagnostics.ReadResult(RecvAddress != 0, read, count, error)) return false;
                    bool queued = QueueHookControl(ProcessId, -10);
                    if (!queued && (Handle == IntPtr.Zero || Global.HookMessage < 0xC000 || Global.HookMessage > 0xFFFF))
                        hookDiagnostics.ReaderQueueResult(false, 0);
                    IsHooked = queued; // Legacy flag now means queued, never native ACK.
                    return queued;
                }
                catch (Exception ex) { hookDiagnostics.ExceptionResult(ex.HResult); throw; }
            }
        }

        private void RestoreReceiveHookWithDiagnostics()
        {
            lock (receiveHookLock)
            {
                if (!IsHooked) return;
                try
                {
                    IntPtr bytes = IntPtr.Zero;
                    bool restored = RecvAddress != 0 && HookWriteMemory(Memory.Id, new IntPtr(RecvAddress), bufferRecv, new IntPtr(10), out bytes);
                    int error = restored || RecvAddress == 0 ? 0 : Marshal.GetLastWin32Error();
                    if (hookDiagnostics.RestoreResult(restored, checked((int)bytes.ToInt64()), error)) IsHooked = false;
                    // A rejected/partial write keeps IsHooked true, preventing another -10 request.
                }
                catch (Exception ex) { hookDiagnostics.ExceptionResult(ex.HResult); hookDiagnostics.RestoreResult(false, 0, 0); throw; }
            }
        }

        public void ObserveHookCopyData(long tag, long senderLength, int copiedLength)
        { hookDiagnostics.ObserveCopyData(tag, senderLength, copiedLength); }

        [DllImport("user32.dll", EntryPoint = "PostMessageW", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool HookPostMessage(IntPtr window, int message, IntPtr value, IntPtr command);

        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool HookReadMemory(IntPtr process, IntPtr address, [Out] byte[] buffer, IntPtr size, out IntPtr read);

        [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool HookWriteMemory(IntPtr process, IntPtr address, byte[] buffer, IntPtr size, out IntPtr written);
    }
}
