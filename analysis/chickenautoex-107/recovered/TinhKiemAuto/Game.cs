using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using TinhKiemAuto.Controllers;
using TinhKiemAuto.CostumeControlner;
using TinhKiemAuto.Models;

namespace TinhKiemAuto
{
	public class Game
	{
		private delegate int LuaToStringDelegate();

		[Flags]
		public enum ThreadAccess
		{
			Terminate = 1,
			SuspendResume = 2,
			GetContext = 8,
			SetContext = 0x10,
			SetInformation = 0x20,
			QueryInformation = 0x40,
			SetThreadToken = 0x80,
			Impersonate = 0x100,
			DirectImpersonation = 0x200
		}

		public enum ThreadInfoClass
		{
			ThreadQuerySetWin32StartAddress = 9
		}

		public bool ChatGan = true;

		public bool ChatTheGioi = true;

		public bool ChatThanhThi;

		public bool ChatDongMinh;

		public bool ChatMonPhai;

		public bool ChatBangPhai;

		public bool ChatDoi;

		public bool ishide;

		public bool AutoEatVatPham;

		public bool CongSinh;

		public int CongSinhValue = 20;

		public bool AutoThuPet;

		public bool HuyetTe;

		public int HuyetTeValue = 20;

		public bool AutoHoiSinh;

		public List<ThongBao> ListThongBao = new List<ThongBao>();

		public bool LenBaiTrain;

		public BaiTrain _baitrain = new BaiTrain();

		public int LenBanDoIndex;

		public int LenBaiIndex;

		public int DelayTime = 9;

		public static BaiTrain baitmp = new BaiTrain();

		public bool AlarmChat;

		public bool IsSellItem;

		public bool IsDropItem;

		public double TimeGiaoChat = 180.0;

		public bool AutoTrain;

		public bool UsingTholinhChau;

		public bool IsArletMP;

		public bool isAreadyBuff;

		public bool IsBank;

		public static bool IsGiamDinh = true;

		public int SoLuongMua = 20;

		public int SoLuongChe = 100;

		public bool HuyNguyenLieu = true;

		public bool IsMuaNguyenLieu;

		public Stopwatch CheDO = new Stopwatch();

		public bool IsOpenBag;

		public static int Def;

		public AOBScan AOB;

		private static LuaToStringDelegate LuaToStringFunc;

		public PacketItem Packet;

		private ListViewItem item;

		public LUA LUA;

		private string idLocDo = "";

		public int ProcessId;

		public Address Address;

		public IntPtr Parrent;

		public Memory Memory;

		public TLBB TLBB;

		public GameObjects Objects;

		public List<Skill> Skills = new List<Skill>();

		public List<GameControl> Controls = new List<GameControl>();

		public int IdleTime;

		public int CaptchaTime;

		public int StandTime;

		public string LastId;

		public string LastName;

		public int ExpStart;

		public Stopwatch AutoTime = Stopwatch.StartNew();

		public float CharX;

		public float CharY;

		public int ExpGain;

		public float ExpSpeed;

		public float RadiusX;

		public float RadiusY;

		public int TargetId;

		public int BaseSkill;

		public Skill NMSKill;

		public bool ExitHPLow;

		public bool IsRide;

		public bool[] Alt = new bool[10];

		public bool[] F = new bool[12];

		public int[] KeyDelay = new int[22];

		public int BuffPetPercent = 50;

		public int DeadX;

		public int DeadY;

		public int DeadMap;

		public int DeadFakeMap;

		public bool IsDead;

		public bool IsAuto = true;

		public bool IsAttack = true;

		private bool isLure;

		public bool IsPet = true;

		public bool IsHP = true;

		public bool IsMP = true;

		public bool IsNM;

		private bool isRadius;

		private int radiusMap;

		public bool IsRao;

		public bool IsLuyenKim;

		public bool IsSuMon;

		public bool IsTuDuong;

		public bool IsKhoang;

		public bool IsDuoc;

		public bool IsTrongTrot;

		public bool IsThuHoach;

		public bool IsBachHoaDuyen;

		public bool BachHoaDuyenCompleted;

		public bool IsQDua;

		public bool QDuaCompleted;

		public bool NhatKieuMoi;

		public List<string> lstNhat = new List<string>();

		public List<string> lstBoQua = new List<string>();

		public List<string> lstXoa = new List<string>();

		public List<string> lstDoNgon = new List<string>();

		public int QDuaIndex;

		public bool DaNhanHoaHong;

		public bool DaNhanHoaChung;

		public bool IsChucPhuc;

		public bool IsCauOThuoc;

		public bool IsPickItem;

		public bool X4;

		public int TrongTrotIndex;

		public int ThuHoachIndex;

		public int MoveIndex = -1;

		public string Pass2 = "1312";

		public bool IsTrongHoa;

		public bool IsBonHoa;

		public bool IsThuHoachHoa;

		public bool IsTrungAc;

		public string MissionInfo;

		public int MissionX;

		public int MissionY;

		public string MissionMonter;

		public int MissionMap;

		public List<int> lstNguoiRom = new List<int>();

		public string TrangThaiTrongTrot = string.Empty;

		public string MissionNPC;

		public int MissionID;

		public Stopwatch MoveExTime = Stopwatch.StartNew();

		public Stopwatch EXITTime = Stopwatch.StartNew();

		public List<int[]> ListMoveEx = new List<int[]>();

		public Stopwatch NhatDoEX = Stopwatch.StartNew();

		public int RoundX;

		public int RoundY;

		private int BachHoaDuyenX;

		private int BachHoaDuyenY;

		public List<int> ListDaBon = new List<int>();

		public List<long[]> ListHoaXuatHien = new List<long[]>();

		public string TrungAcInfo = string.Empty;

		public int Extra1;

		public string TrangThaiBTD = "";

		private int BTDX = -1;

		private int BTDY = -1;

		private int BTDMAP = -1;

		private string BTDInfo = "";

		private int BTDIndex = -1;

		private Stopwatch comeTime;

		private Stopwatch doneTime = Stopwatch.StartNew();

		public string BachHoaDuyenInfo = "";

		public static bool DaRao = false;

		public int MoveCount;

		public string TrangThaiQD = "";

		public string QDInfo = "";

		public int QDuaX = 134;

		public int QDuaY = 165;

		public int QDuaMap = MAP.TayHo;

		public string thongtinbang = "";

		public int SoHopQua;

		public static List<int> ListDangLumHop = new List<int>();

		public string TenThanhKT = "-1";

		public string TenMapThanhKT = "-1";

		public int TrongHoaThuHoachId = -1;

		public int TrongHoaThuHoachX;

		public int TrongHoaThuHoachY;

		private int idHoa = -1;

		private Dictionary<int, DateTime> ListThuHoachBHD = new Dictionary<int, DateTime>();

		private Dictionary<int, DateTime> ListBHDXuatHien = new Dictionary<int, DateTime>();

		public static List<int> ListDangThuHoach = new List<int>();

		public static HashSet<int> HashDangBon = new HashSet<int>();

		public HashSet<string> BlackListHoa = new HashSet<string>();

		public string dialogInfo = "";

		public List<long[]> ListHoaTruongThanh = new List<long[]>();

		public Stopwatch HideTime = Stopwatch.StartNew();

		private HashSet<string> NotSafe = new HashSet<string>();

		private int SafeX;

		private int SafeY;

		private int CurPhungMinhIndex = -1;

		public static HashSet<string> HashToaDo = new HashSet<string>();

		public bool Talked;

		public DateTime BossTime = DateTime.MinValue;

		public bool IsTueHong;

		public int TueHongState;

		public string TrangThaiNhiemVuCoBan = "";

		public List<Script> ListNhiemVu = new List<Script>();

		private int IsBossVanKiemCocDie;

		public bool Is2d;

		private const int GWL_STYLE = -16;

		private Thread InitThread;

		private bool IsOpenPass2;

		public int TimeOnMap;

		public int SafeTime;

		public bool IsNhatHop;

		public bool IsNhatHopQDua;

		public bool IsMoBang;

		private int biendem;

		public bool Live = true;

		public string Enemy = "";

		private Stopwatch ClearTime = Stopwatch.StartNew();

		private int IsTalked = -1;

		private int MapATIndex = -1;

		private int CurMapATIndex = -1;

		private bool isQuangCao;

		public Stopwatch BossDieTime = Stopwatch.StartNew();

		private string TrangThaiThuyLao = "";

		public int AcBa = -1;

		public static NPC HoaHachCan = new NPC
		{
			Id = 8371,
			X = 180,
			Y = 90,
			Map = 236,
			INFOAIM = "#GYến Tử Ổ #RHoa Hách Cấn#{_INFOAIM180,90,236,Hoa Hách Cấn}"
		};

		public bool TraQ;

		public bool NhanQ;

		public bool IsContinute;

		public bool IsClick;

		public HashSet<string> Rac = new HashSet<string> { "Linh Thú Diện", "Linh Thú Trảo", "Linh Thú Giáp", "Linh Thú Hoàn", "Linh Thú Sức" };

		public Stopwatch tranTime = Stopwatch.StartNew();

		public Stopwatch TrimTime = Stopwatch.StartNew();

		public bool IsXongPhuBan;

		private Stopwatch LastPhanDame = Stopwatch.StartNew();

		public Thread ThreadAuto;

		public List<GameObject> ListBay = new List<GameObject>();

		private Stopwatch NeBayTime = Stopwatch.StartNew();

		private bool come;

		private bool comeex;

		private int lastX2TimeSec;

		private Stopwatch TimeStand = Stopwatch.StartNew();

		public int KheLinhCount;

		private bool IsMini;

		private bool setSafeTime;

		private Stopwatch NMTime = Stopwatch.StartNew();

		public int CurTab;

		public int NPCID = 191;

		private bool IsHideAgain;

		public int x2;

		public int SoLanX2;

		public bool IsNguyenVong;

		public bool IsVanMay;

		public bool IsLyHoa;

		public bool OkNhanDa;

		public bool IsNguHanhPhap;

		private List<int> lootPacketId = new List<int>();

		public static List<int> lootPacketIdDua = new List<int>();

		private Stopwatch pickTiem = Stopwatch.StartNew();

		private Stopwatch CareTime = Stopwatch.StartNew();

		private int PickedId = -1;

		private List<int> BlackList = new List<int>();

		public Stopwatch PickTime = Stopwatch.StartNew();

		public bool IsQuit;

		public GameObject BestTarget;

		private bool isAtkFollow;

		private Stopwatch lastAutoMove = Stopwatch.StartNew();

		private Stopwatch lastTalk = Stopwatch.StartNew();

		private Stopwatch LastShoww = Stopwatch.StartNew();

		private int AtackTime;

		public static HashSet<int> LureId = new HashSet<int>();

		private int WM_KEYDOWN = 256;

		private int WM_KEYUP = 257;

		public int[] AddressOneLineEx = new int[20];

		private int curLine;

		public byte[] bufferRecv = new byte[10];

		private int lastResetTime;

		private Stopwatch lastReset = Stopwatch.StartNew();

		public string TrangThaiLuyenKim = "";

		public int ChuyenKhoangX;

		public int ChuyenKhoangY;

		public int LuyenKimX;

		public int LuyenKimY;

		public string TrangThai = "";

		private string lpmh = "";

		public string TrangThaiTuDuong = "";

		public string TuDuongInfo = "";

		public int TuDuongX;

		public int TuDuongY;

		public int TuDuongMap;

		public string TrangThaiXayDung = "";

		public string XayDungInfo = "";

		public bool IsXayDung;

		public int XayDungDanhQuaiTime;

		public int KetMap;

		public bool IsAcceptAll;

		public string TrangThaiTuBaoBon = "";

		public bool IsTuBaoBon;

		public string TrangThaiSuMon = "";

		public string SuMonInfo = "";

		public int SuMonX;

		public int SuMonY;

		public int SuMonMap;

		private bool IsDauCo;

		public string DoSuMon = "";

		public AlarmVaoPhai alarmVaoPhai;

		public bool IsNotClear;

		public List<int[]> ListMove = new List<int[]>();

		public int XayDungX;

		public int XayDungY;

		public int XayDungMap;

		public static HashSet<string> JunkItemName = new HashSet<string>
		{
			"Phục Linh Cao", "Bất Lão Cao", "Hoạt Huyết Tán", "Ngưu Hoàn Phấn", "Hoàn Linh Đan", "Sơn Dược Chúc", "Hành Khí Tán", "Tiểu Hành Nang", "Trung Hành Nang", "Trung Cách Rương",
			"Ngũ Độc Cẩm Y", "Đường Môn Khinh Trang"
		};

		public static HashSet<string> JunkItemType = new HashSet<string> { "Nguyên liệu đúc", "Vật liệu may mặc", "N.liệu công nghệ", "Thịt Sơ Cấp", "Thịt Trung Cấp", "Thịt Cao Cấp", "Da Sơ Cấp", "Da Trung Cấp", "Da Cao Cấp", "Vật liệu chế dược" };

		public static HashSet<string> TrangBi = new HashSet<string>
		{
			"Khuyên", "Nỏ", "Đơn Đoản", "Hộ Phù", "Đao Búa", "Hộ Kiên", "Hộ Uyển", "Y Phục", "Hài", "Song Đoản",
			"Thương Bổng", "Hộ Thủ", "Mão", "Hạng Liên", "Yêu Đai", "Phiến", "Giới Chỉ"
		};

		private int mapAcTac;

		private int mapTKC;

		private bool isAlarmHP;

		private bool isAlarmPK;

		private bool isAlarmDead;

		private bool isAlarmCaptcha;

		private int disconnectedTime;

		private bool isAlarmBachHoaDuyen;

		public bool isAlarmKet;

		public bool isAlarmHong;

		private int AddressToString;

		private int AddressTenBang;

		private int AddressEnemy;

		private bool isAlarmDua;

		public bool IsAlarmAcBa;

		public string TrangThaiPhuMau = "";

		public string PhuMauInfo = "";

		public int PhuMauX;

		public int PhuMauY;

		public int PhuMauMap;

		public bool IsCheDo;

		public int CheLoai;

		public string CheTen = string.Empty;

		public int CheCap;

		public int CheNoiNgoai = 1;

		public int CheSao = 6;

		public int CheDong = 5;

		public int CheDiem;

		public int MuaCount;

		public string MuaName = string.Empty;

		public static int shopIndex = 6;

		public bool IsChayVong;

		public bool IsNhatHopall;

		public bool IsOptLocDo;

		public bool IsNhanQuaHoaHong;

		public bool NhanQuaHoaHongCompleted;

		private string trangthaichayvong = "";

		public int Define { get; set; }

		public ListViewItem Item
		{
			get
			{
				return item;
			}
			set
			{
				item = value;
			}
		}

		public static int TickCount { get; set; }

		public List<AutoEat> TudongAn { get; set; }

		public string IdLocDo
		{
			get
			{
				if (idLocDo == "")
				{
					idLocDo = TINHKIEM.ReadFile(Global.NhatHaPath + "\\" + TLBB.Id + ".txt");
				}
				if (idLocDo == "")
				{
					return "Nước Dưa Hấu\r\nPhát Tài Rồi\r\nBăng Trấn Dưa Hấu\r\nDị Dung Đan: Tuyết Nhân";
				}
				return idLocDo;
			}
			set
			{
				TINHKIEM.WriteFile(Global.NhatHaPath + "\\" + TLBB.Id + ".txt", value);
				idLocDo = value;
			}
		}

		public Process Process { get; set; }

		public IntPtr Handle { get; set; }

		public int style { get; set; }

		public bool IsHide { get; set; }

		public bool IsPMP { get; set; }

		public bool IsHuyetChien { get; set; }

		public bool IsLureEx
		{
			get
			{
				if (!Global.AtkFollowKey && !IsPK)
				{
					if (TLBB.MapId != MAP.TangKinhCac || TLBB.IsLeader)
					{
						if (TLBB.MapId != MAP.TranLongKyCuoc && TLBB.MapId != MAP.LauLanBaoTang)
						{
							return IsLure;
						}
						return false;
					}
					return true;
				}
				return false;
			}
		}

		public bool IsLure
		{
			get
			{
				return isLure;
			}
			set
			{
				isLure = value;
			}
		}

		public bool IsRadius
		{
			get
			{
				if (radiusMap == TLBB.MapId)
				{
					return isRadius;
				}
				return false;
			}
			set
			{
				radiusMap = TLBB.MapId;
				isRadius = value;
			}
		}

		public bool IsBHDByLogin { get; set; }

		public bool IsBTDByLogin { get; set; }

		public bool IsDuaByLogin { get; set; }

		public string RaoTxt { get; set; }

		public bool AutoResetTime { get; set; }

		public int State { get; set; }

		public bool IsMoveEx
		{
			get
			{
				if (ListMoveEx.Count > 20)
				{
					ListMoveEx.Clear();
				}
				if (TLBB.BusyEx)
				{
					MoveExTime = Stopwatch.StartNew();
					ListMoveEx.Clear();
					return true;
				}
				bool flag = false;
				int[] array = new int[2] { RoundX, RoundY };
				foreach (int[] item in ListMoveEx)
				{
					if (item[0] == array[0] && item[1] == array[1])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					ListMoveEx.Add(new int[2] { RoundX, RoundY });
					MoveExTime = Stopwatch.StartNew();
					return true;
				}
				if (MoveExTime.Elapsed.TotalSeconds > 15.0)
				{
					MoveExTime = Stopwatch.StartNew();
					ListMoveEx.Clear();
					return false;
				}
				return MoveExTime.Elapsed.TotalSeconds <= 4.0;
			}
		}

		public bool IsThuyLao { get; set; }

		public int HoaIndex
		{
			get
			{
				foreach (PacketItem item in PacketItem.Enum(this))
				{
					if (item.Type == "CircularTaskTool8_10")
					{
						return item.Index;
					}
				}
				return -1;
			}
		}

		public int PhanHoaIndex
		{
			get
			{
				foreach (PacketItem item in PacketItem.Enum(this))
				{
					if (item.Type == "CircularTaskTool8_11")
					{
						return item.Index;
					}
				}
				return -1;
			}
		}

		public bool IsXongTrungAc { get; set; }

		public bool IsHong { get; set; }

		public bool IsMoBTD { get; set; }

		public bool IsMove
		{
			get
			{
				if (TLBB.MapId == MAP.ThienSon)
				{
					return true;
				}
				if (TLBB.MapId == MAP.TayHo && CharX > 276f && CharX < 280f && CharY > 83f && CharY < 103f)
				{
					return true;
				}
				if (TrangThaiTuDuong.Contains("LeNghi") && TrangThaiTuDuong != "LeNghi")
				{
					return true;
				}
				if (TrangThaiTuDuong == "BanCung" && TINHKIEM.GetDistance(CharX, CharY, 176f, 150f) < 3f && TLBB.MapId == LACDUONG.Id)
				{
					return true;
				}
				int num = 15;
				if (IsNhiemVuCoBan)
				{
					num = 30;
				}
				if (IsBachHoaDuyen)
				{
					num = 5;
				}
				if (IsQDua)
				{
					num = 5;
				}
				if (IsNhatHopQDua)
				{
					num = 5;
				}
				if (TLBB.Busy)
				{
					ListMove.Clear();
					MoveCount = 0;
					return true;
				}
				int[] array = new int[2]
				{
					(int)CharX,
					(int)CharY
				};
				foreach (int[] item in ListMove)
				{
					if (item[0] == array[0] && item[1] == array[1])
					{
						MoveCount++;
						if (MoveCount > num)
						{
							return false;
						}
						return true;
					}
				}
				ListMove.Add(array);
				if (ListMove.Count > 15)
				{
					ListMove.RemoveAt(0);
				}
				MoveCount = 0;
				return true;
			}
		}

		public int BHDCount { get; set; }

		private Stopwatch BuyTime { get; set; }

		public string InfoHoa { get; set; }

		public int LastHoaTime { get; set; }

		public bool IsTrongByLogin { get; set; }

		public int ShowTime { get; set; }

		public static int TrongHoaX { get; set; }

		public bool IsTheoAcTac { get; set; }

		public bool IsMapAcBa
		{
			get
			{
				if (TLBB.MapId != MAP.ThieuLamAcBa && TLBB.MapId != MAP.NgaMyAcBa && TLBB.MapId != MAP.TieuDaoAcBa && TLBB.MapId != MAP.DuongMonAcBa && TLBB.MapId != MAP.MinhGiaoAcBa && TLBB.MapId != MAP.VoDangAcBa && TLBB.MapId != MAP.TinhTucAcBa && TLBB.MapId != MAP.ThienSonAcBa && TLBB.MapId != MAP.CaiBangAcBa && TLBB.MapId != MAP.ThienLongAcBa)
				{
					return TLBB.MapId == MAP.MoDungAcBa;
				}
				return true;
			}
		}

		public bool IsTalkedRose { get; set; }

		public bool IsDoiKTT { get; set; }

		public bool IsNhiemVuCoBan { get; set; }

		public Task TaskCoBan { get; set; }

		public Script ScriptCoBan { get; set; }

		private int IsTalkToNpc { get; set; }

		private bool IsLoad { get; set; }

		public bool NexStep { get; set; }

		public QuestFrame QuestFrame { get; set; }

		public int Radius => (int)TINHKIEM.GetDistance(RadiusX, RadiusY, CharX, CharY);

		public string ExpInfo
		{
			get
			{
				try
				{
					if (TLBB.Lvl < 1 || TLBB.Lvl > 149)
					{
						return "00:00:00\r\n\r\n0.00M\r\n0.00M\r\n0.00M\r\n0.00M\r\n0.00M\r\n0.00M\r\n0.00M (0.00%)\r\n\r\n00:00:00:00";
					}
					string text = "vô tận";
					int num = TLBB.MaxExp - TLBB.Exp;
					if (num <= 0)
					{
						text = "0 giây";
					}
					else if (ExpSpeed != 0f)
					{
						text = TimeSpanToString(TimeSpan.FromHours((float)num / ExpSpeed));
					}
					return TimeSpanToString(AutoTime.Elapsed) + "\r\n\r\n\r\n" + $"{(float)ExpStart / 1000000f:0.00}M\r\n" + $"{(float)TLBB.Exp / 1000000f:0.00}M\r\n" + $"{(float)ExpGain / 1000000f:0.00}M ({(float)ExpGain / (float)TLBB.MaxExp:0.00%})\r\n" + $"{(float)num / 1000000f:0.00}M\r\n" + $"{(float)TLBB.MaxExp / 1000000f:0.00}M\r\n" + $"{ExpSpeed / 1000000f:0.00}M ({ExpSpeed / (float)TLBB.MaxExp:0.00%})" + (IsX2 ? " ( x2 )" : "") + "\r\n\r\n" + text;
				}
				catch
				{
					string text2 = "vô tận";
					int num2 = 0;
					return TimeSpanToString(AutoTime.Elapsed) + "\r\n\r\n\r\n" + $"{(float)ExpStart / 1000000f:0.00}M\r\n" + $"{(float)TLBB.Exp / 1000000f:0.00}M\r\n" + $"{(float)ExpGain / 1000000f:0.00}M ({(float)ExpGain / (float)TLBB.MaxExp:0.00%})\r\n" + $"{(float)num2 / 1000000f:0.00}M\r\n" + $"{(float)TLBB.MaxExp / 1000000f:0.00}M\r\n" + $"{ExpSpeed / 1000000f:0.00}M ({ExpSpeed / (float)TLBB.MaxExp:0.00%})\r\n\r\n" + text2;
				}
			}
		}

		public bool IsChangeMap => !Memory.IsRead(Address.ParaUseSkill);

		public bool IsNhanMam { get; set; }

		private bool IsInit { get; set; }

		public bool ON_SCENE_TRANSING => Memory.Read(new int[4] { Address.ON_SCENE_TRANSING, 0, 12, 100 }) == 1;

		public bool IsPickBTD { get; set; }

		public string Status
		{
			get
			{
				if (TrangThaiNhiemVuCoBan != "")
				{
					return TrangThaiNhiemVuCoBan;
				}
				if (TrangThaiPhuMau != "")
				{
					return TrangThaiPhuMau;
				}
				if (TLBB.IsODaoCuFull || TLBB.IsONguyenLieuFull)
				{
					return "Đầy Tay Nải";
				}
				if (HongTrungAcTime != null && HongTrungAcTime.Elapsed.TotalSeconds > 150.0)
				{
					return "Hỏng Trừng Ác";
				}
				if (IsDome)
				{
					return "Pet Thiếu Hoan Hỉ";
				}
				if (IsKyCuoc)
				{
					return "Kỳ Cuộc";
				}
				if (MapAcTac != 0)
				{
					return "Ác Tặc";
				}
				if (IsTrungAc)
				{
					return "Trừng Ác";
				}
				if (IsAcBa)
				{
					return "Ác Bá";
				}
				if (TLBB.PlayerState == 2)
				{
					return "Di Chuyển";
				}
				if (isAtkFollow && IsAttack && BestTarget != null)
				{
					return "Đánh Theo Key";
				}
				if (isLure && IsAttack && BestTarget != null)
				{
					return "Lure Quái";
				}
				if (TLBB.HPPercent < Global.AlarmHPPercent)
				{
					return "Sắp Hết Máu";
				}
				if (IsCheDo)
				{
					return "Chế Đồ";
				}
				if (TLBB.IsPk)
				{
					return "Bị PK";
				}
				if (ON_SCENE_TRANSING)
				{
					return "Chuyển Cảnh";
				}
				if (IdleTime > 10 && TLBB.PlayerState == 0)
				{
					return "Rảnh";
				}
				if (IdleTime < 10 && IsAttack && TLBB.PlayerState == 7)
				{
					return "Đánh Quái";
				}
				if (TLBB.PlayerState == 8)
				{
					return "Chế";
				}
				if (TLBB.PlayerState == 5)
				{
					return "Sử Dùng Phù";
				}
				if (TLBB.PlayerState == 6)
				{
					return "Phục Hồi";
				}
				if (!TLBB.HaveRide)
				{
					return "Chưa Có Ngựa";
				}
				if (TLBB.PlayerState == 0)
				{
					return "Rảnh";
				}
				return "";
			}
		}

		public bool IsSaveGold { get; set; }

		public bool IsTriLieu { get; set; }

		public bool IsKichAuto { get; set; }

		private int FuncLuaToString { get; set; }

		public static int LastFuncLuaToString { get; set; }

		public bool IsSuspend { get; set; }

		private bool IsOut { get; set; }

		private Stopwatch LyThuThuyDead { get; set; }

		private bool IsLyThuThuyDead { get; set; }

		private int ClickTime { get; set; }

		private bool OLaoDaiDead { get; set; }

		private Stopwatch DieTime { get; set; }

		private bool IsTalkPhuManNghi { get; set; }

		private bool IsTalkOLaoDai { get; set; }

		private bool IsCapDaiBaDie { get; set; }

		private bool IsTangThoCongDie { get; set; }

		private bool IsOLaoDaiDie { get; set; }

		private bool IsNhamBinhSinhDie { get; set; }

		private bool IsLyThuThuyDie { get; set; }

		public string TKCInfo { get; set; }

		public int TKCState { get; set; }

		public Stopwatch TKCStateTime { get; set; }

		public bool IsTraiTKC { get; set; }

		public bool TKCComplete { get; set; }

		public bool CheckTKCComplete { get; set; }

		public bool TKCompleted { get; set; }

		private bool IsBossDie { get; set; }

		private int CurIndex { get; set; }

		public bool IsTrieuTap { get; set; }

		public bool IsAcBa { get; set; }

		public bool IsPhungHoangLangMo { get; set; }

		public bool IsQ123LauLan { get; set; }

		public bool IsQ123ToChau { get; set; }

		public bool IsTheoQ { get; set; }

		private bool DaNhanThuyLao { get; set; }

		public bool IsLauLanTamBao { get; set; }

		public static bool IsHoldPK { get; set; }

		private int XuatPetCount { get; set; }

		public string ToaDo { get; set; }

		public bool IsNhanNguyenLieu { get; set; }

		public int WaitRecv { get; set; }

		public int TrieuTapX { get; set; }

		public int TrieuTapY { get; set; }

		public int TrieuTapMap { get; set; }

		public bool IsTKC => false;

		public bool IsCungMay
		{
			get
			{
				if (Objects.Self == null)
				{
					return true;
				}
				if (Objects.Self != null && Objects.Self.PartyId == -1)
				{
					return true;
				}
				return false;
			}
		}

		public int FreshmanWatchReceive { get; set; }

		public bool IsDome { get; set; }

		public bool IsP { get; set; }

		public bool IsKyCuoc { get; set; }

		public bool IsXuat { get; set; }

		public Stopwatch swTrimTime { get; set; }

		public bool IsTalkGiamNguc { get; set; }

		public bool IsChangeM { get; set; }

		public bool ABC { get; set; }

		public bool IsManMacDie { get; set; }

		public bool IsTanVanDie { get; set; }

		public bool IsDaoThanhDie { get; set; }

		public bool IsBangXiDie { get; set; }

		private bool IsXongTamBao { get; set; }

		private bool IsXongKyCuoc { get; set; }

		private bool IsBusy => IsMapPhuBan();

		public bool IsX2 { get; set; }

		public bool TuAnX2 { get; set; }

		public bool IsPhiThuy { get; set; }

		public bool IsKheLinh { get; set; }

		public bool IsClickKheLinh { get; set; }

		public bool AutoBuyKNB { get; set; }

		public bool IsChangeTab { get; set; }

		public bool IsChange { get; set; }

		public bool IsKichHoat { get; set; }

		public bool IsTalkTieuPhong { get; set; }

		public bool IsHookRecv { get; set; }

		public bool IsCheckOnline { get; set; }

		public bool IsCheckOnlineEx { get; set; }

		public bool IsNhatTuyet { get; set; }

		public bool IsDungIm { get; set; }

		public bool IsGoToShop { get; set; }

		public bool IsWrite { get; set; }

		public bool IsLPMH { get; set; }

		private Stopwatch RaoTime { get; set; }

		public bool IsNhanh { get; set; }

		public int PickId { get; set; }

		public bool IsYenTuO { get; set; }

		public bool IsDoanDienKhanhDie { get; set; }

		public bool IsCuuMaTriDie { get; set; }

		public bool IsTalkPhuBan { get; set; }

		private bool IsPK { get; set; }

		public int AddressOneLine { get; set; }

		public int AddressUnicodeString { get; set; }

		public int AddressString { get; set; }

		private int NumsByte { get; set; }

		public bool IsHooked { get; set; }

		public bool IsLostLeader { get; set; }

		public bool IsOpenShop { get; set; }

		public int[,] AcBaPoint
		{
			get
			{
				string text = Setting.LoadMAP(TLBB.MapAcBa.ToString());
				int num = 0;
				string[] array = text.Split('-');
				foreach (string text2 in array)
				{
					int num2 = 0;
					int num3 = 0;
					try
					{
						num2 = TINHKIEM.ParseInt(text2.Split(',')[0]);
						num3 = TINHKIEM.ParseInt(text2.Split(',')[1]);
					}
					catch
					{
					}
					if (num2 != 0 && num3 != 0)
					{
						num++;
					}
				}
				if (num > 1)
				{
					int[,] array2 = new int[num, 2];
					int num4 = 0;
					array = text.Split('-');
					foreach (string text3 in array)
					{
						int num5 = 0;
						int num6 = 0;
						try
						{
							num5 = TINHKIEM.ParseInt(text3.Split(',')[0]);
							num6 = TINHKIEM.ParseInt(text3.Split(',')[1]);
						}
						catch
						{
						}
						if (num5 != 0 && num6 != 0)
						{
							array2[num4, 0] = num5;
							array2[num4, 1] = num6;
							num4++;
						}
					}
					return array2;
				}
				return null;
			}
		}

		public int[,] TamTaiHiepCocPoint
		{
			get
			{
				string text = "195,140-175,140-170,125-190,120-190,110-194,54-165,60-145,65-135,75-130,50-115,45-95,45-70,45-60,55-65,40-50,43-60,65-52,188";
				int num = 0;
				string[] array = text.Split('-');
				foreach (string text2 in array)
				{
					int num2 = 0;
					int num3 = 0;
					try
					{
						num2 = TINHKIEM.ParseInt(text2.Split(',')[0]);
						num3 = TINHKIEM.ParseInt(text2.Split(',')[1]);
					}
					catch
					{
					}
					if (num2 != 0 && num3 != 0)
					{
						num++;
					}
				}
				if (num > 1)
				{
					int[,] array2 = new int[num, 2];
					int num4 = 0;
					array = text.Split('-');
					foreach (string text3 in array)
					{
						int num5 = 0;
						int num6 = 0;
						try
						{
							num5 = TINHKIEM.ParseInt(text3.Split(',')[0]);
							num6 = TINHKIEM.ParseInt(text3.Split(',')[1]);
						}
						catch
						{
						}
						if (num5 != 0 && num6 != 0)
						{
							array2[num4, 0] = num5;
							array2[num4, 1] = num6;
							num4++;
						}
					}
					return array2;
				}
				return null;
			}
		}

		public int[,] ViemMaSonPoint
		{
			get
			{
				string text = "135,180-135,170-125,165-120,180-110,180-115,160-105,160-90,184-57,81-78,68-83,38-52,31-33,64-55,56-212,40";
				int num = 0;
				string[] array = text.Split('-');
				foreach (string text2 in array)
				{
					int num2 = 0;
					int num3 = 0;
					try
					{
						num2 = TINHKIEM.ParseInt(text2.Split(',')[0]);
						num3 = TINHKIEM.ParseInt(text2.Split(',')[1]);
					}
					catch
					{
					}
					if (num2 != 0 && num3 != 0)
					{
						num++;
					}
				}
				if (num > 1)
				{
					int[,] array2 = new int[num, 2];
					int num4 = 0;
					array = text.Split('-');
					foreach (string text3 in array)
					{
						int num5 = 0;
						int num6 = 0;
						try
						{
							num5 = TINHKIEM.ParseInt(text3.Split(',')[0]);
							num6 = TINHKIEM.ParseInt(text3.Split(',')[1]);
						}
						catch
						{
						}
						if (num5 != 0 && num6 != 0)
						{
							array2[num4, 0] = num5;
							array2[num4, 1] = num6;
							num4++;
						}
					}
					return array2;
				}
				return null;
			}
		}

		public int[,] AcTacPoint
		{
			get
			{
				string text = "96,84-109,52-107,43-108,21-95,20-76,21-63,28-47,20-32,25-23,21-20,31-42,40-42,72-21,94";
				int num = 0;
				string[] array = text.Split('-');
				foreach (string text2 in array)
				{
					int num2 = 0;
					int num3 = 0;
					try
					{
						num2 = TINHKIEM.ParseInt(text2.Split(',')[0]);
						num3 = TINHKIEM.ParseInt(text2.Split(',')[1]);
					}
					catch
					{
					}
					if (num2 != 0 && num3 != 0)
					{
						num++;
					}
				}
				if (num > 1)
				{
					int[,] array2 = new int[num, 2];
					int num4 = 0;
					array = text.Split('-');
					foreach (string text3 in array)
					{
						int num5 = 0;
						int num6 = 0;
						try
						{
							num5 = TINHKIEM.ParseInt(text3.Split(',')[0]);
							num6 = TINHKIEM.ParseInt(text3.Split(',')[1]);
						}
						catch
						{
						}
						if (num5 != 0 && num6 != 0)
						{
							array2[num4, 0] = num5;
							array2[num4, 1] = num6;
							num4++;
						}
					}
					return array2;
				}
				return null;
			}
		}

		private bool IsXongThuyLao { get; set; }

		public int CatchPetTime { get; set; }

		public bool IsNhanLeBao { get; set; }

		public TINHKIEM.Menpai SetMenPai { get; set; }

		public bool IsSetMenPai { get; set; }

		private bool isAlarmVaoPhai { get; set; }

		public bool IsPickEx
		{
			get
			{
				if (TLBB.MapId != MAP.ThieuThatSon && TLBB.MapId != MAP.PhungMinhVuongLang && TLBB.MapId != MAP.PhieuMieuPhong && TLBB.MapId != MAP.YenTuO)
				{
					return TLBB.MapId == MAP.TangKinhCac;
				}
				return true;
			}
		}

		public int MoVangNum
		{
			get
			{
				foreach (PacketItem item in PacketItem.Enum(this))
				{
					if (item.Type == "Ore_5")
					{
						return item.Count;
					}
				}
				return 0;
			}
		}

		private bool IsNhanBinhMau { get; set; }

		public bool IsHold { get; set; }

		public bool IsSale { get; set; }

		public bool IsThienKiepLau { get; set; }

		public float SaveX { get; set; }

		public float SaveY { get; set; }

		public int RecvData { get; set; }

		public int RecvAddress { get; set; }

		public int MyRecvAddress { get; set; }

		public int KetQuaSetTitle { get; set; }

		public string PetId { get; set; }

		public int RecvSize { get; set; }

		public string RecvDat { get; set; }

		public int MapAcTac
		{
			get
			{
				if (mapAcTac == 0)
				{
					return 0;
				}
				if (TLBB.MapId == MAP.ThaiHo || TLBB.MapId == MAP.KiemCac || TLBB.MapId == MAP.KinhHo || TLBB.MapId == MAP.TungSon || TLBB.MapId == MAP.DonHoang)
				{
					return TLBB.MapId;
				}
				return mapAcTac;
			}
			set
			{
				mapAcTac = value;
			}
		}

		public int MapTKC
		{
			get
			{
				if (mapTKC == 0)
				{
					return 0;
				}
				if (TLBB.MapId == MAP.TayHo || TLBB.MapId == MAP.NhiHai || TLBB.MapId == MAP.NhanNam)
				{
					return TLBB.MapId;
				}
				return mapTKC;
			}
			set
			{
				mapTKC = value;
			}
		}

		private int AddressCount { get; set; }

		private bool isAlarmTrungAc { get; set; }

		public Stopwatch HongTrungAcTime { get; set; }

		private bool IsShowCap { get; set; }

		public Stopwatch swCaptchaTime { get; set; }

		private bool isAlarmHuyetMo { get; set; }

		private bool isAlarmDayTayNai { get; set; }

		private bool IsDeadEx
		{
			get
			{
				if (!IsBachHoaDuyen && !IsTrungAc)
				{
					if (TLBB.MapId != MAP.ThieuThatSon && TLBB.MapId != MAP.SinhTuLoiDai && TLBB.MapId != MAP.BinhThanhKyTran)
					{
						return AutoHoiSinh;
					}
					return false;
				}
				return true;
			}
		}

		public bool IsPhuMau { get; set; }

		public List<Game> Party
		{
			get
			{
				List<Game> list = new List<Game>();
				foreach (KeyValuePair<int, Game> item in FrmMain.dicGame)
				{
					Game value = item.Value;
					if (value == this)
					{
						list.Add(value);
					}
					if (value.TLBB.KeyId == TLBB.KeyId)
					{
						list.Add(value);
					}
				}
				return list;
			}
		}

		public Game Leader
		{
			get
			{
				foreach (Game item in Party)
				{
					if (item.TLBB.IsLeader)
					{
						return item;
					}
				}
				return null;
			}
		}

		public int DaChe { get; set; }

		public int DaHuy { get; set; }

		public int TempCount { get; set; }

		public bool Kiemtranguyenlieu { get; set; }

		public string TaskSauCheDO { get; set; }

		public static bool Is69DO { get; set; }

		public List<int> TmpItemBeforeChe { get; set; }

		public int TocDoChe { get; set; }

		public bool IsNhanHoaHongLo { get; set; }

		public bool IsNhanQuaBuiHoaHong { get; set; }

		public Stopwatch TimeKiemTra { get; set; }

		[CompilerGenerated]
		public event EventHandler SettingLoaded;

		[CompilerGenerated]
		public event EventHandler SkillLoaded;

		public Game(Process process, Address address)
		{
			ProcessId = process.Id;
			AOB = new AOBScan((uint)ProcessId);
			Process = process;
			Address = address;
			Memory = new Memory(ProcessId);
			Handle = Win.GetHandle(ProcessId, Win.WndClassNames);
			TLBB = new TLBB(this);
			Objects = new GameObjects(this);
			Init();
			try
			{
				LoadSetting();
			}
			catch (Exception)
			{
			}
			LUA = new LUA(this);
			QuestFrame = new QuestFrame(this);
			Packet = new PacketItem(this);
		}

		public void Sleep(int delay)
		{
			PostMessage(delay, 125);
		}

		public int ConverTime(DateTime dt)
		{
			return (int)(dt - new DateTime(1970, 1, 1)).TotalSeconds;
		}

		public void AutoAnVatPham()
		{
			if (!AutoEatVatPham || TudongAn.Count == 0 || TickCount % 600 != 0)
			{
				return;
			}
			int num = 0;
			try
			{
				foreach (AutoEat item in TudongAn)
				{
					DateTime now = DateTime.Now;
					int num2 = item.TimeEach * 60;
					int num3 = ConverTime(now);
					int num4 = ConverTime(item.StartEat);
					if (num3 - num4 >= num2)
					{
						item.StartEat = DateTime.Now;
						UpdateList(num, item);
						UsingVatPhamByName(item.VatPhamName, DateTime.Now.AddSeconds(num2));
					}
					num++;
				}
			}
			catch
			{
			}
		}

		public void UpdateList(int ID, AutoEat eat)
		{
			AutoEat autoEat = new AutoEat();
			autoEat.ID = eat.ID;
			autoEat.StartEat = DateTime.Now;
			autoEat.TimeEach = eat.TimeEach;
			autoEat.VatPhamName = eat.VatPhamName;
			TudongAn[ID] = autoEat;
		}

		public void UsingVatPhamByName(string name, DateTime dt)
		{
			foreach (PacketItem item in Packet.DaoCu)
			{
				if (TINHKIEM.VietLien(item.Name) == TINHKIEM.VietLien(name))
				{
					PlayerPackageUseItem(item.Index);
					PushDebugMessage("Auto tự ăn vật phẩm :" + item.Name + " Vào lúc [" + dt.ToString() + "] sẽ sử dụng tiếp");
					FrmMain.AddLog(DateTime.Now.ToString("HH:mm dd-MM") + " " + LastName + "Auto tự ăn vật phẩm :" + item.Name + " Vào lúc [" + dt.ToString() + "] sẽ sử dụng tiếp\n");
					break;
				}
			}
		}

		public object getCPUCounter()
		{
			PerformanceCounter performanceCounter = new PerformanceCounter();
			performanceCounter.CategoryName = "Processor";
			performanceCounter.CounterName = "% Processor Time";
			performanceCounter.InstanceName = "_Total";
			performanceCounter.NextValue();
			return performanceCounter.NextValue();
		}

		public void HamLenBaiTrian()
		{
			if (!LenBaiTrain)
			{
				return;
			}
			if (IsAttack)
			{
				IsAttack = false;
			}
			int mapID = _baitrain.MapID;
			int posX = _baitrain.PosX;
			int posY = _baitrain.PosY;
			if (TLBB.MapId != mapID)
			{
				if (!GoTo(posX, posY, mapID))
				{
					TimDuong(posX, posY, mapID);
				}
			}
			else if (TINHKIEM.GetDistance(CharX, CharY, posX, posY) > 2f)
			{
				Move(posX, posY, mapID);
			}
			else
			{
				DownRide();
				IsAttack = true;
				LenBaiTrain = false;
				PushThongBao("Thông báo", "Lên Bãi Thành Công", CanhBao.Kieu.OK);
			}
		}

		public void PushThongBao(string _teude, string _noidung, CanhBao.Kieu _Type)
		{
			ThongBao thongBao = new ThongBao();
			thongBao.tideu = _teude;
			thongBao.noidung = _noidung;
			thongBao.Type = _Type;
			ListThongBao.Add(thongBao);
		}

		public void PostMessage(int wParam, int lParam)
		{
			Win.PostMessage(Handle, Global.HookMessage, wParam, lParam);
		}

		public bool LamDayTayNai()
		{
			if (TLBB.IsODaoCuFull && TLBB.IsONguyenLieuFull)
			{
				return true;
			}
			if (!TLBB.IsODaoCuFull)
			{
				foreach (PacketItem item in Packet.DaoCu)
				{
					if (item.Count > 1)
					{
						Packet.SplitIndex = item.Index;
						LuaDoOneLineString("PlayerPackage:SplitItem(1);");
						return false;
					}
				}
				PushDebugMessage("Không thể làm đầy tay nải");
				PushThongBao("Thông báo", "Không thể làm đầy tay nải", CanhBao.Kieu.Eror);
				IsMoBTD = false;
				return false;
			}
			if (!TLBB.IsONguyenLieuFull)
			{
				foreach (PacketItem item2 in Packet.NguyenLieu)
				{
					if (item2.Count > 1)
					{
						Packet.SplitIndex = item2.Index;
						LuaDoOneLineString("PlayerPackage:SplitItem(1);");
						return false;
					}
				}
				PushDebugMessage("Không thể làm đầy tay nải");
				PushThongBao("Thông báo", "Không thể làm đầy tay nải", CanhBao.Kieu.Eror);
				IsMoBTD = false;
				return false;
			}
			return false;
		}

		public void TrongTrot()
		{
			if (TickCount % 18 != 0 || TimeStand.Elapsed.TotalSeconds < 2.0 || TLBB.PlayerState != 0 || !IsTrongTrot)
			{
				return;
			}
			if (!IsThuHoach)
			{
				if (TrangThaiTrongTrot == string.Empty)
				{
					float num = 100f;
					int num2 = -1;
					if (num2 == -1)
					{
						foreach (GameObject item in Objects.All)
						{
							if ((TINHKIEM.VietLien(item.Name).Contains("nguoirom") || TINHKIEM.VietLien(item.Name).Contains("daothaonhan")) && TINHKIEM.GetDistance(item.X, item.Y, CharX, CharY) < num && !lstNguoiRom.Contains(item.Id))
							{
								num = TINHKIEM.GetDistance(item.X, item.Y, CharX, CharY);
								num2 = item.Id;
							}
						}
					}
					if (num2 != -1)
					{
						lstNguoiRom.Add(num2);
						Talk(num2);
						TrangThaiTrongTrot = "Talk";
						return;
					}
					if (TrangThaiTrongTrot == string.Empty)
					{
						lstNguoiRom.Clear();
					}
				}
				else if (TrangThaiTrongTrot == "Talk")
				{
					if (TrongTrotIndex == 0)
					{
						foreach (QuestFrame item2 in QuestFrame.Enum(this))
						{
							if (item2.Name.EndsWith("sớm"))
							{
								QuestFrameOptionClicked(item2);
								TrangThaiTrongTrot = "Talk1";
								return;
							}
						}
					}
					if (TrongTrotIndex == 1)
					{
						foreach (QuestFrame item3 in QuestFrame.Enum(this))
						{
							if (item3.Name.EndsWith("muộn"))
							{
								QuestFrameOptionClicked(item3);
								TrangThaiTrongTrot = "Talk1";
								return;
							}
						}
					}
				}
				else if (TrangThaiTrongTrot == "Talk1")
				{
					List<QuestFrame> list = QuestFrame.Enum(this);
					if (list.Count > ThuHoachIndex + 1)
					{
						QuestFrame dialog = list[ThuHoachIndex + 1];
						TrangThaiTrongTrot = string.Empty;
						QuestFrameOptionClicked(dialog);
						return;
					}
				}
				TrangThaiTrongTrot = "";
				return;
			}
			float num3 = 100f;
			int num4 = -1;
			foreach (GameObject item4 in Objects.All)
			{
				item4.DistanceEx = TINHKIEM.GetDistance(CharX, CharY, item4.X, item4.Y);
				if (item4.IsTaiNguyen && (double)item4.DistanceEx <= 4.5 && !item4.Name.Contains("("))
				{
					PickItem(item4.Id);
					return;
				}
			}
			if (TrangThaiTrongTrot == string.Empty)
			{
				if (num4 == -1)
				{
					foreach (GameObject item5 in Objects.All)
					{
						if ((TINHKIEM.VietLien(item5.Name).Contains("nguoirom") || TINHKIEM.VietLien(item5.Name).Contains("daothaonhan")) && TINHKIEM.GetDistance(item5.X, item5.Y, CharX, CharY) < num3)
						{
							num3 = TINHKIEM.GetDistance(item5.X, item5.Y, CharX, CharY);
							num4 = item5.Id;
						}
					}
				}
				if (num4 != -1)
				{
					Talk(num4);
					TrangThaiTrongTrot = "Talk";
					return;
				}
			}
			else if (TrangThaiTrongTrot == "Talk")
			{
				if (TrongTrotIndex == 0)
				{
					foreach (QuestFrame item6 in QuestFrame.Enum(this))
					{
						if (item6.Name.EndsWith("sớm"))
						{
							QuestFrameOptionClicked(item6);
							TrangThaiTrongTrot = "Talk1";
							return;
						}
					}
				}
				if (TrongTrotIndex == 1)
				{
					foreach (QuestFrame item7 in QuestFrame.Enum(this))
					{
						if (item7.Name.EndsWith("muộn"))
						{
							QuestFrameOptionClicked(item7);
							TrangThaiTrongTrot = "Talk1";
							return;
						}
					}
				}
			}
			else if (TrangThaiTrongTrot == "Talk1")
			{
				List<QuestFrame> list2 = QuestFrame.Enum(this);
				if (list2.Count > ThuHoachIndex + 1)
				{
					QuestFrame dialog2 = list2[ThuHoachIndex + 1];
					QuestFrameOptionClicked(dialog2);
					TrangThaiTrongTrot = string.Empty;
					return;
				}
			}
			TrangThaiTrongTrot = string.Empty;
		}

		public bool TrongPhamVi(int x, int y, int map)
		{
			if (TLBB.MapId != map)
			{
				Move(x, y, map);
				return false;
			}
			if (TINHKIEM.GetDistance(CharX, CharY, x, y) > 15f)
			{
				Move(x, y);
				return false;
			}
			return true;
		}

		public bool TrongPhamVi(int x, int y)
		{
			if (TINHKIEM.GetDistance(CharX, CharY, x, y) > 15f)
			{
				Move(x, y);
				return false;
			}
			return true;
		}

		public bool DaDenNoi(int x, int y, int map)
		{
			if (TLBB.MapId == map)
			{
				if (TINHKIEM.GetDistance(CharX, CharY, x, y) <= 3f)
				{
					return true;
				}
				if (!IsRide && TLBB.HaveRide)
				{
					UpRide();
					return false;
				}
				Move(x, y);
				return false;
			}
			if (UsingTholinhChau)
			{
				if (PhuIndex(map) != -1)
				{
					if (IsRide)
					{
						Ride();
						return false;
					}
					PlayerPackageUseItem(PhuIndex(map));
					return false;
				}
				return false;
			}
			if (!IsRide && TLBB.HaveRide)
			{
				UpRide();
				return false;
			}
			Move(x, y, map);
			return false;
		}

		public bool GoToBang()
		{
			if (TenMapThanh() == "-1")
			{
				BangOpen();
				IsMoBang = true;
			}
			string[] array = TINHKIEM.GetBangXY(TenMapThanh()).Split(',');
			return GoTo(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]));
		}

		public bool GoTo(float x, float y, bool force = false)
		{
			if (TLBB.MapId != MAP.ViemMaSon || !TLBB.IsFollow || TLBB.IsLeader)
			{
				return GoTo(x, y, -1, force);
			}
			return true;
		}

		public bool GoTo(NPC npc)
		{
			return GoTo(npc.X, npc.Y, npc.Map);
		}

		public void TalkTo(NPC npc)
		{
			if (npc != null && GoTo(npc))
			{
				Talk(npc);
			}
		}

		public bool GoTo(float x, float y, int map, bool force = false)
		{
			if (TLBB.MapId == MAP.ViemMaSon && TLBB.IsFollow && !TLBB.IsLeader)
			{
				return true;
			}
			if (TLBB.PlayerState == 2 && !force)
			{
				return false;
			}
			if (TLBB.MapId == map || map == -1)
			{
				if ((double)TINHKIEM.GetDistance(RoundX, RoundY, x, y) < 1.5)
				{
					MoveExTime = Stopwatch.StartNew();
					ListMoveEx.Clear();
					return true;
				}
				if (TLBB.IsFollow && !TLBB.IsLeader)
				{
					StopFollow();
				}
				if (TINHKIEM.GetDistance(RoundX, RoundY, x, y) > 20f && !IsNhatHopQDua && !IsRide && TLBB.HaveRide && !TLBB.IsBienThan)
				{
					UpRide();
					return false;
				}
				if (!IsMoveEx)
				{
					FixKetMap();
					return false;
				}
				Move(x, y);
			}
			else if (UsingTholinhChau)
			{
				if (PhuIndex(map) != -1)
				{
					if (IsRide)
					{
						DownRide();
						return false;
					}
					PlayerPackageUseItem(PhuIndex(map));
				}
				else
				{
					if (TLBB.IsFollow && !TLBB.IsLeader)
					{
						StopFollow();
					}
					if (!IsNhatHopQDua && !IsRide && TLBB.HaveRide && !TLBB.IsBienThan)
					{
						UpRide();
						return false;
					}
					if (!IsMoveEx)
					{
						FixKetMap();
						return false;
					}
					if (TLBB.MapId > 500 && TLBB.MapId < 545)
					{
						RaBang(0);
						return false;
					}
					Move(x, y, map);
				}
			}
			else
			{
				if (TLBB.IsFollow && !TLBB.IsLeader)
				{
					StopFollow();
				}
				if (!IsNhatHopQDua && !IsRide && TLBB.HaveRide && !TLBB.IsBienThan)
				{
					UpRide();
					return false;
				}
				if (!IsMoveEx)
				{
					FixKetMap();
					return false;
				}
				if (TLBB.MapId > 500 && TLBB.MapId < 545)
				{
					RaBang(0);
					return false;
				}
				Move(x, y, map);
			}
			return false;
		}

		public void TimDuong(float InputX, float InputY, int MapID)
		{
			if (lastAutoMove.Elapsed.TotalSeconds < 4.0)
			{
				return;
			}
			if (Unity.IsMessengerBox(TLBB.MapId, (int)CharX, (int)CharY))
			{
				LuaDoOneLineString("IsMessageBox = 1;");
			}
			else
			{
				if (ON_SCENE_TRANSING || TLBB.PlayerState == 7)
				{
					return;
				}
				if (TLBB.MapId != 6 && TLBB.MapId != 7 && TLBB.MapId != 24 && MapID == 2 && TLBB.MapId != 2)
				{
					DownRide();
					UseSkill(22);
					return;
				}
				if (!IsMove)
				{
					FixKetMap();
					return;
				}
				if (TLBB.MapId == MapID)
				{
					if (!(TINHKIEM.GetDistance(CharX, CharY, InputX, InputY) <= 3f))
					{
						Move(InputX, InputY);
					}
					return;
				}
				if (UsingTholinhChau)
				{
					int num = PhuIndex(Unity.GetFakeMapID(MapID));
					if (num != -1)
					{
						if (IsRide)
						{
							DownRide();
						}
						else
						{
							PlayerPackageUseItem(num);
						}
						return;
					}
				}
				if (TLBB.HaveRide && !IsRide)
				{
					UpRide();
					return;
				}
				PathInfo pathInfo = null;
				try
				{
					pathInfo = FindPath.GetNextPath(TLBB.MapId, MapID);
				}
				catch
				{
					PushDebugMessage("Không thể tìm đường");
					return;
				}
				if (!pathInfo.isNPC)
				{
					Move(pathInfo.x, pathInfo.y);
					return;
				}
				string value = "KhongCo";
				bool flag = false;
				switch (pathInfo.idNext)
				{
				case 9:
					value = "Thiếu Lâm";
					flag = true;
					break;
				case 11:
					value = "Minh Giáo";
					flag = true;
					break;
				case 10:
					value = "Cái Bang";
					flag = true;
					break;
				case 16:
					value = "Tinh Túc";
					flag = true;
					break;
				case 12:
					value = "Võ Đang";
					flag = true;
					break;
				case 13:
					value = "Thiên Long";
					flag = true;
					break;
				case 17:
					value = "Thiên Sơn";
					flag = true;
					break;
				case 15:
					value = "Nga My";
					flag = true;
					break;
				case 14:
					value = "Tiêu Dao";
					flag = true;
					break;
				case 284:
					value = "Mộ Dung";
					flag = true;
					break;
				}
				Move(pathInfo.x, pathInfo.y);
				if (flag)
				{
					foreach (QuestFrame item in QuestFrame.Enum(this))
					{
						if (item.Name.Contains(value))
						{
							QuestFrameOptionClicked(item);
							break;
						}
					}
					foreach (QuestFrame item2 in QuestFrame.Enum(this))
					{
						if (item2.Name.Contains("Đến các môn phái"))
						{
							QuestFrameOptionClicked(item2);
							return;
						}
					}
				}
				if (!flag)
				{
					foreach (QuestFrame item3 in QuestFrame.Enum(this))
					{
						if (item3.Name.Contains("Duyệt") || item3.Name.Contains("Xác nhận"))
						{
							QuestFrameOptionClicked(item3);
							break;
						}
					}
					value = FindPath.GetScreenName(pathInfo.idNext);
					if (TINHKIEM.VietLien(value).Contains("thuchacotran"))
					{
						LuaDoOneLineString("IsMessageBox = 1;");
						return;
					}
					foreach (QuestFrame item4 in QuestFrame.Enum(this))
					{
						string text = TINHKIEM.VietLienRemoveNum(item4.Name);
						string value2 = TINHKIEM.VietLienRemoveNum(value);
						if (text.Contains(value2))
						{
							QuestFrameOptionClicked(item4);
							break;
						}
					}
				}
				if (lastTalk.Elapsed.TotalSeconds > 5.0)
				{
					foreach (GameObject item5 in Objects.AllNpc)
					{
						if (TINHKIEM.VietLienRemoveNum(item5.Name).Contains(TINHKIEM.VietLienRemoveNum(pathInfo.NpcName)))
						{
							Talk(item5.Id);
							lastTalk = Stopwatch.StartNew();
							break;
						}
					}
				}
				lastAutoMove = Stopwatch.StartNew();
			}
		}

		public bool ChuaThuHoachDuoc(int x, int y)
		{
			if (DaThuHoachDuoc(x, y))
			{
				return false;
			}
			foreach (long[] item in ListHoaTruongThanh)
			{
				if (item[1] == x && item[2] == y && DateTime.Now.Ticks / 10000000 - item[0] < 0)
				{
					return true;
				}
			}
			return false;
		}

		public bool HaveHoa(int x, int y)
		{
			foreach (long[] item in ListHoaTruongThanh)
			{
				if (item[1] == x && item[2] == y)
				{
					return true;
				}
			}
			return false;
		}

		public bool DaThuHoachDuoc(int x, int y)
		{
			foreach (long[] item in ListHoaXuatHien)
			{
				if (item[1] == x && item[2] == y && DateTime.Now.Ticks / 10000000 - item[0] > 0)
				{
					return true;
				}
			}
			return false;
		}

		public void RemoveHoa(int x, int y)
		{
			foreach (long[] item in ListHoaTruongThanh)
			{
				if (item[1] == x && item[2] == y)
				{
					ListHoaTruongThanh.Remove(item);
				}
			}
			foreach (long[] item2 in ListHoaXuatHien)
			{
				if (item2[1] == x && item2[2] == y)
				{
					ListHoaXuatHien.Remove(item2);
				}
			}
		}

		public void SetTime(int x, int y, long time)
		{
			foreach (long[] item in ListHoaTruongThanh)
			{
				if (item[1] == x && item[2] == y)
				{
					item[0] = time;
				}
			}
		}

		public void MoBTD()
		{
			if (TLBB.MapId == MAP.HuyetMo)
			{
				TrangThaiBTD = "";
				BTDX = (BTDY = (BTDMAP = (BTDIndex = -1)));
			}
			else
			{
				if (!IsMoBTD || TickCount % 18 != 0)
				{
					return;
				}
				if (!HaveItem("Merchandise4_16"))
				{
					PushDebugMessage("Không có bảo tàng đồ.");
					PushThongBao("Thông báo", "Không có tàng bảo đồ", CanhBao.Kieu.Eror);
					IsMoBTD = false;
					return;
				}
				if (TrangThaiBTD == "")
				{
					foreach (PacketItem item in Packet.DaoCu)
					{
						if (item.Type == "Merchandise4_16")
						{
							PlayerPackageUseItem(item.Index);
							TrangThaiBTD = "GetInfo";
							BTDIndex = item.Index;
							return;
						}
					}
					BTDX = (BTDY = (BTDMAP = (BTDIndex = -1)));
				}
				if (TrangThaiBTD == "GetInfo" && TLBB.IsQuestOpen)
				{
					BTDInfo = QuestFrame.All(this);
					BTDMAP = TINHKIEM.GetMapId(BTDInfo);
					if (BTDMAP != -1)
					{
						BTDInfo = Regex.Replace(BTDInfo, ".*_INFOAIM", "");
						int num = BTDInfo.IndexOf("[");
						int length = BTDInfo.IndexOf("]") - num;
						string text = BTDInfo.Substring(num, length).Replace("[", "");
						if (text.Split(',').Length > 1)
						{
							BTDX = TINHKIEM.ParseInt(text.Split(',')[0]);
							BTDY = TINHKIEM.ParseInt(text.Split(',')[1]);
						}
					}
					CloseQuest();
					if (BTDX != -1 && BTDY != -1 && BTDMAP != -1)
					{
						TrangThaiBTD = "Do";
					}
				}
				if (TrangThaiBTD == "Do")
				{
					if (!GoTo(BTDX, BTDY, BTDMAP))
					{
						return;
					}
					foreach (PacketItem item2 in Packet.DaoCu)
					{
						if (item2.Index == BTDIndex && item2.Type == "Merchandise4_16")
						{
							PlayerPackageUseItem(BTDIndex);
						}
					}
					TrangThaiBTD = "";
				}
				else
				{
					TrangThaiBTD = "";
				}
			}
		}

		public void TrungAc()
		{
			if (Global.Paused || TickCount % 12 != 0 || !IsTrungAc || TLBB.MapId == MAP.GiamNguc || ForcePickItem() || TLBB.PlayerState != 0)
			{
				return;
			}
			if (State != STATE.Done && State != STATE.TalkToCompleteMission && State != STATE.MissionContinute && State != STATE.MissionComplete)
			{
				foreach (Task item in Task.Enum(this))
				{
					if (item.Name.Contains("#{CXDT_090304_01}") && item.Completed)
					{
						State = STATE.Done;
						break;
					}
				}
			}
			if (State == STATE.HuyQ)
			{
				MessageboxSelfOkClicked();
				State = STATE.Null;
				return;
			}
			if (State == STATE.Null || State == STATE.None)
			{
				foreach (PacketItem item2 in PacketItem.Enum(this))
				{
					if (TINHKIEM.VietLien(item2.Name).Contains("trungaclenh"))
					{
						PlayerPackageUseItem(item2.Index);
						State = STATE.GetInfo;
						IsHong = false;
						return;
					}
				}
			}
			if (State == STATE.Null || State == STATE.Done)
			{
				if (TLBB.MapId != 1)
				{
					TimDuong(224f, 226f, 1);
					return;
				}
				if (TINHKIEM.GetDistance(CharX, CharY, 224f, 226f) > 3f)
				{
					Move(224f, 226f);
					return;
				}
				{
					foreach (GameObject item3 in Objects.AllNpc)
					{
						if (TINHKIEM.VietLien(item3.Name).Contains("ngogioi"))
						{
							Talk(item3.Id);
							if (State == STATE.Null)
							{
								State = STATE.TalkToAcceptMission;
							}
							else
							{
								State = STATE.TalkToCompleteMission;
							}
							break;
						}
					}
					return;
				}
			}
			if (State == STATE.None || State == STATE.CheckComplete)
			{
				foreach (PacketItem item4 in PacketItem.Enum(this))
				{
					if (TINHKIEM.VietLien(item4.Name).Contains("trungaclenh"))
					{
						PlayerPackageUseItem(item4.Index);
						State = STATE.GetInfo;
						IsHong = false;
						return;
					}
				}
				if (State == STATE.CheckComplete && QuestFrame.All(this).Contains("#{CXDY_090423_01}") && QuestFrame.All(this).Contains("#{CXDY_090423_02}"))
				{
					IsXongTrungAc = true;
					IsTrungAc = false;
					return;
				}
				if (State == STATE.None)
				{
					IsHong = true;
				}
				if (TLBB.IsTogleMission)
				{
					TogleMission();
					PostMessage(30, 105);
					State = STATE.GetMissionInfo;
					LuaToString();
				}
				else
				{
					TogleMission();
				}
				return;
			}
			if (State == STATE.TalkToAcceptMission)
			{
				foreach (QuestFrame item5 in QuestFrame.Enum(this))
				{
					if (item5.Name == "#{CXDT_090304_01}")
					{
						QuestFrameOptionClicked(item5);
						State = STATE.CheckComplete;
						return;
					}
				}
			}
			if (State == STATE.TalkToCompleteMission)
			{
				foreach (QuestFrame item6 in QuestFrame.Enum(this))
				{
					if (item6.Name == "#{CXDT_090304_01}")
					{
						QuestFrameOptionClicked(item6);
						State = STATE.MissionContinute;
						return;
					}
				}
			}
			if (State == STATE.MissionContinute)
			{
				PostMessage(14, 105);
				State = STATE.MissionComplete;
				return;
			}
			if (State == STATE.MissionComplete)
			{
				QuestFrameMissionComplete();
				State = STATE.Null;
				return;
			}
			if (State == STATE.GetInfo)
			{
				TrungAcInfo = QuestFrame.All(this);
				MissionMap = TINHKIEM.GetMapId(TrungAcInfo);
				int num = TrungAcInfo.IndexOf("[");
				int length = TrungAcInfo.IndexOf("]") - num;
				string text = TrungAcInfo.Substring(num, length).Replace("[", "");
				if (MissionMap != -1 && text.Split(',').Length != 0)
				{
					MissionX = TINHKIEM.ParseInt(text.Split(',')[0]);
					MissionY = TINHKIEM.ParseInt(text.Split(',')[1]);
					State = STATE.Do;
					return;
				}
			}
			if (State == STATE.GetMissionInfo)
			{
				string text2 = LuaString();
				if (text2 == "Xong")
				{
					State = STATE.Done;
					IsHong = false;
					IsXongTrungAc = false;
				}
				else if (text2 == "Chua")
				{
					State = STATE.Null;
					IsHong = false;
					IsXongTrungAc = false;
				}
				else
				{
					State = STATE.None;
				}
				return;
			}
			if (State == STATE.Do)
			{
				if (TLBB.MapId != MissionMap)
				{
					TimDuong(MissionX, MissionY, MissionMap);
					return;
				}
				if (TINHKIEM.GetDistance(CharX, CharY, MissionX, MissionY) > 3f)
				{
					Move(MissionX, MissionY);
					return;
				}
				if (DaDenNoi(MissionX, MissionY, MissionMap))
				{
					State = STATE.Come;
				}
				comeTime = Stopwatch.StartNew();
				doneTime = null;
				return;
			}
			if (State == STATE.TalkToXaPhu)
			{
				if (TLBB.MapId == LACDUONG.Id)
				{
					Extra1 = 400956;
				}
				else if (TLBB.MapId == 1)
				{
					Extra1 = 400957;
				}
				else if (TLBB.MapId == 2)
				{
					Extra1 = 400958;
				}
				else if (TLBB.MapId == 246)
				{
					Extra1 = 400959;
				}
				QuestFrameOptionClicked(Extra1, TINHKIEM.GetTruyen(TrungAcInfo));
				State = STATE.DongY;
				return;
			}
			if (State == STATE.DongY)
			{
				foreach (QuestFrame item7 in QuestFrame.Enum(this))
				{
					if (TINHKIEM.VietLien(item7.Name).Contains("dongy"))
					{
						QuestFrameOptionClicked(item7);
						State = STATE.Do;
						return;
					}
				}
			}
			if (State == STATE.Come)
			{
				if (!TrongPhamVi(MissionX, MissionY, MissionMap))
				{
					return;
				}
				foreach (PacketItem item8 in PacketItem.Enum(this))
				{
					if (TINHKIEM.VietLien(item8.Name).Contains("trungaclenh"))
					{
						PlayerPackageUseItem(item8.Index);
						return;
					}
				}
				if (TLBB.MapId == MAP.ThaoNguyen)
				{
					ForcePickItem();
					{
						foreach (GameObject item9 in Objects.Near20m)
						{
							if (!(item9.Title != "") || item9.Menpai != 28)
							{
								continue;
							}
							if (item9.HP > 0f)
							{
								if (IsRide)
								{
									DownRide();
									break;
								}
								SelectTarget(item9.Id);
								SendKey(Global.BaseSkill);
								doneTime = null;
								break;
							}
							if (doneTime == null)
							{
								doneTime = Stopwatch.StartNew();
							}
							if (TLBB.MapId <= 2 || doneTime.Elapsed.TotalSeconds > 10.0)
							{
								comeTime = null;
								State = STATE.Done;
							}
						}
						return;
					}
				}
				ForcePickItem();
				{
					foreach (GameObject item10 in Objects.Near20m)
					{
						if (item10.Menpai != 28 || TINHKIEM.NumDiff(item10.Lvl, TLBB.Lvl) > 5)
						{
							continue;
						}
						if (item10.HP > 0f)
						{
							if (IsRide)
							{
								DownRide();
								break;
							}
							SelectTarget(item10.Id);
							SendKey(Global.BaseSkill);
							doneTime = null;
						}
						else
						{
							if (doneTime == null)
							{
								doneTime = Stopwatch.StartNew();
							}
							if (TLBB.MapId <= 2 || doneTime.Elapsed.TotalSeconds > 10.0)
							{
								State = STATE.Done;
								comeTime = null;
							}
						}
					}
					return;
				}
			}
			State = STATE.Null;
		}

		private void QuestFrameMissionContinue()
		{
			PostMessage(14, 105);
		}

		private void QuestFrameMissionComplete()
		{
			PostMessage(15, 105);
		}

		public bool InDistance(NPC npc)
		{
			if (TLBB.MapId == npc.Map)
			{
				return InDistance(npc.X, npc.Y);
			}
			return false;
		}

		public bool InDistance(float x, float y)
		{
			return TINHKIEM.GetDistance(CharX, CharY, x, y) <= 3f;
		}

		public NPC NearestObject(string name)
		{
			NPC nPC = new NPC();
			float num = 9999f;
			foreach (GameObject item in Objects.All)
			{
				if (TINHKIEM.VietLien(item.Name).Contains(TINHKIEM.VietLien(name)) && TINHKIEM.GetDistance(CharX, CharY, item.X, item.Y) < num)
				{
					num = TINHKIEM.GetDistance(CharX, CharY, item.X, item.Y);
					nPC.Id = item.Id;
					nPC.X = (int)item.X;
					nPC.Y = (int)item.Y;
				}
			}
			return nPC;
		}

		public void AskTeamFollowEx()
		{
			if (!IsRide && TLBB.HaveRide)
			{
				UpRide();
			}
			else
			{
				PostMessage(21, 105);
			}
		}

		public void AskTeamFollow()
		{
			PostMessage(21, 105);
		}

		public void StopFollow()
		{
			PostMessage(20, 105);
		}

		public NPC HoaGanNhat()
		{
			NPC nPC = new NPC();
			float num = 9999f;
			foreach (GameObject item in Objects.All)
			{
				if ((!FrmMain.IsFixed || (item.X >= (float)(TrongHoaX - 15 - 3) && item.X <= (float)(TrongHoaX + FrmMain.MaxHoaX - 15 + 3))) && !HashDangBon.Contains(item.Id) && !ListDaBon.Contains(item.Id) && item.Title.ToString() != "")
				{
					FrmMain.AllName.Contains(item.Title.Split('#')[0]);
					if (item.Name.Contains("Tiên Hoa Ấu Miêu") && !TINHKIEM.VietLien(TINHKIEM.ReadFile("D:\\bl.txt")).Contains(TINHKIEM.VietLien(item.Title.Substring(0, item.Title.IndexOf("#")))) && num > TINHKIEM.GetDistance(CharX, CharY, item.X, item.Y))
					{
						num = TINHKIEM.GetDistance(CharX, CharY, item.X, item.Y);
						nPC.Id = item.Id;
						nPC.X = (int)item.X;
						nPC.Y = (int)item.Y;
					}
				}
			}
			return nPC;
		}

		public void ThuHoachBHD()
		{
			if (!IsThuHoachHoa)
			{
				return;
			}
			float num = 9999f;
			GameObject gameObject = null;
			foreach (GameObject item in Objects.All)
			{
				if (item.Title.ToString() != "" && item.Name.Contains("Hoa Trưởng Thành"))
				{
					float distance = TINHKIEM.GetDistance(CharX, CharY, item.X, item.Y);
					if (!ListBHDXuatHien.ContainsKey(item.Id))
					{
						ListBHDXuatHien.Add(item.Id, DateTime.Now.AddSeconds(180.0));
					}
					if (((ListThuHoachBHD.ContainsKey(item.Id) && (float)(ListThuHoachBHD[item.Id].Ticks - DateTime.Now.Ticks) - distance / TLBB.RunSpeed < 0f) || (float)(ListBHDXuatHien[item.Id].Ticks - DateTime.Now.Ticks) - distance / TLBB.RunSpeed < 0f) && distance < num)
					{
						num = distance;
						gameObject = item;
					}
				}
			}
			if (gameObject != null)
			{
				if (GoTo(gameObject.X, gameObject.Y))
				{
					if (IsRide)
					{
						DownRide();
					}
					UseSkill(3, gameObject.Id);
				}
				return;
			}
			num = 9999f;
			foreach (GameObject item2 in Objects.All)
			{
				if (item2.Title.ToString() != "" && item2.Name.Contains("Hoa Trưởng Thành") && !ListThuHoachBHD.ContainsKey(item2.Id))
				{
					float distance2 = TINHKIEM.GetDistance(CharX, CharY, item2.X, item2.Y);
					if (distance2 < num)
					{
						num = distance2;
						gameObject = item2;
					}
				}
			}
			if (gameObject == null || !GoTo(gameObject.X, gameObject.Y))
			{
				return;
			}
			if (TLBB.IsQuestOpen && QuestFrame.All(this).Contains("#{SDJZH_091106_08}"))
			{
				dialogInfo = QuestFrame.All(this).Replace("#{SDJZH_091106_08}", "");
				int num2 = TINHKIEM.ParseInt(dialogInfo);
				ListThuHoachBHD.Add(gameObject.Id, DateTime.Now.AddSeconds(num2 - 1));
				CloseQuest();
			}
			else
			{
				if (IsRide)
				{
					DownRide();
				}
				UseSkill(3, gameObject.Id);
			}
		}

		public bool IsClearArea()
		{
			if (TINHKIEM.NumDiff(TrongHoaX - 15, BachHoaDuyenX) >= FrmMain.MaxHoaX)
			{
				return false;
			}
			if (BlackListHoa.Contains(BachHoaDuyenX + "," + BachHoaDuyenY))
			{
				return false;
			}
			foreach (GameObject item in Objects.All)
			{
				if (item.Name.Contains("Tiên Hoa Ấu Miêu") || item.Name.Contains("Hoa Trưởng Thành") || item.IsNPC)
				{
					item.DistanceEx = item.GetDistance(BachHoaDuyenX, BachHoaDuyenY);
					if (item.Y - (float)BachHoaDuyenY < 2f && item.Y - (float)BachHoaDuyenY > -2f && (double)item.DistanceEx <= 2.5)
					{
						BlackListHoa.Add(BachHoaDuyenX + "," + BachHoaDuyenY);
						return false;
					}
				}
			}
			return true;
		}

		public bool IsSafeArea()
		{
			if (TLBB.MapId == MAP.PhungMinhVuongLang)
			{
				foreach (GameObject item in Objects.All)
				{
					if (item.Menpai == 0 && !item.IsLootPacket && TINHKIEM.GetDistance(item.RoundX, item.RoundY, SafeX, SafeY) < 7f)
					{
						return false;
					}
				}
				return true;
			}
			foreach (GameObject item2 in Objects.All)
			{
				if (item2.Menpai == 0 && !item2.IsLootPacket)
				{
					if (NotSafe.Contains(SafeX + "," + SafeY))
					{
						return false;
					}
					if (TINHKIEM.GetDistance(item2.RoundX, item2.RoundY, SafeX, SafeY) <= 10f)
					{
						NotSafe.Add(SafeX + "," + SafeY);
						return false;
					}
				}
				if (item2.CleanName == "hoiamphien")
				{
					if (NotSafe.Contains(SafeX + "," + SafeY))
					{
						return false;
					}
					if (TINHKIEM.GetDistance(item2.RoundX, item2.RoundY, SafeX, SafeY) <= 13f)
					{
						NotSafe.Add(SafeX + "," + SafeY);
						return false;
					}
				}
			}
			return true;
		}

		public void GetSafeToaDo()
		{
			if (TLBB.MapId == MAP.PhungMinhVuongLang)
			{
				if (CurPhungMinhIndex == -1)
				{
					CurPhungMinhIndex = 0;
					SafeX = TINHKIEM.ParseInt(GAMEDIC.PhungMinhVuongLang[CurPhungMinhIndex].Split(',')[0]);
					SafeY = TINHKIEM.ParseInt(GAMEDIC.PhungMinhVuongLang[CurPhungMinhIndex].Split(',')[1]);
				}
				while (!IsSafeArea())
				{
					CurPhungMinhIndex++;
					if (CurPhungMinhIndex > 13)
					{
						SafeX = (SafeY = 0);
						CurPhungMinhIndex = -1;
						break;
					}
					SafeX = TINHKIEM.ParseInt(GAMEDIC.PhungMinhVuongLang[CurPhungMinhIndex].Split(',')[0]);
					SafeY = TINHKIEM.ParseInt(GAMEDIC.PhungMinhVuongLang[CurPhungMinhIndex].Split(',')[1]);
				}
				if (!IsSafeArea())
				{
					SafeX = (SafeY = 0);
				}
			}
			else if (RoundX >= 16 && RoundX <= 42 && RoundY >= 16 && RoundY <= 38)
			{
				if (SafeX == 0)
				{
					SafeX = 16;
					SafeY = 18;
				}
				while (!IsSafeArea())
				{
					SafeX += 2;
					if (SafeX > 42)
					{
						SafeX = 18;
						SafeY += 3;
					}
					if (SafeY > 38)
					{
						SafeX = 16;
						SafeY = 18;
						break;
					}
				}
				if (!IsSafeArea())
				{
					NotSafe.Clear();
				}
			}
			else if (TINHKIEM.GetDistance(CharX, CharY, 100f, 95f) < 25f)
			{
				if (SafeX == 0)
				{
					SafeX = 85;
					SafeY = 80;
				}
				while (!IsSafeArea())
				{
					SafeX += 2;
					if (SafeX > 115)
					{
						SafeX = 85;
						SafeY += 3;
					}
					if (SafeY > 105)
					{
						SafeX = 85;
						SafeY = 80;
						break;
					}
				}
				if (!IsSafeArea())
				{
					NotSafe.Clear();
				}
			}
			else
			{
				if (!(TINHKIEM.GetDistance(CharX, CharY, 30f, 100f) < 30f))
				{
					return;
				}
				if (SafeX == 0)
				{
					SafeX = 10;
					SafeY = 85;
				}
				while (!IsSafeArea())
				{
					SafeX += 2;
					if (SafeX > 42)
					{
						SafeX = 10;
						SafeY += 3;
					}
					if (SafeY > 115)
					{
						SafeX = 10;
						SafeY = 85;
						break;
					}
				}
				if (!IsSafeArea())
				{
					NotSafe.Clear();
				}
			}
		}

		public void GetToaDo()
		{
			if ((TrongHoaX - 55) % 3 != 0)
			{
				TrongHoaX -= (TrongHoaX - 55) % 3;
			}
			BachHoaDuyenX = TrongHoaX - 15;
			BachHoaDuyenY = 154;
			while (!IsClearArea())
			{
				if (BachHoaDuyenY == 144)
				{
					BachHoaDuyenY = 154;
					BachHoaDuyenX += 3;
				}
				else
				{
					BachHoaDuyenY -= 2;
				}
				if (BachHoaDuyenX - TrongHoaX > FrmMain.MaxHoaX)
				{
					BachHoaDuyenX = 0;
					BachHoaDuyenY = 0;
					break;
				}
			}
		}

		public bool IsMapKeoDoi()
		{
			if (TLBB.MapId != MAP.VoLuongSon && TLBB.MapId != MAP.KiemCac && TLBB.MapId != MAP.DonHoang && TLBB.MapId != MAP.TungSon && TLBB.MapId != MAP.ThaiHo && TLBB.MapId != MAP.KinhHo && TLBB.MapId != MAP.DuongMon && TLBB.MapId != MAP.MoDung && TLBB.MapId != MAP.TinhTuc && TLBB.MapId != MAP.TieuDao && TLBB.MapId != MAP.ThieuLam && TLBB.MapId != MAP.ThienSon && TLBB.MapId != MAP.ThienLong && TLBB.MapId != MAP.NgaMy && TLBB.MapId != MAP.VoDang && TLBB.MapId != MAP.MinhGiao && TLBB.MapId != MAP.CaiBang && TLBB.MapId != MAP.TayHo && TLBB.MapId != MAP.NhiHai && TLBB.MapId != MAP.NhanNam && TLBB.MapId != MAP.ThanhThuSon && TLBB.MapId != MAP.LauLan)
			{
				return TLBB.MapId == MAP.PhungHoangCoThanh;
			}
			return true;
		}

		public bool IsMapPhuBan()
		{
			if (TLBB.MapId != MAP.TacKhauDoanhDia && TLBB.MapId != MAP.ThieuLamAcBa && TLBB.MapId != MAP.NgaMyAcBa && TLBB.MapId != MAP.TieuDaoAcBa && TLBB.MapId != MAP.DuongMonAcBa && TLBB.MapId != MAP.MinhGiaoAcBa && TLBB.MapId != MAP.VoDangAcBa && TLBB.MapId != MAP.TinhTucAcBa && TLBB.MapId != MAP.ThienSonAcBa && TLBB.MapId != MAP.CaiBangAcBa && TLBB.MapId != MAP.ThienLongAcBa && TLBB.MapId != MAP.MoDungAcBa && TLBB.MapId != MAP.TangKinhCac && TLBB.MapId != MAP.PhungHoangCoThanhPhuBan && TLBB.MapId != MAP.ViemMaSon && TLBB.MapId != MAP.TamTaiHiepCoc && TLBB.MapId != MAP.ThanhThuSonPhuBan && TLBB.MapId != MAP.HuyenVuDaoPhuBan && TLBB.MapId != MAP.TranLongKyCuoc && TLBB.MapId != MAP.LauLanBaoTang && TLBB.MapId != MAP.PhieuMieuPhong)
			{
				return TLBB.MapId == MAP.YenTuO;
			}
			return true;
		}

		public int GetNPCId(Script script)
		{
			int result = -1;
			foreach (GameObject item in Objects.All)
			{
				if (script.Info.ToLower().Contains(item.Name.ToLower()) && TINHKIEM.GetDistance(item.X, item.Y, CharX, CharY) < 3f)
				{
					result = item.Id;
				}
			}
			return result;
		}

		public int GetNPCId(NPC npc)
		{
			return GetNPCId(npc.X, npc.Y);
		}

		public int GetNPCId(float x, float y)
		{
			float num = 999f;
			int result = -1;
			foreach (GameObject item in Objects.AllNpc)
			{
				float distance = TINHKIEM.GetDistance(x, y, item.X, item.Y);
				if (distance < num)
				{
					num = distance;
					result = item.Id;
				}
			}
			return result;
		}

		public void NhiemVuCoBan()
		{
			if (IsNhiemVuCoBan && TLBB.Online)
			{
				_ = TickCount % 9;
			}
		}

		public void CloseMission()
		{
			PostMessage(18, 105);
		}

		public bool DuocAntiep()
		{
			if (Objects.Self.Buff.Contains(349))
			{
				return false;
			}
			return true;
		}

		public void AutoX2()
		{
			if (!TuAnX2 || !DuocAntiep())
			{
				return;
			}
			foreach (PacketItem item in PacketItem.Enum(this))
			{
				if (item.Type == "Cloth3_14")
				{
					PlayerPackageUseItem(item.Index);
					LuaDoOneLineString("IsMessageBox = 1;");
					break;
				}
			}
		}

		public void Chat(string kenh, string msg)
		{
			LuaDoUnicodeString("Talk:SendChatMessage('" + kenh + "', '" + msg + "');");
		}

		public void AcTac()
		{
			if (!Talked && TLBB.MapId == 272)
			{
				if (!TLBB.IsLeader)
				{
					return;
				}
				foreach (GameObject item in Objects.All)
				{
					if (item.Name.Contains("n Du V") && item.Name.Length == 24)
					{
						Talk(item.Id);
					}
				}
				{
					foreach (QuestFrame item2 in QuestFrame.Enum(this))
					{
						if (item2.StrOptionExtra1 == 402108)
						{
							QuestFrameOptionClicked(item2);
							Talked = true;
							break;
						}
					}
					return;
				}
			}
			if (!Global.IsAcTac)
			{
				return;
			}
			if (TLBB.MapId == 272 && Objects.NearMonter5m.Count != 0)
			{
				if (TLBB.IsFollow)
				{
					StopFollow();
				}
			}
			else
			{
				if (TickCount % 18 != 0)
				{
					return;
				}
				if (TLBB.MapId == MAP.ThanhThuSonPhuBan)
				{
					if (MoveIndex == -1)
					{
						MoveIndex = 0;
					}
					if (TINHKIEM.SecDiff(BossTime, DateTime.Now) >= 15 && TLBB.IsLeader)
					{
						if (!IsRide && TLBB.HaveRide)
						{
							UpRide();
							return;
						}
						if (!TLBB.IsFollow)
						{
							AskTeamFollow();
							return;
						}
					}
					if (BossTime != DateTime.MinValue)
					{
						if (TLBB.IsLeader && !IsRide && TLBB.HaveRide)
						{
							UpRide();
						}
						return;
					}
					if (TLBB.IsFollow)
					{
						StopFollow();
						return;
					}
					if (IsRide)
					{
						Ride();
						return;
					}
					if (TINHKIEM.GetDistance(CharX, CharY, 87f, 64f) > 3f && TLBB.PlayerState != 2)
					{
						Move(87f, 64f);
					}
					{
						foreach (GameObject item3 in Objects.All)
						{
							if (TINHKIEM.VietLien(item3.Name) == "datrudaumuc" && item3.HP == 0f)
							{
								if (BossTime == DateTime.MinValue)
								{
									BossTime = DateTime.Now;
								}
								break;
							}
						}
						return;
					}
				}
				if (IsMapPhuBan())
				{
					if (MoveIndex == -1)
					{
						MoveIndex = 0;
					}
					if (TINHKIEM.SecDiff(BossTime, DateTime.Now) >= 15 && TLBB.IsLeader)
					{
						if (!IsRide && TLBB.HaveRide)
						{
							UpRide();
							return;
						}
						if (!TLBB.IsFollow)
						{
							AskTeamFollow();
							return;
						}
					}
					if (BossTime != DateTime.MinValue)
					{
						if (TLBB.IsLeader && !IsRide && TLBB.HaveRide)
						{
							UpRide();
						}
						return;
					}
					foreach (GameObject item4 in Objects.All)
					{
						if (!(TINHKIEM.VietLien(item4.Name) == "tacbinhdaumuc") && !(TINHKIEM.VietLien(item4.Name) == "acba") && !(TINHKIEM.VietLien(item4.Name) == "bansondaonhan"))
						{
							continue;
						}
						if (item4.HP == 0f)
						{
							if (BossTime == DateTime.MinValue)
							{
								BossTime = DateTime.Now;
							}
							if (TINHKIEM.GetDistance(CharX, CharY, item4.X, item4.Y) > 3f && TLBB.PlayerState != 2)
							{
								Move(item4.X, item4.Y);
							}
						}
						return;
					}
					if (TLBB.IsFollow)
					{
						StopFollow();
						return;
					}
					if (IsRide)
					{
						Ride();
						return;
					}
					Next();
					if (Objects.NearMonter20m.Count == 0 && IdleTime > 1)
					{
						MoveNext();
					}
					return;
				}
				if (IsMapKeoDoi())
				{
					if (TLBB.IsLeader)
					{
						if (!IsRide && TLBB.HaveRide)
						{
							UpRide();
							return;
						}
						if (!TLBB.IsFollow)
						{
							AskTeamFollow();
							return;
						}
						if (!IsMove)
						{
							FixKetMap();
							return;
						}
						MoveNext();
					}
				}
				else if (Objects.NearMonter20m.Count == 0)
				{
					string text = Setting.LoadMAP(TLBB.MapId.ToString());
					int num = 0;
					string[] array = text.Split('-');
					foreach (string text2 in array)
					{
						int num2 = 0;
						int num3 = 0;
						try
						{
							num2 = TINHKIEM.ParseInt(text2.Split(',')[0]);
							num3 = TINHKIEM.ParseInt(text2.Split(',')[1]);
						}
						catch
						{
						}
						if (num2 != 0 && num3 != 0)
						{
							num++;
						}
					}
					if (num > 1)
					{
						MoveNext();
					}
				}
				if (TLBB.IsLeader)
				{
					TalkNPCPhuBan();
				}
			}
		}

		private bool TalkNPCPhuBan()
		{
			float num = 100f;
			int num2 = -1;
			foreach (GameObject item in Objects.All)
			{
				if (((item.Name.Contains("c T") && item.Name.Contains("o Ph")) || item.CleanName == "therebels" || item.CleanName == "thiefraid") && TINHKIEM.GetDistance(CharX, CharY, item.X, item.Y) < num)
				{
					num = TINHKIEM.GetDistance(CharX, CharY, item.X, item.Y);
					num2 = item.Id;
				}
				if (item.Name.StartsWith("Giang h") && item.Name.Contains(" t") && TINHKIEM.GetDistance(CharX, CharY, item.X, item.Y) < num)
				{
					num = TINHKIEM.GetDistance(CharX, CharY, item.X, item.Y);
					num2 = item.Id;
				}
				if (item.Name.Contains("n Du V") && TINHKIEM.GetDistance(CharX, CharY, item.X, item.Y) < num)
				{
					num = TINHKIEM.GetDistance(CharX, CharY, item.X, item.Y);
					num2 = item.Id;
				}
				if (item.Name.StartsWith("M") && item.Name.Contains("Kim H") && item.Name.Contains("u ") && TINHKIEM.GetDistance(CharX, CharY, item.X, item.Y) < num)
				{
					num = TINHKIEM.GetDistance(CharX, CharY, item.X, item.Y);
					num2 = item.Id;
				}
				if (TLBB.MapId == MAP.ThanhThuSon && item.Title.Contains("Linh Thú"))
				{
					num = TINHKIEM.GetDistance(CharX, CharY, item.X, item.Y);
					num2 = item.Id;
				}
				if (TLBB.MapId == MAP.LauLan && item.Title.Contains("Thiên Niên Kỳ Thú"))
				{
					num = TINHKIEM.GetDistance(CharX, CharY, item.X, item.Y);
					num2 = item.Id;
				}
			}
			if (num2 == -1)
			{
				return false;
			}
			if (!TLBB.IsQuestOpen)
			{
				Talk(num2);
				return true;
			}
			if (TLBB.MapId == MAP.ThanhThuSon)
			{
				foreach (QuestFrame item2 in QuestFrame.Enum(this))
				{
					if (item2.Name.Contains("Giải cứu Linh Thú"))
					{
						QuestFrameOptionClicked(item2);
						CloseQuest();
						return true;
					}
				}
			}
			if (TLBB.MapId == MAP.LauLan)
			{
				foreach (QuestFrame item3 in QuestFrame.Enum(this))
				{
					if (item3.Name.Contains("Thiên Giáng Kỳ Thú"))
					{
						QuestFrameOptionClicked(item3);
						CloseQuest();
						return true;
					}
				}
			}
			foreach (QuestFrame item4 in QuestFrame.Enum(this))
			{
				if (item4.StrOptionExtra1 == 50013 && item4.StrOptionExtra2 == -1)
				{
					QuestFrameOptionClicked(item4);
					CloseQuest();
					return true;
				}
			}
			QuestFrame.ClickAll();
			CloseQuest();
			return true;
		}

		public static string TimeSpanToString(TimeSpan timeSpan)
		{
			if (timeSpan.Days > 0)
			{
				return timeSpan.Days + " ngày " + timeSpan.Hours + " giờ";
			}
			if (timeSpan.Hours > 0)
			{
				return timeSpan.Hours + " giờ " + timeSpan.Minutes + " phút";
			}
			if (timeSpan.Minutes > 0)
			{
				return timeSpan.Minutes + " phút " + timeSpan.Seconds + " giây";
			}
			return timeSpan.Seconds + " giây";
		}

		[DllImport("user32.dll")]
		private static extern uint SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

		public void Init()
		{
			FuncLuaToString = Memory.Scan("8B4424 08 85C0 56 57 8B7C24 0C 7E 11", "LuaPlus.dll");
			if (FuncLuaToString == 0)
			{
				return;
			}
			if (Handle != IntPtr.Zero)
			{
				SetHook(Handle);
				AutoSearch();
				SetDll();
				DisableActiveGame();
				if (Address.GameType == 1)
				{
					LuaDoString(ImageResource.Fix3D);
				}
				else if (Address.GameType == 2)
				{
					LuaDoString(ImageResource.Fix2D);
				}
				else
				{
					LuaDoString(ImageResource.FixDG);
				}
				LuaDoString(ImageResource.Lua);
				Pass2 = Setting.LoadStringOffline("PASS2" + TLBB.Id);
				if (Global.AutoAccept && !Global.AcceptAll)
				{
					SetTeamFromList(Setting.BuffValue);
				}
				if (Address.GameType == 3)
				{
					string text = Setting.LoadWAY(TLBB.GuildId.ToString());
					if (text.Contains("@way"))
					{
						SetWay(text);
					}
				}
				IsInit = true;
			}
			else
			{
				IntPtr handle = Win.GetHandle(Process.Id, "#32770");
				if (handle != IntPtr.Zero)
				{
					Memory.Write(Address.MultiAcc, 2425393296u, 4);
					Memory.Write(Address.MultiAcc + 4, 2425393296u, 4);
					Win.PostMessage(handle, 16, 0, 0);
				}
				Handle = Win.GetHandle(ProcessId, Win.WndClassNames);
			}
		}

		public BaiTrain SelectBaitrain(int level)
		{
			BaiTrain baiTrain = new BaiTrain();
			List<BaiTrain> dsbai = TrainData.dsbai;
			foreach (BaiTrain item in dsbai)
			{
				if (item.Level == level)
				{
					return item;
				}
			}
			if (baiTrain.Level == 0)
			{
				List<BaiTrain> list = new List<BaiTrain>();
				foreach (BaiTrain item2 in dsbai)
				{
					if (item2.Level < level)
					{
						list.Add(item2);
					}
				}
				list.Sort((BaiTrain x, BaiTrain y) => y.Level.CompareTo(x.Level));
				return list[0];
			}
			return baiTrain;
		}

		public void ThucThiAutoTrain()
		{
			if (!AutoTrain)
			{
				baitmp = new BaiTrain();
			}
			else
			{
				if (ON_SCENE_TRANSING)
				{
					return;
				}
				if (LenBaiTrain)
				{
					LenBaiTrain = false;
				}
				if (IsTrungAc)
				{
					return;
				}
				int lvl = TLBB.Lvl;
				BaiTrain baiTrain = new BaiTrain();
				baiTrain = SelectBaitrain(lvl);
				if (baitmp != baiTrain)
				{
					if (TLBB.MapId != baiTrain.MapID)
					{
						IsAttack = false;
						TimDuong(baiTrain.PosX, baiTrain.PosY, baiTrain.MapID);
					}
					else if (TINHKIEM.GetDistance(CharX, CharY, baiTrain.PosX, baiTrain.PosY) > 3f)
					{
						IsAttack = false;
						TimDuong(baiTrain.PosX, baiTrain.PosY, baiTrain.MapID);
					}
					else
					{
						DownRide();
						IsAttack = true;
						baitmp = baiTrain;
					}
				}
			}
		}

		public void TriLieu()
		{
			if (!IsTriLieu)
			{
				return;
			}
			if (IsAttack)
			{
				IsAttack = false;
			}
			if (TickCount % 18 != 0)
			{
				return;
			}
			if (TLBB.HPPercent + TLBB.MPPercent > 195)
			{
				PushDebugMessage("Sinh lực đã đầy. Không cần trị liệu");
				IsTriLieu = false;
				return;
			}
			if (TLBB.Gold < 10)
			{
				PushDebugMessage("Tiền không đủ không thể trị liệu");
				IsTriLieu = false;
				return;
			}
			if (Unity.DangOMapTriLieuHienTai(TLBB.MapId))
			{
				NPC nPCTRILIEU = Unity.GETNPCTRILIEU(TLBB.MapId);
				if (!GoTo(nPCTRILIEU.X, nPCTRILIEU.Y, nPCTRILIEU.Map))
				{
					return;
				}
				{
					foreach (GameObject item in Objects.AllNpc)
					{
						if (item.CleanName == "skylong" && TLBB.IsQuestOpen)
						{
							if (!QuestFrame.Click(129, 0))
							{
								QuestFrame.Click(129, 1001);
								QuestFrame.Close();
							}
							break;
						}
						if ((item.CleanName == "dothanhdang" && TLBB.IsQuestOpen) || (item.CleanName == "binhsanhan" && TLBB.IsQuestOpen))
						{
							int num = 0;
							if (num == 0 && TLBB.IsQuestOpen)
							{
								QuestFrame.Click(64, 0);
								num = 1;
							}
							if (num == 1 && TLBB.IsQuestOpen)
							{
								QuestFrame.Click(64, 1001);
								num = 2;
							}
						}
						else if (item.CleanName == "longbathien" || item.CleanName == "binhsanhan" || item.CleanName == "dothanhdang")
						{
							Talk(item.Id);
						}
					}
					return;
				}
			}
			NPC nPCTRILIEU2 = Unity.GETNPCTRILIEU(Option.MaptriLieuIndex);
			if (!GoTo(nPCTRILIEU2.X, nPCTRILIEU2.Y, nPCTRILIEU2.Map))
			{
				return;
			}
			foreach (GameObject item2 in Objects.AllNpc)
			{
				if (item2.CleanName == "skylong" && TLBB.IsQuestOpen)
				{
					if (!QuestFrame.Click(129, 0))
					{
						QuestFrame.Click(129, 1001);
						QuestFrame.Close();
					}
					break;
				}
				if ((item2.CleanName == "dothanhdang" && TLBB.IsQuestOpen) || (item2.CleanName == "binhsanhan" && TLBB.IsQuestOpen))
				{
					int num2 = 0;
					if (num2 == 0 && TLBB.IsQuestOpen)
					{
						QuestFrame.Click(64, 0);
						num2 = 1;
					}
					if (num2 == 1 && TLBB.IsQuestOpen)
					{
						QuestFrame.Click(64, 1001);
						num2 = 2;
					}
				}
				else if (item2.CleanName == "longbathien" || item2.CleanName == "binhsanhan" || item2.CleanName == "dothanhdang")
				{
					Talk(item2.Id);
				}
			}
		}

		public void SaveGold()
		{
			if (!IsSaveGold || TickCount % 18 != 0 || !GoTo(TLBB.NPCThuongKho))
			{
				return;
			}
			if (!TLBB.IsBankOpen)
			{
				if (TLBB.IsQuestOpen)
				{
					foreach (QuestFrame item in QuestFrame.Enum(this))
					{
						if (item.StrOptionExtra1 == 7 && item.StrOptionExtra2 == -1)
						{
							QuestFrameOptionClicked(item);
							break;
						}
					}
					return;
				}
				Talk(TLBB.NPCThuongKho.Id);
			}
			else
			{
				if (TLBB.Gold == 0)
				{
					IsSaveGold = false;
				}
				LuaDoOneLineString("Bank:SaveMoneyToBank(" + TLBB.Gold + ");");
				CloseQuest();
			}
		}

		private void AutoSearch()
		{
			if (Address.QuestInfo[0] == 0)
			{
				int num = Memory.ScanString(TINHKIEM.Sign.QuestInfo);
				num = Memory.Scan(Memory.ReverseString(num.ToString("X8")));
				num = Memory.Read(num + 16);
				Address.QuestInfo[0] = num;
			}
			if (Address.IsBankOpen[0] == 0)
			{
				int num2 = Memory.ScanString("UPDATE_BANK");
				num2 = Memory.Scan(Memory.ReverseString(num2.ToString("X8")));
				num2 = Memory.Read(num2 + 16);
				Address.IsBankOpen[0] = num2;
			}
			if (Address.ON_SCENE_TRANSING == 0)
			{
				int num3 = Memory.ScanString("ON_SCENE_TRANSING");
				num3 = Memory.Scan(Memory.ReverseString(num3.ToString("X8")));
				num3 = Memory.Read(num3 + 16);
				Address.ON_SCENE_TRANSING = num3;
			}
			if (Address.CountDown10Sec[0] == 0)
			{
				int num4 = Memory.ScanString("COUNTDOWN_10SEC");
				num4 = Memory.Scan(Memory.ReverseString(num4.ToString("X8")));
				num4 = Memory.Read(num4 + 16);
				Address.CountDown10Sec[0] = num4;
			}
			if (Address.IsShopOpen[0] == 0)
			{
				int num5 = Memory.ScanString("UPDATE_BOOTH");
				num5 = Memory.Scan(Memory.ReverseString(num5.ToString("X8")));
				num5 = Memory.Read(num5 + 16);
				Address.IsShopOpen[0] = num5;
			}
			FuncLuaToString = Memory.Scan("8B4424 08 85C0 56 57 8B7C24 0C 7E 11", "LuaPlus.dll");
		}

		public void Jump()
		{
			PostMessage(0, 120);
		}

		public void SaveSkill()
		{
			string text = "";
			foreach (Skill skill in Skills)
			{
				if (skill.Use)
				{
					text = text + "-" + skill.PacketId;
				}
			}
			text += "-";
			Setting.SaveSettingOffline(TLBB.Id + "SKILL", text);
		}

		public void SaveSkillPK()
		{
			string text = "";
			foreach (Skill skill in Skills)
			{
				if (skill.UsePK)
				{
					text = text + "-" + skill.PacketId;
				}
			}
			text += "-";
			Setting.SaveSettingOffline(TLBB.Id + "SKILLPK", text);
		}

		public void SaveSkillBuff()
		{
			string text = "";
			foreach (Skill skill in Skills)
			{
				if (skill.UserBuff)
				{
					text = text + "-" + skill.PacketId;
				}
			}
			text += "-";
			Setting.SaveSettingOffline(TLBB.Id + "SKILLBUFF", text);
		}

		public void SendPacket(string hex)
		{
			int wParam = Memory.WriteHex(hex);
			PostMessage(wParam, 114);
		}

		public bool IsNhiemVu()
		{
			if (!IsBachHoaDuyen)
			{
				return IsXayDung;
			}
			return true;
		}

		public void DatDoiAcTac()
		{
			if (TickCount % 9 != 0 || PickItem() || MapAcTac == 0)
			{
				return;
			}
			TrieuTap();
			if (TLBB.MapId != MAP.TacKhauDoanhDia && !IsRide && TLBB.HaveRide)
			{
				StopFollow();
				UpRide();
				return;
			}
			if (IsBossDie)
			{
				if (!IsRide && TLBB.HaveRide)
				{
					StopFollow();
					UpRide();
					return;
				}
				if (ClearTime.Elapsed.TotalSeconds > 40.0)
				{
					IsBossDie = false;
					MapATIndex = 0;
					ClearTime = Stopwatch.StartNew();
				}
				if (ClearTime.Elapsed.TotalSeconds > 20.0)
				{
					AskTeamFollow();
					return;
				}
			}
			int[,] acTacPoint = AcTacPoint;
			if (MapATIndex == -1)
			{
				MapATIndex++;
			}
			if (MapATIndex <= acTacPoint.GetLength(0) - 1 && TINHKIEM.GetDistance(CharX, CharY, acTacPoint[MapATIndex, 0], acTacPoint[MapATIndex, 1]) <= 2f)
			{
				MapATIndex++;
			}
			if (CurMapATIndex != -1 && CurMapATIndex <= acTacPoint.GetLength(0) - 1 && Objects.NearMonter(acTacPoint[CurMapATIndex, 0], acTacPoint[CurMapATIndex, 1], 12f).Count > 0)
			{
				ClearTime = Stopwatch.StartNew();
			}
			if (TLBB.MapId != MapAcTac && TLBB.MapId != MAP.TacKhauDoanhDia)
			{
				GoTo(acTacPoint[0, 0], acTacPoint[0, 1], MapAcTac);
			}
			else if (TLBB.MapId == MapAcTac)
			{
				if (!TalkNPCPhuBan())
				{
					if (!IsMoveEx)
					{
						FixKetMap();
					}
					else
					{
						MoveNext();
					}
				}
			}
			else
			{
				if (TLBB.MapId != MAP.TacKhauDoanhDia)
				{
					return;
				}
				if (ClearTime.Elapsed.TotalSeconds > 2.0 || MapATIndex == 0)
				{
					if (MapATIndex <= acTacPoint.GetLength(0) - 1)
					{
						if (!IsRide && TLBB.HaveRide)
						{
							UpRide();
							return;
						}
						AskTeamFollow();
						Move(acTacPoint[MapATIndex, 0], acTacPoint[MapATIndex, 1]);
						CurMapATIndex = MapATIndex;
						return;
					}
					if (ClearTime.Elapsed.TotalSeconds > 22.0)
					{
						if (IsRide && TLBB.HaveRide)
						{
							UpRide();
							return;
						}
						AskTeamFollow();
					}
					if (ClearTime.Elapsed.TotalSeconds > 10.0)
					{
						IsBossDie = true;
						return;
					}
				}
				if (ClearTime.Elapsed.TotalSeconds < 1.0 && TimeStand.Elapsed.TotalSeconds >= 2.0)
				{
					if (IsRide && IsAuto)
					{
						DownRide();
					}
					StopFollow();
				}
			}
		}

		public void DatDoiTKC()
		{
			if (TickCount % 9 != 0 || Global.IsVIP == 0 || MapTKC == 0)
			{
				return;
			}
			if (Objects.NearMonter12m.Count > 0)
			{
				ClearTime = Stopwatch.StartNew();
			}
			if (Objects.NearMonter20m.Count > 0 && TimeStand.Elapsed.TotalSeconds > 0.5)
			{
				if (TLBB.IsRide && IsAuto)
				{
					DownRide();
				}
				StopFollow();
			}
			if (TLBB.MapId != MAP.TangKinhCac && !IsRide && TLBB.HaveRide)
			{
				StopFollow();
				UpRide();
				return;
			}
			TrieuTap();
			if (TKCComplete && !CheckTKCComplete)
			{
				if (TINHKIEM.GetDistance(CharX, CharY, 64f, 100f) > 1f)
				{
					Move(64f, 100f);
				}
				else if (IdleTime > 2 && ClearTime.Elapsed.TotalSeconds > 2.0)
				{
					if (!IsRide && TLBB.HaveRide)
					{
						UpRide();
						return;
					}
					CheckTKCComplete = true;
					AskTeamFollow();
				}
			}
			else if (CheckTKCComplete && !TKCompleted)
			{
				if (TINHKIEM.GetDistance(CharX, CharY, 64f, 28f) > 3f)
				{
					Move(64f, 28f);
					ClearTime = Stopwatch.StartNew();
				}
				else if (ClearTime.Elapsed.TotalSeconds > 11.0 && IdleTime > 2)
				{
					TKCompleted = true;
				}
			}
			else if (!TKCompleted)
			{
				if (TLBB.MapId != MapTKC && TLBB.MapId != MAP.TangKinhCac)
				{
					int[,] array = MapPOINT(MapTKC);
					if (array != null)
					{
						GoTo(array[0, 0], array[0, 1], MapTKC);
					}
					return;
				}
				if (TLBB.MapId == MapTKC && !TalkNPCPhuBan())
				{
					if (!IsMove)
					{
						FixKetMap();
						return;
					}
					MoveNext();
				}
				if (TLBB.MapId != MAP.TangKinhCac || PickItem())
				{
					return;
				}
				if (TKCState == 0)
				{
					TKCState = 1;
					LuaDoOneLineString("return TKCINFO;");
					LuaToString();
				}
				else
				{
					TKCState = 0;
					string text = LuaString();
					if (text != TKCInfo)
					{
						TKCInfo = text;
						TKCStateTime = Stopwatch.StartNew();
					}
				}
				if (Objects.NearMonter20m.Count == 0)
				{
					if (!IsRide && TLBB.HaveRide)
					{
						UpRide();
						return;
					}
					AskTeamFollow();
				}
				else if (Talked)
				{
					return;
				}
				if (((TINHKIEM.GetDistance(CharX, CharY, 64f, 100f) < 8f && ClearTime.Elapsed.TotalSeconds > 2.0) || TKCInfo == "#{CJG_090605_4}") && TKCInfo != "")
				{
					if (IsTraiTKC)
					{
						Move(97f, 64f);
					}
					else
					{
						Move(27f, 64f);
					}
					return;
				}
				if (TKCInfo == "")
				{
					if (Talked)
					{
						Move(97f, 64f);
					}
					return;
				}
				if (TKCInfo.Contains("1/10") || TKCInfo.Contains("3/10") || TKCInfo.Contains("5/10") || TKCInfo.Contains("7/10") || TKCInfo.Contains("9/10"))
				{
					IsTraiTKC = false;
					if (TKCStateTime.Elapsed.TotalSeconds < 10.0)
					{
						Move(97f, 64f);
						return;
					}
				}
				if (TKCInfo.Contains("2/10") || TKCInfo.Contains("4/10") || TKCInfo.Contains("6/10") || TKCInfo.Contains("8/10") || TKCInfo.Contains("10/10"))
				{
					IsTraiTKC = true;
					if (TKCStateTime.Elapsed.TotalSeconds < 10.0)
					{
						Move(27f, 64f);
						return;
					}
				}
				if (TKCInfo.Contains("10/10"))
				{
					TKCComplete = true;
				}
				if (TKCStateTime.Elapsed.TotalSeconds > 20.0)
				{
					Move(64f, 100f);
				}
			}
			else if (!IsRide && TLBB.HaveRide)
			{
				UpRide();
			}
			else
			{
				AskTeamFollow();
				Move(70f, 20f);
			}
		}

		public void DatDoiQ123LauLan()
		{
			if (!IsQ123LauLan || Global.IsVIP == 0 || TickCount % 18 != 0)
			{
				return;
			}
			if (TLBB.MapId != MAP.ViemMaSon && !IsRide && TLBB.HaveRide)
			{
				StopFollow();
				UpRide();
				return;
			}
			TrieuTap();
			if (PickItem())
			{
				return;
			}
			int[,] viemMaSonPoint = ViemMaSonPoint;
			if (CurMapATIndex != -1 && CurMapATIndex <= viemMaSonPoint.GetLength(0) - 1 && Objects.NearMonter(viemMaSonPoint[CurMapATIndex, 0], viemMaSonPoint[CurMapATIndex, 1], 12f).Count > 0)
			{
				ClearTime = Stopwatch.StartNew();
			}
			if (TLBB.MapId != MAP.ViemMaSon)
			{
				if (GoTo(LAULAN.HaDuyet))
				{
					IsP = true;
				}
			}
			else
			{
				if (MapATIndex == -1)
				{
					MapATIndex++;
				}
				if (MapATIndex <= viemMaSonPoint.GetLength(0) - 1 && TINHKIEM.GetDistance(CharX, CharY, viemMaSonPoint[MapATIndex, 0], viemMaSonPoint[MapATIndex, 1]) <= 3f)
				{
					MapATIndex++;
				}
				if (ClearTime.Elapsed.TotalSeconds > 3.5 || MapATIndex == 0)
				{
					if (MapATIndex == 8 && ClearTime.Elapsed.TotalSeconds < 30.0)
					{
						return;
					}
					if (MapATIndex == 7 && TLBB.PlayerState == 2)
					{
						ClearTime = Stopwatch.StartNew();
					}
					if (MapATIndex <= viemMaSonPoint.GetLength(0) - 1)
					{
						if (!IsRide && TLBB.HaveRide)
						{
							UpRide();
							return;
						}
						AskTeamFollow();
						GoTo(viemMaSonPoint[MapATIndex, 0], viemMaSonPoint[MapATIndex, 1]);
						CurMapATIndex = MapATIndex;
						return;
					}
				}
				if (ClearTime.Elapsed.TotalSeconds < 1.0 && TimeStand.Elapsed.TotalSeconds >= 2.0)
				{
					if (TLBB.IsRide && IsAuto)
					{
						DownRide();
					}
					StopFollow();
				}
			}
			if (ClearTime.Elapsed.TotalSeconds > 65.0 && TLBB.MapId == MAP.ViemMaSon)
			{
				IsBossDie = false;
				MapATIndex = -1;
				ClearTime = Stopwatch.StartNew();
			}
		}

		public void DatDoiAcBa()
		{
			if ((AcBa != TLBB.Menpai && AcBa != -1) || !IsAcBa || TickCount % 9 != 0 || PickItem() || TLBB.MapAcBa == -1)
			{
				return;
			}
			if (!IsMapPhuBan() && !IsRide && TLBB.HaveRide)
			{
				StopFollow();
				UpRide();
				return;
			}
			TrieuTap();
			int[,] acBaPoint = AcBaPoint;
			if (!IsMapPhuBan())
			{
				if (TLBB.MapId != TLBB.MapMonPhai)
				{
					GoTo(acBaPoint[0, 0], acBaPoint[0, 1], TLBB.MapMonPhai);
					return;
				}
				if (TLBB.MapId == TLBB.MapMonPhai)
				{
					if (!TalkNPCPhuBan())
					{
						if (!IsMoveEx)
						{
							FixKetMap();
						}
						else
						{
							MoveNext();
						}
					}
					return;
				}
			}
			else if (Objects.NearMonter18m.Count > 0)
			{
				ClearTime = Stopwatch.StartNew();
				if (IsRide && IsAuto)
				{
					DownRide();
				}
				StopFollow();
			}
			if (IsMapPhuBan())
			{
				foreach (GameObject item in Objects.All)
				{
					if ((item.CleanName == "acba" || item.CleanName == "tyrant") && item.HP == 0f && !IsBossDie)
					{
						IsBossDie = true;
						BossDieTime = Stopwatch.StartNew();
					}
				}
				if (IsBossDie)
				{
					MoveIndex = -1;
					if (BossDieTime.Elapsed.TotalSeconds > 15.0)
					{
						if (!IsRide && TLBB.HaveRide)
						{
							UpRide();
						}
						else
						{
							AskTeamFollow();
						}
					}
					return;
				}
				if (ClearTime.Elapsed.TotalSeconds > 2.0)
				{
					if (!IsRide && TLBB.HaveRide)
					{
						UpRide();
						return;
					}
					AskTeamFollow();
					MoveNext();
				}
				if (ClearTime.Elapsed.TotalSeconds < 1.0 && TimeStand.Elapsed.TotalSeconds >= 2.0)
				{
					if (IsRide && IsAuto)
					{
						DownRide();
					}
					StopFollow();
				}
			}
			if (ClearTime.Elapsed.TotalSeconds > 40.0 && TLBB.MapId == TLBB.MapAcBa)
			{
				IsBossDie = false;
				MapATIndex = -1;
				ClearTime = Stopwatch.StartNew();
			}
		}

		private void DiThuyLao()
		{
			if (TLBB.MapId != MAP.ThaiHo && TLBB.MapId != MAP.ToChau && TLBB.MapId != MAP.ThuyLao)
			{
				TimDuong(THAIHO.HoDienKhanh.X, THAIHO.HoDienKhanh.Y, MAP.ToChau);
			}
			else
			{
				if (!GoTo(THAIHO.HoDienKhanh))
				{
					return;
				}
				if (TLBB.IsQuestOpen)
				{
					if (!IsClick)
					{
						QuestFrameOptionClicked(232002, -1);
						IsClick = true;
					}
					else
					{
						QuestFrameAccept();
						IsClick = false;
						CloseQuest();
					}
				}
				else
				{
					Talk(THAIHO.HoDienKhanh);
				}
			}
		}

		private void NhanThuyLao()
		{
			if (DaNhanThuyLao)
			{
				return;
			}
			if (TrangThaiThuyLao == "")
			{
				if (!TLBB.IsTogleMission && !TLBB.IsTogleMission)
				{
					PostMessage(18, 105);
				}
				TrangThaiThuyLao = "OpenMission";
			}
			else if (TrangThaiThuyLao == "OpenMission")
			{
				PostMessage(18, 105);
				TrangThaiThuyLao = "CloseMission";
			}
			else if (TrangThaiThuyLao == "CloseMission")
			{
				foreach (Task item in Task.Enum(this))
				{
					if (item.ClearName.Contains("binhdinhthuylao"))
					{
						DaNhanThuyLao = true;
						TrangThaiThuyLao = "";
						return;
					}
				}
				TrangThaiThuyLao = "NhanThuyLao";
			}
			else
			{
				if (!(TrangThaiThuyLao == "NhanThuyLao"))
				{
					return;
				}
				if (TLBB.MapId != MAP.ThaiHo && TLBB.MapId != MAP.ToChau && TLBB.MapId != MAP.ThuyLao)
				{
					TimDuong(TOCHAU.HoDienBao.X, TOCHAU.HoDienBao.Y, MAP.ToChau);
				}
				else
				{
					if (!GoTo(TOCHAU.HoDienBao))
					{
						return;
					}
					if (TLBB.IsQuestOpen)
					{
						if (!IsClick)
						{
							QuestFrameOptionClicked(232000, -1);
							IsClick = true;
							return;
						}
						QuestFrameAccept();
						IsClick = false;
						CloseQuest();
						TrangThaiThuyLao = "";
					}
					else
					{
						Talk(TOCHAU.HoDienBao);
					}
				}
			}
		}

		public void DatDoiThuyLao()
		{
			if (!IsThuyLao || TickCount % 18 != 0 || PickItem())
			{
				return;
			}
			TrieuTap();
			if (TLBB.MapId == MAP.ThuyLao)
			{
				if (IsXongThuyLao)
				{
					GoTo(94f, 94f);
					return;
				}
				if (Objects.NearMonter15m.Count > 0)
				{
					if (TLBB.PlayerState == 0)
					{
						if (TLBB.IsFollow)
						{
							StopFollow();
						}
						if (TLBB.IsRide)
						{
							DownRide();
						}
					}
					ClearTime = Stopwatch.StartNew();
				}
				if (ClearTime.Elapsed.TotalSeconds > 2.0)
				{
					if (!IsRide && TLBB.HaveRide)
					{
						UpRide();
					}
					else
					{
						MoveNext();
					}
				}
			}
			else if (!DaNhanThuyLao)
			{
				NhanThuyLao();
			}
			else
			{
				DiThuyLao();
			}
		}

		public void DatDoiKyCuoc()
		{
			if (IsXongKyCuoc)
			{
				IsKyCuoc = false;
			}
			else
			{
				if (!IsKyCuoc || TickCount % 18 != 0 || PickItem())
				{
					return;
				}
				if (!IsRide && TLBB.HaveRide && TLBB.MapId != MAP.TranLongKyCuoc)
				{
					StopFollow();
					UpRide();
					return;
				}
				TrieuTap();
				if (TLBB.MapId != MAP.TranLongKyCuoc)
				{
					if (GoTo(LACDUONG.VuongTichTan))
					{
						if (TLBB.IsQuestOpen)
						{
							QuestFrameOptionClicked(401001, -1);
							CloseQuest();
						}
						else
						{
							Talk(LACDUONG.VuongTichTan.Id);
						}
					}
				}
				else if (TLBB.MapId == MAP.TranLongKyCuoc)
				{
					if (ClearTime.Elapsed.TotalSeconds < 4.0)
					{
						return;
					}
					if (Objects.NearMonter18m.Count > 0)
					{
						ClearTime = Stopwatch.StartNew();
						if (IsRide && IsAuto)
						{
							DownRide();
						}
						StopFollow();
					}
					else if (!IsBossDie)
					{
						if (!IsRide && TLBB.HaveRide)
						{
							UpRide();
							return;
						}
						AskTeamFollow();
						MoveNext();
					}
				}
				else
				{
					IsKyCuoc = false;
				}
			}
		}

		public void DatDoiLauLanTamBao()
		{
			if (IsXongTamBao)
			{
				IsLauLanTamBao = false;
			}
			else
			{
				if (!IsLauLanTamBao || TickCount % 18 != 0 || PickItem())
				{
					return;
				}
				TrieuTap();
				if (TLBB.MapId == MAP.LauLanBaoTang)
				{
					return;
				}
				if (!IsRide && TLBB.HaveRide)
				{
					StopFollow();
					UpRide();
				}
				else if (GoTo(LAULAN.KimCuuLinh))
				{
					if (TLBB.IsQuestOpen)
					{
						QuestFrameOptionClicked(808039, 1);
						CloseQuest();
					}
					else
					{
						Talk(LAULAN.KimCuuLinh.Id);
					}
				}
			}
		}

		public bool TrieuTap()
		{
			bool flag = false;
			foreach (Game item in Party)
			{
				if (item.TLBB.IsLeader || !(item.tranTime.Elapsed.TotalSeconds >= 2.0))
				{
					continue;
				}
				if (IsThuyLao)
				{
					if (item.TLBB.MapId != MAP.ThuyLao || TLBB.MapId != MAP.ThuyLao)
					{
						if (!item.DaNhanThuyLao)
						{
							item.NhanThuyLao();
						}
						else
						{
							item.DiThuyLao();
						}
					}
					else
					{
						if (item.PickItem())
						{
							continue;
						}
						if (item.Objects.NearMonter20m.Count > 0)
						{
							if (item.TLBB.PlayerState == 0)
							{
								if (item.TLBB.IsFollow)
								{
									item.StopFollow();
								}
								if (item.TLBB.IsRide)
								{
									item.DownRide();
								}
							}
						}
						else if (!item.IsRide && item.TLBB.HaveRide)
						{
							item.UpRide();
						}
						if (TINHKIEM.GetDistance(item.RoundX, item.RoundY, RoundX, RoundY) > 4f)
						{
							item.GoTo(RoundX, RoundY);
						}
					}
					continue;
				}
				if (item.TLBB.KeyId != TLBB.Id || item.TLBB.IsLeader || item.TLBB.PlayerState == 2)
				{
					item.IsTrieuTap = false;
					continue;
				}
				if (IsQ123ToChau || IsYenTuO)
				{
					item.IsTheoQ = true;
				}
				if ((item.TLBB.MapId == MAP.ViemMaSon || item.TLBB.MapId == MAP.TamTaiHiepCoc) && item.TLBB.IsFollow)
				{
					continue;
				}
				if (item.IsMapPhuBan())
				{
					if (IsBossDie)
					{
						if (!item.IsRide && !IsNhamBinhSinhDie)
						{
							item.Ride();
						}
					}
					else if ((item.Objects.NearMonter12m.Count > 0 || ((item.TLBB.MapId == MAP.YenTuO || item.IsMapAcBa || item.TLBB.MapId == MAP.TangKinhCac) && item.Objects.NearMonter20m.Count > 0)) && item.TLBB.PlayerState == 0)
					{
						if (item.TLBB.MapId != MAP.ViemMaSon && item.TLBB.MapId != MAP.TamTaiHiepCoc)
						{
							item.StopFollow();
						}
						if (item.IsRide)
						{
							item.DownRide();
						}
					}
				}
				if ((IsQ123LauLan || IsQ123ToChau) && TLBB.MapId != MAP.TamTaiHiepCoc && TLBB.MapId != MAP.ViemMaSon && TLBB.MapId != MAP.SinhTuLoiDai && !item.IsP && IsP)
				{
					item.IsP = true;
					item.TraQ = (item.NhanQ = (item.IsClick = (item.IsContinute = false)));
				}
				if ((item.TLBB.MapId == MAP.ViemMaSon || item.TLBB.MapId == MAP.TacKhauDoanhDia || item.TLBB.MapId == item.TLBB.MapAcBa || item.TLBB.MapId == MAP.TangKinhCac || item.TLBB.MapId == MAP.TamTaiHiepCoc) && item.TLBB.PlayerState != 0)
				{
					item.IsTrieuTap = false;
				}
				else if (item.TLBB.MapId != TLBB.MapId || (double)TINHKIEM.GetDistance(item.CharX, item.CharY, CharX, CharY) >= 7.5 || (TINHKIEM.GetDistance(item.CharX, item.CharY, CharX, CharY) >= 3f && (item.TLBB.MapId == MAP.TangKinhCac || item.IsMapAcBa || item.TLBB.MapId == MAP.PhungHoangCoThanhPhuBan || item.TLBB.MapId == MAP.HuyenVuDaoPhuBan || item.TLBB.MapId == MAP.ThanhThuSonPhuBan || item.TLBB.MapId == MAP.TacKhauDoanhDia || item.TLBB.MapId == MAP.ViemMaSon || item.TLBB.MapId == MAP.TamTaiHiepCoc)))
				{
					if ((item.TLBB.MapId == MAP.TangKinhCac || item.TLBB.MapId == MAP.TacKhauDoanhDia) && item.TLBB.IsFollow)
					{
						item.IsTrieuTap = false;
						continue;
					}
					bool flag2 = (item.IsTrieuTap = true);
					flag = flag2;
					item.GoTo(RoundX, RoundY, TLBB.MapId);
				}
				else
				{
					item.IsTrieuTap = false;
				}
			}
			if (flag)
			{
				_ = TickCount % 36;
			}
			return flag;
		}

		public void UnHookRecv()
		{
			if (IsHooked)
			{
				if (RecvAddress != 0)
				{
					Memory.WriteProcessMemory(Memory.Id, RecvAddress, bufferRecv, 10, 0);
				}
				IsHooked = false;
			}
		}

		public void TapTrung()
		{
			if (!ON_SCENE_TRANSING && TLBB.Online && (Objects.Self == null || Objects.Self.PartyId != -1))
			{
				ReadRecvData();
				string trieuTap = GetTrieuTap(RecvDat);
				if (trieuTap != "")
				{
					ToaDo = trieuTap;
				}
			}
		}

		public void SetSafeTime()
		{
			LuaDoOneLineString("Lua_SetProtectTime(0,1); IsMessageBox = 1;");
		}

		public void P()
		{
			if (TickCount % 18 != 0)
			{
				return;
			}
			if (IsP)
			{
				if (TLBB.IsFollow)
				{
					StopFollow();
				}
				if (TINHKIEM.VietLien(TLBB.MapName) == "loidaisinhtu" && TINHKIEM.GetDistance(CharX, CharY, 12f, 34f) < 8f)
				{
					if (TLBB.IsQuestOpen)
					{
						QuestFrameOptionClicked(402049, 1);
						IsP = false;
					}
					{
						foreach (GameObject item in Objects.All)
						{
							if (TINHKIEM.VietLien(item.Name) == "khovinhdaisu")
							{
								Talk(item.Id);
								break;
							}
						}
						return;
					}
				}
				if (TINHKIEM.VietLien(TLBB.MapName).Contains("thienkieplau"))
				{
					if (TLBB.IsQuestOpen && TLBB.IsQuestOpen)
					{
						if (!IsClick)
						{
							foreach (QuestFrame item2 in QuestFrame.Enum(this))
							{
								if (item2.Name.Contains("#{TJL_xml_XX(01)}"))
								{
									QuestFrameOptionClicked(item2);
									IsClick = true;
									return;
								}
							}
							CloseQuest();
							return;
						}
						if (!TraQ && IsClick)
						{
							QuestFrameMissionComplete();
							CloseQuest();
							TraQ = true;
							IsClick = false;
							return;
						}
						if (!NhanQ && IsClick)
						{
							QuestFrameAccept();
							CloseQuest();
							NhanQ = true;
							IsP = false;
							IsClick = false;
							return;
						}
					}
					{
						foreach (GameObject item3 in Objects.All)
						{
							if (TINHKIEM.VietLien(item3.Name) == "phokiepsinh")
							{
								Talk(item3.Id);
								break;
							}
						}
						return;
					}
				}
				if (TLBB.MapId == DAILY.Id && TINHKIEM.GetDistance(CharX, CharY, 131f, 79f) < 8f)
				{
					if (TLBB.IsQuestOpen)
					{
						if (!IsClick)
						{
							foreach (QuestFrame item4 in QuestFrame.Enum(this))
							{
								if (item4.Name.Contains("#{SXRW_090119_002}"))
								{
									QuestFrameOptionClicked(item4);
									IsClick = true;
									return;
								}
							}
							QuestFrame.Close();
							return;
						}
						if (!TraQ && IsClick)
						{
							QuestFrameMissionComplete();
							CloseQuest();
							TraQ = true;
							IsClick = false;
							return;
						}
						if (!NhanQ && IsClick)
						{
							QuestFrameAccept();
							CloseQuest();
							NhanQ = true;
							IsP = false;
							IsClick = false;
							return;
						}
						if (TLBB.IsLeader && IsClick && NhanQ && TraQ)
						{
							foreach (QuestFrame item5 in QuestFrame.Enum(this))
							{
								if (item5.StrOptionExtra1 == 402048 && item5.StrOptionExtra2 == 2)
								{
									QuestFrameOptionClicked(item5);
								}
							}
							IsClick = (TraQ = (NhanQ = (IsContinute = false)));
						}
					}
					{
						foreach (GameObject item6 in Objects.All)
						{
							if (TINHKIEM.VietLien(item6.Name) == "khovinhdaisu")
							{
								Talk(item6.Id);
								break;
							}
						}
						return;
					}
				}
				if (TLBB.MapId == 61 && TINHKIEM.GetDistance(CharX, CharY, 40f, 40f) < 8f)
				{
					if (TLBB.IsQuestOpen)
					{
						QuestFrameOptionClicked(44000, 0);
						IsP = false;
					}
					{
						foreach (GameObject item7 in Objects.All)
						{
							if (TINHKIEM.VietLien(item7.Name) == "tethanh")
							{
								Talk(item7.Id);
								break;
							}
						}
						return;
					}
				}
				if (TLBB.MapId == 236 && TINHKIEM.GetDistance(CharX, CharY, 180f, 90f) < 8f)
				{
					if (TLBB.IsQuestOpen)
					{
						QuestFrameOptionClicked(402249, 1);
						IsP = false;
					}
					{
						foreach (GameObject item8 in Objects.All)
						{
							if (TINHKIEM.VietLien(item8.Name) == "hoahachcan")
							{
								Talk(item8.Id);
								break;
							}
						}
						return;
					}
				}
				if (TLBB.MapId == MAP.LauLan && TINHKIEM.GetDistance(CharX, CharY, 211f, 176f) < 8f)
				{
					if (TLBB.IsQuestOpen)
					{
						if (!IsClick)
						{
							foreach (QuestFrame item9 in QuestFrame.Enum(this))
							{
								if (item9.StrOptionExtra1 == 505054 && item9.StrOptionExtra2 == 1)
								{
									QuestFrameOptionClicked(item9);
									IsClick = true;
									return;
								}
							}
							CloseQuest();
							return;
						}
						if (!TraQ && IsClick)
						{
							if (!IsContinute)
							{
								IsContinute = true;
								QuestFrameMissionContinue();
								return;
							}
							QuestFrameMissionComplete();
							CloseQuest();
							TraQ = true;
							IsClick = false;
							return;
						}
						if (!NhanQ && IsClick)
						{
							QuestFrameAccept();
							CloseQuest();
							NhanQ = true;
							IsP = false;
							IsClick = false;
							return;
						}
					}
					{
						foreach (GameObject item10 in Objects.All)
						{
							if (TINHKIEM.VietLien(item10.Name) == "caoduong")
							{
								Talk(item10.Id);
								break;
							}
						}
						return;
					}
				}
				if (TLBB.MapId == MAP.LauLan && TINHKIEM.GetDistance(CharX, CharY, 295f, 68f) < 8f)
				{
					if (TLBB.IsQuestOpen)
					{
						if (QuestFrame.All(this).Contains("#{XSHYH_150211_13}"))
						{
							IsQ123LauLan = false;
						}
						if (!IsClick)
						{
							foreach (QuestFrame item11 in QuestFrame.Enum(this))
							{
								if (item11.StrOptionExtra1 == 506030)
								{
									QuestFrameOptionClicked(item11);
									IsClick = true;
									return;
								}
							}
							IsClick = (TraQ = (NhanQ = (IsContinute = false)));
							CloseQuest();
							return;
						}
						if (!TraQ && IsClick)
						{
							QuestFrameMissionComplete();
							CloseQuest();
							TraQ = true;
							IsClick = false;
							return;
						}
						if (!NhanQ && IsClick)
						{
							QuestFrameAccept();
							CloseQuest();
							NhanQ = true;
							IsP = false;
							IsClick = false;
							if (TLBB.IsLeader && IsQ123LauLan)
							{
								IsP = true;
							}
							return;
						}
						if (TLBB.IsLeader && IsQ123LauLan && IsClick)
						{
							QuestFrame.ClickAll();
							IsClick = (TraQ = (NhanQ = (IsContinute = false)));
						}
						CloseQuest();
					}
					{
						foreach (GameObject item12 in Objects.All)
						{
							if (TINHKIEM.VietLien(item12.Name) == "haduyet")
							{
								Talk(item12.Id);
								break;
							}
						}
						return;
					}
				}
				if (TLBB.MapId == TOCHAU.Id && TINHKIEM.GetDistance(CharX, CharY, 134f, 260f) < 8f)
				{
					if (TLBB.IsQuestOpen)
					{
						if (QuestFrame.All(this).Contains("#{LSHYH_150210_6}"))
						{
							IsQ123ToChau = false;
						}
						if (TLBB.IsLeader && TraQ && NhanQ)
						{
							foreach (QuestFrame item13 in QuestFrame.Enum(this))
							{
								if (item13.StrOptionExtra1 == 891074 && item13.StrOptionExtra2 == 2)
								{
									QuestFrameOptionClicked(item13);
									return;
								}
							}
							IsClick = (TraQ = (NhanQ = (IsContinute = false)));
						}
						if (!IsClick)
						{
							foreach (QuestFrame item14 in QuestFrame.Enum(this))
							{
								if (item14.StrOptionExtra1 == 891074 && item14.StrOptionExtra2 == 1)
								{
									QuestFrameOptionClicked(item14);
									IsClick = true;
									return;
								}
							}
							CloseQuest();
							return;
						}
						if (!TraQ && IsClick)
						{
							QuestFrameMissionComplete();
							CloseQuest();
							TraQ = true;
							IsClick = false;
							return;
						}
						if (!NhanQ && IsClick)
						{
							QuestFrameAccept();
							CloseQuest();
							NhanQ = true;
							IsP = false;
							IsClick = false;
							if (TLBB.IsLeader && IsQ123ToChau)
							{
								IsP = true;
							}
							return;
						}
						CloseQuest();
					}
					{
						foreach (GameObject item15 in Objects.All)
						{
							if (TINHKIEM.VietLien(item15.Name) == "tienhoanhvu")
							{
								Talk(item15.Id);
								break;
							}
						}
						return;
					}
				}
				if (TLBB.MapId == TOCHAU.Id && TINHKIEM.GetDistance(CharX, CharY, 195f, 214f) < 8f)
				{
					if (TLBB.IsQuestOpen)
					{
						if (TLBB.IsLeader)
						{
							foreach (QuestFrame item16 in QuestFrame.Enum(this))
							{
								if (item16.Name.Contains("#{SJZ_100129_11}"))
								{
									QuestFrameOptionClicked(item16);
									CloseQuest();
									return;
								}
								if (item16.Name.Contains("#{SJZ_100129_08}"))
								{
									TraQ = (NhanQ = false);
									IsClick = (IsContinute = false);
									return;
								}
							}
						}
						if (!IsClick)
						{
							foreach (QuestFrame item17 in QuestFrame.Enum(this))
							{
								if (item17.StrOptionExtra1 == 402052)
								{
									QuestFrameOptionClicked(item17);
									IsClick = true;
									return;
								}
							}
							CloseQuest();
							return;
						}
						if (!TraQ && IsClick)
						{
							QuestFrameMissionComplete();
							CloseQuest();
							TraQ = true;
							IsClick = false;
							return;
						}
						if (!NhanQ && IsClick)
						{
							QuestFrameAccept();
							CloseQuest();
							NhanQ = true;
							IsP = false;
							IsClick = false;
							if (IsTheoQ)
							{
								IsP = true;
								TraQ = (NhanQ = false);
							}
							return;
						}
					}
					{
						foreach (GameObject item18 in Objects.All)
						{
							if (TINHKIEM.VietLien(item18.Name) == "phanthanhthanh")
							{
								Talk(item18.Id);
								break;
							}
						}
						return;
					}
				}
				if (TINHKIEM.GetDistance(CharX, CharY, 96f, 79f) < 8f || TINHKIEM.GetDistance(CharX, CharY, 35f, 87f) < 8f || TINHKIEM.GetDistance(CharX, CharY, 84f, 23f) < 8f || TINHKIEM.GetDistance(CharX, CharY, 23f, 17f) < 8f)
				{
					if (TINHKIEM.GetDistance(CharX, CharY, 100f, 110f) < 20f)
					{
						IsP = false;
						return;
					}
					if (RoundX >= 32 && RoundX <= 44 && RoundY >= 70 && RoundY <= 80)
					{
						bool isManMacDie = (IsTanVanDie = true);
						IsManMacDie = isManMacDie;
						IsP = false;
						return;
					}
					if (RoundX < 80 && RoundX > 60)
					{
						IsP = false;
						return;
					}
					if (TLBB.IsQuestOpen)
					{
						if (TINHKIEM.GetDistance(CharX, CharY, 23f, 17f) < 8f)
						{
							QuestFrameOptionClicked(402051, 25);
						}
						else if (TINHKIEM.GetDistance(CharX, CharY, 84f, 23f) < 8f)
						{
							QuestFrameOptionClicked(402051, 24);
						}
						else
						{
							QuestFrameOptionClicked(402051, 22);
							QuestFrameOptionClicked(402051, 23);
						}
						IsP = false;
					}
					{
						foreach (GameObject item19 in Objects.All)
						{
							if (TINHKIEM.VietLien(item19.Name) == "phanthanhthanh")
							{
								Talk(item19.Id);
								break;
							}
						}
						return;
					}
				}
			}
			IsP = false;
		}

		public int ToMinute(string s)
		{
			int num = TINHKIEM.ParseInt(s) * 60;
			int num2 = TINHKIEM.ParseInt(Regex.Replace(s, ".*:", ""));
			return num + num2;
		}

		private void RandomAcTac()
		{
			int num = new Random().Next(0, 4);
			if (num == 0)
			{
				MapAcTac = MAP.KinhHo;
			}
			if (num == 1)
			{
				MapAcTac = MAP.ThaiHo;
			}
			if (num == 2)
			{
				MapAcTac = MAP.TungSon;
			}
			if (num == 3)
			{
				MapAcTac = MAP.KiemCac;
			}
			if (num == 4)
			{
				MapAcTac = MAP.DonHoang;
			}
		}

		private void RandomTKC()
		{
			int num = new Random().Next(0, 2);
			if (num == 0)
			{
				MapTKC = MAP.TayHo;
			}
			if (num == 1)
			{
				MapTKC = MAP.NhiHai;
			}
			if (num == 2)
			{
				MapTKC = MAP.NhanNam;
			}
		}

		public void ClearMission()
		{
			int num = (MapTKC = 0);
			MapAcTac = num;
			bool flag = (IsYenTuO = (IsTuBaoBon = (IsLuyenKim = false)));
			bool flag3 = (IsQ123LauLan = flag);
			bool flag5 = (IsQ123ToChau = flag3);
			bool flag7 = (IsHuyetChien = flag5);
			bool flag9 = (IsPMP = flag7);
			bool flag11 = (IsLauLanTamBao = flag9);
			bool flag13 = (IsKyCuoc = flag11);
			bool isAcBa = (IsPhungHoangLangMo = flag13);
			IsAcBa = isAcBa;
		}

		public void Auto()
		{
			if (!IsAuto)
			{
				return;
			}
			if (!IsInit)
			{
				Init();
				return;
			}
			if (TickCount % DelayTime == 0)
			{
				TLBB.Read();
				Objects.Read();
				if (TickCount % 600 == 0 && IdleTime > 2 && !Win.IsWindowVisible(Handle))
				{
					Handle = Win.GetHandle(ProcessId, Win.WndClassNames);
				}
			}
			if (ON_SCENE_TRANSING || IsChangeMap)
			{
				if (TLBB.MapId != TLBB.MapMonPhai)
				{
					if (TLBB.MapId != MAP.TacKhauDoanhDia && TLBB.MapId != MapAcTac && !IsTKC && !IsPhungHoangLangMo)
					{
						MoveIndex = -1;
					}
					if (TLBB.MapId == MAP.PhungHoangCoThanh || IsMapPhuBan() || TLBB.MapId == MAP.ThuyLao)
					{
						MoveIndex = 0;
					}
				}
				if (IsMapPhuBan())
				{
					IsRadius = false;
				}
				IsXongThuyLao = false;
				come = false;
				comeex = false;
				IsXongPhuBan = false;
				IsTheoQ = false;
				lastAutoMove = Stopwatch.StartNew();
				StandTime = 0;
				IdleTime = 0;
				Talked = false;
				TimeOnMap = 0;
				BossTime = DateTime.MinValue;
				MapATIndex = -1;
				IsTrieuTap = false;
				IsBossDie = false;
				IsCapDaiBaDie = false;
				IsTangThoCongDie = false;
				IsOLaoDaiDie = false;
				IsNhamBinhSinhDie = false;
				IsLyThuThuyDie = false;
				IsTalkPhuManNghi = false;
				IsTalkOLaoDai = false;
				OLaoDaiDead = false;
				DieTime = null;
				ClickTime = 0;
				LyThuThuyDead = null;
				IsLyThuThuyDead = false;
				TKCInfo = "";
				IsTraiTKC = false;
				bool flag = (TKCompleted = false);
				bool tKCComplete = (CheckTKCComplete = flag);
				TKCComplete = tKCComplete;
				tranTime = Stopwatch.StartNew();
				IsChangeM = true;
				ListBHDXuatHien.Clear();
				ListThuHoachBHD.Clear();
				IsTrieuTap = false;
				IsTheoQ = false;
				IsClick = (TraQ = (NhanQ = (IsContinute = false)));
				IsP = false;
				CurMapATIndex = -1;
				bool flag4 = (IsBangXiDie = false);
				flag = (IsDaoThanhDie = flag4);
				tKCComplete = (IsTanVanDie = flag);
				IsManMacDie = tKCComplete;
				tKCComplete = (IsCuuMaTriDie = false);
				IsDoanDienKhanhDie = tKCComplete;
				isAlarmHuyetMo = false;
				DaNhanThuyLao = false;
				NotSafe.Clear();
				ListDangLumHop.Clear();
				return;
			}
			TLBB.PlayerState = Memory.Read(Address.CharState);
			if (TLBB.PlayerState == 0 || TLBB.PlayerState == 9)
			{
				StandTime++;
				if (TickCount % 21 == 0)
				{
					IdleTime++;
					if (TLBB.IsCaptcha)
					{
						CaptchaTime = 0;
					}
					else
					{
						CaptchaTime++;
					}
				}
			}
			else
			{
				TimeStand = Stopwatch.StartNew();
				StandTime = 0;
				IdleTime = 0;
			}
			if (tranTime.Elapsed.TotalSeconds < 2.0)
			{
				return;
			}
			if (TickCount % 48 == 0)
			{
				IsX2 = false;
				if (lastX2TimeSec != TLBB.X2TimeSec)
				{
					lastX2TimeSec = TLBB.X2TimeSec;
					IsX2 = true;
					if (lastX2TimeSec == 0)
					{
						IsX2 = false;
					}
				}
			}
			if (IsOpenShop && SafeTime >= 2 && TickCount % 18 == 0 && IsOpenPass2)
			{
				IsOpenShop = false;
				OpenShop();
			}
			if (TLBB.MapId == MAP.DiaPhu || TLBB.MapId == MAP.DiaPhuDaiTheGioi)
			{
				if (TickCount % 18 != 0)
				{
					return;
				}
				XuatPetCount = 0;
				State = STATE.None;
				TrangThaiLuyenKim = (TrangThaiSuMon = (TrangThaiTuBaoBon = (TrangThaiXayDung = (TrangThaiTuDuong = (TrangThaiQD = "")))));
				if (TLBB.IsQuestOpen)
				{
					if (TLBB.MapId == MAP.DiaPhu)
					{
						if (Option.MaptriLieuIndex == 0)
						{
							QuestFrameOptionClicked(12009, 1);
						}
						else if (Option.MaptriLieuIndex == 1)
						{
							QuestFrameOptionClicked(12009, 3);
						}
						else if (Option.MaptriLieuIndex == 2)
						{
							QuestFrameOptionClicked(12009, 5);
						}
						else if (Option.MaptriLieuIndex == 246)
						{
							QuestFrameOptionClicked(12009, 6);
						}
						else
						{
							CloseQuest();
						}
					}
					else
					{
						QuestFrameOptionClicked(505072, 1);
						CloseQuest();
					}
					return;
				}
				{
					foreach (GameObject item in Objects.All)
					{
						if (item.CleanName.Contains("manhba") || item.CleanName == "pomeng")
						{
							Talk(item.Id);
							break;
						}
					}
					return;
				}
			}
			if (IsChangeM)
			{
				IsChangeM = false;
				PostMessage(58, 105);
			}
			P();
			SetCalendar();
			if (TrimTime.Elapsed.TotalMinutes > 3.0 && FrmMain.TrimRam)
			{
				TrimTime = Stopwatch.StartNew();
				TrimProc();
			}
			if (Objects.NearMonter12m.Count > 0)
			{
				ClearTime = Stopwatch.StartNew();
			}
			bool flag9 = false;
			if (TickCount % 9 == 0 || (TLBB.MapId == MAP.YenTuO && TickCount % 3 == 0))
			{
				if (IsLyThuThuyDead && !IsOut)
				{
					IsOut = true;
					LuaDoOneLineString("IsOut = true;");
				}
				if (!IsLyThuThuyDead && IsOut)
				{
					IsOut = false;
					LuaDoOneLineString("IsOut = false;");
				}
				if (TLBB.MapId == MAP.TranLongKyCuoc || TLBB.MapId == MAP.LauLanBaoTang || TLBB.MapId == MAP.ThanhThuSonPhuBan)
				{
					if (IsRide && Objects.Monter.Count > 0)
					{
						DownRide();
					}
					if (!IsBossDie)
					{
						foreach (GameObject item2 in Objects.All)
						{
							if (item2.CleanName == "viencokyhon" && item2.HP == 0f)
							{
								IsBossDie = true;
								BossDieTime = Stopwatch.StartNew();
							}
							if (item2.CleanName == "tranbaolongvuong" && item2.HP == 0f)
							{
								IsXongTamBao = true;
							}
						}
					}
					if (TLBB.MapId == MAP.TranLongKyCuoc && IsBossDie)
					{
						if (BossDieTime.Elapsed.TotalSeconds > 30.0)
						{
							GoTo(TRANLONGKYCUOC.TeThanh);
							IsP = true;
							IsXongKyCuoc = true;
						}
						else if (TickCount % 6 == 0)
						{
							PushDebugMessage("Di chuyển sau " + (20 - BossDieTime.Elapsed.Seconds) + "s");
						}
					}
				}
			}
			if (IsSaveGold)
			{
				SaveGold();
				return;
			}
			TriLieu();
			if (TickCount % DelayTime == 0)
			{
				LoadSkill();
				ResetetRadius();
				if (TLBB.Online)
				{
					Rao();
				}
			}
			XuatPet();
			if (TLBB.PetHP > 0)
			{
				IsDome = false;
				IsXuat = false;
				if (PetId == "")
				{
					PetId = TLBB.PetId.ToString("X8");
					Setting.SaveSettingOffline(TLBB.Id + "PET", PetId);
				}
				if (TLBB.PetLvl >= Global.PetLvl && TickCount % 30 == 0 && AutoThuPet)
				{
					DoAction("PetSkill2_2");
				}
			}
			if (IsLostLeader && TickCount % 18 == 0 && TLBB.OnlineTimeSec < 20 && TLBB.OnlineTimeSec > 3 && !TLBB.IsLeader)
			{
				foreach (KeyValuePair<int, Game> item3 in FrmMain.dicGame)
				{
					Game value = item3.Value;
					if (value != this && value.TLBB.IsLeader && value.TLBB.Id == TLBB.KeyId && value.TLBB.OnlineTimeSec > 3)
					{
						value.AppointLeader(TLBB.Name);
						IsLostLeader = false;
						break;
					}
				}
			}
			if (IsAcBa && AcBa != TLBB.Menpai && TickCount % 18 == 0 && TLBB.IsLeader)
			{
				foreach (KeyValuePair<int, Game> item4 in FrmMain.dicGame)
				{
					Game value2 = item4.Value;
					if (value2 != this && !value2.TLBB.IsLeader && value2.TLBB.KeyId == TLBB.Id && value2.TLBB.OnlineTimeSec > 3 && value2.TLBB.Menpai == AcBa)
					{
						value2.IsAcBa = true;
						AppointLeader(value2.TLBB.Name);
						break;
					}
				}
			}
			if (!TLBB.Online)
			{
				return;
			}
			if (TickCount % 18 == 0)
			{
				if (!IsHooked && (Option.AlarmChat || AlarmChat || (TLBB.IsLeader && FrmMain.AlarmAcBa)))
				{
					RecvAddress = (int)GetRemoteProcAddress(Process.GetProcessById(ProcessId), "ws2_32.dll", "recv");
					Memory.ReadProcessMemory(Memory.Id, RecvAddress, bufferRecv, 10, 0);
					PostMessage(ProcessId, -10);
					IsHooked = true;
				}
				if (IsHooked && !Option.AlarmChat && !AlarmChat && !FrmMain.AlarmAcBa)
				{
					UnHookRecv();
				}
			}
			if (SellItem() || (IsTrieuTap && !TLBB.IsLeader))
			{
				return;
			}
			DatDoiThuyLao();
			if (TLBB.IsLeader)
			{
				if (MapAcTac != 0)
				{
					DatDoiAcTac();
				}
				if (Global.IsVIP > 0)
				{
					DatDoiQ123LauLan();
				}
				DatDoiKyCuoc();
				DatDoiLauLanTamBao();
				DatDoiAcBa();
				DatDoiTKC();
			}
			MoBTD();
			if (TLBB.Online)
			{
				TimeOnMap++;
			}
			if (TLBB.MapId == MAP.GiamNguc)
			{
				if (TickCount % 18 != 0 || IdleTime <= 2)
				{
					return;
				}
				if (!IsTalkGiamNguc)
				{
					foreach (GameObject item5 in Objects.All)
					{
						if (TINHKIEM.VietLien(item5.Name).Contains("truongchinhquy"))
						{
							Talk(item5.Id);
						}
					}
					IsTalkGiamNguc = true;
					return;
				}
				if (TLBB.IsQuestOpen)
				{
					foreach (QuestFrame item6 in QuestFrame.Enum(this))
					{
						if (item6.StrOptionExtra1 == 77011 && (item6.StrOptionExtra2 == 1 || item6.StrOptionExtra2 == 11))
						{
							QuestFrameOptionClicked(item6);
							return;
						}
					}
				}
				IsTalkGiamNguc = false;
				return;
			}
			if (TickCount % 3 == 0 && TLBB.Gold > 10 && IsDead && TLBB.HPPercent > 0 && TLBB.HPPercent < 40 && Global.AutoComeBack && !MAP.IsPhuBan(TLBB.MapId))
			{
				IsTriLieu = true;
				return;
			}
			if (IsBachHoaDuyen)
			{
				IsDead = false;
			}
			if (!MAP.IsPhuBan(TLBB.MapId) && IsDead && TickCount % 3 == 0 && Global.AutoComeBack && !IsBachHoaDuyen && !IsTrungAc && TLBB.PlayerState == 0)
			{
				if (DeadX == 0)
				{
					IsDead = false;
				}
				else
				{
					if (TLBB.PetHP == 0 && XuatPetCount++ < 5 && Global.IsXuat)
					{
						DoAction("PetSkill2_1");
						return;
					}
					if (TLBB.MapId != DeadMap)
					{
						Move(DeadX, DeadY, DeadMap);
					}
					else if (IsHoldPK)
					{
						IsDead = false;
						IsAttack = true;
						DownRide();
					}
					else if (TINHKIEM.GetDistance(CharX, CharY, DeadX, DeadY) > 5f)
					{
						Move(DeadX, DeadY);
					}
					else
					{
						IsDead = false;
						IsAttack = true;
						DownRide();
					}
				}
			}
			Buff();
			AOE();
			CollectItem();
			DropItem();
			if (MapAcTac == 0)
			{
				if (TLBB.MapId != MAP.PhungMinhVuongLang || NeBayTime.Elapsed.TotalSeconds >= 3.0)
				{
					SkillDo();
				}
				AcTac();
				TrungAc();
				KhaiKhoang();
				TrongTrot();
				CheDoFree();
				NhanNguyenLieu();
				HamLenBaiTrian();
				ThucThiAutoTrain();
				AutoX2();
				AutoAnVatPham();
			}
			if (ExpStart > TLBB.Exp || ExpStart <= 0)
			{
				ResetExpSpeed();
			}
			if (IsAcceptAll && TickCount == 18)
			{
				IsAcceptAll = false;
				AcceptAll();
			}
			if (TickCount % 99 == 0 && Global.AutoAccept)
			{
				Accept();
			}
			if (NMTime.Elapsed.TotalSeconds >= 1.0 && !TLBB.BusyEx && !TLBB.IsBienThan && TLBB.MapId != MAP.DiaPhu && TLBB.MapId != MAP.DiaPhuDaiTheGioi && TLBB.PlayerState != 9 && !PickItem() && TLBB.MPPercent > 5 && !TINHKIEM.IsPressed(VirtualKeyStates.VK_LMENU) && !TINHKIEM.IsPressed(VirtualKeyStates.VK_RMENU) && TLBB.Name != "ĐăngNhập" && TLBB.PlayerState != 2 && TLBB.PlayerState != 5 && !IsRide && !TLBB.IsFollow && IsNM && TLBB.Menpai == 5 && Objects.PartyMinHP != null && Objects.PartyMinHP.HP * 100f <= (float)Global.BuffNMPercent)
			{
				int num = 9999;
				if (NMSKill.DelayOffset.ToString().Trim('0').Length < 8)
				{
					num = Memory.Read(TLBB.DelayBase + NMSKill.DelayOffset);
				}
				if (num == 0 || num == -1 || num == 9999)
				{
					if (Global.NMSkill == Keys.F13)
					{
						if (MAP.IsPhuBan(TLBB.MapId))
						{
							NMTime = Stopwatch.StartNew();
							UseSkill(NMSKill.PacketId, Objects.PartyMinHP.Id);
							return;
						}
						if (Objects.Self.HP * 100f <= (float)Global.BuffNMPercent)
						{
							NMTime = Stopwatch.StartNew();
							UseSkill(NMSKill.PacketId, Objects.Self.Id);
						}
					}
					else if (MAP.IsPhuBan(TLBB.MapId))
					{
						NMTime = Stopwatch.StartNew();
						SelectTarget(Objects.PartyMinHP.Id);
						SendKey(Global.NMSkill);
					}
					else if (Objects.Self.HP * 100f <= (float)Global.BuffNMPercent)
					{
						NMTime = Stopwatch.StartNew();
						SelectTarget(Objects.Self.Id);
						SendKey(Global.NMSkill);
					}
				}
			}
			if (TickCount % 3 == 0 && !PickItem() && !flag9)
			{
				Attack();
			}
			if (TickCount % 9 == 0)
			{
				FollowKey();
			}
			if (TickCount % 18 != 0)
			{
				return;
			}
			ExpGain = TLBB.Exp - ExpStart;
			ExpSpeed = (float)((double)ExpGain / AutoTime.Elapsed.TotalHours);
			if (X4 && Objects.Self != null && !Objects.Self.Buff.Contains(1685) && DoActionPacket("CircularTaskTool20_16"))
			{
				LuaDoOneLineString("IsMessageBox = 1");
			}
			if (LastId != TLBB.Id && TLBB.Online)
			{
				LoadSetting();
			}
			ResetTime();
			UpLvl();
			if (TLBB.SafeTime > 0)
			{
				IsOpenPass2 = false;
				SafeTime = 0;
			}
			else
			{
				SafeTime++;
				if (SafeTime >= 3 && Option.SetSafeTime && !setSafeTime && Address.GameType == 1)
				{
					setSafeTime = true;
					SetSafeTime();
				}
			}
			if (!IsOpenPass2 && Pass2 != "" && SafeTime == 2)
			{
				UnlockPass2();
				IsOpenPass2 = true;
			}
			if (!IsOpenBag)
			{
				OpenBag();
				IsOpenBag = true;
			}
			if (Pass2 == "")
			{
				IsOpenPass2 = true;
			}
		}

		private void SetCalendar()
		{
			if (!FrmMain.IsCalender || TickCount % 99 != 0 || IsBusy)
			{
				return;
			}
			string[] array = TINHKIEM.ReadFile(Global.CalenderPath + "\\" + TLBB.Id + ".txt").Split('\n');
			foreach (string text in array)
			{
				if (text.Split('|').Length <= 1)
				{
					continue;
				}
				string text2 = text.Split('|')[0];
				if (text2.Split('-').Length <= 1)
				{
					continue;
				}
				int num = ToMinute(text2.Split('-')[0]);
				int num2 = ToMinute(text2.Split('-')[1]);
				int num3 = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
				if (num3 < num || num3 >= num2)
				{
					continue;
				}
				string text3 = "";
				try
				{
					text3 = TINHKIEM.VietLien(text);
				}
				catch
				{
					break;
				}
				ClearMission();
				if (text3.Contains("tubaobon"))
				{
					IsTuBaoBon = true;
				}
				else if (TLBB.IsLeader)
				{
					if (text3.Contains("actac"))
					{
						RandomAcTac();
					}
					else if (text3.Contains("acba"))
					{
						IsAcBa = true;
					}
					else if (text3.Contains("tangkinhcac"))
					{
						RandomTKC();
					}
					else if (text3.Contains("langmo"))
					{
						IsPhungHoangLangMo = true;
					}
					else if (text3.Contains("kycuoc"))
					{
						IsKyCuoc = true;
					}
					else if (text3.Contains("tambao"))
					{
						IsLauLanTamBao = true;
					}
					else if (text3.Contains("huyetchien"))
					{
						bool isPMP = (IsHuyetChien = true);
						IsPMP = isPMP;
					}
					else if (text3.Contains("phieumieuphong"))
					{
						IsPMP = true;
						IsHuyetChien = false;
					}
					else if (text3.Contains("tochau"))
					{
						IsQ123ToChau = true;
					}
					else if (text3.Contains("laulan"))
					{
						IsQ123LauLan = true;
					}
					else if (text3.Contains("yentuo"))
					{
						IsYenTuO = true;
					}
					else if (text3.Contains("luyenkim"))
					{
						IsLuyenKim = true;
					}
				}
				break;
			}
		}

		private void XuatPet()
		{
			if (TickCount % 30 != 0 || TLBB.PlayerState == 5 || !Global.IsXuat || IsRide || !TLBB.Online || TLBB.PetHP != 0 || !(PetId != "00000000") || !(PetId != ""))
			{
				return;
			}
			int num = Memory.Read(Address.PetBase);
			int num2 = -1;
			for (int i = 0; i < 20; i++)
			{
				num2++;
				int num3 = Memory.Read(num + Address.PetDataSize * i + Address.PetId);
				int num4 = Memory.Read(num + Address.PetDataSize * i + Address.PetEnjoy);
				if (AutoThuPet)
				{
					if (Memory.Read(num + Address.PetDataSize * i + Address.PetLvl) < Global.PetLvl)
					{
						if (num4 < 60)
						{
							IsDome = true;
						}
						else
						{
							IsXuat = true;
						}
						if (num3.ToString("X8") == PetId && TimeStand.Elapsed.TotalSeconds > 0.5)
						{
							LuaDoOneLineString("XuatPet('" + PetId + "')");
						}
					}
				}
				else
				{
					if (num4 < 60)
					{
						IsDome = true;
					}
					else
					{
						IsXuat = true;
					}
					if (num3.ToString("X8") == PetId && TimeStand.Elapsed.TotalSeconds > 0.5)
					{
						LuaDoOneLineString("XuatPet('" + PetId + "')");
					}
				}
			}
		}

		public void TrimProc()
		{
			Memory.TrimMem();
		}

		public void NhanNguyenLieu()
		{
			if (IsNhanNguyenLieu && TickCount % 12 == 0)
			{
				if (TLBB.MapId != 2)
				{
					Move(157f, 169f, 2);
				}
				else if (TINHKIEM.GetDistance(CharX, CharY, 157f, 169f) > 1f)
				{
					Move(157f, 169f);
				}
				else if (!IsTalkTieuPhong)
				{
					IsTalkTieuPhong = true;
					Talk(143);
				}
				else
				{
					IsTalkTieuPhong = false;
					QuestFrameOptionClicked(2084, 1004);
				}
			}
		}

		public void SellItem(int item)
		{
			PostMessage(item, 117);
		}

		public void KhaiKhoang()
		{
			if (TickCount % 12 != 0 || (!IsKhoang && !IsDuoc))
			{
				return;
			}
			if (IsMapNghe() && !IsMove)
			{
				FixKetMap();
				return;
			}
			float num = 1000f;
			int num2 = -1;
			float num3 = 0f;
			float num4 = 0f;
			foreach (GameObject item in Objects.All)
			{
				if (((item.IsKhoang && IsKhoang) || (item.IsDuoc && IsDuoc)) && TINHKIEM.GetDistance(CharX, CharY, item.X, item.Y) < num)
				{
					num = TINHKIEM.GetDistance(CharX, CharY, item.X, item.Y);
					num2 = item.Id;
					num3 = item.X;
					num4 = item.Y;
				}
			}
			if (num2 != -1)
			{
				if (TINHKIEM.GetDistance(CharX, CharY, num3, num4) > 5f)
				{
					Move(num3, num4);
				}
				else if (IsRide)
				{
					DownRide();
				}
				else
				{
					PickItem(num2);
				}
			}
			else if (!IsRide && TLBB.HaveRide)
			{
				UpRide();
			}
			else if (IsMapNghe())
			{
				MoveNext();
			}
		}

		public bool IsMapNghe()
		{
			if (TLBB.MapId != MAP.KiemCac && TLBB.MapId != MAP.CaoXuong && TLBB.MapId != MAP.VoLuongSon && TLBB.MapId != MAP.DonHoang && TLBB.MapId != MAP.TungSon && TLBB.MapId != MAP.ThaiHo)
			{
				if (TLBB.MapId != MAP.TayHo && TLBB.MapId != MAP.NhiHai && TLBB.MapId != MAP.NhanNam && TLBB.MapId != MAP.LongTuyen && TLBB.MapId != MAP.ThuongSon && TLBB.MapId != MAP.NhanBac && TLBB.MapId != MAP.VoDi && TLBB.MapId != MAP.ThachLam && TLBB.MapId != MAP.NganNgaiTuyetNguyen)
				{
					return TLBB.MapId == MAP.ThaoNguyen;
				}
				return true;
			}
			return true;
		}

		public void OkPhu()
		{
			if (Address.GameType == 2 && Memory.Read(new int[5] { 6549480, 0, 0, 12, 100 }) == 1)
			{
				LuaDoOneLineString("setmetatable(_G, {__index = Item_TuDunZhu_Env}); Item_TuDunZhu_OK_Clicked();");
			}
		}

		public void DisableActiveGame()
		{
			if (Address.GameType == 1 || Address.GameType == 2)
			{
				Memory.Write(Address.DisableActiveGame, 2425393296u, 4);
				Memory.Write(Address.DisableActiveGame + 4, 2425393296u, 4);
				Memory.Write(Address.DisableActiveGame + 8, 2425393296u, 4);
				Memory.Write(Address.DisableActiveGame + 12, 37008u, 2);
			}
			else
			{
				Memory.Write(Address.DisableActiveGame, 2425393296u, 4);
				Memory.Write(Address.DisableActiveGame + 4, 2425393296u, 4);
				Memory.Write(Address.DisableActiveGame + 8, 2425393296u, 4);
				Memory.Write(Address.DisableActiveGame + 12, 9474192u, 3);
			}
		}

		public void Rao()
		{
			if (IsRao && (RaoTime == null || RaoTime.Elapsed.TotalSeconds > TimeGiaoChat))
			{
				RaoTime = Stopwatch.StartNew();
				if (RaoTxt.Trim() != "")
				{
					GiaoChat();
				}
			}
		}

		public void GiaoChat()
		{
			string text = "#r#b#eda0000  TLBB C h i c k e n A u t o #r#b#eda0000  h t tp : / / c h i c k e n a u t o . c o m";
			if (ChatGan)
			{
				Thread.Sleep(50);
				Chat("near", RaoTxt.Replace("\"", "") + text);
			}
			if (ChatMonPhai)
			{
				Thread.Sleep(50);
				Chat("menpai", RaoTxt.Replace("\"", "") + text);
			}
			if (ChatDongMinh)
			{
				Thread.Sleep(50);
				Chat("guild_league", RaoTxt.Replace("\"", "") + text);
			}
			if (ChatBangPhai)
			{
				Thread.Sleep(50);
				Chat("guild", RaoTxt.Replace("\"", "") + text);
			}
			if (ChatDoi)
			{
				Thread.Sleep(50);
				Chat("team", RaoTxt.Replace("\"", "") + text);
			}
			if (ChatTheGioi)
			{
				Thread.Sleep(50);
				Chat("scene", RaoTxt.Replace("\"", "") + text);
			}
		}

		public void MessageboxSelfOkClicked()
		{
			PostMessage(19, 105);
		}

		public void CollectItem()
		{
			if (TickCount % 3 != 0)
			{
				return;
			}
			int num = Memory.Read(Address.LootPacketId);
			if (IsOptLocDo)
			{
				lootPacketIdDua.Add(num);
			}
			if (!lootPacketId.Contains(num))
			{
				lootPacketId.Add(num);
			}
			else if ((!IsKhoang || !IsMapNghe()) && (!IsDuoc || !IsMapNghe()) && !IsThuHoach && !IsBachHoaDuyen && !IsNhatHop && !IsNhatTuyet && !IsPhuMau && !IsNhatHopQDua)
			{
				return;
			}
			if (lootPacketId.Count > 30)
			{
				lootPacketId.RemoveAt(0);
			}
			if (!IsCollect())
			{
				return;
			}
			int num2 = Memory.ReadAddress(Address.LootPacketItem);
			for (int i = 0; i < 10; i++)
			{
				int num3 = Memory.Read(num2 + i * 4);
				int num4 = Memory.Read(num3 + 8);
				if (IsOptLocDo)
				{
					int num5 = Memory.Read(num3);
					string str;
					if (num5 == Address.PacketType1 || num5 == Address.PacketType5)
					{
						str = Memory.ReadString(Memory.Read(num3 + 40, 40));
						Memory.ReadString(Memory.Read(num3 + 40, 88));
						Memory.ReadString(Memory.Read(num3 + 40, 84));
					}
					else if (num5 == Address.PacketType2)
					{
						str = Memory.ReadString(Memory.Read(num3 + 40, 24));
						Memory.ReadString(Memory.Read(num3 + 40, 80));
						Memory.ReadString(Memory.Read(num3 + 40, 20));
					}
					else if (num5 == Address.PacketType3)
					{
						str = Memory.ReadString(Memory.Read(num3 + 40, 28));
						Memory.ReadString(Memory.Read(num3 + 40, 304));
						Memory.ReadString(Memory.Read(num3 + 40, 20));
					}
					else if (num5 == Address.PacketType4)
					{
						str = Memory.ReadString(Memory.Read(num3 + 40, 40));
						Memory.ReadString(Memory.Read(num3 + 40, 76));
						Memory.ReadString(Memory.Read(num3 + 40, 20));
					}
					else
					{
						str = Memory.ReadString(Memory.Read(num3 + 40, 88));
						Memory.ReadString(Memory.Read(num3 + 40, 80));
						Memory.ReadString(Memory.Read(num3 + 40, 20));
					}
					if (num5 == Address.PacketType6)
					{
						str = Memory.ReadString(Memory.Read(num3 + 40, 44));
						Memory.ReadString(Memory.Read(num3 + 40, 104));
						Memory.ReadString(Memory.Read(num3 + 40, 40));
					}
					if (TINHKIEM.VietLien(IdLocDo).Contains(TINHKIEM.VietLien(str)) && num3 != 0)
					{
						PostMessage(num3, 107);
					}
				}
				if (IsSuMon || IsXayDung)
				{
					if (num4.ToString().Contains("400030"))
					{
						TrangThaiSuMon = "PickItem";
					}
					if (num4.ToString().Contains("40004"))
					{
						TrangThaiXayDung = "PickItem";
					}
				}
				if ((num3 != 0 && (IsNhatHopQDua || IsNhatHopall || IsQDua || IsBachHoaDuyen || IsPhuMau || IsNhatTuyet || IsNhatHop || (IsKhoang && IsMapNghe()) || (IsDuoc && IsMapNghe()) || IsThuHoach)) || !Global.ItemFillter)
				{
					if (!IsOptLocDo)
					{
						PickAll();
					}
					break;
				}
				if (IsVIPItem(num4) && !IsOptLocDo && num3 != 0)
				{
					PostMessage(num3, 107);
				}
			}
		}

		public static bool IsVIPItem(int item)
		{
			if (item != 0)
			{
				switch (item)
				{
				default:
					if (item != 20109101 && item != 20109102 && item != 30008053 && item != 30103042 && item != 10141153 && item != 10157001 && item != 10157002 && item != 10156001 && item != 10156002 && item != 10156003 && item != 10156004 && !item.ToString().StartsWith("30120") && !item.ToString().StartsWith("10124"))
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

		public bool ForcePickItem()
		{
			if (TLBB.IsFollow)
			{
				return false;
			}
			if (TLBB.PlayerState == 5)
			{
				return false;
			}
			float num = Global.PickRadius;
			int num2 = -1;
			foreach (GameObject item in Objects.LootPacket)
			{
				if (!lootPacketId.Contains(item.Id))
				{
					item.DistanceEx = item.GetDistance(CharX, CharY);
					if (item.DistanceEx < num)
					{
						num = item.DistanceEx;
						num2 = item.Id;
					}
				}
			}
			if (num2 == -1)
			{
				return false;
			}
			PostMessage(num2, 106);
			if (PickId != num2)
			{
				PickId = num2;
				PickTime = Stopwatch.StartNew();
			}
			else if (PickTime.Elapsed.TotalSeconds > 2.0 && TimeStand.Elapsed.TotalSeconds > 2.0 && TLBB.MapId != MAP.PhieuMieuPhong && TLBB.MapId != MAP.BinhThanhKyTran)
			{
				lootPacketId.Add(num2);
			}
			if (TimeStand.Elapsed.TotalSeconds > 3.0)
			{
				FixKetMap();
				StandTime = 0;
				return true;
			}
			return true;
		}

		public void PickItem(int id)
		{
			if (id != PickedId)
			{
				PickedId = id;
				CareTime = Stopwatch.StartNew();
			}
			else if (CareTime.Elapsed.TotalSeconds > 30.0)
			{
				BlackList.Add(PickedId);
			}
			if (Memory.Read(Address.LootPacketId) != id)
			{
				if (TLBB.PlayerState == 8)
				{
					pickTiem = Stopwatch.StartNew();
				}
				if (!(pickTiem.Elapsed.TotalSeconds < 1.0))
				{
					PostMessage(id, 106);
				}
			}
		}

		public bool PickItem()
		{
			if (IsXayDung || IsSuMon || IsTuBaoBon || IsTuDuong || IsBachHoaDuyen || TLBB.IsODaoCuFull || TLBB.IsONguyenLieuFull)
			{
				return false;
			}
			if (TLBB.MapId == MAP.TangKinhCac && TINHKIEM.GetDistance(CharX, CharY, 64f, 28f) > 15f)
			{
				return false;
			}
			if (TLBB.Busy && TLBB.PlayerState != 7)
			{
				return false;
			}
			if (TLBB.IsFollow && !TLBB.IsLeader)
			{
				return false;
			}
			if (!IsPickItem)
			{
				return false;
			}
			if (Global.PickItem || TLBB.MapId == MAP.TangKinhCac || IsLuyenKim || IsPickEx || IsQDua || IsNhatHopall || IsNhatHopQDua)
			{
				float num = Global.PickRadius;
				int num2 = -1;
				float x = 0f;
				float y = 0f;
				foreach (GameObject item in Objects.LootPacket)
				{
					if (!lootPacketId.Contains(item.Id))
					{
						item.DistanceEx = item.GetDistance(CharX, CharY);
						if (item.DistanceEx < num)
						{
							num = item.DistanceEx;
							num2 = item.Id;
							x = item.X;
							y = item.Y;
						}
					}
				}
				if (num2 != -1)
				{
					if (TLBB.MapId == MAP.TangKinhCac)
					{
						Move(x, y);
					}
					PostMessage(num2, 106);
					if (PickId != num2)
					{
						PickId = num2;
						PickTime = Stopwatch.StartNew();
					}
					else if (PickTime.Elapsed.TotalSeconds > 2.0 && TimeStand.Elapsed.TotalSeconds > 2.0 && TLBB.MapId != MAP.PhieuMieuPhong && TLBB.MapId != MAP.BinhThanhKyTran)
					{
						lootPacketId.Add(num2);
					}
					if (TimeStand.Elapsed.TotalSeconds > 3.0)
					{
						FixKetMap();
						StandTime = 0;
					}
					return true;
				}
			}
			return false;
		}

		public void AnDon()
		{
			foreach (Skill skill in Skills)
			{
				if (skill.PacketId == 248)
				{
					if (Memory.Read(TLBB.DelayBase + skill.DelayOffset) <= 0)
					{
						UseSkill(skill.PacketId);
					}
					break;
				}
			}
		}

		private void PickAll()
		{
			PostMessage(33, 105);
			Memory.Write(Address.PickAll, 1);
		}

		public void Hide()
		{
			Thread thread = new Thread(HideThread);
			thread.IsBackground = true;
			thread.Start();
		}

		[DllImport("user32.dll")]
		private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		public void HideThread()
		{
			if (Parrent == IntPtr.Zero)
			{
				Handle = Win.GetHandle(ProcessId, Win.WndClassNames);
				Win.ShowWindow(Handle, Win.WindowShowStyle.Minimize);
				Win.ShowWindow(Handle, Win.WindowShowStyle.Hide);
				return;
			}
			if (style == 0)
			{
				style = Games.GetWindowLong(Handle, -16);
			}
			int num = style;
			num &= -12582913;
			num &= -536870913;
			num &= -65537;
			num &= -131073;
			Games.SetWindowLong(Handle, -16, num);
			LuaDoString("PushEvent('VIEW_RESOLUTION_CHANGED')");
			SetParent(Handle, Parrent);
		}

		public void Active()
		{
			if (!IsSuspend)
			{
				IsHide = false;
				Thread thread = new Thread(ActiveThread);
				thread.IsBackground = true;
				thread.Start();
			}
		}

		public void ActiveThread()
		{
			if (Parrent == IntPtr.Zero)
			{
				Handle = Win.GetHandle(ProcessId, Win.WndClassNames);
			}
			else
			{
				Games.SetWindowLong(Handle, -16, style);
				SetParent(Handle, IntPtr.Zero);
			}
			Win.Active(Handle);
		}

		public void Exit()
		{
			Parrent = IntPtr.Zero;
			FrmMain.dicGame.Remove(ProcessId);
			try
			{
				Process.GetProcessById(ProcessId).Kill();
			}
			catch
			{
			}
			try
			{
				if (LastName != "ĐăngNhập" && !BachHoaDuyenCompleted)
				{
					FrmMain.AddLog(DateTime.Now.ToString("HH:mm dd-MM") + " Thoát " + LastName + "\n");
				}
				if (alarmVaoPhai != null && alarmVaoPhai.Visible)
				{
					alarmVaoPhai.Dispose();
				}
				Live = false;
			}
			catch
			{
			}
		}

		public void Quit()
		{
			IsQuit = true;
			LUA.AskRet2SelServer();
		}

		public void ForceAttack()
		{
			if (TickCount % 3 != 0 || TINHKIEM.IsPressed(VirtualKeyStates.VK_LMENU) || (TLBB.IsNoi && TLBB.PlayerState == 2) || TLBB.IsFollow || IsRide)
			{
				return;
			}
			if (Global.AtkFollowKey && Objects.Key != null && !TLBB.IsLeader)
			{
				if (TickCount % 9 == 0 && Objects.Key.State == 7 && (Objects.Self.AtkToId != Objects.Key.AtkToId || Objects.Self.State != 7))
				{
					SelectTarget(Objects.Key.AtkToId);
					SendKey(Global.BaseSkill);
				}
			}
			else if (!Global.Paused && (Objects.Target == null || Objects.Target.HP <= 0f || (!Objects.TargetIsMine && Objects.MyMonter.Count > 0) || TimeStand.Elapsed.TotalSeconds > 0.4 || (IsLureEx && !Objects.Target.Belong.Contains("FFFFFFFF") && Objects.UnBelongMonter.Count > 0)))
			{
				GetBestTarget();
				if (BestTarget != null)
				{
					SelectTarget(BestTarget.Id);
					SendKey(Global.BaseSkill);
				}
			}
		}

		public void Attack()
		{
			if (IsNhiemVuCoBan || IsPhuMau || IsTrieuTap || TLBB.PlayerState == 5 || TLBB.PlayerState == 6 || IsXayDung || IsSuMon || IsTuBaoBon || IsTuDuong || IsLuyenKim || IsTrungAc || IsBachHoaDuyen || (IsKhoang && IsMapNghe()) || (IsDuoc && IsMapNghe()) || TINHKIEM.IsPressed(VirtualKeyStates.VK_LMENU) || TINHKIEM.IsPressed(VirtualKeyStates.VK_RMENU) || ((TLBB.IsNoi || IsMapPhuBan()) && TLBB.PlayerState == 2) || TLBB.IsFollow || !IsAuto || !IsAttack || IsRide)
			{
				return;
			}
			if (Global.AtkFollowKey && Objects.Key != null && !TLBB.IsLeader)
			{
				if (TickCount % 9 != 0)
				{
					return;
				}
				if (Objects.Key.State == 7)
				{
					if (Objects.Self.AtkToId == Objects.Key.AtkToId && Objects.Self.State == 7)
					{
						return;
					}
					foreach (GameObject item in Objects.All)
					{
						if (item.Id == Objects.Key.AtkToId)
						{
							BestTarget = item;
							break;
						}
					}
					SelectTarget(Objects.Key.AtkToId);
					SendKey(Global.BaseSkill);
				}
				else if (Address.GameType != 2)
				{
					if (!isAtkFollow)
					{
						SendKey(Global.BaseSkill);
						isAtkFollow = true;
					}
					else
					{
						SelectTarget(Objects.Key.Id);
						SelectTargetOfTarget();
						isAtkFollow = false;
					}
				}
			}
			else
			{
				if (Global.Paused)
				{
					return;
				}
				if (TimeStand.Elapsed.TotalSeconds > 0.5 && IsRadius && TINHKIEM.GetDistance(RadiusX, RadiusY, CharX, CharY) > 5f)
				{
					Move(RadiusX, RadiusY);
				}
				if (Objects.Target != null && !(Objects.Target.HP <= 0f) && (Objects.TargetIsMine || Objects.MyMonter.Count <= 0) && !(TimeStand.Elapsed.TotalSeconds > 0.4) && (!IsLureEx || TLBB.MapId == MAP.TranLongKyCuoc || TLBB.MapId == MAP.LauLanBaoTang || Objects.Target.Belong.Contains("FFFFFFFF") || Objects.UnBelongMonter.Count <= 0))
				{
					return;
				}
				GetBestTarget();
				if (BestTarget != null)
				{
					if ((TLBB.MapId == MAP.LauLanBaoTang || TLBB.MapId == 61) && TINHKIEM.GetDistance(CharX, CharY, BestTarget.X, BestTarget.Y) > 3f)
					{
						Move(BestTarget.X, BestTarget.Y);
						return;
					}
					SelectTarget(BestTarget.Id);
					SendKey(Global.BaseSkill);
				}
			}
		}

		public void SelectTargetOfTarget()
		{
			PostMessage(0, 105);
		}

		public void Move(float x, float y, int map)
		{
			if (lastAutoMove.Elapsed.TotalSeconds < 4.0)
			{
				return;
			}
			if (TLBB.MapId == map)
			{
				if (TINHKIEM.GetDistance(CharX, CharY, x, y) > 3f)
				{
					Move(x, y);
				}
				return;
			}
			if (TLBB.MapId == MAP.YenTuO)
			{
				Move(64f, 21f);
				return;
			}
			if (Address.GameType != 1)
			{
				if (Unity.IsMessengerBox(TLBB.MapId, (int)CharX, (int)CharY))
				{
					LuaDoOneLineString("IsMessageBox = 1;");
				}
				else
				{
					if (ON_SCENE_TRANSING || TLBB.PlayerState == 7)
					{
						return;
					}
					if (TLBB.MapId != 6 && TLBB.MapId != 7 && TLBB.MapId != 24 && map == 2 && TLBB.MapId != 2)
					{
						DownRide();
						UseSkill(22);
						return;
					}
					if (!IsMove)
					{
						FixKetMap();
						return;
					}
					if (TLBB.MapId == map)
					{
						if (!(TINHKIEM.GetDistance(CharX, CharY, x, y) <= 3f))
						{
							Move(x, y);
						}
						return;
					}
					if (UsingTholinhChau)
					{
						int num = PhuIndex(Unity.GetFakeMapID(map));
						if (num != -1)
						{
							if (IsRide)
							{
								DownRide();
							}
							else
							{
								PlayerPackageUseItem(num);
							}
							return;
						}
					}
					if (TLBB.HaveRide && !IsRide)
					{
						UpRide();
						return;
					}
					PathInfo pathInfo = null;
					try
					{
						pathInfo = FindPath.GetNextPath(TLBB.MapId, map);
					}
					catch
					{
						PushDebugMessage("Không thể tìm đường");
						return;
					}
					if (!pathInfo.isNPC)
					{
						Move(pathInfo.x, pathInfo.y);
						return;
					}
					string value = "KhongCo";
					bool flag = false;
					switch (pathInfo.idNext)
					{
					case 9:
						value = "Thiếu Lâm";
						flag = true;
						break;
					case 11:
						value = "Minh Giáo";
						flag = true;
						break;
					case 10:
						value = "Cái Bang";
						flag = true;
						break;
					case 16:
						value = "Tinh Túc";
						flag = true;
						break;
					case 12:
						value = "Võ Đang";
						flag = true;
						break;
					case 13:
						value = "Thiên Long";
						flag = true;
						break;
					case 17:
						value = "Thiên Sơn";
						flag = true;
						break;
					case 15:
						value = "Nga My";
						flag = true;
						break;
					case 14:
						value = "Tiêu Dao";
						flag = true;
						break;
					case 284:
						value = "Mộ Dung";
						flag = true;
						break;
					}
					Move(pathInfo.x, pathInfo.y);
					if (flag)
					{
						foreach (QuestFrame item in QuestFrame.Enum(this))
						{
							if (item.Name.Contains(value))
							{
								QuestFrameOptionClicked(item);
								break;
							}
						}
						foreach (QuestFrame item2 in QuestFrame.Enum(this))
						{
							if (item2.Name.Contains("Đến các môn phái"))
							{
								QuestFrameOptionClicked(item2);
								return;
							}
						}
					}
					if (!flag)
					{
						foreach (QuestFrame item3 in QuestFrame.Enum(this))
						{
							if (item3.Name.Contains("Duyệt") || item3.Name.Contains("Xác nhận"))
							{
								QuestFrameOptionClicked(item3);
								break;
							}
						}
						value = FindPath.GetScreenName(pathInfo.idNext);
						if (TINHKIEM.VietLien(value).Contains("thuchacotran"))
						{
							LuaDoOneLineString("IsMessageBox = 1;");
							return;
						}
						foreach (QuestFrame item4 in QuestFrame.Enum(this))
						{
							string text = TINHKIEM.VietLienRemoveNum(item4.Name);
							string value2 = TINHKIEM.VietLienRemoveNum(value);
							if (text.Contains(value2))
							{
								QuestFrameOptionClicked(item4);
								break;
							}
						}
					}
					if (lastTalk.Elapsed.TotalSeconds > 5.0)
					{
						foreach (GameObject item5 in Objects.AllNpc)
						{
							if (TINHKIEM.VietLienRemoveNum(item5.Name).Contains(TINHKIEM.VietLienRemoveNum(pathInfo.NpcName)))
							{
								Talk(item5.Id);
								lastTalk = Stopwatch.StartNew();
								break;
							}
						}
					}
					lastAutoMove = Stopwatch.StartNew();
				}
				return;
			}
			if (TLBB.MapId == MAP.PhieuMieuPhong)
			{
				if (Address.GameType == 1)
				{
					if (GoTo(96f, 40f))
					{
						if (TLBB.IsQuestOpen)
						{
							QuestFrame.Click(402275, 3);
							QuestFrame.Click(402275, 4);
							QuestFrameOptionClicked(402276, 3);
							QuestFrameOptionClicked(402288, 4);
						}
						else
						{
							Talk("olaodai");
						}
					}
				}
				else
				{
					Move(125f, 171f);
				}
				return;
			}
			if (TLBB.MapId == MAP.TangKinhCac)
			{
				Move(70f, 20f);
				return;
			}
			if (TLBB.MapId == MAP.ThienLongPhuBan)
			{
				Move(96f, 142f);
				return;
			}
			if (TLBB.MapId == MAP.MoDungPhuBan)
			{
				Move(160f, 169f);
				return;
			}
			if (TLBB.MapId == MAP.TacKhauDoanhDia)
			{
				Move(86f, 116f);
				return;
			}
			if (TLBB.MapId == MAP.DuongMonPhuBan)
			{
				Move(173f, 170f);
				return;
			}
			if (TLBB.MapId == MAP.TinhTucPhuBan)
			{
				Move(96f, 142f);
				return;
			}
			if (TLBB.MapId == MAP.TieuDaoPhuBan)
			{
				Move(44f, 129f);
				return;
			}
			if (TLBB.MapId == MAP.ThieuLamPhuBan)
			{
				Move(96f, 158f);
				return;
			}
			if (TLBB.MapId == MAP.ThienSonPhuBan)
			{
				Move(95f, 148f);
				return;
			}
			if (TLBB.MapId == MAP.NgaMyPhuBan)
			{
				Move(89f, 146f);
				return;
			}
			if (TLBB.MapId == MAP.VoDangPhuBan)
			{
				Move(95f, 192f);
				return;
			}
			if (TLBB.MapId == MAP.MinhGiaoPhuBan)
			{
				Move(98f, 159f);
				return;
			}
			if (TLBB.MapId == MAP.CaiBangPhuBan)
			{
				Move(91f, 159f);
				return;
			}
			if (TLBB.MapId == 604)
			{
				Move(45f, 51f);
				return;
			}
			_ = TLBB.MapId;
			_ = MAP.GiamNguc;
			if (TLBB.MapId == 550)
			{
				foreach (GameObject item6 in Objects.AllNpc)
				{
					_ = item6.X;
					_ = item6.Y;
					if (x == 40f && y == 40f)
					{
						Talk(item6.Id);
						if (TLBB.IsQuestOpen)
						{
							QuestFrame.ClickOut(this);
						}
						return;
					}
				}
				if (TINHKIEM.GetDistance(CharX, CharY, 104f, 79f) > 3f)
				{
					Move(40f, 40f);
				}
			}
			else if (TLBB.MapId == MAP.VanKiemCoc || TLBB.MapId == MAP.VanKiemCocDem)
			{
				if (TLBB.IsQuestOpen)
				{
					QuestFrame.ClickOut(this);
					CloseQuest();
				}
				foreach (GameObject item7 in Objects.All)
				{
					if (item7.CleanName == "hoahachcan" || item7.CleanName == "vankiepcocmocnhanthuve")
					{
						Talk(item7.Id);
						return;
					}
				}
				if (TINHKIEM.GetDistance(CharX, CharY, 105f, 79f) > 3f)
				{
					Move(105f, 79f);
				}
			}
			else if (map == MAP.VanKiemCoc || map == MAP.VanKiemCocDem)
			{
				if (TLBB.IsQuestOpen)
				{
					QuestFrame.ClickPhuBanMonPhai(this);
					QuestFrame.Close();
				}
				else if (GoTo(DAILY.HoaHachCan))
				{
					Talk(DAILY.HoaHachCan);
				}
			}
			else if (map == MAP.ThieuLamPhuBan)
			{
				if (TLBB.MapId != MAP.ThieuLam)
				{
					GoTo(NPC.HuyenChung.X, NPC.HuyenChung.Y, NPC.HuyenChung.Map);
				}
				else if (TINHKIEM.GetDistance(CharX, CharY, NPC.HuyenChung.X, NPC.HuyenChung.Y) > 3f)
				{
					Move(NPC.HuyenChung.X, NPC.HuyenChung.Y);
				}
				else if (TLBB.IsQuestOpen)
				{
					QuestFrame.ClickPhuBanMonPhai(this);
				}
				else
				{
					Talk(NPC.HuyenChung.Id);
				}
			}
			else if (map == MAP.CaiBangPhuBan)
			{
				if (TLBB.MapId != MAP.CaiBang)
				{
					GoTo(NPC.AuDuongQua.X, NPC.AuDuongQua.Y, NPC.AuDuongQua.Map);
				}
				else if (TINHKIEM.GetDistance(CharX, CharY, NPC.AuDuongQua.X, NPC.AuDuongQua.Y) > 3f)
				{
					Move(NPC.AuDuongQua.X, NPC.AuDuongQua.Y);
				}
				else if (TLBB.IsQuestOpen)
				{
					QuestFrame.ClickPhuBanMonPhai(this);
				}
				else
				{
					Talk(NPC.AuDuongQua.Id);
				}
			}
			else if (map == MAP.MinhGiaoPhuBan)
			{
				if (TLBB.MapId != MAP.MinhGiao)
				{
					GoTo(NPC.ThacCang.X, NPC.ThacCang.Y, NPC.ThacCang.Map);
				}
				else if (TINHKIEM.GetDistance(CharX, CharY, NPC.ThacCang.X, NPC.ThacCang.Y) > 3f)
				{
					Move(NPC.ThacCang.X, NPC.ThacCang.Y);
				}
				else if (TLBB.IsQuestOpen)
				{
					QuestFrame.ClickPhuBanMonPhai(this);
				}
				else
				{
					Talk(NPC.ThacCang.Id);
				}
			}
			else if (map == MAP.VoDangPhuBan)
			{
				if (TLBB.MapId != MAP.VoDang)
				{
					GoTo(NPC.TieuThienDat.X, NPC.TieuThienDat.Y, NPC.TieuThienDat.Map);
				}
				else if (TINHKIEM.GetDistance(CharX, CharY, NPC.TieuThienDat.X, NPC.TieuThienDat.Y) > 3f)
				{
					Move(NPC.TieuThienDat.X, NPC.TieuThienDat.Y);
				}
				else if (TLBB.IsQuestOpen)
				{
					QuestFrame.ClickPhuBanMonPhai(this);
				}
				else
				{
					Talk(NPC.TieuThienDat.Id);
				}
			}
			else if (map == MAP.ThienLongPhuBan)
			{
				if (TLBB.MapId != MAP.ThienLong)
				{
					GoTo(NPC.HoTuTruongLao.X, NPC.HoTuTruongLao.Y, NPC.HoTuTruongLao.Map);
				}
				else if (TINHKIEM.GetDistance(CharX, CharY, NPC.HoTuTruongLao.X, NPC.HoTuTruongLao.Y) > 3f)
				{
					Move(NPC.HoTuTruongLao.X, NPC.HoTuTruongLao.Y);
				}
				else if (TLBB.IsQuestOpen)
				{
					QuestFrame.ClickPhuBanMonPhai(this);
				}
				else
				{
					Talk(NPC.HoTuTruongLao.Id);
				}
			}
			else if (map == MAP.TieuDaoPhuBan)
			{
				if (TLBB.MapId != MAP.TieuDao)
				{
					GoTo(NPC.CongDaTuTruong.X, NPC.CongDaTuTruong.Y, NPC.CongDaTuTruong.Map);
				}
				else if (TINHKIEM.GetDistance(CharX, CharY, NPC.CongDaTuTruong.X, NPC.CongDaTuTruong.Y) > 3f)
				{
					Move(NPC.CongDaTuTruong.X, NPC.CongDaTuTruong.Y);
				}
				else if (TLBB.IsQuestOpen)
				{
					QuestFrame.ClickPhuBanMonPhai(this);
				}
				else
				{
					Talk(NPC.CongDaTuTruong.Id);
				}
			}
			else if (map == MAP.NgaMyPhuBan)
			{
				if (TLBB.MapId != MAP.NgaMy)
				{
					GoTo(NPC.LieuTamMuoi.X, NPC.LieuTamMuoi.Y, NPC.LieuTamMuoi.Map);
				}
				else if (TINHKIEM.GetDistance(CharX, CharY, NPC.LieuTamMuoi.X, NPC.LieuTamMuoi.Y) > 3f)
				{
					Move(NPC.LieuTamMuoi.X, NPC.LieuTamMuoi.Y);
				}
				else if (TLBB.IsQuestOpen)
				{
					QuestFrame.ClickPhuBanMonPhai(this);
				}
				else
				{
					Talk(NPC.LieuTamMuoi.Id);
				}
			}
			else if (map == MAP.TinhTucPhuBan)
			{
				if (TLBB.MapId != MAP.TinhTuc)
				{
					GoTo(NPC.ThienToanTu.X, NPC.ThienToanTu.Y, NPC.ThienToanTu.Map);
				}
				else if (TINHKIEM.GetDistance(CharX, CharY, NPC.ThienToanTu.X, NPC.ThienToanTu.Y) > 3f)
				{
					Move(NPC.ThienToanTu.X, NPC.ThienToanTu.Y);
				}
				else if (TLBB.IsQuestOpen)
				{
					QuestFrame.ClickPhuBanMonPhai(this);
				}
				else
				{
					Talk(NPC.ThienToanTu.Id);
				}
			}
			else if (map == MAP.ThienSonPhuBan)
			{
				if (TLBB.MapId != MAP.ThienSon)
				{
					GoTo(NPC.DangBa.X, NPC.DangBa.Y, NPC.DangBa.Map);
				}
				else if (TINHKIEM.GetDistance(CharX, CharY, NPC.DangBa.X, NPC.DangBa.Y) > 3f)
				{
					Move(NPC.DangBa.X, NPC.DangBa.Y);
				}
				else if (TLBB.IsQuestOpen)
				{
					QuestFrame.ClickPhuBanMonPhai(this);
				}
				else
				{
					Talk(NPC.DangBa.Id);
				}
			}
			else if (map == MAP.MoDungPhuBan)
			{
				if (TLBB.MapId != MAP.MoDung)
				{
					GoTo(NPC.CongDaKhon.X, NPC.CongDaKhon.Y, NPC.CongDaKhon.Map);
				}
				else if (TINHKIEM.GetDistance(CharX, CharY, NPC.CongDaKhon.X, NPC.CongDaKhon.Y) > 3f)
				{
					Move(NPC.CongDaKhon.X, NPC.CongDaKhon.Y);
				}
				else if (TLBB.IsQuestOpen)
				{
					QuestFrame.ClickPhuBanMonPhai(this);
				}
				else
				{
					Talk(NPC.CongDaKhon.Id);
				}
			}
			else if (map == MAP.DuongMonPhuBan)
			{
				if (TLBB.MapId != MAP.DuongMon)
				{
					GoTo(NPC.DuongMoTuong.X, NPC.DuongMoTuong.Y, NPC.DuongMoTuong.Map);
				}
				else if (TINHKIEM.GetDistance(CharX, CharY, NPC.DuongMoTuong.X, NPC.DuongMoTuong.Y) > 3f)
				{
					Move(NPC.DuongMoTuong.X, NPC.DuongMoTuong.Y);
				}
				else if (TLBB.IsQuestOpen)
				{
					QuestFrame.ClickPhuBanMonPhai(this);
				}
				else
				{
					Talk(NPC.DuongMoTuong.Id);
				}
			}
			else if (TLBB.MapId == MAP.PhungHoangCoThanh)
			{
				if (GoTo(PHUNGHOANGCOTHANH.HoangLongThien))
				{
					if (TLBB.IsQuestOpen)
					{
						QuestFrameOptionClicked(403007, 1);
						CloseQuest();
					}
					else
					{
						Talk("hoanglongthien");
					}
				}
			}
			else
			{
				if (TLBB.PlayerState == 2)
				{
					return;
				}
				if (IdleTime > 3)
				{
					CloseMessageBox();
				}
				if (map == MAP.PhungHoangCoThanh)
				{
					if (GoTo(THUCHACOTRAN.LyDa))
					{
						if (TLBB.IsQuestOpen)
						{
							QuestFrameOptionClicked(403001, 1);
							CloseQuest();
						}
						else
						{
							Talk(THUCHACOTRAN.LyDa.Id);
						}
					}
					return;
				}
				lastAutoMove = Stopwatch.StartNew();
				PostMessage((int)x, 50);
				PostMessage((int)y, 51);
				PostMessage(map, 52);
				PostMessage(10, 105);
				if (map == MAP.ThaiHo)
				{
					LuaDoOneLineString("IsMessageBox = 1;");
				}
			}
		}

		public void CloseMessageBox()
		{
			LuaDoOneLineString("setmetatable(_G, {__index = MessageBox_Self_Env}); this:Hide();");
		}

		private void MoveEx(float x, float y)
		{
			PostMessage(Memory.Float2Int(x), 50);
			PostMessage(Memory.Float2Int(y), 51);
			PostMessage(0, 119);
		}

		public bool Move(float x, float y)
		{
			if (lastAutoMove.Elapsed.TotalSeconds < 2.0)
			{
				return false;
			}
			if ((TLBB.MapId == MAP.VanKiemCoc || TLBB.MapId == MAP.VanKiemCocDem) && TINHKIEM.GetDistance(CharX, CharY, 110f, 110f) < 20f)
			{
				foreach (GameObject item in Objects.All)
				{
					if ((int)item.X == 107 && ((int)item.Y == 110 || (int)item.Y == 111))
					{
						Talk(item.Id);
					}
				}
				if (TLBB.IsQuestOpen)
				{
					QuestFrame.ClickPhuBanMonPhai(this);
				}
			}
			if ((double)TINHKIEM.GetDistance(CharX, CharY, x, y) < 1.5)
			{
				return true;
			}
			if (TLBB.MapId == MAP.YenTuO && x > 25f && x < 165f && y > 20f && y < 125f && (RoundX < 25 || RoundX > 165 || RoundY < 30 || RoundY > 125))
			{
				x = YENTUO.HoaHachCan.X;
				y = YENTUO.HoaHachCan.Y;
				IsP = true;
			}
			if ((TLBB.MapId > 500 && IsNhatHopQDua && !IsQDua) || (TLBB.MapId > 500 && IsNhatHopQDua && TrangThaiQD == "NhatQua"))
			{
				if (x > 49f && y > 66f)
				{
					x = 49f;
				}
				if (y > 82f)
				{
					y = 82f;
				}
				if (x > 50f && y > 65f && y <= 66f)
				{
					x = 51f;
					y = 64f;
				}
			}
			PostMessage((int)x, 50);
			PostMessage((int)y, 51);
			PostMessage(9, 105);
			return false;
		}

		public void SelectTarget(int targetId)
		{
			TargetId = targetId;
			PostMessage(targetId, 100);
		}

		public void GetBestTarget()
		{
			BestTarget = null;
			if (TLBB.MapId == MAP.YenTuO)
			{
				foreach (GameObject item in Objects.Monter)
				{
					if (item.CleanName == "doandienkhanh" || item.CleanName == "nhaclaotam" || item.CleanName == "diepnhinuong")
					{
						BestTarget = item;
						return;
					}
				}
			}
			if (TLBB.MapId == MAP.PhungMinhVuongLang)
			{
				foreach (GameObject item2 in Objects.Monter)
				{
					if (item2.Name == "Thị Ma Giả")
					{
						BestTarget = item2;
						return;
					}
				}
			}
			if (TLBB.MapId == 272)
			{
				foreach (GameObject item3 in Objects.Monter)
				{
					if (item3.Name.Contains("o Th") && item3.Name.EndsWith("ng"))
					{
						BestTarget = item3;
						return;
					}
				}
			}
			float num;
			if (!IsRadius)
			{
				num = 9999f;
			}
			else
			{
				num = ((!TLBB.IsNoi) ? ((float)Global.NgoaiRadius) : ((float)Global.NoiRadius));
				float distance = TINHKIEM.GetDistance(CharX, CharY, RadiusX, RadiusY);
				if (num < distance)
				{
					num = distance;
				}
			}
			if (IsTheoQ)
			{
				num = 23f;
			}
			if (IsTheoQ)
			{
				foreach (KeyValuePair<int, Game> item4 in FrmMain.dicGame)
				{
					Game value = item4.Value;
					if (value.TLBB.Id == TLBB.KeyId)
					{
						RadiusX = value.CharX;
						RadiusY = value.CharY;
					}
				}
			}
			if (IsRadius || (IsTheoQ && RadiusX != 0f))
			{
				List<GameObject> list = new List<GameObject>();
				foreach (GameObject item5 in Objects.Monter)
				{
					if (item5.GetDistance(RadiusX, RadiusY) > num)
					{
						list.Add(item5);
					}
				}
				foreach (GameObject item6 in list)
				{
					Objects.Monter.Remove(item6);
					Objects.MyMonter.Remove(item6);
					Objects.UnBelongMonter.Remove(item6);
					if (Objects.UnBelongMonter.Count == 0 && TLBB.PlayerState != 0)
					{
						return;
					}
				}
			}
			List<GameObject> list2 = ((IsLureEx && Objects.UnBelongMonter.Count > 0) ? Objects.UnBelongMonter : ((Objects.MyMonter.Count <= 0) ? Objects.Monter : Objects.MyMonter));
			float num2 = num;
			foreach (GameObject item7 in list2)
			{
				if (!IsLureEx || !LureId.Contains(item7.Id))
				{
					if (IsRadius)
					{
						item7.DistanceEx = item7.GetDistance(RadiusX, RadiusY);
					}
					else
					{
						item7.DistanceEx = item7.GetDistance(CharX, CharY);
					}
					if (list2.Count > 1 && item7 == Objects.Target && TimeStand.Elapsed.TotalSeconds > 0.3 && AtackTime++ == 4)
					{
						AtackTime = 0;
						StandTime = 0;
					}
					else if (item7.DistanceEx <= num2)
					{
						num2 = item7.DistanceEx;
						BestTarget = item7;
					}
				}
			}
			if (BestTarget != Objects.Target)
			{
				AtackTime = 0;
			}
			if (BestTarget != null && IsLureEx)
			{
				LureId.Add(BestTarget.Id);
			}
			if (TLBB.MapId == MAP.TranLongKyCuoc || TLBB.MapId == MAP.LauLanBaoTang)
			{
				LureId.Clear();
			}
			if (BestTarget == null && IsLureEx)
			{
				if (list2.Count > 0)
				{
					BestTarget = list2[0];
				}
				else if (Objects.Monter.Count > 0)
				{
					BestTarget = list2[0];
				}
			}
		}

		public void SkillDo()
		{
			if (IsTheoQ || IsYenTuO || TickCount % 12 != 0 || TLBB.IsFollow || !IsAuto || IsRide || Global.Paused || TLBB.IsBienThan || TLBB.BusyEx)
			{
				return;
			}
			foreach (Skill skill in Skills)
			{
				if (skill.PacketId == 448 && IsMapPhuBan() && Objects.NearMonter5m.Count >= 3 && Global.UsingSkill)
				{
					int num = Memory.Read(TLBB.DelayBase + skill.DelayOffset);
					if (num == 0 || num == -1)
					{
						DoSkill(skill.PacketId);
					}
				}
				else if ((skill.Use || (skill.UsePK && IsPK) || (skill.UserBuff && !Skill.IsBase(skill.PacketId) && TLBB.PlayerState != 2 && TLBB.PlayerState != 5)) && Global.UsingSkill && IsAttack)
				{
					int num2 = Memory.Read(TLBB.DelayBase + skill.DelayOffset);
					if (num2 == 0 || num2 == -1)
					{
						DoSkill(skill.PacketId);
					}
				}
			}
		}

		public void DoSkill(int id)
		{
			if (ON_SCENE_TRANSING || IsCheDo)
			{
				return;
			}
			if (TLBB.MPPercent < 2)
			{
				_ = IsArletMP;
				IsArletMP = true;
				return;
			}
			IsArletMP = false;
			SkillModel skillByID = SkillData.GetSkillByID(id);
			if (skillByID == null || skillByID.isPassive || Skill.IsBand(id))
			{
				return;
			}
			switch (id)
			{
			case 447:
				if (TLBB.MPPercent <= 80)
				{
					UseSkill(id);
				}
				return;
			case 407:
			case 424:
				return;
			}
			if (!isAreadyBuff)
			{
				if (skillByID.skillTargetType == "Friend" && skillByID.skillType == "NeedPointToTarget")
				{
					if (skillByID.impact == null)
					{
						return;
					}
					{
						foreach (GameObject item in Objects.Party)
						{
							if (!item.IsPet && !item.Buff.Contains(skillByID.impact.id))
							{
								UseSkill(id, item.Id);
								isAreadyBuff = true;
								break;
							}
						}
						return;
					}
				}
				if (skillByID.skillTargetType == "Friend" && skillByID.skillType == "GlobalSkillFromTargetPoint")
				{
					if (skillByID.impact == null)
					{
						return;
					}
					{
						foreach (GameObject item2 in Objects.Party)
						{
							if (!item2.IsPet && !item2.Buff.Contains(skillByID.impact.id))
							{
								UseSkill(id, item2.Id);
								isAreadyBuff = true;
								break;
							}
						}
						return;
					}
				}
				if (skillByID.skillTargetType == "NoTarget" && skillByID.skillType == "SelfBuffNoTarget")
				{
					if (skillByID.impact != null && Objects.Self != null && !Objects.Self.Buff.Contains(skillByID.impact.id))
					{
						UseSkill(id, Objects.Self.Id);
						isAreadyBuff = true;
					}
					return;
				}
				if (((skillByID.skillTargetType == "Friend" && skillByID.skillType == "GlobalSkillFromSelf") || (skillByID.skillTargetType == "Friend" && skillByID.skillType == "SelfBuffNoTarget")) && skillByID.impact != null && Objects.Self != null && !Objects.Self.Buff.Contains(skillByID.impact.id))
				{
					UseSkill(id, Objects.Self.Id);
					isAreadyBuff = true;
					return;
				}
			}
			if (skillByID.skillTargetType == "Enemy" && skillByID.skillType == "NeedPointToTarget" && BestTarget != null && BestTarget.HP > 0f && TINHKIEM.GetDistance(CharX, CharY, BestTarget.X, BestTarget.Y) <= (float)skillByID.useRange)
			{
				UseSkill(id, BestTarget.Id);
				isAreadyBuff = false;
			}
			else if (skillByID.skillTargetType == "Enemy" && skillByID.skillType == "GlobalSkillFromSelf" && skillByID.impact != null && Objects.Self != null && !Objects.Self.Buff.Contains(skillByID.impact.id))
			{
				UseSkill(id, Objects.Self.Id);
				isAreadyBuff = false;
			}
			else if (skillByID.skillTargetType == "Enemy" && skillByID.skillType == "SelfBuffNoTarget" && skillByID.impact != null && Objects.Self != null && !Objects.Self.Buff.Contains(skillByID.impact.id))
			{
				UseSkill(id, Objects.Self.Id);
				isAreadyBuff = false;
			}
			else
			{
				isAreadyBuff = false;
			}
		}

		public int DelayOffset(int id)
		{
			foreach (Skill skill in Skills)
			{
				if (skill.PacketId == id)
				{
					return skill.DelayOffset;
				}
			}
			return 0;
		}

		public int SkillId(int key)
		{
			if (key < 20)
			{
				key += 112;
			}
			else if (key < 48)
			{
				key += 28;
			}
			int num = Memory.Read(Address.KeySkillIdBase);
			if (key >= 112)
			{
				key = Memory.Read(num + (key - 112) * 24 + 4);
			}
			else
			{
				int num2 = key - 28 - 20 - 1;
				if (num2 < 0)
				{
					num2 = 9;
				}
				key = Memory.Read(num + num2 * 24 + 720 + 4);
			}
			return key;
		}

		public void SendKey(int key)
		{
			if (key < 20)
			{
				key += 112;
			}
			else if (key < 48)
			{
				key += 28;
			}
			PostMessage(key, 101);
		}

		public void BuffPet()
		{
			if (TickCount % 9 == 0 && !(TLBB.Name == "ĐăngNhập") && IsPet && TLBB.PetHPPercent != 0 && !TLBB.IsFollow && !IsRide)
			{
				if (TLBB.PetHPPercent <= 50 || (TLBB.PetHPPercent <= 85 && TLBB.PetMaxHP - TLBB.PetHP >= 10000))
				{
					PostMessage(1, 105);
				}
				if (TickCount % 66 == 0 && TLBB.PetEnjoy <= 81 && TLBB.PetEnjoy > 0 && HaveItem("PetBauble_4"))
				{
					PostMessage(2, 105);
				}
			}
		}

		public void Buff()
		{
			if (TickCount % 66 != 0 || TLBB.Name == "ĐăngNhập" || TLBB.IsFollow || IsRide || TLBB.HPPercent == 0)
			{
				return;
			}
			if (IsHP && TLBB.HPPercent <= Global.BuffHPPercent)
			{
				foreach (PacketItem item in PacketItem.Enum(this))
				{
					if (item.Type == "Icons03_1" || item.Type == "Medicine1_3" || item.Type == "Medicine1_13" || item.Type == "Cloth2_4" || item.Type == "Cloth2_5")
					{
						PlayerPackageUseItem(item.Index);
					}
				}
				if (TLBB.SkillPetType.Contains("PetSkill1_11"))
				{
					UseSkillPet(686);
				}
			}
			if (IsMP && TLBB.MPPercent <= Global.BuffMPPercent)
			{
				foreach (PacketItem item2 in PacketItem.Enum(this))
				{
					if (item2.Type == "Medicine1_1" || item2.Type == "Medicine1_12" || item2.Type == "Cloth2_6" || item2.Type == "Cloth2_7")
					{
						PlayerPackageUseItem(item2.Index);
					}
				}
				if (TLBB.SkillPetType.Contains("PetSkill1_9"))
				{
					UseSkillPet(696);
				}
			}
			if (TLBB.SkillPetType.Contains("PetSkill1_10") && HuyetTe && TLBB.MPPercent <= HuyetTeValue)
			{
				UseSkillPet(697);
			}
			if (TLBB.SkillPetType.Contains("PetSkill1_12") && CongSinh && TLBB.HPPercent <= CongSinhValue)
			{
				UseSkillPet(687);
			}
		}

		public void UpRide()
		{
			if (TLBB.IsFollow)
			{
				StopFollow();
			}
			if (TLBB.IsRide || !TLBB.HaveRide || TLBB.PlayerState == 5)
			{
				return;
			}
			if (TLBB.PlayerState != 0 && TLBB.PlayerState != 2)
			{
				Jump();
				return;
			}
			foreach (Skill skill in Skills)
			{
				if (skill.PacketId == 21)
				{
					if (Memory.Read(TLBB.DelayBase + skill.DelayOffset) <= 0)
					{
						UseSkill(skill.PacketId);
					}
					break;
				}
			}
		}

		public void HuyBienThan()
		{
			LuaDoOneLineString("local buff_num = Player:GetBuffNumber(); local i = 0; while i < buff_num do szToolTips = Player:GetBuffIconNameByIndex(i); if string.find(szToolTips, 'Buff') then Player:DispelBuffByIndex(i); end i = i+1 end");
		}

		public void DownRide()
		{
			if (TLBB.IsRide)
			{
				LuaDoOneLineString("local buff_num = Player:GetBuffNumber(); local i = 0; while i < buff_num do szToolTips = Player:GetBuffIconNameByIndex(i); if string.find(szToolTips, 'Ride') or string.find(szToolTips, 'ThaiIcons') then Player:DispelBuffByIndex(i); return; end i = i+1 end");
			}
		}

		public void PlayerPackageUseItem(int index)
		{
			LuaDoOneLineString("PlayerPackage:UseItem(" + index + ");");
		}

		public void LuaDoOneLineString(string lua)
		{
			if (AddressOneLineEx[curLine] == 0)
			{
				AddressOneLineEx[curLine] = Memory.VirtualAllocEx(10240);
			}
			if (AddressOneLineEx[curLine] != 0)
			{
				Memory.WriteUnicodeString(lua + "--", AddressOneLineEx[curLine]);
				PostMessage(AddressOneLineEx[curLine], 104);
			}
			if (curLine < 19)
			{
				curLine++;
			}
			else
			{
				curLine = 0;
			}
		}

		public void LuaDoOneineVCISIILString(string lua)
		{
			if (AddressOneLine == 0)
			{
				AddressOneLine = Memory.VirtualAllocEx(102400);
				return;
			}
			Memory.WriteString(lua + "--", AddressOneLine);
			PostMessage(AddressOneLine, 104);
		}

		public void LuaDoString(string lua)
		{
			int wParam = Memory.WriteString(lua);
			PostMessage(wParam, 104);
		}

		public void LuaDoUnicodeString(string lua)
		{
			int wParam = Memory.WriteUnicodeString(lua);
			PostMessage(wParam, 104);
		}

		public void UseSkillPet(int id, float x, float y)
		{
			PostMessage(Memory.Float2Int(x), 50);
			PostMessage(Memory.Float2Int(y), 51);
			PostMessage(-1, 53);
			PostMessage(id, 103);
		}

		public void KinhCong()
		{
			SendKey(Keys.F6);
			PushDebugMessage("KHINH CONG");
			PostMessage(190, 50);
			PostMessage(202, 51);
			PostMessage(1, 100);
		}

		public void UseSkillPet(int id)
		{
			PostMessage(Memory.Float2Int(CharX), 50);
			PostMessage(Memory.Float2Int(CharY), 51);
			PostMessage(-1, 53);
			PostMessage(id, 103);
		}

		public void UseSkill(int skillId, int targetId, float x, float y)
		{
			PostMessage(Memory.Float2Int(x), 50);
			PostMessage(Memory.Float2Int(y), 51);
			PostMessage(targetId, 53);
			PostMessage(skillId, 102);
		}

		public void UseSkill(int skillId, int targetId)
		{
			UseSkill(skillId, targetId, -1f, -1f);
		}

		public void UseSkill(int skillId)
		{
			UseSkill(skillId, -1, -1f, -1f);
		}

		public Keys GetNMKey()
		{
			return Keys.F13;
		}

		public void SendKey(Keys key)
		{
			SendKey((int)key);
		}

		public void SetDll()
		{
			if (Define == 0)
			{
				Define = Def++;
			}
			PostMessage((int)FrmMain.Instance.Handle, -5);
			PostMessage(Address.GameType, 0);
			PostMessage(Address.ParaSelectTarget, 1);
			PostMessage(Address.ParaSendKey[0], 2);
			PostMessage(Address.ParaSendKey[1], 3);
			PostMessage(Address.FuncSendKey, 4);
			PostMessage(Address.ParaUseSkill[0], 5);
			PostMessage(Address.ParaUseSkill[1], 6);
			PostMessage(Address.ParaUseSkill[2], 7);
			PostMessage(Address.FuncUseSkill, 8);
			PostMessage(Address.ParaUseSkillPet, 9);
			PostMessage(Address.FuncUseSkillPet, 10);
			PostMessage(Address.ParaLuaDoString, 11);
			PostMessage(Address.FuncLuaDoString, 12);
			PostMessage(Address.ParaPickItem, 13);
			PostMessage(Address.ParaCollectItem, 14);
			PostMessage(Address.DropBase[0], 15);
			PostMessage(Address.DropBase[1], 16);
			PostMessage(Address.DropBase[2], 17);
			PostMessage(Address.DropBase[3], 18);
			PostMessage(Address.ParaLuaToString, 19);
			PostMessage(FuncLuaToString, 20);
			PostMessage(Address.ParaTalk, 21);
			PostMessage(Address.FuncUpLvl, 22);
			PostMessage(Address.FuncSelectTargetOfTarget, 23);
			PostMessage(Address.ParaSendPacket, 24);
			PostMessage(Address.FuncSendPacket, 25);
			AddressToString = Memory.VirtualAllocEx(20248);
			AddressTenBang = Memory.VirtualAllocEx(20248);
			PostMessage(Define, -6);
			PostMessage(Address.CharState[0], 26);
			int num = Memory.Scan("56 8BF1 8B0D ???????? 57 8B78");
			num = Memory.Scan("55 8BEC", num - 32, num, 0);
			PostMessage(num, 27);
			PostMessage(Address.CharBase[0], 28);
			try
			{
				if (Address.GameType == 1)
				{
					return;
				}
				Process processById = Process.GetProcessById(ProcessId);
				int num2 = -1;
				int num3 = -1;
				for (int i = 0; i < processById.Modules.Count; i++)
				{
					if (processById.Modules[i].ModuleName.ToLower() == "misahelp.dll")
					{
						num2 = (int)processById.Modules[i].BaseAddress;
						num3 = processById.Modules[i].ModuleMemorySize;
					}
				}
				if (num2 != -1)
				{
					for (int j = 0; j < processById.Threads.Count; j++)
					{
						int num4 = (int)GetThreadStartAddress(processById.Threads[j].Id);
						if (num4 > num2 && num4 < num2 + num3)
						{
							SuspendThread((int)OpenThread(ThreadAccess.SuspendResume, bInheritHandle: false, (uint)processById.Threads[j].Id));
						}
					}
				}
				int num5 = -1;
				int num6 = -1;
				for (int k = 0; k < processById.Modules.Count; k++)
				{
					if (processById.Modules[k].ModuleName.ToLower() == "celisttl.dll")
					{
						num5 = (int)processById.Modules[k].BaseAddress;
						num6 = processById.Modules[k].ModuleMemorySize;
					}
				}
				if (num5 == -1)
				{
					return;
				}
				for (int l = 0; l < processById.Threads.Count; l++)
				{
					int num7 = (int)GetThreadStartAddress(processById.Threads[l].Id);
					if (num7 > num5 && num7 < num5 + num6)
					{
						SuspendThread((int)OpenThread(ThreadAccess.SuspendResume, bInheritHandle: false, (uint)processById.Threads[l].Id));
					}
				}
			}
			catch
			{
			}
		}

		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern uint GetProcAddress(IntPtr hModule, string procName);

		private uint GetRemoteProcAddress(Process targetProcess, string moduleName, string functionName)
		{
			uint num = 0u;
			uint result = 0u;
			foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
			{
				if (module.ModuleName.ToLower() == moduleName)
				{
					uint procAddress = GetProcAddress(module.BaseAddress, functionName);
					if (procAddress != 0)
					{
						num = procAddress - (uint)(int)module.BaseAddress;
					}
					break;
				}
			}
			if (num != 0)
			{
				foreach (ProcessModule module2 in targetProcess.Modules)
				{
					if (module2.ModuleName.ToLower() == moduleName)
					{
						result = (uint)(int)module2.BaseAddress + num;
						break;
					}
				}
			}
			return result;
		}

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

		private int ErasePEHeader(IntPtr hModule, string procName)
		{
			byte[] lpBuffer = new byte[4];
			byte[] lpBuffer2 = new byte[120];
			byte[] lpBuffer3 = new byte[264];
			int lpNumberOfBytesWritten = 0;
			IntPtr hProcess = OpenProcess(2035711, bInheritHandle: false, Process.GetProcessesByName(procName)[0].Id);
			IntPtr lpBaseAddress = new IntPtr(hModule.ToInt32() + 60);
			IntPtr lpNumberOfBytesRead = IntPtr.Zero;
			ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, 4, out lpNumberOfBytesRead);
			if (WriteProcessMemory(hProcess, hModule, lpBuffer2, 120u, out lpNumberOfBytesWritten) && WriteProcessMemory(hProcess, hModule, lpBuffer3, 256u, out var lpNumberOfBytesWritten2))
			{
				return lpNumberOfBytesWritten + lpNumberOfBytesWritten2;
			}
			return 0;
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

		private static IntPtr GetThreadStartAddress(int threadId)
		{
			IntPtr intPtr = OpenThread(ThreadAccess.QueryInformation, bInheritHandle: false, threadId);
			if (intPtr == IntPtr.Zero)
			{
				return IntPtr.Zero;
			}
			IntPtr intPtr2 = Marshal.AllocHGlobal(IntPtr.Size);
			try
			{
				if (NtQueryInformationThread(intPtr, ThreadInfoClass.ThreadQuerySetWin32StartAddress, intPtr2, IntPtr.Size, IntPtr.Zero) != 0)
				{
					return IntPtr.Zero;
				}
				return Marshal.ReadIntPtr(intPtr2);
			}
			finally
			{
				CloseHandle(intPtr);
				Marshal.FreeHGlobal(intPtr2);
			}
		}

		[DllImport("ntdll.dll", SetLastError = true)]
		private static extern int NtQueryInformationThread(IntPtr threadHandle, ThreadInfoClass threadInformationClass, IntPtr threadInformation, int threadInformationLength, IntPtr returnLengthPtr);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, int dwThreadId);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern int SuspendThread(int hThread);

		public void ResetTime()
		{
			if ((Global.AutoResetTime || AutoResetTime) && Address.GameType == 1 && !(lastReset.Elapsed.TotalMinutes < 2.0) && (TLBB.PlayerState != 10 || TLBB.OnlineTime >= 300) && TLBB.OnlineTime > 177)
			{
				if (TLBB.PlayerState == 10)
				{
					IsOpenShop = true;
				}
				lastReset = Stopwatch.StartNew();
				IsOpenPass2 = false;
				SafeTime = -10;
				if (TLBB.IsLeader)
				{
					IsLostLeader = true;
					TickCount = 1;
				}
				LUA.DataPoolReConnect();
			}
		}

		public void ReConnect()
		{
			if (Address.GameType == 1)
			{
				IsOpenPass2 = false;
				LUA.DataPoolReConnect();
			}
		}

		private void UpLvl()
		{
			if (Global.AutoUpLvl && TLBB.Exp > TLBB.MaxExp && TLBB.Lvl < Global.AutoUpLvlBelow && TLBB.Menpai != 0)
			{
				PostMessage(7, 105);
			}
		}

		public void RaBang(int map)
		{
			if (TLBB.MapId < 500 || (Global.Mapbang && IsQDua && !GoTo(100f, 158f)) || !GoTo(BANG.TrinhVoDanh))
			{
				return;
			}
			if (!TLBB.IsQuestOpen)
			{
				Talk(BANG.TrinhVoDanh.Id);
				return;
			}
			foreach (QuestFrame item in QuestFrame.Enum(this))
			{
				if (item.Name.Contains("#{BHCS_090226_10}"))
				{
					QuestFrameOptionClicked(item);
					return;
				}
				if (map == 0 && item.Name.Contains("Quay về Lạc Dương"))
				{
					QuestFrameOptionClicked(item);
					return;
				}
				if (map == 1 && item.Name.Contains("#{BHCS_090219_03}"))
				{
					QuestFrameOptionClicked(item);
					return;
				}
				if (map == 2 && item.Name.Contains("#{BHCS_090219_02}"))
				{
					QuestFrameOptionClicked(item);
					return;
				}
			}
			CloseQuest();
		}

		public bool HaveItem(string type)
		{
			foreach (PacketItem item in PacketItem.Enum(this))
			{
				if (item.Type == type)
				{
					return true;
				}
			}
			return false;
		}

		public void DoAction(GameControl control)
		{
			PostMessage(control.Object, 112);
		}

		public void DoSubAction(GameControl control)
		{
			PostMessage(control.Object, 124);
		}

		public bool DoActionPacket(string type)
		{
			List<PacketItem> list = PacketItem.Enum(this);
			bool flag = false;
			foreach (PacketItem item in list)
			{
				if (!(item.Type.Trim() == type))
				{
					continue;
				}
				foreach (GameControl control in Controls)
				{
					if (control.Type == item.Type && control.PacketId == item.PacketId)
					{
						DoAction(control);
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
				Controls = GameControl.Enum(this);
				foreach (GameControl control2 in Controls)
				{
					if (control2.Type == item.Type && control2.PacketId == item.PacketId)
					{
						DoAction(control2);
						flag = true;
						break;
					}
				}
			}
			return flag;
		}

		public bool DoSubActionPacket(string type)
		{
			List<PacketItem> list = PacketItem.Enum(this);
			bool flag = false;
			foreach (PacketItem item in list)
			{
				if (!(item.Type.Trim() == type))
				{
					continue;
				}
				foreach (GameControl control in Controls)
				{
					if (control.Type == item.Type && control.PacketId == item.PacketId)
					{
						DoSubAction(control);
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
				Controls = GameControl.Enum(this);
				foreach (GameControl control2 in Controls)
				{
					if (control2.Type == item.Type && control2.PacketId == item.PacketId)
					{
						DoSubAction(control2);
						flag = true;
						break;
					}
				}
			}
			return flag;
		}

		public void DoAction(string Type)
		{
			Controls = GameControl.Enum(this);
			foreach (GameControl control in Controls)
			{
				if (control.Type == Type)
				{
					DoAction(control);
					break;
				}
			}
		}

		public int PhuIndex(int map)
		{
			foreach (PacketItem item in PacketItem.Enum(this))
			{
				if ((TINHKIEM.VietLien(item.Name).Contains("dinhviphu") || TINHKIEM.VietLien(item.Name).Contains("tholinhchau") || TINHKIEM.VietLien(item.Name).Contains("lightdustcapsule")) && item.MapId == map && item.X > 0)
				{
					return item.Index;
				}
			}
			return -1;
		}

		public int PhuIndex()
		{
			foreach (PacketItem item in PacketItem.Enum(this))
			{
				if ((TINHKIEM.VietLien(item.Name).Contains("dinhviphu") || TINHKIEM.VietLien(item.Name).Contains("tholinhchau")) && item.X > 0 && (item.MapId == 0 || item.MapId == 1 || item.MapId == 2))
				{
					return item.Index;
				}
			}
			return -1;
		}

		public void CloseQuest()
		{
			PostMessage(0, 122);
		}

		public void QuestFrameAccept()
		{
			PostMessage(13, 105);
		}

		private void TogleMission()
		{
			PostMessage(18, 105);
		}

		public int[,] MapPOINT(int map)
		{
			string text = Setting.LoadMAP(map.ToString());
			text = "96,84-109,52-107,43-108,21-95,20-76,21-63,28-47,20-32,25-23,21-20,31-42,40-42,72-21,94";
			int num = 0;
			string[] array = text.Split('-');
			foreach (string text2 in array)
			{
				int num2 = 0;
				int num3 = 0;
				try
				{
					num2 = TINHKIEM.ParseInt(text2.Split(',')[0]);
					num3 = TINHKIEM.ParseInt(text2.Split(',')[1]);
				}
				catch
				{
				}
				if (num2 != 0 && num3 != 0)
				{
					num++;
				}
			}
			if (num > 1)
			{
				int[,] array2 = new int[num, 2];
				int num4 = 0;
				array = text.Split('-');
				foreach (string text3 in array)
				{
					int num5 = 0;
					int num6 = 0;
					try
					{
						num5 = TINHKIEM.ParseInt(text3.Split(',')[0]);
						num6 = TINHKIEM.ParseInt(text3.Split(',')[1]);
					}
					catch
					{
					}
					if (num5 != 0 && num6 != 0)
					{
						array2[num4, 0] = num5;
						array2[num4, 1] = num6;
						num4++;
					}
				}
				return array2;
			}
			return null;
		}

		public void MoveNext()
		{
			if (TLBB.PlayerState != 0)
			{
				return;
			}
			string text = Setting.LoadMAP(TLBB.MapId.ToString());
			if (TLBB.MapId == LACDUONG.Id || (TLBB.MapId == 242 && text.Trim() == ""))
			{
				text = "361,187-356,227-329,251-305,251-278,346-214,345-206,317-184,291-181,256-213,203";
			}
			if (TLBB.MapId == MAP.PhungHoangCoThanhPhuBan)
			{
				text = "72,109-110,112-108,66-108,23-63,24-21,19-15,25-18,73-65,73";
			}
			if (TLBB.MapId == MAP.ThienLongAcBa)
			{
				text = "95,115-75,108-114,106-96,90-70,54-42,48-86,36-109,36-153,46-146,57-122,65-96,38";
			}
			if (TLBB.MapId == MAP.ThieuLamAcBa)
			{
				text = "95,131-95,105-69,79-113,83-94,80-94,80";
			}
			if (TLBB.MapId == MAP.ThuyLao)
			{
				text = "71,41-107,40-140,39-150,71-148,106-150,139-122,150-85,147-49,149-43,120-41,84-41,50";
			}
			if (TLBB.MapId == MAP.PhungHoangCoThanh)
			{
				text = "228,158-228,77-181,92-172,124-116,135-81,162-90,228-162,200-162,231-210,242-230,229-172,151-231,158";
			}
			if (TLBB.MapId == MAP.TranLongKyCuoc)
			{
				text = "42,42-84,42-81,81-42,85-50,49-71,50";
			}
			int num = 0;
			string[] array = text.Split('-');
			foreach (string text2 in array)
			{
				int num2 = 0;
				int num3 = 0;
				try
				{
					num2 = TINHKIEM.ParseInt(text2.Split(',')[0]);
					num3 = TINHKIEM.ParseInt(text2.Split(',')[1]);
				}
				catch
				{
				}
				if (num2 != 0 && num3 != 0)
				{
					num++;
				}
			}
			if (num > 1)
			{
				int[,] array2 = new int[num, 2];
				int num4 = 0;
				array = text.Split('-');
				foreach (string text3 in array)
				{
					int num5 = 0;
					int num6 = 0;
					try
					{
						num5 = TINHKIEM.ParseInt(text3.Split(',')[0]);
						num6 = TINHKIEM.ParseInt(text3.Split(',')[1]);
					}
					catch
					{
					}
					if (num5 != 0 && num6 != 0)
					{
						array2[num4, 0] = num5;
						array2[num4, 1] = num6;
						num4++;
					}
				}
				MoveNext(array2);
				return;
			}
			if (TLBB.MapId == MAP.ThieuLam || TLBB.MapId == MAP.ThieuLamPhuBan)
			{
				MoveNext(POINT.ThieuLam);
			}
			if (TLBB.MapId == MAP.CaiBang || TLBB.MapId == MAP.CaiBangPhuBan)
			{
				MoveNext(POINT.CaiBang);
			}
			if (TLBB.MapId == MAP.MinhGiao || TLBB.MapId == MAP.MinhGiaoPhuBan)
			{
				MoveNext(POINT.MinhGiao);
			}
			if (TLBB.MapId == MAP.VoDang || TLBB.MapId == MAP.VoDangPhuBan)
			{
				MoveNext(POINT.VoDang);
			}
			if (TLBB.MapId == MAP.ThienLong || TLBB.MapId == MAP.ThienLongPhuBan)
			{
				MoveNext(POINT.ThienLong);
			}
			if (TLBB.MapId == MAP.TieuDao || TLBB.MapId == MAP.TieuDaoPhuBan)
			{
				MoveNext(POINT.TieuDao);
			}
			if (TLBB.MapId == MAP.NgaMy || TLBB.MapId == MAP.NgaMyPhuBan)
			{
				MoveNext(POINT.NgaMy);
			}
			if (TLBB.MapId == MAP.TinhTuc || TLBB.MapId == MAP.TinhTucPhuBan)
			{
				MoveNext(POINT.TinTuc);
			}
			if (TLBB.MapId == MAP.ThienSon || TLBB.MapId == MAP.ThienSonPhuBan)
			{
				MoveNext(POINT.ThienSon);
			}
			if (TLBB.MapId == MAP.MoDung)
			{
				MoveNext(POINT.MoDung);
			}
			if (TLBB.MapId == MAP.DuongMon)
			{
				MoveNext(POINT.DuongMon);
			}
			if (TLBB.MapId == MAP.ThaiHo)
			{
				MoveNext(POINT.ThaiHo);
			}
			if (TLBB.MapId == MAP.KiemCac)
			{
				MoveNext(POINT.KiemCac);
			}
			if (TLBB.MapId == MAP.VoLuongSon)
			{
				MoveNext(POINT.VoLuongSon);
			}
			if (TLBB.MapId == MAP.DonHoang)
			{
				MoveNext(POINT.DonHoang);
			}
			if (TLBB.MapId == MAP.TungSon)
			{
				MoveNext(POINT.TungSon);
			}
			if (TLBB.MapId == MAP.TayHo)
			{
				MoveNext(POINT.TayHo);
			}
			if (TLBB.MapId == MAP.NhiHai)
			{
				MoveNext(POINT.NhiHai);
			}
			if (TLBB.MapId == MAP.NhanNam)
			{
				MoveNext(POINT.NhanNam);
			}
			if (TLBB.MapId == MAP.LongTuyen)
			{
				MoveNext(POINT.LongTuyen);
			}
			if (TLBB.MapId == MAP.ThuongSon)
			{
				MoveNext(POINT.ThuongSon);
			}
			if (TLBB.MapId == MAP.NhanBac)
			{
				MoveNext(POINT.NhanBac);
			}
			if (TLBB.MapId == MAP.VoDi)
			{
				MoveNext(POINT.VoDi);
			}
			if (TLBB.MapId == MAP.ThachLam)
			{
				MoveNext(POINT.ThachLam);
			}
			if (TLBB.MapId == MAP.NganNgaiTuyetNguyen)
			{
				MoveNext(POINT.NganNgaiTuyetNguyen);
			}
			if (TLBB.MapId == MAP.ThaoNguyen)
			{
				MoveNext(POINT.ThaoNguyen);
			}
			if (TLBB.MapId == MAP.ThieuLamAcBa)
			{
				MoveNext(POINT.ThieuLamAcBa);
			}
			if (TLBB.MapId == MAP.NgaMyAcBa)
			{
				MoveNext(POINT.NgaMyAcBa);
			}
			if (TLBB.MapId == MAP.TieuDaoAcBa)
			{
				MoveNext(POINT.TieuDaoAcBa);
			}
			if (TLBB.MapId == MAP.DuongMonAcBa)
			{
				MoveNext(POINT.DuongMonAcBa);
			}
			if (TLBB.MapId == MAP.MinhGiaoAcBa)
			{
				MoveNext(POINT.MinhGiaoAcBa);
			}
			if (TLBB.MapId == MAP.VoDangAcBa)
			{
				MoveNext(POINT.VoDangAcBa);
			}
			if (TLBB.MapId == MAP.TinhTucAcBa)
			{
				MoveNext(POINT.TinhTucAcBa);
			}
			if (TLBB.MapId == MAP.ThienSonAcBa)
			{
				MoveNext(POINT.ThienSonAcBa);
			}
			if (TLBB.MapId == MAP.CaiBangAcBa)
			{
				MoveNext(POINT.CaiBangAcBa);
			}
			if (TLBB.MapId == MAP.ThienLongAcBa)
			{
				MoveNext(POINT.ThienLongAcBa);
			}
			if (TLBB.MapId == MAP.MoDungAcBa)
			{
				MoveNext(POINT.MoDungAcBa);
			}
			if (TLBB.MapId == MAP.TacKhauDoanhDia)
			{
				MoveNext(POINT.TacKhauDoanhDia);
			}
			if (TLBB.MapId == MAP.ThanhThuSon)
			{
				MoveNext(POINT.ThanhThuSon);
			}
			if (TLBB.MapId == MAP.LauLan)
			{
				MoveNext(POINT.LauLan);
			}
			if (TLBB.MapId == MAP.PhungHoangCoThanh)
			{
				MoveNext(POINT.PhungHoangCoThanh);
			}
			if (TLBB.MapId == MAP.PhungHoangCoThanhPhuBan)
			{
				MoveNext(POINT.PhungHoangCoThanhPhuBan);
			}
			if (IsNhatHopQDua && TLBB.MapId >= 500)
			{
				MoveNext(POINT.BangHoiDua);
			}
		}

		public void MoveNext(int[,] point)
		{
			if (MoveIndex == -1)
			{
				if (TLBB.MapId == MAP.PhungHoangCoThanh || IsMapPhuBan() || TLBB.MapId == MAP.ThuyLao)
				{
					MoveIndex = 0;
				}
				else
				{
					float num = 9999f;
					for (int i = 0; i < point.GetLength(0); i++)
					{
						float distance = TINHKIEM.GetDistance(CharX, CharY, point[i, 0], point[i, 0]);
						if (distance < num)
						{
							num = distance;
							MoveIndex = i;
						}
					}
				}
			}
			if (MoveIndex > point.GetLength(0) - 1)
			{
				if (IsThuyLao)
				{
					IsXongThuyLao = true;
				}
				MoveIndex = 0;
				if (IsKyCuoc)
				{
					MoveIndex = 0;
				}
			}
			if (TINHKIEM.GetDistance(CharX, CharY, point[MoveIndex, 0], point[MoveIndex, 1]) <= 5f)
			{
				MoveIndex++;
			}
			if (MoveIndex > point.GetLength(0) - 1)
			{
				if (IsThuyLao)
				{
					IsXongThuyLao = true;
				}
				MoveIndex = 0;
			}
			Move(point[MoveIndex, 0], point[MoveIndex, 1]);
		}

		public void Next(int[,] point)
		{
			if (MoveIndex == -1)
			{
				float num = 9999f;
				for (int i = 0; i < point.GetLength(0); i++)
				{
					if (TINHKIEM.GetDistance(CharX, CharY, point[i, 0], point[i, 0]) < num)
					{
						MoveIndex = i;
					}
				}
			}
			if (MoveIndex > point.GetLength(0) - 1)
			{
				MoveIndex = 0;
			}
			if (TINHKIEM.GetDistance(CharX, CharY, point[MoveIndex, 0], point[MoveIndex, 1]) <= 3f)
			{
				MoveIndex++;
			}
			if (MoveIndex > point.GetLength(0) - 1)
			{
				MoveIndex = 0;
			}
		}

		public void Next()
		{
			string text = Setting.LoadMAP(TLBB.MapId.ToString());
			int num = 0;
			string[] array = text.Split('-');
			foreach (string text2 in array)
			{
				int num2 = 0;
				int num3 = 0;
				try
				{
					num2 = TINHKIEM.ParseInt(text2.Split(',')[0]);
					num3 = TINHKIEM.ParseInt(text2.Split(',')[1]);
				}
				catch
				{
				}
				if (num2 != 0 && num3 != 0)
				{
					num++;
				}
			}
			if (num > 1)
			{
				int[,] array2 = new int[num, 2];
				int num4 = 0;
				array = text.Split('-');
				foreach (string text3 in array)
				{
					int num5 = 0;
					int num6 = 0;
					try
					{
						num5 = TINHKIEM.ParseInt(text3.Split(',')[0]);
						num6 = TINHKIEM.ParseInt(text3.Split(',')[1]);
					}
					catch
					{
					}
					if (num5 != 0 && num6 != 0)
					{
						array2[num4, 0] = num5;
						array2[num4, 1] = num6;
						num4++;
					}
				}
				Next(array2);
				return;
			}
			if (TLBB.MapId == MAP.ThieuLam || TLBB.MapId == MAP.ThieuLamPhuBan)
			{
				Next(POINT.ThieuLam);
			}
			if (TLBB.MapId == MAP.CaiBang || TLBB.MapId == MAP.CaiBangPhuBan)
			{
				Next(POINT.CaiBang);
			}
			if (TLBB.MapId == MAP.MinhGiao || TLBB.MapId == MAP.MinhGiaoPhuBan)
			{
				Next(POINT.MinhGiao);
			}
			if (TLBB.MapId == MAP.VoDang || TLBB.MapId == MAP.VoDangPhuBan)
			{
				Next(POINT.VoDang);
			}
			if (TLBB.MapId == MAP.ThienLong || TLBB.MapId == MAP.ThienLongPhuBan)
			{
				Next(POINT.ThienLong);
			}
			if (TLBB.MapId == MAP.TieuDao || TLBB.MapId == MAP.TieuDaoPhuBan)
			{
				Next(POINT.TieuDao);
			}
			if (TLBB.MapId == MAP.NgaMy || TLBB.MapId == MAP.NgaMyPhuBan)
			{
				Next(POINT.NgaMy);
			}
			if (TLBB.MapId == MAP.TinhTuc || TLBB.MapId == MAP.TinhTucPhuBan)
			{
				Next(POINT.TinTuc);
			}
			if (TLBB.MapId == MAP.ThienSon || TLBB.MapId == MAP.ThienSonPhuBan)
			{
				Next(POINT.ThienSon);
			}
			if (TLBB.MapId == MAP.MoDung)
			{
				Next(POINT.MoDung);
			}
			if (TLBB.MapId == MAP.DuongMon)
			{
				Next(POINT.DuongMon);
			}
			if (TLBB.MapId == MAP.ThaiHo)
			{
				Next(POINT.ThaiHo);
			}
			if (TLBB.MapId == MAP.KiemCac)
			{
				Next(POINT.KiemCac);
			}
			if (TLBB.MapId == MAP.VoLuongSon)
			{
				Next(POINT.VoLuongSon);
			}
			if (TLBB.MapId == MAP.DonHoang)
			{
				Next(POINT.DonHoang);
			}
			if (TLBB.MapId == MAP.TungSon)
			{
				Next(POINT.TungSon);
			}
			if (TLBB.MapId == MAP.TayHo)
			{
				Next(POINT.TayHo);
			}
			if (TLBB.MapId == MAP.NhiHai)
			{
				Next(POINT.NhiHai);
			}
			if (TLBB.MapId == MAP.NhanNam)
			{
				Next(POINT.NhanNam);
			}
			if (TLBB.MapId == MAP.LongTuyen)
			{
				Next(POINT.LongTuyen);
			}
			if (TLBB.MapId == MAP.ThuongSon)
			{
				Next(POINT.ThuongSon);
			}
			if (TLBB.MapId == MAP.NhanBac)
			{
				Next(POINT.NhanBac);
			}
			if (TLBB.MapId == MAP.VoDi)
			{
				Next(POINT.VoDi);
			}
			if (TLBB.MapId == MAP.ThachLam)
			{
				Next(POINT.ThachLam);
			}
			if (TLBB.MapId == MAP.NganNgaiTuyetNguyen)
			{
				Next(POINT.NganNgaiTuyetNguyen);
			}
			if (TLBB.MapId == MAP.ThaoNguyen)
			{
				Next(POINT.ThaoNguyen);
			}
			if (TLBB.MapId == MAP.ThieuLamAcBa)
			{
				Next(POINT.ThieuLamAcBa);
			}
			if (TLBB.MapId == MAP.NgaMyAcBa)
			{
				Next(POINT.NgaMyAcBa);
			}
			if (TLBB.MapId == MAP.TieuDaoAcBa)
			{
				Next(POINT.TieuDaoAcBa);
			}
			if (TLBB.MapId == MAP.DuongMonAcBa)
			{
				Next(POINT.DuongMonAcBa);
			}
			if (TLBB.MapId == MAP.MinhGiaoAcBa)
			{
				Next(POINT.MinhGiaoAcBa);
			}
			if (TLBB.MapId == MAP.VoDangAcBa)
			{
				Next(POINT.VoDangAcBa);
			}
			if (TLBB.MapId == MAP.TinhTucAcBa)
			{
				Next(POINT.TinhTucAcBa);
			}
			if (TLBB.MapId == MAP.ThienSonAcBa)
			{
				Next(POINT.ThienSonAcBa);
			}
			if (TLBB.MapId == MAP.CaiBangAcBa)
			{
				Next(POINT.CaiBangAcBa);
			}
			if (TLBB.MapId == MAP.ThienLongAcBa)
			{
				Next(POINT.ThienLongAcBa);
			}
			if (TLBB.MapId == MAP.MoDungAcBa)
			{
				Next(POINT.MoDungAcBa);
			}
			if (TLBB.MapId == MAP.TacKhauDoanhDia)
			{
				Next(POINT.TacKhauDoanhDia);
			}
			if (TLBB.MapId == MAP.ThanhThuSon)
			{
				Next(POINT.ThanhThuSon);
			}
			if (TLBB.MapId == MAP.LauLan)
			{
				Next(POINT.LauLan);
			}
			if (TLBB.MapId == MAP.PhungHoangCoThanh)
			{
				Next(POINT.PhungHoangCoThanh);
			}
			if (TLBB.MapId == MAP.PhungHoangCoThanhPhuBan)
			{
				Next(POINT.PhungHoangCoThanhPhuBan);
			}
		}

		public void SetTeam(string team)
		{
			if (team == null || team == "")
			{
				LuaDoUnicodeString("TEAM = nil;");
			}
			else
			{
				LuaDoUnicodeString("TEAM = \"" + team.Replace("\r\n", "") + "\";");
			}
		}

		public void SetTeamFromList(List<BuffPramenter> DanhSachBuff)
		{
			string text = "";
			if (DanhSachBuff.Count > 0)
			{
				BuffPramenter[] array = DanhSachBuff.ToArray();
				foreach (BuffPramenter buffPramenter in array)
				{
					text = text + buffPramenter.IDnguoichoi + " - " + buffPramenter.TenNguoiChoi;
				}
			}
			if (text == null || text == "")
			{
				LuaDoUnicodeString("TEAM = nil;");
			}
			else
			{
				LuaDoUnicodeString("TEAM = \"" + text.Replace("\r\n", "") + "\";");
			}
		}

		public void Accept()
		{
			PostMessage(29, 105);
		}

		public void AppointLeader(string name)
		{
			LuaDoUnicodeString("AppointLeader(\"" + name + "\")");
		}

		public void AcceptAll()
		{
			SetTeam(null);
			Accept();
			if (Global.AutoAccept && !Global.AcceptAll)
			{
				SetTeamFromList(Setting.BuffValue);
			}
		}

		public void FixKetMap()
		{
			if (KetMap == 0)
			{
				Move(CharX - 3f, CharY - 3f);
				KetMap++;
			}
			else if (KetMap == 1)
			{
				Move(CharX, CharY - 3f);
				KetMap++;
			}
			else if (KetMap == 2)
			{
				Move(CharX + 3f, CharY + 3f);
				KetMap++;
			}
			else if (KetMap == 3)
			{
				Move(CharX + 3f, CharY);
				KetMap++;
			}
			else if (KetMap == 4)
			{
				Move(CharX + 3f, CharY + 3f);
				KetMap++;
			}
			else if (KetMap == 5)
			{
				Move(CharX, CharY + 3f);
				KetMap++;
			}
			else if (KetMap == 6)
			{
				Move(CharX - 3f, CharY + 3f);
				KetMap++;
			}
			else if (KetMap == 7)
			{
				Move(CharX - 3f, CharY);
				KetMap = 0;
			}
		}

		public void Buy(int index)
		{
			int num = Memory.Read(Address.BaseShopItem);
			num = Memory.Read(num + index * 4);
			if (num != 0)
			{
				PostMessage(num, 113);
			}
		}

		public bool IsNPCSuMon(GameObject _object)
		{
			if (!TINHKIEM.VietLien(_object.Title).Contains("nguoigiaonhiemvu") && !TINHKIEM.VietLien(_object.Title).Contains("congbonhiemvu"))
			{
				if (!TINHKIEM.VietLien(_object.Title).Contains("nhiemvutuyendatsu"))
				{
					return TINHKIEM.VietLien(_object.Title).Contains("nhiemvutuyendotsu");
				}
				return true;
			}
			return true;
		}

		public void VaoPhai()
		{
			if (ON_SCENE_TRANSING || IsChangeMap)
			{
				return;
			}
			TLBB.Base = Memory.Read(Address.CharBase);
			if (Address.GameType == 1)
			{
				TLBB.Id = Memory.Read8Byte(TLBB.Base + Address.CharId).ToString("X8");
			}
			else
			{
				TLBB.Id = Memory.Read(TLBB.Base + Address.CharId).ToString("X8");
			}
			TLBB.Name = Memory.ReadString(TLBB.Base + Address.CharName);
			if (TLBB.Name == "")
			{
				TLBB.Name = "ĐăngNhập";
			}
			if (!TLBB.Online)
			{
				return;
			}
			if (Option.PutBase && TLBB.Menpai != 0 && !Skill.IsBase(SkillId(0)))
			{
				LuaDoOneLineString("MainmenuBar_JoinMenpai()");
			}
			if (TLBB.Menpai != 0)
			{
				return;
			}
			if (!isAlarmVaoPhai)
			{
				isAlarmVaoPhai = true;
				alarmVaoPhai = new AlarmVaoPhai(this);
			}
			if (!IsSetMenPai || TLBB.Lvl != 10)
			{
				return;
			}
			NPC nPC = new NPC();
			switch (SetMenPai)
			{
			case TINHKIEM.Menpai.ThieuLam:
				nPC = NPC.HuyenTich;
				break;
			case TINHKIEM.Menpai.MinhGiao:
				nPC = NPC.LaSuTuong;
				break;
			case TINHKIEM.Menpai.CaiBang:
				nPC = NPC.TranCoNhan;
				break;
			case TINHKIEM.Menpai.VoDang:
				nPC = NPC.TruongHuyenTo;
				break;
			case TINHKIEM.Menpai.NgaMy:
				nPC = NPC.LyThapNhiNuong;
				break;
			case TINHKIEM.Menpai.TinhTuc:
				nPC = NPC.HanTheTrung;
				break;
			case TINHKIEM.Menpai.ThienLong:
				nPC = NPC.BanNhan;
				break;
			case TINHKIEM.Menpai.ThienSon:
				nPC = NPC.MaiKiem;
				break;
			case TINHKIEM.Menpai.TieuDao:
				nPC = NPC.ToTinhHa;
				break;
			case TINHKIEM.Menpai.DuongMon:
				nPC = NPC.DuongXichPhong;
				break;
			case TINHKIEM.Menpai.MoDung:
				nPC = NPC.MoDungKiet;
				break;
			}
			if (nPC.Id == -1 || !GoTo(nPC))
			{
				return;
			}
			if (IsTalkToNpc == 0)
			{
				IsTalkToNpc = 1;
				Talk(nPC.Id);
				return;
			}
			if (QuestFrame.GetCount(this) == 3)
			{
				QuestFrameOptionClicked(QuestFrame.Enum(this)[1]);
				IsTalkToNpc = 0;
			}
			foreach (QuestFrame item in QuestFrame.Enum(this))
			{
				if (item.Name == "#GVào môn phái")
				{
					QuestFrameOptionClicked(item);
					return;
				}
			}
			IsTalkToNpc = 0;
		}

		public bool GanNPCSuMon()
		{
			if ((TLBB.Menpai != 1 || TLBB.MapId != 9 || !(TINHKIEM.GetDistance(CharX, CharY, 96f, 82f) <= 3f)) && (TLBB.Menpai != 2 || TLBB.MapId != 11 || !(TINHKIEM.GetDistance(CharX, CharY, 98f, 105f) <= 3f)) && (TLBB.Menpai != 3 || TLBB.MapId != 10 || !(TINHKIEM.GetDistance(CharX, CharY, 92f, 77f) <= 3f)) && (TLBB.Menpai != 4 || TLBB.MapId != 12 || !(TINHKIEM.GetDistance(CharX, CharY, 78f, 95f) <= 3f)) && (TLBB.Menpai != 5 || TLBB.MapId != 15 || !(TINHKIEM.GetDistance(CharX, CharY, 95f, 86f) <= 3f)) && (TLBB.Menpai != 6 || TLBB.MapId != 16 || !(TINHKIEM.GetDistance(CharX, CharY, 96f, 92f) <= 3f)) && (TLBB.Menpai != 8 || TLBB.MapId != 17 || !(TINHKIEM.GetDistance(CharX, CharY, 95f, 60f) <= 3f)) && (TLBB.Menpai != 9 || TLBB.MapId != 14 || !(TINHKIEM.GetDistance(CharX, CharY, 119f, 152f) <= 3f)) && (TLBB.Menpai != 32 || TLBB.MapId != 284 || !(TINHKIEM.GetDistance(CharX, CharY, 69f, 125f) <= 3f)) && (TLBB.Menpai != 37 || TLBB.MapId != 615 || !(TINHKIEM.GetDistance(CharX, CharY, 100f, 64f) <= 3f)))
			{
				if (TLBB.Menpai == MENPAI.ThienLong && TLBB.MapId == MAP.ThienLong)
				{
					return TINHKIEM.GetDistance(CharX, CharY, 95f, 88f) <= 3f;
				}
				return false;
			}
			return true;
		}

		public bool DenSuMon()
		{
			if (TLBB.Menpai == 1)
			{
				if (TLBB.MapId != 9)
				{
					GoTo(96f, 82f, 9);
					return false;
				}
				if (TINHKIEM.GetDistance(CharX, CharY, 96f, 82f) > 3f)
				{
					GoTo(96f, 82f);
					return false;
				}
			}
			if (TLBB.Menpai == 2)
			{
				if (TLBB.MapId != 11)
				{
					GoTo(98f, 105f, 11);
					return false;
				}
				if (TINHKIEM.GetDistance(CharX, CharY, 98f, 105f) > 3f)
				{
					GoTo(98f, 105f);
					return false;
				}
			}
			if (TLBB.Menpai == 3)
			{
				if (TLBB.MapId != 10)
				{
					GoTo(92f, 77f, 10);
					return false;
				}
				if (TINHKIEM.GetDistance(CharX, CharY, 92f, 77f) > 3f)
				{
					GoTo(92f, 77f);
					return false;
				}
			}
			if (TLBB.Menpai == 4)
			{
				if (TLBB.MapId != 12)
				{
					GoTo(78f, 95f, 12);
					return false;
				}
				if (TINHKIEM.GetDistance(CharX, CharY, 78f, 95f) > 3f)
				{
					GoTo(78f, 95f);
					return false;
				}
			}
			if (TLBB.Menpai == 5)
			{
				if (TLBB.MapId != 15)
				{
					GoTo(95f, 86f, 15);
					return false;
				}
				if (TINHKIEM.GetDistance(CharX, CharY, 95f, 86f) > 3f)
				{
					GoTo(95f, 86f);
					return false;
				}
			}
			if (TLBB.Menpai == 6)
			{
				if (TLBB.MapId != 16)
				{
					GoTo(96f, 92f, 16);
					return false;
				}
				if (TINHKIEM.GetDistance(CharX, CharY, 96f, 92f) > 3f)
				{
					GoTo(96f, 92f);
					return false;
				}
			}
			if (TLBB.Menpai == 8)
			{
				if (TLBB.MapId != 17)
				{
					GoTo(95f, 60f, 17);
					return false;
				}
				if (TINHKIEM.GetDistance(CharX, CharY, 95f, 60f) > 3f)
				{
					GoTo(95f, 60f);
					return false;
				}
			}
			if (TLBB.Menpai == 9)
			{
				if (TLBB.MapId != 14)
				{
					GoTo(119f, 152f, 14);
					return false;
				}
				if (TINHKIEM.GetDistance(CharX, CharY, 119f, 152f) > 3f)
				{
					GoTo(119f, 152f);
					return false;
				}
			}
			if (TLBB.Menpai == 32)
			{
				if (TLBB.MapId != 284)
				{
					GoTo(69f, 125f, 284);
					return false;
				}
				if (TINHKIEM.GetDistance(CharX, CharY, 69f, 125f) > 3f)
				{
					GoTo(69f, 125f);
					return false;
				}
			}
			if (TLBB.Menpai == 37)
			{
				if (TLBB.MapId != 615)
				{
					GoTo(100f, 64f, 615);
					return false;
				}
				if (TINHKIEM.GetDistance(CharX, CharY, 100f, 64f) > 3f)
				{
					GoTo(100f, 64f);
					return false;
				}
			}
			if (TLBB.Menpai == MENPAI.ThienLong)
			{
				if (TLBB.MapId != MAP.ThienLong)
				{
					GoTo(95f, 88f, MAP.ThienLong);
					return false;
				}
				if (TINHKIEM.GetDistance(CharX, CharY, 95f, 88f) > 3f)
				{
					GoTo(95f, 88f);
					return false;
				}
			}
			return true;
		}

		public void ClearNhiemVu()
		{
			TrangThaiLuyenKim = (TrangThaiSuMon = (TrangThaiTuBaoBon = (TrangThaiXayDung = (TrangThaiTuDuong = (TrangThaiQD = "")))));
			IsLuyenKim = (IsSuMon = (IsTuBaoBon = (IsTuDuong = (IsXayDung = (IsTrungAc = (IsNguyenVong = false))))));
			IsLPMH = false;
			IsNhanh = false;
			State = STATE.None;
			if (IsTrungAc)
			{
				State = STATE.Null;
			}
			IsDauCo = false;
			DaNhanHoaHong = false;
			DaNhanHoaChung = false;
			IsKhoang = (IsDuoc = false);
			IsTrongTrot = (IsThuHoach = false);
			IsVanMay = (IsLyHoa = (IsNguHanhPhap = false));
			OkNhanDa = false;
			IsTueHong = false;
			IsChucPhuc = false;
			IsNhatHop = false;
			BachHoaDuyenCompleted = false;
			IsCauOThuoc = false;
			IsDead = false;
			IsCheDo = false;
			MapAcTac = 0;
			LuaDoOneLineString("COUNT = nil;");
			if (!IsNotClear)
			{
				IsNhanQuaBuiHoaHong = false;
				IsNhanHoaHongLo = false;
				NhanQuaHoaHongCompleted = false;
				IsNhatHopall = false;
				IsChayVong = false;
				IsNhatHopQDua = false;
				IsQDua = false;
				QDuaCompleted = false;
				IsMoBang = false;
			}
		}

		public bool IsCollect()
		{
			if (!IsNhatHopQDua && !IsOptLocDo && !IsQDua && !IsNhatHopall && !IsLuyenKim && !IsPhuMau && !IsSuMon && !IsTuBaoBon && !IsTuDuong && !IsXayDung && !IsTrungAc && !IsBachHoaDuyen && !IsNguyenVong && !IsThuHoach && !IsKhoang && !IsDuoc && !IsNhatTuyet && !IsNhatHop && !IsNhiemVuCoBan)
			{
				if (!IsPickItem && !IsPickEx)
				{
					return Global.PickItem;
				}
				return true;
			}
			return true;
		}

		public bool OSuMon()
		{
			if (TLBB.Menpai == MENPAI.ThieuLam && TLBB.MapId == MAP.ThieuLam)
			{
				return true;
			}
			if (TLBB.Menpai == MENPAI.MinhGiao && TLBB.MapId == MAP.MinhGiao)
			{
				return true;
			}
			if (TLBB.Menpai == MENPAI.CaiBang && TLBB.MapId == MAP.CaiBang)
			{
				return true;
			}
			if (TLBB.Menpai == MENPAI.VoDang && TLBB.MapId == MAP.VoDang)
			{
				return true;
			}
			if (TLBB.Menpai == MENPAI.NgaMy && TLBB.MapId == MAP.NgaMy)
			{
				return true;
			}
			if (TLBB.Menpai == MENPAI.TinhTuc && TLBB.MapId == MAP.TinhTuc)
			{
				return true;
			}
			if (TLBB.Menpai == MENPAI.ThienLong && TLBB.MapId == MAP.ThienLong)
			{
				return true;
			}
			if (TLBB.Menpai == MENPAI.ThienSon && TLBB.MapId == MAP.ThienSon)
			{
				return true;
			}
			if (TLBB.Menpai == MENPAI.TieuDao && TLBB.MapId == MAP.TieuDao)
			{
				return true;
			}
			if (TLBB.Menpai == MENPAI.MoDung && TLBB.MapId == MAP.MoDung)
			{
				return true;
			}
			if (TLBB.Menpai == MENPAI.DuongMon && TLBB.MapId == MAP.DuongMon)
			{
				return true;
			}
			DenSuMon();
			return false;
		}

		public void GoHuyenVuDao()
		{
			if (User.TienXu >= 100000)
			{
				PostMessage(25, 105);
			}
		}

		public void Ride()
		{
			if (!(TimeStand.Elapsed.TotalSeconds < 1.0))
			{
				UseSkill(21);
				StandTime = 0;
			}
		}

		public void AOE()
		{
			if (TickCount % 150 == 0 && Global.UseSkillPet && Objects.NearMonter20m.Count >= 0 && SkillPetId(TLBB.SkillPetType) != -1)
			{
				UseSkillPet(SkillPetId(TLBB.SkillPetType), Objects.Monter[0].X, Objects.Monter[0].Y);
			}
		}

		public int SkillPetId(string name)
		{
			if (name.Contains("MenpaiLiveSkill2_14"))
			{
				return 742;
			}
			if (name.Contains("MenpaiLiveSkill2_13"))
			{
				return 743;
			}
			if (name.Contains("MenpaiLiveSkill2_16"))
			{
				return 744;
			}
			if (name.Contains("MenpaiLiveSkill2_15"))
			{
				return 745;
			}
			if (name.Contains("PetSkill4_13"))
			{
				return 676;
			}
			if (name.Contains("PetSkill4_14"))
			{
				return 677;
			}
			if (name.Contains("PetSkill7_7"))
			{
				return 672;
			}
			if (name.Contains("PetSkill1_8"))
			{
				return 673;
			}
			if (name.Contains("PetSkill7_8"))
			{
				return 674;
			}
			if (name.Contains("PetSkill3_4"))
			{
				return 675;
			}
			if (name.Contains("PetSkill4_15"))
			{
				return 747;
			}
			if (name.Contains("PetSkill7_1"))
			{
				return 694;
			}
			if (name.Contains("PetSkill7_2"))
			{
				return 695;
			}
			return -1;
		}

		public void Talk(NPC npc)
		{
			PostMessage(npc.Id, 110);
		}

		public void Talk(string name)
		{
			foreach (GameObject item in Objects.All)
			{
				if (item.CleanName.Contains(TINHKIEM.VietLien(name)))
				{
					Talk(item.Id);
					break;
				}
			}
		}

		public void Talk(int id)
		{
			PostMessage(id, 110);
		}

		public void QuestFrameOptionClicked(QuestFrame dialog)
		{
			QuestFrameOptionClicked(dialog.StrOptionExtra1, dialog.StrOptionExtra2);
		}

		public void QuestFrameOptionClicked(int StrOptionExtra1, int StrOptionExtra2)
		{
			PostMessage(StrOptionExtra1, 54);
			PostMessage(StrOptionExtra2, 55);
			PostMessage(12, 105);
		}

		public bool IsDropEx(PacketItem item)
		{
			if (item.Lvl != 0)
			{
				if (item.Star >= 1 && item.Star <= 1)
				{
					return TrangBi.Contains(item.TypeName);
				}
				return false;
			}
			return false;
		}

		public void DropItem()
		{
			if (!IsOpenPass2 || TLBB.SafeTime > 0)
			{
				return;
			}
			if (IsBank && TickCount % 18 == 0 && Address.GameType == 2 && !Global.Paused)
			{
				if (TLBB.MapId != LACDUONG.Id)
				{
					TimDuong(275f, 295f, 0);
					return;
				}
				if (TINHKIEM.GetDistance(CharX, CharY, LACDUONG.ThuongKho.X, LACDUONG.ThuongKho.Y) > 3f)
				{
					GoTo(LACDUONG.ThuongKho);
					return;
				}
				if (!TLBB.IsBankOpen)
				{
					Talk(LACDUONG.ThuongKho);
					QuestFrameOptionClicked(7, -1);
				}
				if (TLBB.IsBankOpen)
				{
					_ = new Bank(this).DaoCu.Count;
					foreach (PacketItem item in PacketItem.Enum(this))
					{
						if (!(item.Name == "") && !(item.TypeName == "") && !(item.Type == ""))
						{
							DoSubActionPacket(item.Type);
							PushDebugMessage("Auto vừa cất " + item.Name + " vào rương");
						}
					}
				}
			}
			if (TickCount % 18 == 0 && Global.IsVutRac)
			{
				if (!IsOpenBag)
				{
					IsOpenBag = true;
					OpenBag();
				}
				foreach (PacketItem item2 in PacketItem.Enum(this))
				{
					if (IsCanDelete(item2) && !(item2.Name == "") && !(item2.TypeName == "") && !(item2.Type == "") && (IsDropEx(item2) || JunkItemName.Contains(item2.Name) || JunkItemType.Contains(item2.TypeName)) && !item2.IsHaveLongVan && !item2.IsHaveNgoc)
					{
						PostMessage(item2.Index, 108);
						PushDebugMessage("Auto vừa hủy " + item2.Name + " [" + item2.TypeName + "] (Rác)");
						FrmMain.AddLog(DateTime.Now.ToString("dd-MM | HH:mm") + " [" + LastName + "] hủy vật phẩm :" + item2.Name + " (Rác) \n");
						return;
					}
				}
			}
			if (TickCount % 60 == 0 && IsNhiemVuCoBan)
			{
				if (!IsNhanBinhMau && TLBB.Lvl >= 20 && TickCount % 300 == 0)
				{
					LuaDoOneLineString("setmetatable(_G, {__index = QiankunBag_Env}); QiankunBag_Clicked(1);");
				}
				foreach (PacketItem item3 in PacketItem.Enum(this))
				{
					if (!(item3.Name == "") && !(item3.TypeName == "") && !(item3.Type == ""))
					{
						if (item3.ClearName.Contains("ngandanholo"))
						{
							IsNhanBinhMau = true;
						}
						if (item3.Name.Contains("Thú Cưỡi") && (item3.Name.Contains("Hoa Hồng Đen") || item3.Name.Contains("Phúc Thụy Tuyết Điêu")))
						{
							DoActionPacket(item3.Type);
						}
						if (item3.Name.Contains("Vô Ưu"))
						{
							DoActionPacket(item3.Type);
						}
						if (IsDropEx(item3) || JunkItemName.Contains(item3.Name) || JunkItemType.Contains(item3.TypeName))
						{
							PostMessage(item3.Index, 108);
						}
					}
				}
			}
			if (TickCount % 60 != 0 || SafeTime <= 30 || !IsDropItem)
			{
				return;
			}
			foreach (PacketItem item4 in PacketItem.Enum(this))
			{
				if (IsCanDelete(item4) && IsDrop(item4) && !item4.IsHaveLongVan && !item4.IsHaveNgoc)
				{
					PostMessage(item4.Index, 108);
					FrmMain.AddLog(DateTime.Now.ToString("dd-MM | HH:mm") + " [" + LastName + "] hủy vật phẩm :" + item4.Name + "\n");
					PushDebugMessage("Auto vừa hủy vật phẩm :" + item4.Name);
				}
				if (TINHKIEM.VietLien(item4.Name) == "tuikimngocphuquy" || TINHKIEM.VietLien(item4.Name) == "tuitieuphuc" || TINHKIEM.VietLien(item4.Name) == "tuidaiphuc" || TINHKIEM.VietLien(item4.Name) == "tieulucdan" || TINHKIEM.VietLien(item4.Name) == "dialucdan" || TINHKIEM.VietLien(item4.Name) == "thienlucdan")
				{
					LuaDoOneLineString("PlayerPackage:UseItem(" + item4.Index + ");");
					break;
				}
				if (TINHKIEM.VietLien(item4.Name) == "channguyenphach" && item4.IsCoDinh)
				{
					LuaDoOneLineString("PlayerPackage:UseItem(" + item4.Index + ");");
					break;
				}
			}
		}

		public bool IsCanDelete(PacketItem Item)
		{
			bool result = true;
			if (Array.IndexOf(new string[18]
			{
				"Mão", "Y phục", "Hộ thủ", "Hài", "Yêu đái", "Giới chỉ", "Hạng liên", "Võ Hồn", "Hộ phù", "Hộ uyển",
				"Hộ kiên", "Ám Khí", "Đao búa", "Thương tần", "Đơn đoản", "Song đoản", "Phiến", "Khuyên"
			}, Item.TypeName) > -1)
			{
				result = ((!(Item.DiemType == "0000000000000000000000000000000000000000000000000000000000000000")) ? true : false);
			}
			return result;
		}

		public void PickObject(GameObject _object)
		{
			PostMessage(_object.Object, 118);
		}

		public void ThienKiepLau()
		{
			if (IsThienKiepLau && !TINHKIEM.VietLien(TLBB.MapName).Contains("thienkieplau") && GoTo(DAILY.PhoKiepSinh))
			{
				_ = TLBB.IsQuestOpen;
			}
		}

		public bool SellItem()
		{
			if (!IsSellItem)
			{
				return false;
			}
			if (TickCount % 18 != 0)
			{
				return false;
			}
			NPC vANDIEUDIEU = NPC.VANDIEUDIEU;
			vANDIEUDIEU = ((!Unity.DangOMapVutRac(TLBB.MapId)) ? Unity.GETNPCVUTRAC(Option.MapBanDoIndex) : Unity.GETNPCVUTRAC(TLBB.MapId));
			if (GoTo(vANDIEUDIEU.X, vANDIEUDIEU.Y, vANDIEUDIEU.Map))
			{
				if (!TLBB.IsShopOpen)
				{
					if (TLBB.IsQuestOpen)
					{
						if (vANDIEUDIEU.Map == 0)
						{
							QuestFrame.Click(101, 0);
						}
						if (vANDIEUDIEU.Map == 2)
						{
							QuestFrame.Click(2048, 11);
						}
					}
					foreach (GameObject item in Objects.AllNpc)
					{
						if (item.CleanName == "vandieudieu" || item.CleanName == "tontuvu" || item.CleanName == "truongthienthien" || item.CleanName == "donghoakim")
						{
							Talk(item.Id);
						}
					}
				}
				else
				{
					if (TLBB.SafeTime > 0)
					{
						return false;
					}
					if (TickCount % 9 == 0)
					{
						if (IsSale)
						{
							IsSale = false;
							foreach (PacketItem item2 in PacketItem.Enum(this))
							{
								if (IsSell(item2))
								{
									IsSale = true;
									break;
								}
							}
						}
						if (TLBB.IsShopOpen)
						{
							foreach (PacketItem item3 in PacketItem.Enum(this))
							{
								if (IsCanDelete(item3) && IsSell(item3) && !item3.IsHaveLongVan && !item3.IsHaveNgoc)
								{
									SellItem(item3.Address);
									FrmMain.AddLog(DateTime.Now.ToString("dd-MM | HH:mm") + " [" + LastName + "] bán vật phẩm :" + item3.Name + "\n");
									PushDebugMessage("Auto vừa bán bán vật phẩm :" + item3.Name);
									Thread.Sleep(150);
								}
							}
							IsSellItem = false;
							PushThongBao(TLBB.Name, "Đã bán xong vật phẩm\nKiểm tra danh sách tại Logs", CanhBao.Kieu.Info);
							return false;
						}
						if ((TLBB.IsODaoCuFull || TLBB.IsONguyenLieuFull) && IsNhatTuyet)
						{
							IsSale = true;
						}
						if (IsSale)
						{
							if (SaveX == 0f && SaveY == 0f)
							{
								SaveX = CharX;
								SaveY = CharY;
								IsGoToShop = true;
							}
							return true;
						}
					}
				}
			}
			return IsSale;
		}

		public void DropItem(int index)
		{
			PostMessage(index, 108);
		}

		public static bool IsDrop(PacketItem packetItem)
		{
			if (packetItem.Name.Trim() == "" || packetItem.TypeName == "")
			{
				return false;
			}
			string[] array = Setting.DropName.Split('\n');
			for (int i = 0; i < array.Length; i++)
			{
				if (TINHKIEM.VietLien(array[i]) == TINHKIEM.VietLien(packetItem.Name) && packetItem.Name.Trim() != "")
				{
					return true;
				}
			}
			array = Setting.DropType.Split('\n');
			for (int j = 0; j < array.Length; j++)
			{
				if (TINHKIEM.VietLien(array[j]) == TINHKIEM.VietLien(packetItem.TypeName) && packetItem.TypeName.Trim() != "")
				{
					return true;
				}
			}
			return false;
		}

		public bool IsSell(PacketItem packetItem)
		{
			if (packetItem.Name.Trim() == "" || packetItem.TypeName == "")
			{
				return false;
			}
			string[] array = Setting.SellName.Split('\n');
			for (int i = 0; i < array.Length; i++)
			{
				if (TINHKIEM.VietLien(array[i]) == TINHKIEM.VietLien(packetItem.Name) && packetItem.Name.Trim() != "")
				{
					return true;
				}
			}
			array = Setting.SellType.Split('\n');
			for (int j = 0; j < array.Length; j++)
			{
				if (TINHKIEM.VietLien(array[j]) == TINHKIEM.VietLien(packetItem.TypeName) && packetItem.TypeName.Trim() != "")
				{
					return true;
				}
			}
			return false;
		}

		public void MoiThemDoi()
		{
			Objects.Read();
			if (Objects.Self != null && Objects.Self.PartyId != -1)
			{
				return;
			}
			foreach (GameObject item in Objects.All)
			{
				if (FrmMain.AllCurGameTrueID.Contains(item.TrueId) && item.TrueId != TLBB.Id && item.PartyId != -1)
				{
					SelectTarget(item.Id);
					PostMessage(11, 105);
					return;
				}
			}
			foreach (GameObject item2 in Objects.All)
			{
				if (item2.PartyId != -1 && item2.PartyId != 0 && item2.Menpai >= 0 && item2.Menpai <= 9 && item2.Id != 0)
				{
					SelectTarget(item2.Id);
					PostMessage(11, 105);
					break;
				}
			}
		}

		public void LoadSkill()
		{
			if (TimeOnMap < 40 || !TLBB.Online || Skills.Count != 0)
			{
				return;
			}
			Skills = Skill.Enum(this);
			if (Skills.Count > 4)
			{
				string text = Setting.LoadStringOffline(TLBB.Id + "SKILL");
				string text2 = Setting.LoadStringOffline(TLBB.Id + "SKILLPK");
				string text3 = Setting.LoadStringOffline(TLBB.Id + "SKILLBUFF");
				NMSKill = null;
				BaseSkill = 0;
				Controls = GameControl.Enum(this);
				foreach (Skill skill in Skills)
				{
					if (Skill.IsBase(skill.PacketId))
					{
						BaseSkill = skill.PacketId;
					}
					if (skill.PacketId == 424)
					{
						NMSKill = skill;
					}
					foreach (GameControl control in Controls)
					{
						if (control.IsSkill && control.PacketId == skill.PacketId)
						{
							skill.Name = control.Name;
							break;
						}
					}
					if (text.Contains("-" + skill.PacketId + "-"))
					{
						skill.Use = true;
					}
					if (text2.Contains("-" + skill.PacketId + "-"))
					{
						skill.UsePK = true;
					}
					if (text3.Contains("-" + skill.PacketId + "-"))
					{
						skill.UserBuff = true;
					}
				}
				if (NMSKill == null)
				{
					foreach (Skill skill2 in Skills)
					{
						if (skill2.PacketId == 407)
						{
							NMSKill = skill2;
						}
					}
				}
				if (this.SkillLoaded != null)
				{
					this.SkillLoaded(this, null);
				}
			}
			else
			{
				Skills.Clear();
			}
		}

		private void FollowKey()
		{
			if ((!IsMapPhuBan() || !Global.IsAcTac) && TLBB.PlayerState != 2 && !TLBB.IsFollow && Global.FollowKey && Objects.Key != null && TINHKIEM.GetDistance(CharX, CharY, Objects.Key.X, Objects.Key.Y) >= (float)Global.FollowRadius)
			{
				Move(Objects.Key.X, Objects.Key.Y);
			}
		}

		public void UnlockPass2()
		{
			LuaDoOneLineString("UnLockMinorPassword(\"" + Pass2 + "\");");
		}

		public void SavePass2()
		{
			Setting.SaveSettingOffline("PASS2" + TLBB.Id, Pass2);
		}

		public void SetWay(string way)
		{
			LuaDoOneLineString("WAY = \"" + way + "\"; SetWay()");
		}

		public string ReadRecvData()
		{
			int num = Memory.Read(RecvData);
			if (num == 0)
			{
				return "";
			}
			string text = "";
			bool flag = false;
			byte[] array = new byte[100];
			Memory.ReadProcessMemory(Memory.Id, num, array, 100, 0);
			for (int i = 0; i < 100; i++)
			{
				string text2 = array[i].ToString("X2");
				if (text2 != "00")
				{
					flag = true;
				}
				text += text2;
			}
			if (flag)
			{
				RecvDat = text;
			}
			return text;
		}

		public string GetTrieuTap(string hex)
		{
			string text = "";
			if (!hex.Contains("746965756461747461692067") || !hex.StartsWith("DA03"))
			{
				return "";
			}
			string input = Regex.Replace(hex, ".*D569205B", "");
			input = Regex.Replace(input, "5D.*", "");
			for (int i = 0; i < input.Length / 2; i++)
			{
				try
				{
					text += (char)short.Parse(input.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier);
				}
				catch
				{
					text += ".";
				}
			}
			return text;
		}

		public void LoadSetting()
		{
			try
			{
				RaoTxt = Setting.LoadStringOffline("RAO" + TLBB.Id);
			}
			catch (Exception)
			{
			}
			ClearNhiemVu();
			LastId = TLBB.Id;
			LastName = TLBB.Name;
			BachHoaDuyenCompleted = false;
			TudongAn = JsonConvert.DeserializeObject<List<AutoEat>>(Setting.AutoEat);
			PetId = Setting.LoadStringOffline(TLBB.Id + "PET");
			if (TLBB.Online)
			{
				Init();
				if (!FrmMain.AllCurGameTrueID.Contains(TLBB.Id))
				{
					FrmMain.AllCurGameTrueID = FrmMain.AllCurGameTrueID + TLBB.Id + ",";
				}
			}
			try
			{
				ResetExpSpeed();
				KetQuaSetTitle = Win.SetWindowText(Handle, TINHKIEM.ClearSign(TLBB.Name) + " - game4you.us");
				int[] array = Setting.LoadSettingOffline(TLBB.Id);
				if (array == null || array.Length < 53)
				{
					IsAuto = true;
					for (int i = 0; i < 22; i++)
					{
						KeyDelay[i] = 1;
					}
					IsAttack = (IsPet = (IsHP = (IsMP = true)));
					this.SettingLoaded?.Invoke(this, null);
					return;
				}
				IsAuto = array[0] == 1;
				IsAttack = array[1] == 1;
				IsLure = array[2] == 1;
				for (int j = 0; j < 12; j++)
				{
					F[j] = array[j + 3] == 1;
				}
				IsPet = array[15] == 1;
				IsHP = array[16] == 1;
				IsMP = array[17] == 1;
				IsRadius = array[18] == 1;
				IsNM = array[19] == 1;
				for (int k = 0; k < 10; k++)
				{
					Alt[k] = array[20 + k] == 1;
				}
				for (int l = 0; l < 22; l++)
				{
					KeyDelay[l] = array[30 + l];
				}
				BuffPetPercent = array[52];
				if (array.Length > 53)
				{
					IsPickItem = array[53] == 1;
				}
				if (array.Length > 59)
				{
					CheLoai = array[54];
					CheCap = array[55];
					CheNoiNgoai = array[56];
					CheSao = array[57];
					CheDong = array[58];
					CheDiem = array[59];
				}
				if (array.Length > 60)
				{
					HuyetTe = array[60] == 1;
					HuyetTeValue = array[61];
					CongSinh = array[62] == 1;
					CongSinhValue = array[63];
					AutoEatVatPham = array[64] == 1;
					AutoThuPet = array[65] == 1;
					TuAnX2 = array[66] == 1;
					UsingTholinhChau = array[67] == 1;
					SoLuongMua = array[68];
					SoLuongChe = array[69];
				}
				if (array.Length > 70)
				{
					ChatGan = array[70] == 1;
					ChatTheGioi = array[71] == 1;
					ChatThanhThi = array[72] == 1;
					ChatDongMinh = array[73] == 1;
					ChatMonPhai = array[74] == 1;
					ChatBangPhai = array[75] == 1;
					ChatDoi = array[76] == 1;
					AutoHoiSinh = array[77] == 1;
				}
				this.SettingLoaded?.Invoke(this, null);
			}
			catch (Exception ex2)
			{
				Console.Write(ex2.ToString());
			}
		}

		public void SaveSetting()
		{
			Setting.SaveSettingOffline("RAO" + TLBB.Id, RaoTxt);
			string text = TINHKIEM.Bool2Int(IsAuto) + "," + TINHKIEM.Bool2Int(IsAttack) + "," + TINHKIEM.Bool2Int(IsLure) + ",";
			for (int i = 0; i < 12; i++)
			{
				text = text + TINHKIEM.Bool2Int(F[i]) + ",";
			}
			text = text + TINHKIEM.Bool2Int(IsPet) + "," + TINHKIEM.Bool2Int(IsHP) + "," + TINHKIEM.Bool2Int(IsMP) + "," + TINHKIEM.Bool2Int(IsRadius) + "," + TINHKIEM.Bool2Int(IsNM) + ",";
			for (int j = 0; j < 10; j++)
			{
				text = text + TINHKIEM.Bool2Int(Alt[j]) + ",";
			}
			for (int k = 0; k < 22; k++)
			{
				text = text + KeyDelay[k] + ",";
			}
			text = text + BuffPetPercent + "," + TINHKIEM.Bool2Int(IsPickItem) + ",";
			text = text + CheLoai + "," + CheCap + "," + CheNoiNgoai + "," + CheSao + "," + CheDong + "," + CheDiem + "," + TINHKIEM.Bool2Int(HuyetTe) + "," + HuyetTeValue + "," + TINHKIEM.Bool2Int(CongSinh) + "," + CongSinhValue + "," + TINHKIEM.Bool2Int(AutoEatVatPham) + "," + TINHKIEM.Bool2Int(AutoThuPet) + "," + TINHKIEM.Bool2Int(TuAnX2) + "," + TINHKIEM.Bool2Int(UsingTholinhChau) + "," + SoLuongMua + "," + SoLuongChe + "," + TINHKIEM.Bool2Int(ChatGan) + "," + TINHKIEM.Bool2Int(ChatTheGioi) + "," + TINHKIEM.Bool2Int(ChatThanhThi) + "," + TINHKIEM.Bool2Int(ChatDongMinh) + "," + TINHKIEM.Bool2Int(ChatMonPhai) + "," + TINHKIEM.Bool2Int(ChatBangPhai) + "," + TINHKIEM.Bool2Int(ChatDoi) + "," + TINHKIEM.Bool2Int(AutoHoiSinh) + "," + TINHKIEM.Bool2Int(AlarmChat);
			Setting.SaveSettingOffline(TLBB.Id, text);
		}

		public void ResetExpSpeed()
		{
			if (TLBB.Lvl >= 1 && TLBB.Lvl <= 149)
			{
				AutoTime = Stopwatch.StartNew();
				ExpStart = TLBB.Exp;
			}
		}

		public void ResetetRadius()
		{
			if (IsRide || TLBB.IsFollow || !IsAuto || Global.Paused || !IsRadius || RadiusX == 0f)
			{
				RadiusX = CharX;
				RadiusY = CharY;
			}
		}

		public string GetEnemy()
		{
			if (AddressEnemy == 0)
			{
				AddressEnemy = Memory.VirtualAllocEx(4096);
			}
			PostMessage(AddressEnemy, 109);
			return Memory.ReadString(Memory.Read(AddressEnemy));
		}

		public string LuaToString()
		{
			PostMessage(AddressToString, 109);
			return Memory.ReadString(Memory.Read(AddressToString));
		}

		public string LuaToStringBang()
		{
			PostMessage(AddressTenBang, 109);
			return Memory.ReadString(Memory.Read(AddressTenBang));
		}

		public string DoCount()
		{
			if (AddressCount == 0)
			{
				AddressCount = Memory.VirtualAllocEx(4096);
			}
			PostMessage(AddressCount, 109);
			return Memory.ReadString(Memory.Read(AddressCount));
		}

		public string GetCount()
		{
			return Memory.ReadString(Memory.Read(AddressCount));
		}

		public string LuaString()
		{
			return Memory.ReadString(Memory.Read(AddressToString));
		}

		public string LuaStringBang()
		{
			return Memory.ReadString(Memory.Read(AddressTenBang));
		}

		public string LuaStringEx()
		{
			return Memory.ReadStringEx(Memory.Read(AddressToString));
		}

		public void EnterReconnect()
		{
			PostMessage(24, 105);
		}

		public void Pause()
		{
			if (IsHide)
			{
				PROCESS.Suspend(ProcessId);
				IsSuspend = true;
			}
		}

		public void Resume()
		{
			if (IsSuspend)
			{
				IsSuspend = false;
				PROCESS.Resume(ProcessId);
			}
		}

		public void TKC()
		{
		}

		public void PushAlarm(string msg)
		{
		}

		public void TheoDoiCanhBao()
		{
			VaoPhai();
			if (TLBB.MapId == MAP.HuyetMo && !isAlarmHuyetMo && IsMoBTD)
			{
				string text = "Đã vào huyệt mộ";
				LuaDoUnicodeString("TXT = '" + text + "#r#b#eda0000 A u t o Chính Thức Tình Kiếm#r#b#eda0000  h t tp ://game4you.u s ';");
				PostMessage(3, 105);
				isAlarmHuyetMo = true;
				PushAlarm("đã vào huyệt mộ");
			}
			if (ExitHPLow && TLBB.HPPercent < Global.ExitHPPercent && TLBB.HP > 0 && TLBB.MaxHP > 0 && TLBB.HP < TLBB.MaxHP && TLBB.Name != "ĐăngNhập")
			{
				Exit();
				return;
			}
			if (TLBB.IsPk && TLBB.MapId != 92)
			{
				if (Global.ExitPk)
				{
					Exit();
					return;
				}
				if (!isAlarmPK && Global.AlarmPk)
				{
					isAlarmPK = true;
					PushThongBao(TLBB.Name, "Đang bị PK", CanhBao.Kieu.Eror);
				}
			}
			else if (isAlarmPK)
			{
				isAlarmPK = false;
			}
			if (IsBachHoaDuyen)
			{
				if (TLBB.IsODaoCuFull)
				{
					if (!isAlarmDayTayNai)
					{
						isAlarmDayTayNai = true;
						PushThongBao(TLBB.Name, "Đầy Tay Nải", CanhBao.Kieu.Eror);
					}
				}
				else
				{
					isAlarmDayTayNai = false;
				}
			}
			if (!IsAlarmAcBa && AcBa != -1)
			{
				PushThongBao(TLBB.Name, "Ác Bá", CanhBao.Kieu.Eror);
				IsAlarmAcBa = true;
			}
			if (MoveCount > 40)
			{
				if (!isAlarmKet && MapAcTac == 0)
				{
					PushThongBao(TLBB.Name, "Bị Kẹt", CanhBao.Kieu.Eror);
					isAlarmKet = true;
				}
			}
			else if (isAlarmKet)
			{
				isAlarmKet = false;
			}
			if (BachHoaDuyenCompleted)
			{
				if (!isAlarmBachHoaDuyen)
				{
					FrmMain.AddLog(DateTime.Now.ToString("HH:mm dd-MM") + " " + LastName + " xong BHD\n");
					PushThongBao(TLBB.Name, "Xong BHD", CanhBao.Kieu.Info);
					isAlarmBachHoaDuyen = true;
				}
			}
			else
			{
				isAlarmBachHoaDuyen = false;
			}
			if (QDuaCompleted)
			{
				if (!isAlarmDua)
				{
					FrmMain.AddLog(DateTime.Now.ToString("HH:mm dd-MM") + " " + LastName + " xong Dua\n");
					PushThongBao(TLBB.Name, "Xong Dua", CanhBao.Kieu.Info);
					isAlarmDua = true;
				}
			}
			else
			{
				isAlarmDua = false;
			}
			if (IsXongTrungAc)
			{
				if (!isAlarmTrungAc)
				{
					FrmMain.AddLog(DateTime.Now.ToString("HH:mm dd-MM") + " " + LastName + " xong Trừng Ác\n");
					PushThongBao(TLBB.Name, "Xong Trừng Ác", CanhBao.Kieu.Info);
					isAlarmTrungAc = true;
				}
			}
			else
			{
				isAlarmTrungAc = false;
			}
			if (comeTime != null && comeTime.Elapsed.TotalSeconds > 150.0)
			{
				IsHong = true;
			}
			if (IsHong && HongTrungAcTime == null)
			{
				HongTrungAcTime = Stopwatch.StartNew();
			}
			if (!IsHong)
			{
				HongTrungAcTime = null;
			}
			if (IsHong && HongTrungAcTime.Elapsed.TotalSeconds > 300.0)
			{
				LuaDoUnicodeString("local cnt = 0; while true do local name, content = DataPool:GetPlayerMission_Memo(cnt); if string.find(name, '#{CXDT_090304_01}') then DataPool:Mission_Abnegate_Popup(cnt,DataPool:GetPlayerMission_Memo(cnt)); end cnt = cnt + 1; if cnt == 20 then return end end");
				State = STATE.HuyQ;
				HongTrungAcTime = null;
				IsHong = false;
				comeTime = null;
				return;
			}
			if (TLBB.Disconnected)
			{
				PushThongBao(TLBB.Name, "Mất Kết Nối", CanhBao.Kieu.Eror);
				EnterReconnect();
			}
			else if (disconnectedTime != 0)
			{
				disconnectedTime = 0;
			}
			if (TLBB.IsCaptcha)
			{
				CaptchaTime = 0;
				if (!isAlarmCaptcha && !TLBB.IsRead)
				{
				}
			}
			else
			{
				swCaptchaTime = null;
				IsShowCap = false;
				TLBB.Captcha = null;
				TLBB.BinEx = null;
				if (isAlarmCaptcha)
				{
					if (IsTrungAc && Global.HideBHD)
					{
						Hide();
					}
					isAlarmCaptcha = false;
				}
			}
			if (TLBB.PlayerState == 9)
			{
				if (Address.GameType == 1 && TLBB.IsRelive)
				{
					LUA.Relive();
				}
				else if (IsDeadEx)
				{
					if (!isAlarmDead)
					{
						if (Global.AutoComeBack && !MAP.IsPhuBan(TLBB.MapId))
						{
							DeadX = (int)CharX;
							DeadY = (int)CharY;
							DeadMap = TLBB.MapId;
							DeadFakeMap = TLBB.FakeMapId;
							IsDead = true;
						}
						else
						{
							DeadX = (DeadY = 0);
						}
						isAlarmDead = true;
					}
					LUA.OutGhost();
				}
				else if (!isAlarmDead)
				{
					if (Global.AutoComeBack && !MAP.IsPhuBan(TLBB.MapId))
					{
						DeadX = (int)CharX;
						DeadY = (int)CharY;
						DeadMap = TLBB.MapId;
						DeadFakeMap = TLBB.FakeMapId;
						IsDead = true;
					}
					else
					{
						DeadX = (DeadY = 0);
					}
					PostMessage(0, 90);
					isAlarmDead = true;
					if (!Global.AutoComeBack)
					{
						PushThongBao(TLBB.Name, "Đã Tử Vong", CanhBao.Kieu.Eror);
					}
					else
					{
						PushThongBao(TLBB.Name, "Đã Tử Vong", CanhBao.Kieu.Eror);
					}
				}
			}
			else if (isAlarmDead)
			{
				isAlarmDead = false;
			}
			if (Global.AlarmHP && TLBB.HPPercent < Global.AlarmHPPercent && TLBB.Online && TLBB.HP > 0 && TLBB.MaxHP > 0 && TLBB.HP < TLBB.MaxHP)
			{
				if (!isAlarmHP)
				{
					isAlarmHP = true;
					PushThongBao(TLBB.Name, "Sắp Hết Máu", CanhBao.Kieu.Eror);
				}
			}
			else if (isAlarmHP)
			{
				isAlarmHP = false;
			}
		}

		public void OpenShop()
		{
			LuaDoOneLineString("setmetatable(_G, {__index = Packet_Env }); Packet_Sale_Clicked(); IsMessageBox = 1;");
		}

		public void DragTo42()
		{
			LuaDoOneLineString("setmetatable(_G, {__index = Packet_Env }); Packet_ItemBtnClicked(4,2); ");
		}

		public void OpenBag()
		{
			IsOpenBag = true;
		}

		public VatLieu getsoluong(string loai, int cap)
		{
			VatLieu vatLieu = new VatLieu();
			string str = "";
			switch (loai)
			{
			case "Đao búa":
				str = "Đao Phủ Đả Tạo Đồ";
				break;
			case "Thương tần":
				str = "Thương Bổng Đả Tạo Đồ";
				break;
			case "Đơn đoản":
				str = "Đơn Đoản đả tạo đồ";
				break;
			case "Song đoản":
				str = "Song đoản đả tạo đồ";
				break;
			case "Phiến":
				str = "Phiến đả tạo độ";
				break;
			case "Khuyên":
				str = "Hoàn đả tạo độ";
				break;
			case "Mão":
				str = "Mão tử đả tạo độ";
				break;
			case "Y phục":
				str = "Y phục đả tạo độ";
				break;
			case "Hộ thủ":
				str = "Hộ thủ đả tạo độ";
				break;
			case "Hài":
				str = "Hài đả tạo độ";
				break;
			case "Hộ uyển":
				str = "Hộ uyển đả tạo độ";
				break;
			case "Hộ kiên":
				str = "Hộ kiên đả tạo độ";
				break;
			case "Yêu đái":
				str = "Yêu đái đả tạo độ";
				break;
			case "Hạng liên":
				str = "Hạng liên đả tạo độ";
				break;
			case "Giới chỉ":
				str = "Giới chỉ đả tạo độ";
				break;
			case "Hộ phù":
				str = "Hộ phù đả tạo độ";
				break;
			}
			using (List<PacketItem>.Enumerator enumerator = PacketItem.Enum(this).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (TINHKIEM.VietLien(enumerator.Current.Name).Contains("tinhthiet"))
					{
						vatLieu.TinhThiet += enumerator.Current.Count;
					}
					if (TINHKIEM.VietLien(enumerator.Current.Name).Contains("vaibong"))
					{
						vatLieu.VaiBong += enumerator.Current.Count;
					}
					if (TINHKIEM.VietLien(enumerator.Current.Name).Contains("bingan"))
					{
						vatLieu.BiNgan += enumerator.Current.Count;
					}
					if (TINHKIEM.VietLien(enumerator.Current.Name).Contains(TINHKIEM.VietLien(str)) && enumerator.Current.GetNumberFromString == cap)
					{
						vatLieu.DaTaoDo += enumerator.Current.Count;
					}
				}
			}
			vatLieu.DaChe = DaChe;
			vatLieu.DaHuy = DaHuy;
			return vatLieu;
		}

		public void MuaNguyenLieu(int loai)
		{
			if (loai == 1)
			{
				if (TLBB.MaxONguyenLieu - 3 < 0)
				{
					PushThongBao("Thông báo", "Thiếu ô nhận nguyên liệu vui lòng Sắp Xếp", CanhBao.Kieu.Eror);
					IsCheDo = false;
				}
				else if (TLBB.MapId != 2)
				{
					TimDuong(157f, 169f, 2);
				}
				else if (TINHKIEM.GetDistance(CharX, CharY, 157f, 169f) > 1f)
				{
					Move(157f, 169f);
				}
				else if (IsTalkTieuPhong)
				{
					IsTalkTieuPhong = false;
					QuestFrameOptionClicked(2084, 1004);
				}
				else
				{
					IsTalkTieuPhong = true;
					Talk(143);
				}
				return;
			}
			if (!TLBB.IsToggleYuanbaoShop)
			{
				LUA.ToggleYuanbaoShop();
				return;
			}
			string text = HaveDTD();
			if (text != "")
			{
				MuaName = text;
			}
			string text2 = TINHKIEM.VietLien(CheTen).Replace("khuyen", "hoan");
			if (text2.Contains("daobua") || text2.Contains("thuongtan"))
			{
				foreach (Shop item in Shop.Enum(this))
				{
					if (TINHKIEM.VietLien(item.Name).Contains(MuaName))
					{
						LUA.YuanbaoShop(shopIndex, 1);
						Buy(item.Index);
						MuaCount--;
						return;
					}
				}
				if (Is69DO)
				{
					LuaDoOneLineString("setmetatable(_G, {__index = YuanbaoShop_Env}); YuanbaoShop_ChangeTabIndex(1);");
					Thread.Sleep(1000);
					LuaDoOneLineString("setmetatable(_G, {__index = YuanbaoShop_Env}); YuanbaoShop_UpdateList_Bind(6); YuanbaoShop_UpdateShop_Bind(1);");
				}
				else
				{
					LUA.YuanbaoShop(shopIndex, 1);
				}
			}
			else if (text2.Contains("dondoan") || text2.Contains("songdoan"))
			{
				foreach (Shop item2 in Shop.Enum(this))
				{
					if (TINHKIEM.VietLien(item2.Name).Contains(MuaName))
					{
						LUA.YuanbaoShop(shopIndex, 2);
						Buy(item2.Index);
						MuaCount--;
						return;
					}
				}
				if (Is69DO)
				{
					LuaDoOneLineString("setmetatable(_G, {__index = YuanbaoShop_Env}); YuanbaoShop_ChangeTabIndex(1);");
					Thread.Sleep(1000);
					LuaDoOneLineString("setmetatable(_G, {__index = YuanbaoShop_Env}); YuanbaoShop_UpdateList_Bind(6); YuanbaoShop_UpdateShop_Bind(2);");
				}
				else
				{
					LUA.YuanbaoShop(shopIndex, 2);
				}
			}
			else if (text2.Contains("phien") || text2.Contains("hoan"))
			{
				foreach (Shop item3 in Shop.Enum(this))
				{
					if (TINHKIEM.VietLien(item3.Name).Contains(MuaName))
					{
						LUA.YuanbaoShop(shopIndex, 3);
						Buy(item3.Index);
						MuaCount--;
						return;
					}
				}
				if (Is69DO)
				{
					LuaDoOneLineString("setmetatable(_G, {__index = YuanbaoShop_Env}); YuanbaoShop_ChangeTabIndex(1);");
					Thread.Sleep(1000);
					LuaDoOneLineString("setmetatable(_G, {__index = YuanbaoShop_Env}); YuanbaoShop_UpdateList_Bind(6); YuanbaoShop_UpdateShop_Bind(3);");
				}
				else
				{
					LUA.YuanbaoShop(shopIndex, 3);
				}
			}
			else if (text2.Contains("mao") || text2.Contains("yphuc"))
			{
				foreach (Shop item4 in Shop.Enum(this))
				{
					if (TINHKIEM.VietLien(item4.Name).Contains(MuaName))
					{
						LUA.YuanbaoShop(shopIndex, 4);
						Buy(item4.Index);
						MuaCount--;
						return;
					}
				}
				if (Is69DO)
				{
					LuaDoOneLineString("setmetatable(_G, {__index = YuanbaoShop_Env}); YuanbaoShop_ChangeTabIndex(1);");
					Thread.Sleep(1000);
					LuaDoOneLineString("setmetatable(_G, {__index = YuanbaoShop_Env}); YuanbaoShop_UpdateList_Bind(6); YuanbaoShop_UpdateShop_Bind(4);");
				}
				else
				{
					LUA.YuanbaoShop(shopIndex, 4);
				}
			}
			else if (text2.Contains("hothu") || text2.Contains("hai"))
			{
				foreach (Shop item5 in Shop.Enum(this))
				{
					if (TINHKIEM.VietLien(item5.Name).Contains(MuaName))
					{
						LUA.YuanbaoShop(shopIndex, 5);
						Buy(item5.Index);
						MuaCount--;
						return;
					}
				}
				if (Is69DO)
				{
					LuaDoOneLineString("setmetatable(_G, {__index = YuanbaoShop_Env}); YuanbaoShop_ChangeTabIndex(1);");
					Thread.Sleep(1000);
					LuaDoOneLineString("setmetatable(_G, {__index = YuanbaoShop_Env}); YuanbaoShop_UpdateList_Bind(6); YuanbaoShop_UpdateShop_Bind(5);");
				}
				else
				{
					LUA.YuanbaoShop(shopIndex, 5);
				}
			}
			else if (text2.Contains("houyen") || text2.Contains("hokien"))
			{
				foreach (Shop item6 in Shop.Enum(this))
				{
					if (TINHKIEM.VietLien(item6.Name).Contains(MuaName))
					{
						LUA.YuanbaoShop(shopIndex, 6);
						Buy(item6.Index);
						MuaCount--;
						return;
					}
				}
				if (Is69DO)
				{
					LuaDoOneLineString("setmetatable(_G, {__index = YuanbaoShop_Env}); YuanbaoShop_ChangeTabIndex(1);");
					Thread.Sleep(1000);
					LuaDoOneLineString("setmetatable(_G, {__index = YuanbaoShop_Env}); YuanbaoShop_UpdateList_Bind(6); YuanbaoShop_UpdateShop_Bind(6);");
				}
				else
				{
					LUA.YuanbaoShop(shopIndex, 6);
				}
			}
			else if (text2.Contains("yeudai") || text2.Contains("hanglien"))
			{
				foreach (Shop item7 in Shop.Enum(this))
				{
					if (TINHKIEM.VietLien(item7.Name).Contains(MuaName))
					{
						LUA.YuanbaoShop(shopIndex, 7);
						Buy(item7.Index);
						MuaCount--;
						return;
					}
				}
				if (Is69DO)
				{
					LuaDoOneLineString("setmetatable(_G, {__index = YuanbaoShop_Env}); YuanbaoShop_ChangeTabIndex(1);");
					Thread.Sleep(1000);
					LuaDoOneLineString("setmetatable(_G, {__index = YuanbaoShop_Env}); YuanbaoShop_UpdateList_Bind(6); YuanbaoShop_UpdateShop_Bind(7);");
				}
				else
				{
					LUA.YuanbaoShop(shopIndex, 7);
				}
			}
			else
			{
				if (!text2.Contains("gioichi") && !text2.Contains("hophu"))
				{
					return;
				}
				foreach (Shop item8 in Shop.Enum(this))
				{
					if (TINHKIEM.VietLien(item8.Name).Contains(MuaName))
					{
						LUA.YuanbaoShop(shopIndex, 8);
						Buy(item8.Index);
						MuaCount--;
						return;
					}
				}
				if (Is69DO)
				{
					LuaDoOneLineString("setmetatable(_G, {__index = YuanbaoShop_Env}); YuanbaoShop_ChangeTabIndex(1);");
					Thread.Sleep(1000);
					LuaDoOneLineString("setmetatable(_G, {__index = YuanbaoShop_Env}); YuanbaoShop_UpdateList_Bind(6); YuanbaoShop_UpdateShop_Bind(8);");
				}
				else
				{
					LUA.YuanbaoShop(shopIndex, 8);
				}
			}
		}

		public void CheDoFree()
		{
			if (!IsCheDo)
			{
				return;
			}
			if (LenBaiTrain)
			{
				LenBaiTrain = false;
			}
			if (AutoTrain)
			{
				AutoTrain = false;
			}
			if (IsDead)
			{
				IsDead = false;
			}
			GiamDinh();
			if (TickCount % TocDoChe != 0)
			{
				return;
			}
			if (Kiemtranguyenlieu)
			{
				VatLieu vatLieu = new VatLieu();
				vatLieu = getsoluong(CheTen, CheCap + 1);
				if (!IsMuaNguyenLieu)
				{
					if ((CheTen == "Đao búa" || CheTen == "Thương tần" || CheTen == "Đơn đoản" || CheTen == "Song đoản" || CheTen == "Phiến" || CheTen == "Khuyên") && vatLieu.TinhThiet < SoLuongMua)
					{
						MuaNguyenLieu(1);
						return;
					}
					if ((CheTen == "Mão" || CheTen == "Y phục" || CheTen == "Hộ thủ" || CheTen == "Hài" || CheTen == "Hộ uyển" || CheTen == "Hộ kiên" || CheTen == "Yêu đái") && vatLieu.VaiBong < SoLuongMua)
					{
						MuaNguyenLieu(1);
						return;
					}
					if ((CheTen == "Hạng liên" || CheTen == "Giới chỉ" || CheTen == "Hộ phù") && vatLieu.BiNgan < SoLuongMua)
					{
						MuaNguyenLieu(1);
						return;
					}
				}
				if (vatLieu.DaTaoDo < SoLuongMua)
				{
					MuaNguyenLieu(0);
					return;
				}
				if (TLBB.IsToggleYuanbaoShop)
				{
					LUA.ToggleYuanbaoShop();
					Move(CharX + 3f, CharY + 2f);
				}
				if (IsRide)
				{
					DownRide();
					return;
				}
				Kiemtranguyenlieu = false;
				Thread.Sleep(1000);
			}
			if (TLBB.IsODaoCuFull)
			{
				PushThongBao("Thông báo", "Thiếu ô chứa đồ chế vui lòng sắp xếp", CanhBao.Kieu.Eror);
				IsCheDo = false;
			}
			else
			{
				if (TickCount % 99 != 0)
				{
					return;
				}
				if (TempCount >= SoLuongMua && DaChe < SoLuongChe)
				{
					TempCount = 0;
					Kiemtranguyenlieu = true;
					if (HuyNguyenLieu)
					{
						HuyNguyenLieuTHUA();
					}
					return;
				}
				IsDrop();
				if (DaChe < SoLuongChe)
				{
					Che();
				}
				if (DaChe != SoLuongChe)
				{
					return;
				}
				if (!CheDO.IsRunning)
				{
					CheDO.Start();
				}
				else
				{
					if (!(CheDO.Elapsed.TotalSeconds > 6.0))
					{
						return;
					}
					IsCheDo = false;
					PushThongBao("Thông báo", "Đã Chế Xong", CanhBao.Kieu.OK);
					if (TaskSauCheDO != "Ngồi Chơi")
					{
						if (TaskSauCheDO == "Tắt Máy")
						{
							Process.Start("shutdown", "-s -f -t 0");
						}
						if (TaskSauCheDO == "Đi Train")
						{
							AutoTrain = true;
						}
					}
					CheDO.Stop();
				}
			}
		}

		public void HuyNguyenLieuTHUA()
		{
			foreach (PacketItem item in Packet.NguyenLieu)
			{
				if (TINHKIEM.VietLien(item.Name).Contains("tinhthiet") && item.IsCoDinh)
				{
					DropItem(item.Index);
				}
				if (TINHKIEM.VietLien(item.Name).Contains("vaibong") && item.IsCoDinh)
				{
					DropItem(item.Index);
				}
				if (TINHKIEM.VietLien(item.Name).Contains("bingan") && item.IsCoDinh)
				{
					DropItem(item.Index);
				}
			}
		}

		public void GiamDinh()
		{
			if (!IsGiamDinh)
			{
				return;
			}
			foreach (PacketItem item in PacketItem.Enum(this))
			{
				item.GiamDinh();
			}
		}

		public void Che()
		{
			string hex = "0C9C5F000000000000000000" + IdLoai().ToString("X2") + "000000" + IdCheCap().ToString("X2") + IdNoiNgoai().ToString("X2") + "0000FFFFFFFF" + IdNguyenLieu();
			SendPacket(hex);
			DaChe++;
			TempCount++;
		}

		public void OpenKNBShop()
		{
			string hex = "3C585F00000085150000000046900D000F4F70656E5975616E62616F53686F7000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000030000FFFFFFFF01000000010";
			SendPacket(hex);
		}

		public string DTDName()
		{
			string text = string.Empty;
			if (Is69DO)
			{
				text = "Lv " + (CheCap + 1) + " ";
				switch (TINHKIEM.VietLien(CheTen))
				{
				case "daobua":
					text += "Falchion Plans";
					break;
				case "thuongtan":
					text += "Spear Plans";
					break;
				case "dondoan":
					text += "One-Hand Plans";
					break;
				case "songdoan":
					text += "Two-Hand Plans";
					break;
				case "phien":
					text += "Fan Plans";
					break;
				case "hoan":
					text += "Circle Plans";
					break;
				case "mao":
					text += "Hat Pattern";
					break;
				case "yphuc":
					text += "Garment Plans";
					break;
				case "hothu":
					text += "Glove Pattern";
					break;
				case "hai":
					text += "Shoe Pattern";
					break;
				case "houyen":
					text += "Wristband Plans";
					break;
				case "hokien":
					text += "Shp. Plans";
					break;
				case "yeudai":
					text += "Belt Pattern";
					break;
				case "hanglien":
					text += "Necklace Plans";
					break;
				case "gioichi":
					text += "Ring Design";
					break;
				case "hophu":
					text += "Amulet Design";
					break;
				}
			}
			return TINHKIEM.VietLien(text);
		}

		public string HaveDTD()
		{
			string empty = string.Empty;
			empty = ((TINHKIEM.VietLien(CheTen) == "daobua" || TINHKIEM.VietLien(CheTen) == "thuongtan") ? TINHKIEM.VietLien(CheTen.Replace("Đao búa", "Đao phủ").Replace("Thương tần", "Thương bổng") + "dataodocap" + (CheCap + 1)) : (TINHKIEM.VietLien(CheTen).Contains("mao") ? ((CheCap + 1 <= 10) ? TINHKIEM.VietLien(CheTen + "tudataodo" + (CheCap + 1)) : TINHKIEM.VietLien(CheTen + "tudataodocap" + (CheCap + 1))) : (TINHKIEM.VietLien(CheTen).Contains("khuyen") ? ((CheCap + 1 <= 10) ? TINHKIEM.VietLien("hoandataodo" + (CheCap + 1)) : TINHKIEM.VietLien("hoandataodocap" + (CheCap + 1))) : ((CheCap + 1 <= 10) ? TINHKIEM.VietLien(CheTen + "dataodo" + (CheCap + 1)) : TINHKIEM.VietLien(CheTen + "dataodocap" + (CheCap + 1))))));
			return TINHKIEM.VietLien(empty);
		}

		public string HaveNguyenLieu()
		{
			switch (CheLoai)
			{
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
				if (Is69DO)
				{
					foreach (PacketItem item in PacketItem.Enum(this))
					{
						if (TINHKIEM.VietLien(item.Name).Contains("refinediron"))
						{
							return string.Empty;
						}
					}
					return "refinediron";
				}
				foreach (PacketItem item2 in PacketItem.Enum(this))
				{
					if (TINHKIEM.VietLien(item2.Name).Contains("tinhthiet"))
					{
						return string.Empty;
					}
				}
				return "tinhthiet";
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
				if (Is69DO)
				{
					foreach (PacketItem item3 in PacketItem.Enum(this))
					{
						if (TINHKIEM.VietLien(item3.Name).Contains("cottoncloth"))
						{
							return string.Empty;
						}
					}
					return "cottoncloth";
				}
				foreach (PacketItem item4 in PacketItem.Enum(this))
				{
					if (TINHKIEM.VietLien(item4.Name).Contains("vaibong"))
					{
						return string.Empty;
					}
				}
				return "vaibong";
			case 13:
			case 14:
			case 15:
				if (Is69DO)
				{
					foreach (PacketItem item5 in PacketItem.Enum(this))
					{
						if (TINHKIEM.VietLien(item5.Name).Contains("darksilver"))
						{
							return string.Empty;
						}
					}
					return "darksilver";
				}
				foreach (PacketItem item6 in PacketItem.Enum(this))
				{
					if (TINHKIEM.VietLien(item6.Name).Contains("bingan"))
					{
						return string.Empty;
					}
				}
				return "bingan";
			default:
				return "";
			}
		}

		public string IdNguyenLieu()
		{
			switch (CheLoai)
			{
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
				foreach (PacketItem item in PacketItem.Enum(this))
				{
					if (item.Type.Contains("Ore_15"))
					{
						return item.Index.ToString("X2");
					}
				}
				return "";
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
				foreach (PacketItem item2 in PacketItem.Enum(this))
				{
					if (item2.Type.Contains("Ore_14"))
					{
						return item2.Index.ToString("X2");
					}
				}
				return "";
			case 13:
			case 14:
			case 15:
				foreach (PacketItem item3 in PacketItem.Enum(this))
				{
					if (item3.Type.Contains("Ore_13"))
					{
						return item3.Index.ToString("X2");
					}
				}
				return "";
			default:
				return "";
			}
		}

		public int IdLoai()
		{
			switch (CheLoai)
			{
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
				return 46;
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
				return 47;
			case 13:
			case 14:
			case 15:
				return 48;
			default:
				return 0;
			}
		}

		public int IdCheCap()
		{
			int result = 0;
			switch (CheLoai)
			{
			case 0:
				return 132 + CheCap;
			case 1:
				return 142 + CheCap;
			case 2:
				return 152 + CheCap;
			case 3:
				return 162 + CheCap;
			case 4:
				return 172 + CheCap;
			case 5:
				return 182 + CheCap;
			case 6:
				if (CheNoiNgoai == 1)
				{
					return 192 + CheCap;
				}
				if (CheNoiNgoai == 2)
				{
					return 76 + CheCap;
				}
				return 36 + CheCap;
			case 7:
				if (CheNoiNgoai == 1)
				{
					return 222 + CheCap;
				}
				if (CheNoiNgoai == 2)
				{
					return 106 + CheCap;
				}
				return 66 + CheCap;
			case 8:
				if (CheNoiNgoai == 1)
				{
					return 212 + CheCap;
				}
				if (CheNoiNgoai == 2)
				{
					return 96 + CheCap;
				}
				return 56 + CheCap;
			case 9:
				if (CheNoiNgoai == 1)
				{
					return 202 + CheCap;
				}
				if (CheNoiNgoai == 2)
				{
					return 86 + CheCap;
				}
				return 46 + CheCap;
			case 10:
				return 232 + CheCap;
			case 11:
				return 242 + CheCap;
			case 12:
				if (CheCap <= 4)
				{
					return 252 + CheCap;
				}
				return CheCap - 4;
			case 13:
				return 6 + CheCap;
			case 14:
				if (CheNoiNgoai != 1)
				{
					if (CheNoiNgoai != 2)
					{
						return 16 + CheCap;
					}
					if (CheCap <= 6)
					{
						return 16 + CheCap;
					}
					switch (CheCap)
					{
					case 7:
						return 40;
					case 8:
						return 42;
					case 9:
						return 44;
					default:
						return result;
					}
				}
				if (CheCap <= 6)
				{
					return 16 + CheCap;
				}
				switch (CheCap)
				{
				case 7:
					return 41;
				case 8:
					return 43;
				case 9:
					return 45;
				default:
					return result;
				}
			case 15:
				if (CheNoiNgoai != 1)
				{
					if (CheNoiNgoai != 2)
					{
						return 26 + CheCap;
					}
					if (CheCap <= 6)
					{
						return 26 + CheCap;
					}
					switch (CheCap)
					{
					case 7:
						return 47;
					case 8:
						return 48;
					case 9:
						return 50;
					default:
						return result;
					}
				}
				if (CheCap <= 6)
				{
					return 26 + CheCap;
				}
				switch (CheCap)
				{
				case 7:
					return 46;
				case 8:
					return 49;
				case 9:
					return 51;
				default:
					return result;
				}
			default:
				return result;
			}
		}

		public int IdNoiNgoai()
		{
			switch (CheLoai)
			{
			case 0:
				return 2;
			case 1:
				return 2;
			case 2:
				return 2;
			case 3:
				return 2;
			case 4:
				return 2;
			case 5:
				return 2;
			case 6:
				if (CheNoiNgoai != 1)
				{
					_ = CheNoiNgoai;
					return 3;
				}
				return 2;
			case 7:
				if (CheNoiNgoai != 1)
				{
					_ = CheNoiNgoai;
					return 3;
				}
				return 2;
			case 8:
				if (CheNoiNgoai != 1)
				{
					_ = CheNoiNgoai;
					return 3;
				}
				return 2;
			case 9:
				if (CheNoiNgoai != 1)
				{
					_ = CheNoiNgoai;
					return 3;
				}
				return 2;
			case 10:
				return 2;
			case 11:
				return 2;
			case 12:
				if (CheCap <= 4)
				{
					return 2;
				}
				return 3;
			case 13:
				return 3;
			case 14:
				if (CheCap <= 6)
				{
					return 3;
				}
				if (CheNoiNgoai == 1)
				{
					return 4;
				}
				if (CheNoiNgoai == 2)
				{
					return 4;
				}
				return 3;
			case 15:
				if (CheCap <= 6)
				{
					return 3;
				}
				if (CheNoiNgoai == 1)
				{
					return 4;
				}
				if (CheNoiNgoai == 2)
				{
					return 4;
				}
				return 3;
			default:
				return 0;
			}
		}

		public void PushDebugMessage(string msg)
		{
			LuaDoOneLineString("PushDebugMessage(\"" + msg + "\");");
		}

		public bool IsDrop()
		{
			bool result = false;
			foreach (PacketItem item in PacketItem.Enum(this))
			{
				if (TINHKIEM.VietLien(item.TypeName) == TINHKIEM.VietLien(CheTen).Replace("hoan", "khuyen") || (!item.Name.Contains("Lv ") && Is69DO && TINHKIEM.VietLien(DTDName().Replace("lv", "")).Contains(TINHKIEM.VietLien(item.TypeName).Replace("shoulderpad", "shp.").TrimEnd('s')
					.Replace("two-handed", "two-hand")
					.Replace("one-handed", "one-hand"))))
				{
					string text = "";
					if (item.Star < CheSao)
					{
						text = text + "[Sao] = " + item.Star + " nhỏ hơn " + CheSao;
					}
					if (item.Line < CheDong)
					{
						text = text + "| [Số Dòng] = " + item.Line + " nhỏ hơn " + CheDong;
					}
					if (item.TheLuc < CheDiem)
					{
						text = text + "| [Thể Lực] = " + item.TheLuc + " nhỏ hơn " + CheDiem;
					}
					if (IsCanDelete(item) && !item.IsHaveLongVan && !item.IsHaveNgoc && text.Length > 0 && !TmpItemBeforeChe.Contains(item.Index))
					{
						string text2 = "Hủy vật phẩm :" + item.Name + " VÌ " + text;
						FrmMain.AddLog(DateTime.Now.ToString("dd-MM | HH:mm") + " [" + LastName + "] " + text2 + " \n");
						PushDebugMessage(text2);
						DropItem(item.Index);
						DaHuy++;
						result = true;
					}
				}
			}
			return result;
		}

		public void BuyKNBShop(int index)
		{
			if (Address.GameType == 2)
			{
				string hex = "48DA60000000120000000000" + index.ToString("X2") + "009615FFFF00000000120000000000010";
				SendPacket(hex);
			}
		}

		public void Muabake()
		{
			SendPacket(Address.bakePacket + "00 00 00 00 FF FF FF FF 11 00 00 00 02 00 00 00 0C 00 00 00 00 00 00 00 FF FF FF FF 14");
		}

		public string TenThanh()
		{
			LuaDoOneLineString("local szMsgtenthanh = Guild:GetMyGuildDetailInfo('CityName'); return szMsgtenthanh;");
			LuaToStringBang();
			return LuaStringBang();
		}

		public string TenMapThanh()
		{
			LuaDoOneLineString("local szMsgtenmap = Guild:GetMyGuildDetailInfo('LocalScene'); return szMsgtenmap;");
			LuaToString();
			return LuaString();
		}

		public void ReturnBang()
		{
			LuaDoString(TINHKIEM.Hasher.Decrypt("[REDACTED]", "[REDACTED]" /* analysis redaction */));
		}

		public void BangOpen()
		{
			LuaDoString("setmetatable(_G, {__index = NewBangHui_Hygl_Env});Guild:AskGuildDetailInfo();");
		}

		public void BangClose()
		{
			PostMessage(63, 105);
		}

		[DllImport("Bin\\EasyHook.dll")]
		private static extern IntPtr SetHook(IntPtr handle);

		[DllImport("Bin\\EasyHook.dll")]
		public static extern IntPtr UnHook(IntPtr handle);
	}
}
