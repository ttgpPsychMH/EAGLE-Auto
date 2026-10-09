using System.Collections.Generic;

namespace TinhKiemAuto
{
	public class GameControl
	{
		public int Address;

		public int Object;

		public int Class;

		public int Id;

		public string Name;

		public string Type;

		public int PacketId;

		public bool IsSkill
		{
			get
			{
				if (!Type.Contains("FightSkillXinShou_12"))
				{
					if (!Type.Contains("FightSkill") && !Type.Contains("FabaoSkill") && !Type.Contains("MiJiSkill") && !Type.Contains("fuqiskill") && !Type.Contains("Shoes2_5") && !Type.Contains("RideHeader1_1") && !Type.Contains("MenpaiLiveSkill2_7") && (!Type.Contains("WuhunSkill") || PacketId <= 100) && !Type.Contains("TaskTools2_13") && !Type.Contains("PetSkill2_4") && !Type.Contains("CommonLiveSkill2_2") && !Type.Contains("CircularTaskTool43_2") && !Type.Contains("Shoes2_4"))
					{
						return Type.Contains("TaskTools4_1");
					}
					return true;
				}
				return false;
			}
		}

		public override string ToString()
		{
			return string.Empty + "Address: " + Address.ToString("X8") + "\r\nId: " + Id + "\r\nObject: " + Object.ToString("X8") + "\r\nClass: " + Class.ToString("X8") + "\r\nType: " + Type + "\r\nPacketId: " + PacketId;
		}

		public static List<GameControl> Enum(Game game)
		{
			List<GameControl> list = new List<GameControl>();
			foreach (int item in EnumGameControl(game.Address.ActionBase, game))
			{
				GameControl gameControl = new GameControl();
				gameControl.Address = item;
				gameControl.Object = game.Memory.Read(item + 16);
				gameControl.Class = game.Memory.Read(gameControl.Object);
				gameControl.Id = game.Memory.Read(gameControl.Object + 4);
				gameControl.Name = game.Memory._ReadString(gameControl.Object + 12);
				gameControl.Type = game.Memory._ReadString(gameControl.Object + 40);
				gameControl.PacketId = game.Memory.Read(gameControl.Object + 92);
				list.Add(gameControl);
			}
			return list;
		}

		private static HashSet<int> EnumGameControl(int address, Game game)
		{
			HashSet<int> hashSet = new HashSet<int>();
			NextGameControl(address, hashSet, game);
			hashSet.Remove(address);
			return hashSet;
		}

		public static HashSet<int> EnumGameControl(int[] addresses, Game game)
		{
			int num = game.Memory.Read(addresses);
			if (num > 0)
			{
				return EnumGameControl(num, game);
			}
			return new HashSet<int>();
		}

		private static void NextGameControl(int address, HashSet<int> hash, Game game)
		{
			if (hash.Count <= 10000 && !hash.Contains(address))
			{
				hash.Add(address);
				int num = game.Memory.Read(address);
				if (num > 0)
				{
					NextGameControl(num, hash, game);
				}
				int num2 = game.Memory.Read(address + 4);
				if (num2 > 0)
				{
					NextGameControl(num2, hash, game);
				}
				int num3 = game.Memory.Read(address + 8);
				if (num3 > 0)
				{
					NextGameControl(num3, hash, game);
				}
			}
		}
	}
}
