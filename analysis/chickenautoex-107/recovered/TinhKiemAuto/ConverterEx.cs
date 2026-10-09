using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TinhKiemAuto
{
	internal class ConverterEx
	{
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

		public static readonly string[] UnicodesStr = new string[256]
		{
			"\\u0000", "\\u0001", "\\u1EB2", "\\u0003", "\\u0004", "\\u1EB4", "\\u1EAA", "\\u0007", "\\u0008", "\\u0009",
			"\\u000A", "\\u000B", "\\u000C", "\\u000D", "\\u000E", "\\u000F", "\\u0010", "\\u0011", "\\u0012", "\\u0013",
			"\\u1EF6", "\\u0015", "\\u0016", "\\u0017", "\\u0018", "\\u1EF8", "\\u001A", "\\u001B", "\\u001C", "\\u001D",
			"\\u1EF4", "\\u001F", "\\u0020", "\\u0021", "\\u0022", "\\u0023", "\\u0024", "\\u0025", "\\u0026", "\\u0027",
			"\\u0028", "\\u0029", "\\u002A", "\\u002B", "\\u002C", "\\u002D", "\\u002E", "\\u002F", "\\u0030", "\\u0031",
			"\\u0032", "\\u0033", "\\u0034", "\\u0035", "\\u0036", "\\u0037", "\\u0038", "\\u0039", "\\u003A", "\\u003B",
			"\\u003C", "\\u003D", "\\u003E", "\\u003F", "\\u0040", "\\u0041", "\\u0042", "\\u0043", "\\u0044", "\\u0045",
			"\\u0046", "\\u0047", "\\u0048", "\\u0049", "\\u004A", "\\u004B", "\\u004C", "\\u004D", "\\u004E", "\\u004F",
			"\\u0050", "\\u0051", "\\u0052", "\\u0053", "\\u0054", "\\u0055", "\\u0056", "\\u0057", "\\u0058", "\\u0059",
			"\\u005A", "\\u005B", "\\u005C", "\\u005D", "\\u005E", "\\u005F", "\\u0060", "\\u0061", "\\u0062", "\\u0063",
			"\\u0064", "\\u0065", "\\u0066", "\\u0067", "\\u0068", "\\u0069", "\\u006A", "\\u006B", "\\u006C", "\\u006D",
			"\\u006E", "\\u006F", "\\u0070", "\\u0071", "\\u0072", "\\u0073", "\\u0074", "\\u0075", "\\u0076", "\\u0077",
			"\\u0078", "\\u0079", "\\u007A", "\\u007B", "\\u007C", "\\u007D", "\\u007E", "\\u007F", "\\u1EA0", "\\u1EAE",
			"\\u1EB0", "\\u1EB6", "\\u1EA4", "\\u1EA6", "\\u1EA8", "\\u1EAC", "\\u1EBC", "\\u1EB8", "\\u1EBE", "\\u1EC0",
			"\\u1EC2", "\\u1EC4", "\\u1EC6", "\\u1ED0", "\\u1ED2", "\\u1ED4", "\\u1ED6", "\\u1ED8", "\\u1EE2", "\\u1EDA",
			"\\u1EDC", "\\u1EDE", "\\u1ECA", "\\u1ECE", "\\u1ECC", "\\u1EC8", "\\u1EE6", "\\u0168", "\\u1EE4", "\\u1EF2",
			"\\u00D5", "\\u1EAF", "\\u1EB1", "\\u1EB7", "\\u1EA5", "\\u1EA7", "\\u1EA9", "\\u1EAD", "\\u1EBD", "\\u1EB9",
			"\\u1EBF", "\\u1EC1", "\\u1EC3", "\\u1EC5", "\\u1EC7", "\\u1ED1", "\\u1ED3", "\\u1ED5", "\\u1ED7", "\\u1EE0",
			"\\u01A0", "\\u1ED9", "\\u1EDD", "\\u1EDF", "\\u1ECB", "\\u1EF0", "\\u1EE8", "\\u1EEA", "\\u1EEC", "\\u01A1",
			"\\u1EDB", "\\u01AF", "\\u00C0", "\\u00C1", "\\u00C2", "\\u00C3", "\\u1EA2", "\\u0102", "\\u1EB3", "\\u1EB5",
			"\\u00C8", "\\u00C9", "\\u00CA", "\\u1EBA", "\\u00CC", "\\u00CD", "\\u0128", "\\u1EF3", "\\u0110", "\\u1EE9",
			"\\u00D2", "\\u00D3", "\\u00D4", "\\u1EA1", "\\u1EF7", "\\u1EEB", "\\u1EED", "\\u00D9", "\\u00DA", "\\u1EF9",
			"\\u1EF5", "\\u00DD", "\\u1EE1", "\\u01B0", "\\u00E0", "\\u00E1", "\\u00E2", "\\u00E3", "\\u1EA3", "\\u0103",
			"\\u1EEF", "\\u1EAB", "\\u00E8", "\\u00E9", "\\u00EA", "\\u1EBB", "\\u00EC", "\\u00ED", "\\u0129", "\\u1EC9",
			"\\u0111", "\\u1EF1", "\\u00F2", "\\u00F3", "\\u00F4", "\\u00F5", "\\u1ECF", "\\u1ECD", "\\u1EE5", "\\u00F9",
			"\\u00FA", "\\u0169", "\\u1EE7", "\\u00FD", "\\u1EE3", "\\u1EEE"
		};

		public static string CleanJarVar(string input)
		{
			return "";
		}

		public static bool HasSpecialChars(string yourString)
		{
			return !Regex.IsMatch(yourString, "^[a-zA-Z0-9]+$");
		}

		public static float GetDistance(float fromX, float fromY, float toX, float toY)
		{
			return (float)Math.Sqrt(Math.Pow(fromX - toX, 2.0) + Math.Pow(fromY - toY, 2.0));
		}

		public static string ConvertStringToHex(string asciiString)
		{
			string text = "";
			for (int i = 0; i < asciiString.Length; i++)
			{
				text += $"{Convert.ToUInt32(((int)asciiString[i]).ToString()):x2}";
			}
			return text;
		}

		public static string ConvertHexToString(string HexValue)
		{
			string text = "";
			while (HexValue.Length > 0)
			{
				text += Convert.ToChar(Convert.ToUInt32(HexValue.Substring(0, 2), 16));
				HexValue = HexValue.Substring(2, HexValue.Length - 2);
			}
			return text;
		}

		public static string Hex2Bin(string hex)
		{
			return "";
		}

		public static string Hex2String(string input)
		{
			input = ReplaceInsensitive(input, "\\x20", " ");
			input = ReplaceInsensitive(input, "\\x21", "!");
			input = ReplaceInsensitive(input, "\\x22", "\"");
			input = ReplaceInsensitive(input, "\\x23", "#");
			input = ReplaceInsensitive(input, "\\x24", "$");
			input = ReplaceInsensitive(input, "\\x25", "%");
			input = ReplaceInsensitive(input, "\\x26", "&");
			input = ReplaceInsensitive(input, "\\x27", "'");
			input = ReplaceInsensitive(input, "\\x28", "(");
			input = ReplaceInsensitive(input, "\\x29", ")");
			input = ReplaceInsensitive(input, "\\x2A", "*");
			input = ReplaceInsensitive(input, "\\x2B", "+");
			input = ReplaceInsensitive(input, "\\x2C", ",");
			input = ReplaceInsensitive(input, "\\x2D", "-");
			input = ReplaceInsensitive(input, "\\x2E", ".");
			input = ReplaceInsensitive(input, "\\x2F", "/");
			input = ReplaceInsensitive(input, "\\x30", "0");
			input = ReplaceInsensitive(input, "\\x31", "1");
			input = ReplaceInsensitive(input, "\\x32", "2");
			input = ReplaceInsensitive(input, "\\x33", "3");
			input = ReplaceInsensitive(input, "\\x34", "4");
			input = ReplaceInsensitive(input, "\\x35", "5");
			input = ReplaceInsensitive(input, "\\x36", "6");
			input = ReplaceInsensitive(input, "\\x37", "7");
			input = ReplaceInsensitive(input, "\\x38", "8");
			input = ReplaceInsensitive(input, "\\x39", "9");
			input = ReplaceInsensitive(input, "\\x3A", ":");
			input = ReplaceInsensitive(input, "\\x3B", ";");
			input = ReplaceInsensitive(input, "\\x3C", "<");
			input = ReplaceInsensitive(input, "\\x3D", "=");
			input = ReplaceInsensitive(input, "\\x3E", ">");
			input = ReplaceInsensitive(input, "\\x3F", "?");
			input = ReplaceInsensitive(input, "\\x40", "@");
			input = ReplaceInsensitive(input, "\\x41", "A");
			input = ReplaceInsensitive(input, "\\x42", "B");
			input = ReplaceInsensitive(input, "\\x43", "C");
			input = ReplaceInsensitive(input, "\\x44", "D");
			input = ReplaceInsensitive(input, "\\x45", "E");
			input = ReplaceInsensitive(input, "\\x46", "F");
			input = ReplaceInsensitive(input, "\\x47", "G");
			input = ReplaceInsensitive(input, "\\x48", "H");
			input = ReplaceInsensitive(input, "\\x49", "I");
			input = ReplaceInsensitive(input, "\\x4A", "J");
			input = ReplaceInsensitive(input, "\\x4B", "K");
			input = ReplaceInsensitive(input, "\\x4C", "L");
			input = ReplaceInsensitive(input, "\\x4D", "M");
			input = ReplaceInsensitive(input, "\\x4E", "N");
			input = ReplaceInsensitive(input, "\\x4F", "O");
			input = ReplaceInsensitive(input, "\\x50", "P");
			input = ReplaceInsensitive(input, "\\x51", "Q");
			input = ReplaceInsensitive(input, "\\x52", "R");
			input = ReplaceInsensitive(input, "\\x53", "S");
			input = ReplaceInsensitive(input, "\\x54", "T");
			input = ReplaceInsensitive(input, "\\x55", "U");
			input = ReplaceInsensitive(input, "\\x56", "V");
			input = ReplaceInsensitive(input, "\\x57", "W");
			input = ReplaceInsensitive(input, "\\x58", "X");
			input = ReplaceInsensitive(input, "\\x59", "Y");
			input = ReplaceInsensitive(input, "\\x5A", "Z");
			input = ReplaceInsensitive(input, "\\x5B", "[");
			input = ReplaceInsensitive(input, "\\x5C", "\\");
			input = ReplaceInsensitive(input, "\\x5D", "]");
			input = ReplaceInsensitive(input, "\\x5E", "^");
			input = ReplaceInsensitive(input, "\\x5F", "_");
			input = ReplaceInsensitive(input, "\\x60", "`");
			input = ReplaceInsensitive(input, "\\x61", "a");
			input = ReplaceInsensitive(input, "\\x62", "b");
			input = ReplaceInsensitive(input, "\\x63", "c");
			input = ReplaceInsensitive(input, "\\x64", "d");
			input = ReplaceInsensitive(input, "\\x65", "e");
			input = ReplaceInsensitive(input, "\\x66", "f");
			input = ReplaceInsensitive(input, "\\x67", "g");
			input = ReplaceInsensitive(input, "\\x68", "h");
			input = ReplaceInsensitive(input, "\\x69", "i");
			input = ReplaceInsensitive(input, "\\x6A", "j");
			input = ReplaceInsensitive(input, "\\x6B", "k");
			input = ReplaceInsensitive(input, "\\x6C", "l");
			input = ReplaceInsensitive(input, "\\x6D", "m");
			input = ReplaceInsensitive(input, "\\x6E", "n");
			input = ReplaceInsensitive(input, "\\x6F", "o");
			input = ReplaceInsensitive(input, "\\x70", "p");
			input = ReplaceInsensitive(input, "\\x71", "q");
			input = ReplaceInsensitive(input, "\\x72", "r");
			input = ReplaceInsensitive(input, "\\x73", "s");
			input = ReplaceInsensitive(input, "\\x74", "t");
			input = ReplaceInsensitive(input, "\\x75", "u");
			input = ReplaceInsensitive(input, "\\x76", "v");
			input = ReplaceInsensitive(input, "\\x77", "w");
			input = ReplaceInsensitive(input, "\\x78", "x");
			input = ReplaceInsensitive(input, "\\x79", "y");
			input = ReplaceInsensitive(input, "\\x7A", "z");
			input = ReplaceInsensitive(input, "\\x7B", "{");
			input = ReplaceInsensitive(input, "\\x7C", "|");
			input = ReplaceInsensitive(input, "\\x7D", "}");
			input = ReplaceInsensitive(input, "\\x7E", "~");
			return input;
		}

		public static string ReplaceInsensitive(string str, string from, string to)
		{
			return str.Replace(from, to);
		}

		public static string FormatMoney(int value)
		{
			if (value == 0)
			{
				return "0";
			}
			return $"{value:#,###}";
		}

		public static int Float2Int(float value)
		{
			return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
		}

		public static int[] ToArr(int address, int[] offset)
		{
			int[] array = new int[offset.Length + 1];
			array[0] = address;
			offset.CopyTo(array, 1);
			return array;
		}

		public static int Percent(int min, int max)
		{
			if (max == 0)
			{
				return 0;
			}
			int num = min * 100 / max;
			if (num == 0 && min > 0)
			{
				return 1;
			}
			if (num >= 100)
			{
				return 99;
			}
			return num;
		}

		public static int Bool2Int(bool value)
		{
			if (value)
			{
				return 1;
			}
			return 0;
		}

		public static string String2Hex(string s)
		{
			string text = "";
			for (int i = 0; i < s.Length; i++)
			{
				text += Char2Int(s[i]).ToString("X2");
			}
			return text;
		}

		public static int Char2Int(char c)
		{
			return c;
		}

		public static int Hex2Int(string hex)
		{
			if (hex.Contains("?"))
			{
				return -1;
			}
			if (hex.Contains("#"))
			{
				return 257;
			}
			int result = -1;
			int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
			return result;
		}

		public static int[] Hex2IntArr(string hex)
		{
			hex = hex.Replace(" ", "");
			if (hex.Length % 2 != 0)
			{
				hex += "0";
			}
			int[] array = new int[hex.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Hex2Int(hex.Substring(i * 2, 2));
			}
			return array;
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
					stringBuilder.Append(Unicodes[(uint)c]);
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public static string VISCII2UnicodeEx(byte[] input)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < input.Length; i++)
			{
				char c = (char)input[i];
				if (c != 0)
				{
					if (c < 'Ā')
					{
						stringBuilder.Append(Unicodes[(uint)c]);
					}
					else
					{
						stringBuilder.Append("?");
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static string Unicode2VISCII(string input)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] >= 'ÿ')
				{
					stringBuilder.Append(Unicode2VISCII(input[i]));
				}
				else
				{
					stringBuilder.Append(input[i]);
				}
			}
			return stringBuilder.ToString();
		}

		public static char Unicode2VISCII(char c)
		{
			for (int i = 0; i < 256; i++)
			{
				if (Unicodes[i] == c)
				{
					return (char)i;
				}
			}
			return '?';
		}
	}
}
