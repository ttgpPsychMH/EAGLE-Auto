using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using ComponentAce.Compression.Libs.zlib;
using ProtoBuf;

namespace TinhKiemAuto
{
	public class DataHelper
	{
		public static int MinZipBytesSize = 256;

		public static List<string> LegalCopyrightKILL = new List<string>();

		public static List<string> OriginalFilename = new List<string>();

		public static List<string> FileDescription = new List<string>();

		public static List<string> MD5Prosecc = new List<string>();

		public static List<string> CompanyName = new List<string>();

		public static List<string> FileName = new List<string>();

		public static List<string> Hex = new List<string>();

		public static List<string> ModunName = new List<string>();

		public static List<string> Title = new List<string>();

		public static byte[] ObjectToBytes<T>(T instance)
		{
			try
			{
				byte[] array;
				if (instance == null)
				{
					array = new byte[0];
				}
				else
				{
					MemoryStream memoryStream = new MemoryStream();
					Serializer.Serialize(memoryStream, instance);
					array = new byte[memoryStream.Length];
					memoryStream.Position = 0L;
					memoryStream.Read(array, 0, array.Length);
					memoryStream.Dispose();
				}
				if (array.Length > MinZipBytesSize)
				{
					byte[] array2 = Compress(array);
					if (array2 != null && array2.Length < array.Length)
					{
						array = array2;
					}
				}
				return array;
			}
			catch (Exception)
			{
			}
			return new byte[0];
		}

		public static string smethod_3(string string_0)
		{
			string text = "";
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			try
			{
				if (File.Exists(string_0))
				{
					FileStream fileStream = new FileStream(string_0, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					byte[] array = mD5CryptoServiceProvider.ComputeHash(fileStream);
					fileStream.Close();
					text = BitConverter.ToString(array).Replace("-", "");
				}
				else
				{
					MD5 mD = MD5.Create();
					byte[] bytes = Encoding.ASCII.GetBytes(string_0);
					byte[] array2 = mD.ComputeHash(bytes);
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < array2.Length; i++)
					{
						stringBuilder.Append(array2[i].ToString("X2"));
					}
					text = stringBuilder.ToString();
				}
			}
			catch
			{
				MD5 mD2 = MD5.Create();
				byte[] bytes2 = Encoding.ASCII.GetBytes(string_0);
				byte[] array3 = mD2.ComputeHash(bytes2);
				StringBuilder stringBuilder2 = new StringBuilder();
				for (int j = 0; j < array3.Length; j++)
				{
					stringBuilder2.Append(array3[j].ToString("X2"));
				}
				text = stringBuilder2.ToString();
			}
			return text.ToUpper();
		}

		public static byte[] StrToByteArray(string str)
		{
			Dictionary<string, byte> dictionary = new Dictionary<string, byte>();
			for (int i = 0; i <= 255; i++)
			{
				dictionary.Add(i.ToString("X2"), (byte)i);
			}
			List<byte> list = new List<byte>();
			for (int j = 0; j < str.Length; j += 2)
			{
				list.Add(dictionary[str.Substring(j, 2)]);
			}
			return list.ToArray();
		}

