namespace TinhKiemAuto
{
	internal interface ISegmentCalculator
	{
		CalculatedSegment[] GetSegments(int segmentCount, RemoteFileInfo fileSize);
	}
}
