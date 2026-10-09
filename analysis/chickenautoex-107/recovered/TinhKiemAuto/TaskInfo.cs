using System.Collections.Generic;

namespace TinhKiemAuto
{
	internal class TaskInfo
	{
		public int Address;

		public int Class;

		public int Id;

		private Game game;

		public int TrangThai { get; set; }

		public string Info => string.Empty + "Address: " + Address.ToString("X8");

		public TaskInfo(Game game)
		{
			this.game = game;
		}

		public void SetTrangThai(int value)
		{
			game.Memory.Write(Address + 8, value);
		}

		public static List<TaskInfo> Enum(Game game)
		{
			List<TaskInfo> list = new List<TaskInfo>();
			int num = game.Memory.Read(game.Address.TaskInfoBase);
			for (int i = 0; i < 80; i++)
			{
				if (game.Memory.Read(num + 5 + i * 41) != 0)
				{
					TaskInfo taskInfo = new TaskInfo(game);
					taskInfo.Address = num + 5 + i * 41;
					taskInfo.Class = game.Memory.Read(taskInfo.Address);
					taskInfo.Id = game.Memory.Read(taskInfo.Address + 4);
					taskInfo.TrangThai = game.Memory.Read(taskInfo.Address + 8);
					if (taskInfo.Id > 0)
					{
						list.Add(taskInfo);
					}
				}
			}
			return list;
		}
	}
}
