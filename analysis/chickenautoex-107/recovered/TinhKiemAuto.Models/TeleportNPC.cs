namespace TinhKiemAuto.Models
{
	public class TeleportNPC
	{
		public readonly string npcName;

		public readonly int teleportationCost;

		public readonly int posX;

		public readonly int posY;

		public TeleportNPC(string npcName, int teleportationCost, int posX, int posY)
		{
			this.npcName = npcName;
			this.teleportationCost = teleportationCost;
			this.posX = posX;
			this.posY = posY;
		}
	}
}
