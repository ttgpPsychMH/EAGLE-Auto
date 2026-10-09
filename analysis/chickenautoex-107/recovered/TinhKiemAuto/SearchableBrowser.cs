namespace TinhKiemAuto
{
	public interface SearchableBrowser
	{
		bool Search(string text, bool forward, bool matchWholeWord, bool matchCase);
	}
}
