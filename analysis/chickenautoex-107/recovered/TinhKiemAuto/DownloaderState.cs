namespace TinhKiemAuto
{
	public enum DownloaderState : byte
	{
		NeedToPrepare,
		Preparing,
		WaitingForReconnect,
		Prepared,
		Working,
		Pausing,
		Paused,
		Ended,
		EndedWithError
	}
}
