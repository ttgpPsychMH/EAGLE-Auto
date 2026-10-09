using System;

namespace TinhKiemAuto
{
	[Serializable]
	public struct IMAGE_RESOURCE_DIRECTORY
	{
		public uint Characteristics;

		public uint TimeDateStamp;

		public ushort MajorVersion;

		public ushort MinorVersion;

		public ushort NumberOfNamedEntries;

		public ushort NumberOfIdEntries;
	}
}
