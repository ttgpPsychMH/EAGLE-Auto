using System;
using System.Diagnostics;
using System.Collections.Generic;

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
        private static readonly int[,] ThuyLaoPatrolRoute = { {71,41},{107,40},{140,39},{150,71},{148,106},{150,139},
            {122,150},{85,147},{49,149},{43,120},{41,84},{41,50} };
        private readonly bool[] thuyLaoVisited = new bool[12];
        private bool thuyLaoReanchor = true;
        private float thuyLaoBestDistance = float.MaxValue;
        private const long ThuyLaoTimeoutMilliseconds = 60000;

        private Game thuyLaoController;
        private string thuyLaoOwner;
        private long thuyLaoPausedAt = -1, thuyLaoPauseOffset;
        private long ThuyLaoNow { get { return (thuyLaoPausedAt >= 0 ? thuyLaoPausedAt : thuyLaoClock.ElapsedMilliseconds) - thuyLaoPauseOffset; } }

        private void ObserveThuyLaoPause()
        {
            bool pause = Global.Paused || ON_SCENE_TRANSING || IsChangeMap;
            long raw = thuyLaoClock.ElapsedMilliseconds;
            if (pause && thuyLaoPausedAt < 0) thuyLaoPausedAt = raw;
            if (!pause && thuyLaoPausedAt >= 0)
            { thuyLaoPauseOffset += raw - thuyLaoPausedAt; thuyLaoPausedAt = -1; }
        }

        private bool CanActThuyLao()
        {
            ObserveThuyLaoPause();
            if (thuyLaoBlocked || !IsAuto || !IsInit || TLBB == null || !TLBB.Online
                || !DungeonMovementPolicy.CanObserve(TLBB.PlayerState) || Global.Paused || ON_SCENE_TRANSING || IsChangeMap
                || IsTrungAc || IsKyCuoc || IsAcBa || MapAcTac != 0 || (thuyLaoOwner != null && thuyLaoOwner != TLBB.Id)) return false;
            if (thuyLaoController != null)
                return thuyLaoController != this && thuyLaoController.CanActThuyLao()
                    && thuyLaoController.TLBB.IsLeader && !TLBB.IsLeader && TLBB.KeyId == thuyLaoController.TLBB.Id;
            return IsThuyLao;
        }

        private void ResetThuyLaoProgress()
        {
            ResetDungeonPatrol(DungeonTask.ThuyLao);
            TrangThaiThuyLao = "";
            DaNhanThuyLao = IsXongThuyLao = ThuyLaoRouteFinished = false;
            thuyLaoSelected = thuyLaoTalkSent = thuyLaoAwaitEntry = thuyLaoBlocked = false;
            thuyLaoContextMap = -1;
            thuyLaoOwner = null;
            thuyLaoPausedAt = -1;
            thuyLaoPauseOffset = 0;
            thuyLaoContextTeam = null;
            thuyLaoRouteIndex = 0;
            Array.Clear(thuyLaoVisited, 0, thuyLaoVisited.Length);
            thuyLaoReanchor = true; thuyLaoBestDistance = float.MaxValue;
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
            int[] stopped = CaptureDungeonStop(DungeonTask.ThuyLao, reason);
            IsThuyLao = false;
            ResetThuyLaoProgress();
            dungeonPatrolDiagnostic = stopped;
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
            if (!CanActThuyLao() || DungeonMovementPolicy.IsAttacking(TLBB.PlayerState)) return false;
            if (!CanActThuyLao() || !GoTo(npc) || !CanActThuyLao() || !HasThuyLaoNpc(npc)) return false;
            if (TLBB.IsFollow || IsRide || DungeonMovementPolicy.IsMoving(TLBB.PlayerState))
            {
                Game controller = thuyLaoController ?? this;
                if (!controller.dungeonTaskBound)
                    controller.BindDungeonParty(DungeonTask.ThuyLao, new List<Game> { controller, this }, controller.CanControlThuyLaoActor);
                if (!controller.PrepareDungeonNpc(this, controller.CanControlThuyLaoActor, controller.ThuyLaoNow)) return false;
            }
            if (!DungeonMovementPolicy.CanTalk(TLBB.PlayerState)) return false;
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
            if (!DungeonMovementPolicy.CanTalk(TLBB.PlayerState)) return;
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
                    // In-dungeon movement is coordinated once for the whole team below.
                    return;
                }
                if (!member.DaNhanThuyLao) member.NhanThuyLao();
                else member.DiThuyLao();
            }
            finally { member.thuyLaoController = previousController; }
        }

        private bool CanControlThuyLaoActor(Game actor)
        {
            if (!CanActThuyLao() || !TLBB.IsLeader || actor == null) return false;
            if (actor == this) return true;
            return actor.IsAuto && actor.IsInit && actor.TLBB != null && actor.TLBB.Online
                && !string.IsNullOrEmpty(actor.TLBB.Id) && !actor.TLBB.IsLeader && actor.TLBB.KeyId == TLBB.Id
                && DungeonMovementPolicy.CanObserve(actor.TLBB.PlayerState) && !actor.ON_SCENE_TRANSING && !actor.IsChangeMap
                && !actor.IsTrungAc && !actor.IsKyCuoc && !actor.IsAcBa && actor.MapAcTac == 0
                && actor.tranTime.Elapsed.TotalSeconds >= 2.0;
        }

        private void PatrolThuyLao()
        {
            if (thuyLaoReanchor)
            {
                float best = float.MaxValue; int nearest = -1;
                for (int i = 0; i < thuyLaoVisited.Length; i++)
                {
                    if (thuyLaoVisited[i]) continue;
                    float value = TINHKIEM.GetDistance(CharX, CharY, ThuyLaoPatrolRoute[i,0], ThuyLaoPatrolRoute[i,1]);
                    if (value < best) { best = value; nearest = i; }
                }
                if (nearest < 0) { StopThuyLao("tọa độ tuần tra không hợp lệ."); return; }
                thuyLaoRouteIndex = nearest; thuyLaoReanchor = false; thuyLaoBestDistance = float.MaxValue;
                thuyLaoStepStarted = ThuyLaoNow;
            }
            float distance = TINHKIEM.GetDistance(CharX, CharY, ThuyLaoPatrolRoute[thuyLaoRouteIndex,0], ThuyLaoPatrolRoute[thuyLaoRouteIndex,1]);
            if (float.IsNaN(distance) || float.IsInfinity(distance)) { StopThuyLao("tọa độ tuần tra không hợp lệ."); return; }
            if (distance <= 5f)
            {
                thuyLaoVisited[thuyLaoRouteIndex] = true;
                int next = -1;
                for (int offset = 1; offset <= thuyLaoVisited.Length; offset++)
                { int candidate = (thuyLaoRouteIndex + offset) % thuyLaoVisited.Length;
                    if (!thuyLaoVisited[candidate]) { next = candidate; break; } }
                if (next < 0) { ThuyLaoRouteFinished = true; return; }
                thuyLaoRouteIndex = next; thuyLaoBestDistance = float.MaxValue; thuyLaoStepStarted = ThuyLaoNow;
                distance = TINHKIEM.GetDistance(CharX, CharY, ThuyLaoPatrolRoute[next,0], ThuyLaoPatrolRoute[next,1]);
            }
            if (distance < thuyLaoBestDistance - 1f) { thuyLaoBestDistance = distance; thuyLaoStepStarted = ThuyLaoNow; }
            if (CanActThuyLao()) Move(ThuyLaoPatrolRoute[thuyLaoRouteIndex,0], ThuyLaoPatrolRoute[thuyLaoRouteIndex,1]);
        }

        public void DatDoiThuyLao()
        {
            if (!IsThuyLao) return;
            try
            {
                if (Global.Paused || ON_SCENE_TRANSING || IsChangeMap) { ObserveThuyLaoPause(); return; }
                if (!IsAuto || !IsInit || TLBB == null || !TLBB.Online || !TLBB.IsLeader
                    || TLBB.PlayerState == 9
                    || (thuyLaoOwner != null && thuyLaoOwner != TLBB.Id)
                    || IsTrungAc || IsKyCuoc) { StopThuyLao(null); return; }
                if (!CanActThuyLao()) return;
                ObserveThuyLaoContext();
                if (TickCount % 18 != 0) return;
                var participants = new List<Game> { this };
                bool allInside = TLBB.MapId == MAP.ThuyLao, combat = false, loot = false;
                foreach (Game member in DungeonWindowSnapshot())
                {
                    if (!CanActThuyLao()) return;
                    if (member == null || member == this || participants.Contains(member)
                        || !member.IsAuto || member.TLBB == null || member.TLBB.KeyId != TLBB.Id || member.TLBB.IsLeader) continue;
                    participants.Add(member);
                    allInside &= CanControlThuyLaoActor(member) && member.TLBB.MapId == MAP.ThuyLao;
                    RunThuyLaoMember(member);
                }
                if (!CanActThuyLao()) return;
                BindDungeonParty(DungeonTask.ThuyLao, participants, CanControlThuyLaoActor);
                if (TLBB.MapId != MAP.ThuyLao)
                {
                    if (!DaNhanThuyLao) NhanThuyLao(); else DiThuyLao();
                    return;
                }
                foreach (Game actor in participants)
                {
                    if (!CanControlThuyLaoActor(actor) || actor.TLBB.MapId != MAP.ThuyLao) continue;
                    combat |= actor.Objects.NearMonter20m.Count > 0 || actor.Objects.NearMonter15m.Count > 0
                        || DungeonMovementPolicy.IsAttacking(actor.TLBB.PlayerState);
                    if (actor.PickItem()) loot = true;
                }
                if (combat || loot)
                {
                    thuyLaoClearAt = thuyLaoStepStarted = ThuyLaoNow; thuyLaoReanchor = true;
                    PrepareDungeonPatrol(DungeonTask.ThuyLao, participants, CanControlThuyLaoActor,
                        ThuyLaoNow, thuyLaoClearAt, 2000, combat, loot, StopThuyLao);
                    return;
                }
                if (!allInside) { RegroupDungeonPatrol(participants, CanControlThuyLaoActor, ThuyLaoNow); return; }
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
                if (!PrepareDungeonPatrol(DungeonTask.ThuyLao, participants, CanControlThuyLaoActor,
                    ThuyLaoNow, thuyLaoClearAt, 2000, false, false, StopThuyLao)) return;
                PatrolThuyLao();
                RegroupDungeonPatrol(participants, CanControlThuyLaoActor, ThuyLaoNow);
            }
            catch (Exception error)
            {
                StopThuyLao("lỗi đọc/xử lý " + error.GetType().Name + ".");
            }
        }
    }
}
