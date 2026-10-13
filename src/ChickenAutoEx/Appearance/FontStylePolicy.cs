using System;

namespace TinhKiemAuto.Appearance
{
    // Kept independent of WinForms/GDI so the selection rules can be tested
    // without loading the application or any user-supplied font.
    internal static class FontStylePolicy
    {
        internal const string PreferredFamily = "Aptos";
        internal const string FallbackFamily = "Segoe UI";
        internal const float DefaultPointSize = 9f;

        internal static string ResolveFamilyName(bool privateAptosAvailable, bool installedAptosAvailable)
        {
            return ResolveSource(privateAptosAvailable, installedAptosAvailable) == FontSource.SegoeUi
                ? FallbackFamily : PreferredFamily;
        }

        internal static FontSource ResolveSource(bool privateAptosAvailable, bool installedAptosAvailable)
        {
            return privateAptosAvailable ? FontSource.PrivateAptos
                : installedAptosAvailable ? FontSource.InstalledAptos : FontSource.SegoeUi;
        }

        internal static float PreservePointSize(float pointSize)
        {
            return float.IsNaN(pointSize) || float.IsInfinity(pointSize) || pointSize <= 0f
                ? DefaultPointSize : Math.Max(DefaultPointSize, pointSize);
        }

        internal static int PreserveStyleBits(int style)
        {
            // FontStyle: Bold=1, Italic=2, Underline=4, Strikeout=8.
            return style & 15;
        }

        internal static int FaceStyleBits(int style)
        {
            return PreserveStyleBits(style) & 3;
        }

        internal static bool IsPreferredFamily(string familyName)
        {
            return string.Equals(familyName, PreferredFamily, StringComparison.OrdinalIgnoreCase);
        }
    }

    internal enum FontSource { PrivateAptos, InstalledAptos, SegoeUi }
}
