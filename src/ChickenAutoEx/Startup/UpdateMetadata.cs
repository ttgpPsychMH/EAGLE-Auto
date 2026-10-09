using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ChickenAutoEx.Startup
{
    internal static class UpdateMetadata
    {
        internal static UpdateResult Parse(string content, int currentVersion)
        {
            if (string.IsNullOrWhiteSpace(content) || content.IndexOf('<') >= 0 || content.IndexOf('>') >= 0)
                return Invalid();

            int? version = null;
            foreach (string original in content.TrimStart('\ufeff').Split(new[] { '\r', '\n' }))
            {
                string line = original.Trim();
                if (line.Length == 0 || line.StartsWith(";") || line.StartsWith("#")) continue;
                int equals = line.IndexOf('=');
                if (equals < 1) return Invalid();
                string key = line.Substring(0, equals).Trim();
                if (!Regex.IsMatch(key, "^[A-Za-z][A-Za-z0-9_]*$")) return Invalid();
                if (key != "Version") continue;

                int parsed;
                if (version.HasValue || !int.TryParse(line.Substring(equals + 1).Trim(),
                    NumberStyles.None, CultureInfo.InvariantCulture, out parsed) || parsed < 1)
                    return Invalid();
                version = parsed;
            }
            if (!version.HasValue) return Invalid();
            return new UpdateResult(version.Value > currentVersion ? UpdateStatus.UpdateAvailable : UpdateStatus.UpToDate,
                "VersionChecked", version.Value);
        }

        private static UpdateResult Invalid()
        {
            return new UpdateResult(UpdateStatus.InvalidMetadata, "InvalidIni");
        }
    }
}
