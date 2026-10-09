using ProtoBuf;

namespace TinhKiemAuto.Models
{
	[ProtoContract]
	public class PacketDef
	{
		[ProtoMember(1)]
		public int IDPacket { get; set; }

		[ProtoMember(2)]
		public byte[] data { get; set; }
	}
}
