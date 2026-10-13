using System;
using System.IO;
using System.Text;

namespace TinhKiemAuto.Appearance
{
    internal static class FontDirectorySettings
    {
        private const int MaximumSettingsBytes = 16384;
        private static readonly string[][] Faces = {
            new[] { "Aptos.ttf" }, new[] { "Aptos-Bold.ttf", "AptosBold.ttf" },
            new[] { "Aptos-Italic.ttf", "AptosItalic.ttf" },
            new[] { "Aptos-Bold-Italic.ttf", "Aptos-BoldItalic.ttf", "AptosBoldItalic.ttf" }
        };
        internal static bool HasFourFaces(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory) || directory.Length > 4096 || !Path.IsPathRooted(directory)) return false;
            try
            {
                foreach (var aliases in Faces)
                {
                    bool found = false;
                    foreach (string name in aliases)
                    {
                        string path = Path.Combine(directory, name);
                        if (!File.Exists(path)) continue;
                        using (var stream = File.OpenRead(path))
                        {
                            if (stream.Length < 12 || stream.Length > 4 * 1024 * 1024) continue;
                            var signature = new byte[4];
                            if (stream.Read(signature, 0, 4) == 4 && signature[0] == 0 && signature[1] == 1 && signature[2] == 0 && signature[3] == 0) found = true;
                        }
                        if (found) break;
                    }
                    if (!found) return false;
                }
                return true;
            }
            catch (Exception) { return false; }
        }
        private static string SettingsPath(string localData) => Path.Combine(localData, "EAGLE Auto", "Appearance", "font-directory.txt");
        internal static string Read(string localData)
        {
            try
            {
                string path = SettingsPath(localData);
                var file = new FileInfo(path);
                if (!file.Exists || file.Length < 1 || file.Length > MaximumSettingsBytes) return null;
                string directory = File.ReadAllText(path, new UTF8Encoding(false, true)).Trim();
                return HasFourFaces(directory) ? directory : null;
            }
            catch (Exception) { return null; }
        }
        internal static bool Save(string localData, string directory)
        {
            try
            {
                if (!HasFourFaces(directory)) return false;
                string path = SettingsPath(localData);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, Path.GetFullPath(directory), new UTF8Encoding(false));
                return true;
            }
            catch (Exception) { return false; }
        }
    }
}
