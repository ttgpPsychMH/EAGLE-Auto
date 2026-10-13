using System;
using System.Diagnostics;

namespace TinhKiemAuto
{
    public partial class Game
    {
        private readonly Stopwatch thuyLaoClock = Stopwatch.StartNew();
        private int thuyLaoContextMap = -1, thuyLaoRouteIndex;
        private string thuyLaoContextTeam, thuyLaoStep = "";
        private long thuyLaoStepStarted, thuyLaoClearAt;
        private bool thuyLaoSelected, thuyLaoTalkSent, thuyLaoAwaitEntry, thuyLaoBlocked;
        private bool ThuyLaoRouteFinished;
        private bool thuyLaoEnabled;
        private const long ThuyLaoTimeoutMilliseconds = 60000;

        private Game thuyLaoController;
        private string thuyLaoOwner;
        private long thuyLaoPausedAt = -1, thuyLaoPauseOffset;
        private long ThuyLaoNow { get { return thuyLaoClock.ElapsedMilliseconds - thuyLaoPauseOffset; } }

        private void ObserveThuyLaoPause()
        {
            bool pause = Global.Paused || ON_SCENE_TRANSING || IsChangeMap || (TLBB != null && TLBB.PlayerState == 7);
            long raw = thuyLaoClock.ElapsedMilliseconds;
            if (pause && thuyLaoPausedAt < 0) thuyLaoPausedAt = raw;
            if (!pause && thuyLaoPausedAt >= 0)
            { thuyLaoPauseOffset += raw - thuyLaoPausedAt; thuyLaoPausedAt = -1; }
        }

        private bool CanActThuyLao()
        {
            ObserveThuyLaoPause();
            if (thuyLaoBlocked || !IsAuto || !IsInit || TLBB == null || !TLBB.Online
                || TLBB.PlayerState != 0 || Global.Paused || ON_SCENE_TRANSING || IsChangeMap
                || IsTrungAc || IsKyCuoc || (thuyLaoOwner != null && thuyLaoOwner != TLBB.Id)) return false;
            if (thuyLaoController != null)
                return thuyLaoController != this && thuyLaoController.CanActThuyLao()
                    && thuyLaoController.TLBB.IsLeader && !TLBB.IsLeader && TLBB.KeyId == thuyLaoController.TLBB.Id;
            return IsThuyLao;
        }

        private void ResetThuyLaoProgress()
        {
            TrangThaiThuyLao = "";
            DaNhanThuyLao = IsXongThuyLao = ThuyLaoRouteFinished = false;
            thuyLaoSelected = thuyLaoTalkSent = thuyLaoAwaitEntry = thuyLaoBlocked = false;
            thuyLaoContextMap = -1;
            thuyLaoOwner = null;
            thuyLaoPausedAt = -1;
            thuyLaoPauseOffset = 0;
            thuyLaoContextTeam = null;
            thuyLaoRouteIndex = 0;
            thuyLaoStep = "";
            thuyLaoStepStarted = thuyLaoClearAt = ThuyLaoNow;
        }

        private void ObserveThuyLaoContext()
        {
            if (thuyLaoContextMap != -1 && (thuyLaoContextMap != TLBB.MapId || thuyLaoContextTeam != TLBB.KeyId))
                ResetThuyLaoProgress();
            if (thuyLaoContextMap == -1)
                thuyLaoClearAt = ThuyLaoNow;
            thuyLaoOwner = TLBB.Id;
            thuyLaoContextMap = TLBB.MapId;
            thuyLaoContextTeam = TLBB.KeyId;
        }

        private void StopThuyLao(string reason)
        {
            IsThuyLao = false;
            ResetThuyLaoProgress();
            thuyLaoBlocked = true;
            if (reason != null)
                CanhBao.Msg("Thủy Lao", "Đã dừng Thủy Lao: " + reason, CanhBao.Kieu.Eror);
        }

        private bool WaitThuyLaoStep(string step)
        {
            if (thuyLaoStep != step)
            {
                thuyLaoStep = step;
                thuyLaoStepStarted = ThuyLaoNow;
            }
            if (ThuyLaoNow - thuyLaoStepStarted < ThuyLaoTimeoutMilliseconds)
                return true;
            StopThuyLao("hết thời gian chờ bước " + step + ".");
            return false;
        }

