using System;

namespace TinhKiemAuto.Dashboard
{
    internal enum DungeonKind { TrungAc, KyCuoc, AcTac, AcBa, ThuyLao }

    // Object identity and login identity are both required. Never render/log these IDs.
    internal sealed class DungeonBinding
    {
        internal object Selected, Controller;
        internal string SelectedId, ControllerId, TeamId;
        internal bool Matches(DungeonBinding other)
        {
            return other != null && ReferenceEquals(Selected, other.Selected)
                && ReferenceEquals(Controller, other.Controller) && SelectedId == other.SelectedId
                && ControllerId == other.ControllerId && TeamId == other.TeamId;
        }
    }

    internal sealed class DungeonContext
    {
        internal DungeonBinding Binding;
        internal string SelectedName, ControllerName, Phase;
        internal bool SelectedPresent, ControllerPresent, SameTeam, Online, Initialized, Auto;
        internal bool Paused, ChangingMap, Busy, Dead, Following, Conflict, Active;
    }

    internal sealed class DungeonView
    {
        internal DungeonContext Context;
        internal bool CanStart, CanStop;
        internal string State, Reason;
    }

    internal static class DungeonDashboardPolicy
    {
        internal static bool IsPersonal(DungeonKind kind) { return kind == DungeonKind.TrungAc; }
        internal static string Name(DungeonKind kind)
        {
            switch (kind)
            {
                case DungeonKind.TrungAc: return "Trừng Ác";
                case DungeonKind.KyCuoc: return "Trân Long Kỳ Cuộc";
                case DungeonKind.AcTac: return "Ác Tặc";
                case DungeonKind.AcBa: return "Ác Bá";
                default: return "Thủy Lao";
            }
        }
        internal static bool ValidOption(DungeonKind kind, int option)
        {
            if (kind == DungeonKind.AcTac) return option == 0 || (option >= 3 && option <= 8);
            if (kind == DungeonKind.AcBa) return option == -1 || (option >= 1 && option <= 9) || option == 32 || option == 37;
            return (kind == DungeonKind.TrungAc || kind == DungeonKind.KyCuoc || kind == DungeonKind.ThuyLao) && option == 0;
        }
        internal static DungeonView Evaluate(DungeonKind kind, DungeonContext context)
        {
            var view = new DungeonView { Context = context, State = "Chưa bật / đã dừng" };
            string reason = null;
            if (context == null || !context.SelectedPresent) reason = "Chọn một nhân vật trong danh sách.";
            else if (!context.ControllerPresent) reason = "Chưa có đội trưởng trong các cửa sổ game được nhận diện.";
            else if (!context.SameTeam) reason = "Đội trưởng hoặc tổ đội đã thay đổi; chọn lại nhân vật.";
            else if (!context.Online) reason = "Nhân vật hoặc đội trưởng chưa online.";
            else if (!context.Initialized) reason = "Chờ engine nhận diện và khởi tạo nhân vật.";
            else if (!context.Auto) reason = "Chưa bật Auto cho nhân vật hoặc đội trưởng.";
            else if (context.Dead) reason = "Nhân vật hoặc đội trưởng chết / ở Giám Ngục.";
            else if (context.Paused) reason = "Auto đang tạm dừng.";
            else if (context.ChangingMap) reason = "Chờ chuyển cảnh / chuyển map.";
            else if (context.Busy) reason = "Chờ nhân vật hoặc đội trưởng sẵn sàng.";
            else if (IsPersonal(kind) && context.Following) reason = "Dừng theo đội trước khi làm Trừng Ác cá nhân.";
            else if (context.Conflict) reason = "Đang có nhiệm vụ khác; dừng nhiệm vụ đó trước khi bắt đầu.";
            // Stop is permitted while paused, offline, dead, or after Auto was switched off.
            view.CanStop = context != null && context.SelectedPresent && context.ControllerPresent
                && context.SameTeam && context.Active;
            view.CanStart = reason == null && context != null && !context.Active;
            if (context != null && context.Active)
            {
                view.State = reason == null ? "Đã bật — theo dõi engine" : "Đã bật — chưa sẵn sàng";
                view.Reason = reason == null ? context.Phase ?? "Chờ phản hồi từ engine; chưa xác nhận hoàn tất."
                    : reason + (context.Phase == null ? "" : "\nBước engine: " + context.Phase);
            }
            else view.Reason = reason ?? "Sẵn sàng. Nếu engine vừa dừng, xem cảnh báo/nhật ký; không suy đoán hoàn tất.";
            return view;
        }
    }

    internal interface IDungeonDashboardPort
    {
        DungeonContext Capture(DungeonKind kind);
        void Apply(DungeonKind kind, int option, bool start, DungeonBinding binding);
    }

    internal sealed class DungeonDashboardAdapter
    {
        private readonly IDungeonDashboardPort port;
        internal DungeonDashboardAdapter(IDungeonDashboardPort port) { this.port = port; }
        internal DungeonView Read(DungeonKind kind)
        { return DungeonDashboardPolicy.Evaluate(kind, port.Capture(kind)); }
        internal string Execute(DungeonKind kind, int option, bool start, DungeonBinding expected)
        {
            if (!Enum.IsDefined(typeof(DungeonKind), kind) || (start && !DungeonDashboardPolicy.ValidOption(kind, option)))
                return "Lựa chọn không hợp lệ; chưa đổi nhiệm vụ.";
            var view = Read(kind);
            if (expected == null || view.Context == null || !expected.Matches(view.Context.Binding))
                return "Nhân vật/đội trưởng đã thay đổi; kiểm tra lại trước khi thao tác.";
            if (start ? !view.CanStart : !view.CanStop) return view.Reason;
            // A second snapshot protects re-login/team changes between display and click.
            var current = Read(kind);
            if (current.Context == null || !expected.Matches(current.Context.Binding))
                return "Nhân vật/đội trưởng đã thay đổi; chưa đổi nhiệm vụ.";
            if (start ? !current.CanStart : !current.CanStop) return current.Reason;
            port.Apply(kind, option, start, expected);
            return start ? "Đã gửi yêu cầu bật; cần theo dõi trạng thái engine." : "Đã gửi yêu cầu dừng nhiệm vụ được chọn.";
        }
    }
}
