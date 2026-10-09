using System.Collections.Generic;

namespace TinhKiemAuto
{
	internal class Shop
	{
		public int Address;

		public int Class;

		public int DefineId;

		public string Name;

		public int Index;

		public string TypeName;

		public string ClearName => TINHKIEM.VietLien(Name);

		public override string ToString()
		{
			return string.Empty + "Address: " + Address.ToString("X8") + "\r\nClass: " + Class.ToString("X8") + "\r\nIndex: " + Index + "\r\nTypeName: " + TypeName + "\r\n" + Name;
		}

		public static List<Shop> Enum(Game game)
		{
			List<Shop> list = new List<Shop>();
			int num = game.Memory.Read(game.Address.BaseShopItem);
			for (int i = 0; i < 80; i++)
			{
				if (game.Memory.Read(num + i * 4) != 0)
				{
					Shop shop = new Shop();
					shop.Address = game.Memory.Read(num + i * 4);
					shop.Class = game.Memory.Read(shop.Address);
					shop.DefineId = game.Memory.Read(shop.Address + 8);
					if (shop.Class == game.Address.PacketType2)
					{
						shop.Name = game.Memory.ReadString(game.Memory.Read(shop.Address + 40, 24));
						shop.TypeName = game.Memory.ReadString(game.Memory.Read(shop.Address + 40, 80));
					}
					else
					{
						shop.Name = game.Memory.ReadString(game.Memory.Read(shop.Address + 40, 40));
						shop.TypeName = game.Memory.ReadString(game.Memory.Read(shop.Address + 40, 88));
					}
					shop.Index = game.Memory.Read(shop.Address + 16);
					list.Add(shop);
				}
			}
			return list;
		}
	}
}
