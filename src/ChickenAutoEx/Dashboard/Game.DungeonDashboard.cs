using System;
using TinhKiemAuto.Dashboard;

namespace TinhKiemAuto
{
    public partial class Game
    {
        internal bool DashboardInitialized { get { return IsInit; } }
        // Read-only phase projection: does not advance a clock, parse notices or send commands.
        internal string DashboardPhase(DungeonKind kind)
        {
            if (kind != DungeonKind.TrungAc)
            {
                int task = kind == DungeonKind.KyCuoc ? 0 : kind == DungeonKind.AcTac ? 1 : kind == DungeonKind.AcBa ? 2 : 3;
                int[] state = dungeonPatrolDiagnostic;
                if (state[0] == task && state[2] != 0 && state[1] >= 0 && state[1] < 9)
                {
                    string[] names = { "Chờ yên quái", "Chờ xác nhận lên thú cưỡi", "Chờ xác nhận Theo Sau",
                        "Dẫn đội tuần tra", "Chờ xác nhận hủy theo", "Chờ xác nhận xuống thú cưỡi",
                        "Chờ xác nhận dừng di chuyển", "Đánh quái", "Nhặt đồ" };
                    return state[8] == (int)TinhKiemAuto.Diagnostics.DiagnosticReason.CombatDisabled
                        ? "Chưa bật Đánh quái cho một số nhân vật" : names[state[1]];
                }
            }
            switch (kind)
            {
                case DungeonKind.TrungAc:
                    lock (trungAcLock)
                    {
                        switch (trungAcPhase)
                        {
                            case TrungAcPhase.Scan: return "Đọc trạng thái quest";
                            case TrungAcPhase.Info: return "Chờ thông tin mục tiêu";
                            case TrungAcPhase.TravelTarget: return "Di chuyển đến mục tiêu";
                            case TrungAcPhase.Spawn: return "Chờ mục tiêu xuất hiện";
                            case TrungAcPhase.Combat: return "Đánh mục tiêu";
                            default: return "Nhận / trả quest: " + trungAcPhase;
                        }
                    }
                case DungeonKind.KyCuoc:
                    lock (kyCuocLock)
                        return kyCuocPhase == KyCuocPhase.Entry ? "Chờ NPC / cửa vào"
                            : kyCuocPhase == KyCuocPhase.Patrol ? "Tuần tra / đánh quái"
                            : kyCuocPhase == KyCuocPhase.WaitingExit ? "Chờ điều kiện cửa ra" : "Chờ xác nhận map ra";
                case DungeonKind.AcTac:
                    lock (acTacLock)
                        return acTacPhase == AcTacPhase.Entry ? "Đến map đã chọn / chờ cửa vào"
                            : acTacPhase == AcTacPhase.Patrol ? "Tuần tra / đánh quái"
                            : acTacPhase == AcTacPhase.AwaitBoss ? "Chờ xác nhận boss" : "Chờ xác nhận map ra";
                case DungeonKind.AcBa: return AcBaStatus;
                default: return "Thủy Lao (thử nghiệm): " + (thuyLaoStep == "" ? "chờ nhiệm vụ / cửa vào" : thuyLaoStep);
            }
        }
    }

