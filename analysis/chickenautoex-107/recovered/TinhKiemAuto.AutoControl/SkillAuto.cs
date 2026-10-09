using ProtoBuf;

namespace TinhKiemAuto.AutoControl
{
	[ProtoContract]
	public class SkillAuto
	{
		[ProtoMember(1)]
		public int Id { get; set; }

		[ProtoMember(2)]
		public string Name { get; set; }

		[ProtoMember(3)]
		public bool IsUsing { get; set; }
	}
}
