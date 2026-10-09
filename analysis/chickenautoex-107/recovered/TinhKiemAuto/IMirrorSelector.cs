namespace TinhKiemAuto
{
	internal interface IMirrorSelector
	{
		void Init(Downloader downloader);

		ResourceLocation GetNextResourceLocation();
	}
}
