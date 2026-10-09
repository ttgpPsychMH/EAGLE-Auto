using System;

namespace TinhKiemAuto
{
	internal static class TimeSpanFormatter
	{
		public static string ToString(TimeSpan ts)
		{
			if (ts == TimeSpan.MaxValue)
			{
				return "?";
			}
			string text = ts.ToString();
			int num = text.LastIndexOf('.');
			if (num > 0)
			{
				return text.Remove(num);
			}
			return text;
		}
	}
}
