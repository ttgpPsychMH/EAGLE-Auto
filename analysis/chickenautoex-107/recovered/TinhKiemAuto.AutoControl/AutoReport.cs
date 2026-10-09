using ProtoBuf;

namespace TinhKiemAuto.AutoControl
{
	[ProtoContract]
	public class AutoReport
	{
		[ProtoMember(1)]
		public bool IsAttack { get; set; }

		[ProtoMember(2)]
		public bool IsPickItem { get; set; }

		[ProtoMember(3)]
		public int MapIndex { get; set; }

		[ProtoMember(4)]
		public string MapName { get; set; }

		[ProtoMember(5)]
		public int PosX { get; set; }

		[ProtoMember(6)]
		public int PosY { get; set; }

		[ProtoMember(7)]
		public int Gold { get; set; }

		[ProtoMember(8)]
		public int PlayState { get; set; }

		[ProtoMember(9)]
		public int HpPercent { get; set; }

		[ProtoMember(10)]
		public int MpPercent { get; set; }

		[ProtoMember(11)]
		public int PetPercent { get; set; }

		[ProtoMember(12)]
		public float ExpPercent { get; set; }

		[ProtoMember(13)]
		public bool IsRide { get; set; }

		[ProtoMember(14)]
		public bool IsLear { get; set; }

		[ProtoMember(15)]
		public bool Online { get; set; }

		[ProtoMember(16)]
		public bool IsDuoc { get; set; }

		[ProtoMember(17)]
		public bool IsDisconnect { get; set; }

		[ProtoMember(18)]
		public bool IsX25 { get; set; }

		[ProtoMember(19)]
		public string Msg { get; set; }

		[ProtoMember(20)]
		public LenBai LenBai { get; set; }

		[ProtoMember(21)]
		public CheDo CheDo { get; set; }

		[ProtoMember(22)]
		public string CharID { get; set; }

		[ProtoMember(23)]
		public string CharName { get; set; }

		[ProtoMember(24)]
		public int Level { get; set; }

		[ProtoMember(25)]
		public string GuildName { get; set; }

		[ProtoMember(26)]
		public string GuildId { get; set; }

		[ProtoMember(27)]
		public string Phai { get; set; }

		[ProtoMember(28)]
		public OverView OverView { get; set; }

		[ProtoMember(29)]
		public TotalSkill Skills { get; set; }

		[ProtoMember(30)]
		public PickUpItems PickUpItems { get; set; }

		[ProtoMember(31)]
		public TotalPet Pets { get; set; }

		[ProtoMember(32)]
		public Features Features { get; set; }

		[ProtoMember(33)]
		public bool IsLenBai { get; set; }
	}
}
