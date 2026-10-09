using System.Collections.Generic;

namespace TinhKiemAuto
{
	public class GameObjects
	{
		private Game game;

		public List<GameObject> TaiNguyen = new List<GameObject>();

		public List<GameObject> All = new List<GameObject>();

		public List<GameObject> LootPacket = new List<GameObject>();

		public List<GameObject> Party = new List<GameObject>();

		public List<GameObject> Monter = new List<GameObject>();

		public List<GameObject> MyMonter = new List<GameObject>();

		public List<GameObject> UnBelongMonter = new List<GameObject>();

		public List<GameObject> Pk = new List<GameObject>();

		public GameObject Self;

		public GameObject Key;

		public GameObject Target;

		public GameObject PartyMinHP;

		public string ToaDoTaiNguyen = "";

		private string MineId = "";

		public List<GameObject> AllNpc
		{
			get
			{
				List<GameObject> list = new List<GameObject>();
				foreach (GameObject item in All)
				{
					if (item.IsNPC)
					{
						list.Add(item);
					}
				}
				return list;
			}
		}

		public List<GameObject> AllPlayer
		{
			get
			{
				List<GameObject> list = new List<GameObject>();
				foreach (GameObject item in All)
				{
					if (item.IsPlayer)
					{
						list.Add(item);
					}
				}
				return list;
			}
		}

		public List<GameObject> NearMonter5m
		{
			get
			{
				List<GameObject> list = new List<GameObject>();
				foreach (GameObject item in Monter)
				{
					if (item.GetDistance(game.CharX, game.CharY) < 5f)
					{
						list.Add(item);
					}
				}
				return list;
			}
		}

		public List<GameObject> NearMonter15m
		{
			get
			{
				List<GameObject> list = new List<GameObject>();
				foreach (GameObject item in Monter)
				{
					if (TINHKIEM.GetDistance(game.CharX, game.CharY, item.X, item.Y) < 15f)
					{
						list.Add(item);
					}
				}
				return list;
			}
		}

		public List<GameObject> NearMonter20m
		{
			get
			{
				List<GameObject> list = new List<GameObject>();
				foreach (GameObject item in Monter)
				{
					if (TINHKIEM.GetDistance(game.CharX, game.CharY, item.X, item.Y) < 20f)
					{
						list.Add(item);
					}
				}
				return list;
			}
		}

		public List<GameObject> NearMonter18m
		{
			get
			{
				List<GameObject> list = new List<GameObject>();
				foreach (GameObject item in Monter)
				{
					if (TINHKIEM.GetDistance(game.CharX, game.CharY, item.X, item.Y) < 18f)
					{
						list.Add(item);
					}
				}
				return list;
			}
		}

		public GameObject NearestMonter
		{
			get
			{
				float num = 9999f;
				GameObject result = null;
				foreach (GameObject item in Monter)
				{
					item.DistanceEx = TINHKIEM.GetDistance(game.CharX, game.CharY, item.X, item.Y);
					if (item.DistanceEx < num)
					{
						num = item.DistanceEx;
						result = item;
					}
				}
				return result;
			}
		}

		public List<GameObject> NearMonter9m
		{
			get
			{
				List<GameObject> list = new List<GameObject>();
				foreach (GameObject item in Monter)
				{
					if (TINHKIEM.GetDistance(game.CharX, game.CharY, item.X, item.Y) < 10f)
					{
						list.Add(item);
					}
				}
				return list;
			}
		}

		public List<GameObject> NearMonter12m
		{
			get
			{
				List<GameObject> list = new List<GameObject>();
				foreach (GameObject item in Monter)
				{
					if (TINHKIEM.GetDistance(game.CharX, game.CharY, item.X, item.Y) < 12f)
					{
						list.Add(item);
					}
				}
				return list;
			}
		}

