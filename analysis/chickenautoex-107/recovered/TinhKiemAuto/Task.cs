using System.Collections.Generic;

namespace TinhKiemAuto
{
	public class Task
	{
		public int Address;

		public int Id;

		public string Name;

		public string MucTieu;

		public int Lvl;

		public int X;

		public int Y;

		public string NPCNhanTask;

		public bool Completed;

		private Game game;

		public int Complete { get; set; }

		public string ClearName => TINHKIEM.VietLien(Name);

		public int TaskInfoAddress { get; set; }

		public int CountEx1 => game.Memory.Read2Byte(TaskInfoAddress + 54);

		public int CountEx2 => game.Memory.Read2Byte(TaskInfoAddress + 58);

		public int CountEx3 => game.Memory.Read2Byte(TaskInfoAddress + 62);

		public int Count1
		{
			get
			{
				return game.Memory.Read(TaskInfoAddress + 13);
			}
			set
			{
				game.Memory.Write(TaskInfoAddress + 13, 1);
			}
		}

		public int Count2
		{
			get
			{
				return game.Memory.Read(TaskInfoAddress + 13 + 4);
			}
			set
			{
				game.Memory.Write(TaskInfoAddress + 13 + 4, 1);
			}
		}

		public int Count3
		{
			get
			{
				return game.Memory.Read(TaskInfoAddress + 13 + 8);
			}
			set
			{
				game.Memory.Write(TaskInfoAddress + 13 + 8, 1);
			}
		}

		public Task(Game game)
		{
			this.game = game;
		}

		public void SetComplete()
		{
			foreach (TaskInfo item in TaskInfo.Enum(game))
			{
				if (item.Id == Id)
				{
					item.SetTrangThai(256);
					break;
				}
			}
		}

		public override string ToString()
		{
			string empty = string.Empty;
			empty = empty + "ID: " + Id.ToString("X8");
			empty += "\r\n";
			empty = empty + "Address: " + Address.ToString("X8");
			empty += "\r\n";
			empty = empty + "Name: " + Name;
			empty += "\r\n";
			empty = empty + "MucTieu: " + MucTieu;
			empty += "\r\n";
			empty = empty + "Lvl: " + Lvl;
			empty += "\r\n";
			empty = empty + "XY: " + X + "," + Y;
			empty += "\r\n";
			empty = empty + "NPC: " + NPCNhanTask;
			empty += "\r\n";
			empty = empty + "TaksInfoAddress: " + TaskInfoAddress.ToString("X8");
			empty += "\r\n";
			empty = empty + "Count: " + Count1 + Count2 + Count3;
			empty += "\r\n";
			empty = empty + "CountEx: " + CountEx1 + CountEx2 + CountEx3;
			empty += "\r\n";
			empty = ((!Completed) ? (empty + "Doing") : (empty + "Completed"));
			empty += "\r\n";
			return empty + "Complete: " + Complete;
		}

		public static bool Have(Game game, string name)
		{
			game.LUA.OpenWindowMissionTrack();
			foreach (int item in EnumTask(game.Address.TaskBase, game))
			{
				if (new Task(game)
				{
					Name = game.Memory._ReadString(item + 224)
				}.Name.Contains(name))
				{
					return true;
				}
			}
			return false;
		}

		public static List<Task> Enum(Game game)
		{
			game.LUA.OpenWindowMissionTrack();
			List<int> list = EnumTask(game.Address.TaskBase, game);
			List<Task> list2 = new List<Task>();
			List<TaskInfo> list3 = TaskInfo.Enum(game);
			foreach (int item in list)
			{
				Task task = new Task(game);
				task.Address = item;
				task.Lvl = game.Memory.Read(item + 20);
				task.Id = game.Memory.Read(item + 12);
				task.X = game.Memory.Read(item + 44);
				task.Y = game.Memory.Read(item + 48);
				task.Name = game.Memory._ReadString(item + 224);
				task.NPCNhanTask = game.Memory._ReadString(item + 60);
				task.MucTieu = game.Memory._ReadString(item + 200);
				if (task.Id <= 0)
				{
					continue;
				}
				list2.Add(task);
				foreach (TaskInfo item2 in list3)
				{
					if (item2.Id == task.Id)
					{
						if (item2.TrangThai >= 256)
						{
							task.Completed = true;
						}
						task.Complete = item2.TrangThai;
						task.TaskInfoAddress = item2.Address;
					}
				}
			}
			return list2;
		}

		public static List<int> EnumTask(int address, Game game)
		{
			List<int> list = new List<int>();
			NextTask(address, list, game);
			return list;
		}

		public static List<int> EnumTask(int[] addresses, Game game)
		{
			int num = game.Memory.Read(addresses);
			if (num > 0)
			{
				return EnumTask(num, game);
			}
			return new List<int>();
		}

		private static void NextTask(int address, List<int> listAddress, Game game)
		{
			if (listAddress.Count <= 1000 && !listAddress.Contains(address))
			{
				listAddress.Add(address);
				int num = game.Memory.Read(address);
				if (num > 0)
				{
					NextTask(num, listAddress, game);
				}
				int num2 = game.Memory.Read(address + 4);
				if (num2 > 0)
				{
					NextTask(num2, listAddress, game);
				}
				int num3 = game.Memory.Read(address + 8);
				if (num3 > 0)
				{
					NextTask(num3, listAddress, game);
				}
			}
		}
	}
}
