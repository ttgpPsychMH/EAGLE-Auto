using System;
using System.Runtime.InteropServices;
using System.Text;

namespace TinhKiemAuto
{
	public class Native
	{
		public struct ModuleInformation
		{
			public IntPtr lpBaseOfDll;

			public uint SizeOfImage;

			public IntPtr EntryPoint;
		}

		internal enum ModuleFilter
		{
			ListModulesDefault,
			ListModules32Bit,
			ListModules64Bit,
			ListModulesAll
		}

		[DllImport("psapi.dll")]
		public static extern bool EnumProcessModulesEx(IntPtr hProcess, [In][Out][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] IntPtr[] lphModule, int cb, [MarshalAs(UnmanagedType.U4)] out int lpcbNeeded, uint dwFilterFlag);

		[DllImport("psapi.dll")]
		public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In][MarshalAs(UnmanagedType.U4)] uint nSize);

		[DllImport("psapi.dll", SetLastError = true)]
		public static extern bool GetModuleInformation(IntPtr hProcess, IntPtr hModule, out ModuleInformation lpmodinfo, uint cb);
	}
}
