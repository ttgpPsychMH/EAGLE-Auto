using ProtoBuf;

namespace TinhKiemAuto.AutoControl
{
	[ProtoContract]
	public class OverView
	{
		[ProtoMember(1)]
		public int PhamViDanh { get; set; }

		[ProtoMember(2)]
		public bool IsDanhQuanhDiem { get; set; }

		[ProtoMember(3)]
		public bool IsGomQuai { get; set; }

		[ProtoMember(4)]
		public bool IsUsingThoLinhChau { get; set; }

		[ProtoMember(5)]
		public bool IsRengeHP { get; set; }

		[ProtoMember(6)]
		public int RengeHPPercent { get; set; }

		[ProtoMember(7)]
		public bool IsRengeMP { get; set; }

		[ProtoMember(8)]
		public int RengeMPPercent { get; set; }

		[ProtoMember(9)]
		public bool IsCongSinh { get; set; }

		[ProtoMember(10)]
		public int CongSinhValue { get; set; }

		[ProtoMember(11)]
		public bool IsHuyetTe { get; set; }

		[ProtoMember(12)]
		public int HuyetTeValue { get; set; }

		[ProtoMember(13)]
		public bool IsNM { get; set; }

		[ProtoMember(14)]
		public int BuffNMPercent { get; set; }

		[ProtoMember(15)]
		public bool IsAutoReborn { get; set; }

		[ProtoMember(16)]
		public bool IsAutoComeBack { get; set; }

		[ProtoMember(17)]
		public bool isArletHP { get; set; }

		[ProtoMember(18)]
		public int ArletHPPercent { get; set; }
	}
}
