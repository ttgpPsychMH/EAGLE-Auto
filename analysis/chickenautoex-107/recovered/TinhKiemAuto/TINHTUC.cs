namespace TinhKiemAuto
{
	internal class TINHTUC
	{
		public static int Id = 16;

		public static NPC HanTheTrung = new NPC
		{
			Id = 1,
			X = 96,
			Y = 75,
			Map = Id,
			INFOAIM = "#GTinh Túc Hải #RHàn Thế Trung#{_INFOAIM96,75,16,Hàn Thế Trung}"
		};

		public static NPC VuongNgan = new NPC
		{
			Id = 9,
			X = 96,
			Y = 93,
			Map = Id,
			INFOAIM = "#GTinh Túc Hải #RVương Ngạn#{_INFOAIM96,93,16,Vương Ngạn}"
		};

		public static NPC ThiToan = new NPC
		{
			Id = 6,
			X = 87,
			Y = 70,
			Map = Id,
			INFOAIM = "#GTinh Túc Hải #RThi Toàn#{_INFOAIM87,70,16,Thi Toàn}"
		};
	}
}
