using System;
using System.Text;

namespace TinhKiemAuto
{
	internal class CRTInjection : StandardInjectionMethod
	{
		public override IntPtr Inject(string dllPath, IntPtr hProcess)
		{
			ClearErrors();
			if (hProcess.IsNull() || hProcess.Compare(-1L))
			{
				throw new ArgumentOutOfRangeException("hProcess", "Invalid process handle specified.");
			}
			try
			{
				IntPtr result = IntPtr.Zero;
				IntPtr procAddress = WinAPI.GetProcAddress(WinAPI.GetModuleHandleA("kernel32.dll"), "LoadLibraryW");
				if (procAddress.IsNull())
				{
					throw new Exception("Unable to locate the LoadLibraryW entry point");
				}
				IntPtr intPtr = WinAPI.CreateRemotePointer(hProcess, Encoding.Unicode.GetBytes(dllPath + "\0"), 4);
				if (intPtr.IsNull())
				{
					throw new InvalidOperationException("Failed to allocate memory in the remote process");
				}
				try
				{
					uint num = WinAPI.RunThread(hProcess, procAddress, (uint)intPtr.ToInt32(), 10000);
					switch (num)
					{
					case 0u:
						throw new Exception("Failed to load module into remote process. Error code: " + WinAPI.GetLastErrorEx(hProcess));
					case uint.MaxValue:
						throw new Exception("Error occurred when calling function in the remote process");
					default:
						result = Win32Ptr.Create(num);
						break;
					}
				}
				finally
				{
					WinAPI.VirtualFreeEx(hProcess, intPtr, 0, 32768);
				}
				return result;
			}
			catch (Exception lastError)
			{
				SetLastError(lastError);
				return IntPtr.Zero;
			}
		}

		public override IntPtr[] InjectAll(string[] dllPaths, IntPtr hProcess)
		{
			ClearErrors();
			if (hProcess.IsNull() || hProcess.Compare(-1L))
			{
				throw new ArgumentOutOfRangeException("hProcess", "Invalid process handle specified.");
			}
			try
			{
				IntPtr pModuleBuffer = IntPtr.Zero;
				IntPtr intPtr = CreateMultiLoadStub(dllPaths, hProcess, out pModuleBuffer);
				IntPtr[] array = null;
				if (!intPtr.IsNull())
				{
					try
					{
						if (WinAPI.RunThread(hProcess, intPtr, 0u, 10000) == uint.MaxValue)
						{
							throw new Exception("Error occurred while executing remote thread.");
						}
						byte[] array2 = WinAPI.ReadRemoteMemory(hProcess, pModuleBuffer, (uint)(dllPaths.Length << 2));
						if (array2 == null)
						{
							throw new InvalidOperationException("Unable to read from the remote process.");
						}
						array = new IntPtr[dllPaths.Length];
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = new IntPtr(BitConverter.ToInt32(array2, i << 2));
						}
					}
					finally
					{
						WinAPI.VirtualFreeEx(hProcess, pModuleBuffer, 0, 32768);
						WinAPI.VirtualFreeEx(hProcess, intPtr, 0, 32768);
					}
				}
				return array;
			}
			catch (Exception lastError)
			{
				SetLastError(lastError);
				return null;
			}
		}
	}
}
