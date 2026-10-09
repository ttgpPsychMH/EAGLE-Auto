using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace TinhKiemAuto
{
	public abstract class StandardInjectionMethod : InjectionMethod
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class InterFaceInject
		{
			public static readonly InterFaceInject thanh = new InterFaceInject();

			public static Converter<PortableExecutable, string> thanh__5_0;

			internal string TimString(PortableExecutable pe)
			{
				return Utils.WriteTempData(pe.ToArray());
			}
		}

		protected static readonly byte[] MULTILOAD_STUB = new byte[103]
		{
			85, 139, 236, 131, 236, 12, 185, 0, 0, 0,
			0, 137, 12, 36, 185, 0, 0, 0, 0, 137,
			76, 36, 4, 185, 0, 0, 0, 0, 137, 76,
			36, 8, 139, 76, 36, 4, 131, 249, 0, 116,
			58, 131, 233, 1, 137, 76, 36, 4, 255, 52,
			36, 232, 0, 0, 0, 0, 131, 248, 0, 117,
			8, 255, 52, 36, 232, 0, 0, 0, 0, 139,
			76, 36, 8, 137, 1, 131, 193, 4, 137, 76,
			36, 8, 139, 12, 36, 138, 1, 131, 193, 1,
			60, 0, 117, 247, 137, 12, 36, 235, 189, 139,
			229, 93, 195
		};

		protected static readonly byte[] MULTIUNLOAD_STUB = new byte[95]
		{
			85, 139, 236, 131, 236, 12, 185, 0, 0, 0,
			0, 137, 12, 36, 185, 0, 0, 0, 0, 137,
			76, 36, 4, 139, 12, 36, 139, 9, 131, 249,
			0, 116, 58, 137, 76, 36, 8, 139, 76, 36,
			4, 199, 1, 0, 0, 0, 0, 255, 116, 36,
			8, 232, 0, 0, 0, 0, 131, 248, 0, 116,
			8, 139, 76, 36, 4, 137, 1, 235, 234, 139,
			12, 36, 131, 193, 4, 137, 12, 36, 139, 76,
			36, 4, 131, 193, 4, 137, 76, 36, 4, 235,
			188, 139, 229, 93, 195
		};

		protected virtual IntPtr CreateMultiLoadStub(string[] paths, IntPtr hProcess, out IntPtr pModuleBuffer, uint nullmodule = 0u)
		{
			pModuleBuffer = IntPtr.Zero;
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				IntPtr moduleHandleA = WinAPI.GetModuleHandleA("kernel32.dll");
				IntPtr procAddress = WinAPI.GetProcAddress(moduleHandleA, "LoadLibraryA");
				IntPtr procAddress2 = WinAPI.GetProcAddress(moduleHandleA, "GetModuleHandleA");
				if (procAddress.IsNull() || procAddress2.IsNull())
				{
					throw new Exception("Unable to find necessary function entry points in the remote process");
				}
				pModuleBuffer = WinAPI.VirtualAllocEx(hProcess, IntPtr.Zero, (uint)(paths.Length << 2), 12288, 4);
				IntPtr intPtr2 = WinAPI.CreateRemotePointer(hProcess, Encoding.ASCII.GetBytes(string.Join("\0", paths) + "\0"), 4);
				if (pModuleBuffer.IsNull() || intPtr2.IsNull())
				{
					throw new InvalidOperationException("Unable to allocate memory in the remote process");
				}
				try
				{
					uint lpNumberOfBytesRead = 0u;
					byte[] array = new byte[paths.Length << 2];
					for (int i = 0; i < array.Length >> 2; i++)
					{
						BitConverter.GetBytes(nullmodule).CopyTo(array, i << 2);
					}
					WinAPI.WriteProcessMemory(hProcess, pModuleBuffer, array, array.Length, out lpNumberOfBytesRead);
					byte[] array2 = (byte[])MULTILOAD_STUB.Clone();
					intPtr = WinAPI.VirtualAllocEx(hProcess, IntPtr.Zero, (uint)array2.Length, 12288, 64);
					if (intPtr.IsNull())
					{
						throw new InvalidOperationException("Unable to allocate memory in the remote process");
					}
					BitConverter.GetBytes(intPtr2.ToInt32()).CopyTo(array2, 7);
					BitConverter.GetBytes(paths.Length).CopyTo(array2, 15);
					BitConverter.GetBytes(pModuleBuffer.ToInt32()).CopyTo(array2, 24);
					BitConverter.GetBytes(procAddress2.Subtract(intPtr.Add(56L)).ToInt32()).CopyTo(array2, 52);
					BitConverter.GetBytes(procAddress.Subtract(intPtr.Add(69L)).ToInt32()).CopyTo(array2, 65);
					if (!WinAPI.WriteProcessMemory(hProcess, intPtr, array2, array2.Length, out lpNumberOfBytesRead) || lpNumberOfBytesRead != array2.Length)
					{
						throw new Exception("Error creating the remote function stub.");
					}
					return intPtr;
				}
				finally
				{
					WinAPI.VirtualFreeEx(hProcess, pModuleBuffer, 0, 32768);
					WinAPI.VirtualFreeEx(hProcess, intPtr2, 0, 32768);
					if (!intPtr.IsNull())
					{
						WinAPI.VirtualFreeEx(hProcess, intPtr, 0, 32768);
					}
					pModuleBuffer = IntPtr.Zero;
				}
			}
			catch (Exception lastError)
			{
				SetLastError(lastError);
				return IntPtr.Zero;
			}
		}

		public override IntPtr Inject(PortableExecutable dll, IntPtr hProcess)
		{
			ClearErrors();
			string text = Utils.WriteTempData(dll.ToArray());
			IntPtr result = IntPtr.Zero;
			if (!string.IsNullOrEmpty(text))
			{
				result = Inject(text, hProcess);
				try
				{
					File.Delete(text);
				}
				catch
				{
				}
			}
			return result;
		}

		public override IntPtr[] InjectAll(PortableExecutable[] dlls, IntPtr hProcess)
		{
			ClearErrors();
			Converter<PortableExecutable, string> converter;
			if ((converter = InterFaceInject.thanh__5_0) == null)
			{
				converter = (InterFaceInject.thanh__5_0 = (PortableExecutable pe) => Utils.WriteTempData(pe.ToArray()));
			}
			return InjectAll(Array.ConvertAll(dlls, converter), hProcess);
		}

		public override bool Unload(IntPtr hModule, IntPtr hProcess)
		{
			ClearErrors();
			if (hProcess.IsNull() || hProcess.Compare(-1L))
			{
				throw new ArgumentOutOfRangeException("hProcess", "Invalid process handle specified.");
			}
			if (hModule.IsNull())
			{
				throw new ArgumentNullException("hModule", "Invalid module handle");
			}
			try
			{
				IntPtr[] hModules = new IntPtr[1] { hModule };
				bool[] array = UnloadAll(hModules, hProcess);
				return array != null && array.Length != 0 && array[0];
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
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			IntPtr intPtr3 = IntPtr.Zero;
			try
			{
				uint lpNumberOfBytesRead = 0u;
				IntPtr procAddress = WinAPI.GetProcAddress(WinAPI.GetModuleHandleA("kernel32.dll"), "FreeLibrary");
				if (procAddress.IsNull())
				{
					throw new Exception("Unable to find necessary function entry points in the remote process");
				}
				intPtr = WinAPI.VirtualAllocEx(hProcess, IntPtr.Zero, (uint)(hModules.Length << 2), 12288, 4);
				intPtr2 = WinAPI.VirtualAllocEx(hProcess, IntPtr.Zero, (uint)(hModules.Length + 1 << 2), 12288, 4);
				intPtr3 = WinAPI.VirtualAllocEx(hProcess, IntPtr.Zero, (uint)MULTIUNLOAD_STUB.Length, 12288, 64);
				if (intPtr.IsNull() || intPtr2.IsNull() || intPtr3.IsNull())
				{
					throw new InvalidOperationException("Unable to allocate memory in the remote process");
				}
				byte[] array = new byte[hModules.Length + 1 << 2];
				for (int i = 0; i < hModules.Length; i++)
				{
					BitConverter.GetBytes(hModules[i].ToInt32()).CopyTo(array, i << 2);
				}
				WinAPI.WriteProcessMemory(hProcess, intPtr2, array, array.Length, out lpNumberOfBytesRead);
				byte[] array2 = (byte[])MULTIUNLOAD_STUB.Clone();
				BitConverter.GetBytes(intPtr2.ToInt32()).CopyTo(array2, 7);
				BitConverter.GetBytes(intPtr.ToInt32()).CopyTo(array2, 15);
				BitConverter.GetBytes(procAddress.Subtract(intPtr3.Add(56L)).ToInt32()).CopyTo(array2, 52);
				if (!WinAPI.WriteProcessMemory(hProcess, intPtr3, array2, array2.Length, out lpNumberOfBytesRead) || lpNumberOfBytesRead != array2.Length)
				{
					throw new InvalidOperationException("Unable to write the function stub to the remote process.");
				}
				if (WinAPI.RunThread(hProcess, intPtr3, 0u) == uint.MaxValue)
				{
					throw new InvalidOperationException("Error occurred when running remote function stub.");
				}
				byte[] array3 = WinAPI.ReadRemoteMemory(hProcess, intPtr, (uint)(hModules.Length << 2));
				if (array3 == null)
				{
					throw new Exception("Unable to read results from the remote process.");
				}
				bool[] array4 = new bool[hModules.Length];
				for (int j = 0; j < array4.Length; j++)
				{
					array4[j] = BitConverter.ToInt32(array3, j << 2) != 0;
				}
				return array4;
			}
			catch (Exception lastError)
			{
				SetLastError(lastError);
				return null;
			}
			finally
			{
				WinAPI.VirtualFreeEx(hProcess, intPtr3, 0, 32768);
				WinAPI.VirtualFreeEx(hProcess, intPtr, 0, 32768);
				WinAPI.VirtualFreeEx(hProcess, intPtr2, 0, 32768);
			}
		}
	}
}
