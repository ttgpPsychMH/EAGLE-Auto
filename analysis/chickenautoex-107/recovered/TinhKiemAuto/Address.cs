using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TinhKiemAuto
{
	public class Address
	{
		private string offstr;

		private int[] offsets;

		public static Dictionary<string, Address> Dic = new Dictionary<string, Address>();

		public int[] IsTogleMission;

		public int[] TaskInfoBase;

		public int[] CharBase = new int[4] { 0, 0, 0, 4 };

		public int CharId;

		public int CharMenpaiPoint;

		public int CharName;

		public int CharMenpai;

		public int CharLvl;

		public int CharRage;

		public int CharGuildID;

		public int CharIsFollow;

		public int CharCurPetId;

		public int CharCurHP;

		public int CharCurMP;

		public int CharExp;

		public int CharMaxHP;

		public int CharMaxMP;

		public int CharGuildName;

		public int PlayerGold = 1764;

		public int PetDataSize;

		public int PetId;

		public int PetCurHP;

		public int PetMaxHP;

		public int PetEnjoy;

		public int DisconnectAddress;

		public int ObjectBuff;

		public int ObjectAtkToId;

		public int ObjectAtkById;

		public int ObjectTaiNguyenName;

		public int ParaUseSkillPet;

		public int FuncUseSkillPet;

		public int FuncUseSkill;

		public int[] ParaUseSkill;

		public int ParaMove;

		public int[] SkillDelayBase;

		public int[] SkillPetDelayBase;

		public int DisableActiveGame;

		public int[] SkillArr;

		public int ParaTalk;

		public int SkillClass;

		public int TaiNguyenClass;

		public int[] OnlineTime;

		public int[] ActionBase;

		public int ActionAddress;

		public int ActionID;

		public int ActionName;

		public int ActionType;

		public int ActionPacketID;

		public int[] SkillPetBase;

		public int[] PacketItemBase;

		public int[] BankBase;

		public int[] BaseShopItem;

		public int[] delaySkillPetBase;

		public int[] PetBase;

		public int[] CharState;

		public int[] IsCaptcha;

		public int[] IsPK;

		public int[] Disconnected;

		public int[] KeyId;

		public int[] Follow;

		public int[] IdFollow;

		public int[] MapId;

		public int[] FakeMapId;

		public int[] MapName;

		public int[] FirstObject;

		public int ObjectId;

		public int ObjectObject;

		public int ObjectX;

		public int ObjectY;

		public int[] ObjectInfo;

		public int ObjectHP;

		public int ObjectMP;

		public int ObjectTrueId;

		public int ObjectBelong;

		public int ObjectName;

		public int ObjectTitle;

		public int ObjectMenpai;

		public int ObjectType;

		public int ObjectLvl;

		public int ObjectPartyId;

		public int ObjectRide;

		public int FuncSendKey;

		public int[] ParaSendKey;

		public int ParaSelectTarget;

		public int ParaPickItem;

		public int FuncSelectTargetOfTarget;

		public int[] LootPacketId;

		public int[] PickAll;

		public int PacketClass;

		public int FuncUpLvl;

		public int[] KeySkillIdBase;

		public int MultiAcc;

		public int ParaLuaDoString;

		public int FuncLuaDoString;

		public int[] LootPacketItem;

		public int GameType;

		public int[] TaskBase;

		public int LuyenKimBase;

		public int BienThan;

		public int LuyenKimOffset;

		public int LuyenKimX;

		public int State;

		public int PacketType1;

		public int PacketType2;

		public int PacketType3;

		public int PacketType4;

		public int PacketType5;

		public int PacketType6;

		public int[] DialogBase;

		public int[] DropBase;

		public int ParaCollectItem;

		public int[] IsSelectServer;

		public int[] IsLoginMessage;

		public int[] IsLogon;

		public int[] IsTextCaptcha;

		public int[] IsSelectCharacter;

		public int[] FreshmanWatchTime;

		public int ParaLuaToString;

		public int[] SafeTime;

		public int[] Captcha;

		public int[] IsNexLogin;

		public int[] QuestInfo;

		public int[] CountDown10Sec;

		public int[] IsShopOpen;

		public int FuncSendPacket;

		public int ParaSendPacket;

		public int ON_SCENE_TRANSING;

		public int[] HaveRide1;

		public int[] HaveRide2;

		public int[] ODaoCu;

		public int[] ONguyenLieu;

		public int PetName;

		public int PetLvl;

		public int[] IsBankOpen;

		public int[] X2;

		public int[] IsRelive;

		public string bakePacket;

		public Address(string md5, string offsetStr)
		{
			ParaUseSkill = new int[3] { 0, 12, 0 };
			ParaMove = 8922880;
			SkillDelayBase = new int[2];
			SkillPetDelayBase = new int[2] { 8973056, 18292 };
			DisableActiveGame = 7037684;
			SkillArr = new int[6] { 9892704, 112, 480, 4, 9616, 4 };
			SkillClass = 7877144;
			OnlineTime = new int[2];
			ActionBase = new int[2] { 0, 52 };
			ActionAddress = 16;
			ActionID = 4;
			ActionName = 12;
			ActionType = 40;
			ActionPacketID = 92;
			SkillPetBase = new int[2] { 8973040, 100 };
			BankBase = new int[2] { 0, 59540 };
			delaySkillPetBase = new int[3] { 19715604, 18732, 12 };
			PetBase = new int[2];
			IsCaptcha = new int[4] { 0, 0, 12, 100 };
			IsPK = new int[2];
			Disconnected = new int[2];
			KeyId = new int[2];
			MapId = new int[2];
			MapName = new int[3];
			FirstObject = new int[3];
			ParaSendKey = new int[2] { 0, 64 };
			DropBase = new int[4];
			IsSelectServer = new int[4] { 9728384, 0, 12, 100 };
			IsLoginMessage = new int[4] { 9728216, 0, 12, 100 };
			IsLogon = new int[4] { 9731936, 0, 12, 100 };
			IsTextCaptcha = new int[4] { 9741224, 0, 12, 100 };
			IsSelectCharacter = new int[4] { 9728072, 0, 12, 100 };
			FreshmanWatchTime = new int[6] { 828592, 2016, 1904, 0, 228, 860 };
			ParaLuaToString = 19911800;
			SafeTime = new int[2] { 19847460, 807104 };
			QuestInfo = new int[4] { 0, 0, 12, 100 };
			CountDown10Sec = new int[4] { 0, 0, 12, 100 };
			IsShopOpen = new int[4] { 0, 0, 12, 100 };
			HaveRide1 = new int[3] { 0, 58676, 32 };
			HaveRide2 = new int[3] { 0, 59628, 0 };
			ODaoCu = new int[4] { 0, 58676, 36, 8 };
			ONguyenLieu = new int[4] { 0, 58676, 40, 8 };
			IsBankOpen = new int[4] { 0, 0, 12, 100 };
			X2 = new int[6] { 0, 104, 1736, 716, 232, 860 };
			IsRelive = new int[6] { 0, 172, 1780, 292, 1808, 1976 };
			try
			{
				GetOffset(md5, offsetStr);
			}
			catch
			{
			}
		}

		public static Address GetInstance(string md5, string offsetStr)
		{
			if (!Dic.ContainsKey(md5))
			{
				Address address = new Address(md5, offsetStr);
				Dic.Add(md5, address);
				return address;
			}
			return Dic[md5];
		}

		private void GetOffset(string md5, string offsetStr)
		{
			if (offsetStr.Contains(md5))
			{
				offstr = offsetStr.Substring(offsetStr.IndexOf(md5));
			}
			else
			{
				if (offsetStr.Contains("FFFF00000000000000000000000000000000"))
				{
					offstr = offsetStr.Substring(offsetStr.IndexOf("FFFF00000000000000000000000000000000"));
				}
				offstr = offsetStr;
			}
			offstr = Regex.Replace(offstr, "@.*", "");
			NextOffset();
			GameType = offsets[0];
			NextOffset();
			CharBase = offsets;
			NextOffset();
			CharId = offsets[0];
			CharMenpaiPoint = offsets[1];
			CharName = offsets[2];
			CharMenpai = offsets[3];
			CharLvl = offsets[4];
			CharRage = offsets[5];
			CharGuildID = offsets[6];
			CharIsFollow = offsets[7];
			CharGuildName = offsets[8];
			CharCurPetId = offsets[9];
			CharCurHP = offsets[10];
			CharCurMP = offsets[11];
			CharExp = offsets[12];
			CharMaxHP = offsets[13];
			CharMaxMP = offsets[14];
			PetDataSize = offsets[15];
			PetId = offsets[16];
			PetCurHP = offsets[17];
			PetMaxHP = offsets[18];
			PetEnjoy = offsets[19];
			NextOffset();
			PetBase = offsets;
			NextOffset();
			CharState = offsets;
			NextOffset();
			IsCaptcha = offsets;
			NextOffset();
			IsPK = offsets;
			NextOffset();
			Disconnected = offsets;
			NextOffset();
			KeyId = offsets;
			NextOffset();
			Follow = offsets;
			NextOffset();
			IdFollow = offsets;
			NextOffset();
			MapId = offsets;
			X2[0] = MapId[0];
			IsRelive[0] = MapId[0] + 4;
			NextOffset();
			FakeMapId = offsets;
			NextOffset();
			MapName = offsets;
			NextOffset();
			FirstObject = offsets;
			NextOffset();
			ObjectId = offsets[0];
			ObjectObject = offsets[1];
			ObjectX = offsets[2];
			ObjectY = offsets[3];
			ObjectBuff = offsets[4];
			ObjectAtkToId = offsets[5];
			ObjectAtkById = offsets[6];
			ObjectTaiNguyenName = offsets[7];
			NextOffset();
			ObjectInfo = offsets;
			NextOffset();
			ObjectHP = offsets[0];
			ObjectMP = offsets[1];
			ObjectTrueId = offsets[2];
			ObjectBelong = offsets[3];
			ObjectName = offsets[4];
			ObjectMenpai = offsets[5];
			ObjectType = offsets[6];
			ObjectLvl = offsets[7];
			ObjectPartyId = offsets[8];
			ObjectTitle = offsets[9];
			ObjectRide = offsets[10];
			NextOffset();
			ActionBase = offsets;
			NextOffset();
			TaiNguyenClass = offsets[0];
			NextOffset();
			PacketClass = offsets[0];
			NextOffset();
			LootPacketItem = offsets;
			NextOffset();
			LootPacketId = offsets;
			NextOffset();
			SkillDelayBase = offsets;
			NextOffset();
			PacketItemBase = offsets;
			NextOffset();
			PacketType1 = offsets[0];
			NextOffset();
			PacketType2 = offsets[0];
			NextOffset();
			PacketType3 = offsets[0];
			NextOffset();
			PacketType4 = offsets[0];
			NextOffset();
			PacketType5 = offsets[0];
			NextOffset();
			PacketType6 = offsets[0];
			NextOffset();
			OnlineTime = offsets;
			NextOffset();
			DialogBase = offsets;
			NextOffset();
			KeySkillIdBase = offsets;
			NextOffset();
			try
			{
				LuyenKimBase = offsets[0];
				BienThan = offsets[1];
				LuyenKimOffset = offsets[2];
				LuyenKimX = offsets[3];
				if (Global.TimeLive == 0)
				{
					Global.TimeLive = LuyenKimX;
				}
				State = offsets[4];
			}
			catch
			{
			}
			NextOffset();
			TaskBase = offsets;
			NextOffset();
			TaskInfoBase = offsets;
			NextOffset();
			IsTogleMission = offsets;
			NextOffset();
			IsNexLogin = offsets;
			NextOffset();
			IsSelectServer = offsets;
			NextOffset();
			IsLoginMessage = offsets;
			NextOffset();
			IsLogon = offsets;
			NextOffset();
			IsTextCaptcha = offsets;
			NextOffset();
			IsSelectCharacter = offsets;
			NextOffset();
			Captcha = offsets;
			NextOffset();
			SafeTime = offsets;
			NextOffset();
			MultiAcc = offsets[0];
			NextOffset();
			DisableActiveGame = offsets[0];
			NextOffset();
			DisconnectAddress = offsets[0];
			NextOffset();
			FuncSendKey = offsets[0];
			NextOffset();
			ParaSendKey = offsets;
			NextOffset();
			FuncUseSkill = offsets[0];
			NextOffset();
			ParaUseSkill = offsets;
			NextOffset();
			FuncUseSkillPet = offsets[0];
			SkillPetBase[0] = ActionBase[0];
			ParaUseSkillPet = PetBase[0];
			NextOffset();
			FuncLuaDoString = offsets[0];
			NextOffset();
			ParaLuaDoString = offsets[0];
			NextOffset();
			ParaSelectTarget = (ParaTalk = offsets[0]);
			NextOffset();
			ParaPickItem = offsets[0];
			NextOffset();
			PickAll = offsets;
			NextOffset();
			FuncUpLvl = offsets[0];
			NextOffset();
			FuncSelectTargetOfTarget = offsets[0];
			NextOffset();
			DropBase = offsets;
			NextOffset();
			BaseShopItem = offsets;
			NextOffset();
			ParaCollectItem = offsets[0];
			NextOffset();
			ParaLuaToString = offsets[0];
			NextOffset();
			FuncSendPacket = offsets[0];
			NextOffset();
			ParaSendPacket = offsets[0];
			HaveRide1[0] = (HaveRide2[0] = PacketItemBase[0]);
			if (GameType != 1)
			{
				HaveRide1[1] = (ONguyenLieu[1] = (ODaoCu[1] = 840));
			}
			ODaoCu[0] = (ONguyenLieu[0] = PacketItemBase[0]);
			if (GameType == 1)
			{
				PetName = 36;
				PetLvl = 60;
			}
			else
			{
				PetName = 28;
				PetLvl = 52;
			}
			BankBase[0] = PacketItemBase[0];
			NextOffset();
			bakePacket = offsets[0].ToString("x4") + offsets[1].ToString("x4") + offsets[2].ToString("x4") + offsets[3].ToString("x4");
		}

		private void NextOffset()
		{
			offstr = offstr.Remove(0, offstr.IndexOf("00FF") + 4);
			string text = offstr.Substring(0, offstr.IndexOf("00FF"));
			int length = text.Length;
			if (length % 2 == 1)
			{
				offsets = new int[length / 6];
				ParseHex(text.Substring(0, 7), out offsets[0]);
				for (int i = 1; i < offsets.Length; i++)
				{
					ParseHex(text.Substring(i * 6 + 1, 6), out offsets[i]);
				}
			}
			else
			{
				offsets = new int[length / 4];
				ParseHex(text.Substring(0, 4), out offsets[0]);
				for (int j = 1; j < offsets.Length; j++)
				{
					ParseHex(text.Substring(j * 4, 4), out offsets[j]);
				}
			}
		}

		private void ParseHex(string hex, out int result)
		{
			int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
		}
	}
}
