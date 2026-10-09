namespace TinhKiemAuto.Models
{
	public class Option
	{
		public static int MaptriLieuIndex = 0;

		public static int MapBanDoIndex = 0;

		public static bool PutBase = false;

		public static bool AlarmChat = false;

		public static int HideTime = 60;

		public static bool SetSafeTime { get; set; }

		public static bool IsDead { get; set; }

		public static bool IsHoTro { get; set; }

		public static bool NotDongMon { get; set; }

		public static int Delay => 50;

		public static bool AutoPoint { get; set; }

		public static bool IsBank { get; set; }

		private bool IsPass2 { get; set; }
	}
}
