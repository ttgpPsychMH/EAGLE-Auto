using System;
using System.Runtime.InteropServices;

namespace TinhKiemAuto
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public struct U1
	{
		[FieldOffset(0)]
		public uint AddressOfData;

		[FieldOffset(0)]
		public uint ForwarderString;

		[FieldOffset(0)]
		public uint Function;

		[FieldOffset(0)]
		public uint Ordinal;
	}
}
