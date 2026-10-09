using System.Collections.Generic;

namespace TinhKiemAuto
{
	internal class MinSizeSegmentCalculator : ISegmentCalculator
	{
		public CalculatedSegment[] GetSegments(int segmentCount, RemoteFileInfo remoteFileInfo)
		{
			long num = DownloadSettings.MinSegmentSize;
			long num2 = remoteFileInfo.FileSize / segmentCount;
			while (segmentCount > 1 && num2 < num)
			{
				segmentCount--;
				num2 = remoteFileInfo.FileSize / segmentCount;
			}
			long num3 = 0L;
			List<CalculatedSegment> list = new List<CalculatedSegment>();
			for (int i = 0; i < segmentCount; i++)
			{
				if (segmentCount - 1 == i)
				{
					list.Add(new CalculatedSegment(num3, remoteFileInfo.FileSize));
				}
				else
				{
					list.Add(new CalculatedSegment(num3, num3 + (int)num2));
				}
				num3 = list[list.Count - 1].EndPosition;
			}
			return list.ToArray();
		}
	}
}
