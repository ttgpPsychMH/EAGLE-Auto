using ProtoBuf;

namespace TinhKiemAuto.AutoControl
{
	[ProtoContract]
	public class CheDo
	{
		[ProtoMember(1)]
		public bool IsCheDo { get; set; }

		[ProtoMember(2)]
		public bool IsMienPhiNguyenLieu { get; set; }

		[ProtoMember(3)]
		public int CheLoai { get; set; }

		[ProtoMember(4)]
		public int CheCap { get; set; }

		[ProtoMember(5)]
		public int TotalChe { get; set; }

		[ProtoMember(6)]
		public int TongNhan { get; set; }

		[ProtoMember(7)]
		public int CheDiem { get; set; }

		[ProtoMember(8)]
		public int CheDong { get; set; }

		[ProtoMember(9)]
		public int CheSao { get; set; }
	}
}
