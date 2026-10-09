namespace TinhKiemAuto.Models
{
	public class NearScene
	{
		public readonly Scene scene;

		public readonly int nearScenePortalX;

		public readonly int nearScenePortalY;

		public NearScene(Scene scene, int portalX, int portalY)
		{
			this.scene = scene;
			nearScenePortalX = portalX;
			nearScenePortalY = portalY;
		}
	}
}
