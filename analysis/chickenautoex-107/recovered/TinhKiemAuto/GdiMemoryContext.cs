using System;
using System.Drawing;

namespace TinhKiemAuto
{
	internal class GdiMemoryContext : IDisposable
	{
		private IntPtr fDC;

		private IntPtr fBitmap;

		private IntPtr fStockMonoBmp;

		private int fWidth;

		private int fHeight;

		private Graphics gdiPlusContext;

		public Graphics Graphics => gdiPlusContext;

		public int Width => fWidth;

		public int Height => fHeight;

		public GdiMemoryContext(Graphics compatibleTo, int width, int height)
		{
			if (compatibleTo == null || width <= 0 || height <= 0)
			{
				throw new ArgumentException("Arguments are unacceptable");
			}
			IntPtr hdc = compatibleTo.GetHdc();
			bool flag = true;
			if (!((fDC = NativeMethods.CreateCompatibleDC(hdc)) == IntPtr.Zero))
			{
				if ((fBitmap = NativeMethods.CreateCompatibleBitmap(hdc, width, height)) == IntPtr.Zero)
				{
					NativeMethods.DeleteDC(fDC);
				}
				else
				{
					fStockMonoBmp = NativeMethods.SelectObject(fDC, fBitmap);
					if (fStockMonoBmp == IntPtr.Zero)
					{
						NativeMethods.DeleteObject(fBitmap);
						NativeMethods.DeleteDC(fDC);
					}
					else
					{
						flag = false;
					}
				}
			}
			compatibleTo.ReleaseHdc(hdc);
			if (flag)
			{
				throw new SystemException("GDI error occured while creating context");
			}
			gdiPlusContext = Graphics.FromHdc(fDC);
			fWidth = width;
			fHeight = height;
		}

		~GdiMemoryContext()
		{
			Dispose(disposing: false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && gdiPlusContext != null)
			{
				gdiPlusContext.Dispose();
			}
			NativeMethods.SelectObject(fDC, fStockMonoBmp);
			NativeMethods.DeleteDC(fDC);
			fDC = (fStockMonoBmp = IntPtr.Zero);
			NativeMethods.DeleteObject(fBitmap);
			fBitmap = IntPtr.Zero;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public void FlipVertical()
		{
			if (fDC != IntPtr.Zero)
			{
				NativeMethods.StretchBlt(fDC, 0, fHeight - 1, fWidth, -fHeight, fDC, 0, 0, fWidth, fHeight, 13369376u);
			}
		}

		public uint GetPixel(int x, int y)
		{
			if (fDC != IntPtr.Zero)
			{
				return NativeMethods.GetPixel(fDC, x, y);
			}
			throw new ObjectDisposedException(null, "GDI context seems to be disposed.");
		}

		public void SetPixel(int x, int y, uint value)
		{
			if (fDC != IntPtr.Zero)
			{
				NativeMethods.SetPixel(fDC, x, y, value);
				return;
			}
			throw new ObjectDisposedException(null, "GDI context seems to be disposed.");
		}

		public void DrawContextClipped(Graphics drawTo, Rectangle drawRect)
		{
			if (drawTo != null && !(fDC == IntPtr.Zero))
			{
				IntPtr hdc = drawTo.GetHdc();
				if (!(hdc == IntPtr.Zero))
				{
					NativeMethods.BitBlt(hdc, drawRect.Left, drawRect.Top, drawRect.Width, drawRect.Height, fDC, 0, 0, 13369376u);
					drawTo.ReleaseHdc(hdc);
				}
			}
		}
	}
}
