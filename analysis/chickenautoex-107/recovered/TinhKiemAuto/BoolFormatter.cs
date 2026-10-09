namespace TinhKiemAuto
{
	internal static class BoolFormatter
	{
		private const string Yes = "Yes";

		private const string No = "No";

		public static bool FromString(string s)
		{
			return s == "Yes";
		}

		public static string ToString(bool v)
		{
			if (v)
			{
				return "Yes";
			}
			return "No";
		}
	}
}
