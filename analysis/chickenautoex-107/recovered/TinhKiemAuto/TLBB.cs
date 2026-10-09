using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace TinhKiemAuto
{
	public class TLBB
	{
		public Game Game;

		public Memory Memory;

		public Address Address;

		public int Base;

		public string Id;

		public string Name;

		public int MenPhaiLuaEX;

		public int Lvl;

		public int Rage;

		public bool IsFollow;

		public int PetId;

		public int HP;

		public int MP;

		public int Exp;

		public int MaxHP;

		public int MaxMP;

		public int PetBase;

		public int PetHP;

		public int PetMaxHP;

		public int PetEnjoy;

		public int PetLvl;

		public string[] answer;

		private Bitmap captcha;

		public bool IsCaptcha;

		public bool IsPk;

		public bool Disconnected;

		public string KeyId;

		public string MapName;

		public string SkillPetType;

		public int OnlineTime;

		public int DelayBase;

		public string GuildName;

		public int GuildId;

		private int cEGUIBaseAddress;

		public bool IsReadCaptcha;

		public static float RunSpeed = 3.5f;

		public bool IsRelive => Memory.Read1Byte(Memory.ReadAddress(Address.IsRelive)) == 1;

		public int Gold
		{
			get
			{
				if (Address.GameType == 1)
				{
					Address.PlayerGold = 10044;
				}
				return Memory.Read(Base + Address.PlayerGold);
			}
		}

		public int MaxODaoCu
		{
			get
			{
				int num = Memory.Read(Address.ODaoCu);
				if (num == 10141355)
				{
					num = 30;
				}
				else if (num >= 10141020)
				{
					num = num - 10141020 + 20;
				}
				if (num == 0)
				{
					num = 20;
				}
				return num;
			}
		}

		public string Img { get; set; }

		private int BaseImg { get; set; }

		private int BaseAnswer { get; set; }

		public string[] Answer => answer;

		public string ImgHash { get; set; }

		public string bin { get; set; }

		public string BinEx { get; set; }

		public Bitmap Captcha
		{
			get
			{
				if (captcha == null && BinEx != null && BinEx != "")
				{
					captcha = new Bitmap(128, 36);
					int num = 0;
					string binEx = BinEx;
					for (int i = 0; i < binEx.Length; i++)
					{
						if (binEx[i] == '0')
						{
							captcha.SetPixel(num / 2 % 128, num / 2 / 128, Color.Black);
						}
						else
						{
							captcha.SetPixel(num / 2 % 128, num / 2 / 128, Color.White);
						}
						num += 2;
					}
					captcha = new Bitmap(captcha, new Size(160, 45));
				}
				return captcha;
			}
			set
			{
				captcha = value;
			}
		}

		public string RaoTxt
		{
			get
			{
				int moduleAddress = Memory.GetModuleAddress("UI_CEGUI.dll");
				moduleAddress = ((Address.GameType != 1) ? (moduleAddress + 226052) : (moduleAddress + 400392));
				return Memory.ReadString(Memory.Read(moduleAddress));
			}
		}

		public bool IsFresh => Memory.Read(Address.CountDown10Sec) == 1;

		public bool IsShopOpen => Memory.Read(Address.IsShopOpen) == 1;

		public int MaxONguyenLieu
		{
			get
			{
				int num = Memory.Read(Address.ONguyenLieu);
				if (num == 10141356)
				{
					num = 30;
				}
				else if (num >= 10141030)
				{
					num = num - 10141030 + 20;
				}
				if (num == 0)
				{
					num = 20;
				}
				return num;
			}
		}

		public bool IsODaoCuFull
		{
			get
			{
				int maxODaoCu = MaxODaoCu;
				int num = Memory.Read(Address.PacketItemBase);
				for (int i = 0; i < maxODaoCu; i++)
				{
					if (Memory.Read(num + i * 4) == 0)
					{
						return false;
					}
				}
				return true;
			}
		}

		public bool IsONguyenLieuFull
		{
			get
			{
				int maxONguyenLieu = MaxONguyenLieu;
				int num = Memory.Read(Address.PacketItemBase);
				for (int i = 30; i < 30 + maxONguyenLieu; i++)
				{
					if (Memory.Read(num + i * 4) == 0)
					{
						return false;
					}
				}
				return true;
			}
		}

		public int PlayerState { get; set; }

		public int MapId { get; set; }

		public int OnlineTimeSec { get; set; }

		public int MenpaiPoint => Memory.Read(Base + Address.CharMenpaiPoint);

		public int SafeTime => Memory.Read(Address.SafeTime);

		public bool IsNexLogin => Memory.Read(Address.IsNexLogin) == 1;

		public bool IsSelectServer => Memory.Read(Address.IsSelectServer) == 1;

		public bool IsTextCaptcha => Memory.Read(Address.IsTextCaptcha) == 1;

		public bool IsLogon => Memory.Read(Address.IsLogon) == 1;

		public bool IsSelectServerQuest => Memory.Read(Address.IsLoginMessage) == 1;

		public bool IsSelectCharacter => Memory.Read(Address.IsSelectCharacter) == 1;

		public int FakeMapId => Memory.Read(Address.FakeMapId);

		public int HPPercent => Percent(HP, MaxHP);

		public int MPPercent => Percent(MP, MaxMP);

		public int PetHPPercent => Percent(PetHP, PetMaxHP);

		public bool Busy
		{
			get
			{
				if (PlayerState > 2)
				{
					return PlayerState < 10;
				}
				return false;
			}
		}

		public bool BusyEx
		{
			get
			{
				if (PlayerState > 0 && PlayerState != 2)
				{
					return PlayerState != 7;
				}
				return false;
			}
		}

		public int MaxExp
		{
			get
			{
				if (Lvl > 0 && Lvl < 150)
				{
					return MAXEXP.Lvl[Lvl];
				}
				return MAXEXP.Lvl[1];
			}
		}

		public float ExpPercent => (float)Exp * 100f / (float)MaxExp;

		public bool IsNoi
		{
			get
			{
				if (Menpai >= 4 && Menpai != 8)
				{
					return Menpai != 32;
				}
				return false;
			}
		}

		public bool IsRide
		{
			get
			{
				Base = Memory.Read(Address.CharBase);
				return Memory.Read(Base + Address.ObjectRide) != -1;
			}
		}

		public bool IsLeader
		{
			get
			{
				if (Online)
				{
					return Id.Contains(KeyId);
				}
				return false;
			}
		}

		public bool IsOnline { get; set; }

		public int CEGUIBaseAddress
		{
			get
			{
				if (cEGUIBaseAddress == 0)
				{
					cEGUIBaseAddress = Memory.GetModuleAddress("CEGUIBase.dll");
				}
				return cEGUIBaseAddress;
			}
		}

		public int FreshmanWatchTime => Memory.Read(new int[6]
		{
			CEGUIBaseAddress + Address.FreshmanWatchTime[0],
			Address.FreshmanWatchTime[1],
			Address.FreshmanWatchTime[2],
			Address.FreshmanWatchTime[3],
			Address.FreshmanWatchTime[4],
			Address.FreshmanWatchTime[5]
		});

		public bool Online
		{
			get
			{
				if (Id.Contains("00000000") || Id.Contains("FFFFFFFF") || Name == "ĐăngNhập" || Lvl < 1)
				{
					return false;
				}
				IsOnline = true;
				return true;
			}
		}

		public int PetCount
		{
			get
			{
				int num = 0;
				int num2 = Memory.Read(Address.PetBase);
				for (int i = 0; i < 20 && Memory.Read(num2 + Address.PetDataSize * i + Address.PetId) != 0 && Memory.Read(num2 + Address.PetDataSize * i + Address.PetMaxHP) > 0; i++)
				{
					num++;
				}
				return num;
			}
		}

		public bool IsBienThan
		{
			get
			{
				if (Address.GameType == 1)
				{
					return Memory.Read(Base + 156) != -1;
				}
				return Memory.Read(Address.LuyenKimBase, Address.BienThan) == 1;
			}
		}

		public bool IsTogleMission => Memory.Read(Address.IsTogleMission) == 1;

		public bool IsToggleYuanbaoShop => Memory.Read(new int[4] { 6563448, 0, 12, 100 }) == 1;

		public bool IsQuestOpen => Memory.Read(Address.QuestInfo) == 1;

		public bool HaveRide
		{
			get
			{
				bool flag = Memory.Read(Address.HaveRide1) != 0;
				if (Address.GameType == 1)
				{
					int num = 0;
					while (!flag)
					{
						Address.HaveRide2[2] = num;
						flag = Memory.Read(Address.HaveRide2) != 0;
						num += 4;
						if (num > 12)
						{
							break;
						}
					}
				}
				return flag;
			}
		}

		public string MenpaiName => MemPhaiNameByLua();

		public NPC NPCBaiSu
		{
			get
			{
				if (Menpai == MENPAI.ThieuLam)
				{
					return THIEULAM.HuyenTich;
				}
				if (Menpai == MENPAI.MinhGiao)
				{
					return MINHGIAO.LaSuTuong;
				}
				if (Menpai == MENPAI.CaiBang)
				{
					return CAIBANG.TranCoNhan;
				}
				if (Menpai == MENPAI.VoDang)
				{
					return VODANG.TruongHuyenTo;
				}
				if (Menpai == MENPAI.NgaMy)
				{
					return NGAMY.LyThapNhiNuong;
				}
				if (Menpai == MENPAI.TinhTuc)
				{
					return TINHTUC.HanTheTrung;
				}
				if (Menpai == MENPAI.ThienLong)
				{
					return THIENLONG.BanNhan;
				}
				if (Menpai == MENPAI.ThienSon)
				{
					return THIENSON.MaiKiem;
				}
				if (Menpai == MENPAI.TieuDao)
				{
					return TIEUDAO.ToTinhHa;
				}
				if (Menpai == MENPAI.MoDung)
				{
					return MODUNG.MoDungKiet;
				}
				if (Menpai == MENPAI.DuongMon)
				{
					return DUONGMON.DuongXichPhong;
				}
				return null;
			}
		}

		public NPC NPCThuongKho
		{
			get
			{
				if (Address.GameType == 1)
				{
					if (MapId == DAILY.Id)
					{
						return DAILY.ThuongKho;
					}
					if (MapId == TOCHAU.Id)
					{
						return TOCHAU.ThuongKho;
					}
					if (MapId == LACDUONG.Id)
					{
						return LACDUONG.ThuongKho;
					}
					if (MapId == LAULAN.Id)
					{
						return LAULAN.ThuongKho;
					}
					if (MapId == THUCHACOTRAN.Id)
					{
						return THUCHACOTRAN.ThuongKho;
					}
					if (MapId == PHUNGMINHTRAN.Id)
					{
						return PHUNGMINHTRAN.ThuongKho;
					}
					return LACDUONG.ThuongKho;
				}
				if (MapId == DAILY.Id)
				{
					return DAILY.ThuongKhoTinhKiem;
				}
				if (MapId == TOCHAU.Id)
				{
					return TOCHAU.ThuongKhoTinhKiem;
				}
				if (MapId == LACDUONG.Id)
				{
					return LACDUONG.ThuongKhoTinhKiem;
				}
				if (MapId == LAULAN.Id)
				{
					return LAULAN.ThuongKho;
				}
				if (MapId == THUCHACOTRAN.Id)
				{
					return THUCHACOTRAN.ThuongKhoTinhKiem;
				}
				if (MapId == PHUNGMINHTRAN.Id)
				{
					return PHUNGMINHTRAN.ThuongKho;
				}
				return LACDUONG.ThuongKhoTinhKiem;
			}
		}

		public bool IsBankOpen => Memory.Read(Address.IsBankOpen) == 1;

		public NPC NPCBaiSuDaiLy
		{
			get
			{
				if (Menpai == MENPAI.DuongMon)
				{
					return DAILY.DuongDuc;
				}
				if (Menpai == MENPAI.MinhGiao)
				{
					return DAILY.ThachBao;
				}
				if (Menpai == MENPAI.ThienSon)
				{
					return DAILY.TrinhThanhSuong;
				}
				if (Menpai == MENPAI.TinhTuc)
				{
					return DAILY.HaiPhongTu;
				}
				if (Menpai == MENPAI.ThienLong)
				{
					return DAILY.PhaTham;
				}
				if (Menpai == MENPAI.TieuDao)
				{
					return DAILY.DamDaiTuVu;
				}
				if (Menpai == MENPAI.MoDung)
				{
					return DAILY.MoDungTruyen;
				}
				if (Menpai == MENPAI.NgaMy)
				{
					return DAILY.LoTamNuong;
				}
				if (Menpai == MENPAI.CaiBang)
				{
					return DAILY.GianNinh;
				}
				if (Menpai == MENPAI.ThieuLam)
				{
					return DAILY.TueDich;
				}
				if (Menpai == MENPAI.VoDang)
				{
					return DAILY.TruongHoach;
				}
				return null;
			}
		}

		public NPC NPCTamPhap
		{
			get
			{
				if (Menpai == MENPAI.ThieuLam)
				{
					return THIEULAM.HuyenNan;
				}
				if (Menpai == MENPAI.MinhGiao)
				{
					return MINHGIAO.BangVanXuan;
				}
				if (Menpai == MENPAI.CaiBang)
				{
					return CAIBANG.HeTamKi;
				}
				if (Menpai == MENPAI.VoDang)
				{
					return VODANG.DuVienSon;
				}
				if (Menpai == MENPAI.NgaMy)
				{
					return NGAMY.ThoiLucHoa;
				}
				if (Menpai == MENPAI.TinhTuc)
				{
					return TINHTUC.ThiToan;
				}
				if (Menpai == MENPAI.ThienLong)
				{
					return THIENLONG.BanQuan;
				}
				if (Menpai == MENPAI.ThienSon)
				{
					return THIENSON.LanKiem;
				}
				if (Menpai == MENPAI.TieuDao)
				{
					return TIEUDAO.KhangQuangLang;
				}
				if (Menpai == MENPAI.MoDung)
				{
					return MODUNG.MoDungThanhSon;
				}
				if (Menpai == MENPAI.DuongMon)
				{
					return DUONGMON.DuongNhacXung;
				}
				return null;
			}
		}

		public int MapAcBa
		{
			get
			{
				if (Menpai == MENPAI.ThieuLam)
				{
					return MAP.ThieuLamAcBa;
				}
				if (Menpai == MENPAI.MinhGiao)
				{
					return MAP.MinhGiaoAcBa;
				}
				if (Menpai == MENPAI.CaiBang)
				{
					return MAP.CaiBangAcBa;
				}
				if (Menpai == MENPAI.VoDang)
				{
					return MAP.VoDangAcBa;
				}
				if (Menpai == MENPAI.NgaMy)
				{
					return MAP.NgaMyAcBa;
				}
				if (Menpai == MENPAI.TinhTuc)
				{
					return MAP.TinhTucAcBa;
				}
				if (Menpai == MENPAI.ThienLong)
				{
					return MAP.ThienLongAcBa;
				}
				if (Menpai == MENPAI.ThienSon)
				{
					return MAP.ThienSonAcBa;
				}
				if (Menpai == MENPAI.TieuDao)
				{
					return MAP.TieuDaoAcBa;
				}
				if (Menpai == MENPAI.MoDung)
				{
					return MAP.MoDungAcBa;
				}
				if (Menpai == MENPAI.DuongMon)
				{
					return MAP.DuongMonAcBa;
				}
				return -1;
			}
		}

		public int MapMonPhai
		{
			get
			{
				if (Menpai == MENPAI.ThieuLam)
				{
					return THIEULAM.Id;
				}
				if (Menpai == MENPAI.MinhGiao)
				{
					return MINHGIAO.Id;
				}
				if (Menpai == MENPAI.CaiBang)
				{
					return CAIBANG.Id;
				}
				if (Menpai == MENPAI.VoDang)
				{
					return VODANG.Id;
				}
				if (Menpai == MENPAI.NgaMy)
				{
					return NGAMY.Id;
				}
				if (Menpai == MENPAI.TinhTuc)
				{
					return TINHTUC.Id;
				}
				if (Menpai == MENPAI.ThienLong)
				{
					return THIENLONG.Id;
				}
				if (Menpai == MENPAI.ThienSon)
				{
					return THIENSON.Id;
				}
				if (Menpai == MENPAI.TieuDao)
				{
					return TIEUDAO.Id;
				}
				if (Menpai == MENPAI.MoDung)
				{
					return MODUNG.Id;
				}
				if (Menpai == MENPAI.DuongMon)
				{
					return DUONGMON.Id;
				}
				return -1;
			}
		}

		public int X2TimeSec => Memory.Read(Address.X2);

		public Dictionary<int, string> DicPet
		{
			get
			{
				Dictionary<int, string> dictionary = new Dictionary<int, string>();
				PetBase = Memory.Read(Address.PetBase);
				for (int i = 0; i < 20; i++)
				{
					int num = Memory.Read(PetBase + Address.PetDataSize * i + Address.PetId);
					string value = ((Address.GameType == 1) ? Memory._ReadString(PetBase + Address.PetDataSize * i + 36) : Memory._ReadString(PetBase + Address.PetDataSize * i + 28));
					if (num == 0)
					{
						break;
					}
					if (!dictionary.ContainsKey(num))
					{
						dictionary.Add(num, value);
					}
				}
				return dictionary;
			}
		}

		public bool IsRead { get; set; }

		public string PetName { get; set; }

		public bool Valid
		{
			get
			{
				if (!IsNexLogin && !IsSelectServer && !IsLogon && !IsSelectCharacter)
				{
					return IsOnline;
				}
				return true;
			}
		}

		public int Menpai
		{
			get
			{
				int result = -1;
				switch (MenPhaiLuaEX)
				{
				case 0:
					result = 1;
					break;
				case 1:
					result = 2;
					break;
				case 2:
					result = 3;
					break;
				case 3:
					result = 4;
					break;
				case 4:
					result = 5;
					break;
				case 5:
					result = 6;
					break;
				case 6:
					result = 7;
					break;
				case 7:
					result = 8;
					break;
				case 8:
					result = 9;
					break;
				case 9:
					result = -1;
					break;
				case 10:
					result = 32;
					break;
				}
				return result;
			}
		}

		public void ReadCaptcha()
		{
			try
			{
				if (IsReadCaptcha && (BaseAnswer == 0 || BaseImg == 0))
				{
					return;
				}
				bin = "";
				BinEx = "";
				if (BaseAnswer == 0)
				{
					BaseAnswer = (int)Game.AOB.AobScan(new byte[43]
					{
						65, 110, 116, 105, 82, 111, 98, 111, 116, 95,
						70, 114, 97, 109, 101, 0, 15, 0, 0, 0,
						15, 0, 0, 0, 0, 0, 0, 0, 84, 76,
						66, 66, 95, 77, 97, 105, 110, 70, 114, 97,
						109, 101, 48
					}, 1073741824) + 6240;
				}
				answer = new string[4];
				int num = Memory.ScanString("TLBB_ButtonNULL", BaseAnswer, BaseAnswer + 16777215, 0) - 444;
				answer[0] = ConverterEx.Unicodes[Memory.Read(num)].ToString() + ConverterEx.Unicodes[Memory.Read(num + 4)] + ConverterEx.Unicodes[Memory.Read(num + 8)] + ConverterEx.Unicodes[Memory.Read(num + 12)];
				int num2 = Memory.ScanString("TLBB_ButtonNULL", num + 511, num + 16777215, 0) - 444;
				answer[1] = ConverterEx.Unicodes[Memory.Read(num2)].ToString() + ConverterEx.Unicodes[Memory.Read(num2 + 4)] + ConverterEx.Unicodes[Memory.Read(num2 + 8)] + ConverterEx.Unicodes[Memory.Read(num2 + 12)];
				int num3 = Memory.ScanString("TLBB_ButtonNULL", num2 + 511, num2 + 16777215, 0) - 444;
				answer[2] = ConverterEx.Unicodes[Memory.Read(num3)].ToString() + ConverterEx.Unicodes[Memory.Read(num3 + 4)] + ConverterEx.Unicodes[Memory.Read(num3 + 8)] + ConverterEx.Unicodes[Memory.Read(num3 + 12)];
				int num4 = Memory.ScanString("TLBB_ButtonNULL", num3 + 511, num3 + 16777215, 0) - 444;
				answer[3] = ConverterEx.Unicodes[Memory.Read(num4)].ToString() + ConverterEx.Unicodes[Memory.Read(num4 + 4)] + ConverterEx.Unicodes[Memory.Read(num4 + 8)] + ConverterEx.Unicodes[Memory.Read(num4 + 12)];
				try
				{
					Img = string.Empty;
					if (BaseImg == 0)
					{
						BaseImg = (int)Game.AOB.AobScan(new byte[16]
						{
							1, 0, 0, 0, 255, 160, 255, 160, 255, 160,
							255, 160, 255, 160, 255, 160
						}, 0u, 536870911u) + 4;
					}
					int num5 = Memory.Read2Byte(BaseImg);
					if (num5 == 40960 || num5 == 41215)
					{
						byte[] array = new byte[9218];
						Memory.ReadProcessMemory(Memory.Id, BaseImg, array, array.Length, 0);
						for (int i = 0; i < 9216; i += 2)
						{
							num5 = BitConverter.ToInt32(new byte[4]
							{
								array[i],
								array[i + 1],
								0,
								0
							}, 0);
							if (num5 == 40960)
							{
								bin += "0";
								BinEx += "0";
							}
							else
							{
								bin += "1";
								BinEx += "1";
							}
							if (bin.Length == 8)
							{
								Img += Convert.ToInt32(bin, 2).ToString("X2");
								bin = "";
							}
						}
					}
					Img = Img + answer[0] + answer[1] + answer[2] + answer[3];
					ImgHash = TINHKIEM.Hasher.MD5(Img);
				}
				catch
				{
				}
			}
			catch
			{
			}
			IsReadCaptcha = true;
		}

		private static int Percent(int min, int max)
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

		public string MemPhaiNameByLua()
		{
			switch (MenPhaiLuaEX)
			{
			case 0:
				if (Name != "ĐăngNhập")
				{
					return "Thiếu Lâm";
				}
				break;
			case 1:
				return "Minh Giáo";
			case 2:
				return "Cái Bang";
			case 3:
				return "Võ Đang";
			case 4:
				return "Nga My";
			case 5:
				return "Tinh Túc";
			case 6:
				return "Thiên Long";
			case 7:
				return "Thiên Sơn";
			case 8:
				return "Tiêu Dao";
			case 9:
				return "Tân Thủ";
			case 10:
				return "Mộ Dung";
			}
			return "Không Có";
		}

		public string GetMonPhaiName()
		{
			Game.LuaDoOneLineString("local menpai = Player:GetData(\"MEMPAI\"); return menpai;");
			return Game.LuaToString();
		}

		public static bool IsVIPItem(int item)
		{
			if (item != 0)
			{
				switch (item)
				{
				default:
					if (item != 20109101 && item != 20109102 && item != 30008053 && item != 30103042 && item != 10141153 && item != 10157001 && item != 10157002 && item != 10156001 && item != 10156002 && item != 10156003 && item != 10156004)
					{
						if (!item.ToString().StartsWith("101") && !item.ToString().StartsWith("102") && !item.ToString().StartsWith("103") && !item.ToString().StartsWith("104") && !item.ToString().StartsWith("201") && !item.ToString().StartsWith("300"))
						{
							return !item.ToString().StartsWith("301");
						}
						return false;
					}
					break;
				case 20109001:
				case 20109002:
				case 20109003:
				case 20109004:
				case 20109005:
				case 20109006:
				case 20109007:
				case 20109008:
				case 20109009:
				case 20109010:
				case 20109011:
				case 20109012:
				case 20109013:
				case 20109014:
				case 20109015:
				case 30000000:
				case 30008034:
					break;
				}
				return true;
			}
			return false;
		}

		public TLBB(Game game)
		{
			Game = game;
			Address = game.Address;
			Memory = game.Memory;
			Read();
		}

		public void Read()
		{
			Base = Memory.Read(Address.CharBase);
			if (Address.GameType == 1)
			{
				Id = Memory.Read8Byte(Base + Address.CharId).ToString("X8");
			}
			else
			{
				Id = Memory.Read(Base + Address.CharId).ToString("X8");
			}
			Name = Memory.ReadShortString(Base + Address.CharName);
			if (Name == "")
			{
				Name = "ĐăngNhập";
			}
			MenPhaiLuaEX = Memory.Read(Base + 168);
			Lvl = Memory.Read(Base + Address.CharLvl);
			Rage = Memory.Read(Base + Address.CharRage);
			IsFollow = Memory.Read(Base + Address.CharIsFollow) == 1;
			PetId = Memory.Read(Base + Address.CharCurPetId);
			HP = Memory.Read(Base + Address.CharCurHP);
			MaxHP = Memory.Read(Base + Address.CharMaxHP);
			MP = Memory.Read(Base + Address.CharCurMP);
			Exp = Memory.Read(Base + Address.CharExp);
			MaxMP = Memory.Read(Base + Address.CharMaxMP);
			GuildName = Memory.ReadString(Base + Address.CharGuildName);
			GuildId = Memory.Read(Base + Address.CharGuildID);
			if (PetId != 0)
			{
				PetBase = Memory.Read(Address.PetBase);
				for (int i = 0; i < 20; i++)
				{
					if (Memory.Read(PetBase + Address.PetDataSize * i + Address.PetId) == PetId)
					{
						PetHP = Memory.Read(PetBase + Address.PetDataSize * i + Address.PetCurHP);
						PetMaxHP = Memory.Read(PetBase + Address.PetDataSize * i + Address.PetMaxHP);
						PetEnjoy = Memory.Read(PetBase + Address.PetDataSize * i + Address.PetEnjoy);
						PetLvl = Memory.Read(PetBase + Address.PetDataSize * i + Address.PetLvl);
						if (Address.GameType == 1)
						{
							PetName = Memory._ReadString(PetBase + Address.PetDataSize * i + 36);
						}
						else
						{
							PetName = Memory._ReadString(PetBase + Address.PetDataSize * i + 28);
						}
						break;
					}
				}
			}
			else
			{
				PetHP = (PetMaxHP = (PetEnjoy = 0));
			}
			_ = IsCaptcha;
			IsCaptcha = Memory.Read(Address.IsCaptcha) == 1;
			if (IsCaptcha && (BinEx == null || BinEx == ""))
			{
				if (Game.swCaptchaTime == null)
				{
					Game.swCaptchaTime = Stopwatch.StartNew();
				}
				if (Game.swCaptchaTime.Elapsed.TotalSeconds >= 2.0 && !IsRead)
				{
					IsRead = true;
				}
			}
			if (!IsCaptcha)
			{
				IsRead = false;
			}
			IsPk = Memory.Read(Address.IsPK) == 1;
			Disconnected = Memory.Read(Address.Disconnected) == 1;
			if (Address.GameType == 1)
			{
				KeyId = Memory.Read8Byte(Address.KeyId).ToString("X8");
			}
			else
			{
				KeyId = Memory.Read(Address.KeyId).ToString("X8");
			}
			MapName = Memory._ReadString(Address.MapName);
			if (Address.GameType == 1)
			{
				Address.SkillPetBase[1] = 104;
			}
			SkillPetType = Memory._ReadString(Memory.Read(Address.SkillPetBase), 40) + Memory._ReadString(Memory.Read(Address.SkillPetBase) + 4, 40);
			OnlineTime = Memory.Read(Address.OnlineTime) / 60000;
			OnlineTimeSec = Memory.Read(Address.OnlineTime) / 1000;
			DelayBase = Memory.Read(Address.SkillDelayBase);
			MapId = Memory.Read(Address.MapId);
		}

		public override string ToString()
		{
			string empty = string.Empty;
			empty = empty + "InfoAddress: " + Base.ToString("X8");
			empty += "\r\n";
			empty += SkillPetType;
			empty += "\r\n";
			empty += Game.TLBB.Menpai;
			empty += "\r\n";
			empty = empty + "Menpai Point: " + Game.TLBB.MenpaiPoint;
			empty += "\r\n";
			empty += Game.ListHoaTruongThanh.Count;
			empty += "\r\n";
			empty = empty + Game.TrongHoaThuHoachX + "," + Game.TrongHoaThuHoachY;
			empty += "\r\n";
			empty = empty + "hoanhy: " + PetEnjoy;
			empty += "\r\n";
			empty = empty + "isquest: " + IsQuestOpen;
			empty += "\r\n";
			empty = empty + "handle: " + Game.Handle.ToString("X8");
			empty += "\r\n";
			empty = empty + "recvdata: " + Game.RecvData.ToString("X8");
			empty += "\r\n";
			if (Game.ScriptCoBan != null)
			{
				empty = empty + "Script: " + Game.ScriptCoBan.Name;
			}
			empty += "\r\n";
			empty = empty + MaxODaoCu + ".";
			empty = empty + "\r\n" + BaseImg.ToString("X8");
			empty = empty + "\r\n" + Game.ExpStart;
			return empty + "\r\nmap" + MapId;
		}
	}
}
