using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TinhKiemAuto.Models;

namespace TinhKiemAuto
{
	internal class Setting
	{
		public static IniFile Ini = new IniFile(Global.DataPath + "\\Setting.dat");

		public static IniFile MapIni = new IniFile(Global.DataPath + "\\MapPath.dat");

		public static List<BuffPramenter> BuffValue = new List<BuffPramenter>();

		public static string IdLocDo = string.Empty;

		public static string KoKhaiThac = string.Empty;

		public static string Leader = string.Empty;

		public static string BoQua
		{
			get
			{
				return LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\BoQua.dat");
			}
			set
			{
				LoadFile.WriteFileWithEncrypt(value, Global.DataPath + "\\BoQua.dat");
			}
		}

		public static string DropName
		{
			get
			{
				return LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\DropName.dat");
			}
			set
			{
				LoadFile.WriteFileWithEncrypt(value, Global.DataPath + "\\DropName.dat");
			}
		}

		public static string DropType
		{
			get
			{
				return LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\DropType.dat");
			}
			set
			{
				LoadFile.WriteFileWithEncrypt(value, Global.DataPath + "\\DropType.dat");
			}
		}

		public static string AutoEat
		{
			get
			{
				return LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\AutoEat.dat");
			}
			set
			{
				LoadFile.WriteFileWithEncrypt(value, Global.DataPath + "\\AutoEat.dat");
			}
		}

		public static string SellName
		{
			get
			{
				return LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\SellName.dat");
			}
			set
			{
				LoadFile.WriteFileWithEncrypt(value, Global.DataPath + "\\SellName.dat");
			}
		}

		public static string SellType
		{
			get
			{
				return LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\SellType.dat");
			}
			set
			{
				LoadFile.WriteFileWithEncrypt(value, Global.DataPath + "\\SellType.dat");
			}
		}

		public static void SetValue(string Name, string value)
		{
			LoadFile.WriteFileWithEncrypt(value, Global.DataPath + "\\" + Name + ".dat");
		}

		public static string GetValue(string Name)
		{
			return LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\" + Name + ".dat");
		}

		public static void Write(string section, string key, string value)
		{
			Ini.Write(section, key, value);
		}

		public static string Read(string section, string key)
		{
			return Ini.Read(section, key);
		}

		public static int[] LoadSettingOffline(string name)
		{
			return String2Arr(LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\" + name));
		}

		public static string LoadStringOffline(string name)
		{
			return LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\" + name);
		}

		public static void SaveSettingOffline(string name, string value)
		{
			LoadFile.WriteFileWithEncrypt(value, Global.DataPath + "\\" + name);
		}

		public static void SaveMAP(string name, string value)
		{
			MapIni.Write("MAP", name, value);
		}

		public static string LoadMAP(string name)
		{
			name = MapIni.Read("MAP", name);
			return name;
		}

		public static void SaveWAY(string name, string value)
		{
			Ini.Write("WAY", name, value);
		}

		public static string LoadWAY(string name)
		{
			name = Ini.Read("WAY", name);
			return name;
		}

		public static int[] String2Arr(string str)
		{
			string[] array = str.Split(',');
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = String2Int(array[i]);
			}
			return array2;
		}

		public static int String2Int(string input)
		{
			int result = 0;
			int.TryParse(input, out result);
			return result;
		}

		public static void LoadBUff()
		{
			if (File.Exists(Global.DataPath + "\\BuffNM.dat"))
			{
				try
				{
					BuffValue = JsonConvert.DeserializeObject<List<BuffPramenter>>(LoadFile.LoadFileWithDecrypt(Global.DataPath + "\\BuffNM.dat"));
				}
				catch
				{
					BuffValue = new List<BuffPramenter>();
				}
			}
		}

		public static bool CheckBuff(string PlayID)
		{
			if (BuffValue.Find((BuffPramenter x) => TINHKIEM.VietLien(x.IDnguoichoi) == TINHKIEM.VietLien(PlayID)) != null)
			{
				return true;
			}
			return false;
		}

		public static void SaveBuff()
		{
			LoadFile.WriteFileWithEncrypt(JsonConvert.SerializeObject(BuffValue), Global.DataPath + "\\BuffNM.dat");
		}
	}
}
