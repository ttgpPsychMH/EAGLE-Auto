using System.Windows.Forms;

namespace TinhKiemAuto
{
	internal interface IUIExtension
	{
		Control[] CreateSettingsView();

		void PersistSettings(Control[] settingsView);
	}
}
