using System;
using System.Collections.Generic;

namespace TinhKiemAuto
{
	public class GameObject
	{
		public List<int> buff;

		private Game Game { get; set; }

		private Memory Memory { get; set; }

		private Address ADD { get; set; }

		public int Address { get; set; }

		public int Id { get; set; }

		public int Object { get; set; }

		public int Class { get; set; }

		public float X { get; set; }

		public float Y { get; set; }

		public List<int> Buff
		{
			get
			{
				if (buff == null)
				{
					buff = new List<int>();
					foreach (int item in BuffAddress)
					{
						buff.Add(Game.Memory.Read(item + 12));
					}
				}
				return buff;
			}
		}

		public int State { get; set; }

		public int AtkToId { get; set; }

		public int AtkById { get; set; }

		public int InfoAddress { get; set; }

		public float HP { get; set; }

		public float MP { get; set; }

		public string TrueId { get; set; }

		public string Belong { get; set; }

		public string Name { get; set; }

		public string CleanName => TINHKIEM.VietLien(Name);

		public int Menpai { get; set; }

		public string Type { get; set; }

		public int Lvl { get; set; }

		public int PartyId { get; set; }

		public string Title { get; set; }

		public int Ride { get; set; }

		public int QDId { get; set; }

		public float Distance => TINHKIEM.GetDistance(Game.CharX, Game.CharY, X, Y);

		public float DistanceEx { get; set; }

		public int NameAddress { get; set; }

		public int RoundX => (int)Math.Round(X, 0, MidpointRounding.AwayFromZero);

		public int RoundY => (int)Math.Round(Y, 0, MidpointRounding.AwayFromZero);

		public string BuffToString
		{
			get
			{
				string text = "";
				foreach (int item in Buff)
				{
					text = text + item + ",";
				}
				return text.Trim(',');
			}
		}

		public bool IsLootPacket => Class == Game.Address.PacketClass;

		public bool IsTaiNguyen => Class == Game.Address.TaiNguyenClass;

		public bool IsKhoang
		{
			get
			{
				if (IsTaiNguyen && (!(Setting.KoKhaiThac != string.Empty) || !TINHKIEM.VietLien(Setting.KoKhaiThac).Contains(TINHKIEM.VietLien(Name))))
				{
					if (TINHKIEM.IsKhoang(Name) == -1)
					{
						return Game.Is69DO;
					}
					return true;
				}
				return false;
			}
		}

		public string MD => TINHKIEM.Hasher.MD5(Name).Substring(0, 3);

		public bool IsDuoc
		{
			get
			{
				if (IsTaiNguyen && (!(Setting.KoKhaiThac != string.Empty) || !TINHKIEM.VietLien(Setting.KoKhaiThac).Contains(TINHKIEM.VietLien(Name))))
				{
					if (TINHKIEM.IsDuoc(Name) == -1)
					{
						return Game.Is69DO;
					}
					return true;
				}
				return false;
			}
		}

		public bool IsMonter
		{
			get
			{
				if (Game.TLBB.MapId > 2 && HP > 0f && (Menpai < -1 || Menpai >= 16) && Menpai != 32 && Menpai != 21 && Menpai != 22 && Menpai != 19 && Menpai != 37 && Menpai <= 40 && !GAMEDIC.BoQua.Contains(Name) && (Game.TLBB.MapId != MAP.YenTuO || !GAMEDIC.YenTuOBoQua.Contains(Name)) && (!Global.IsBoQua || !(Name.Trim() != "")) && !Name.Contains("Tháp") && !TINHKIEM.VietLien(Name).Contains("tieulang"))
				{
					if (Name.Contains("Niên Thú"))
					{
						return Game.TLBB.MapId == 547;
					}
					return true;
				}
				return false;
			}
		}

		public bool IsBoQua => TINHKIEM.VietLien(Setting.BoQua).Contains(TINHKIEM.VietLien(Name));

		public bool IsPlayer
		{
			get
			{
				if (Menpai < 1 || Menpai > 9)
				{
					if (Menpai != 32)
					{
						return Menpai == 37;
					}
					return true;
				}
				return true;
			}
		}

		public bool IsPet
		{
			get
			{
				if (Type == "40600000")
				{
					return TrueId.Contains("FFFFFFFF");
				}
				return false;
			}
		}

