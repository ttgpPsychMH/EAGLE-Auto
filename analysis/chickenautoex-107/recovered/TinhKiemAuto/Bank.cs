using System;
using System.Collections.Generic;

namespace TinhKiemAuto
{
	internal class Bank
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
				return empty + "IsCoDinh: " + IsCoDinh;
			}
		}

		public int SplitIndex
		{
			get
			{
				return game.Memory.Read(game.Address.BankBase[0], 856396);
			}
			set
			{
				game.Memory.Write(game.Memory.ReadAddress(game.Address.BankBase[0], 856392), 2);
				game.Memory.Write(game.Memory.ReadAddress(game.Address.BankBase[0], 856396), value);
			}
		}

		public int Lvl { get; set; }

		public bool IsCoDinh => game.Memory.Read1Byte(game.Memory.Read(Address + 20) + 13) == 1;

		public List<Bank> DaoCu
		{
			get
			{
				List<Bank> list = new List<Bank>();
				int num = game.Memory.Read(game.Address.BankBase);
				for (int i = 0; i < 30; i++)
				{
					if (game.Memory.Read(num + i * 4) == 0)
					{
						continue;
					}
					Bank bank = new Bank(game);
					bank.Address = game.Memory.Read(num + i * 4);
					bank.Class = game.Memory.Read(bank.Address);
					bank.PacketId = game.Memory.Read(bank.Address + 4);
					if (bank.Class == game.Address.PacketType1 || bank.Class == game.Address.PacketType5)
					{
						bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 40));
						bank.Count = 1;
						bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 88));
						bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 84));
					}
					else if (bank.Class == game.Address.PacketType2)
					{
						bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 24));
						bank.Count = game.Memory.Read1Byte(bank.Address + 20, 60);
						if (game.Address.GameType == 2)
						{
							bank.Count = game.Memory.Read1Byte(bank.Address + 20, 88);
						}
						bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 80));
						bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
					}
					else if (bank.Class == game.Address.PacketType3)
					{
						bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 28));
						bank.Count = 1;
						bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 304));
						bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
					}
					else if (bank.Class == game.Address.PacketType4)
					{
						bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 40));
						bank.Count = 1;
						bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 76));
						bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
					}
					else
					{
						bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 88));
						bank.Count = 1;
						bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 80));
						bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
					}
					if (bank.Class == game.Address.PacketType6)
					{
						bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 44));
						bank.Count = 1;
						bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 104));
						bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 40));
					}
					bank.Index = game.Memory.Read(bank.Address + 16);
					if (game.Address.GameType == 1)
					{
						bank.InfoBase = 66;
					}
					else
					{
						bank.InfoBase = 94;
					}
					bank.Line = game.Memory.Read2Byte(bank.Address + 20, bank.InfoBase);
					bank.Star = game.Memory.Read2Byte(bank.Address + 20, bank.InfoBase + 12);
					if (bank.Type == "Cloth2_9" || bank.Type.Contains("Charm"))
					{
						if (game.Address.GameType == 1)
						{
							bank.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
							{
								bank.Address + 20,
								52
							}));
							bank.X = game.Memory.Read2Byte(bank.Address + 20, 54);
							bank.Y = game.Memory.Read2Byte(bank.Address + 20, 56);
						}
						else
						{
							bank.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
							{
								bank.Address + 20,
								80
							}));
							bank.X = game.Memory.Read2Byte(bank.Address + 20, 82);
							bank.Y = game.Memory.Read2Byte(bank.Address + 20, 84);
						}
					}
					bank.Lvl = game.Memory.Read(bank.Address + 40, 44);
					if (!(TINHKIEM.VietLien(bank.TypeName) == "daocunhiemvu"))
					{
						list.Add(bank);
					}
				}
				return list;
			}
		}

		public List<Bank> NguyenLieu
		{
			get
			{
				List<Bank> list = new List<Bank>();
				int num = game.Memory.Read(game.Address.BankBase);
				for (int i = 30; i < 60; i++)
				{
					if (game.Memory.Read(num + i * 4) == 0)
					{
						continue;
					}
					Bank bank = new Bank(game);
					bank.Address = game.Memory.Read(num + i * 4);
					bank.Class = game.Memory.Read(bank.Address);
					bank.PacketId = game.Memory.Read(bank.Address + 4);
					if (bank.Class == game.Address.PacketType1 || bank.Class == game.Address.PacketType5)
					{
						bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 40));
						bank.Count = 1;
						bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 88));
						bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 84));
					}
					else if (bank.Class == game.Address.PacketType2)
					{
						bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 24));
						bank.Count = game.Memory.Read1Byte(bank.Address + 20, 60);
						if (game.Address.GameType == 2)
						{
							bank.Count = game.Memory.Read1Byte(bank.Address + 20, 88);
						}
						bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 80));
						bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
					}
					else if (bank.Class == game.Address.PacketType3)
					{
						bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 28));
						bank.Count = 1;
						bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 304));
						bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
					}
					else if (bank.Class == game.Address.PacketType4)
					{
						bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 40));
						bank.Count = 1;
						bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 76));
						bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
					}
					else
					{
						bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 88));
						bank.Count = 1;
						bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 80));
						bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
					}
					if (bank.Class == game.Address.PacketType6)
					{
						bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 44));
						bank.Count = 1;
						bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 104));
						bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 40));
					}
					bank.Index = game.Memory.Read(bank.Address + 16);
					if (game.Address.GameType == 1)
					{
						bank.InfoBase = 66;
					}
					else
					{
						bank.InfoBase = 94;
					}
					bank.Line = game.Memory.Read2Byte(bank.Address + 20, bank.InfoBase);
					bank.Star = game.Memory.Read2Byte(bank.Address + 20, bank.InfoBase + 12);
					if (bank.Type == "Cloth2_9" || bank.Type.Contains("Charm"))
					{
						if (game.Address.GameType == 1)
						{
							bank.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
							{
								bank.Address + 20,
								52
							}));
							bank.X = game.Memory.Read2Byte(bank.Address + 20, 54);
							bank.Y = game.Memory.Read2Byte(bank.Address + 20, 56);
						}
						else
						{
							bank.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
							{
								bank.Address + 20,
								80
							}));
							bank.X = game.Memory.Read2Byte(bank.Address + 20, 82);
							bank.Y = game.Memory.Read2Byte(bank.Address + 20, 84);
						}
					}
					bank.Lvl = game.Memory.Read(bank.Address + 40, 44);
					if (!(TINHKIEM.VietLien(bank.TypeName) == "daocunhiemvu"))
					{
						list.Add(bank);
					}
				}
				return list;
			}
		}

		public Bank(Game game)
		{
			this.game = game;
		}

		public void GiamDinh()
		{
			if (game.Memory.Read(game.Memory.Read(Address + 20) + 13).ToString("X8").EndsWith("000010") || game.Memory.Read(game.Memory.Read(Address + 20) + 13).ToString("X8").EndsWith("000011"))
			{
				game.Memory.Write(game.Memory.Read(Address + 20) + 13, 50);
			}
		}

		public static List<Bank> EnumTrangBi(Game game)
		{
			List<Bank> list = new List<Bank>();
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
				Bank bank = new Bank(game);
				bank.Address = game.Memory.Read(num + i * 4);
				bank.Class = game.Memory.Read(bank.Address);
				bank.PacketId = game.Memory.Read(bank.Address + 4);
				if (bank.Class == game.Address.PacketType1 || bank.Class == game.Address.PacketType5)
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 40));
					bank.Count = 1;
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 88));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 84));
				}
				else if (bank.Class == game.Address.PacketType2)
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 24));
					bank.Count = game.Memory.Read1Byte(bank.Address + 20, 60);
					if (game.Address.GameType == 2)
					{
						bank.Count = game.Memory.Read1Byte(bank.Address + 20, 88);
					}
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 80));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
				}
				else if (bank.Class == game.Address.PacketType3)
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 28));
					bank.Count = 1;
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 304));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
				}
				else if (bank.Class == game.Address.PacketType4)
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 40));
					bank.Count = 1;
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 76));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
				}
				else
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 88));
					bank.Count = 1;
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 80));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
				}
				if (bank.Class == game.Address.PacketType6)
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 44));
					bank.Count = 1;
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 104));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 40));
				}
				bank.Index = game.Memory.Read(bank.Address + 16);
				if (game.Address.GameType == 1)
				{
					bank.InfoBase = 66;
				}
				else
				{
					bank.InfoBase = 94;
				}
				bank.Line = game.Memory.Read2Byte(bank.Address + 20, bank.InfoBase);
				bank.Star = game.Memory.Read2Byte(bank.Address + 20, bank.InfoBase + 12);
				if (bank.Type == "Cloth2_9" || bank.Type.Contains("Charm"))
				{
					if (game.Address.GameType == 1)
					{
						bank.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
						{
							bank.Address + 20,
							52
						}));
						bank.X = game.Memory.Read2Byte(bank.Address + 20, 54);
						bank.Y = game.Memory.Read2Byte(bank.Address + 20, 56);
					}
					else
					{
						bank.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
						{
							bank.Address + 20,
							80
						}));
						bank.X = game.Memory.Read2Byte(bank.Address + 20, 82);
						bank.Y = game.Memory.Read2Byte(bank.Address + 20, 84);
					}
				}
				bank.Lvl = game.Memory.Read(bank.Address + 40, 44);
				list.Add(bank);
			}
			return list;
		}

		public List<Bank> Enum()
		{
			List<Bank> list = new List<Bank>();
			int num = game.Memory.Read(game.Address.BankBase);
			for (int i = 0; i < 20; i++)
			{
				if (game.Memory.Read(num + i * 4) == 0)
				{
					continue;
				}
				Bank bank = new Bank(game);
				bank.Address = game.Memory.Read(num + i * 4);
				bank.Class = game.Memory.Read(bank.Address);
				bank.PacketId = game.Memory.Read(bank.Address + 4);
				if (bank.Class == game.Address.PacketType1 || bank.Class == game.Address.PacketType5)
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 40));
					bank.Count = 1;
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 88));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 84));
				}
				else if (bank.Class == game.Address.PacketType2)
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 24));
					bank.Count = game.Memory.Read1Byte(bank.Address + 20, 60);
					if (game.Address.GameType == 2)
					{
						bank.Count = game.Memory.Read1Byte(bank.Address + 20, 88);
					}
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 80));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
				}
				else if (bank.Class == game.Address.PacketType3)
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 28));
					bank.Count = 1;
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 304));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
				}
				else if (bank.Class == game.Address.PacketType4)
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 40));
					bank.Count = 1;
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 76));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
				}
				else
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 88));
					bank.Count = 1;
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 80));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
				}
				if (bank.Class == game.Address.PacketType6)
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 44));
					bank.Count = 1;
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 104));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 40));
				}
				bank.Index = game.Memory.Read(bank.Address + 16);
				if (game.Address.GameType == 1)
				{
					bank.InfoBase = 66;
				}
				else
				{
					bank.InfoBase = 94;
				}
				bank.Line = game.Memory.Read2Byte(bank.Address + 20, bank.InfoBase);
				bank.Star = game.Memory.Read2Byte(bank.Address + 20, bank.InfoBase + 12);
				if (bank.Type == "Cloth2_9" || bank.Type.Contains("Charm"))
				{
					if (game.Address.GameType == 1)
					{
						bank.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
						{
							bank.Address + 20,
							52
						}));
						bank.X = game.Memory.Read2Byte(bank.Address + 20, 54);
						bank.Y = game.Memory.Read2Byte(bank.Address + 20, 56);
					}
					else
					{
						bank.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
						{
							bank.Address + 20,
							80
						}));
						bank.X = game.Memory.Read2Byte(bank.Address + 20, 82);
						bank.Y = game.Memory.Read2Byte(bank.Address + 20, 84);
					}
				}
				bank.Lvl = game.Memory.Read(bank.Address + 40, 44);
				list.Add(bank);
			}
			return list;
		}

		public static List<Bank> EnumDrop(Game game)
		{
			List<Bank> list = new List<Bank>();
			int num = game.Memory.Read(game.Address.BankBase);
			for (int i = 0; i < 80; i++)
			{
				if (game.Memory.Read(num + i * 4) == 0)
				{
					continue;
				}
				Bank bank = new Bank(game);
				bank.Address = game.Memory.Read(num + i * 4);
				bank.Class = game.Memory.Read(bank.Address);
				bank.PacketId = game.Memory.Read(bank.Address + 4);
				if (bank.Class == game.Address.PacketType1 || bank.Class == game.Address.PacketType5)
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 40));
					bank.Count = 1;
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 88));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 84));
				}
				else if (bank.Class == game.Address.PacketType2)
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 24));
					bank.Count = game.Memory.Read1Byte(bank.Address + 20, 60);
					if (game.Address.GameType == 2)
					{
						bank.Count = game.Memory.Read1Byte(bank.Address + 20, 88);
					}
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 80));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
				}
				else if (bank.Class == game.Address.PacketType3)
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 28));
					bank.Count = 1;
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 304));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
				}
				else if (bank.Class == game.Address.PacketType4)
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 40));
					bank.Count = 1;
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 76));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
				}
				else
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 88));
					bank.Count = 1;
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 80));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 20));
				}
				if (bank.Class == game.Address.PacketType6)
				{
					bank.Name = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 44));
					bank.Count = 1;
					bank.TypeName = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 104));
					bank.Type = game.Memory.ReadString(game.Memory.Read(bank.Address + 40, 40));
				}
				bank.Index = game.Memory.Read(bank.Address + 16);
				if (game.Address.GameType == 1)
				{
					bank.InfoBase = 66;
				}
				else
				{
					bank.InfoBase = 94;
				}
				bank.Line = game.Memory.Read2Byte(bank.Address + 20, bank.InfoBase);
				bank.Star = game.Memory.Read2Byte(bank.Address + 20, bank.InfoBase + 12);
				if (bank.Type == "Cloth2_9" || bank.Type.Contains("Charm"))
				{
					if (game.Address.GameType == 1)
					{
						bank.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
						{
							bank.Address + 20,
							52
						}));
						bank.X = game.Memory.Read2Byte(bank.Address + 20, 54);
						bank.Y = game.Memory.Read2Byte(bank.Address + 20, 56);
					}
					else
					{
						bank.MapId = game.Memory.Read2Byte(game.Memory.ReadAddress(new int[2]
						{
							bank.Address + 20,
							80
						}));
						bank.X = game.Memory.Read2Byte(bank.Address + 20, 82);
						bank.Y = game.Memory.Read2Byte(bank.Address + 20, 84);
					}
				}
				bank.Lvl = game.Memory.Read(bank.Address + 40, 44);
				if (!(TINHKIEM.VietLien(bank.TypeName) == "daocunhiemvu"))
				{
					list.Add(bank);
				}
			}
			return list;
		}
	}
}
