using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TinhKiemAuto
{
	internal class ProcessManager
	{
		public static List<Modules> CollectModules(Process process)
		{
			List<Modules> list = new List<Modules>();
			IntPtr[] lphModule = new IntPtr[0];
			int lpcbNeeded = 0;
			if (!Native.EnumProcessModulesEx(process.Handle, lphModule, 0, out lpcbNeeded, 3u))
			{
				return list;
			}
			int num = lpcbNeeded / IntPtr.Size;
			lphModule = new IntPtr[num];
			if (Native.EnumProcessModulesEx(process.Handle, lphModule, lpcbNeeded, out lpcbNeeded, 3u))
			{
				for (int i = 0; i < num; i++)
				{
					StringBuilder stringBuilder = new StringBuilder(1024);
					Native.GetModuleFileNameEx(process.Handle, lphModule[i], stringBuilder, (uint)stringBuilder.Capacity);
					string fileName = Path.GetFileName(stringBuilder.ToString());
					Native.ModuleInformation lpmodinfo = default(Native.ModuleInformation);
					Native.GetModuleInformation(process.Handle, lphModule[i], out lpmodinfo, (uint)(IntPtr.Size * lphModule.Length));
					Modules item = new Modules(fileName, lpmodinfo.lpBaseOfDll, lpmodinfo.SizeOfImage);
					list.Add(item);
				}
			}
			return list;
		}
	}
}
