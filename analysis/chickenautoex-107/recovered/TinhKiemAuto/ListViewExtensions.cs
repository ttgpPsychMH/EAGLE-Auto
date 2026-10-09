using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TinhKiemAuto
{
	internal class ListViewExtensions
	{
		public struct LVCOLUMN
		{
			public int mask;

			public int cx;

			[MarshalAs(UnmanagedType.LPTStr)]
			public string pszText;

			public IntPtr hbm;

			public int cchTextMax;

			public int fmt;

			public int iSubItem;

			public int iImage;

			public int iOrder;
		}

		private const int HDI_WIDTH = 1;

		private const int HDI_HEIGHT = 1;

		private const int HDI_TEXT = 2;

		private const int HDI_FORMAT = 4;

		private const int HDI_LPARAM = 8;

		private const int HDI_BITMAP = 16;

		private const int HDI_IMAGE = 32;

		private const int HDI_DI_SETITEM = 64;

		private const int HDI_ORDER = 128;

		private const int HDI_FILTER = 256;

		private const int HDF_LEFT = 0;

		private const int HDF_RIGHT = 1;

		private const int HDF_CENTER = 2;

		private const int HDF_JUSTIFYMASK = 3;

		private const int HDF_RTLREADING = 4;

		private const int HDF_OWNERDRAW = 32768;

		private const int HDF_STRING = 16384;

		private const int HDF_BITMAP = 8192;

		private const int HDF_BITMAP_ON_RIGHT = 4096;

		private const int HDF_IMAGE = 2048;

		private const int HDF_SORTUP = 1024;

		private const int HDF_SORTDOWN = 512;

		private const int LVM_FIRST = 4096;

		private const int LVM_GETHEADER = 4127;

		private const int HDM_FIRST = 4608;

		private const int HDM_SETIMAGELIST = 4616;

		private const int HDM_GETIMAGELIST = 4617;

		private const int HDM_GETITEM = 4619;

		private const int HDM_SETITEM = 4620;

		[DllImport("user32.dll")]
		private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", EntryPoint = "SendMessage")]
		private static extern IntPtr SendMessageLVCOLUMN(IntPtr hWnd, int Msg, IntPtr wParam, ref LVCOLUMN lPLVCOLUMN);

		public static void SetSortIcon(ListView listView, int columnIndex, SortOrder order)
		{
			IntPtr hWnd = SendMessage(listView.Handle, 4127u, IntPtr.Zero, IntPtr.Zero);
			for (int i = 0; i <= listView.Columns.Count - 1; i++)
			{
				IntPtr wParam = new IntPtr(i);
				LVCOLUMN lPLVCOLUMN = new LVCOLUMN
				{
					mask = 4
				};
				SendMessageLVCOLUMN(hWnd, 4619, wParam, ref lPLVCOLUMN);
				if (order != SortOrder.None && i == columnIndex)
				{
					switch (order)
					{
					case SortOrder.Descending:
						lPLVCOLUMN.fmt &= -1025;
						lPLVCOLUMN.fmt |= 512;
						break;
					case SortOrder.Ascending:
						lPLVCOLUMN.fmt &= -513;
						lPLVCOLUMN.fmt |= 1024;
						break;
					}
					lPLVCOLUMN.fmt |= 4096;
				}
				else
				{
					lPLVCOLUMN.fmt &= -5633;
				}
				SendMessageLVCOLUMN(hWnd, 4620, wParam, ref lPLVCOLUMN);
			}
		}
	}
}
