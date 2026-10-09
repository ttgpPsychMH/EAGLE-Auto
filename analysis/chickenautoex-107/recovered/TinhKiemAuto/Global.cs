using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TinhKiemAuto.AutoControl;
using TinhKiemAuto.Models;

namespace TinhKiemAuto
{
	internal class Global
	{
		public static string UpdateURL = "http://update.chickenauto.com/PatchInfoEx.ini";

		public static bool LanLuot = true;

		public static DateTime ExaclyTime = DateTime.MinValue;

		public static int Delay = 50;

		public static bool IsTrimRam = true;

		public static int PetLvl = 125;

		public static int Wait = 2;

		public static bool HideBHD = true;

		public static int IsFull = 5000;

		public static bool ssss;

		public static int HookMessage;

		public static bool BuffPet = true;

		public static bool ItemFillter = true;

		public static bool AtkFollowKey = false;

		public static int BuffHPPercent = 50;

		public static int BuffMPPercent = 50;

		public static int BuffNMPercent = 75;

		public static int ExitHPPercent = 10;

		public static int NoiRadius = 20;

		public static int NgoaiRadius = 15;

		public static int PickRadius = 30;

		public static bool ExitPk = false;

		public static bool AlarmPk = true;

		public static bool AlarmHP = true;

		public static int AlarmHPPercent = 30;

		public static bool Mute = false;

		public static bool AutoComeBack = false;

		public static bool PickItem = true;

		public static bool AutoUpLvl = false;

		public static bool AutoDropItem = false;

		public static bool AutoSellItem = false;

		public static bool FollowKey = false;

		public static bool UsingSkill = true;

		public static int AutoUpLvlBelow = 20;

		public static bool AutoShutDown = false;

		public static bool UseSkillPet = true;

		public static bool IsBoQua = false;

		public static bool AutoResetTime = true;

		public static bool BuffQuanDoan = false;

		public static bool AutoAccept = true;

		public static bool AcceptAll = true;

		public static bool IsAcTac = false;

		public static bool Paused = false;

		public static bool IsXaPhu = true;

		public static Keys BaseSkill = Keys.F1;

		public static Keys NMSkill = Keys.F13;

		public static Keys HPKey = Keys.F13;

		public static bool IsHuyDanhQuai = false;

		public static bool IsHuyThaiHo = false;

		public static bool IsHyHuu = false;

		public static int Speed = 3;

		public static int FollowRadius = 3;

		public static int BHDCount = 20;

		public static bool AntiCaptcha = true;

		public static bool AntiCaptchaSelf = false;

		public static bool AutoPk = true;

		public static int MaxBHD = 6;

		public static bool IsTuVaoPhai = false;

		public static int XDua = 134;

		public static bool openbang = false;

		public static int NumNhiemVuDua = 1;

		public static bool Mapbang = false;

		public static int YDua = 165;

		public static int MapDua = MAP.TayHo;

		public static List<string> BangOnPC = new List<string>();

		public static List<string> DoNgonDua = new List<string>();

		public static bool IsKhacMay = true;

		public static string Version => "107";

		public static int YearExp { get; set; }

		public static bool IsVutRac { get; set; }

		public static bool IsAdmin => false;

		public static bool IsFix => Setting.Read("User", "Fix") == "True";

		public static bool IsMulti => Setting.Read("User", "Multi") == "True";

		public static bool IsXuat { get; set; }

		public static int TimeLive { get; set; }

		public static TINHKIEM.Menpai GlSetMenPai { get; set; }

		public static bool GlIsSetMenPai { get; set; }

		public static int IsVIP { get; set; }

		public static bool RemoveAd { get; set; }

		public static string SelfMd5 => TINHKIEM.Hasher.MD5(Application.ExecutablePath);

		public static string DataPath
		{
			get
			{
				string text = APPPath + "\\Data";
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				return text;
			}
		}

		public static string LogPath
		{
			get
			{
				string text = APPPath + "\\Logs";
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				return text;
			}
		}

