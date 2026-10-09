using System.Collections.Generic;

namespace TinhKiemAuto
{
	public class QuestFrame
	{
		private Game game;

		public int Address;

		public int Id;

		public int StrOptionExtra1;

		public int StrOptionExtra2;

		public string Name;

		public string ClearName => TINHKIEM.VietLien(Name);

		public string MD => TINHKIEM.Hasher.MD5(StrOptionExtra1.ToString() + StrOptionExtra2).Substring(0, 3);

		public bool IsClickPhuBanMonPhai
		{
			get
			{
				if (StrOptionExtra1 != 200005 && StrOptionExtra1 != 118001 && StrOptionExtra1 != 119005 && StrOptionExtra1 != 119001 && StrOptionExtra1 != 200001 && StrOptionExtra1 != 13035 && StrOptionExtra1 != 9044 && StrOptionExtra1 != 10051 && StrOptionExtra1 != 16035 && StrOptionExtra1 != 14035 && StrOptionExtra1 != 9035 && StrOptionExtra1 != 17035 && StrOptionExtra1 != 15035 && StrOptionExtra1 != 12035 && StrOptionExtra1 != 11035)
				{
					return StrOptionExtra1 == 10035;
				}
				return true;
			}
		}

		public QuestFrame(Game game)
		{
			this.game = game;
		}

		public void ClickAll()
		{
			foreach (QuestFrame item in Enum(game))
			{
				game.QuestFrameOptionClicked(item);
			}
		}

		public bool Click(int option1, int option2)
		{
			foreach (QuestFrame item in Enum(game))
			{
				if (item.StrOptionExtra1 == option1 && item.StrOptionExtra2 == option2)
				{
					game.QuestFrameOptionClicked(item);
					return true;
				}
			}
			return false;
		}

		public bool Click(string name)
		{
			foreach (QuestFrame item in Enum(game))
			{
				if (item.Name == name)
				{
					game.QuestFrameOptionClicked(item);
					return true;
				}
			}
			return false;
		}

		public void Close()
		{
			game.PostMessage(0, 122);
		}

		public static int GetId(Game game)
		{
			return game.Memory.Read(game.Address.DialogBase[0], 88);
		}

		public override string ToString()
		{
			return string.Empty + "Address: " + Address.ToString("X8") + "\r\nName: " + Name + "\r\nStrOptionExtra1: " + StrOptionExtra1 + "\r\nStrOptionExtra2: " + StrOptionExtra2 + "\r\nMD: " + MD;
		}

		public static void ClickPhuBanMonPhai(Game game)
		{
			foreach (QuestFrame item in Enum(game))
			{
				if (item.IsClickPhuBanMonPhai)
				{
					game.QuestFrameOptionClicked(item);
					break;
				}
			}
		}

		public static void ClickOut(Game game)
		{
			foreach (QuestFrame item in Enum(game))
			{
				if (((item.StrOptionExtra1 == 119001 || item.StrOptionExtra1 == 118001) && item.StrOptionExtra2 == 1) || item.StrOptionExtra1 == 119005 || item.StrOptionExtra1 == 118012 || item.StrOptionExtra1 == 2108)
				{
					game.QuestFrameOptionClicked(item);
					break;
				}
			}
		}

		public static List<QuestFrame> Enum(Game game)
		{
			List<QuestFrame> list = new List<QuestFrame>();
			int num = game.Memory.Read(game.Address.DialogBase) + 8;
			QuestFrame questFrame = new QuestFrame(game);
			questFrame.Address = num;
			questFrame.StrOptionExtra1 = game.Memory.Read(questFrame.Address + 272);
			questFrame.StrOptionExtra2 = game.Memory.Read(questFrame.Address + 8);
			questFrame.Name = game.Memory.ReadString(questFrame.Address + 14);
			list.Add(questFrame);
			for (int i = 1; i < 12; i++)
			{
				int num2 = num + 280 * i;
				questFrame = new QuestFrame(game);
				questFrame.Address = num2;
				questFrame.Id = i;
				questFrame.StrOptionExtra1 = game.Memory.Read(questFrame.Address + 272);
				questFrame.StrOptionExtra2 = game.Memory.Read(questFrame.Address + 8);
				questFrame.Name = game.Memory.ReadString(num2 + 14).Trim();
				if (questFrame.StrOptionExtra1 > 0 && questFrame.Name.Trim() != "")
				{
					list.Add(questFrame);
				}
			}
			return list;
		}

		public static int GetCount(Game game)
		{
			int num = 0;
			foreach (QuestFrame item in Enum(game))
			{
				if (item.Name.Trim() != "")
				{
					num++;
				}
			}
			return num;
		}

		public static string All(Game game)
		{
			string text = "";
			foreach (QuestFrame item in Enum(game))
			{
				text += item.Name;
			}
			return text;
		}
	}
}
