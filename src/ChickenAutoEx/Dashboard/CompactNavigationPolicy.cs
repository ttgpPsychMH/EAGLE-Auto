namespace TinhKiemAuto.Dashboard
{
    internal static class CompactNavigationPolicy
    {
        internal static readonly string[] Captions = { "Cơ bản", "Kỹ năng", "Tiện ích", "Nhiệm vụ" };
        internal static readonly int[] AdvancedIndexes = { 2, 3, 5, 6 };
        private static readonly int[] MainIndexes = { 0, 1, 4, 7 };
        internal static int LegacyIndex(int front) { return front >= 0 && front < MainIndexes.Length ? MainIndexes[front] : -1; }
        internal static int FrontIndex(int legacy)
        {
            for (int i = 0; i < MainIndexes.Length; i++) if (MainIndexes[i] == legacy) return i;
            return -1;
        }
    }
}