		public static string CalenderPath
		{
			get
			{
				string text = APPPath + "\\Config\\Calender";
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				return text;
			}
		}

		public static string NhatHaPath
		{
			get
			{
				string text = APPPath + "\\Config\\NhatHaPath";
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				return text;
			}
		}

		public static string APPPath => Path.GetDirectoryName(Application.ExecutablePath);

		public static string Argument => "\"" + Application.ExecutablePath + "\"";

		public static void SetInfo(AutoReport autoreport)
		{
			foreach (KeyValuePair<int, Game> item in FrmMain.dicGame)
			{
				if (autoreport.CharID == item.Value.TLBB.Id)
				{
					BaiTrain baiTrain = new BaiTrain();
					baiTrain.Level = 40;
					baiTrain.MapID = autoreport.LenBai.MapID;
					baiTrain.MapName = autoreport.LenBai.MapName;
					baiTrain.Name = "Bãi tùy chỉnh";
					baiTrain.PosX = autoreport.LenBai.PosX;
					baiTrain.PosY = autoreport.LenBai.PosY;
					item.Value._baitrain = baiTrain;
					item.Value.LenBaiTrain = autoreport.IsLenBai;
					item.Value.IsAttack = autoreport.IsAttack;
				}
			}
		}

		public static AutoReport CreateFromGame(Game game)
		{
			AutoReport autoReport = new AutoReport();
			autoReport.ExpPercent = game.TLBB.ExpPercent;
			autoReport.Gold = game.TLBB.Gold;
			autoReport.HpPercent = game.TLBB.HPPercent;
			autoReport.IsAttack = game.IsAttack;
			autoReport.IsDisconnect = game.TLBB.Disconnected;
			autoReport.IsDuoc = game.IsDuoc;
			autoReport.IsLear = game.TLBB.IsLeader;
			autoReport.IsPickItem = game.IsPickItem;
			autoReport.IsRide = game.IsRide;
			autoReport.IsX25 = game.TuAnX2;
			autoReport.MapIndex = game.TLBB.MapId;
			autoReport.MapName = game.TLBB.MapName;
			autoReport.MpPercent = game.TLBB.MPPercent;
			autoReport.Msg = "";
			autoReport.Online = game.TLBB.Online;
			autoReport.PetPercent = game.TLBB.PetHPPercent;
			autoReport.PlayState = game.TLBB.PlayerState;
			autoReport.PosX = (int)game.CharX;
			autoReport.PosY = (int)game.CharY;
			CheDo cheDo = new CheDo();
			cheDo.CheCap = game.CheCap;
			cheDo.CheLoai = game.CheLoai;
			cheDo.IsCheDo = game.IsCheDo;
			cheDo.IsMienPhiNguyenLieu = !game.IsMuaNguyenLieu;
			cheDo.TongNhan = game.SoLuongMua;
			cheDo.TotalChe = game.SoLuongChe;
			cheDo.CheDiem = game.CheDiem;
			cheDo.CheDong = game.CheDong;
			cheDo.CheSao = game.CheSao;
			autoReport.IsLenBai = game.LenBaiTrain;
			autoReport.CheDo = cheDo;
			LenBai lenBai = new LenBai();
			lenBai.MapID = game._baitrain.MapID;
			lenBai.MapName = game._baitrain.MapName;
			lenBai.PosX = game._baitrain.PosX;
			lenBai.PosY = game._baitrain.PosY;
			autoReport.LenBai = lenBai;
			autoReport.CharID = game.TLBB.Id;
			autoReport.CharName = game.TLBB.Name;
			autoReport.Level = game.TLBB.Lvl;
			autoReport.GuildName = game.TLBB.GuildName;
			autoReport.GuildId = game.TLBB.GuildId.ToString() ?? "";
			autoReport.Phai = game.TLBB.MenpaiName;
			OverView overView = new OverView();
			if (game.TLBB.IsNoi)
			{
				overView.PhamViDanh = NoiRadius;
			}
			else
			{
				overView.PhamViDanh = NgoaiRadius;
			}
			TotalSkill totalSkill = new TotalSkill();
			List<SkillAuto> list = new List<SkillAuto>();
			List<SkillAuto> list2 = new List<SkillAuto>();
			Skill[] array = game.Skills.ToArray();
			foreach (Skill skill in array)
			{
				if (Skill.IsBuffSkill(skill.PacketId))
				{
					SkillAuto skillAuto = new SkillAuto();
					skillAuto.Id = skill.PacketId;
					skillAuto.IsUsing = skill.UserBuff;
					skillAuto.Name = skill.Name;
					list2.Add(skillAuto);
				}
				else
				{
					SkillAuto skillAuto2 = new SkillAuto();
					skillAuto2.Id = skill.PacketId;
					skillAuto2.IsUsing = skill.Use;
					skillAuto2.Name = skill.Name;
					list.Add(skillAuto2);
				}
			}
			Features features = new Features();
			features.IsAcceptParty = AutoAccept;
			features.IsAcceptAllPartyInvites = AcceptAll;
			features.IsUseSkillF1 = Option.PutBase;
			features.LimitLevelUp = AutoUpLvlBelow;
			features.MakeAdvertisement = game.IsRao;
			features.MakeAdvertisingTime = (int)game.TimeGiaoChat;
			features.NoticePrivateMessage = game.AlarmChat;
			features.Pass2 = game.Pass2;
			features.ChatMSG = game.RaoTxt;
			features.Chanel = 1;
			features.FollowRadius = FollowRadius;
			TotalPet totalPet = new TotalPet();
			List<PetAutoInfo> list3 = new List<PetAutoInfo>();
			totalPet.AutoCallBackAtLevel = PetLvl;
			totalPet.IsAutoBuffPet = BuffPet;
			totalPet.IsAutoCallBackPet = game.AutoThuPet;
			totalPet.IsAutoCallPet = IsXuat;
			totalPet.IsAutoTakeCare = game.IsPet;
			totalPet.IsAutoUsePetSkill = UseSkillPet;
			totalPet.PetInfo = list3;
			int num = 0;
			KeyValuePair<int, string>[] array2 = game.TLBB.DicPet.ToArray();
			for (int i = 0; i < array2.Length; i++)
			{
				KeyValuePair<int, string> keyValuePair = array2[i];
				PetAutoInfo petAutoInfo = new PetAutoInfo();
				petAutoInfo.Id = keyValuePair.Key;
				petAutoInfo.Name = keyValuePair.Value;
				petAutoInfo.Pos = num;
				num++;
				list3.Add(petAutoInfo);
			}
			totalSkill.ActiveSkill = list;
			totalSkill.PassiveSkill = list2;
			overView.IsDanhQuanhDiem = game.IsRadius;
			overView.IsGomQuai = game.IsLure;
			overView.IsUsingThoLinhChau = game.UsingTholinhChau;
			overView.IsRengeHP = game.IsHP;
			overView.RengeHPPercent = BuffHPPercent;
			overView.IsRengeMP = game.IsMP;
			overView.RengeMPPercent = BuffMPPercent;
			overView.IsCongSinh = game.CongSinh;
			overView.CongSinhValue = game.CongSinhValue;
			overView.IsHuyetTe = game.HuyetTe;
			overView.HuyetTeValue = game.HuyetTeValue;
			overView.IsNM = game.IsNM;
			overView.BuffNMPercent = BuffNMPercent;
			overView.IsAutoReborn = game.AutoHoiSinh;
			overView.IsAutoComeBack = AutoComeBack;
			overView.isArletHP = AlarmHP;
			overView.ArletHPPercent = AlarmHPPercent;
			autoReport.OverView = overView;
			autoReport.Pets = totalPet;
			autoReport.Skills = totalSkill;
			autoReport.Features = features;
			return autoReport;
		}
	}
}
