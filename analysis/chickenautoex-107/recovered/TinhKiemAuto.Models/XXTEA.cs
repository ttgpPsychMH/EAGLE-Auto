using System;
using System.Text;

namespace TinhKiemAuto.Models
{
	public sealed class XXTEA
	{
		private const uint delta = 2654435769u;

		private static readonly UTF8Encoding utf8 = new UTF8Encoding();

		private XXTEA()
		{
		}

		public static byte[] Decrypt(byte[] data, byte[] key)
		{
			if (data.Length == 0)
			{
				return data;
			}
			return ToByteArray(Decrypt(ToUInt32Array(data, includeLength: false), ToUInt32Array(FixKey(key), includeLength: false)), includeLength: true);
		}

		public static byte[] Decrypt(byte[] data, string key)
		{
			return Decrypt(data, utf8.GetBytes(key));
		}

		private static uint[] Decrypt(uint[] v, uint[] k)
		{
			int num = v.Length - 1;
			if (num >= 1)
			{
				uint y = v[0];
				for (uint num2 = (uint)((ulong)(6 + 52 / (num + 1)) * 18446744072069020089uL); num2 != 0; num2 -= 2654435769u)
				{
					uint e = (num2 >> 2) & 3;
					int num3;
					uint z;
					for (num3 = num; num3 > 0; num3--)
					{
						z = v[num3 - 1];
						y = (v[num3] -= MX(num2, y, z, num3, e, k));
					}
					z = v[num];
					y = (v[0] -= MX(num2, y, z, num3, e, k));
				}
			}
			return v;
		}

		public static byte[] DecryptBase64String(string data, byte[] key)
		{
			return Decrypt(Convert.FromBase64String(data), key);
		}

		public static byte[] DecryptBase64String(string data, string key)
		{
			return Decrypt(Convert.FromBase64String(data), key);
		}

		public static string DecryptBase64StringToString(string data, byte[] key)
		{
			return utf8.GetString(DecryptBase64String(data, key));
		}

		public static string DecryptBase64StringToString(string data, string key)
		{
			return utf8.GetString(DecryptBase64String(data, key));
		}

		public static string DecryptToString(byte[] data, byte[] key)
		{
			return utf8.GetString(Decrypt(data, key));
		}

		public static string DecryptToString(byte[] data, string key)
		{
			return utf8.GetString(Decrypt(data, key));
		}

		public static byte[] Encrypt(byte[] data, byte[] key)
		{
			if (data.Length == 0)
			{
				return data;
			}
			return ToByteArray(Encrypt(ToUInt32Array(data, includeLength: true), ToUInt32Array(FixKey(key), includeLength: false)), includeLength: false);
		}

		public static byte[] Encrypt(string data, byte[] key)
		{
			return Encrypt(utf8.GetBytes(data), key);
		}

		public static byte[] Encrypt(byte[] data, string key)
		{
			return Encrypt(data, utf8.GetBytes(key));
		}

		public static byte[] Encrypt(string data, string key)
		{
			return Encrypt(utf8.GetBytes(data), utf8.GetBytes(key));
		}

		private static uint[] Encrypt(uint[] v, uint[] k)
		{
			int num = v.Length - 1;
			if (num >= 1)
			{
				uint z = v[num];
				uint num2 = 0u;
				int num3 = 6 + 52 / (num + 1);
				while (0 < num3--)
				{
					num2 += 2654435769u;
					uint e = (num2 >> 2) & 3;
					int i;
					uint y;
					for (i = 0; i < num; i++)
					{
						y = v[i + 1];
						z = (v[i] += MX(num2, y, z, i, e, k));
					}
					y = v[0];
					z = (v[num] += MX(num2, y, z, i, e, k));
				}
			}
			return v;
		}

		public static string EncryptToBase64String(byte[] data, byte[] key)
		{
			return Convert.ToBase64String(Encrypt(data, key));
		}

		public static string EncryptToBase64String(string data, byte[] key)
		{
			return Convert.ToBase64String(Encrypt(data, key));
		}

		public static string EncryptToBase64String(byte[] data, string key)
		{
			return Convert.ToBase64String(Encrypt(data, key));
		}

		public static string EncryptToBase64String(string data, string key)
		{
			return Convert.ToBase64String(Encrypt(data, key));
		}

		private static byte[] FixKey(byte[] key)
		{
			if (key.Length == 16)
			{
				return key;
			}
			byte[] array = new byte[16];
			if (key.Length < 16)
			{
				key.CopyTo(array, 0);
				return array;
			}
			Array.Copy(key, 0, array, 0, 16);
			return array;
		}

		private static uint MX(uint sum, uint y, uint z, int p, uint e, uint[] k)
		{
			return (((z >> 5) ^ (y << 2)) + ((y >> 3) ^ (z << 4))) ^ ((sum ^ y) + (k[(int)(IntPtr)((p & 3) ^ e)] ^ z));
		}

		private static byte[] ToByteArray(uint[] data, bool includeLength)
		{
			int num = data.Length << 2;
			if (includeLength)
			{
				int num2 = (int)data[data.Length - 1];
				num -= 4;
				if (num2 < num - 3 || num2 > num)
				{
					return null;
				}
				num = num2;
			}
			byte[] array = new byte[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = (byte)(data[i >> 2] >> ((i & 3) << 3));
			}
			return array;
		}

		private static uint[] ToUInt32Array(byte[] data, bool includeLength)
		{
			int num = data.Length;
			int num2 = (((num & 3) != 0) ? ((num >> 2) + 1) : (num >> 2));
			uint[] array;
			if (includeLength)
			{
				array = new uint[num2 + 1];
				array[num2] = (uint)num;
			}
			else
			{
				array = new uint[num2];
			}
			for (int i = 0; i < num; i++)
			{
				array[i >> 2] |= (uint)(data[i] << ((i & 3) << 3));
			}
			return array;
		}
	}
}