    internal sealed class GameDungeonDashboardPort : IDungeonDashboardPort
    {
        private readonly Func<Game> selected;
        private readonly Func<Game, bool> registered;
        internal GameDungeonDashboardPort(Func<Game> selected, Func<Game, bool> registered)
        { this.selected = selected; this.registered = registered; }
        private static bool Active(Game game, DungeonKind kind)
        {
            switch (kind)
            {
                case DungeonKind.TrungAc: return game.IsTrungAc;
                case DungeonKind.KyCuoc: return game.IsKyCuoc;
                case DungeonKind.AcTac: return game.MapAcTac != 0;
                case DungeonKind.AcBa: return game.IsAcBa;
                default: return game.IsThuyLao;
            }
        }
        private static bool Conflict(Game game, DungeonKind kind)
        {
            return (kind != DungeonKind.TrungAc && game.IsTrungAc)
                || (kind != DungeonKind.KyCuoc && game.IsKyCuoc)
                || (kind != DungeonKind.AcTac && game.MapAcTac != 0)
                || (kind != DungeonKind.AcBa && game.IsAcBa) || (kind != DungeonKind.ThuyLao && game.IsThuyLao)
                || game.IsLauLanTamBao || game.MapTKC != 0 || game.IsQ123LauLan || game.IsQ123ToChau
                || game.IsYenTuO || game.IsPhungHoangLangMo || game.IsPMP || game.IsTuBaoBon
                || game.IsLuyenKim || game.IsHuyetChien || game.IsBachHoaDuyen || game.IsSuMon
                || game.IsXayDung || game.IsTuDuong || game.IsNhiemVuCoBan || game.IsTrieuTap;
        }
        public DungeonContext Capture(DungeonKind kind)
        {
            var context = new DungeonContext();
            Game actor = selected();
            if (actor == null || !registered(actor) || actor.TLBB == null) return context;
            context.SelectedPresent = true;
            context.SelectedName = actor.TLBB.Name;
            bool personal = DungeonDashboardPolicy.IsPersonal(kind);
            Game controller = personal ? actor : actor.Leader;
            if (controller == null || !registered(controller) || controller.TLBB == null) return context;
            context.ControllerPresent = true;
            context.ControllerName = controller.TLBB.Name;
            context.Binding = new DungeonBinding {
                Selected = actor, Controller = controller, SelectedId = actor.TLBB.Id,
                ControllerId = controller.TLBB.Id, TeamId = personal ? null : actor.TLBB.KeyId
            };
            context.SameTeam = personal || (controller.TLBB.IsLeader
                && !string.IsNullOrEmpty(controller.TLBB.Id) && controller.TLBB.KeyId == controller.TLBB.Id
                && actor.TLBB.KeyId == controller.TLBB.Id);
            context.Online = actor.TLBB.Online && controller.TLBB.Online
                && !string.IsNullOrEmpty(actor.TLBB.Id) && !string.IsNullOrEmpty(controller.TLBB.Id);
            context.Initialized = actor.DashboardInitialized && controller.DashboardInitialized;
            context.Auto = actor.IsAuto && controller.IsAuto;
            context.Paused = Global.Paused;
            context.ChangingMap = actor.ON_SCENE_TRANSING || controller.ON_SCENE_TRANSING
                || actor.IsChangeMap || controller.IsChangeMap;
            context.Dead = actor.TLBB.PlayerState == 9 || controller.TLBB.PlayerState == 9
                || actor.TLBB.MapId == MAP.GiamNguc || controller.TLBB.MapId == MAP.GiamNguc;
            context.Busy = personal ? actor.TLBB.PlayerState != 0 || controller.TLBB.PlayerState != 0
                : !DungeonMovementPolicy.CanObserve(actor.TLBB.PlayerState) || !DungeonMovementPolicy.CanObserve(controller.TLBB.PlayerState);
            context.Following = actor.TLBB.IsFollow;
            context.Conflict = Conflict(actor, kind) || Conflict(controller, kind);
            // Personal quests still must not interfere with a team task on the leader.
            Game teamLeader = personal ? actor.Leader : controller;
            if (personal && teamLeader != null && teamLeader != actor && registered(teamLeader)
                && teamLeader.TLBB != null && teamLeader.TLBB.IsLeader
                && actor.TLBB.KeyId == teamLeader.TLBB.Id)
                context.Conflict |= Conflict(teamLeader, kind);
            context.Active = Active(controller, kind);
            context.Phase = context.Active ? controller.DashboardPhase(kind) : null;
            // Revalidate the selection at the end of this read-only capture.
            if (!ReferenceEquals(actor, selected())) return new DungeonContext();
            return context;
        }
        public void Apply(DungeonKind kind, int option, bool start, DungeonBinding binding)
        {
            var current = Capture(kind);
            var view = DungeonDashboardPolicy.Evaluate(kind, current);
            if (!Enum.IsDefined(typeof(DungeonKind), kind) || (start && !DungeonDashboardPolicy.ValidOption(kind, option)) || binding == null
                || !binding.Matches(current.Binding) || (start ? !view.CanStart : !view.CanStop))
                throw new InvalidOperationException("Ngữ cảnh đã thay đổi; chưa gửi yêu cầu.");
            var controller = (Game)binding.Controller;
            // Only the chosen flag changes. Other tasks/actors/configuration are never reset here.
            switch (kind)
            {
                case DungeonKind.TrungAc: controller.IsTrungAc = start; break;
                case DungeonKind.KyCuoc: controller.IsKyCuoc = start; break;
                case DungeonKind.AcTac:
                    controller.MapAcTac = start ? (option == 0 ? Game.ChooseAcTacMap() : option) : 0; break;
                case DungeonKind.AcBa:
                    if (start && option >= 0) controller.SelectManualAcBaSchool(option);
                    else controller.IsAcBa = start;
                    break;
                case DungeonKind.ThuyLao: controller.IsThuyLao = start; break;
                default: throw new InvalidOperationException("Nhiệm vụ không hợp lệ.");
            }
        }
    }
}
