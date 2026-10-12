using System;
using System.Globalization;

namespace TinhKiemAuto.Dashboard
{
    // Presentation only. No native reads, credentials, game commands or setting writes.
    internal sealed class CharacterHeader
    {
        internal string Title, Hint, HP = "—", MP = "—", Pet = "—", Position = "—", Online = "—";
        internal bool CanConfigure;

        internal static CharacterHeader Empty(int selectedCount)
        {
            return new CharacterHeader {
                Title = selectedCount > 1 ? "Đang chọn nhiều nhân vật" : "Chưa chọn nhân vật",
                Hint = "Chọn một dòng để cấu hình. Ô tick bật/tắt Auto."
            };
        }
        internal static CharacterHeader Unavailable(string name, bool online)
        {
            return new CharacterHeader { Title = string.IsNullOrEmpty(name) ? "Chờ nhận diện nhân vật" : name,
                Hint = online ? "Chờ nhận diện nhân vật." : "Nhân vật chưa online." };
        }
        internal static CharacterHeader Live(string name, int hp, int mp, int pet, string position, long seconds, bool auto)
        {
            return new CharacterHeader { Title = string.IsNullOrEmpty(name) ? "Nhân vật đã chọn" : name,
                HP = Percent(hp), MP = Percent(mp), Pet = Percent(pet),
                Position = string.IsNullOrEmpty(position) ? "—" : position,
                Online = Duration(seconds), CanConfigure = true,
                Hint = auto ? "Auto đang bật • cấu hình nhân vật được chọn" : "Auto đang tắt • có thể cấu hình trước khi bật" };
        }
        private static string Percent(int value) { return value >= 0 && value <= 100 ? value + "%" : "—"; }
        internal static string Location(string map, float x, float y)
        {
            if (string.IsNullOrEmpty(map) || float.IsNaN(x) || float.IsNaN(y) || float.IsInfinity(x)
                || float.IsInfinity(y) || x < int.MinValue || x >= int.MaxValue || y < int.MinValue || y >= int.MaxValue)
                return "—";
            return map + " [" + (int)x + "," + (int)y + "]";
        }
        private static string Duration(long seconds)
        {
            if (seconds < 0) return "—";
            long hours = seconds / 3600;
            return hours.ToString("00", CultureInfo.InvariantCulture) + ":"
                + (seconds / 60 % 60).ToString("00", CultureInfo.InvariantCulture) + ":"
                + (seconds % 60).ToString("00", CultureInfo.InvariantCulture);
        }
    }

    internal sealed class CompactPanelBudget
    {
        internal float Shared, Log;
        internal static CompactPanelBudget Allocate(float clientHeight, float fixedHeight, float minimumContent,
            float sharedRequested, float logRequested)
        {
            float available = Math.Max(0, clientHeight - fixedHeight - minimumContent);
            float shared = Math.Min(Math.Max(0, sharedRequested), available);
            float log = Math.Min(Math.Max(0, logRequested), available - shared);
            // A tiny log strip is unusable; retain its preference and restore it when space returns.
            return new CompactPanelBudget { Shared = shared, Log = log >= 30 ? log : 0 };
        }
    }
}
