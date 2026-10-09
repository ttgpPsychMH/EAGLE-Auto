using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace TinhKiemAuto
{
	internal class ManualMap : InjectionMethod
	{
		private static readonly byte[] DLLMAIN_STUB = new byte[20]
		{
			104, 0, 0, 0, 0, 104, 1, 0, 0, 0,
			104, 0, 0, 0, 0, 255, 84, 36, 16, 195
		};

		private static readonly IntPtr FN_ACTIVATEACTCTX = WinAPI.GetProcAddress(H_KERNEL32, "ActivateActCtx");

		private static readonly IntPtr FN_CREATEACTCTXA = WinAPI.GetProcAddress(H_KERNEL32, "CreateActCtxA");

		private static readonly IntPtr FN_DEACTIVATEACTCTX = WinAPI.GetProcAddress(H_KERNEL32, "DeactivateActCtx");

		private static readonly IntPtr FN_GETMODULEHANDLEA = WinAPI.GetProcAddress(H_KERNEL32, "GetModuleHandleA");

		private static readonly IntPtr FN_LOADLIBRARYA = WinAPI.GetProcAddress(H_KERNEL32, "LoadLibraryA");

		private static readonly IntPtr FN_RELEASEACTCTX = WinAPI.GetProcAddress(H_KERNEL32, "ReleaseActCtx");

		private static readonly IntPtr H_KERNEL32 = WinAPI.GetModuleHandleA("KERNEL32.dll");

		private static readonly byte[] RESOLVER_STUB = new byte[217]
		{
			85, 139, 236, 131, 236, 60, 139, 204, 139, 209,
			131, 194, 60, 199, 1, 0, 0, 0, 0, 131,
			193, 4, 59, 202, 126, 243, 198, 4, 36, 32,
			185, 0, 0, 0, 0, 137, 76, 36, 8, 185,
			0, 0, 0, 0, 137, 76, 36, 40, 185, 0,
			0, 0, 0, 137, 76, 36, 44, 84, 232, 0,
			0, 0, 0, 131, 56, 255, 15, 132, 137, 0,
			0, 0, 137, 68, 36, 48, 139, 204, 131, 193,
			32, 81, 80, 232, 0, 0, 0, 0, 131, 248,
			0, 116, 107, 198, 68, 36, 36, 1, 139, 76,
			36, 40, 131, 249, 0, 126, 62, 131, 233, 1,
			137, 76, 36, 40, 139, 76, 36, 36, 131, 249,
			0, 116, 46, 255, 116, 36, 44, 232, 0, 0,
			0, 0, 131, 248, 0, 117, 9, 255, 116, 36,
			44, 232, 0, 0, 0, 0, 137, 68, 36, 36,
			139, 76, 36, 44, 138, 1, 131, 193, 1, 60,
			0, 117, 247, 137, 76, 36, 44, 235, 185, 139,
			68, 36, 36, 185, 1, 0, 0, 0, 35, 193,
			137, 76, 36, 36, 131, 249, 0, 117, 20, 255,
			116, 36, 32, 106, 0, 232, 0, 0, 0, 0,
			255, 116, 36, 48, 232, 0, 0, 0, 0, 139,
			68, 36, 36, 139, 229, 93, 195
		};

		private static byte[] ExtractManifest(PortableExecutable image)
		{
			byte[] result = null;
			ResourceWalker resourceWalker = new ResourceWalker(image);
			ResourceWalker.ResourceDirectory resourceDirectory = null;
			for (int i = 0; i < resourceWalker.Root.Directories.Length; i++)
			{
				if (resourceDirectory != null)
				{
					break;
				}
				if ((long)resourceWalker.Root.Directories[i].Id == 24)
				{
					resourceDirectory = resourceWalker.Root.Directories[i];
				}
			}
			if (resourceDirectory != null && resourceDirectory.Directories.Length != 0 && IsManifestResource(resourceDirectory.Directories[0].Id) && resourceDirectory.Directories[0].Files.Length == 1)
			{
				result = resourceDirectory.Directories[0].Files[0].GetData();
			}
			return result;
		}

		private static uint FindEntryPoint(IntPtr hProcess, IntPtr hModule)
		{
			if (hProcess.IsNull() || hProcess.Compare(-1L))
			{
				throw new ArgumentException("Invalid process handle.", "hProcess");
			}
			if (hModule.IsNull())
			{
				throw new ArgumentException("Invalid module handle.", "hModule");
			}
			byte[] array = WinAPI.ReadRemoteMemory(hProcess, hModule, (uint)Marshal.SizeOf(typeof(IMAGE_DOS_HEADER)));
			if (array != null)
			{
				ushort num = BitConverter.ToUInt16(array, 0);
				uint num2 = BitConverter.ToUInt32(array, 60);
				if (num == 23117)
				{
					byte[] array2 = WinAPI.ReadRemoteMemory(hProcess, hModule.Add(num2), (uint)Marshal.SizeOf(typeof(IMAGE_NT_HEADER32)));
					if (array2 != null && BitConverter.ToUInt32(array2, 0) == 17744)
					{
						IMAGE_NT_HEADER32 result = default(IMAGE_NT_HEADER32);
						using (UnmanagedBuffer unmanagedBuffer = new UnmanagedBuffer(256))
						{
							if (unmanagedBuffer.Translate(array2, out result))
							{
								return result.OptionalHeader.AddressOfEntryPoint;
							}
						}
						return 0u;
					}
				}
			}
			return 0u;
		}

		private static IntPtr GetRemoteModuleHandle(string module, int processId)
		{
			IntPtr intPtr = IntPtr.Zero;
			Process processById = Process.GetProcessById(processId);
			for (int i = 0; i < processById.Modules.Count; i++)
			{
				if (!intPtr.IsNull())
				{
					break;
				}
				if (processById.Modules[i].ModuleName.ToLower() == module.ToLower())
				{
					intPtr = processById.Modules[i].BaseAddress;
				}
			}
			return intPtr;
		}

		public override IntPtr Inject(PortableExecutable image, IntPtr hProcess)
		{
			ClearErrors();
			try
			{
				return MapModule(Utils.DeepClone(image), hProcess, preserveHeaders: true);
			}
			catch (Exception lastError)
			{
				SetLastError(lastError);
				return IntPtr.Zero;
			}
		}

		public override IntPtr Inject(string dllPath, IntPtr hProcess)
		{
			ClearErrors();
			try
			{
				using (PortableExecutable image = new PortableExecutable(dllPath))
				{
					return Inject(image, hProcess);
				}
			}
			catch (Exception lastError)
			{
				SetLastError(lastError);
				return IntPtr.Zero;
			}
		}

		public override IntPtr[] InjectAll(PortableExecutable[] images, IntPtr hProcess)
		{
			ClearErrors();
			return Array.ConvertAll(images, (PortableExecutable pe) => Inject(pe, hProcess));
		}

		public override IntPtr[] InjectAll(string[] dllPaths, IntPtr hProcess)
		{
			ClearErrors();
			return Array.ConvertAll(dllPaths, (string dp) => Inject(dp, hProcess));
		}

		private static bool IsManifestResource(int id)
		{
			if ((uint)(id - 1) <= 2u)
			{
				return true;
			}
			return false;
		}

		private static bool LoadDependencies(PortableExecutable image, IntPtr hProcess, int processId)
		{
			List<string> list = new List<string>();
			string lpBuffer = string.Empty;
			bool result = false;
			foreach (IMAGE_IMPORT_DESCRIPTOR item in image.EnumImports())
			{
				if (image.ReadString(image.GetPtrFromRVA(item.Name), SeekOrigin.Begin, out lpBuffer) && !string.IsNullOrEmpty(lpBuffer) && GetRemoteModuleHandle(lpBuffer, processId).IsNull())
				{
					list.Add(lpBuffer);
				}
			}
			if (list.Count > 0)
			{
				byte[] array = ExtractManifest(image);
				string empty = string.Empty;
				if (array == null)
				{
					if (string.IsNullOrEmpty(image.FileLocation) || !File.Exists(Path.Combine(Path.GetDirectoryName(image.FileLocation), Path.GetFileName(image.FileLocation) + ".manifest")))
					{
						IntPtr[] array2 = InjectionMethod.Create(InjectionMethodType.Standard).InjectAll(list.ToArray(), hProcess);
						for (int i = 0; i < array2.Length; i++)
						{
							if (array2[i].IsNull())
							{
								return false;
							}
						}
						return true;
					}
					empty = Path.Combine(Path.GetDirectoryName(image.FileLocation), Path.GetFileName(image.FileLocation) + ".manifest");
				}
				else
				{
					empty = Utils.WriteTempData(array);
				}
				if (string.IsNullOrEmpty(empty))
				{
					return false;
				}
				IntPtr intPtr = WinAPI.VirtualAllocEx(hProcess, IntPtr.Zero, (uint)RESOLVER_STUB.Length, 12288, 64);
				IntPtr lpAddress = WinAPI.CreateRemotePointer(hProcess, Encoding.ASCII.GetBytes(empty + "\0"), 4);
				IntPtr lpAddress2 = WinAPI.CreateRemotePointer(hProcess, Encoding.ASCII.GetBytes(string.Join("\0", list.ToArray()) + "\0"), 4);
				if (!intPtr.IsNull())
				{
					byte[] array3 = (byte[])RESOLVER_STUB.Clone();
					uint lpNumberOfBytesRead = 0u;
					BitConverter.GetBytes(FN_CREATEACTCTXA.Subtract(intPtr.Add(63L)).ToInt32()).CopyTo(array3, 59);
					BitConverter.GetBytes(FN_ACTIVATEACTCTX.Subtract(intPtr.Add(88L)).ToInt32()).CopyTo(array3, 84);
					BitConverter.GetBytes(FN_GETMODULEHANDLEA.Subtract(intPtr.Add(132L)).ToInt32()).CopyTo(array3, 128);
					BitConverter.GetBytes(FN_LOADLIBRARYA.Subtract(intPtr.Add(146L)).ToInt32()).CopyTo(array3, 142);
					BitConverter.GetBytes(FN_DEACTIVATEACTCTX.Subtract(intPtr.Add(200L)).ToInt32()).CopyTo(array3, 196);
					BitConverter.GetBytes(FN_RELEASEACTCTX.Subtract(intPtr.Add(209L)).ToInt32()).CopyTo(array3, 205);
					BitConverter.GetBytes(lpAddress.ToInt32()).CopyTo(array3, 31);
					BitConverter.GetBytes(list.Count).CopyTo(array3, 40);
					BitConverter.GetBytes(lpAddress2.ToInt32()).CopyTo(array3, 49);
					if (WinAPI.WriteProcessMemory(hProcess, intPtr, array3, array3.Length, out lpNumberOfBytesRead) && lpNumberOfBytesRead == array3.Length)
					{
						uint num = WinAPI.RunThread(hProcess, intPtr, 0u, 5000);
						result = num != uint.MaxValue && num != 0;
					}
					WinAPI.VirtualFreeEx(hProcess, lpAddress2, 0, 32768);
					WinAPI.VirtualFreeEx(hProcess, lpAddress, 0, 32768);
					WinAPI.VirtualFreeEx(hProcess, intPtr, 0, 32768);
				}
			}
			return result;
		}

		private static IntPtr MapModule(PortableExecutable image, IntPtr hProcess, bool preserveHeaders = false)
		{
			if (hProcess.IsNull() || hProcess.Compare(-1L))
			{
				throw new ArgumentException("Invalid process handle.", "hProcess");
			}
			if (image == null)
			{
				throw new ArgumentException("Cannot map a non-existant PE Image.", "image");
			}
			int processId = WinAPI.GetProcessId(hProcess);
			if (processId == 0)
			{
				throw new ArgumentException("Provided handle doesn't have sufficient permissions to inject", "hProcess");
			}
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			uint lpNumberOfBytesRead = 0u;
			try
			{
				intPtr = WinAPI.VirtualAllocEx(hProcess, IntPtr.Zero, image.NTHeader.OptionalHeader.SizeOfImage, 12288, 4);
				if (intPtr.IsNull())
				{
					throw new InvalidOperationException("Unable to allocate memory in the remote process.");
				}
				PatchRelocations(image, intPtr);
				LoadDependencies(image, hProcess, processId);
				PatchImports(image, hProcess, processId);
				if (preserveHeaders)
				{
					byte[] array = new byte[image.DOSHeader.e_lfanew + Marshal.SizeOf(typeof(IMAGE_FILE_HEADER)) + 4 + image.NTHeader.FileHeader.SizeOfOptionalHeader];
					if (image.Read(0L, SeekOrigin.Begin, array))
					{
						WinAPI.WriteProcessMemory(hProcess, intPtr, array, array.Length, out lpNumberOfBytesRead);
					}
				}
				MapSections(image, hProcess, intPtr);
				if (image.NTHeader.OptionalHeader.AddressOfEntryPoint == 0)
				{
					return intPtr;
				}
				byte[] array2 = (byte[])DLLMAIN_STUB.Clone();
				BitConverter.GetBytes(intPtr.ToInt32()).CopyTo(array2, 11);
				intPtr2 = WinAPI.VirtualAllocEx(hProcess, IntPtr.Zero, (uint)DLLMAIN_STUB.Length, 12288, 64);
				if (intPtr2.IsNull() || !WinAPI.WriteProcessMemory(hProcess, intPtr2, array2, array2.Length, out lpNumberOfBytesRead) || lpNumberOfBytesRead != array2.Length)
				{
					throw new InvalidOperationException("Unable to write stub to the remote process.");
				}
				IntPtr intPtr3 = WinAPI.CreateRemoteThread(hProcess, 0, 0, intPtr2, (uint)intPtr.Add(image.NTHeader.OptionalHeader.AddressOfEntryPoint).ToInt32(), 0, 0);
				if ((long)WinAPI.WaitForSingleObject(intPtr3, 5000) != 0L)
				{
					return intPtr;
				}
				WinAPI.GetExitCodeThread(intPtr3, out lpNumberOfBytesRead);
				if (lpNumberOfBytesRead == 0)
				{
					WinAPI.VirtualFreeEx(hProcess, intPtr, 0, 32768);
					throw new Exception("Entry method of module reported a failure " + Marshal.GetLastWin32Error());
				}
				WinAPI.VirtualFreeEx(hProcess, intPtr2, 0, 32768);
				WinAPI.CloseHandle(intPtr3);
				return intPtr;
			}
			catch (Exception ex)
			{
				if (!intPtr.IsNull())
				{
					WinAPI.VirtualFreeEx(hProcess, intPtr, 0, 32768);
				}
				if (!intPtr2.IsNull())
				{
					WinAPI.VirtualFreeEx(hProcess, intPtr, 0, 32768);
				}
				intPtr = IntPtr.Zero;
				throw ex;
			}
		}

		private static void MapSections(PortableExecutable image, IntPtr hProcess, IntPtr pModule)
		{
			foreach (IMAGE_SECTION_HEADER item in image.EnumSectionHeaders())
			{
				byte[] array = new byte[item.SizeOfRawData];
				if (!image.Read(item.PointerToRawData, SeekOrigin.Begin, array))
				{
					throw image.GetLastError();
				}
				if ((item.Characteristics & 0x2000000) == 0)
				{
					WinAPI.WriteProcessMemory(hProcess, pModule.Add(item.VirtualAddress), array, array.Length, out var lpNumberOfBytesRead);
					IntPtr lpAddress = pModule.Add(item.VirtualAddress);
					WinAPI.VirtualProtectEx(hProcess, lpAddress, item.SizeOfRawData, item.Characteristics & 0xFFFFFF, out lpNumberOfBytesRead);
				}
			}
		}

		private static void PatchImports(PortableExecutable image, IntPtr hProcess, int processId)
		{
			string lpBuffer = string.Empty;
			string lpBuffer2 = string.Empty;
			foreach (IMAGE_IMPORT_DESCRIPTOR item in image.EnumImports())
			{
				if (!image.ReadString(image.GetPtrFromRVA(item.Name), SeekOrigin.Begin, out lpBuffer))
				{
					continue;
				}
				IntPtr zero = IntPtr.Zero;
				zero = GetRemoteModuleHandle(lpBuffer, processId);
				if (zero.IsNull())
				{
					throw new FileNotFoundException($"Unable to load dependent module '{lpBuffer}'.");
				}
				uint num = image.GetPtrFromRVA(item.FirstThunkPtr);
				IMAGE_THUNK_DATA result;
				for (uint num2 = (uint)Marshal.SizeOf(typeof(IMAGE_THUNK_DATA)); image.Read((long)num, SeekOrigin.Begin, out result); num += num2)
				{
					if (result.u1.AddressOfData == 0)
					{
						break;
					}
					IntPtr zero2 = IntPtr.Zero;
					object obj;
					if ((result.u1.Ordinal & 0x80000000u) == 0)
					{
						if (!image.ReadString(image.GetPtrFromRVA(result.u1.AddressOfData) + 2, SeekOrigin.Begin, out lpBuffer2))
						{
							throw image.GetLastError();
						}
						obj = lpBuffer2;
					}
					else
					{
						obj = (ushort)(result.u1.Ordinal & 0xFFFF);
					}
					if (!(zero2 = WinAPI.GetModuleHandleA(lpBuffer)).IsNull())
					{
						IntPtr ptr = (obj.GetType().Equals(typeof(string)) ? WinAPI.GetProcAddress(zero2, (string)obj) : WinAPI.GetProcAddress(zero2, (uint)((ushort)obj & 0xFFFF)));
						if (!ptr.IsNull())
						{
							zero2 = zero.Add(ptr.Subtract(zero2.ToInt32()).ToInt32());
						}
					}
					else
					{
						zero2 = WinAPI.GetProcAddressEx(hProcess, zero, obj);
					}
					if (zero2.IsNull())
					{
						throw new EntryPointNotFoundException($"Unable to locate imported function '{lpBuffer2}' from module '{lpBuffer}' in the remote process.");
					}
					image.Write(num, SeekOrigin.Begin, zero2.ToInt32());
				}
			}
		}

		private static void PatchRelocations(PortableExecutable image, IntPtr pAlloc)
		{
			IMAGE_DATA_DIRECTORY iMAGE_DATA_DIRECTORY = image.NTHeader.OptionalHeader.DataDirectory[5];
			if (iMAGE_DATA_DIRECTORY.Size == 0)
			{
				return;
			}
			uint num = 0u;
			uint num2 = (uint)pAlloc.ToInt32() - image.NTHeader.OptionalHeader.ImageBase;
			uint num3 = image.GetPtrFromRVA(iMAGE_DATA_DIRECTORY.VirtualAddress);
			uint num4 = (uint)Marshal.SizeOf(typeof(IMAGE_BASE_RELOCATION));
			IMAGE_BASE_RELOCATION result;
			while (num < iMAGE_DATA_DIRECTORY.Size && image.Read((long)num3, SeekOrigin.Begin, out result))
			{
				int num5 = (int)((result.SizeOfBlock - num4) / 2);
				uint ptrFromRVA = image.GetPtrFromRVA(result.VirtualAddress);
				for (int i = 0; i < num5; i++)
				{
					if (image.Read(num3 + num4 + ((long)i << 1), SeekOrigin.Begin, out ushort result2) && ((result2 >> 12) & 3) != 0)
					{
						uint num6 = ptrFromRVA + (uint)(result2 & 0xFFF);
						if (!image.Read((long)num6, SeekOrigin.Begin, out uint result3))
						{
							throw image.GetLastError();
						}
						image.Write(-4L, SeekOrigin.Current, result3 + num2);
					}
				}
				num += result.SizeOfBlock;
				num3 += result.SizeOfBlock;
			}
		}

		public override bool Unload(IntPtr hModule, IntPtr hProcess)
		{
			ClearErrors();
			if (hModule.IsNull())
			{
				throw new ArgumentNullException("hModule", "Invalid module handle");
			}
			if (hProcess.IsNull() || hProcess.Compare(-1L))
			{
				throw new ArgumentException("Invalid process handle.", "hProcess");
			}
			IntPtr zero = IntPtr.Zero;
			uint lpNumberOfBytesRead = 0u;
			try
			{
				uint num = FindEntryPoint(hProcess, hModule);
				if (num != 0)
				{
					byte[] array = (byte[])DLLMAIN_STUB.Clone();
					BitConverter.GetBytes(hModule.ToInt32()).CopyTo(array, 11);
					BitConverter.GetBytes(0u).CopyTo(array, 6);
					BitConverter.GetBytes(1000u).CopyTo(array, 1);
					zero = WinAPI.VirtualAllocEx(hProcess, IntPtr.Zero, (uint)DLLMAIN_STUB.Length, 12288, 64);
					if (zero.IsNull() || !WinAPI.WriteProcessMemory(hProcess, zero, array, array.Length, out lpNumberOfBytesRead) || lpNumberOfBytesRead != array.Length)
					{
						throw new InvalidOperationException("Unable to write stub to the remote process.");
					}
					IntPtr intPtr = WinAPI.CreateRemoteThread(hProcess, 0, 0, zero, (uint)hModule.Add(num).ToInt32(), 0, 0);
					if ((long)WinAPI.WaitForSingleObject(intPtr, 5000) == 0L)
					{
						WinAPI.VirtualFreeEx(hProcess, zero, 0, 32768);
						WinAPI.CloseHandle(intPtr);
						return WinAPI.VirtualFreeEx(hProcess, hModule, 0, 32768);
					}
					return false;
				}
				return WinAPI.VirtualFreeEx(hProcess, hModule, 0, 32768);
			}
			catch (Exception lastError)
			{
				SetLastError(lastError);
				return false;
			}
		}

		public override bool[] UnloadAll(IntPtr[] hModules, IntPtr hProcess)
		{
			ClearErrors();
			if (hModules == null)
			{
				throw new ArgumentNullException("hModules", "Parameter cannot be null.");
			}
			if (hProcess.IsNull() || hProcess.Compare(-1L))
			{
				throw new ArgumentOutOfRangeException("hProcess", "Invalid process handle specified.");
			}
			try
			{
				bool[] array = new bool[hModules.Length];
				for (int i = 0; i < hModules.Length; i++)
				{
					array[i] = Unload(hModules[i], hProcess);
				}
				return array;
			}
			catch (Exception lastError)
			{
				SetLastError(lastError);
				return null;
			}
		}
	}
}
