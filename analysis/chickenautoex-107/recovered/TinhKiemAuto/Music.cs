using System;
using System.Runtime.InteropServices;
using System.Text;

namespace TinhKiemAuto
{
	internal class Music
	{
		private static bool isOpen = false;

		public static string path = "";

		public static string Path
		{
			get
			{
				return path;
			}
			set
			{
				path = value;
			}
		}

		private static void Open()
		{
			mciSendString("open \"" + Path + "\" type mpegvideo alias MediaFile", null, 0, IntPtr.Zero);
			isOpen = true;
		}

		private static void Pause()
		{
			try
			{
				mciSendString("stop MediaFile", null, 0, IntPtr.Zero);
			}
			catch
			{
			}
		}

		public static void Play()
		{
			if (!Global.Mute && !isOpen)
			{
				Open();
				mciSendString("play MediaFile REPEAT", null, 0, IntPtr.Zero);
			}
		}

		public static void ForcePlay()
		{
			if (!isOpen)
			{
				Open();
				mciSendString("play MediaFile REPEAT", null, 0, IntPtr.Zero);
			}
		}

		public static void Stop()
		{
			mciSendString("close MediaFile", null, 0, IntPtr.Zero);
			isOpen = false;
		}

		[DllImport("winmm.dll")]
		private static extern long mciSendString(string stay, StringBuilder strbuilder, int width, IntPtr sign);
	}
}
