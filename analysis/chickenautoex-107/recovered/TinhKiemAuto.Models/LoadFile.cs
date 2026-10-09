using System.IO;
using System.Text;

namespace TinhKiemAuto.Models
{
	public static class LoadFile
	{
		private static byte[] DecryptABFile(string fileName, string password)
		{
			byte[] data = File.ReadAllBytes(fileName);
			string mD5Hash = MD5Util.GetMD5Hash(password);
			return XXTEA.Decrypt(data, mD5Hash);
		}

		private static byte[] EncryptABFile(byte[] data, string password)
		{
			string mD5Hash = MD5Util.GetMD5Hash(password);
			return XXTEA.Encrypt(data, mD5Hash);
		}

		public static void WriteFileWithEncrypt(string data, string filename)
		{
			string password = "[REDACTED]" /* analysis redaction */;
			try
			{
				byte[] bytes = EncryptABFile(Encoding.UTF8.GetBytes(data), password);
				File.WriteAllBytes(filename, bytes);
			}
			catch
			{
			}
		}

		public static string LoadFileWithDecrypt(string filename)
		{
			string result = "";
			try
			{
				string password = "[REDACTED]" /* analysis redaction */;
				byte[] bytes = DecryptABFile(filename, password);
				result = Encoding.UTF8.GetString(bytes);
			}
			catch
			{
			}
			return result;
		}
	}
}
