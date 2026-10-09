using System;
using System.Runtime.InteropServices;
using System.Text;

namespace TinhKiemAuto
{
	internal static class NativeMethods
	{
		public struct POINT
		{
			public int x;

			public int y;
		}

		public struct TCHITTESTINFO
		{
			public POINT pt;

			public uint flags;
		}

		public struct WINDOWPOS
		{
			public IntPtr hwnd;

			public IntPtr hwndInsertAfter;

			public int x;

			public int y;

			public int cx;

			public int cy;

			public int flags;
		}

		public const uint SRCCOPY = 13369376u;

		public const int TCM_HITTEST = 4877;

		public const int WM_SETFONT = 48;

		public const int WM_THEMECHANGED = 794;

		public const int WM_DESTROY = 2;

		public const int WM_NCDESTROY = 130;

		public const int WM_WINDOWPOSCHANGING = 70;

		public const int WM_PARENTNOTIFY = 528;

		public const int WM_CREATE = 1;

		public const int WM_MOUSEMOVE = 512;

		public const int WM_LBUTTONDOWN = 513;

		[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteDC(IntPtr hdc);

		[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

		[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

		[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, uint dwRop);

		[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

		[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

		[DllImport("gdi32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern uint SetPixel(IntPtr hdc, int X, int Y, uint crColor);

		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "SendMessageW")]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "RealGetWindowClassW")]
		public static extern uint RealGetWindowClass(IntPtr hWnd, StringBuilder ClassName, uint ClassNameMax);
	}
}
