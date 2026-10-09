using System;
using System.Runtime.InteropServices;
using System.Text;

namespace TinhKiemAuto
{
	public static class WinAPI
	{
		public struct CONTEXT
		{
			public uint ContextFlags;

			public uint Dr0;

			public uint Dr1;

			public uint Dr2;

			public uint Dr3;

			public uint Dr6;

			public uint Dr7;

			public FLOATING_SAVE_AREA FloatSave;

			public uint SegGs;

			public uint SegFs;

			public uint SegEs;

			public uint SegDs;

			public uint Edi;

			public uint Esi;

			public uint Ebx;

			public uint Edx;

			public uint Ecx;

			public uint Eax;

			public uint Ebp;

			public uint Eip;

			public uint SegCs;

			public uint EFlags;

			public uint Esp;

			public uint SegSs;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
			public byte[] ExtendedRegisters;
		}

		public struct FLOATING_SAVE_AREA
		{
			public uint ControlWord;

			public uint StatusWord;

			public uint TagWord;

			public uint ErrorOffset;

			public uint ErrorSelector;

			public uint DataOffset;

			public uint DataSelector;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
			public byte[] RegisterArea;

			public uint Cr0NpxState;
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CloseHandle(IntPtr handle);

		public static IntPtr CreateRemotePointer(IntPtr hProcess, byte[] pData, int flProtect)
		{
			IntPtr intPtr = IntPtr.Zero;
			if (pData != null && hProcess != IntPtr.Zero)
			{
				intPtr = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)pData.Length, 12288, flProtect);
				uint lpNumberOfBytesRead = 0u;
				if (intPtr != IntPtr.Zero && WriteProcessMemory(hProcess, intPtr, pData, pData.Length, out lpNumberOfBytesRead) && lpNumberOfBytesRead == pData.Length)
				{
					return intPtr;
				}
				if (intPtr != IntPtr.Zero)
				{
					VirtualFreeEx(hProcess, intPtr, 0, 32768);
					intPtr = IntPtr.Zero;
				}
			}
			return intPtr;
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr CreateRemoteThread(IntPtr hProcess, int lpThreadAttributes, int dwStackSize, IntPtr lpStartAddress, uint lpParameter, int dwCreationFlags, int lpThreadId);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetExitCodeThread(IntPtr hThread, out uint lpExitCode);

		public static uint GetLastErrorEx(IntPtr hProcess)
		{
			IntPtr procAddress = GetProcAddress(GetModuleHandleA("kernel32.dll"), "GetLastError");
			return RunThread(hProcess, procAddress, 0u);
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetModuleHandleA(string lpModuleName);

		public static IntPtr GetModuleHandleEx(IntPtr hProcess, string lpModuleName)
		{
			IntPtr procAddress = GetProcAddress(GetModuleHandleA("kernel32.dll"), "GetModuleHandleW");
			IntPtr result = IntPtr.Zero;
			if (!procAddress.IsNull())
			{
				IntPtr intPtr = CreateRemotePointer(hProcess, Encoding.Unicode.GetBytes(lpModuleName + "\0"), 4);
				if (!intPtr.IsNull())
				{
					result = Win32Ptr.Create(RunThread(hProcess, procAddress, (uint)intPtr.ToInt32()));
					VirtualFreeEx(hProcess, intPtr, 0, 32768);
				}
			}
			return result;
		}

		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(string dllToLoad);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, uint lpProcName);

		public static IntPtr GetProcAddressEx(IntPtr hProc, IntPtr hModule, object lpProcName)
		{
			IntPtr result = IntPtr.Zero;
			byte[] array = ReadRemoteMemory(hProc, hModule, 64u);
			if (array == null || BitConverter.ToUInt16(array, 0) != 23117)
			{
				return result;
			}
			uint num = BitConverter.ToUInt32(array, 60);
			if (num == 0)
			{
				return result;
			}
			byte[] array2 = ReadRemoteMemory(hProc, hModule.Add(num), 264u);
			if (array2 == null || BitConverter.ToUInt32(array2, 0) != 17744)
			{
				return result;
			}
			uint num2 = BitConverter.ToUInt32(array2, 120);
			uint num3 = BitConverter.ToUInt32(array2, 124);
			if (num2 == 0 || num3 == 0)
			{
				return result;
			}
			byte[] array3 = ReadRemoteMemory(hProc, hModule.Add(num2), 40u);
			uint num4 = BitConverter.ToUInt32(array3, 28);
			uint num5 = BitConverter.ToUInt32(array3, 36);
			uint num6 = BitConverter.ToUInt32(array3, 20);
			int num7 = -1;
			if (num4 == 0 || num5 == 0)
			{
				return result;
			}
			if (lpProcName.GetType().Equals(typeof(string)))
			{
				int num8 = SearchExports(hProc, hModule, array3, (string)lpProcName);
				if (num8 > -1)
				{
					byte[] array4 = ReadRemoteMemory(hProc, hModule.Add(num5 + ((long)num8 << 1)), 2u);
					num7 = ((array4 == null) ? (-1) : BitConverter.ToUInt16(array4, 0));
				}
			}
			else if (lpProcName.GetType().Equals(typeof(short)) || lpProcName.GetType().Equals(typeof(ushort)))
			{
				num7 = int.Parse(lpProcName.ToString());
			}
			if (num7 <= -1 || num7 >= num6)
			{
				return result;
			}
			byte[] array5 = ReadRemoteMemory(hProc, hModule.Add(num4 + ((long)num7 << 2)), 4u);
			if (array5 == null)
			{
				return result;
			}
			uint num9 = BitConverter.ToUInt32(array5, 0);
			if (num9 >= num2 && num9 < num2 + num3)
			{
				string text = ReadRemoteString(hProc, hModule.Add(num9));
				if (!string.IsNullOrEmpty(text) && text.Contains("."))
				{
					result = GetProcAddressEx(hProc, GetModuleHandleEx(hProc, text.Split('.')[0]), text.Split('.')[1]);
				}
				return result;
			}
			return hModule.Add(num9);
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern int GetProcessId(IntPtr hProcess);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetThreadContext(IntPtr hThread, ref CONTEXT pContext);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, int dwThreadId);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out uint lpNumberOfBytesRead);

