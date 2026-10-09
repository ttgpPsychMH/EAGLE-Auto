using System;
using System.Diagnostics;
using System.Threading;

namespace TinhKiemAuto
{
	internal class ThreadHijack : StandardInjectionMethod
	{
		private static readonly byte[] REDIRECT_STUB = new byte[14]
		{
			156, 96, 232, 0, 0, 0, 0, 97, 157, 233,
			0, 0, 0, 0
		};

		public override IntPtr Inject(string dllPath, IntPtr hProcess)
		{
			ClearErrors();
			IntPtr[] array = InjectAll(new string[1] { dllPath }, hProcess);
			if (array != null && array[0].IsNull() && GetLastError() == null)
			{
				SetLastError(new Exception("Module's entry point function reported a failure"));
			}
			if (array == null || array.Length == 0)
			{
				return IntPtr.Zero;
			}
			return array[0];
		}

		public override IntPtr[] InjectAll(string[] dllPaths, IntPtr hProcess)
		{
			ClearErrors();
			try
			{
				if (hProcess.IsNull() || hProcess.Compare(-1L))
				{
					throw new ArgumentException("Invalid process handle.", "hProcess");
				}
				int processId = WinAPI.GetProcessId(hProcess);
				if (processId == 0)
				{
					throw new ArgumentException("Provided handle doesn't have sufficient permissions to inject", "hProcess");
				}
				Process processById = Process.GetProcessById(processId);
				if (processById.Threads.Count == 0)
				{
					throw new Exception("Target process has no targetable threads to hijack.");
				}
				ProcessThread processThread = SelectOptimalThread(processById);
				IntPtr intPtr = WinAPI.OpenThread(26u, bInheritHandle: false, processThread.Id);
				if (intPtr.IsNull() || intPtr.Compare(-1L))
				{
					throw new Exception("Unable to obtain a handle for the remote thread.");
				}
				IntPtr pModuleBuffer = IntPtr.Zero;
				IntPtr zero = IntPtr.Zero;
				IntPtr intPtr2 = CreateMultiLoadStub(dllPaths, hProcess, out pModuleBuffer, 1u);
				IntPtr[] array = null;
				if (!intPtr2.IsNull())
				{
					if (WinAPI.SuspendThread(intPtr) == uint.MaxValue)
					{
						throw new Exception("Unable to suspend the remote thread");
					}
					try
					{
						uint lpNumberOfBytesRead = 0u;
						WinAPI.CONTEXT pContext = new WinAPI.CONTEXT
						{
							ContextFlags = 65537u
						};
						if (!WinAPI.GetThreadContext(intPtr, ref pContext))
						{
							throw new InvalidOperationException("Cannot get the remote thread's context");
						}
						byte[] rEDIRECT_STUB = REDIRECT_STUB;
						IntPtr intPtr3 = WinAPI.VirtualAllocEx(hProcess, IntPtr.Zero, (uint)rEDIRECT_STUB.Length, 12288, 64);
						if (intPtr3.IsNull())
						{
							throw new InvalidOperationException("Unable to allocate memory in the remote process.");
						}
						BitConverter.GetBytes(intPtr2.Subtract(intPtr3.Add(7L)).ToInt32()).CopyTo(rEDIRECT_STUB, 3);
						BitConverter.GetBytes(pContext.Eip - (uint)intPtr3.Add(rEDIRECT_STUB.Length).ToInt32()).CopyTo(rEDIRECT_STUB, rEDIRECT_STUB.Length - 4);
						if (!WinAPI.WriteProcessMemory(hProcess, intPtr3, rEDIRECT_STUB, rEDIRECT_STUB.Length, out lpNumberOfBytesRead) || lpNumberOfBytesRead != rEDIRECT_STUB.Length)
						{
							throw new InvalidOperationException("Unable to write stub to the remote process.");
						}
						pContext.Eip = (uint)intPtr3.ToInt32();
						WinAPI.SetThreadContext(intPtr, ref pContext);
					}
					catch (Exception lastError)
					{
						SetLastError(lastError);
						array = null;
						WinAPI.VirtualFreeEx(hProcess, pModuleBuffer, 0, 32768);
						WinAPI.VirtualFreeEx(hProcess, intPtr2, 0, 32768);
						WinAPI.VirtualFreeEx(hProcess, zero, 0, 32768);
					}
					WinAPI.ResumeThread(intPtr);
					if (GetLastError() == null)
					{
						Thread.Sleep(100);
						array = new IntPtr[dllPaths.Length];
						byte[] array2 = WinAPI.ReadRemoteMemory(hProcess, pModuleBuffer, (uint)(dllPaths.Length << 2));
						if (array2 != null)
						{
							for (int i = 0; i < array.Length; i++)
							{
								array[i] = Win32Ptr.Create(BitConverter.ToInt32(array2, i << 2));
							}
						}
					}
					WinAPI.CloseHandle(intPtr);
				}
				return array;
			}
			catch (Exception lastError2)
			{
				SetLastError(lastError2);
				return null;
			}
		}

		private static ProcessThread SelectOptimalThread(Process target)
		{
			return target.Threads[0];
		}
	}
}
