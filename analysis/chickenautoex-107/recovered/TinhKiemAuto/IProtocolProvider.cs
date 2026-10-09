using System.IO;

namespace TinhKiemAuto
{
	internal interface IProtocolProvider
	{
		void Initialize(Downloader downloader);

		Stream CreateStream(ResourceLocation rl, long initialPosition, long endPosition);

		RemoteFileInfo GetFileInfo(ResourceLocation rl, out Stream stream);
	}
}
