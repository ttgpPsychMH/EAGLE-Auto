using System.Collections.Generic;

namespace TinhKiemAuto
{
	internal class GAMEDIC
	{
		public static HashSet<string> BoQua = new HashSet<string> { "Bích Lân Cương Thi", "Thực Phẩm Hỏng" };

		public static HashSet<string> YenTuOBoQua = new HashSet<string> { "Yến Tử Ổ trang đinh", "Công Dã Càn", "Bao Bất Đồng", "Đặng Bách Xuyên", "Nhất Phẩm Đường Võ Sĩ" };

		public static List<string> PhungMinhVuongLang = new List<string>
		{
			"35,36", "48,30", "60,36", "60,36", "65,49", "60,60", "49,66", "36,61", "30,49", "41,41",
			"55,41", "55,57", "41,57", "49,49"
		};

		public static Dictionary<string, int> ThucAnPet = new Dictionary<string, int>();
	}
}
