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

        private bool CanActThuyLao()
        {
            return !thuyLaoBlocked && IsAuto && TLBB != null && TLBB.Online
                && TLBB.PlayerState != 2 && TLBB.PlayerState != 7 && !ON_SCENE_TRANSING && !IsChangeMap;
        }

        private void ResetThuyLaoProgress()
        {
            TrangThaiThuyLao = "";
            DaNhanThuyLao = IsXongThuyLao = ThuyLaoRouteFinished = false;
            thuyLaoSelected = thuyLaoTalkSent = thuyLaoAwaitEntry = thuyLaoBlocked = false;
            thuyLaoContextMap = -1;
            thuyLaoContextTeam = null;
            thuyLaoRouteIndex = 0;
            thuyLaoStep = "";
            thuyLaoStepStarted = thuyLaoClearAt = thuyLaoClock.ElapsedMilliseconds;
        }

        private void ObserveThuyLaoContext()
        {
            if (thuyLaoContextMap != -1 && (thuyLaoContextMap != TLBB.MapId || thuyLaoContextTeam != TLBB.KeyId))
                ResetThuyLaoProgress();
            if (thuyLaoContextMap == -1)
                thuyLaoClearAt = thuyLaoClock.ElapsedMilliseconds;
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
                thuyLaoStepStarted = thuyLaoClock.ElapsedMilliseconds;
            }
            if (thuyLaoClock.ElapsedMilliseconds - thuyLaoStepStarted < ThuyLaoTimeoutMilliseconds)
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
            if (!GoTo(npc) || !HasThuyLaoNpc(npc)) return false;
            if (!TLBB.IsQuestOpen)
            {
                if (!thuyLaoTalkSent) { Talk(npc); thuyLaoTalkSent = true; }
                return false;
            }
            if (!HasThuyLaoOption(option)) return false;
            if (!thuyLaoSelected)
            {
                QuestFrameOptionClicked(option, -1);
                thuyLaoSelected = true;
                return false;
            }
            QuestFrameAccept();
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
                        if (member.TLBB.IsRide) member.DownRide();
                    }
                    return;
                }
                if (!member.IsRide && member.TLBB.HaveRide) member.UpRide();
                else if (TINHKIEM.GetDistance(member.RoundX, member.RoundY, RoundX, RoundY) > 4f)
                    member.GoTo(RoundX, RoundY);
                return;
            }
            if (!member.DaNhanThuyLao) member.NhanThuyLao();
            else member.DiThuyLao();
        }

        public void DatDoiThuyLao()
        {
            if (!IsThuyLao) return;
            try
            {
                if (!CanActThuyLao() || !TLBB.IsLeader) { StopThuyLao(null); return; }
                ObserveThuyLaoContext();
                if (thuyLaoStep != "" && !WaitThuyLaoStep(thuyLaoStep)) return;
                if (TickCount % 18 != 0 || PickItem()) return;
                TrieuTap();
                if (!IsThuyLao) return;
                if (TLBB.MapId != MAP.ThuyLao)
                {
                    if (!DaNhanThuyLao) NhanThuyLao(); else DiThuyLao();
                    return;
                }
                if (Objects.NearMonter15m.Count > 0)
                {
                    thuyLaoClearAt = thuyLaoClock.ElapsedMilliseconds;
                    if (TLBB.PlayerState == 0)
                    {
                        if (TLBB.IsFollow) StopFollow();
                        if (TLBB.IsRide) DownRide();
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
                if (!WaitThuyLaoStep("Patrol") || thuyLaoClock.ElapsedMilliseconds - thuyLaoClearAt <= 2000) return;
                if (!IsRide && TLBB.HaveRide) { UpRide(); return; }
                int savedIndex = MoveIndex;
                try
                {
                    MoveIndex = thuyLaoRouteIndex;
                    MoveNext();
                    if (MoveIndex != thuyLaoRouteIndex) thuyLaoStepStarted = thuyLaoClock.ElapsedMilliseconds;
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
