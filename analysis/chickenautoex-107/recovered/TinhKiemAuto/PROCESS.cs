using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TinhKiemAuto
{
	internal class PROCESS
	{
		[Flags]
		public enum ThreadAccess
		{
			TERMINATE = 1,
			SUSPEND_RESUME = 2,
			GET_CONTEXT = 8,
			SET_CONTEXT = 0x10,
			SET_INFORMATION = 0x20,
			QUERY_INFORMATION = 0x40,
			SET_THREAD_TOKEN = 0x80,
			IMPERSONATE = 0x100,
			DIRECT_IMPERSONATION = 0x200
		}

		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

		[DllImport("kernel32.dll")]
		private static extern bool CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll")]
		private static extern uint SuspendThread(IntPtr hThread);

		[DllImport("kernel32.dll")]
		private static extern int ResumeThread(IntPtr hThread);

		public static void Suspend(int pid)
		{
			Process processById = Process.GetProcessById(pid);
			if (processById.ProcessName == string.Empty)
			{
				return;
			}
			foreach (ProcessThread thread in processById.Threads)
			{
				IntPtr intPtr = OpenThread(ThreadAccess.SUSPEND_RESUME, bInheritHandle: false, (uint)thread.Id);
				if (!(intPtr == IntPtr.Zero))
				{
					SuspendThread(intPtr);
					CloseHandle(intPtr);
				}
			}
		}

		public static void Resume(int pid)
		{
			Process processById = Process.GetProcessById(pid);
			if (processById.ProcessName == string.Empty)
			{
				return;
			}
			foreach (ProcessThread thread in processById.Threads)
			{
				IntPtr intPtr = OpenThread(ThreadAccess.SUSPEND_RESUME, bInheritHandle: false, (uint)thread.Id);
				if (!(intPtr == IntPtr.Zero))
				{
					int num;
					do
					{
						num = ResumeThread(intPtr);
					}
					while (num > 0);
					CloseHandle(intPtr);
				}
			}
		}
	}
}