        private Task ReadThuyLaoTask()
        {
            foreach (Task task in Task.Enum(this))
                if (task != null && task.ClearName != null && task.ClearName.Contains("binhdinhthuylao"))
                    return task;
            return null;
        }

        private bool HasThuyLaoNpc(NPC npc)
        {
            foreach (GameObject obj in Objects.All)
                if (obj != null && obj.IsNPC && obj.Id == npc.Id)
                    return true;
            return false;
        }

        private bool HasThuyLaoOption(int option)
        {
            foreach (QuestFrame frame in QuestFrame.Enum(this))
                if (frame != null && frame.StrOptionExtra1 == option && frame.StrOptionExtra2 == -1)
                    return true;
            return false;
        }

        private bool SendThuyLaoDialog(NPC npc, int option)
        {
            if (!CanActThuyLao() || !GoTo(npc) || !CanActThuyLao() || !HasThuyLaoNpc(npc)) return false;
            if (!TLBB.IsQuestOpen)
            {
                if (!thuyLaoTalkSent && CanActThuyLao()) { Talk(npc); thuyLaoTalkSent = true; }
                return false;
            }
            if (!HasThuyLaoOption(option) || !CanActThuyLao()) return false;
            if (!thuyLaoSelected)
            {
                QuestFrameOptionClicked(option, -1);
                thuyLaoSelected = true;
                return false;
            }
            if (!CanActThuyLao()) return false;
            QuestFrameAccept();
            if (!CanActThuyLao()) return false;
            CloseQuest();
            thuyLaoSelected = thuyLaoTalkSent = false;
            return true;
        }

        private void DiThuyLao()
        {
            if (!CanActThuyLao()) return;
            ObserveThuyLaoContext();
            if (TLBB.MapId == MAP.ThuyLao) return;
            if (!WaitThuyLaoStep("Entry")) return;
            if (thuyLaoAwaitEntry) return;
            if (TLBB.MapId != MAP.ThaiHo && TLBB.MapId != MAP.ToChau)
                TimDuong(THAIHO.HoDienKhanh.X, THAIHO.HoDienKhanh.Y, THAIHO.HoDienKhanh.Map);
            else if (SendThuyLaoDialog(THAIHO.HoDienKhanh, 232002))
                thuyLaoAwaitEntry = true;
        }

        private void NhanThuyLao()
        {
            if (!CanActThuyLao()) return;
            ObserveThuyLaoContext();
            if (TLBB.MapId == MAP.ThuyLao || DaNhanThuyLao) return;
            if (TrangThaiThuyLao == "")
            {
                if (!WaitThuyLaoStep("OpenMission")) return;
                if (!TLBB.IsTogleMission) PostMessage(18, 105);
                TrangThaiThuyLao = "OpenMission";
                return;
            }
            if (!WaitThuyLaoStep(TrangThaiThuyLao)) return;
            if (TrangThaiThuyLao == "OpenMission")
            {
                if (!TLBB.IsTogleMission) return;
                TrangThaiThuyLao = "ReadMission";
                return;
            }
            if (TrangThaiThuyLao == "ReadMission" || TrangThaiThuyLao == "VerifyMission")
            {
                Task task = ReadThuyLaoTask();
                if (task != null)
                {
                    if (task.Completed || task.Complete >= 256)
                    {
                        StopThuyLao("nhiệm vụ đã hoàn thành hoặc đang chờ trả; chưa vào lượt mới.");
                        return;
                    }
                    DaNhanThuyLao = true;
                    TrangThaiThuyLao = "";
                    thuyLaoStep = "";
                    return;
                }
                if (TrangThaiThuyLao == "ReadMission") TrangThaiThuyLao = "NhanThuyLao";
                return;
            }
            if (TrangThaiThuyLao != "NhanThuyLao") return;
            if (TLBB.MapId != TOCHAU.HoDienBao.Map)
                TimDuong(TOCHAU.HoDienBao.X, TOCHAU.HoDienBao.Y, TOCHAU.HoDienBao.Map);
            else if (SendThuyLaoDialog(TOCHAU.HoDienBao, 232000))
                TrangThaiThuyLao = "VerifyMission";
        }

