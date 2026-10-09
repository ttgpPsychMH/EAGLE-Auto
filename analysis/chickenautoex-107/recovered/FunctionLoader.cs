using System;
using System.Runtime.InteropServices;

internal class FunctionLoader
{
	[DllImport("Kernel32.dll")]
	private static extern IntPtr LoadLibrary(string path);

	[DllImport("Kernel32.dll")]
	private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

	public static Delegate LoadFunction<T>(string dllPath, string functionName)
	{
		return Marshal.GetDelegateForFunctionPointer(GetProcAddress(LoadLibrary(dllPath), functionName), typeof(T));
	}
}