		public List<GameObject> Near5m
		{
			get
			{
				List<GameObject> list = new List<GameObject>();
				foreach (GameObject item in All)
				{
					if (TINHKIEM.GetDistance(game.CharX, game.CharY, item.X, item.Y) < 5f)
					{
						list.Add(item);
					}
				}
				return list;
			}
		}

		public List<GameObject> Near20m
		{
			get
			{
				List<GameObject> list = new List<GameObject>();
				foreach (GameObject item in All)
				{
					if (TINHKIEM.GetDistance(game.CharX, game.CharY, item.X, item.Y) < 20f)
					{
						list.Add(item);
					}
				}
				return list;
			}
		}

		public bool TargetIsMine
		{
			get
			{
				if (Target != null)
				{
					return MineId.Contains(Target.Belong);
				}
				return false;
			}
		}

		public bool Have(string name)
		{
			foreach (GameObject item in All)
			{
				if (TINHKIEM.VietLien(item.Name).Contains(TINHKIEM.VietLien(name)))
				{
					return true;
				}
			}
			return false;
		}

		public bool HaveEx(string name)
		{
			foreach (GameObject item in All)
			{
				if (item.CleanName == name)
				{
					return true;
				}
			}
			return false;
		}

		public bool HaveMonter(string name)
		{
			foreach (GameObject item in All)
			{
				if (item.IsMonter && TINHKIEM.VietLien(item.Name).Contains(TINHKIEM.VietLien(name)))
				{
					return true;
				}
			}
			return false;
		}

		public bool HaveTaiNguyen(string name)
		{
			foreach (GameObject item in All)
			{
				if (item.IsTaiNguyen && TINHKIEM.VietLien(item.Name).Contains(TINHKIEM.VietLien(name)))
				{
					return true;
				}
			}
			return false;
		}

