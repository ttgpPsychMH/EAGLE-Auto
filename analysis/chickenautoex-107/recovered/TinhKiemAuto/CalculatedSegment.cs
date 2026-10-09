using System;

namespace TinhKiemAuto
{
	[Serializable]
	public struct CalculatedSegment
	{
		private long startPosition;

		private long endPosition;

		public long StartPosition => startPosition;

		public long EndPosition => endPosition;

		public CalculatedSegment(long startPos, long endPos)
		{
			endPosition = endPos;
			startPosition = startPos;
		}
	}
}
