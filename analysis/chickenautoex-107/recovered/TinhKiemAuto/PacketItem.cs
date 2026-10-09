using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TinhKiemAuto
{
	public class PacketItem
	{
		private Game game;

		public int Address;

		public int Class;

		public int PacketId;

		public string Name;

		public int Count;

		public int Index;

		public string TypeName;

		public int Star;

		public int Line;

		public string Type;

		public int MapId = -1;

		public int X;

		public int Y;

		public int InfoBase;

		public string ClearName => TINHKIEM.VietLien(Name);

		public int DiemAddress => game.Memory.Read(Address + 20) + 144;

		public bool HaveTheLuc => DiemType[51] == '1';

		public string DiemType
		{
			get
			{
				string text = "";
				for (int i = 0; i < 2; i++)
				{
					int num = ((game.Address.GameType != 1) ? game.Memory.Read(game.Memory.Read(Address + 20) + 112 + i * 4) : game.Memory.Read(game.Memory.Read(Address + 20) + 84 + i * 4));
					string text2 = Convert.ToString(num, 2);
					while (text2.Length < 32)
					{
						text2 = "0" + text2;
					}
					text += text2;
				}
				return text;
			}
		}

		public bool IsHaveLongVan
		{
			get
			{
				string text = game.Memory.ReadString(game.Memory.Read(Address + 20) + 16);
				if (text.Replace(game.TLBB.Name, "").Contains("*") || text.Replace(game.TLBB.Name, "").Contains("~"))
				{
					return true;
				}
				return false;
			}
		}

		public int TheLuc
		{
			get
			{
				int num = 0;
				for (int i = 0; i < 32; i++)
				{
					if (DiemType[i] == '1')
					{
						num++;
					}
				}
				for (int j = 52; j < 64; j++)
				{
					if (DiemType[j] == '1')
					{
						num++;
					}
				}
				if (!HaveTheLuc)
				{
					return 0;
				}
				if (game.Address.GameType == 1)
				{
					return game.Memory.Read2Byte(game.Memory.Read(Address + 20) + (110 + num * 2));
				}
				return game.Memory.Read2Byte(game.Memory.Read(Address + 20) + (138 + num * 2));
			}
		}

		public int Diem
		{
			get
			{
				int num = 0;
				for (int i = 0; i < 4; i++)
				{
					num = game.Memory.Read2Byte(game.Memory.Read(Address + 20) + (144 + i * 2));
					if (num > 0 && num < 200)
					{
						return num;
					}
				}
				return num;
			}
		}

		public int Diem1
		{
			get
			{
				int num = game.Memory.Read2Byte(game.Memory.Read(Address + 20), 144);
				if (num >= 120)
				{
					return 0;
				}
				return num;
			}
		}

		public int Diem2
		{
			get
			{
				int num = game.Memory.Read2Byte(game.Memory.Read(Address + 20), 146);
				if (num >= 120)
				{
					return 0;
				}
				return num;
			}
		}

		public int Diem3
		{
			get
			{
				int num = game.Memory.Read2Byte(game.Memory.Read(Address + 20), 148);
				if (num >= 120)
				{
					return 0;
				}
				return num;
			}
		}

		public bool IsHaveNgoc
		{
			get
			{
				bool result = false;
				if ((ReadNgocID1() >= 50101001 && ReadNgocID1() <= 50921409) || (ReadNgocID2() >= 50101001 && ReadNgocID2() <= 50921409) || (ReadNgocID3() >= 50101001 && ReadNgocID3() <= 50921409) || (ReadNgocID4() >= 50101001 && ReadNgocID4() <= 50921409))
				{
					result = true;
				}
				return result;
			}
		}

		public string Info
		{
			get
			{
				string empty = string.Empty;
				empty = empty + "Address: " + Address.ToString("X8");
				empty += "\r\n";
				empty = empty + "Class: " + Class.ToString("X8");
				empty += "\r\n";
				empty = empty + "Num: " + Count;
				empty += "\r\n";
				empty = empty + "Index: " + Index;
				empty += "\r\n";
				empty = empty + "TypeName: " + TypeName;
				empty += "\r\n";
				empty = empty + "Star: " + Star;
				empty += "\r\n";
				empty = empty + "star address: " + game.Memory.ReadAddress(Address + 20, 78);
				empty += "\r\n";
				empty = empty + "Line: " + Line;
				empty += "\r\n";
				empty = empty + "Type: " + Type;
				empty += "\r\n";
				empty = empty + "MapId: " + MapId;
				empty += "\r\n";
				empty = empty + "XY: " + X + "," + Y;
				empty += "\r\n";
				empty = empty + "InfoAddress: " + game.Memory.Read(Address + 20).ToString("X8");
				empty += "\r\n";
				empty = empty + "DieuVanAddress: " + (game.Memory.Read(Address + 20) + 16).ToString("X8");
				empty += "\r\n";
				empty = empty + "StringDieuVan: " + game.Memory.ReadString(game.Memory.Read(Address + 20) + 16);
				empty += "\r\n";
				empty = empty + "HaveLongVan-DieuVan: " + IsHaveLongVan;
				empty += "\r\n";
				empty = empty + "NGOCID1: " + ReadNgocID1();
				empty += "\r\n";
				empty = empty + "NGOCID2: " + ReadNgocID2();
				empty += "\r\n";
				empty = empty + "NGOCID3: " + ReadNgocID3();
				empty += "\r\n";
				empty = empty + "NGOCID4: " + ReadNgocID4();
				empty += "\r\n";
				empty = empty + "IsHaveNgoc: " + IsHaveNgoc;
				empty += "\r\n";
				empty = empty + "TheLuc: " + Diem;
				empty += "\r\n";
				empty = empty + "DiemType: " + DiemType;
				empty += "\r\n";
				empty = empty + "PacketId: " + PacketId;
				empty += "\r\n";
				empty = empty + "GiamDinh: " + game.Memory.Read(game.Memory.Read(Address + 20) + 13).ToString("X8");
				empty += "\r\n";
				empty = empty + "DiemAddress: " + DiemAddress.ToString("X8");
				empty += "\r\n";
				empty = empty + "HaveTheLuc: " + HaveTheLuc;
				empty += "\r\n";
				empty = empty + "TheLuc: " + TheLuc;
				empty += "\r\n";
				empty = empty + "Lvl: " + Lvl;
				empty += "\r\n";
				empty = empty + "IsCoDinh: " + IsCoDinh;
				empty += "\r\n";
				return empty + "Name: " + Name;
			}
		}

		public int SplitIndex
		{
			get
			{
				return game.Memory.Read(game.Address.PacketItemBase[0], 856396);
			}
			set
			{
				game.Memory.Write(game.Memory.ReadAddress(game.Address.PacketItemBase[0], 856392), 2);
				game.Memory.Write(game.Memory.ReadAddress(game.Address.PacketItemBase[0], 856396), value);
			}
		}

		public int Lvl { get; set; }

		public List<int> GetListIndex
		{
			get
			{
				List<int> list = new List<int>();
				foreach (PacketItem item in DaoCu)
				{
					list.Add(item.Index);
				}
				return list;
			}
		}

		public bool IsCoDinh => game.Memory.Read1Byte(game.Memory.Read(Address + 20) + 13) == 1;

		public List<PacketItem> DaoCu
		{
			get
			{
				List<PacketItem> list = new List<PacketItem>();
				int num = game.Memory.Read(game.Address.PacketItemBase);
				for (int i = 0; i < 30; i++)
				{
					if (game.Memory.Read(num + i * 4) == 0)
					{
						continue;
					}
					PacketItem packetItem = new PacketItem(game);
					packetItem.Address = game.Memory.Read(num + i * 4);
					packetItem.Class = game.Memory.Read(packetItem.Address);
					packetItem.PacketId = game.Memory.Read(packetItem.Address + 4);
					if (packetItem.Class == game.Address.PacketType1 || packetItem.Class == game.Address.PacketType5)
					{
						packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 40));
						packetItem.Count = 1;
						packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 88));
						packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 84));
					}
					else if (packetItem.Class == game.Address.PacketType2)
					{
						packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 24));
						packetItem.Count = game.Memory.Read1Byte(packetItem.Address + 20, 60);
						if (game.Address.GameType == 2)
						{
							packetItem.Count = game.Memory.Read1Byte(packetItem.Address + 20, 88);
						}
						packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 80));
						packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
					}
					else if (packetItem.Class == game.Address.PacketType3)
					{
						packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 28));
						packetItem.Count = 1;
						packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 304));
						packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
					}
					else if (packetItem.Class == game.Address.PacketType4)
					{
						packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 40));
						packetItem.Count = 1;
						packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 76));
						packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
					}
					else
					{
						packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 88));
						packetItem.Count = 1;
						packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 80));
						packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
					}
					if (packetItem.Class == game.Address.PacketType6)
					{
						packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 44));
						packetItem.Count = 1;
						packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 104));
						packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 40));
					}
					packetItem.Index = game.Memory.Read(packetItem.Address + 16);
					if (game.Address.GameType == 1)
					{
						packetItem.InfoBase = 66;
					}
					else
					{
						packetItem.InfoBase = 94;
					}
					packetItem.Line = game.Memory.Read2Byte(packetItem.Address + 20, packetItem.InfoBase);
					packetItem.Star = game.Memory.Read2Byte(packetItem.Address + 20, packetItem.InfoBase + 12);
					if (packetItem.Type == "Cloth2_9" || packetItem.Type.Contains("Charm"))
					{
						if (game.Address.GameType == 1)
						{
							packetItem.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
							{
								packetItem.Address + 20,
								52
							}));
							packetItem.X = game.Memory.Read2Byte(packetItem.Address + 20, 54);
							packetItem.Y = game.Memory.Read2Byte(packetItem.Address + 20, 56);
						}
						else
						{
							packetItem.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
							{
								packetItem.Address + 20,
								80
							}));
							packetItem.X = game.Memory.Read2Byte(packetItem.Address + 20, 82);
							packetItem.Y = game.Memory.Read2Byte(packetItem.Address + 20, 84);
						}
					}
					packetItem.Lvl = game.Memory.Read(packetItem.Address + 40, 44);
					if (!(TINHKIEM.VietLien(packetItem.TypeName) == "daocunhiemvu"))
					{
						list.Add(packetItem);
					}
				}
				return list;
			}
		}

		public List<PacketItem> NguyenLieu
		{
			get
			{
				List<PacketItem> list = new List<PacketItem>();
				int num = game.Memory.Read(game.Address.PacketItemBase);
				for (int i = 30; i < 60; i++)
				{
					if (game.Memory.Read(num + i * 4) == 0)
					{
						continue;
					}
					PacketItem packetItem = new PacketItem(game);
					packetItem.Address = game.Memory.Read(num + i * 4);
					packetItem.Class = game.Memory.Read(packetItem.Address);
					packetItem.PacketId = game.Memory.Read(packetItem.Address + 4);
					if (packetItem.Class == game.Address.PacketType1 || packetItem.Class == game.Address.PacketType5)
					{
						packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 40));
						packetItem.Count = 1;
						packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 88));
						packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 84));
					}
					else if (packetItem.Class == game.Address.PacketType2)
					{
						packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 24));
						packetItem.Count = game.Memory.Read1Byte(packetItem.Address + 20, 60);
						if (game.Address.GameType == 2)
						{
							packetItem.Count = game.Memory.Read1Byte(packetItem.Address + 20, 88);
						}
						packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 80));
						packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
					}
					else if (packetItem.Class == game.Address.PacketType3)
					{
						packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 28));
						packetItem.Count = 1;
						packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 304));
						packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
					}
					else if (packetItem.Class == game.Address.PacketType4)
					{
						packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 40));
						packetItem.Count = 1;
						packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 76));
						packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
					}
					else
					{
						packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 88));
						packetItem.Count = 1;
						packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 80));
						packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
					}
					if (packetItem.Class == game.Address.PacketType6)
					{
						packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 44));
						packetItem.Count = 1;
						packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 104));
						packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 40));
					}
					packetItem.Index = game.Memory.Read(packetItem.Address + 16);
					if (game.Address.GameType == 1)
					{
						packetItem.InfoBase = 66;
					}
					else
					{
						packetItem.InfoBase = 94;
					}
					packetItem.Line = game.Memory.Read2Byte(packetItem.Address + 20, packetItem.InfoBase);
					packetItem.Star = game.Memory.Read2Byte(packetItem.Address + 20, packetItem.InfoBase + 12);
					if (packetItem.Type == "Cloth2_9" || packetItem.Type.Contains("Charm"))
					{
						if (game.Address.GameType == 1)
						{
							packetItem.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
							{
								packetItem.Address + 20,
								52
							}));
							packetItem.X = game.Memory.Read2Byte(packetItem.Address + 20, 54);
							packetItem.Y = game.Memory.Read2Byte(packetItem.Address + 20, 56);
						}
						else
						{
							packetItem.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
							{
								packetItem.Address + 20,
								80
							}));
							packetItem.X = game.Memory.Read2Byte(packetItem.Address + 20, 82);
							packetItem.Y = game.Memory.Read2Byte(packetItem.Address + 20, 84);
						}
					}
					packetItem.Lvl = game.Memory.Read(packetItem.Address + 40, 44);
					if (!(TINHKIEM.VietLien(packetItem.TypeName) == "daocunhiemvu"))
					{
						list.Add(packetItem);
					}
				}
				return list;
			}
		}

		public int GetNumberFromString
		{
			get
			{
				int result = 0;
				try
				{
					result = int.Parse(Regex.Match(Name, "\\d+").Value);
				}
				catch
				{
				}
				return result;
			}
		}

		public PacketItem(Game game)
		{
			this.game = game;
		}

		public int ReadNgocID1()
		{
			return game.Memory.Read(game.Memory.Read(Address + 20) + 120);
		}

		public int ReadNgocID2()
		{
			return game.Memory.Read(game.Memory.Read(Address + 20) + 124);
		}

		public int ReadNgocID3()
		{
			return game.Memory.Read(game.Memory.Read(Address + 20) + 128);
		}

		public int ReadNgocID4()
		{
			return game.Memory.Read(game.Memory.Read(Address + 20) + 132);
		}

		public void GiamDinh()
		{
			if (game.Memory.Read(game.Memory.Read(Address + 20) + 13).ToString("X8").EndsWith("000010") || game.Memory.Read(game.Memory.Read(Address + 20) + 13).ToString("X8").EndsWith("000011"))
			{
				game.Memory.Write(game.Memory.Read(Address + 20) + 13, 50);
			}
		}

		public static List<PacketItem> EnumTrangBi(Game game)
		{
			List<PacketItem> list = new List<PacketItem>();
			int num = game.Memory.Read(new int[2]
			{
				game.Address.HaveRide1[0],
				game.Address.HaveRide1[1]
			});
			for (int i = 0; i < 80; i++)
			{
				if (game.Memory.Read(num + i * 4) == 0)
				{
					continue;
				}
				PacketItem packetItem = new PacketItem(game);
				packetItem.Address = game.Memory.Read(num + i * 4);
				packetItem.Class = game.Memory.Read(packetItem.Address);
				packetItem.PacketId = game.Memory.Read(packetItem.Address + 4);
				if (packetItem.Class == game.Address.PacketType1 || packetItem.Class == game.Address.PacketType5)
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 40));
					packetItem.Count = 1;
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 88));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 84));
				}
				else if (packetItem.Class == game.Address.PacketType2)
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 24));
					packetItem.Count = game.Memory.Read1Byte(packetItem.Address + 20, 60);
					if (game.Address.GameType == 2)
					{
						packetItem.Count = game.Memory.Read1Byte(packetItem.Address + 20, 88);
					}
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 80));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
				}
				else if (packetItem.Class == game.Address.PacketType3)
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 28));
					packetItem.Count = 1;
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 304));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
				}
				else if (packetItem.Class == game.Address.PacketType4)
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 40));
					packetItem.Count = 1;
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 76));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
				}
				else
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 88));
					packetItem.Count = 1;
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 80));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
				}
				if (packetItem.Class == game.Address.PacketType6)
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 44));
					packetItem.Count = 1;
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 104));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 40));
				}
				packetItem.Index = game.Memory.Read(packetItem.Address + 16);
				if (game.Address.GameType == 1)
				{
					packetItem.InfoBase = 66;
				}
				else
				{
					packetItem.InfoBase = 94;
				}
				packetItem.Line = game.Memory.Read2Byte(packetItem.Address + 20, packetItem.InfoBase);
				packetItem.Star = game.Memory.Read2Byte(packetItem.Address + 20, packetItem.InfoBase + 12);
				if (packetItem.Type == "Cloth2_9" || packetItem.Type.Contains("Charm"))
				{
					if (game.Address.GameType == 1)
					{
						packetItem.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
						{
							packetItem.Address + 20,
							52
						}));
						packetItem.X = game.Memory.Read2Byte(packetItem.Address + 20, 54);
						packetItem.Y = game.Memory.Read2Byte(packetItem.Address + 20, 56);
					}
					else
					{
						packetItem.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
						{
							packetItem.Address + 20,
							80
						}));
						packetItem.X = game.Memory.Read2Byte(packetItem.Address + 20, 82);
						packetItem.Y = game.Memory.Read2Byte(packetItem.Address + 20, 84);
					}
				}
				packetItem.Lvl = game.Memory.Read(packetItem.Address + 40, 44);
				list.Add(packetItem);
			}
			return list;
		}

		public void Use(int index)
		{
			game.LuaDoOneLineString("PlayerPackage:UseItem(" + index + ");");
		}

		public static List<PacketItem> Enum(Game game)
		{
			List<PacketItem> list = new List<PacketItem>();
			int num = game.Memory.Read(game.Address.PacketItemBase);
			for (int i = 0; i < 80; i++)
			{
				if (game.Memory.Read(num + i * 4) == 0)
				{
					continue;
				}
				PacketItem packetItem = new PacketItem(game);
				packetItem.Address = game.Memory.Read(num + i * 4);
				packetItem.Class = game.Memory.Read(packetItem.Address);
				packetItem.PacketId = game.Memory.Read(packetItem.Address + 4);
				if (packetItem.Class == game.Address.PacketType1 || packetItem.Class == game.Address.PacketType5)
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 40));
					packetItem.Count = 1;
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 88));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 84));
				}
				else if (packetItem.Class == game.Address.PacketType2)
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 24));
					packetItem.Count = game.Memory.Read1Byte(packetItem.Address + 20, 60);
					if (game.Address.GameType == 2)
					{
						packetItem.Count = game.Memory.Read1Byte(packetItem.Address + 20, 88);
					}
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 80));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
				}
				else if (packetItem.Class == game.Address.PacketType3)
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 28));
					packetItem.Count = 1;
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 304));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
				}
				else if (packetItem.Class == game.Address.PacketType4)
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 40));
					packetItem.Count = 1;
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 76));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
				}
				else
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 88));
					packetItem.Count = 1;
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 80));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
				}
				if (packetItem.Class == game.Address.PacketType6)
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 44));
					packetItem.Count = 1;
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 104));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 40));
				}
				packetItem.Index = game.Memory.Read(packetItem.Address + 16);
				if (game.Address.GameType == 1)
				{
					packetItem.InfoBase = 66;
				}
				else
				{
					packetItem.InfoBase = 94;
				}
				packetItem.Line = game.Memory.Read2Byte(packetItem.Address + 20, packetItem.InfoBase);
				packetItem.Star = game.Memory.Read2Byte(packetItem.Address + 20, packetItem.InfoBase + 12);
				if (packetItem.Type == "Cloth2_9" || packetItem.Type.Contains("Charm"))
				{
					if (game.Address.GameType == 1)
					{
						packetItem.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
						{
							packetItem.Address + 20,
							52
						}));
						packetItem.X = game.Memory.Read2Byte(packetItem.Address + 20, 54);
						packetItem.Y = game.Memory.Read2Byte(packetItem.Address + 20, 56);
					}
					else
					{
						packetItem.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
						{
							packetItem.Address + 20,
							80
						}));
						packetItem.X = game.Memory.Read2Byte(packetItem.Address + 20, 82);
						packetItem.Y = game.Memory.Read2Byte(packetItem.Address + 20, 84);
					}
				}
				packetItem.Lvl = game.Memory.Read(packetItem.Address + 40, 44);
				list.Add(packetItem);
			}
			return list;
		}

		public static List<PacketItem> EnumDrop(Game game)
		{
			List<PacketItem> list = new List<PacketItem>();
			int num = game.Memory.Read(game.Address.PacketItemBase);
			for (int i = 0; i < 80; i++)
			{
				if (game.Memory.Read(num + i * 4) == 0)
				{
					continue;
				}
				PacketItem packetItem = new PacketItem(game);
				packetItem.Address = game.Memory.Read(num + i * 4);
				packetItem.Class = game.Memory.Read(packetItem.Address);
				packetItem.PacketId = game.Memory.Read(packetItem.Address + 4);
				if (packetItem.Class == game.Address.PacketType1 || packetItem.Class == game.Address.PacketType5)
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 40));
					packetItem.Count = 1;
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 88));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 84));
				}
				else if (packetItem.Class == game.Address.PacketType2)
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 24));
					packetItem.Count = game.Memory.Read1Byte(packetItem.Address + 20, 60);
					if (game.Address.GameType == 2)
					{
						packetItem.Count = game.Memory.Read1Byte(packetItem.Address + 20, 88);
					}
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 80));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
				}
				else if (packetItem.Class == game.Address.PacketType3)
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 28));
					packetItem.Count = 1;
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 304));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
				}
				else if (packetItem.Class == game.Address.PacketType4)
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 40));
					packetItem.Count = 1;
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 76));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
				}
				else
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 88));
					packetItem.Count = 1;
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 80));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 20));
				}
				if (packetItem.Class == game.Address.PacketType6)
				{
					packetItem.Name = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 44));
					packetItem.Count = 1;
					packetItem.TypeName = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 104));
					packetItem.Type = game.Memory.ReadString(game.Memory.Read(packetItem.Address + 40, 40));
				}
				packetItem.Index = game.Memory.Read(packetItem.Address + 16);
				if (game.Address.GameType == 1)
				{
					packetItem.InfoBase = 66;
				}
				else
				{
					packetItem.InfoBase = 94;
				}
				packetItem.Line = game.Memory.Read2Byte(packetItem.Address + 20, packetItem.InfoBase);
				packetItem.Star = game.Memory.Read2Byte(packetItem.Address + 20, packetItem.InfoBase + 12);
				if (packetItem.Type == "Cloth2_9" || packetItem.Type.Contains("Charm"))
				{
					if (game.Address.GameType == 1)
					{
						packetItem.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
						{
							packetItem.Address + 20,
							52
						}));
						packetItem.X = game.Memory.Read2Byte(packetItem.Address + 20, 54);
						packetItem.Y = game.Memory.Read2Byte(packetItem.Address + 20, 56);
					}
					else
					{
						packetItem.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
						{
							packetItem.Address + 20,
							80
						}));
						packetItem.X = game.Memory.Read2Byte(packetItem.Address + 20, 82);
						packetItem.Y = game.Memory.Read2Byte(packetItem.Address + 20, 84);
					}
				}
				packetItem.Lvl = game.Memory.Read(packetItem.Address + 40, 44);
				if (!(TINHKIEM.VietLien(packetItem.TypeName) == "daocunhiemvu"))
				{
					list.Add(packetItem);
				}
			}
			return list;
		}
	}
}
