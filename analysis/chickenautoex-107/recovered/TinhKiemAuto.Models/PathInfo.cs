namespace TinhKiemAuto.Models
{
	public class PathInfo
	{
		public readonly bool isNPC;

		public readonly int x;

		public readonly int y;

		public readonly int idNext;

		public readonly string NpcName;

		public PathInfo(bool isNPC, int x, int y, int idNext, string npcname)
		{
			this.isNPC = isNPC;
			this.x = x;
			this.y = y;
			this.idNext = idNext;
			NpcName = npcname;
		}
	}
}
