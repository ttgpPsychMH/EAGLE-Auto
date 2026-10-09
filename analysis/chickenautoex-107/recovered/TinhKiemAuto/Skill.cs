using System;
using System.Collections.Generic;
using TinhKiemAuto.Controllers;

namespace TinhKiemAuto
{
	public class Skill
	{
		public int Address;

		public int PacketId;

		public int DelayOffset;

		public string Name = "";

		private bool use;

		public Game game;

		public bool UsePK { get; set; }

		public bool UserBuff { get; set; }

		public bool Use
		{
			get
			{
				return use;
			}
			set
			{
				use = value;
			}
		}

		public static bool IsBuffSkill(int ID)
		{
			SkillModel skillByID = SkillData.GetSkillByID(ID);
			if (skillByID != null)
			{
				if (skillByID.skillTargetType == "Enemy")
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public Skill(Game game)
		{
			this.game = game;
		}

		public static bool IsBase(int id)
		{
			return "-311-341-371-281-401-431-461-491-521-760-2900-;".Contains("-" + id + "-");
		}

		public static bool IsBand(int id)
		{
			return "-0-1-22-21-35-34-37-245-;".Contains("-" + id + "-");
		}

		public override string ToString()
		{
			return "Address: " + Address.ToString("X8") + "\r\nDelayOffset: " + DelayOffset.ToString("X8") + "\r\n\r\n\r\n";
		}

		public static List<Skill> Enum(Game game)
		{
			List<Skill> list = new List<Skill>();
			new List<int>();
			int[] array = game.Address.CharBase;
			Array.Resize(ref array, array.Length + 2);
			if (game.Address.GameType == 1)
			{
				array[array.Length - 2] = 10344;
			}
			else if (game.Address.GameType == 2)
			{
				array[array.Length - 2] = 1980;
			}
			else
			{
				array[array.Length - 2] = 2768;
			}
			array[array.Length - 1] = 4;
			int address = game.Memory.Read(array);
			List<int> list2 = new List<int>();
			try
			{
				NextSkill(address, list2, game);
			}
			catch
			{
			}
			foreach (int item in list2)
			{
				Skill skill = new Skill(game);
				skill.Address = item;
				skill.PacketId = game.Memory.Read(item + 12);
				skill.DelayOffset = game.Memory.Read(item + 16 + 4, 64) * 12;
				if (skill.PacketId < 4096)
				{
					list.Add(skill);
				}
			}
			if (list.Count > 100)
			{
				list.Clear();
			}
			return list;
		}

		public static void NextSkill(int address, List<int> listAddress, Game game)
		{
			if (!listAddress.Contains(address) && listAddress.Count <= 110)
			{
				listAddress.Add(address);
				NextSkill(game.Memory.Read(address), listAddress, game);
				NextSkill(game.Memory.Read(address + 4), listAddress, game);
				NextSkill(game.Memory.Read(address + 8), listAddress, game);
			}
		}
	}
}
