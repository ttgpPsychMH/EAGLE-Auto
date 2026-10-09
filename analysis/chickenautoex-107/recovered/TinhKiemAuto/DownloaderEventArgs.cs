using System;

namespace TinhKiemAuto
{
	internal class DownloaderEventArgs : EventArgs
	{
		private Downloader downloader;

		private bool willStart;

		public Downloader Downloader => downloader;

		public bool WillStart => willStart;

		public DownloaderEventArgs(Downloader download)
		{
			downloader = download;
		}

		public DownloaderEventArgs(Downloader download, bool willStart)
			: this(download)
		{
			this.willStart = willStart;
		}
	}
}
