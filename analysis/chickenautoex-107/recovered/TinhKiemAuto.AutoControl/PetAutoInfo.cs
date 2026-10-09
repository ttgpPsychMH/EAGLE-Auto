using ProtoBuf;

namespace TinhKiemAuto.AutoControl
{
	[ProtoContract]
	public class PetAutoInfo
	{
		[ProtoMember(1)]
		public string Name { get; set; }

		[ProtoMember(2)]
		public int Id { get; set; }

		[ProtoMember(3)]
		public int Pos { get; set; }
	}
}
