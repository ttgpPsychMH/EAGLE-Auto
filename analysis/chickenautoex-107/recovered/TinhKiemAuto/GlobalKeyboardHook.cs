using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TinhKiemAuto
{
	internal class GlobalKeyboardHook
	{
		public delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct lParam);

		public struct keyboardHookStruct
		{
			public int vkCode;

			public int scanCode;

			public int flags;

			public int time;

			public int dwExtraInfo;
		}

		public static keyboardHookProc _keyboardHookProc;

		private const int WH_KEYBOARD_LL = 13;

		private const int WM_KEYDOWN = 256;

		private const int WM_KEYUP = 257;

		private const int WM_SYSKEYDOWN = 260;

		private const int WM_SYSKEYUP = 261;

		public List<Keys> HookedKeys = new List<Keys>();

		private IntPtr hhook = IntPtr.Zero;

		[CompilerGenerated]
		public event KeyEventHandler KeyDown;

		[CompilerGenerated]
		public event KeyEventHandler KeyUp;

		[CompilerGenerated]
		public event EventHandler DisTruct;

		public GlobalKeyboardHook()
		{
			Hook();
		}

		~GlobalKeyboardHook()
		{
			UnHook();
			if (this.DisTruct != null)
			{
				this.DisTruct(null, null);
			}
		}

		public void Hook()
		{
			IntPtr hInstance = LoadLibrary("User32");
			_keyboardHookProc = hookProc;
			hhook = SetWindowsHookEx(13, _keyboardHookProc, hInstance, 0u);
		}

		public void UnHook()
		{
			UnhookWindowsHookEx(hhook);
		}

		public int hookProc(int code, int wParam, ref keyboardHookStruct lParam)
		{
			if (code >= 0)
			{
				Keys vkCode = (Keys)lParam.vkCode;
				if (HookedKeys.Contains(vkCode))
				{
					KeyEventArgs e = new KeyEventArgs(vkCode);
					if ((wParam == 256 || wParam == 260) && this.KeyDown != null)
					{
						this.KeyDown(this, e);
					}
					else if ((wParam == 257 || wParam == 261) && this.KeyUp != null)
					{
						this.KeyUp(this, e);
					}
					if (e.Handled)
					{
						return 1;
					}
				}
			}
			return CallNextHookEx(hhook, code, wParam, ref lParam);
		}

		[DllImport("user32.dll")]
		private static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);

		[DllImport("user32.dll")]
		private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

		[DllImport("user32.dll")]
		private static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref keyboardHookStruct lParam);

		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibrary(string lpFileName);
	}
}
