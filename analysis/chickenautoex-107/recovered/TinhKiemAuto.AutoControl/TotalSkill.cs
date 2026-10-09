using System.Collections.Generic;
using ProtoBuf;

namespace TinhKiemAuto.AutoControl
{
	[ProtoContract]
	public class TotalSkill
	{
		[ProtoMember(1)]
		public List<SkillAuto> ActiveSkill { get; set; }

		[ProtoMember(2)]
		public List<SkillAuto> PassiveSkill { get; set; }
	}
}
