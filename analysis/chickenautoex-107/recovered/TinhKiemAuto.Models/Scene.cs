namespace TinhKiemAuto.Models
{
	public class Scene
	{
		public readonly int sceneId;

		public readonly string sceneName;

		public Scene(int sceneId, string sceneName)
		{
			this.sceneId = sceneId;
			this.sceneName = sceneName;
		}
	}
}
