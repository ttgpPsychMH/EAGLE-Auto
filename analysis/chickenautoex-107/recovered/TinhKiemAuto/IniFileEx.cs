using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace TinhKiemAuto
{
	internal class IniFileEx
	{
		public static int capacity = 512;

		[DllImport("kernel32", CharSet = CharSet.Unicode)]
		private static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder value, int size, string filePath);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		private static extern int GetPrivateProfileString(string section, string key, string defaultValue, [In][Out] char[] value, int size, string filePath);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern int GetPrivateProfileSection(string section, IntPtr keyValue, int size, string filePath);

		[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool WritePrivateProfileString(string section, string key, string value, string filePath);

		public static string ReadValue(string section, string key, string filePath, string defaultValue = "")
		{
			StringBuilder stringBuilder = new StringBuilder(capacity);
			GetPrivateProfileString(section, key, defaultValue, stringBuilder, stringBuilder.Capacity, filePath);
			return stringBuilder.ToString();
		}

		public static string[] ReadSections(string filePath)
		{
			char[] value;
			int privateProfileString;
			while (true)
			{
				value = new char[capacity];
				privateProfileString = GetPrivateProfileString(null, null, "", value, capacity, filePath);
				if (privateProfileString != 0)
				{
					if (privateProfileString < capacity - 2)
					{
						break;
					}
					capacity *= 2;
					continue;
				}
				return null;
			}
			return new string(value, 0, privateProfileString).Split(new char[1], StringSplitOptions.RemoveEmptyEntries);
		}

		public static string[] ReadKeys(string section, string filePath)
		{
			char[] value;
			int privateProfileString;
			while (true)
			{
				value = new char[capacity];
				privateProfileString = GetPrivateProfileString(section, null, "", value, capacity, filePath);
				if (privateProfileString != 0)
				{
					if (privateProfileString < capacity - 2)
					{
						break;
					}
					capacity *= 2;
					continue;
				}
				return null;
			}
			return new string(value, 0, privateProfileString).Split(new char[1], StringSplitOptions.RemoveEmptyEntries);
		}

		public static string[] ReadKeyValuePairs(string section, string filePath)
		{
			IntPtr intPtr;
			int privateProfileSection;
			while (true)
			{
				intPtr = Marshal.AllocCoTaskMem(capacity * 2);
				privateProfileSection = GetPrivateProfileSection(section, intPtr, capacity, filePath);
				if (privateProfileSection != 0)
				{
					if (privateProfileSection < capacity - 2)
					{
						break;
					}
					Marshal.FreeCoTaskMem(intPtr);
					capacity *= 2;
					continue;
				}
				Marshal.FreeCoTaskMem(intPtr);
				return null;
			}
			string text = Marshal.PtrToStringAuto(intPtr, privateProfileSection - 1);
			Marshal.FreeCoTaskMem(intPtr);
			return text.Split(default(char));
		}

		public static bool WriteValue(string section, string key, string value, string filePath)
		{
			if (!File.Exists(filePath))
			{
				TINHKIEM.CreateFile(filePath, "");
			}
			return WritePrivateProfileString(section, key, value, filePath);
		}

		public static bool DeleteSection(string section, string filepath)
		{
			return WritePrivateProfileString(section, null, null, filepath);
		}

		public static bool DeleteKey(string section, string key, string filepath)
		{
			return WritePrivateProfileString(section, key, null, filepath);
		}
	}
}
