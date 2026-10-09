using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TinhKiemAuto
{
	public class AOBScan
	{
		protected struct MEMORY_BASIC_INFORMATION
		{
			public IntPtr BaseAddress;

			public IntPtr AllocationBase;

			public uint AllocationProtect;

			public uint RegionSize;

			public uint State;

			public uint Protect;

			public uint Type;
		}

		protected uint ProcessID;

		protected List<MEMORY_BASIC_INFORMATION> MemoryRegion { get; set; }

		public AOBScan(uint ProcessID)
		{
			this.ProcessID = ProcessID;
		}

		[DllImport("kernel32.dll")]
		protected static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesRead);

		[DllImport("kernel32.dll")]
		protected static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);

		protected void MemInfo(IntPtr pHandle)
		{
			IntPtr lpAddress = (IntPtr)0;
			while (true)
			{
				MEMORY_BASIC_INFORMATION lpBuffer = default(MEMORY_BASIC_INFORMATION);
				if (VirtualQueryEx(pHandle, lpAddress, out lpBuffer, Marshal.SizeOf(lpBuffer)) != 0)
				{
					if ((lpBuffer.State & 0x1000) != 0 && (lpBuffer.Protect & 0x100) == 0)
					{
						MemoryRegion.Add(lpBuffer);
					}
					lpAddress = new IntPtr(lpBuffer.BaseAddress.ToInt32() + (int)lpBuffer.RegionSize);
					continue;
				}
				break;
			}
		}

		public static bool Compare(byte[] input, byte[] pattern, int index)
		{
			for (int i = 0; i < pattern.Length; i++)
			{
				if (pattern[i] == byte.MaxValue)
				{
					continue;
				}
				if (pattern[i] == 1)
				{
					if (input[index + i] == 0)
					{
						return false;
					}
				}
				else if (input[index + i] != pattern[i])
				{
					return false;
				}
			}
			return true;
		}

		protected IntPtr Scan(byte[] sIn, byte[] sFor)
		{
			for (int i = 0; i < sIn.Length - sFor.Length; i++)
			{
				if (Compare(sIn, sFor, i))
				{
					return (IntPtr)i;
				}
			}
			return IntPtr.Zero;
		}

		public IntPtr AobScan(byte[] Pattern, uint startAddress, uint endAddress)
		{
			Process processById = Process.GetProcessById((int)ProcessID);
			if (processById.Id == 0)
			{
				return IntPtr.Zero;
			}
			MemoryRegion = new List<MEMORY_BASIC_INFORMATION>();
			MemInfo(processById.Handle);
			for (int i = 0; i < MemoryRegion.Count; i++)
			{
				if ((int)MemoryRegion[i].BaseAddress >= (int)startAddress && (int)MemoryRegion[i].BaseAddress <= (int)endAddress)
				{
					byte[] array = new byte[MemoryRegion[i].RegionSize];
					ReadProcessMemory(processById.Handle, MemoryRegion[i].BaseAddress, array, MemoryRegion[i].RegionSize, 0);
					IntPtr intPtr = Scan(array, Pattern);
					if (intPtr != IntPtr.Zero)
					{
						return new IntPtr(MemoryRegion[i].BaseAddress.ToInt32() + intPtr.ToInt32());
					}
				}
			}
			return IntPtr.Zero;
		}

		public IntPtr AobScan(byte[] Pattern, int startAddress)
		{
			Process processById = Process.GetProcessById((int)ProcessID);
			if (processById.Id == 0)
			{
				return IntPtr.Zero;
			}
			MemoryRegion = new List<MEMORY_BASIC_INFORMATION>();
			MemInfo(processById.Handle);
			for (int i = 0; i < MemoryRegion.Count; i++)
			{
				if ((int)MemoryRegion[i].BaseAddress >= startAddress)
				{
					byte[] array = new byte[MemoryRegion[i].RegionSize];
					ReadProcessMemory(processById.Handle, MemoryRegion[i].BaseAddress, array, MemoryRegion[i].RegionSize, 0);
					IntPtr intPtr = Scan(array, Pattern);
					if (intPtr != IntPtr.Zero)
					{
						return new IntPtr(MemoryRegion[i].BaseAddress.ToInt32() + intPtr.ToInt32());
					}
				}
			}
			return IntPtr.Zero;
		}

		public IntPtr AobScan(byte[] Pattern)
		{
			Process processById = Process.GetProcessById((int)ProcessID);
			if (processById.Id == 0)
			{
				return IntPtr.Zero;
			}
			MemoryRegion = new List<MEMORY_BASIC_INFORMATION>();
			MemInfo(processById.Handle);
			for (int i = 0; i < MemoryRegion.Count; i++)
			{
				byte[] array = new byte[MemoryRegion[i].RegionSize];
				ReadProcessMemory(processById.Handle, MemoryRegion[i].BaseAddress, array, MemoryRegion[i].RegionSize, 0);
				IntPtr intPtr = Scan(array, Pattern);
				if (intPtr != IntPtr.Zero)
				{
					return new IntPtr(MemoryRegion[i].BaseAddress.ToInt32() + intPtr.ToInt32());
				}
			}
			return IntPtr.Zero;
		}
	}
}
