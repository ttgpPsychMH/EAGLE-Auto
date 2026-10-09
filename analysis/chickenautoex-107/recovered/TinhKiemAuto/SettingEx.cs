namespace TinhKiemAuto
{
	internal class SettingEx
	{
		public string this[string val]
		{
			get
			{
				return Settings.Read(val);
			}
			set
			{
				Settings.Write(val, value);
			}
		}
	}
}
