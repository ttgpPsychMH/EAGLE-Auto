using System.Collections.Generic;
using ProtoBuf;

namespace TinhKiemAuto.AutoControl
{
	[ProtoContract]
	public class PacketSend
	{
		[ProtoMember(1)]
		public string HardwareID { get; set; }

		[ProtoMember(2)]
		public Dictionary<string, AutoReport> DanhSachGame { get; set; }
	}
}