		public List<GameObject> NearMonter(float x, float y, float distance)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject item in Monter)
			{
				if (item.GetDistance(x, y) < distance)
				{
					list.Add(item);
				}
			}
			if (game.TLBB.MapId == MAP.ViemMaSon)
			{
				foreach (GameObject item2 in All)
				{
					if (item2.GetDistance(x, y) < distance && item2.Menpai == 12 && item2.IsNPC)
					{
						list.Add(item2);
					}
				}
			}
			return list;
		}

		public GameObjects(Game game)
		{
			this.game = game;
			Read();
		}

		public void Read()
		{
			Self = (Key = (Target = (PartyMinHP = null)));
			All.Clear();
			LootPacket.Clear();
			Party.Clear();
			Monter.Clear();
			MyMonter.Clear();
			UnBelongMonter.Clear();
			TaiNguyen.Clear();
			ToaDoTaiNguyen = "";
			foreach (int item in EnumGameObject(game.Address.FirstObject))
			{
				GameObject gameObject = new GameObject(game, item);
				if (gameObject.Name == null || (int)gameObject.X == 0 || (int)gameObject.Y == 0)
				{
					continue;
				}
				if (gameObject.IsPlayer)
				{
					string[] array = Setting.Leader.Split('\n');
					foreach (string text in array)
					{
						if (!(text.Trim() == "") && TINHKIEM.VietLien(text) == TINHKIEM.VietLien(gameObject.Name) && (Self == null || gameObject != Self))
						{
							Key = gameObject;
							break;
						}
					}
				}
				All.Add(gameObject);
				if (gameObject.Id == game.TargetId)
				{
					Target = gameObject;
				}
				if (gameObject.IsLootPacket)
				{
					LootPacket.Add(gameObject);
				}
				if (gameObject.IsTaiNguyen)
				{
					TaiNguyen.Add(gameObject);
					ToaDoTaiNguyen = ToaDoTaiNguyen + gameObject.X + "," + gameObject.Y + "," + gameObject.Belong + "-";
				}
				if (gameObject.TrueId == game.TLBB.Id && game.TLBB.Online)
				{
					Self = gameObject;
					game.CharX = Self.X;
					game.CharY = Self.Y;
					game.RoundX = Self.RoundX;
					game.RoundY = Self.RoundY;
					game.IsRide = Self.Ride != -1;
				}
				if (game.TLBB.Lvl >= 10 || !(gameObject.Title.Trim() != ""))
				{
					if (gameObject.IsMonter && !gameObject.IsBoQua)
					{
						Monter.Add(gameObject);
					}
					if (gameObject.Belong.Contains("FFFFFFFF") && gameObject.IsMonter)
					{
						UnBelongMonter.Add(gameObject);
					}
				}
			}
			MineId = "0000000000000000FFFFFFFFFFFFFFFF";
			if (Self != null)
			{
				Party.Add(Self);
				MineId += Self.TrueId;
				PartyMinHP = Self;
				foreach (GameObject item2 in All)
				{
					if (item2 == Self)
					{
						continue;
					}
					if (game.Enemy.Contains("-" + item2.Name + "-") && Global.AutoPk)
					{
						Pk.Add(item2);
					}
					if ((double)Self.HP > 0.3 && Self.QDId != 65535 && game.Address.GameType == 1 && Global.BuffQuanDoan)
					{
						if (item2.HP > 0f && Self.QDId == item2.QDId && item2.IsPlayer)
						{
							Party.Add(item2);
							MineId += item2.TrueId;
							if ((double)Self.HP > 0.3 && item2.HP < PartyMinHP.HP)
							{
								PartyMinHP = item2;
							}
						}
					}
					else if (Self.PartyId != -1 && item2.PartyId == Self.PartyId)
					{
						if (item2.HP > 0f)
						{
							Party.Add(item2);
							MineId += item2.TrueId;
							if ((double)Self.HP > 0.3 && item2.HP < PartyMinHP.HP)
							{
								PartyMinHP = item2;
							}
							if (item2.TrueId == game.TLBB.KeyId && Key == null)
							{
								Key = item2;
							}
						}
					}
					else if (Setting.CheckBuff(item2.TrueId.ToLower()) && item2.Name != "" && item2.IsPlayer && item2.HP > 0f)
					{
						Party.Add(item2);
						if ((double)Self.HP > 0.3 && item2.HP < PartyMinHP.HP)
						{
							PartyMinHP = item2;
						}
					}
					if (Global.BuffPet && item2.IsPet && item2.Name == game.TLBB.PetName && item2.HP > 0f)
					{
						Party.Add(item2);
						if ((double)Self.HP > 0.3 && item2.HP < PartyMinHP.HP)
						{
							PartyMinHP = item2;
						}
					}
					if (game.TLBB.MapId == MAP.YenTuO && (item2.CleanName == "hodienbao" || item2.CleanName == "tienhoanhvu") && (double)item2.HP < 0.5)
					{
						PartyMinHP = item2;
					}
					if (game.TLBB.MapId == MAP.ThanhThuSonPhuBan && TINHKIEM.VietLien(item2.Title) == "linhthu" && (double)item2.HP < 0.5)
					{
						PartyMinHP = item2;
					}
				}
			}
			foreach (GameObject item3 in Monter)
			{
				if (MineId.Contains(item3.Belong) && item3.IsMonter)
				{
					MyMonter.Add(item3);
				}
			}
		}

		public HashSet<int> EnumGameObject(int address)
		{
			HashSet<int> hashSet = new HashSet<int>();
			NextGameObject(address, hashSet);
			return hashSet;
		}

		public HashSet<int> EnumGameObject(int[] addresses)
		{
			int num = game.Memory.Read(addresses);
			if (num > 0)
			{
				return EnumGameObject(num);
			}
			return new HashSet<int>();
		}

		private void NextGameObject(int address, HashSet<int> hash)
		{
			if (address > 0 && hash.Count <= 10000 && !hash.Contains(address))
			{
				hash.Add(address);
				NextGameObject(game.Memory.Read(address), hash);
				NextGameObject(game.Memory.Read(address + 8), hash);
			}
		}
	}
}