        private void RunThuyLaoMember(Game member)
        {
            if (!CanActThuyLao() || !TLBB.IsLeader || member == null || member == this) return;
            Game previousController = member.thuyLaoController;
            member.thuyLaoController = this;
            try
            {
                if (!member.CanActThuyLao() || member.TLBB.IsLeader || member.TLBB.KeyId != TLBB.Id) return;
                if (member.tranTime.Elapsed.TotalSeconds < 2.0) return;
                member.ObserveThuyLaoContext();
                if (member.TLBB.MapId == MAP.ThuyLao)
                {
                    if (TLBB.MapId != MAP.ThuyLao || member.PickItem()) return;
                    if (member.Objects.NearMonter20m.Count > 0)
                    {
                        if (member.TLBB.PlayerState == 0)
                        {
                            if (member.TLBB.IsFollow) member.StopFollow();
                            if (member.CanActThuyLao() && member.TLBB.IsRide) member.DownRide();
                        }
                        return;
                    }
                    if (!member.CanActThuyLao()) return;
                    if (!member.IsRide && member.TLBB.HaveRide) member.UpRide();
                    else if (TINHKIEM.GetDistance(member.RoundX, member.RoundY, RoundX, RoundY) > 4f)
                        member.GoTo(RoundX, RoundY);
                    return;
                }
                if (!member.DaNhanThuyLao) member.NhanThuyLao();
                else member.DiThuyLao();
            }
            finally { member.thuyLaoController = previousController; }
        }

        public void DatDoiThuyLao()
        {
            if (!IsThuyLao) return;
            try
            {
                if (!IsAuto || !IsInit || TLBB == null || !TLBB.Online || !TLBB.IsLeader
                    || TLBB.PlayerState == 2 || TLBB.PlayerState == 9
                    || (thuyLaoOwner != null && thuyLaoOwner != TLBB.Id)
                    || IsTrungAc || IsKyCuoc) { StopThuyLao(null); return; }
                if (!CanActThuyLao()) return;
                ObserveThuyLaoContext();
                if (thuyLaoStep != "" && !WaitThuyLaoStep(thuyLaoStep)) return;
                if (TickCount % 18 != 0 || PickItem()) return;
                foreach (Game member in Party.ToArray())
                {
                    if (!CanActThuyLao()) return;
                    RunThuyLaoMember(member);
                }
                if (!CanActThuyLao()) return;
                if (TLBB.MapId != MAP.ThuyLao)
                {
                    if (!DaNhanThuyLao) NhanThuyLao(); else DiThuyLao();
                    return;
                }
                if (Objects.NearMonter15m.Count > 0)
                {
                    thuyLaoClearAt = ThuyLaoNow;
                    if (TLBB.PlayerState == 0)
                    {
                        if (TLBB.IsFollow) StopFollow();
                        if (CanActThuyLao() && TLBB.IsRide) DownRide();
                    }
                    return;
                }
                if (ThuyLaoRouteFinished)
                {
                    if (!WaitThuyLaoStep("AwaitCompletion")) return;
                    Task task = ReadThuyLaoTask();
                    if (task == null || (!task.Completed && task.Complete < 256)) return;
                    IsXongThuyLao = true;
                    GoTo(94f, 94f);
                    return;
                }
                if (!WaitThuyLaoStep("Patrol") || ThuyLaoNow - thuyLaoClearAt <= 2000) return;
                if (!IsRide && TLBB.HaveRide) { UpRide(); return; }
                int savedIndex = MoveIndex;
                try
                {
                    MoveIndex = thuyLaoRouteIndex;
                    MoveNext();
                    if (MoveIndex != thuyLaoRouteIndex) thuyLaoStepStarted = ThuyLaoNow;
                    thuyLaoRouteIndex = MoveIndex;
                }
                finally { MoveIndex = savedIndex; }
            }
            catch (Exception error)
            {
                StopThuyLao("lỗi đọc/xử lý " + error.GetType().Name + ".");
            }
        }
    }
}
