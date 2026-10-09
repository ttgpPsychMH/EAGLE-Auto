using System.IO;

namespace TinhKiemAuto
{
	internal static class PathHelper
	{
		public static string GetWithBackslash(string path)
		{
			if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
			{
				path += Path.DirectorySeparatorChar;
			}
			return path;
		}
	}
}
