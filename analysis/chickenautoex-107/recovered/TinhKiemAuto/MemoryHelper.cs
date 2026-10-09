using System;
using System.Diagnostics;

namespace TinhKiemAuto
{
	internal class MemoryHelper
	{
		private static bool _Toggle = true;

		public static void ReduceMemory(int processId)
		{
			try
			{
				Process processById = Process.GetProcessById(processId);
				if (_Toggle)
				{
					processById.MaxWorkingSet = (IntPtr)((int)processById.MaxWorkingSet - 1);
					processById.MinWorkingSet = (IntPtr)((int)processById.MinWorkingSet - 1);
				}
				else
				{
					processById.MaxWorkingSet = (IntPtr)((int)processById.MaxWorkingSet + 1);
					processById.MinWorkingSet = (IntPtr)((int)processById.MinWorkingSet + 1);
				}
				_Toggle = !_Toggle;
			}
			catch
			{
			}
		}
	}
}
