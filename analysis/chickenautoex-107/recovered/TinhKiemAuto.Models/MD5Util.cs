using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TinhKiemAuto.Models
{
	public class MD5Util
	{
		public static string GetFileMD5(string fileName)
		{
			string result = string.Empty;
			if (File.Exists(fileName))
			{
				using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					result = GetMD5Hash(stream);
				}
			}
			return result;
		}

		public static string GetMD5Hash(Stream stream)
		{
			byte[] array = MD5.Create().ComputeHash(stream);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		public static string GetMD5Hash(string input)
		{
			byte[] array = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		public static bool VerfyMd5Hash(string input, string hash)
		{
			string mD5Hash = GetMD5Hash(input);
			StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
			return ordinalIgnoreCase.Compare(mD5Hash, hash) == 0;
		}
	}
}
