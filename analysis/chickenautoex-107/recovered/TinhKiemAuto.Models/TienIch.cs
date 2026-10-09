using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;

namespace TinhKiemAuto.Models
{
	internal class TienIch
	{
		public static int CountMSG = 0;

		public static string domain = "http://update.chickenauto.com/";

		public static string LangName { get; set; }

		public static string ServerName { get; set; }

		public static int ServerID { get; set; }

		public static int GameCount()
		{
			return FrmMain.GameCount;
		}

		public static int GetTime()
		{
			return (int)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
		}

		public static string GetMD5(string input)
		{
			string text = "";
			try
			{
				MD5 mD = MD5.Create();
				byte[] bytes = Encoding.ASCII.GetBytes(input);
				byte[] array = mD.ComputeHash(bytes);
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("x2"));
				}
				return stringBuilder.ToString();
			}
			catch
			{
				return "UnknowMD5";
			}
		}

		public static string Genkey(string time)
		{
			return GetMD5("[REDACTED]" /* analysis redaction */ + time);
		}

		public static bool IsMessengerBox(int MapID, int x, int y)
		{
			bool result = false;
			switch (MapID)
			{
			case 19:
				if (x == 139 && y == 259)
				{
					result = true;
				}
				break;
			case 246:
				if (x == 21 && y == 143)
				{
					result = true;
				}
				break;
			case 244:
				if (x == 26 && y == 103)
				{
					result = true;
				}
				break;
			case 245:
				if (x == 72 && y == 144)
				{
					result = true;
				}
				break;
			case 249:
				if (x == 20 && y == 210)
				{
					result = true;
				}
				break;
			}
			return result;
		}

		public List<Point> GetDanhSach()
		{
			List<Point> list = new List<Point>();
			Point item = new Point(20, 220);
			list.Add(item);
			return list;
		}

		public static int GetFakeMapID(int MapID)
		{
			int num = 0;
			switch (MapID)
			{
			case 112:
				return 39;
			case 164:
				return 121;
			case 166:
				return 123;
			case 169:
				return 126;
			case 191:
				return 148;
			case 192:
				return 149;
			case 193:
				return 150;
			case 199:
				return 156;
			case 200:
				return 157;
			case 201:
				return 158;
			case 202:
				return 159;
			case 203:
				return 160;
			case 204:
				return 161;
			case 205:
				return 162;
			case 206:
				return 163;
			case 207:
				return 164;
			case 208:
				return 165;
			case 209:
				return 166;
			case 210:
				return 167;
			case 211:
				return 168;
			case 213:
				return 170;
			case 214:
				return 171;
			case 215:
				return 172;
			case 216:
				return 173;
			case 217:
				return 174;
			case 218:
				return 175;
			case 219:
				return 176;
			case 220:
				return 177;
			case 221:
				return 178;
			case 222:
				return 179;
			case 246:
				return 186;
			case 229:
				return 188;
			case 262:
				return 400;
			case 263:
				return 401;
			case 264:
				return 402;
			case 235:
				return 415;
			case 260:
				return 420;
			case 244:
				return 423;
			case 245:
				return 424;
			case 247:
				return 425;
			case 253:
				return 427;
			case 249:
				return 431;
			case 255:
				return 432;
			case 284:
				return 435;
			case 237:
				return 517;
			case 251:
				return 519;
			case 252:
				return 520;
			default:
				return MapID;
			}
		}

		public static string Base64Encode(string plainText)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
		}

		public static string Base64Decode(string base64EncodedData)
		{
			byte[] bytes = Convert.FromBase64String(base64EncodedData);
			return Encoding.UTF8.GetString(bytes);
		}
	}
}
