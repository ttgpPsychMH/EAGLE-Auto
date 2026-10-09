namespace TinhKiemAuto
{
	internal class SequentialMirrorSelector : IMirrorSelector
	{
		private Downloader downloader;

		private int queryMirrorCount;

		public void Init(Downloader downloader)
		{
			queryMirrorCount = 0;
			this.downloader = downloader;
		}

		public ResourceLocation GetNextResourceLocation()
		{
			if (downloader.Mirrors == null || downloader.Mirrors.Count == 0)
			{
				return downloader.ResourceLocation;
			}
			lock (downloader.Mirrors)
			{
				if (queryMirrorCount >= downloader.Mirrors.Count)
				{
					queryMirrorCount = 0;
					return downloader.ResourceLocation;
				}
				return downloader.Mirrors[queryMirrorCount++];
			}
		}
	}
}