		public static bool Contains(byte[] self, byte[] candidate)
		{
			if (IsEmptyLocate(self, candidate))
			{
				return false;
			}
			for (int i = 0; i < self.Length; i++)
			{
				if (IsMatch(self, i, candidate))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsMatch(byte[] array, int position, byte[] candidate)
		{
			if (candidate.Length > array.Length - position)
			{
				return false;
			}
			for (int i = 0; i < candidate.Length; i++)
			{
				if (array[position + i] != candidate[i])
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsEmptyLocate(byte[] array, byte[] candidate)
		{
			if (array != null && candidate != null && array.Length != 0 && candidate.Length != 0)
			{
				return candidate.Length > array.Length;
			}
			return true;
		}

		public static string checkMD5(string filename)
		{
			using (MD5 mD = MD5.Create())
			{
				using (FileStream inputStream = File.OpenRead(filename))
				{
					return Encoding.Default.GetString(mD.ComputeHash(inputStream));
				}
			}
		}

		public static string Bytes2HexString(byte[] b)
		{
			string text = "";
			for (int i = 0; i < b.Length; i++)
			{
				text += (b[i] & 0xFF).ToString("X2").ToUpper();
			}
			return text;
		}

		public static byte[] HexString2Bytes(string s)
		{
			if (s.Length % 2 != 0)
			{
				return null;
			}
			byte[] array = new byte[s.Length / 2];
			for (int i = 0; i < s.Length / 2; i++)
			{
				int num = int.Parse(s.Substring(i * 2, 2), NumberStyles.HexNumber) & 0xFF;
				array[i] = (byte)num;
			}
			return array;
		}

		public static string GetPhysicalMemory()
		{
			long num = 0L;
			long num2 = 0L;
			try
			{
				ManagementScope scope = new ManagementScope();
				ObjectQuery query = new ObjectQuery("SELECT Capacity FROM Win32_PhysicalMemory");
				foreach (ManagementObject item in new ManagementObjectSearcher(scope, query).Get())
				{
					num2 = Convert.ToInt64(item["Capacity"]);
					num += num2;
				}
				num = num / 1024 / 1024;
			}
			catch
			{
			}
			return num + "MB";
		}

		public static string GetAccountName()
		{
			try
			{
				foreach (ManagementObject item in new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_UserAccount").Get())
				{
					try
					{
						return item.GetPropertyValue("Name").ToString();
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
			return "User Account Name: Unknown";
		}

		public static string GetOSInformation()
		{
			try
			{
				foreach (ManagementObject item in new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get())
				{
					try
					{
						return ((string)item["Caption"]).Trim() + ", " + (string)item["Version"] + ", " + (string)item["OSArchitecture"];
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
			return "BIOS Maker: Unknown";
		}

		public static string GetProcessorInformation()
		{
			string result = string.Empty;
			try
			{
				foreach (ManagementObject instance in new ManagementClass("win32_processor").GetInstances())
				{
					string text = (string)instance["Name"];
					text = text.Replace("(TM)", "™").Replace("(tm)", "™").Replace("(R)", "®")
						.Replace("(r)", "®")
						.Replace("(C)", "©")
						.Replace("(c)", "©")
						.Replace("    ", " ")
						.Replace("  ", " ");
					result = text + ", " + (string)instance["Caption"] + ", " + (string)instance["SocketDesignation"];
				}
			}
			catch
			{
			}
			return result;
		}

		public static long ConvertToTicks(string str)
		{
			try
			{
				if (!DateTime.TryParse(str, out var result))
				{
					return 0L;
				}
				return result.Ticks / 10000;
			}
			catch (Exception)
			{
			}
			return 0L;
		}

		public static byte[] GIAIMA(byte[] data, string password)
		{
			string mD5Hash = MD5Util.GetMD5Hash(password);
			return XXTEA.Decrypt(data, mD5Hash);
		}

		public static byte[] MAHOA(byte[] data, string password)
		{
			string mD5Hash = MD5Util.GetMD5Hash(password);
			return XXTEA.Encrypt(data, mD5Hash);
		}

		public static byte[] Compress(byte[] bytes)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ZOutputStream zOutputStream = new ZOutputStream(memoryStream, -1))
				{
					zOutputStream.Write(bytes, 0, bytes.Length);
					zOutputStream.Flush();
				}
				return memoryStream.ToArray();
			}
		}

		public static T BytesToObject<T>(byte[] bytesData, int offset, int length)
		{
			if (bytesData.Length == 0)
			{
				return default(T);
			}
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					if (bytesData.Length - offset < 2 || 120 != bytesData[offset] || (156 != bytesData[offset + 1] && 218 != bytesData[offset + 1]))
					{
						memoryStream.Write(bytesData, offset, length);
						memoryStream.Position = 0L;
						return Serializer.Deserialize<T>(memoryStream);
					}
					using (ZOutputStream zOutputStream = new ZOutputStream(memoryStream))
					{
						zOutputStream.Write(bytesData, offset, length);
						zOutputStream.Flush();
						memoryStream.Position = 0L;
						return Serializer.Deserialize<T>(memoryStream);
					}
				}
			}
			catch (Exception)
			{
			}
			return default(T);
		}
	}
}
