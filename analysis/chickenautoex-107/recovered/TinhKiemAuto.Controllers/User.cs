using System;
using System.Runtime.CompilerServices;

namespace TinhKiemAuto.Controllers
{
	public class User
	{
		public static int TienXu = 50000;

		public static int ThoiGian = 50000;

		public static string Email = "tlbb@gmail.com";

		public static string Pass = "123456";

		public static bool IsAlarm { get; set; }

		[CompilerGenerated]
		public event EventHandler Updated;
	}
}
