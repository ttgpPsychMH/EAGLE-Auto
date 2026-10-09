using ProtoBuf;

namespace TinhKiemAuto.AutoControl
{
	[ProtoContract]
	public class LenBai
	{
		[ProtoMember(1)]
		public int MapID { get; set; }

		[ProtoMember(2)]
		public string MapName { get; set; }

		[ProtoMember(3)]
		public int PosX { get; set; }

		[ProtoMember(4)]
		public int PosY { get; set; }
	}
}
