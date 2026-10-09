namespace TinhKiemAuto
{
	internal class SegmentEventArgs : DownloaderEventArgs
	{
		private Segment segment;

		public Segment Segment
		{
			get
			{
				return segment;
			}
			set
			{
				segment = value;
			}
		}

		public SegmentEventArgs(Downloader d, Segment segment)
			: base(d)
		{
			this.segment = segment;
		}
	}
}