		public static byte[] ReadRemoteMemory(IntPtr hProc, IntPtr address, uint len)
		{
			byte[] array = new byte[len];
			uint lpNumberOfBytesRead = 0u;
			if (!ReadProcessMemory(hProc, address, array, array.Length, out lpNumberOfBytesRead) || lpNumberOfBytesRead != len)
			{
				array = null;
			}
			return array;
		}

		public static IntPtr ReadRemotePointer(IntPtr hProcess, IntPtr pData)
		{
			IntPtr result = IntPtr.Zero;
			if (!hProcess.IsNull() && !pData.IsNull())
			{
				byte[] array = ReadRemoteMemory(hProcess, pData, (uint)IntPtr.Size);
				if (array != null)
				{
					result = new IntPtr(BitConverter.ToInt32(array, 0));
				}
			}
			return result;
		}

		public static string ReadRemoteString(IntPtr hProcess, IntPtr lpAddress, Encoding encoding = null)
		{
			if (encoding == null)
			{
				encoding = Encoding.ASCII;
			}
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array = new byte[256];
			uint lpNumberOfBytesRead = 0u;
			int num = -1;
			while (num < 0 && ReadProcessMemory(hProcess, lpAddress, array, array.Length, out lpNumberOfBytesRead) && lpNumberOfBytesRead != 0)
			{
				lpAddress = lpAddress.Add(lpNumberOfBytesRead);
				int length = stringBuilder.Length;
				stringBuilder.Append(encoding.GetString(array, 0, (int)lpNumberOfBytesRead));
				num = stringBuilder.ToString().IndexOf('\0', length);
			}
			return stringBuilder.ToString().Substring(0, num);
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern uint ResumeThread(IntPtr hThread);

		public static uint RunThread(IntPtr hProcess, IntPtr lpStartAddress, uint lpParam, int timeout = 1000)
		{
			uint lpExitCode = uint.MaxValue;
			IntPtr intPtr = CreateRemoteThread(hProcess, 0, 0, lpStartAddress, lpParam, 0, 0);
			if (intPtr != IntPtr.Zero && (long)WaitForSingleObject(intPtr, timeout) == 0L)
			{
				GetExitCodeThread(intPtr, out lpExitCode);
			}
			return lpExitCode;
		}

		private static int SearchExports(IntPtr hProcess, IntPtr hModule, byte[] exports, string name)
		{
			uint num = BitConverter.ToUInt32(exports, 24);
			uint num2 = BitConverter.ToUInt32(exports, 32);
			int num3 = -1;
			if (num != 0 && num2 != 0)
			{
				byte[] array = ReadRemoteMemory(hProcess, hModule.Add(num2), num << 2);
				if (array == null)
				{
					return num3;
				}
				uint[] array2 = new uint[num];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = BitConverter.ToUInt32(array, i << 2);
				}
				int num4 = 0;
				int num5 = array2.Length - 1;
				string empty = string.Empty;
				while (num4 >= 0 && num4 <= num5 && num3 == -1)
				{
					int num6 = (num4 + num5) / 2;
					empty = ReadRemoteString(hProcess, hModule.Add(array2[num6]));
					if (empty.Equals(name))
					{
						num3 = num6;
					}
					else if (string.CompareOrdinal(empty, name) < 0)
					{
						num4 = num6 - 1;
					}
					else
					{
						num5 = num6 + 1;
					}
				}
			}
			return num3;
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetThreadContext(IntPtr hThread, ref CONTEXT pContext);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern uint SuspendThread(IntPtr hThread);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, int flAllocationType, int flProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, int dwFreeType);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint flOldProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern uint WaitForSingleObject(IntPtr hObject, int dwTimeout);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpAddress, byte[] lpBuffer, int dwSize, out uint lpNumberOfBytesRead);
	}
}
