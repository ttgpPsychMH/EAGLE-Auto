using System.Collections.Generic;

namespace TinhKiemAuto.Models
{
	public class SceneInfo
	{
		public readonly Scene scene;

		public readonly List<NearScene> nearScenes = new List<NearScene>();

		public readonly List<TeleportNPC> teleportNPCs = new List<TeleportNPC>();

		public SceneInfo(Scene scene)
		{
			this.scene = scene;
		}

		public void AddNearScene(Scene scene, int portalX, int portalY)
		{
			NearScene item = new NearScene(scene, portalX, portalY);
			nearScenes.Add(item);
		}

		public void AddTeleportNPC(TeleportNPC npc)
		{
			if (npc.npcName.CompareTo("N/A") != 0)
			{
				teleportNPCs.Add(npc);
			}
		}
	}
}
