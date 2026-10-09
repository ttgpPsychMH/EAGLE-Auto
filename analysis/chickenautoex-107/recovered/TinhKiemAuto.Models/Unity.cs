using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;

namespace TinhKiemAuto.Models
{
	public class Unity
	{
		public static List<NPC> ListTriLieu = new List<NPC>
		{
			NPC.LONGBATHIEN,
			NPC.DOTHANHDANG,
			NPC.BINHSANHAN
		};

		public static List<NPC> VatPhamRac = new List<NPC>
		{
			NPC.VANDIEUDIEU,
			NPC.TONTUVU,
			NPC.TRUONGTHIENTHIEN,
			NPC.DONGHOAKIM
		};

		public static bool DangOMapVutRac(int MapID)
		{
			foreach (NPC item in VatPhamRac)
			{
				if (item.Map == MapID)
				{
					return true;
				}
			}
			return false;
		}

		public static NPC GETNPCTRILIEU(int MapID)
		{
			foreach (NPC item in ListTriLieu)
			{
				if (item.Map == MapID)
				{
					return item;
				}
			}
			return null;
		}

		public static NPC GETNPCVUTRAC(int MapID)
		{
			foreach (NPC item in VatPhamRac)
			{
				if (item.Map == MapID)
				{
					return item;
				}
			}
			return null;
		}

		public static bool DangOMapTriLieuHienTai(int MapID)
		{
			foreach (NPC item in ListTriLieu)
			{
				if (item.Map == MapID)
				{
					return true;
				}
			}
			return false;
		}

		public static List<ServerList> GetDanhSach()
		{
			return new List<ServerList>
			{
				new ServerList
				{
					ServerID = 1,
					ServerName = "Nhất Kiếm"
				},
				new ServerList
				{
					ServerID = 2,
					ServerName = "Tái Chiến"
				},
				new ServerList
				{
					ServerID = 3,
					ServerName = "Nhị Kiếm"
				},
				new ServerList
				{
					ServerID = 4,
					ServerName = "Tam Kiếm"
				},
				new ServerList
				{
					ServerID = 5,
					ServerName = "Tứ Kiếm"
				},
				new ServerList
				{
					ServerID = 6,
					ServerName = "Tiếu Ngạo"
				},
				new ServerList
				{
					ServerID = 7,
					ServerName = "Long Kiếm"
				},
				new ServerList
				{
					ServerID = 9,
					ServerName = "Song Kiếm"
				},
				new ServerList
				{
					ServerID = 10,
					ServerName = "Du Kiếm"
				},
				new ServerList
				{
					ServerID = 11,
					ServerName = "Ảnh Kiếm"
				}
			};
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

		public static bool IsMessengerBox(int MapID, int x, int y)
		{
			bool result = false;
			int num = 0;
			switch (MapID)
			{
			case 19:
				if (x == 139 + num && y == 259 + num)
				{
					result = true;
				}
				break;
			case 1:
				if (x == 65 + num && y == 270 + num)
				{
					result = true;
				}
				break;
			case 246:
				if (x == 21 + num && y == 143 + num)
				{
					result = true;
				}
				break;
			case 247:
				if (x == 104 + num && y == 215 + num)
				{
					result = true;
				}
				break;
			case 244:
				if (x == 29 + num && y == 103 + num)
				{
					result = true;
				}
				break;
			case 245:
				if (x == 72 + num && y == 144 + num)
				{
					result = true;
				}
				break;
			case 249:
				if (x == 20 + num && y == 210 + num)
				{
					result = true;
				}
				break;
			}
			return result;
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

		public static object ByteArrayToObject(byte[] arrBytes)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				memoryStream.Write(arrBytes, 0, arrBytes.Length);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				return binaryFormatter.Deserialize(memoryStream);
			}
		}

		public static byte[] ObjectToByteArray(object obj)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				binaryFormatter.Serialize(memoryStream, obj);
				return memoryStream.ToArray();
			}
		}

		public static bool IsNumeric(object Expression)
		{
			double result;
			return double.TryParse(Convert.ToString(Expression), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out result);
		}

		public static T Deserialize<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json);
		}
	}
}
