using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TinhKiemAuto.Appearance
{
    internal enum CopyRole { Body, Heading1, Heading2 }
    internal static class UiCopyPolicy
    {
        private static readonly CultureInfo Vietnamese = CultureInfo.GetCultureInfo("vi-VN");
        private static readonly Regex[] NamePatterns = UiCopyCatalog.ProtectedNames.Select(name =>
            new Regex(@"(?<![\p{L}\p{N}])" + Regex.Escape(name) + @"(?![\p{L}\p{N}])",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).ToArray();
        internal static bool TryFormat(string original, CopyRole role, out string rendered)
        {
            rendered = original;
            if (original == null) return false;
            string canonical;
            if (!UiCopyCatalog.Captions.TryGetValue(original.Normalize(NormalizationForm.FormC), out canonical)) return false;
            if (role == CopyRole.Heading1) rendered = canonical.ToUpper(Vietnamese);
            else if (role == CopyRole.Heading2) rendered = Vietnamese.TextInfo.ToTitleCase(canonical.ToLower(Vietnamese));
            else rendered = canonical;
            // Only known UI copy is formatted. Game data never passes through a global case conversion.
            for (int i = 0; i < NamePatterns.Length; i++)
                rendered = NamePatterns[i].Replace(rendered, UiCopyCatalog.ProtectedNames[i]);
            rendered = Regex.Replace(rendered, @"(\+\s*)([a-z])(?![\p{L}\p{N}])",
                match => match.Groups[1].Value + match.Groups[2].Value.ToUpperInvariant());
            return true;
        }
    }
}
