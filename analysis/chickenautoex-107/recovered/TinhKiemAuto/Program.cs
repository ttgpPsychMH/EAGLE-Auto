using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace TinhKiemAuto
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			Mutex mutex = new Mutex(initiallyOwned: false, "MyUniqueMutexName");
			try
			{
				AntiDump.Initialize();
				if (mutex.WaitOne(0, exitContext: false))
				{
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(defaultValue: false);
					CheckTaiNguyen();
					Application.Run(new FrmMain());
				}
				else
				{
					MessageBox.Show("Đã có phiên bản [ChickenAuto] khác đang hoạt động\nNếu bạn chắc chắn rằng Auto không bật\nVui lòng khởi động lại máy tính và thử lại. ", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
			finally
			{
				if (mutex != null)
				{
					mutex.Close();
					mutex = null;
				}
			}
		}

		public static bool IsUserAdministrator()
		{
			try
			{
				return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
			}
			catch (UnauthorizedAccessException)
			{
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal static bool IsRunAsAdmin()
		{
			return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
		}

		public static void CheckTaiNguyen()
		{
			TINHKIEM.FileInstall("TinhKiemAuto", "EasyHook.dll", Global.APPPath + "\\Bin\\EasyHook.dll");
			TINHKIEM.FileInstall("TinhKiemAuto", "Newtonsoft.Json.dll", Global.APPPath + "\\Newtonsoft.Json.dll");
		}

		private static bool Elevate()
		{
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				UseShellExecute = true,
				WorkingDirectory = Environment.CurrentDirectory,
				FileName = Application.ExecutablePath,
				Verb = "runas"
			};
			try
			{
				Process.Start(startInfo);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
