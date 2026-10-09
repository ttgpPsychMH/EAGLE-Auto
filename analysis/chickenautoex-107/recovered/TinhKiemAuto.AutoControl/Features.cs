using ProtoBuf;

namespace TinhKiemAuto.AutoControl
{
	[ProtoContract]
	public class Features
	{
		[ProtoMember(1)]
		public bool IsAcceptParty { get; set; }

		[ProtoMember(2)]
		public bool IsAcceptAllPartyInvites { get; set; }

		[ProtoMember(3)]
		public bool IsUseSkillF1 { get; set; }

		[ProtoMember(4)]
		public bool IsAutoLevelUp { get; set; }

		[ProtoMember(5)]
		public int LimitLevelUp { get; set; }

		[ProtoMember(6)]
		public bool MakeAdvertisement { get; set; }

		[ProtoMember(7)]
		public int MakeAdvertisingTime { get; set; }

		[ProtoMember(8)]
		public int Chanel { get; set; }

		[ProtoMember(9)]
		public string ChatMSG { get; set; }

		[ProtoMember(10)]
		public bool NoticePrivateMessage { get; set; }

		[ProtoMember(11)]
		public int FollowRadius { get; set; }

		[ProtoMember(12)]
		public string Pass2 { get; set; }
	}
}
