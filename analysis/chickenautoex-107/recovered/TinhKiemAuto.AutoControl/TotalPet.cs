using System.Collections.Generic;
using ProtoBuf;

namespace TinhKiemAuto.AutoControl
{
	[ProtoContract]
	public class TotalPet
	{
		[ProtoMember(1)]
		public List<PetAutoInfo> PetInfo { get; set; }

		[ProtoMember(2)]
		public bool IsAutoCallPet { get; set; }

		[ProtoMember(3)]
		public bool IsAutoBuffPet { get; set; }

		[ProtoMember(4)]
		public bool IsAutoUsePetSkill { get; set; }

		[ProtoMember(5)]
		public bool IsAutoCallBackPet { get; set; }

		[ProtoMember(6)]
		public int AutoCallBackAtLevel { get; set; }

		[ProtoMember(7)]
		public bool IsAutoTakeCare { get; set; }
	}
}
