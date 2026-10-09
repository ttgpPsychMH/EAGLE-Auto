namespace TinhKiemAuto
{
	internal interface IExtension
	{
		string Name { get; }

		IUIExtension UIExtension { get; }
	}
}
