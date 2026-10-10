using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TinhKiemAuto
{
    public partial class Game
    {
        private readonly object kyCuocLock = new object();
        private readonly Stopwatch kyCuocClock = Stopwatch.StartNew();
        private volatile bool kyCuocEnabled;
        private KyCuocState kyCuocLeader;
        private readonly Dictionary<Game, KyCuocState> kyCuocMembers = new Dictionary<Game, KyCuocState>();
        private string kyCuocTeam;
        private KyCuocPhase kyCuocPhase;
        private long kyCuocBossDeadAt = -1;
        private const long KyCuocDialogTimeout = 60000, KyCuocTravelTimeout = 180000;
        private const long KyCuocClearWait = 4000, KyCuocExitWait = 30000;
        private static readonly int[,] KyCuocRoute = { {42,42}, {84,42}, {81,81}, {42,85}, {50,49}, {71,50} };
        private enum KyCuocPhase { Entry, Patrol, WaitingExit, Exit }
        private sealed class KyCuocState
        {
            internal int Map = -1, RouteIndex;
            internal bool Selected, TalkSent, Exited, Reanchor = true;
            internal readonly bool[] Visited = new bool[6];
            internal readonly HashSet<int> LiveBossIds = new HashSet<int>();
            internal long Started, ClearAt, ProgressAt, UnavailableAt = -1;
            internal float BestDistance = float.MaxValue;
        }

        private void ResetKyCuocSession()
        {
            kyCuocLeader = null;
            kyCuocMembers.Clear();
            kyCuocTeam = null;
            kyCuocBossDeadAt = -1;
            kyCuocPhase = KyCuocPhase.Entry;
            IsXongKyCuoc = false;
        }

        private void InitializeKyCuocSession()
        {
            if (kyCuocLeader != null) return;
            long now = kyCuocClock.ElapsedMilliseconds;
            kyCuocLeader = new KyCuocState { Started = now, ClearAt = now, ProgressAt = now };
            kyCuocTeam = TLBB == null ? null : TLBB.KeyId;
        }

        private void StopKyCuoc(string reason)
        {
            IsKyCuoc = false;
            if (reason != null)
                CanhBao.Msg("Trân Long Kỳ Cuộc", "Đã dừng Kỳ Cuộc: " + reason, CanhBao.Kieu.Eror);
        }

        private bool CanControlKyCuoc(Game participant)
        {
            if (!IsKyCuoc || !IsAuto || !IsInit || TLBB == null || !TLBB.Online || !TLBB.IsLeader
                || TLBB.Id != kyCuocTeam || TLBB.KeyId != kyCuocTeam || TLBB.PlayerState == 2 || TLBB.PlayerState == 7
                || ON_SCENE_TRANSING || IsChangeMap) return false;
            if (participant == null || !participant.IsAuto || !participant.IsInit || participant.TLBB == null || !participant.TLBB.Online
                || participant.TLBB.PlayerState == 2 || participant.TLBB.PlayerState == 7
                || participant.ON_SCENE_TRANSING || participant.IsChangeMap) return false;
            if (participant != this && participant.tranTime.Elapsed.TotalSeconds < 2.0) return false;
            return participant == this || (!participant.TLBB.IsLeader && participant.TLBB.KeyId == TLBB.Id);
        }

        // Called outside the leader-only dispatcher as well, so losing leadership cancels this session.
        private void CheckKyCuocSession()
        {
            if (!IsKyCuoc) return;
            lock (kyCuocLock)
            {
                try
                {
                    InitializeKyCuocSession();
                    if (!IsAuto || TLBB == null || !TLBB.Online || !TLBB.IsLeader
                        || TLBB.Id != kyCuocTeam || TLBB.KeyId != kyCuocTeam || TLBB.PlayerState == 2 || !IsInit)
                    { StopKyCuoc(null); return; }
                    if (IsThuyLao || IsAcBa || IsLauLanTamBao || IsTrungAc || MapAcTac != 0 || MapTKC != 0
                        || IsQ123LauLan || IsQ123ToChau || IsYenTuO || IsPhungHoangLangMo || IsPMP || IsTuBaoBon)
                    { StopKyCuoc("đang có nhiệm vụ khác; hãy dừng nhiệm vụ đó trước."); return; }
                    if (!CanControlKyCuoc(this) || TLBB.MapId != MAP.TranLongKyCuoc || kyCuocBossDeadAt >= 0) return;
                    ObserveKyCuocBoss(this, kyCuocLeader);
                }
                catch (Exception error) { StopKyCuoc("lỗi đọc/xử lý " + error.GetType().Name + "."); }
            }
        }

        private void ObserveKyCuocBoss(Game participant, KyCuocState state)
        {
            if (kyCuocBossDeadAt >= 0 || !CanControlKyCuoc(participant)
                || participant.TLBB.MapId != MAP.TranLongKyCuoc) return;
            foreach (GameObject obj in participant.Objects.All)
            {
                if (obj == null || obj.IsNPC || obj.CleanName != "viencokyhon") continue;
                if (obj.HP > 0f) state.LiveBossIds.Add(obj.Id);
                else if (obj.HP == 0f && state.LiveBossIds.Contains(obj.Id))
                { kyCuocBossDeadAt = kyCuocClock.ElapsedMilliseconds; break; }
            }
        }

        private bool KyCuocTimeout(KyCuocState state, long milliseconds, string step)
        {
            if (kyCuocClock.ElapsedMilliseconds - state.Started < milliseconds) return false;
            StopKyCuoc("hết thời gian chờ " + step + ".");
            return true;
        }

        private bool ObserveKyCuocMap(Game participant, KyCuocState state)
        {
            int map = participant.TLBB.MapId;
            if (state.Map == map) return true;
            if (kyCuocPhase == KyCuocPhase.Exit && state.Selected && map != MAP.TranLongKyCuoc)
            {
                if (map != LACDUONG.Id) { StopKyCuoc("map sau cửa ra không khớp Lạc Dương."); return false; }
                state.Exited = true;
            }
            else if ((kyCuocPhase == KyCuocPhase.Patrol || kyCuocPhase == KyCuocPhase.WaitingExit)
                && map != MAP.TranLongKyCuoc)
            { StopKyCuoc("rời phụ bản trước khi xác nhận cửa ra."); return false; }
            if (map == MAP.TranLongKyCuoc && kyCuocPhase == KyCuocPhase.Entry)
            {
                state.Selected = state.TalkSent = false;
                state.Started = state.ClearAt = state.ProgressAt = kyCuocClock.ElapsedMilliseconds;
                state.Reanchor = true;
            }
            else if (!state.Selected) state.TalkSent = false;
            state.Map = map;
            return true;
        }

        private bool HasKyCuocNpc(Game participant, NPC npc)
        {
            foreach (GameObject obj in participant.Objects.All)
                if (obj != null && obj.IsNPC && obj.Id == npc.Id) return true;
            return false;
        }

        private bool SendKyCuocDialog(Game participant, KyCuocState state, NPC npc, int option, int extra)
        {
            if (!CanControlKyCuoc(participant) || state.Selected) return false;
            if (!participant.GoTo(npc) || !HasKyCuocNpc(participant, npc)) return false;
            if (!participant.TLBB.IsQuestOpen)
            {
                if (!state.TalkSent && CanControlKyCuoc(participant))
                { participant.Talk(npc); state.TalkSent = true; }
                return false;
            }
            foreach (QuestFrame frame in QuestFrame.Enum(participant))
                if (frame != null && frame.StrOptionExtra1 == option && frame.StrOptionExtra2 == extra)
                {
                    if (!CanControlKyCuoc(participant)) return false;
                    participant.QuestFrameOptionClicked(option, extra);
                    state.Selected = true;
                    state.Started = kyCuocClock.ElapsedMilliseconds;
                    if (CanControlKyCuoc(participant)) participant.CloseQuest();
                    return true;
                }
            return false;
        }

        private int NearestUnvisitedKyCuocPoint(KyCuocState state)
        {
            int best = state.RouteIndex;
            float distance = float.MaxValue;
            for (int offset = 0; offset < state.Visited.Length; offset++)
            {
                int i = (state.RouteIndex + offset) % state.Visited.Length;
                if (state.Visited[i]) continue;
                float candidate = TINHKIEM.GetDistance(CharX, CharY, KyCuocRoute[i,0], KyCuocRoute[i,1]);
                if (candidate < distance - 0.01f) { distance = candidate; best = i; }
            }
            return best;
        }

        private void PatrolKyCuoc()
        {
            KyCuocState state = kyCuocLeader;
            long now = kyCuocClock.ElapsedMilliseconds;
            if (now - state.ClearAt < KyCuocClearWait) return;
            if (state.Reanchor)
            {
                state.RouteIndex = NearestUnvisitedKyCuocPoint(state);
                state.Reanchor = false;
                state.BestDistance = float.MaxValue;
                state.ProgressAt = now;
            }
            int index = state.RouteIndex;
            float distance = TINHKIEM.GetDistance(CharX, CharY, KyCuocRoute[index,0], KyCuocRoute[index,1]);
            if (distance <= 5f)
            {
                state.Visited[index] = true;
                int next = -1;
                for (int offset = 1; offset <= state.Visited.Length; offset++)
                {
                    int candidate = (index + offset) % state.Visited.Length;
                    if (!state.Visited[candidate]) { next = candidate; break; }
                }
                if (next == -1)
                { Array.Clear(state.Visited, 0, state.Visited.Length); next = (index + 1) % state.Visited.Length; }
                state.RouteIndex = index = next;
                state.ProgressAt = now;
                state.BestDistance = float.MaxValue;
                distance = TINHKIEM.GetDistance(CharX, CharY, KyCuocRoute[index,0], KyCuocRoute[index,1]);
            }
            if (distance < state.BestDistance - 1f)
            { state.BestDistance = distance; state.ProgressAt = now; }
            if (now - state.ProgressAt >= KyCuocTravelTimeout)
            { StopKyCuoc("không tiến triển trên tuyến tuần tra."); return; }
            if (!CanControlKyCuoc(this)) return;
            if (!IsRide && TLBB.HaveRide) { UpRide(); return; }
            Move(KyCuocRoute[index,0], KyCuocRoute[index,1]);
        }

        public void DatDoiKyCuoc()
        {
            if (!IsKyCuoc) return;
            lock (kyCuocLock)
            {
                try
                {
                    CheckKyCuocSession();
                    if (!IsKyCuoc || !CanControlKyCuoc(this) || TickCount % 18 != 0) return;
                    var participants = new List<Game> { this };
                    foreach (Game member in Party.ToArray())
                    {
                        if (member == null || member == this || participants.Contains(member) || !member.IsAuto
                            || member.TLBB == null || member.TLBB.KeyId != TLBB.Id || member.TLBB.IsLeader) continue;
                        participants.Add(member);
                        if (!kyCuocMembers.ContainsKey(member))
                        {
                            if (kyCuocPhase == KyCuocPhase.Exit) { StopKyCuoc("đội thay đổi trong bước ra cửa."); return; }
                            long now = kyCuocClock.ElapsedMilliseconds;
                            kyCuocMembers.Add(member, new KyCuocState { Started = now, ClearAt = now, ProgressAt = now });
                        }
                    }
                    if (kyCuocPhase == KyCuocPhase.Exit)
                        foreach (Game tracked in kyCuocMembers.Keys)
                            if (!participants.Contains(tracked) && !kyCuocMembers[tracked].Exited)
                            { StopKyCuoc("thành viên đã tắt Auto hoặc rời đội trong bước ra cửa."); return; }
                    if (kyCuocPhase != KyCuocPhase.Exit)
                    {
                        var inactive = new List<Game>();
                        foreach (Game tracked in kyCuocMembers.Keys)
                            if (!participants.Contains(tracked)) inactive.Add(tracked);
                        foreach (Game tracked in inactive) kyCuocMembers.Remove(tracked);
                    }
                    bool ready = true, combat = false, loot = false;
                    foreach (Game participant in participants)
                    {
                        KyCuocState state = participant == this ? kyCuocLeader : kyCuocMembers[participant];
                        if (!CanControlKyCuoc(participant))
                        {
                            ready = false;
                            long now = kyCuocClock.ElapsedMilliseconds;
                            if (state.UnavailableAt < 0) state.UnavailableAt = now;
                            if (now - state.UnavailableAt >= KyCuocTravelTimeout)
                            { StopKyCuoc("hết thời gian chờ thành viên sẵn sàng."); return; }
                            continue;
                        }
                        state.UnavailableAt = -1;
                        if (!ObserveKyCuocMap(participant, state)) return;
                        if (state.Exited) continue;
                        ObserveKyCuocBoss(participant, state);
                        if (participant.TLBB.MapId == MAP.TranLongKyCuoc
                            && participant.Objects.NearMonter20m.Count > 0)
                        {
                            combat = true;
                            if (participant.TLBB.IsFollow && CanControlKyCuoc(participant)) participant.StopFollow();
                            if (participant.IsRide && CanControlKyCuoc(participant)) participant.DownRide();
                        }
                        if (CanControlKyCuoc(participant) && participant.PickItem()) loot = true;
                    }
                    if (!ready || !IsKyCuoc) return;
                    if (combat)
                    {
                        kyCuocLeader.ClearAt = kyCuocLeader.ProgressAt = kyCuocClock.ElapsedMilliseconds;
                        kyCuocLeader.Reanchor = true;
                        return;
                    }
                    if (loot) { kyCuocLeader.ProgressAt = kyCuocClock.ElapsedMilliseconds; return; }
                    if (kyCuocPhase == KyCuocPhase.Entry)
                    {
                        bool allInside = true, atDoor = true;
                        foreach (Game participant in participants)
                        {
                            if (participant.TLBB.MapId == MAP.TranLongKyCuoc) continue;
                            allInside = false;
                            KyCuocState state = participant == this ? kyCuocLeader : kyCuocMembers[participant];
                            if (KyCuocTimeout(state, state.Selected ? KyCuocDialogTimeout : KyCuocTravelTimeout, "vào cửa")) return;
                            if (!state.Selected && CanControlKyCuoc(participant) && !participant.GoTo(LACDUONG.VuongTichTan)) atDoor = false;
                        }
                        if (!allInside)
                        {
                            if (atDoor)
                                foreach (Game participant in participants)
                                {
                                    if (participant.TLBB.MapId == MAP.TranLongKyCuoc) continue;
                                    KyCuocState state = participant == this ? kyCuocLeader : kyCuocMembers[participant];
                                    SendKyCuocDialog(participant, state, LACDUONG.VuongTichTan, 401001, -1);
                                }
                            return;
                        }
                        kyCuocPhase = KyCuocPhase.Patrol;
                    }
                    if (kyCuocBossDeadAt >= 0 && kyCuocPhase == KyCuocPhase.Patrol)
                        kyCuocPhase = KyCuocPhase.WaitingExit;
                    if (kyCuocPhase == KyCuocPhase.WaitingExit)
                    {
                        if (kyCuocClock.ElapsedMilliseconds - kyCuocLeader.ClearAt < KyCuocClearWait) return;
                        long elapsed = kyCuocClock.ElapsedMilliseconds - kyCuocBossDeadAt;
                        if (elapsed < KyCuocExitWait)
                        { PushDebugMessage("Di chuyển sau " + Math.Max(0, (KyCuocExitWait - elapsed + 999) / 1000) + "s"); return; }
                        kyCuocPhase = KyCuocPhase.Exit;
                        foreach (Game participant in participants)
                        {
                            KyCuocState state = participant == this ? kyCuocLeader : kyCuocMembers[participant];
                            state.Started = kyCuocClock.ElapsedMilliseconds;
                            state.Selected = state.TalkSent = false;
                        }
                    }
                    if (kyCuocPhase == KyCuocPhase.Exit)
                    {
                        bool allExited = true;
                        foreach (Game participant in participants)
                        {
                            KyCuocState state = participant == this ? kyCuocLeader : kyCuocMembers[participant];
                            if (state.Exited) continue;
                            allExited = false;
                            if (KyCuocTimeout(state, KyCuocDialogTimeout, "xác nhận cửa ra")) return;
                            SendKyCuocDialog(participant, state, TRANLONGKYCUOC.TeThanh, 44000, 0);
                        }
                        if (allExited) { IsKyCuoc = false; IsXongKyCuoc = true; }
                        return;
                    }
                    PatrolKyCuoc();
                    if (!IsKyCuoc) return;
                    foreach (Game participant in participants)
                        if (participant != this && CanControlKyCuoc(participant)
                            && TINHKIEM.GetDistance(participant.CharX, participant.CharY, CharX, CharY) > 4f)
                            participant.GoTo(CharX, CharY);
                }
                catch (Exception error) { StopKyCuoc("lỗi đọc/xử lý " + error.GetType().Name + "."); }
            }
        }
    }
}
