using System;

namespace TinhKiemAuto
{
	public class Modules
	{
		public string ModuleName { get; set; }

		public IntPtr BaseAddress { get; set; }

		public uint Size { get; set; }

		public Modules(string moduleName, IntPtr baseAddress, uint size)
		{
			ModuleName = moduleName;
			BaseAddress = baseAddress;
			Size = size;
		}
	}
}
