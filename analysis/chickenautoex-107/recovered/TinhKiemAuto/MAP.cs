using System.Collections.Generic;

namespace TinhKiemAuto
{
	internal class MAP
	{
		public static int LacDuong = 0;

		public static int ToChau = 1;

		public static int DaiLy = 2;

		public static int GiamNguc = 194;

		public static int DiaPhu = 77;

		public static int DiaPhuDaiTheGioi = 578;

		public static int TungSon = 3;

		public static int ThaiHo = 4;

		public static int KinhHo = 5;

		public static int VoLuongSon = 6;

		public static int KiemCac = 7;

		public static int DonHoang = 8;

		public static int ThieuLam = 9;

		public static int MinhGiao = 11;

		public static int CaiBang = 10;

		public static int VoDang = 12;

		public static int NgaMy = 15;

		public static int TinhTuc = 16;

		public static int ThienLong = 13;

		public static int ThienSon = 17;

		public static int TieuDao = 14;

		public static int MoDung = 284;

		public static int DuongMon = 615;

		public static int CongDia = 153;

		public static int ThieuLamPhuBan = 182;

		public static int CaiBangPhuBan = 183;

		public static int MinhGiaoPhuBan = 184;

		public static int VoDangPhuBan = 185;

		public static int ThienLongPhuBan = 186;

		public static int TieuDaoPhuBan = 187;

		public static int NgaMyPhuBan = 188;

		public static int TinhTucPhuBan = 189;

		public static int ThienSonPhuBan = 190;

		public static int MoDungPhuBan = 289;

		public static int DuongMonPhuBan = 616;

		public static int LongTuyen = 31;

		public static int ThuongSon = 25;

		public static int ThachLam = 26;

		public static int CaoXuong = 245;

		public static int NhanBac = 19;

		public static int ThaoNguyen = 20;

		public static int NganNgaiTuyetNguyen = 229;

		public static int VoDi = 32;

		public static int TayHo = 30;

		public static int NhiHai = 24;

		public static int NhanNam = 18;

		public static int ThieuLamAcBa = 173;

		public static int NgaMyAcBa = 179;

		public static int TieuDaoAcBa = 178;

		public static int DuongMonAcBa = 618;

		public static int MinhGiaoAcBa = 175;

		public static int VoDangAcBa = 176;

		public static int TinhTucAcBa = 180;

		public static int ThienSonAcBa = 181;

		public static int CaiBangAcBa = 174;

		public static int ThienLongAcBa = 177;

		public static int MoDungAcBa = 288;

		public static int TacKhauDoanhDia = 170;

		public static int TangKinhCac = 272;

		public static int ThanhThuSon = 201;

		public static int ThanhThuSonPhuBan = 232;

		public static int LauLan = 246;

		public static int HuyenVuDaoPhuBan = 268;

		public static int PhungHoangCoThanh = 280;

		public static int PhungHoangCoThanhPhuBan = 281;

		public static int PhieuMieuPhong = 261;

		public static int VanKiemCoc = 119;

		public static int VanKiemCocDem = 118;

		public static int LauLanBaoTang = 269;

		public static int ViemMaSon = 651;

		public static int TamTaiHiepCoc = 653;

		public static int TranLongKyCuoc = 61;

		public static int SinhTuLoiDai = 546;

		public static int YenTuO = 236;

		public static int PhungMinhVuongLang = 600;

		public static int HuyetMo = 110;

		public static int ThuyLao = 66;

		public static int ThieuThatSon = 566;

		public static int HuyenVuDao = 112;

		public static int BinhThanhKyTran = 294;

		public static int QuynhChau = 35;

		public static int NamVuc = 34;

		public static int MieuCuong = 29;

		public static List<int> DanhSachPHUBAN = new List<int>
		{
			36, 37, 38, 42, 47, 61, 66, 78, 79, 80,
			81, 102, 103, 104, 105, 106, 107, 108, 110, 111,
			109, 124, 125, 126, 127, 128, 113, 114, 115, 116,
			118, 120, 121, 119, 117, 133, 134, 135, 136, 137,
			138, 139, 140, 141, 142, 143, 144, 145, 146, 147,
			148, 149, 150, 151, 152, 153, 154, 155, 156, 157,
			158, 159, 160, 161, 162, 163, 167, 170, 173, 174,
			175, 176, 177, 178, 179, 180, 181, 195, 196, 197,
			198, 230, 236, 243, 272, 265, 266, 267, 268, 269,
			546, 261, 257, 258, 259, 293, 281, 617, 614, 231,
			232, 233, 600, 580, 566, 291, 580, 294
		};

		public static bool IsPhuBan(int MapID)
		{
			if (DanhSachPHUBAN.Contains(MapID))
			{
				return true;
			}
			return false;
		}
	}
}
