using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace TinhKiemAuto
{
	public class Memory
	{
		public enum Protection
		{
			PAGE_NOACCESS = 1,
			PAGE_READONLY = 2,
			PAGE_READWRITE = 4,
			PAGE_WRITECOPY = 8,
			PAGE_EXECUTE = 0x10,
			PAGE_EXECUTE_READ = 0x20,
			PAGE_EXECUTE_READWRITE = 0x40,
			PAGE_EXECUTE_WRITECOPY = 0x80,
			PAGE_GUARD = 0x100,
			PAGE_NOCACHE = 0x200,
			PAGE_WRITECOMBINE = 0x400
		}

		private IntPtr id;

		public int BytesCount;

		public static int BufferSize = 20248;

		public static int Decommit = 16384;

		public static int Release = 32768;

		public static int MEM_RESERVE = 8192;

		public static int MEM_COMMIT = 4096;

		public static int PAGE_READWRITE = 4;

		public IntPtr Id => id;

		public int ProcessID { get; set; }

		public Memory(int processId)
		{
			ProcessID = processId;
			id = OpenProcess(2035711, bInheritHandle: false, processId);
		}

		public static int Char2Byte(char c)
		{
			return c;
		}

		public static int Hex2Int(string hex)
		{
			if (hex == "??")
			{
				return -1;
			}
			int result = -1;
			int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
			return result;
		}

		public static byte Hex2Byte(string hex)
		{
			byte result = 0;
			byte.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
			return result;
		}

		public static byte[] Hex2ByteArr(string hex)
		{
			byte[] array = new byte[hex.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Hex2Byte(hex.Substring(i * 2, 2));
			}
			return array;
		}

		public int WriteHex(string hex)
		{
			byte[] array = Hex2ByteArr(hex.Replace(" ", ""));
			int num = VirtualAllocEx(id, 0, array.Length + 16, 4096u, 64u);
			WriteProcessMemory(id, num, array, array.Length, 0);
			return num;
		}

		public int GetModuleAddress(string moduleName)
		{
			Process processById = Process.GetProcessById(ProcessID);
			for (int i = 0; i < processById.Modules.Count; i++)
			{
				if (processById.Modules[i].ModuleName.ToLower() == moduleName.ToLower())
				{
					return (int)processById.Modules[i].BaseAddress;
				}
			}
			return 0;
		}

		public string ReverseString(string input)
		{
			string text = "";
			if (input.Length % 2 != 0)
			{
				input = "0" + input;
			}
			for (int num = input.Length / 2 - 1; num >= 0; num--)
			{
				text += input.Substring(num * 2, 2);
			}
			return text;
		}

		public static bool Compare(byte[] input, int[] pattern, int index)
		{
			for (int i = 0; i < pattern.Length; i++)
			{
				if (pattern[i] == -1)
				{
					continue;
				}
				if (pattern[i] == 257)
				{
					if (input[index + i] == 0)
					{
						return false;
					}
				}
				else if (input[index + i] != pattern[i])
				{
					return false;
				}
			}
			return true;
		}

		public int Scan(string hex, string moduleName)
		{
			Process processById = Process.GetProcessById(ProcessID);
			for (int i = 0; i < processById.Modules.Count; i++)
			{
				if (processById.Modules[i].ModuleName.ToLower() == moduleName.ToLower())
				{
					return Scan(hex, (int)processById.Modules[i].BaseAddress, (int)processById.Modules[i].BaseAddress + processById.Modules[i].ModuleMemorySize, 0);
				}
			}
			return 0;
		}

		public int Scan(string hex, string moduleName, int index)
		{
			Process processById = Process.GetProcessById(ProcessID);
			for (int i = 0; i < processById.Modules.Count; i++)
			{
				if (processById.Modules[i].ModuleName.ToLower() == moduleName.ToLower())
				{
					return Scan(hex, (int)processById.Modules[i].BaseAddress, (int)processById.Modules[i].BaseAddress + processById.Modules[i].ModuleMemorySize, index);
				}
			}
			return 0;
		}

		public int Scan(string hex)
		{
			return Scan(hex, -1, -1, 0);
		}

		public int Scan(string hex, int index)
		{
			return Scan(hex, -1, -1, index);
		}

		public int ScanString(string s, int startAdd, int endAdd, int index)
		{
			return Scan(ConverterEx.String2Hex(s), startAdd, endAdd, index);
		}

		public int ScanString(string s)
		{
			return Scan(ConverterEx.String2Hex(s));
		}

		public int Scan(string hex, int startAddress, int endAddress, int index)
		{
			int num = 0;
			int result = 0;
			try
			{
				if (startAddress == -1)
				{
					startAddress = (int)Process.GetProcessById(ProcessID).MainModule.BaseAddress;
				}
				if (endAddress == -1)
				{
					endAddress = startAddress + Process.GetProcessById(ProcessID).MainModule.ModuleMemorySize;
				}
			}
			catch
			{
				startAddress = 262144;
				endAddress = 8978431;
			}
			if (startAddress == 0 && endAddress == 0)
			{
				startAddress = 0;
				endAddress = int.MaxValue;
			}
			int[] array = ConverterEx.Hex2IntArr(hex);
			byte[] array2 = new byte[BufferSize + array.Length];
			int num2 = (endAddress - startAddress) / BufferSize;
			int num3 = (endAddress - startAddress) % BufferSize;
			for (int i = 0; i < num2; i++)
			{
				ReadProcessMemory(Id, startAddress + i * BufferSize, array2, array2.Length, out BytesCount);
				for (int j = 0; j < BufferSize; j++)
				{
					if (Compare(array2, array, j))
					{
						int num4 = j + i * BufferSize + startAddress;
						if (num++ >= index)
						{
							return num4;
						}
						result = num4;
					}
				}
			}
			if (num3 > 0)
			{
				array2 = new byte[num3 + array.Length];
				ReadProcessMemory(Id, startAddress + num2 * BufferSize, array2, array2.Length, 0);
				for (int k = 0; k < num3; k++)
				{
					if (Compare(array2, array, k))
					{
						int num5 = k + startAddress + num2 * BufferSize;
						if (num++ >= index)
						{
							return num5;
						}
						result = num5;
					}
				}
			}
			return result;
		}

		public int[] ToArr(int address, int offset)
		{
			return new int[2] { address, offset };
		}

		public int[] ToArr(int address, int[] offset)
		{
			int[] array = new int[offset.Length + 1];
			array[0] = address;
			offset.CopyTo(array, 1);
			return array;
		}

		public int Read(int address)
		{
			byte[] array = new byte[4];
			ReadProcessMemory(id, address, array, 4, 0);
			return BitConverter.ToInt32(array, 0);
		}

		public int Read1Byte(int address)
		{
			byte[] array = new byte[4];
			ReadProcessMemory(id, address, array, 1, 0);
			return BitConverter.ToInt32(array, 0);
		}

		public int Read1Byte(int address, int offset)
		{
			address = ReadAddress(address, offset);
			byte[] array = new byte[4];
			ReadProcessMemory(id, address, array, 1, 0);
			return BitConverter.ToInt32(array, 0);
		}

		public int Read2Byte(int address)
		{
			byte[] array = new byte[4];
			ReadProcessMemory(id, address, array, 2, 0);
			return BitConverter.ToInt32(array, 0);
		}

		public int Read2Byte(int address, int offset)
		{
			address = ReadAddress(address, offset);
			byte[] array = new byte[4];
			ReadProcessMemory(id, address, array, 2, 0);
			return BitConverter.ToInt32(array, 0);
		}

		public ulong Read8Byte(int address)
		{
			byte[] array = new byte[8];
			ReadProcessMemory(id, address, array, 8, 0);
			return BitConverter.ToUInt64(array, 0);
		}

		public ulong Read8Byte(int[] offsets)
		{
			byte[] array = new byte[8];
			int lpBaseAddress = ReadAddress(offsets);
			ReadProcessMemory(id, lpBaseAddress, array, 8, 0);
			return BitConverter.ToUInt64(array, 0);
		}

		public int Read(int address, int offset)
		{
			address = Read(address);
			address = Read(address + offset);
			return address;
		}

		public int Read(int address, int[] offsets)
		{
			return Read(ToArr(address, offsets));
		}

		public int ReadPointer(int address, int[] offsets)
		{
			offsets[0] = address + offsets[0];
			return Read(offsets);
		}

		public int Read(int[] pointer, int offset)
		{
			int num = Read(pointer);
			return Read(num + offset);
		}

		public int Read(int[] offsets)
		{
			int num = Read(offsets[0]);
			for (int i = 1; i < offsets.Length; i++)
			{
				num = Read(num + offsets[i]);
			}
			return num;
		}

		public bool IsRead(int[] offsets)
		{
			byte[] array = new byte[4];
			ReadProcessMemory(id, offsets[0], array, 4, out var lpNumberOfBytesRead);
			int num = BitConverter.ToInt32(array, 0);
			if (lpNumberOfBytesRead == 0)
			{
				return false;
			}
			for (int i = 1; i < offsets.Length - 1; i++)
			{
				lpNumberOfBytesRead = 0;
				ReadProcessMemory(id, num + offsets[i], array, 4, out lpNumberOfBytesRead);
				if (lpNumberOfBytesRead == 0)
				{
					return false;
				}
				num = BitConverter.ToInt32(array, 0);
			}
			if (offsets.Length > 1)
			{
				lpNumberOfBytesRead = 0;
				ReadProcessMemory(id, num + offsets[offsets.Length - 1], array, 4, out lpNumberOfBytesRead);
				if (lpNumberOfBytesRead == 0)
				{
					return false;
				}
			}
			return true;
		}

		public float ReadFloat(int address)
		{
			byte[] array = new byte[4];
			ReadProcessMemory(id, address, array, 4, 0);
			return BitConverter.ToSingle(array, 0);
		}

		public float ReadFloat(int address, int offset)
		{
			return ReadFloat(Read(address) + offset);
		}

		public float ReadFloat(int[] offsets)
		{
			return ReadFloat(ReadAddress(offsets));
		}

		public string ReadString(int address)
		{
			byte[] array = new byte[500];
			ReadProcessMemory(id, address, array, 500, 0);
			return VISCII2Unicode(array);
		}

		public string ReadStringWithLength(int address, int length)
		{
			byte[] array = new byte[length];
			ReadProcessMemory(id, address, array, length, 0);
			return VISCII2Unicode(array);
		}

		public string ReadCodeHanler(int address, int length)
		{
			int num = (int)Process.GetProcessById(ProcessID).MainModule.BaseAddress;
			byte[] array = new byte[length];
			ReadProcessMemory(id, num + address, array, length, 0);
			return Encoding.Default.GetString(array);
		}

		public string ReadShortString(int address)
		{
			byte[] array = new byte[60];
			ReadProcessMemory(id, address, array, 60, 0);
			return VISCII2Unicode(array);
		}

		public string ReadStringEx(int address)
		{
			byte[] array = new byte[20248];
			ReadProcessMemory(id, address, array, array.Length, 0);
			return VISCII2Unicode(array);
		}

		public string ReadString(int address, int offset)
		{
			return ReadString(Read(address) + offset);
		}

		public string _ReadString(int address)
		{
			if (Read(address + 20) == 15)
			{
				return ReadShortString(address);
			}
			return ReadShortString(Read(address));
		}

		public string _ReadString(int address, int offset)
		{
			address = Read(address) + offset;
			if (Read(address + 20) == 15)
			{
				return ReadString(address);
			}
			return ReadString(Read(address));
		}

		public string _ReadString(int[] offsets)
		{
			int num = ReadAddress(offsets);
			if (Read(num + 20) == 15)
			{
				return ReadString(num);
			}
			return ReadString(Read(num));
		}

		public string ReadString(int[] offsets)
		{
			return ReadString(ReadAddress(offsets));
		}

		public void Write(int address, int value)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			WriteProcessMemory(id, address, bytes, 4, 0);
		}

		public void Write(int address, uint value, int length)
		{
			byte[] bytes = BitConverter.GetBytes(value);
			WriteProcessMemory(id, address, bytes, length, 0);
		}

		public void Write(int[] offsets, int value)
		{
			Write(ReadAddress(offsets), value);
		}

		public int ReadAddress(int address, int[] offset)
		{
			offset[0] = Read(address) + offset[0];
			return ReadAddress(offset);
		}

		public int ReadAddress(int address, int offset)
		{
			return ReadAddress(new int[2] { address, offset });
		}

		public int ReadAddress(int[] offsets)
		{
			int num = Read(offsets[0]);
			for (int i = 1; i < offsets.Length - 1; i++)
			{
				num = Read(num + offsets[i]);
			}
			if (offsets.Length == 1)
			{
				return num;
			}
			return num + offsets[offsets.Length - 1];
		}

		public static string VISCII2Unicode(string input)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in input)
			{
				if (c == '\0')
				{
					break;
				}
				stringBuilder.Append(VISCII2Unicode(c));
			}
			return stringBuilder.ToString();
		}

		public static string VISCII2Unicode(int input)
		{
			if (input < 256)
			{
				return TINHKIEM.Unicodes[input].ToString();
			}
			return ((char)input).ToString();
		}

		public static string VISCII2Unicode(byte[] input)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < input.Length; i++)
			{
				char c = (char)input[i];
				if (c == '\0')
				{
					break;
				}
				if (c < 'Ā')
				{
					stringBuilder.Append(TINHKIEM.Unicodes[(uint)c]);
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public static byte[] Unicode2VISCII(byte[] input)
		{
			for (int i = 0; i < input.Length; i++)
			{
				input[i] = Unicode2VISCII(input[i]);
			}
			return input;
		}

		public static byte Unicode2VISCII(int input)
		{
			for (int i = 0; i < 256; i++)
			{
				if (TINHKIEM.Unicodes[i] == input)
				{
					return (byte)i;
				}
			}
			return 63;
		}

		public static int Float2Int(float value)
		{
			return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
		}

		public int VirtualAllocEx(int length)
		{
			return VirtualAllocEx(id, 0, length + 16, 4096u, 64u);
		}

		public int WriteString(string str)
		{
			byte[] bytes = Encoding.Default.GetBytes(str);
			int num = VirtualAllocEx(id, 0, bytes.Length + 16, 4096u, 64u);
			WriteProcessMemory(id, num, bytes, bytes.Length, 0);
			return num;
		}

		public int WriteString(string str, int address)
		{
			byte[] bytes = Encoding.Default.GetBytes(str);
			WriteProcessMemory(id, address, bytes, bytes.Length, 0);
			return address;
		}

		public int WriteUnicodeString(string str, int address)
		{
			str = ConverterEx.Unicode2VISCII(str);
			byte[] bytes = Encoding.Default.GetBytes(str);
			WriteProcessMemory(id, address, bytes, bytes.Length, 0);
			return address;
		}

		public int WriteUnicodeString(string str)
		{
			byte[] array = new byte[4];
			str = ConverterEx.Unicode2VISCII(str);
			array = Encoding.Default.GetBytes(str);
			int num = VirtualAllocEx(id, 0, array.Length + 16, 4096u, 64u);
			WriteProcessMemory(id, num, array, array.Length, 0);
			return num;
		}

		private static byte[] GetBytes(string str)
		{
			byte[] array = new byte[str.Length * 2];
			Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
			return array;
		}

		public void FreeMem(int address, int size)
		{
			VirtualFreeEx(Id, address, size, 32768);
		}

		public void TrimMem()
		{
			SetProcessWorkingSetSize(Process.GetProcessById(ProcessID).Handle, -1, -1);
		}

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll")]
		public static extern bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, int lpNumberOfBytesRead);

		[DllImport("kernel32.dll")]
		public static extern bool ReadProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

		[DllImport("kernel32.dll")]
		public static extern bool WriteProcessMemory(IntPtr hProcess, int lpBaseAddress, byte[] lpBuffer, int nSize, int lpNumberOfBytesWritten);

		[DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern int VirtualAllocEx(IntPtr hProcess, int lpAddress, int dwSize, uint flAllocationType, uint flProtect);

		[DllImport("kernel32.dll")]
		public static extern bool VirtualFreeEx(IntPtr hProcess, int lpAddress, int dwSize, int dwFreeType);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool VirtualProtect(int lpAddress, uint dwSize, uint flNewProtect, int lpflOldProtect);

		[DllImport("kernel32.dll")]
		private static extern bool SetProcessWorkingSetSize(IntPtr hProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

		[DllImport("kernel32.dll")]
		public static extern bool FlushInstructionCache(int hProcess, int lpBaseAddress, int dwSize);
	}
}
