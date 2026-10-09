using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TinhKiemAuto
{
	internal class Win
	{
		public enum SpecialWindowHandles
		{
			HWND_TOP = 0,
			HWND_BOTTOM = 1,
			HWND_TOPMOST = -1,
			HWND_NOTOPMOST = -2
		}

		[Flags]
		public enum SetWindowPosFlags : uint
		{
			SWP_ASYNCWINDOWPOS = 0x4000u,
			SWP_DEFERERASE = 0x2000u,
			SWP_DRAWFRAME = 0x20u,
			SWP_FRAMECHANGED = 0x20u,
			SWP_HIDEWINDOW = 0x80u,
			SWP_NOACTIVATE = 0x10u,
			SWP_NOCOPYBITS = 0x100u,
			SWP_NOMOVE = 2u,
			SWP_NOOWNERZORDER = 0x200u,
			SWP_NOREDRAW = 8u,
			SWP_NOREPOSITION = 0x200u,
			SWP_NOSENDCHANGING = 0x400u,
			SWP_NOSIZE = 1u,
			SWP_NOZORDER = 4u,
			SWP_SHOWWINDOW = 0x40u
		}

		public enum WindowLocation : byte
		{
			TopLeft,
			TopRight,
			BottomRight,
			BottomLeft,
			Center,
			None,
			TopCenter,
			RightCenter,
			BottomCenter,
			LeftCenter
		}

		public enum WindowShowStyle : uint
		{
			Hide = 0u,
			ShowNormal = 1u,
			ShowMinimized = 2u,
			ShowMaximized = 3u,
			Maximize = 3u,
			ShowNormalNoActivate = 4u,
			Show = 5u,
			Minimize = 6u,
			ShowMinNoActivate = 7u,
			ShowNoActivate = 8u,
			Restore = 9u,
			ShowDefault = 10u,
			ForceMinimized = 11u
		}

		public struct RECT
		{
			public int Left;

			public int Top;

			public int Right;

			public int Bottom;
		}

		private enum GetWindow_Cmd : uint
		{
			GW_HWNDFIRST,
			GW_HWNDLAST,
			GW_HWNDNEXT,
			GW_HWNDPREV,
			GW_OWNER,
			GW_CHILD,
			GW_ENABLEDPOPUP
		}

		public static string[] WndClassNames = new string[8] { "TianLongBaBu WndClass", "ThienLongHub WndClass", "ThienLongPri WndClass", "ThienLongTK2 WndClass", "TLBBTinhKiem2WndClass", "ThienLong KN WndClass", "TLBBPhatKhin WndClass", "#32770" };

		public static string[] GameExeProcessNames = new string[7] { "Game.exe", "Plugin_OgreManager.dll", "Plugin_Khin.dll", "Plugin_Game.dll", "Plugin_TinhKiem2.dll", "TLBB (32 bit)", "tConfig.exe" };

		public static int ScreenWidth => Screen.PrimaryScreen.Bounds.Width;

		public static int ScreenHight => Screen.PrimaryScreen.Bounds.Height - (Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

		public static IntPtr GetHandle(int processId, string className)
		{
			IntPtr window = GetWindow(GetForegroundWindow(), GetWindow_Cmd.GW_HWNDFIRST);
			StringBuilder stringBuilder = new StringBuilder(100);
			while (window != IntPtr.Zero)
			{
				GetClassName(window, stringBuilder, 100);
				if (stringBuilder.ToString().IndexOf(className) != -1)
				{
					int lpdwProcessId = 0;
					GetWindowThreadProcessId(window, out lpdwProcessId);
					if (lpdwProcessId == processId)
					{
						return window;
					}
				}
				window = GetWindow(window, GetWindow_Cmd.GW_HWNDNEXT);
			}
			return IntPtr.Zero;
		}

		public static IntPtr GetHandle(int processId, string[] classNames)
		{
			IntPtr window = GetWindow(GetForegroundWindow(), GetWindow_Cmd.GW_HWNDFIRST);
			StringBuilder stringBuilder = new StringBuilder(100);
			while (window != IntPtr.Zero)
			{
				GetClassName(window, stringBuilder, 100);
				foreach (string value in classNames)
				{
					if (stringBuilder.ToString().IndexOf(value) != -1)
					{
						int lpdwProcessId = 0;
						GetWindowThreadProcessId(window, out lpdwProcessId);
						if (lpdwProcessId == processId)
						{
							return window;
						}
					}
				}
				window = GetWindow(window, GetWindow_Cmd.GW_HWNDNEXT);
			}
			return IntPtr.Zero;
		}

		public static IntPtr GetHandle(string className, string windowText)
		{
			IntPtr window = GetWindow(GetForegroundWindow(), GetWindow_Cmd.GW_HWNDFIRST);
			StringBuilder stringBuilder = new StringBuilder(100);
			StringBuilder stringBuilder2 = new StringBuilder(100);
			while (window != IntPtr.Zero)
			{
				GetClassName(window, stringBuilder, 100);
				GetWindowText(window, stringBuilder2, 100);
				if (stringBuilder.ToString().IndexOf(className) != -1 && stringBuilder2.ToString().IndexOf(windowText) != -1)
				{
					return window;
				}
				window = GetWindow(window, GetWindow_Cmd.GW_HWNDNEXT);
			}
			return IntPtr.Zero;
		}

		public static HashSet<Process> GetProcessByClassName(string className)
		{
			HashSet<Process> hashSet = new HashSet<Process>();
			IntPtr window = GetWindow(GetForegroundWindow(), GetWindow_Cmd.GW_HWNDFIRST);
			StringBuilder stringBuilder = new StringBuilder(100);
			int lpdwProcessId = 0;
			while (window != IntPtr.Zero)
			{
				GetClassName(window, stringBuilder, 100);
				if (stringBuilder.ToString().IndexOf(className) != -1)
				{
					GetWindowThreadProcessId(window, out lpdwProcessId);
					hashSet.Add(Process.GetProcessById(lpdwProcessId));
				}
				window = GetWindow(window, GetWindow_Cmd.GW_HWNDNEXT);
			}
			return hashSet;
		}

		public static void Active(Form form)
		{
			form.Show();
			Active(form.Handle);
			form.Activate();
			form.Refresh();
		}

		public static void Active(IntPtr handle)
		{
			GetWindowRect(handle, out var lpRect);
			bool num = lpRect.Left == -32000;
			if (!IsWindowVisible(handle))
			{
				ShowWindow(handle, WindowShowStyle.Show);
			}
			if (num)
			{
				ShowWindow(handle, WindowShowStyle.Restore);
			}
			if (GetForegroundWindow() != handle)
			{
				SetForegroundWindow(handle);
			}
		}

		public static bool IsHideOrMini(IntPtr handle)
		{
			if (!IsWindowVisible(handle))
			{
				return IsWindowMini(handle);
			}
			return true;
		}

		public static bool IsWindowMini(IntPtr handle)
		{
			GetWindowRect(handle, out var lpRect);
			return lpRect.Left == -32000;
		}

		public static void Hide(IntPtr handle)
		{
			ShowWindow(handle, WindowShowStyle.Hide);
		}

		public static void MoveEx(Form form, WindowLocation location, int width, int height)
		{
			int width2 = Screen.PrimaryScreen.Bounds.Width;
			int height2 = Screen.PrimaryScreen.Bounds.Height;
			switch (location)
			{
			case WindowLocation.TopLeft:
				form.Location = new Point(0, 0);
				break;
			case WindowLocation.TopRight:
				form.Location = new Point(width2 - width, 0);
				break;
			case WindowLocation.BottomRight:
				form.Location = new Point(width2 - width, height2 - height - (Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom));
				break;
			case WindowLocation.BottomLeft:
				form.Location = new Point(0, height2 - height - (Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom));
				break;
			case WindowLocation.Center:
				form.Location = new Point((width2 - width) / 2, (height2 - height - (Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom)) / 2);
				break;
			case WindowLocation.None:
				form.Location = new Point(-1000, -1000);
				break;
			case WindowLocation.TopCenter:
				form.Location = new Point((width2 - width) / 2, 0);
				break;
			case WindowLocation.RightCenter:
				form.Location = new Point(width2 - width, (height2 - height - (Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom)) / 2);
				break;
			case WindowLocation.BottomCenter:
				form.Location = new Point((width2 - width) / 2, height2 - height - (Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom));
				break;
			case WindowLocation.LeftCenter:
				form.Location = new Point(0, (height2 - height - (Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom)) / 2);
				break;
			}
		}

		public static void Move(Form form, WindowLocation location)
		{
			int width = Screen.PrimaryScreen.Bounds.Width;
			int height = Screen.PrimaryScreen.Bounds.Height;
			switch (location)
			{
			case WindowLocation.TopLeft:
				form.Location = new Point(0, 0);
				break;
			case WindowLocation.TopRight:
				form.Location = new Point(width - form.Width, 0);
				break;
			case WindowLocation.BottomRight:
				form.Location = new Point(width - form.Width, height - form.Height - (Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom));
				break;
			case WindowLocation.BottomLeft:
				form.Location = new Point(0, height - form.Height - (Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom));
				break;
			case WindowLocation.Center:
				form.Location = new Point((width - form.Width) / 2, (height - form.Height - (Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom)) / 2);
				break;
			case WindowLocation.None:
				form.Location = new Point(-1000, -1000);
				break;
			case WindowLocation.TopCenter:
				form.Location = new Point((width - form.Width) / 2, 0);
				break;
			case WindowLocation.RightCenter:
				form.Location = new Point(width - form.Width, (height - form.Height - (Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom)) / 2);
				break;
			case WindowLocation.BottomCenter:
				form.Location = new Point((width - form.Width) / 2, height - form.Height - (Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom));
				break;
			case WindowLocation.LeftCenter:
				form.Location = new Point(0, (height - form.Height - (Screen.PrimaryScreen.Bounds.Bottom - Screen.PrimaryScreen.WorkingArea.Bottom)) / 2);
				break;
			}
		}

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll")]
		public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

		[DllImport("user32.dll")]
		public static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		[DllImport("user32.dll")]
		private static extern bool BringWindowToTop(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);

		[DllImport("user32.dll")]
		public static extern int SetWindowText(IntPtr hWnd, string text);

		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport("user32.dll")]
		private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool EnableWindow(IntPtr hwnd, bool enabled);

		[DllImport("user32.dll")]
		public static extern bool ShowWindowAsync(IntPtr hWnd, WindowShowStyle nCmdShow);
	}
}
