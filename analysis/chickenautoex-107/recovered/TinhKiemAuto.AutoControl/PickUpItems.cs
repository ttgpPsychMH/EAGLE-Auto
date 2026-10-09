using ProtoBuf;

namespace TinhKiemAuto.AutoControl
{
	[ProtoContract]
	public class PickUpItems
	{
		[ProtoMember(1)]
		public bool DestroyItem { get; set; }

		[ProtoMember(2)]
		public bool SellItem { get; set; }

		[ProtoMember(3)]
		public int PickUpRadius { get; set; }

		[ProtoMember(4)]
		public bool PutToBank { get; set; }

		[ProtoMember(5)]
		public bool ThrowTrash { get; set; }

		[ProtoMember(6)]
		public bool AutoEatX25 { get; set; }

		[ProtoMember(7)]
		public bool UseSpecialItem { get; set; }
	}
}
