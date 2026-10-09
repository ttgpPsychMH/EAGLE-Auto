namespace TinhKiemAuto
{
	internal static class ByteFormatter
	{
		private const long KB = 1024L;

		private const long MB = 1048576L;

		private const long GB = 1073741824L;

		private const string BFormatPattern = "{0} b";

		private const string KBFormatPattern = "{0:0} KB";

		private const string MBFormatPattern = "{0:0,###} MB";

		private const string GBFormatPattern = "{0:0,###.###} GB";

		public static string ToString(long size)
		{
			if (size < 1024)
			{
				return $"{size} b";
			}
			if (size >= 1024 && size < 1048576)
			{
				return $"{(float)size / 1024f:0} KB";
			}
			if (size >= 1048576 && size < 1073741824)
			{
				return $"{(float)size / 1024f:0,###} MB";
			}
			return $"{(float)size / 1024f:0,###.###} GB";
		}
	}
}
