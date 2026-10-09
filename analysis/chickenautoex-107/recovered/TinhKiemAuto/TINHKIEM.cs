using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using TinhKiemAuto.Models;

namespace TinhKiemAuto
{
	public class TINHKIEM
	{
		public enum Menpai
		{
			MoDung = 32,
			ThieuLam = 1,
			MinhGiao = 2,
			CaiBang = 3,
			VoDang = 4,
			NgaMy = 5,
			TinhTuc = 6,
			ThienLong = 7,
			ThienSon = 8,
			TieuDao = 9,
			DuongMon = 37,
			KhongCo = 0
		}

		public class Sign
		{
			public static string CharBase = "8B15 ???????? 8B42 ?? 8B80 ???????? 8B40";

			public static string ActionBase = "CHelperSystem";

			public static string MultiAcc = "E8 ???????? 85C0 0F84";

			public static string QuestInfo = "QUEST_INFO";

			public static string Logon = "LOGIN_MIBAO";
		}

		public class GZip
		{
			public static byte[] Compress(string input)
			{
				byte[] array = ((!File.Exists(input)) ? Encoding.ASCII.GetBytes(input) : File.ReadAllBytes(input));
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, leaveOpen: true))
					{
						gZipStream.Write(array, 0, array.Length);
					}
					return memoryStream.ToArray();
				}
			}

			public static void Compress(string input, string destinationPath)
			{
				byte[] array = ((!File.Exists(input)) ? Encoding.ASCII.GetBytes(input) : File.ReadAllBytes(input));
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, leaveOpen: true))
					{
						gZipStream.Write(array, 0, array.Length);
					}
					using (FileStream stream = new FileStream(destinationPath, FileMode.Create))
					{
						memoryStream.WriteTo(stream);
					}
				}
			}

			public static byte[] Uncompress(string input)
			{
				byte[] buffer = ((!File.Exists(input)) ? Encoding.ASCII.GetBytes(input) : File.ReadAllBytes(input));
				using (GZipStream gZipStream = new GZipStream(new MemoryStream(buffer), CompressionMode.Decompress))
				{
					byte[] buffer2 = new byte[4096];
					using (MemoryStream memoryStream = new MemoryStream())
					{
						int num;
						do
						{
							num = gZipStream.Read(buffer2, 0, 4096);
							if (num > 0)
							{
								memoryStream.Write(buffer2, 0, num);
							}
						}
						while (num > 0);
						return memoryStream.ToArray();
					}
				}
			}

			public static void Uncompress(string input, string destinationPath)
			{
				byte[] buffer = ((!File.Exists(input)) ? Encoding.ASCII.GetBytes(input) : File.ReadAllBytes(input));
				using (GZipStream gZipStream = new GZipStream(new MemoryStream(buffer), CompressionMode.Decompress))
				{
					byte[] buffer2 = new byte[4096];
					using (MemoryStream memoryStream = new MemoryStream())
					{
						int num;
						do
						{
							num = gZipStream.Read(buffer2, 0, 4096);
							if (num > 0)
							{
								memoryStream.Write(buffer2, 0, num);
							}
						}
						while (num > 0);
						using (FileStream stream = new FileStream(destinationPath, FileMode.Create))
						{
							memoryStream.WriteTo(stream);
						}
					}
				}
			}
		}

		public class Hasher
		{
			private Hasher()
			{
			}

			private static byte[] ConvertStringToByteArray(string data)
			{
				return new UnicodeEncoding().GetBytes(data);
			}

			private static FileStream GetFileStream(string pathName)
			{
				return new FileStream(pathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			}

			public static string SHA1(string pathName)
			{
				string result = "";
				SHA1CryptoServiceProvider sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
				try
				{
					FileStream fileStream = GetFileStream(pathName);
					byte[] array = sHA1CryptoServiceProvider.ComputeHash(fileStream);
					fileStream.Close();
					result = BitConverter.ToString(array).Replace("-", "");
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
				}
				return result;
			}

			public static string MD5(string input)
			{
				string text = "";
				MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
				try
				{
					if (File.Exists(input))
					{
						FileStream fileStream = GetFileStream(input);
						byte[] array = mD5CryptoServiceProvider.ComputeHash(fileStream);
						fileStream.Close();
						text = BitConverter.ToString(array).Replace("-", "");
					}
					else
					{
						MD5 mD = System.Security.Cryptography.MD5.Create();
						byte[] bytes = Encoding.ASCII.GetBytes(input);
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
					MD5 mD2 = System.Security.Cryptography.MD5.Create();
					byte[] bytes2 = Encoding.ASCII.GetBytes(input);
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

			public static string Encrypt(string toEncrypt, string key)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(toEncrypt);
				new AppSettingsReader();
				MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
				byte[] key2 = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(key));
				mD5CryptoServiceProvider.Clear();
				TripleDESCryptoServiceProvider obj = new TripleDESCryptoServiceProvider
				{
					Key = key2,
					Mode = CipherMode.ECB,
					Padding = PaddingMode.PKCS7
				};
				byte[] array = obj.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length);
				obj.Clear();
				return Convert.ToBase64String(array, 0, array.Length);
			}

			public static string Decrypt(string cipherString, string key)
			{
				byte[] array = Convert.FromBase64String(cipherString);
				new AppSettingsReader();
				MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
				byte[] key2 = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(key));
				mD5CryptoServiceProvider.Clear();
				TripleDESCryptoServiceProvider obj = new TripleDESCryptoServiceProvider
				{
					Key = key2,
					Mode = CipherMode.ECB,
					Padding = PaddingMode.PKCS7
				};
				byte[] bytes = obj.CreateDecryptor().TransformFinalBlock(array, 0, array.Length);
				obj.Clear();
				return Encoding.UTF8.GetString(bytes);
			}
		}

		public static Dictionary<string, string> TLBBDIC = new Dictionary<string, string> { { "#{XSLC_130831_01}", "Duyên khởi vô lượng" } };

		public static Dictionary<string, string> DicVatPhamNhiemVu = new Dictionary<string, string>();

		public static readonly char[] Unicodes = new char[256]
		{
			'\0', '\u0001', 'Ẳ', '\u0003', '\u0004', 'Ẵ', 'Ẫ', '\a', '\b', '\t',
			'\n', '\v', '\f', '\r', '\u000e', '\u000f', '\u0010', '\u0011', '\u0012', '\u0013',
			'Ỷ', '\u0015', '\u0016', '\u0017', '\u0018', 'Ỹ', '\u001a', '\u001b', '\u001c', '\u001d',
			'Ỵ', '\u001f', ' ', '!', '"', '#', '$', '%', '&', '\'',
			'(', ')', '*', '+', ',', '-', '.', '/', '0', '1',
			'2', '3', '4', '5', '6', '7', '8', '9', ':', ';',
			'<', '=', '>', '?', '@', 'A', 'B', 'C', 'D', 'E',
			'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O',
			'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y',
			'Z', '[', '\\', ']', '^', '_', '`', 'a', 'b', 'c',
			'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
			'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w',
			'x', 'y', 'z', '{', '|', '}', '~', '\u007f', 'Ạ', 'Ắ',
			'Ằ', 'Ặ', 'Ấ', 'Ầ', 'Ẩ', 'Ậ', 'Ẽ', 'Ẹ', 'Ế', 'Ề',
			'Ể', 'Ễ', 'Ệ', 'Ố', 'Ồ', 'Ổ', 'Ỗ', 'Ộ', 'Ợ', 'Ớ',
			'Ờ', 'Ở', 'Ị', 'Ỏ', 'Ọ', 'Ỉ', 'Ủ', 'Ũ', 'Ụ', 'Ỳ',
			'Õ', 'ắ', 'ằ', 'ặ', 'ấ', 'ầ', 'ẩ', 'ậ', 'ẽ', 'ẹ',
			'ế', 'ề', 'ể', 'ễ', 'ệ', 'ố', 'ồ', 'ổ', 'ỗ', 'Ỡ',
			'Ơ', 'ộ', 'ờ', 'ở', 'ị', 'Ự', 'Ứ', 'Ừ', 'Ử', 'ơ',
			'ớ', 'Ư', 'À', 'Á', 'Â', 'Ã', 'Ả', 'Ă', 'ẳ', 'ẵ',
			'È', 'É', 'Ê', 'Ẻ', 'Ì', 'Í', 'Ĩ', 'ỳ', 'Đ', 'ứ',
			'Ò', 'Ó', 'Ô', 'ạ', 'ỷ', 'ừ', 'ử', 'Ù', 'Ú', 'ỹ',
			'ỵ', 'Ý', 'ỡ', 'ư', 'à', 'á', 'â', 'ã', 'ả', 'ă',
			'ữ', 'ẫ', 'è', 'é', 'ê', 'ẻ', 'ì', 'í', 'ĩ', 'ỉ',
			'đ', 'ự', 'ò', 'ó', 'ô', 'õ', 'ỏ', 'ọ', 'ụ', 'ù',
			'ú', 'ũ', 'ủ', 'ý', 'ợ', 'Ữ'
		};

		public static string[] vietnameseSigns = new string[15]
		{
			"aAeEoOuUiIdDyY", "áàạảãâấầậẩẫăắằặẳẵ", "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ", "éèẹẻẽêếềệểễ", "ÉÈẸẺẼÊẾỀỆỂỄ", "óòọỏõôốồộổỗơớờợởỡ", "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ", "úùụủũưứừựửữ", "ÚÙỤỦŨƯỨỪỰỬỮ", "íìịỉĩ",
			"ÍÌỊỈĨ", "đ", "Đ", "ýỳỵỷỹ", "ÝỲỴỶỸ"
		};

		public static bool FileHostValid => !ReadFile(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\drivers\\etc\\hosts").ToLower().Contains("tinhkiem.us");

		public static bool IsPressedCtrl
		{
			get
			{
				if (!IsPressed(VirtualKeyStates.VK_CONTROL))
				{
					return IsPressed(VirtualKeyStates.VK_RCONTROL);
				}
				return true;
			}
		}

		public static bool IsPressedShift
		{
			get
			{
				if (!IsPressed(VirtualKeyStates.VK_LSHIFT))
				{
					return IsPressed(VirtualKeyStates.VK_RSHIFT);
				}
				return true;
			}
		}

		public static bool IsPressedAlt
		{
			get
			{
				if (!IsPressed(VirtualKeyStates.VK_LMENU))
				{
					return IsPressed(VirtualKeyStates.VK_RMENU);
				}
				return true;
			}
		}

		public static string FormatMoney(int value)
		{
			if (value == 0)
			{
				return "0";
			}
			return $"{value:#,###}";
		}

		public static void MoveToEnd(TextBox txt)
		{
			txt.Select(txt.Text.Length, 0);
			txt.Focus();
			txt.ScrollToCaret();
		}

		public static void MoveToBeg(TextBox txt)
		{
			txt.Select(0, 0);
			txt.Focus();
			txt.ScrollToCaret();
		}

		public static string GetMACAddress()
		{
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			string text = string.Empty;
			NetworkInterface[] array = allNetworkInterfaces;
			foreach (NetworkInterface networkInterface in array)
			{
				if (text == string.Empty)
				{
					networkInterface.GetIPProperties();
					text = networkInterface.GetPhysicalAddress().ToString();
				}
			}
			return text;
		}

		public static string HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc)
		{
			string text = "---------------------------" + DateTime.Now.Ticks.ToString("x");
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.ContentType = "multipart/form-data; boundary=" + text;
			httpWebRequest.Method = "POST";
			httpWebRequest.KeepAlive = true;
			httpWebRequest.ServicePoint.Expect100Continue = false;
			using (Stream stream = httpWebRequest.GetRequestStream())
			{
				using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8))
				{
					foreach (string key in nvc.Keys)
					{
						streamWriter.Write("\r\n" + text + "\r\n");
						streamWriter.Write($"Content-Disposition: form-data; name=\"{key}\"\r\n\r\n{nvc[key]}");
					}
					streamWriter.Write("\r\n" + text + "\r\n");
					streamWriter.Write($"Content-Disposition: form-data; name=\"{paramName}\"; filename=\"{file}\"\r\nContent-Type: {contentType}\r\n\r\n");
					using (new FileStream(file, FileMode.Open, FileAccess.Read))
					{
					}
					streamWriter.Write("\r\n--" + text + "--\r\n");
				}
			}
			try
			{
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					if (httpWebResponse.StatusCode == HttpStatusCode.OK)
					{
						using (Stream stream2 = httpWebResponse.GetResponseStream())
						{
							if (stream2 != null)
							{
								using (StreamReader streamReader = new StreamReader(stream2))
								{
									return streamReader.ReadToEnd();
								}
							}
							return null;
						}
					}
					throw new ApplicationException("Error while upload files. Server status code: " + httpWebResponse.StatusCode);
				}
			}
			catch (Exception innerException)
			{
				throw new ApplicationException("Error while uploading file", innerException);
			}
		}

		public static void CopyTo(Stream input, Stream output)
		{
			byte[] array = new byte[4096];
			int count;
			while ((count = input.Read(array, 0, array.Length)) != 0)
			{
				output.Write(array, 0, count);
			}
		}

		public static byte[] ToArr(Stream input)
		{
			int num = Convert.ToInt32(input.Length);
			byte[] array = new byte[num];
			input.Read(array, 0, num);
			return array;
		}

		public static int Bool2Int(bool value)
		{
			if (value)
			{
				return 1;
			}
			return 0;
		}

		public static uint ComputeStringHash(string string_0)
		{
			uint num = 0u;
			if (string_0 != null)
			{
				num = 2166136261u;
				for (int i = 0; i < string_0.Length; i++)
				{
					num = (string_0[i] ^ num) * 16777619;
				}
			}
			return num;
		}

		public static Keys String2Key(string key)
		{
			switch (ComputeStringHash(key))
			{
			case 87056132u:
				if (key == "Alt 8")
				{
					return Keys.D8;
				}
				break;
			case 3168037u:
				if (key == "Alt 3")
				{
					return Keys.D3;
				}
				break;
			case 215134355u:
				if (key == "F8")
				{
					return Keys.F8;
				}
				break;
			case 198356736u:
				if (key == "F9")
				{
					return Keys.F9;
				}
				break;
			case 103833751u:
				if (key == "Alt 9")
				{
					return Keys.D9;
				}
				break;
			case 382910545u:
				if (key == "F2")
				{
					return Keys.F2;
				}
				break;
			case 366132926u:
				if (key == "F3")
				{
					return Keys.F3;
				}
				break;
			case 332577688u:
				if (key == "F1")
				{
					return Keys.F1;
				}
				break;
			case 433243402u:
				if (key == "F7")
				{
					return Keys.F7;
				}
				break;
			case 416465783u:
				if (key == "F4")
				{
					return Keys.F4;
				}
				break;
			case 399688164u:
				if (key == "F5")
				{
					return Keys.F5;
				}
				break;
			case 3703400824u:
				if (key == "F10")
				{
					return Keys.F10;
				}
				break;
			case 450021021u:
				if (key == "F6")
				{
					return Keys.F6;
				}
				break;
			case 4180692000u:
				if (key == "Alt 4")
				{
					return Keys.D4;
				}
				break;
			case 3736956062u:
				if (key == "F12")
				{
					return Keys.F12;
				}
				break;
			case 3720178443u:
				if (key == "F11")
				{
					return Keys.F11;
				}
				break;
			case 4231024857u:
				if (key == "Alt 7")
				{
					return Keys.D7;
				}
				break;
			case 4214247238u:
				if (key == "Alt 6")
				{
					return Keys.D6;
				}
				break;
			case 4197469619u:
				if (key == "Alt 5")
				{
					return Keys.D5;
				}
				break;
			case 4281357714u:
				if (key == "Alt 2")
				{
					return Keys.D2;
				}
				break;
			case 4264580095u:
				if (key == "Alt 1")
				{
					return Keys.D1;
				}
				break;
			case 4247802476u:
				if (key == "Alt 0")
				{
					return Keys.D0;
				}
				break;
			}
			return Keys.F13;
		}

		public static Keys Int2Key(int key)
		{
			switch (key)
			{
			case 0:
				return Keys.F1;
			case 1:
				return Keys.F2;
			case 2:
				return Keys.F3;
			case 3:
				return Keys.F4;
			case 4:
				return Keys.F5;
			case 5:
				return Keys.F6;
			case 6:
				return Keys.F7;
			case 7:
				return Keys.F8;
			case 8:
				return Keys.F9;
			case 9:
				return Keys.F10;
			case 10:
				return Keys.F11;
			case 11:
				return Keys.F12;
			case 12:
				return Keys.D1;
			case 13:
				return Keys.D2;
			case 14:
				return Keys.D3;
			case 15:
				return Keys.D4;
			case 16:
				return Keys.D5;
			case 17:
				return Keys.D6;
			case 18:
				return Keys.D7;
			case 19:
				return Keys.D8;
			case 20:
				return Keys.D9;
			case 21:
				return Keys.D0;
			default:
				return Keys.F13;
			}
		}

		public static int Key2Int(Keys key)
		{
			switch (key)
			{
			case Keys.D0:
				return 21;
			case Keys.D1:
				return 12;
			case Keys.D2:
				return 13;
			case Keys.D3:
				return 14;
			case Keys.D4:
				return 15;
			case Keys.D5:
				return 16;
			case Keys.D6:
				return 17;
			case Keys.D7:
				return 18;
			case Keys.D8:
				return 19;
			case Keys.D9:
				return 20;
			case Keys.F1:
				return 0;
			case Keys.F2:
				return 1;
			case Keys.F3:
				return 2;
			case Keys.F4:
				return 3;
			case Keys.F5:
				return 4;
			case Keys.F6:
				return 5;
			case Keys.F7:
				return 6;
			case Keys.F8:
				return 7;
			case Keys.F9:
				return 8;
			case Keys.F10:
				return 9;
			case Keys.F11:
				return 10;
			case Keys.F12:
				return 11;
			default:
				return 22;
			}
		}

		public static int GetTruyen(string ChuoiTruyen)
		{
			ChuoiTruyen = VietLien(ChuoiTruyen);
			if (ChuoiTruyen.Contains("namvuc"))
			{
				return 4;
			}
			if (ChuoiTruyen.Contains("quynhchau"))
			{
				return 4;
			}
			if (ChuoiTruyen.Contains("vodi"))
			{
				return 4;
			}
			if (ChuoiTruyen.Contains("haitacdong"))
			{
				return 4;
			}
			if (ChuoiTruyen.Contains("haitocdong"))
			{
				return 4;
			}
			if (ChuoiTruyen.Contains("mieunhandong"))
			{
				return 4;
			}
			if (ChuoiTruyen.Contains("namchieu"))
			{
				return 5;
			}
			if (ChuoiTruyen.Contains("diemho"))
			{
				return 5;
			}
			if (ChuoiTruyen.Contains("bachsadiemkhanh"))
			{
				return 5;
			}
			if (ChuoiTruyen.Contains("bochsadiemkhanh"))
			{
				return 5;
			}
			if (ChuoiTruyen.Contains("thachlam"))
			{
				return 5;
			}
			if (ChuoiTruyen.Contains("thochlam"))
			{
				return 5;
			}
			if (ChuoiTruyen.Contains("mieucuong"))
			{
				return 5;
			}
			if (ChuoiTruyen.Contains("ngockhe"))
			{
				return 5;
			}
			if (ChuoiTruyen.Contains("truongbachson"))
			{
				return 6;
			}
			if (ChuoiTruyen.Contains("truongbochson"))
			{
				return 6;
			}
			if (ChuoiTruyen.Contains("hoanglongphu"))
			{
				return 6;
			}
			if (ChuoiTruyen.Contains("tuyetlangho"))
			{
				return 6;
			}
			if (ChuoiTruyen.Contains("thaonguyen"))
			{
				return 6;
			}
			if (ChuoiTruyen.Contains("thuykinhho"))
			{
				return 6;
			}
			if (ChuoiTruyen.Contains("lieutay"))
			{
				return 6;
			}
			if (ChuoiTruyen.Contains("tienvuongphan"))
			{
				return 6;
			}
			if (ChuoiTruyen.Contains("nganngaituyetnguyen"))
			{
				return 6;
			}
			return -1;
		}

		public static int SecDiff(DateTime from, DateTime to)
		{
			return (int)(to - from).TotalSeconds;
		}

		public static int IsKhoang(string ChuoiTruyen)
		{
			ChuoiTruyen = VietLien(ChuoiTruyen);
			switch (ChuoiTruyen)
			{
			case "modong":
				return 1;
			case "mosat":
				return 2;
			case "mobac":
				return 3;
			case "mohanthiet":
				return 4;
			case "movang":
				return 5;
			case "mohuyenthiet":
				return 6;
			case "mophale":
				return 7;
			case "mophithuy":
				return 8;
			case "mochanvu":
				return 9;
			case "molonghuyet":
				return 10;
			case "mophunghuyet":
				return 11;
			default:
				return -1;
			}
		}

		public static int IsDuoc(string ChuoiTruyen)
		{
			ChuoiTruyen = VietLien(ChuoiTruyen);
			switch (ChuoiTruyen)
			{
			case "bachanh":
				return 1;
			case "bochanh":
				return 1;
			case "bohoang":
				return 2;
			case "xuyenboi":
				return 3;
			case "nguyenho":
				return 4;
			case "tyba":
				return 5;
			case "camthao":
				return 6;
			case "kimnganhoa":
				return 7;
			case "hoangcam":
				return 8;
			case "cauky":
				return 9;
			case "tramhuong":
				return 10;
			case "dotrong":
				return 11;
			case "thuongthuat":
				return 12;
			case "phuclinh":
				return 13;
			case "phongphong":
				return 14;
			case "huongnhu":
				return 15;
			case "hoanglien":
				return 16;
			case "duongqui":
				return 17;
			case "quetam":
				return 18;
			case "huongphu":
				return 19;
			case "hoachuong":
				return 20;
			case "hoithanthao":
				return 21;
			case "thuo":
				return 22;
			case "dongtrunghathao":
				return 23;
			case "dongtrunghothao":
				return 23;
			case "longquitu":
				return 24;
			case "tuongboi":
				return 25;
			case "nhansam":
				return 26;
			case "linhchi":
				return 27;
			case "tuanthao":
				return 28;
			case "lientu":
				return 29;
			case "khomocxuan":
				return 30;
			default:
				return -1;
			}
		}

		public static string MapToString(int mapid)
		{
			switch (mapid)
			{
			case 0:
				return "lacduong";
			case 1:
				return "tochau";
			case 2:
				return "daily";
			case 3:
				return "tungson";
			case 4:
				return "thaiho";
			case 7:
				return "kiemcac";
			case 8:
				return "donhoang";
			case 18:
				return "nhannam";
			case 19:
				return "nhanbac";
			case 20:
				return "thaonguyen";
			case 21:
				return "lieutay";
			case 22:
				return "truongbachson";
			case 23:
				return "hoanglongphu";
			case 24:
				return "nhihai";
			case 25:
				return "thuongson";
			case 26:
				return "thachlam";
			case 27:
				return "ngockhue";
			case 28:
				return "namchieu";
			case 29:
				return "mieucuong";
			case 30:
				return "tayho";
			case 31:
				return "longtuyen";
			case 32:
				return "vodi";
			case 33:
				return "mailinh";
			case 34:
				return "namhai";
			case 35:
				return "quynhchau";
			default:
				return "khongbiet";
			}
		}

		public static string GetBangXY(string input)
		{
			input = VietLien(input);
			if (input.Contains("thaonguyen"))
			{
				if (input.Contains("chinhdong"))
				{
					return "271,191,20," + NPC.ThaoNguyenChinhDong.Id;
				}
				if (input.Contains("chinhbac"))
				{
					return "0,0,20," + NPC.ThaoNguyenChinhDong.Id;
				}
				if (input.Contains("chinhtay"))
				{
					return "66,202,20," + NPC.ThaoNguyenChinhTay.Id;
				}
				if (input.Contains("taynam"))
				{
					return "97,281,20," + NPC.ThaoNguyenTayNam.Id;
				}
			}
			else if (input.Contains("nhannam") || input.Contains("nhonnam"))
			{
				if (input.Contains("chinhdong"))
				{
					return "283,113,18," + NPC.NhanNamChinhDong.Id;
				}
				if (input.Contains("chinhbac"))
				{
					return "102,36,18," + NPC.NhanNamChinhBac.Id;
				}
				if (input.Contains("chinhtay"))
				{
					return "0,0,18," + NPC.NhanNamChinhBac.Id;
				}
				if (input.Contains("chinhnam"))
				{
					return "72,284,18," + NPC.NhanNamChinhNam.Id;
				}
			}
			else if (input.Contains("nhanbac") || input.Contains("nhonbac"))
			{
				if (input.Contains("dongbac"))
				{
					return "234,24,19," + NPC.NhanBacDongBac.Id;
				}
				if (input.Contains("taybac"))
				{
					return "116,29,19," + NPC.NhanBacTayBac.Id;
				}
				if (input.Contains("chinhtay"))
				{
					return "32,128,19," + NPC.NhanBacChinhTay.Id;
				}
				if (input.Contains("huongnam"))
				{
					return "0,0,19," + NPC.NhanBacChinhTay.Id;
				}
			}
			else if (input.Contains("lieutay"))
			{
				if (input.Contains("dongnam"))
				{
					return "277,258,21," + NPC.LieuTayDongNam.Id;
				}
				if (input.Contains("taybac"))
				{
					return "75,35,21," + NPC.LieuTayTayBac.Id;
				}
				if (input.Contains("chinhtay"))
				{
					return "40,142,21," + NPC.LieuTayChinhTay.Id;
				}
				if (input.Contains("huongnam"))
				{
					return "0,0,21," + NPC.LieuTayChinhTay.Id;
				}
			}
			else if (input.Contains("truongbachson") || input.Contains("truongbochson"))
			{
				if (input.Contains("chinhnam"))
				{
					return "216,282," + NPC.TruongBachSonChinhNam.Id;
				}
				if (input.Contains("taybac"))
				{
					return "39,63," + NPC.TruongBachSonTayBac.Id;
				}
				if (input.Contains("chinhdong"))
				{
					return "280,154," + NPC.TruongBachSonChinhDong.Id;
				}
				if (input.Contains("chinhnam"))
				{
					return "0,0,22," + NPC.TruongBachSonChinhDong.Id;
				}
			}
			else if (input.Contains("hoanglongphu"))
			{
				if (input.Contains("dongnam"))
				{
					return "253,285,23," + NPC.HoangLongPhuDongNam.Id;
				}
				if (input.Contains("taybac"))
				{
					return "28,54,23," + NPC.HoangLongPhuTayBac.Id;
				}
				if (input.Contains("chinhdong"))
				{
					return "290,115,23," + NPC.HoangLongPhuChinhDong.Id;
				}
				if (input.Contains("huongnam"))
				{
					return "0,0,,23," + NPC.HoangLongPhuChinhDong.Id;
				}
			}
			else if (input.Contains("nhihai"))
			{
				if (input.Contains("chinhdong"))
				{
					return "285,166,24," + NPC.NhiHaiChinhDong.Id;
				}
				if (input.Contains("chinhnam"))
				{
					return "173,283,24," + NPC.NhiHaiChinhNam.Id;
				}
				if (input.Contains("chinhtay"))
				{
					return "34,100,24," + NPC.NhiHaiChinhTay.Id;
				}
				if (input.Contains("huongnam"))
				{
					return "0,0,24," + NPC.NhiHaiChinhTay.Id;
				}
			}
			else if (input.Contains("thuongson"))
			{
				if (input.Contains("tay"))
				{
					return "37,172,25," + NPC.ThuongSonTranTay.Id;
				}
				if (input.Contains("chinhdong"))
				{
					return "294,153,25," + NPC.ThuongSonChinhDong.Id;
				}
				if (input.Contains("chinhnam"))
				{
					return "146,284,25," + NPC.ThuongSonChinhNam.Id;
				}
				if (input.Contains("huongnam"))
				{
					return "0,0,25," + NPC.ThuongSonChinhNam.Id;
				}
			}
			else if (input.Contains("thachlam") || input.Contains("thochlam"))
			{
				if (input.Contains("chinhbac"))
				{
					return "226,36,26," + NPC.ThachLamChinhBac.Id;
				}
				if (input.Contains("chinhnam"))
				{
					return "278,281,26," + NPC.ThachLamChinhNam.Id;
				}
				if (input.Contains("chinhtay"))
				{
					return "45,177,26," + NPC.ThachLamChinhTay.Id;
				}
				if (input.Contains("huongnam"))
				{
					return "0,0,26," + NPC.ThachLamChinhTay.Id;
				}
			}
			else if (input.Contains("mieucuong"))
			{
				if (input.Contains("dongbac"))
				{
					return "250,45,29," + NPC.MieuCuongDongBac.Id;
				}
				if (input.Contains("taynam"))
				{
					return "37,251,29," + NPC.MieuCuongTayNam.Id;
				}
				if (input.Contains("chinhdong"))
				{
					return "281,160,29," + NPC.MieuCuongChinhDong.Id;
				}
				if (input.Contains("huongnam"))
				{
					return "0,0,29," + NPC.MieuCuongChinhDong.Id;
				}
			}
			else if (input.Contains("tayho"))
			{
				if (input.Contains("taynam"))
				{
					return "45,267,30," + NPC.TayHoTayNam.Id;
				}
				if (input.Contains("chinhdong"))
				{
					return "261,231,30," + NPC.TayHoChinhDong.Id;
				}
				if (input.Contains("chinhtay"))
				{
					return "39,139,30," + NPC.TayHoChinhTay.Id;
				}
				if (input.Contains("huongnam"))
				{
					return "0,0,30," + NPC.TayHoChinhTay.Id;
				}
			}
			else if (input.Contains("longtuyen"))
			{
				if (input.Contains("dongnam"))
				{
					return "218,282,31," + NPC.LongTuyenDongNam.Id;
				}
				if (input.Contains("chinhbac"))
				{
					return "63,33,31," + NPC.LongTuyenChinhBac.Id;
				}
				if (input.Contains("chinhtay"))
				{
					return "35,121,31," + NPC.LongTuyenChinhTay.Id;
				}
				if (input.Contains("huongnam"))
				{
					return "0,0,31," + NPC.LongTuyenChinhTay.Id;
				}
			}
			else if (input.Contains("vodi"))
			{
				if (input.Contains("dongbac"))
				{
					return "254,38,32," + NPC.VoDiDongBac.Id;
				}
				if (input.Contains("chinhnam"))
				{
					return "113,280,32," + NPC.VoDiChinhNam.Id;
				}
				if (input.Contains("chinhtay"))
				{
					return "92,172,32," + NPC.VoDiChinhTay.Id;
				}
				if (input.Contains("huongnam"))
				{
					return "0,0,32," + NPC.VoDiChinhTay.Id;
				}
			}
			else if (input.Contains("mailinh"))
			{
				if (input.Contains("dongbac"))
				{
					return "271,37,33," + NPC.MaiLinhDongBac.Id;
				}
				if (input.Contains("taybac"))
				{
					return "31,90,33," + NPC.MaiLinhTayBac.Id;
				}
				if (input.Contains("chinhdong"))
				{
					return "282,236,33," + NPC.MaiLinhChinhDong.Id;
				}
				if (input.Contains("huongnam"))
				{
					return "0,0,33," + NPC.MaiLinhChinhDong.Id;
				}
			}
			else if (input.Contains("namvuc"))
			{
				if (input.Contains("dongbac"))
				{
					return "292,58,34," + NPC.NamVucDongBac.Id;
				}
				if (input.Contains("taynam"))
				{
					return "108,227,34," + NPC.NamVucTayNam.Id;
				}
				if (input.Contains("chinhbac"))
				{
					return "134,40,34," + NPC.NamVucChinhBac.Id;
				}
				if (input.Contains("huongnam"))
				{
					return "0,0,34," + NPC.NamVucChinhBac.Id;
				}
			}
			else
			{
				if (!input.Contains("quynhchau"))
				{
					return "";
				}
				if (input.Contains("dongc"))
				{
					return "243,150,35," + NPC.QuynhChauChinhDong.Id;
				}
				if (input.Contains("dongbac"))
				{
					return "273,52,35," + NPC.QuynhChauDongBac.Id;
				}
				if (input.Contains("tay"))
				{
					return "80,139,35," + NPC.QuynhChauChinhTay.Id;
				}
				if (input.Contains("nam"))
				{
					return "0,0,35," + NPC.QuynhChauChinhTay.Id;
				}
			}
			return "";
		}

		public static string GetDuaBangXY(string input)
		{
			input = VietLien(input);
			if (input.Contains("thaonguyen") || input.Contains("hoanglongphu") || input.Contains("truongbachson") || input.Contains("truongbochson") || input.Contains("lieutay"))
			{
				return "256,51,20";
			}
			if (input.Contains("nhanbac") || input.Contains("nhonbac") || input.Contains("nhannam") || input.Contains("nhonnam"))
			{
				return "277,54,19";
			}
			if (input.Contains("nhihai") || input.Contains("thuongson"))
			{
				return "77,206,24";
			}
			if (input.Contains("thachlam") || input.Contains("thochlam") || input.Contains("namchieu") || input.Contains("ngockhe"))
			{
				return "73,214,26";
			}
			if (input.Contains("mieucuong"))
			{
				return "195,49,29";
			}
			if (input.Contains("tayho") || input.Contains("longtuyen"))
			{
				return "134,165,30";
			}
			if (input.Contains("vodi"))
			{
				return "69,106,32";
			}
			if (input.Contains("namvuc"))
			{
				return "110,64,34";
			}
			if (input.Contains("quynhchau"))
			{
				return "136,236,35";
			}
			if (input.Contains("thaiho"))
			{
				return "246,146,4";
			}
			return "-1";
		}

		public static int GetMapId(string input)
		{
			input = VietLien(input);
			if (input.Contains("lacduong"))
			{
				return 0;
			}
			if (input.Contains("locduong"))
			{
				return 0;
			}
			if (input.Contains("tochau"))
			{
				return 1;
			}
			if (input.Contains("daily"))
			{
				return 2;
			}
			if (input.Contains("doily"))
			{
				return 2;
			}
			if (input.Contains("doilu"))
			{
				return 2;
			}
			if (input.Contains("tungson"))
			{
				return 3;
			}
			if (input.Contains("thaiho"))
			{
				return 4;
			}
			if (input.Contains("kinhho"))
			{
				return 5;
			}
			if (input.Contains("voluongson"))
			{
				return 6;
			}
			if (input.Contains("kiemcac"))
			{
				return 7;
			}
			if (input.Contains("donhoang"))
			{
				return 8;
			}
			if (input.Contains("thieulamtu"))
			{
				return 9;
			}
			if (input.Contains("caibangtongda"))
			{
				return 10;
			}
			if (input.Contains("quangminhdien"))
			{
				return 11;
			}
			if (input.Contains("vodangson"))
			{
				return 12;
			}
			if (input.Contains("thienlongtu"))
			{
				return 13;
			}
			if (input.Contains("langbadong"))
			{
				return 14;
			}
			if (input.Contains("ngamison"))
			{
				return 15;
			}
			if (input.Contains("tinhtuchai"))
			{
				return 16;
			}
			if (input.Contains("thienson"))
			{
				return 17;
			}
			if (input.Contains("nhannam"))
			{
				return 18;
			}
			if (input.Contains("nhonnam"))
			{
				return 18;
			}
			if (input.Contains("nhanbac"))
			{
				return 19;
			}
			if (input.Contains("nhonbac"))
			{
				return 19;
			}
			if (input.Contains("thaonguyen"))
			{
				return 20;
			}
			if (input.Contains("lieutay"))
			{
				return 21;
			}
			if (input.Contains("truongbachson"))
			{
				return 22;
			}
			if (input.Contains("truongbochson"))
			{
				return 22;
			}
			if (input.Contains("hoanglongphu"))
			{
				return 23;
			}
			if (input.Contains("nhihai"))
			{
				return 24;
			}
			if (input.Contains("thuongson"))
			{
				return 25;
			}
			if (input.Contains("thachlam"))
			{
				return 26;
			}
			if (input.Contains("thochlam"))
			{
				return 26;
			}
			if (input.Contains("ngockhe"))
			{
				return 27;
			}
			if (input.Contains("namchieu"))
			{
				return 28;
			}
			if (input.Contains("mieucuong"))
			{
				return 29;
			}
			if (input.Contains("tayho"))
			{
				return 30;
			}
			if (input.Contains("longtuyen"))
			{
				return 31;
			}
			if (input.Contains("vodi"))
			{
				return 32;
			}
			if (input.Contains("mailinh"))
			{
				return 33;
			}
			if (input.Contains("namvuc"))
			{
				return 34;
			}
			if (input.Contains("namhai"))
			{
				return 34;
			}
			if (input.Contains("quynhchau"))
			{
				return 35;
			}
			if (input.Contains("huyenvudao"))
			{
				return 112;
			}
			if (input.Contains("baotangdongtang1"))
			{
				return 166;
			}
			if (input.Contains("baotangdongtang2"))
			{
				return 169;
			}
			if (input.Contains("nganngaituyetnguyen"))
			{
				return 229;
			}
			if (input.Contains("baotangdongtang3"))
			{
				return 191;
			}
			if (input.Contains("baotangdongtang4"))
			{
				return 192;
			}
			if (input.Contains("baotangdongtang5"))
			{
				return 193;
			}
			if (input.Contains("thaolieutruong"))
			{
				return 199;
			}
			if (input.Contains("mieu nhan dong"))
			{
				return 200;
			}
			if (input.Contains("thanh thu son"))
			{
				return 201;
			}
			if (input.Contains("yenvuongcomotang1"))
			{
				return 202;
			}
			if (input.Contains("yenvuongcomotang2"))
			{
				return 203;
			}
			if (input.Contains("yenvuongcomotang3"))
			{
				return 204;
			}
			if (input.Contains("yenvuongcomotang4"))
			{
				return 205;
			}
			if (input.Contains("yenvuongcomotang5"))
			{
				return 206;
			}
			if (input.Contains("yenvuongcomotang6"))
			{
				return 207;
			}
			if (input.Contains("yenvuongcomotang7"))
			{
				return 208;
			}
			if (input.Contains("yenvuongcomotang8"))
			{
				return 209;
			}
			if (input.Contains("yenvuongcomotang9"))
			{
				return 210;
			}
			if (input.Contains("bentausondong"))
			{
				return 211;
			}
			if (input.Contains("kiemgia"))
			{
				return 212;
			}
			if (input.Contains("manhaidong"))
			{
				return 213;
			}
			if (input.Contains("danhancau"))
			{
				return 214;
			}
			if (input.Contains("ontuyendong"))
			{
				return 215;
			}
			if (input.Contains("hoanglongdong"))
			{
				return 216;
			}
			if (input.Contains("thuykinhho"))
			{
				return 217;
			}
			if (input.Contains("tienvuongphan"))
			{
				return 218;
			}
			if (input.Contains("thienkhanhthudong"))
			{
				return 219;
			}
			if (input.Contains("daohoanguyen"))
			{
				return 220;
			}
			if (input.Contains("haitacdong"))
			{
				return 221;
			}
			if (input.Contains("tuyetlangho"))
			{
				return 222;
			}
			if (input.Contains("diemho"))
			{
				return 235;
			}
			if (input.Contains("bachsadiemkhanh"))
			{
				return 237;
			}
			if (input.Contains("bochsadiemkhanh"))
			{
				return 237;
			}
			if (input.Contains("hoadiemson"))
			{
				return 244;
			}
			if (input.Contains("caoxuong"))
			{
				return 245;
			}
			if (input.Contains("laulan"))
			{
				return 246;
			}
			if (input.Contains("thaplymoc"))
			{
				return 247;
			}
			if (input.Contains("thaplumoc"))
			{
				return 247;
			}
			if (input.Contains("hoadiemcoc"))
			{
				return 251;
			}
			if (input.Contains("caoxuongmecung"))
			{
				return 252;
			}
			if (input.Contains("thapkhaclapmacan"))
			{
				return 253;
			}
			if (input.Contains("daiuyen"))
			{
				return 249;
			}
			if (input.Contains("hanhuyetlinh"))
			{
				return 255;
			}
			if (input.Contains("honhuyetlinh"))
			{
				return 255;
			}
			if (input.Contains("tanhoangdiacungtang1"))
			{
				return 262;
			}
			if (input.Contains("tanhoangdiacungtang2"))
			{
				return 263;
			}
			if (input.Contains("tanhoangdiacungtang3"))
			{
				return 264;
			}
			if (input.Contains("tanhoangdiacungtang4"))
			{
				return 292;
			}
			if (input.Contains("datayho"))
			{
				return 164;
			}
			if (input.Contains("dotayho"))
			{
				return 164;
			}
			if (input.Contains("conlonphucdia"))
			{
				return 254;
			}
			if (input.Contains("conlonson"))
			{
				return 248;
			}
			if (input.Contains("thanhnguyen"))
			{
				return 282;
			}
			if (input.Contains("thanhnguyensondong"))
			{
				return 283;
			}
			if (input.Contains("modungsontrang"))
			{
				return 284;
			}
			if (input.Contains("tatmanhihan"))
			{
				return 250;
			}
			if (input.Contains("thanhhoacung"))
			{
				return 256;
			}
			if (input.Contains("lamhaikhecoc"))
			{
				return 569;
			}
			if (input.Contains("macnamthanhnguyen"))
			{
				return 573;
			}
			if (input.Contains("vongxuyenhoahai"))
			{
				return 574;
			}
			if (input.Contains("thienkynamhoai"))
			{
				return 575;
			}
			if (input.Contains("thongthienthapdiacung"))
			{
				return 295;
			}
			if (input.Contains("thongthienthaptang1"))
			{
				return 296;
			}
			if (input.Contains("thongthienthaptang2"))
			{
				return 297;
			}
			if (input.Contains("thongthienthaptang3"))
			{
				return 298;
			}
			if (input.Contains("dinhthongthienthap"))
			{
				return 299;
			}
			if (input.Contains("phungminhtran"))
			{
				return 580;
			}
			if (input.Contains("laulan"))
			{
				return 246;
			}
			if (input.Contains("thuchacotran"))
			{
				return 260;
			}
			if (input.Contains("denhatkhunghingoitailacduong"))
			{
				return 238;
			}
			if (input.Contains("khunghingoitaidaily"))
			{
				return 240;
			}
			if (input.Contains("khunghingoitaitochau"))
			{
				return 241;
			}
			if (input.Contains("hanngoccoc"))
			{
				return 243;
			}
			if (input.Contains("thuynguyetdongthien"))
			{
				return 613;
			}
			if (input.Contains("huyenhai"))
			{
				return 611;
			}
			if (input.Contains("daicondihai"))
			{
				return 612;
			}
			if (input.Contains("phungminhtran"))
			{
				return 580;
			}
			if (input.Contains("thuynguyetdongthien"))
			{
				return 613;
			}
			if (input.Contains("lacduong"))
			{
				return 242;
			}
			if (input.Contains("huyenvudao"))
			{
				return 112;
			}
			if (input.Contains("laulan"))
			{
				return 246;
			}
			if (input.Contains("thuchacotran"))
			{
				return 260;
			}
			if (input.Contains("denhatkhunghingoitailacdduong"))
			{
				return 238;
			}
			if (input.Contains("denhikhunghingoitailacduong"))
			{
				return 239;
			}
			if (input.Contains("modungsontrang"))
			{
				return 284;
			}
			if (input.Contains("tientrang"))
			{
				return 224;
			}
			if (input.Contains("phungminhtran"))
			{
				return 580;
			}
			if (input.Contains("huyenvudao"))
			{
				return 112;
			}
			if (input.Contains("quangminhdong"))
			{
				return 601;
			}
			if (input.Contains("daycoctieudao"))
			{
				return 602;
			}
			if (input.Contains("linhtinhphong"))
			{
				return 603;
			}
			if (input.Contains("caibangtuudieu"))
			{
				return 604;
			}
			if (input.Contains("daohoatran"))
			{
				return 605;
			}
			if (input.Contains("thaplam"))
			{
				return 606;
			}
			if (input.Contains("nguthandong"))
			{
				return 607;
			}
			if (input.Contains("chietmaiphong"))
			{
				return 608;
			}
			if (input.Contains("chanthap"))
			{
				return 609;
			}
			if (input.Contains("tangthuthuycac"))
			{
				return 610;
			}
			if (input.Contains("hauhoavien"))
			{
				return 123;
			}
			if (input.Contains("tieumocnhanhang"))
			{
				return 122;
			}
			if (input.Contains("duonggiabao"))
			{
				return 615;
			}
			if (input.Contains("laulan"))
			{
				return 246;
			}
			if (input.Contains("thuchacotran"))
			{
				return 260;
			}
			if (input.Contains("modungsontrang"))
			{
				return 284;
			}
			if (input.Contains("phungminhtran"))
			{
				return 580;
			}
			if (input.Contains("dienvotruong"))
			{
				return 617;
			}
			if (input.Contains("quanthienthanh"))
			{
				return 581;
			}
			if (input.Contains("trieukinhthanh"))
			{
				return 583;
			}
			if (input.Contains("laphuthanh"))
			{
				return 582;
			}
			return -1;
		}

		public static string GapNPC(string input)
		{
			input = VietLien(input);
			if (input.Contains("trithanhdaisu") || input.Contains("bhrwsc_110331_53"))
			{
				return "0,176,192,trithanhdaisu,34";
			}
			if (input.Contains("trithanhdoisu") || input.Contains("bhrwsc_110331_53"))
			{
				return "0,176,192,trithanhdoisu,34";
			}
			if (input.Contains("doanchinhthuan"))
			{
				return "2,71,18,doanchinhthuan,16";
			}
			if (input.Contains("tothuc"))
			{
				return "1,166,311,tothuc,2";
			}
			return "";
		}

		public static string MuaDo(string input)
		{
			if (input.Contains("#{SDHDRW_091109_44}"))
			{
				return "104,123";
			}
			return "";
		}

		public static string HaiDuoc(string input)
		{
			if (input.Contains("#{SDHDRW_091109_41}"))
			{
				return "4,168,200,229,117,114,128,116,186,168,200";
			}
			return "";
		}

		public static string PhuBanMP(string input)
		{
			input = VietLien(input);
			if (input.Contains("longtu"))
			{
				return "186,98,142,hotutruonglao,13035,96,142";
			}
			if (input.Contains("modungsontrang"))
			{
				return "289,152,154,congdakhon,9044,160,169";
			}
			if (input.Contains("duonggiabao"))
			{
				return "616,152,154,duongmotuong,10051,173,170";
			}
			if (input.Contains("tinhtuchai"))
			{
				return "189,100,145,thientoantu,16035,96,142";
			}
			if (input.Contains("badong"))
			{
				return "187,45,126,congdatutruong,14035,44,129";
			}
			if (input.Contains("thieulamtu"))
			{
				return "182,99,146,huyenchung,9035,96,158";
			}
			if (input.Contains("thienson"))
			{
				return "190,94,147,dangba,17035,95,148";
			}
			if (input.Contains("ngamison"))
			{
				return "188,95,146,lieutammuoi,15035,89,146";
			}
			if (input.Contains("vodangson"))
			{
				return "185,100,181,tieuthiendat,12035,95,192";
			}
			if (input.Contains("quangminhdien"))
			{
				return "184,95,162,thaccang,11035,98,159";
			}
			if (input.Contains("caibangtongda"))
			{
				return "183,93,152,auduongqua,10035,91,159";
			}
			return "";
		}

		public static string PhuBanDanhQuai(string input)
		{
			input = VietLien(input);
			if (input.Contains("dietyeukhoiloi"))
			{
				return "127,113,dietyeukhoiloi";
			}
			if (input.Contains("trutienkhoiloi"))
			{
				return "95,80,trutienkhoiloi";
			}
			if (input.Contains("thithankhoiloi"))
			{
				return "51,72,thithankhoiloi";
			}
			if (input.Contains("thambiphitac"))
			{
				return "69,126,thambiphitac";
			}
			if (input.Contains("tamthuphitac"))
			{
				return "54,61,tamthuphitac";
			}
			if (input.Contains("suubaophitac"))
			{
				return "68,142,phitieuthiettac";
			}
			if (input.Contains("tatlethiettac"))
			{
				return "98,70,tatlethiettac";
			}
			if (input.Contains("docchamthiettac"))
			{
				return "51,69,docchamthiettac";
			}
			if (input.Contains("phitieuthiettac"))
			{
				return "100,181,phitieuthiettac";
			}
			if (input.Contains("mocvuongtrithu"))
			{
				return "96,126,mocvuongtrithu";
			}
			if (input.Contains("thuyvuongtrithu"))
			{
				return "118,112,thuyvuongtrithu";
			}
			if (input.Contains("hoavuongtrithu"))
			{
				return "96,86,hoavuongtrithu";
			}
			if (input.Contains("huthekhoiloi"))
			{
				return "52,72,huthekhoiloi";
			}
			if (input.Contains("thuctamkhoiloi"))
			{
				return "120,140,thuctamkhoiloi";
			}
			if (input.Contains("hoaphachkhoiloi"))
			{
				return "146,58,hoaphachkhoiloi";
			}
			if (input.Contains("mocnhanlaula"))
			{
				return "96,110,mocnhanlaula";
			}
			if (input.Contains("mocnhantinhanh"))
			{
				return "96,80,mocnhantinhanh";
			}
			if (input.Contains("mocnhanvosi"))
			{
				return "40,98,mocnhanvosi";
			}
			if (input.Contains("thiensontieutuyetquai"))
			{
				return "96,110,thiensontieutuyetquai";
			}
			if (input.Contains("thiensondaituyetquai"))
			{
				return "96,86,thiensondaituyetquai";
			}
			if (input.Contains("thiensontuyetquaivuong"))
			{
				return "96,50,thiensontuyetquaivuong";
			}
			if (input.Contains("ngamibachmyacvien"))
			{
				return "139,106,ngamibachmyacvien";
			}
			if (input.Contains("ngamiloitraoacvien"))
			{
				return "96,63,ngamiloitraoacvien";
			}
			if (input.Contains("ngamihungnhu"))
			{
				return "44,45,ngamihungnhu";
			}
			if (input.Contains("yeuditamma"))
			{
				return "59,180,yeuditamma";
			}
			if (input.Contains("phasantamma"))
			{
				return "78,132,phasantamma";
			}
			if (input.Contains("satductamma"))
			{
				return "46,58,satductamma";
			}
			if (input.Contains("matthamtienphong"))
			{
				return "97,117,matthamtienphong";
			}
			if (input.Contains("thanhkythamma"))
			{
				return "155,102,thanhkythamma";
			}
			if (input.Contains("lamkythamma"))
			{
				return "98,65,lamkythamma";
			}
			if (input.Contains("phuccuuachau"))
			{
				return "69,145,phuccuuachau";
			}
			if (input.Contains("cuongtrangachau"))
			{
				return "45,115,cuongtrangachau";
			}
			if (input.Contains("tinhtrangachau"))
			{
				return "44,79,tinhtrangachau";
			}
			return "";
		}

		public static string GetInfo(string info)
		{
			info = info.Replace("Vương Đức Phú", "Vương Đức Phúc");
			info = info.Replace("Bách Hiểu Sinh", "Bạch Manh Sinh");
			info = info.Replace("Mộ Dung Chùy", "Mộ Dung Thùy");
			if (info.Contains("Thính Hương Thủy Tạ"))
			{
				return "INFOAIM154,93,284,TuoiNuoc";
			}
			if (info.Contains("Cầm Âm Tiểu Trúc"))
			{
				return "INFOAIM78,142,284,TuoiNuoc";
			}
			if (info.Contains("Sâm Hợp Trang"))
			{
				return "INFOAIM28,28,284,TuoiNuoc";
			}
			if (info.Contains("Mạn Đà Viên"))
			{
				return "INFOAIM119,36,284,TuoiNuoc";
			}
			if (info.Contains("#{SMFB_120214_47}#r#G"))
			{
				return "INFOAIM67,110,284,MoDungThuy,PhuBan";
			}
			if (info.Contains("Túc Thái Âm Tì Kinh Đồng Nhân"))
			{
				return "INFOAIM121,90,13,TuoiNuoc";
			}
			if (info.Contains("Thủ Thái Âm Phế Kinh Đồng Nhân"))
			{
				return "INFOAIM62,90,13,TuoiNuoc";
			}
			if (info.Contains("Túc Dương Minh Vị Kinh Đồng Nhân"))
			{
				return "INFOAIM106,85,13,TuoiNuoc";
			}
			if (info.Contains("#{SMFB_120214_41}#{SMXL_090819_dali}#r#{SMRW_090206_01}"))
			{
				return "INFOAIM35,86,13,BanTuong,PhuBan";
			}
			if (info.Contains("Người đồng thủ dương minh đại trường kinh"))
			{
				return "INFOAIM84,84,13,TuoiNuoc";
			}
			if (info.Contains("Hoàng Thổ Kỳ"))
			{
				return "INFOAIM62,38,11,TuoiNuoc";
			}
			if (info.Contains("Bạch Kim Kỳ"))
			{
				return "INFOAIM65,139,11,TuoiNuoc";
			}
			if (info.Contains("Thanh Mộc Kỳ"))
			{
				return "INFOAIM131,139,11,TuoiNuoc";
			}
			if (info.Contains("Hắc Thủy Kỳ"))
			{
				return "INFOAIM129,55,11,TuoiNuoc";
			}
			if (info.Contains("#{SMFB_120214_51}#{SMXL_090819_mingjiao}#r#{SMRW_090206_01}"))
			{
				return "INFOAIM89,56,11,PhuongLap,PhuBan";
			}
			if (info.Contains("Đang Thanh Họa"))
			{
				return "INFOAIM142,59,14,TuoiNuoc";
			}
			if (info.Contains("Lạn Kha Kỳ"))
			{
				return "INFOAIM136,145,14,TuoiNuoc";
			}
			if (info.Contains("Phụng Hoàng Cầm"))
			{
				return "INFOAIM42,144,14,TuoiNuoc";
			}
			if (info.Contains("Thánh Hiền Thư"))
			{
				return "INFOAIM47,54,14,TuoiNuoc";
			}
			if (info.Contains("#{SMFB_120214_45}#{SMXL_090819_xiaoyao}#r#{SMRW_090206_01}"))
			{
				return "INFOAIM62,68,14,PhungATam,PhuBan";
			}
			if (info.Contains("Kim Điện"))
			{
				return "INFOAIM82,58,12,TuoiNuoc";
			}
			if (info.Contains("Thiên Giới"))
			{
				return "INFOAIM45,87,12,TuoiNuoc";
			}
			if (info.Contains("Hồi Long Đài"))
			{
				return "INFOAIM76,133,12,TuoiNuoc";
			}
			if (info.Contains("Giải Kiếm Trì"))
			{
				return "INFOAIM49,180,12,TuoiNuoc";
			}
			if (info.Contains("#{SMFB_120214_39}#{SMXL_090819_wudang}#r#{SMRW_090206_01}"))
			{
				return "INFOAIM58,73,12,LamLinhTo,PhuBan";
			}
			if (info.Contains("Nham Băng Hộ"))
			{
				return "INFOAIM125,50,17,TuoiNuoc";
			}
			if (info.Contains("Huyền Băng Hộ"))
			{
				return "INFOAIM65,43,17,TuoiNuoc";
			}
			if (info.Contains("Hàn Băng Hộ"))
			{
				return "INFOAIM71,65,17,TuoiNuoc";
			}
			if (info.Contains("Toái Băng Hộ"))
			{
				return "INFOAIM123,89,17,TuoiNuoc";
			}
			if (info.Contains("#{SMFB_120214_43}#{SMXL_090819_tianshan}#r#{SMRW_090206_01}"))
			{
				return "INFOAIM101,44,17,CucKiem,PhuBan";
			}
			if (info.Contains("Chung Lâu"))
			{
				return "INFOAIM81,69,9,TuoiNuoc";
			}
			if (info.Contains("Đại Hùng Bảo Điện"))
			{
				return "INFOAIM96,82,9,TuoiNuoc";
			}
			if (info.Contains("Tàng Kinh Các"))
			{
				return "INFOAIM134,132,9,TuoiNuoc";
			}
			if (info.Contains("GSơn môn"))
			{
				return "INFOAIM90,110,9,TuoiNuoc";
			}
			if (info.Contains("#{SMFB_120214_35}#{SMXL_090819_shaolin}#r#{SMRW_090206_01}"))
			{
				return "INFOAIM61,62,9,HuyenTrung,PhuBan";
			}
			if (info.Contains("Thiên Cơ Phường"))
			{
				return "INFOAIM56,136,615,TuoiNuoc";
			}
			if (info.Contains("Đường Gia Nội Bảo"))
			{
				return "INFOAIM80,46,615,TuoiNuoc";
			}
			if (info.Contains("Diễn Võ Trường"))
			{
				return "INFOAIM48,80,615,TuoiNuoc";
			}
			if (info.Contains("Phong Vũ Lâu"))
			{
				return "INFOAIM102,97,615,TuoiNuoc";
			}
			if (info.Contains("#{TMSM_130808_01}#") && info.Contains("#{SMRW_090206_01}"))
			{
				return "INFOAIM66,30,615,DuongNhacThien,Phuban";
			}
			if (info.Contains("Phật Quang Phụng Hoàng"))
			{
				return "INFOAIM39,152,15,TuoiNuoc";
			}
			if (info.Contains("Kim Đỉnh Phụng Hoàng"))
			{
				return "INFOAIM45,42,15,TuoiNuoc";
			}
			if (info.Contains("Linh Tuyền Phụng Hoàng"))
			{
				return "INFOAIM146,46,15,TuoiNuoc";
			}
			if (info.Contains("Vạn Niên Phụng Hoàng"))
			{
				return "INFOAIM146,156,15,TuoiNuoc";
			}
			if (info == "#{SMFB_120214_37}#{SMXL_090819_emei}#r#{SMRW_090206_01}")
			{
				return "INFOAIM96,73,15,ManhThanhThanh,PhuBan";
			}
			if (info.Contains("Đỗ khang từ"))
			{
				return "INFOAIM131,112,10,TuoiNuoc";
			}
			if (info.Contains("Tiểu đào viên"))
			{
				return "INFOAIM39,147,10,TuoiNuoc";
			}
			if (info.Contains("Diễn binh đàn"))
			{
				return "INFOAIM46,36,10,TuoiNuoc";
			}
			if (info.Contains("Tây sương phòng"))
			{
				return "INFOAIM53,88,10,TuoiNuoc";
			}
			if (info == "#{SMFB_120214_49}#{SMXL_090819_gaibang}#r#{SMRW_090206_01}")
			{
				return "INFOAIM41,144,10,PhatAn,PhuBan";
			}
			if (info.Contains("Rương Rết Độc"))
			{
				return "INFOAIM87,98,16,TuoiNuoc";
			}
			if (info.Contains("Rương Bọ Cạp Độc"))
			{
				return "INFOAIM127,73,16,TuoiNuoc";
			}
			if (info.Contains("Rương Nhện Độc"))
			{
				return "INFOAIM106,98,16,TuoiNuoc";
			}
			if (info.Contains("Rương Cóc Độc"))
			{
				return "INFOAIM95,56,16,TuoiNuoc";
			}
			if (info.Contains("#{SMFB_120214_33}#{SMXL_090819_xingxiu}#r#{SMRW_090206_01}"))
			{
				return "INFOAIM128,78,16,HongNgoc,PhuBan";
			}
			return info;
		}

		public static int ParseInt(string input)
		{
			int result = 0;
			string text = string.Empty;
			for (int i = 0; i < input.Length; i++)
			{
				if (char.IsDigit(input[i]))
				{
					text += input[i];
				}
				else if (text.Length > 0)
				{
					break;
				}
			}
			if (text.Length > 0)
			{
				result = int.Parse(text);
			}
			return result;
		}

		public static string ReplaceFirst(string text, string search, string replace)
		{
			int num = text.IndexOf(search);
			if (num < 0)
			{
				return text;
			}
			return text.Substring(0, num) + replace + text.Substring(num + search.Length);
		}

		public static void Unlock(string fileName)
		{
			Process[] processes = Process.GetProcesses();
			foreach (Process process in processes)
			{
				try
				{
					if (process.MainModule.FileName == fileName)
					{
						process.Kill();
					}
				}
				catch
				{
				}
			}
		}

		public static string LocalIPAddress()
		{
			string result = "";
			IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
			foreach (IPAddress iPAddress in addressList)
			{
				if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
				{
					result = iPAddress.ToString();
					break;
				}
			}
			return result;
		}

		public static float GetDistance(float fromX, float fromY, float toX, float toY)
		{
			return (float)Math.Sqrt(Math.Pow(fromX - toX, 2.0) + Math.Pow(fromY - toY, 2.0));
		}

		public static int NumDiff(int num1, int num2)
		{
			int num3 = num1 - num2;
			if (num3 < 0)
			{
				num3 = -num3;
			}
			return num3;
		}

		public static void FileInstall(string defaltNamespace, string resourceName, string destinationPath)
		{
			Directory.CreateDirectory(destinationPath + "\\..");
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(defaltNamespace + "." + resourceName);
			FileStream fileStream = new FileStream(destinationPath, FileMode.Create);
			for (int i = 0; i < manifestResourceStream.Length; i++)
			{
				fileStream.WriteByte((byte)manifestResourceStream.ReadByte());
			}
			fileStream.Close();
		}

		public static void FileInstallMaHoa(string defaltNamespace, string resourceName, string destinationPath)
		{
			using (StreamReader streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(defaltNamespace + "." + resourceName)))
			{
				LoadFile.WriteFileWithEncrypt(streamReader.ReadToEnd(), destinationPath);
			}
		}

		public static string ClearString(string str)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (char c in str)
			{
				bool flag = false;
				string[] array = vietnameseSigns;
				foreach (string text in array)
				{
					int num = 0;
					if (num < text.Length && text[num] == c)
					{
						flag = true;
					}
					if (flag)
					{
						break;
					}
				}
				if (flag)
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public static string RemoveNumber(string input)
		{
			return Regex.Replace(input, "[\\d-]", string.Empty);
		}

		public static string VietLien(string str)
		{
			if (str == null)
			{
				return "";
			}
			return ClearSign(str).Replace(" ", "").Replace("\r", "").ToLower();
		}

		public static string VietLienRemoveNum(string str)
		{
			if (str == null)
			{
				return "";
			}
			return RemoveNumber(ClearSign(str).Replace(" ", "").Replace("\r", "").ToLower());
		}

		public static bool Contain(string input, string pattern)
		{
			if (pattern != null && !(pattern == ""))
			{
				return VietLien(input).Contains(VietLien(pattern));
			}
			return false;
		}

		public static string ClearSign(string str)
		{
			for (int i = 1; i < vietnameseSigns.Length; i++)
			{
				for (int j = 0; j < vietnameseSigns[i].Length; j++)
				{
					str = str.Replace(vietnameseSigns[i][j], vietnameseSigns[0][i - 1]);
				}
			}
			return str;
		}

		public static string ReadFile(string name)
		{
			if (!File.Exists(name))
			{
				return "";
			}
			try
			{
				StreamReader streamReader = new StreamReader(name);
				string result = streamReader.ReadToEnd();
				streamReader.Close();
				return result;
			}
			catch
			{
			}
			return "";
		}

		public static void AppendFile(string name, string content)
		{
			try
			{
				content = content + "\r\n" + ReadFile(name);
				if (content.Split('\n').Length > 1000)
				{
					string text = "";
					for (int i = 0; i < 1000; i++)
					{
						text = text + content.Split('\n')[i].Trim() + "\r\n";
					}
					content = text;
				}
				new FileStream(name, FileMode.Create).Close();
				StreamWriter streamWriter = new StreamWriter(name);
				streamWriter.Write(content);
				streamWriter.Close();
			}
			catch
			{
			}
		}

		public static void WriteFile(string name, string content)
		{
			try
			{
				new FileStream(name, FileMode.Create).Close();
				StreamWriter streamWriter = new StreamWriter(name);
				streamWriter.Write(content);
				streamWriter.Close();
			}
			catch
			{
			}
		}

		public static void CreateFile(string name, string content)
		{
			try
			{
				UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
				new FileStream(name, FileMode.Create).Close();
				StreamWriter streamWriter = new StreamWriter(name, append: false, encoding);
				streamWriter.Write(content);
				streamWriter.Close();
			}
			catch
			{
			}
		}

		public static bool IsPhoneNumber(string number)
		{
			if (!Regex.Match(number, "^[0]([0-9]{9})$").Success)
			{
				return Regex.Match(number, "^[0]([0-9]{10})$").Success;
			}
			return Regex.Match(number, "^[0]([0-9]{9})$").Success;
		}

		public static bool IsValidEmail(string email)
		{
			string pattern = "^(([^<>()[\\]\\\\.,;:\\s@\\\"]+(\\.[^<>()[\\]\\\\.,;:\\s@\\\"]+)*)|(\\\".+\\\"))@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\])|(([a-zA-Z\\-0-9]+\\.)+[a-zA-Z]{2,}))$";
			if (email != null && email != "")
			{
				return Regex.IsMatch(email, pattern);
			}
			return false;
		}

		public static bool IsPressed(VirtualKeyStates key)
		{
			int num = 32768;
			return Convert.ToBoolean(GetKeyState(key) & num);
		}

		[DllImport("user32.dll")]
		private static extern short GetKeyState(VirtualKeyStates nVirtKey);

		[DllImport("user32.dll")]
		public static extern bool CloseWindow(IntPtr hWnd);

		private void UnZip(string file, string unZipTo)
		{
		}

		public static string HKLM_GetString(string path, string key)
		{
			try
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(path);
				if (registryKey == null)
				{
					return "";
				}
				return (string)registryKey.GetValue(key);
			}
			catch
			{
				return "";
			}
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

		public static string FriendlyName()
		{
			string text = HKLM_GetString("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "ProductName");
			string text2 = HKLM_GetString("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "CSDVersion");
			if (text != "")
			{
				return (text.StartsWith("Microsoft") ? "" : "Microsoft ") + text + ((text2 != "") ? (" " + text2) : "");
			}
			return "";
		}
	}
}