		public bool IsNPC
		{
			get
			{
				if (!(Type == "3FE66666"))
				{
					return Type == "3F4CCCCD";
				}
				return true;
			}
		}

		public HashSet<int> BuffAddress => EnumGameBuff(Memory.Read(Object + Game.Address.ObjectBuff));

		public void ChangeName(string name)
		{
			Game.Memory.WriteUnicodeString(name, NameAddress);
		}

		public GameObject(Game game, int address)
		{
			Game = game;
			Memory = game.Memory;
			ADD = game.Address;
			Address = address;
			InfoAddress = game.Memory.Read(address + ADD.ObjectObject, ADD.ObjectInfo);
			Id = Memory.Read(address + ADD.ObjectId);
			Object = Memory.Read(address + ADD.ObjectObject);
			Class = Memory.Read(Object);
			X = Memory.ReadFloat(Object + ADD.ObjectX);
			Y = Memory.ReadFloat(Object + ADD.ObjectY);
			if (Id >= 0 && (int)X >= 0 && (int)Y >= 0)
			{
				if (ADD.GameType == 2)
				{
					State = Memory.Read(Object + 344);
				}
				else
				{
					State = Memory.Read(Object + ADD.State);
				}
				AtkToId = Memory.Read(Object + ADD.ObjectAtkToId);
				AtkById = Memory.Read(Object + ADD.ObjectAtkById);
				byte[] array = new byte[ADD.ObjectPartyId + 4];
				Memory.ReadProcessMemory(game.Memory.Id, InfoAddress, array, array.Length, 0);
				HP = BitConverter.ToSingle(array, ADD.ObjectHP);
				MP = BitConverter.ToSingle(array, ADD.ObjectMP);
				if (ADD.GameType == 1)
				{
					TrueId = BitConverter.ToInt64(array, ADD.ObjectTrueId).ToString("X8");
				}
				else
				{
					TrueId = BitConverter.ToInt32(array, ADD.ObjectTrueId).ToString("X8");
				}
				if (ADD.GameType == 1)
				{
					Belong = BitConverter.ToInt64(array, ADD.ObjectBelong).ToString("X8");
				}
				else
				{
					Belong = BitConverter.ToInt32(array, ADD.ObjectBelong).ToString("X8");
				}
				Menpai = BitConverter.ToInt32(array, ADD.ObjectMenpai);
				Type = BitConverter.ToInt32(array, ADD.ObjectType).ToString("X8");
				Lvl = BitConverter.ToInt32(array, ADD.ObjectLvl);
				PartyId = BitConverter.ToInt32(array, ADD.ObjectPartyId);
				Title = Memory._ReadString(InfoAddress + ADD.ObjectTitle).Trim();
				Ride = BitConverter.ToInt32(array, ADD.ObjectRide);
				if (IsTaiNguyen)
				{
					Memory memory = game.Memory;
					Name = memory.ReadString(new int[3]
					{
						Object + ADD.ObjectTaiNguyenName,
						4,
						0
					}).Trim();
				}
				else
				{
					Name = Memory._ReadString(InfoAddress + ADD.ObjectName).Trim();
				}
				QDId = Memory.Read2Byte(InfoAddress + 9852);
			}
		}

		public float GetDistance(float x, float y)
		{
			return (float)Math.Sqrt(Math.Pow(x - X, 2.0) + Math.Pow(y - Y, 2.0));
		}

		public HashSet<int> EnumGameBuff(int address)
		{
			HashSet<int> hashSet = new HashSet<int>();
			NextGameBuff(address, hashSet);
			hashSet.Remove(address);
			return hashSet;
		}

		public HashSet<int> EnumGameBuff(int[] addresses)
		{
			int num = Game.Memory.Read(addresses);
			if (num > 0)
			{
				return EnumGameBuff(num);
			}
			return new HashSet<int>();
		}

		private void NextGameBuff(int address, HashSet<int> hash)
		{
			if (address > 0 && hash.Count <= 200 && !hash.Contains(address))
			{
				hash.Add(address);
				NextGameBuff(Game.Memory.Read(address), hash);
				NextGameBuff(Game.Memory.Read(address + 4), hash);
				NextGameBuff(Game.Memory.Read(address + 8), hash);
			}
		}
	}
}
