using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TinhKiemAuto
{
    public partial class Game
    {
        private readonly object acBaLock = new object();
        private readonly Stopwatch acBaClock = Stopwatch.StartNew();

        private int acBaSchool = -1;
        private int[,] acBaDungeonRoute;
        private bool[] acBaVisited = new bool[0];
        private readonly Dictionary<Game, AcBaActor> acBaActors = new Dictionary<Game, AcBaActor>();
        private string acBaOwner, acBaTeam;
        private AcBaPhase acBaPhase;
        private long acBaPausedAt = -1, acBaPauseOffset, acBaPhaseAt, acBaClearAt, acBaProgressAt;
        private int acBaIndex;
        private bool acBaReanchor = true;
        private float acBaBestDistance = float.MaxValue;
        private const long AcBaDialogTimeout = 60000, AcBaTravelTimeout = 180000;
        private enum AcBaPhase { Entry, Patrol, AwaitBoss, AwaitExit }
        private sealed class AcBaActor
        {
            internal string CharacterId;
            internal int Map = -1, NpcId = -1;
            internal bool TalkSent, Selected, ClosedStale, WasInside, Transitioning;
            internal long Started, UnavailableAt = -1;
            internal readonly HashSet<int> LiveBossIds = new HashSet<int>();
        }
        private long AcBaNow { get { return acBaClock.ElapsedMilliseconds - acBaPauseOffset; } }

        private void ResetAcBaSession()
        {
            acBaActors.Clear();
            acBaSchool = -1;
            acBaDungeonRoute = null;
            acBaVisited = new bool[0];
            acBaOwner = acBaTeam = null;
            acBaPhase = AcBaPhase.Entry;
            acBaPausedAt = -1;
            acBaPauseOffset = 0;
            acBaIndex = 0;
            Array.Clear(acBaVisited, 0, acBaVisited.Length);
            acBaReanchor = true;
            acBaBestDistance = float.MaxValue;
            acBaPhaseAt = acBaClearAt = acBaProgressAt = AcBaNow;
        }

        private void StopAcBa(string reason)
        {
            IsAcBa = false;
            if (reason != null) CanhBao.Msg("Ác Bá", "Đã dừng Ác Bá: " + reason, CanhBao.Kieu.Eror);
        }

        private bool AcBaConflict(Game actor)
        {
            return actor.IsThuyLao || actor.IsKyCuoc || actor.IsTrungAc || actor.MapAcTac != 0 || actor.IsLauLanTamBao
                || actor.MapTKC != 0 || actor.IsQ123LauLan || actor.IsQ123ToChau || actor.IsYenTuO
                || actor.IsPhungHoangLangMo || actor.IsPMP || actor.IsTuBaoBon || actor.IsLuyenKim
                || actor.IsHuyetChien || actor.IsBachHoaDuyen || actor.IsSuMon || actor.IsXayDung || actor.IsTuDuong || actor.IsNhiemVuCoBan;
        }

        private bool CanControlAcBa(Game actor)
        {
            if (!IsAcBa || !IsAuto || !IsInit || TLBB == null || !TLBB.Online || !TLBB.IsLeader
                || TLBB.Id != acBaOwner || TLBB.KeyId != acBaTeam || acBaTeam != acBaOwner
                || TLBB.PlayerState != 0 || Global.Paused || ON_SCENE_TRANSING || IsChangeMap || AcBaConflict(this)) return false;
            if (actor == null || !actor.IsAuto || !actor.IsInit || actor.TLBB == null || !actor.TLBB.Online
                || string.IsNullOrEmpty(actor.TLBB.Id)
                || actor.TLBB.PlayerState != 0 || actor.ON_SCENE_TRANSING || actor.IsChangeMap || AcBaConflict(actor)) return false;
            AcBaActor state;
            if (acBaActors.TryGetValue(actor, out state) && state.CharacterId != actor.TLBB.Id) return false;
            return actor == this || (!actor.TLBB.IsLeader && actor.TLBB.KeyId == acBaOwner
                && !actor.IsAcBa && actor.tranTime.Elapsed.TotalSeconds >= 2.0);
        }

        // Called before Auto's scene/leader dispatcher; session cancellation cannot depend on that dispatcher.
        private void CheckAcBaSession()
        {
            if (!IsAcBa) return;
            lock (acBaLock)
            {
                try
                {
                    if (!IsAuto || !IsInit || TLBB == null || !TLBB.Online
                        || string.IsNullOrEmpty(TLBB.Id)
                        || TLBB.PlayerState == 2 || TLBB.PlayerState == 9 || TLBB.MapId == MAP.GiamNguc)
                    { StopAcBa(null); return; }
                    bool pause = Global.Paused || ON_SCENE_TRANSING || IsChangeMap || TLBB.PlayerState == 7;
                    long raw = acBaClock.ElapsedMilliseconds;
                    if (pause && acBaPausedAt < 0) acBaPausedAt = raw;
                    if (!pause && acBaPausedAt >= 0)
                    { acBaPauseOffset += raw - acBaPausedAt; acBaPausedAt = -1; }
                    foreach (KeyValuePair<Game, AcBaActor> tracked in acBaActors)
                    {
                        bool transition = tracked.Key.ON_SCENE_TRANSING || tracked.Key.IsChangeMap;
                        if (transition && !tracked.Value.Transitioning)
                        {
                            tracked.Value.LiveBossIds.Clear();
                            tracked.Value.NpcId = -1;
                            tracked.Value.TalkSent = tracked.Value.Selected = tracked.Value.ClosedStale = false;
                            acBaReanchor = true;
                        }
                        tracked.Value.Transitioning = transition;
                    }
                    CheckAcBaNoticeIdentity();
                    if (acBaOwner != null && acBaOwner != TLBB.Id) { StopAcBa("nhân vật đã thay đổi."); return; }
                    if (AcBaConflict(this)) { StopAcBa("đang có nhiệm vụ khác."); return; }
                    if (CheckAcBaHandoff()) return;
                    if (!TLBB.IsLeader || TLBB.KeyId != TLBB.Id) { StopAcBa(null); return; }
                    if (acBaOwner == null) { acBaOwner = TLBB.Id; acBaTeam = TLBB.KeyId; }
                    if (acBaTeam != TLBB.KeyId) { StopAcBa("đội đã thay đổi."); return; }
                }
                catch (Exception error) { StopAcBa("lỗi đọc/xử lý " + error.GetType().Name + "."); }
            }
        }

        private bool AcBaTimeout(long started, long limit, string step)
        {
            if (AcBaNow - started < limit) return false;
            StopAcBa("hết thời gian chờ " + step + "; hãy kiểm tra bằng tay.");
            return true;
        }

        private int AcBaOutdoorMap { get { return AcBaEvents.OutdoorMap(acBaSchool); } }
        private int AcBaDungeonMap { get { return AcBaEvents.DungeonMap(acBaSchool); } }
        private int[,] AcBaOutdoorRoute() { return AcBaEvents.OutdoorRoute(acBaSchool); }

        private bool ObserveAcBaMap(Game actor, AcBaActor state)
        {
            if (state.CharacterId != actor.TLBB.Id) { StopAcBa("nhân vật trong đội đã thay đổi."); return false; }
            int map = actor.TLBB.MapId;
            if (map == state.Map) return true;
            if (state.WasInside && map != AcBaDungeonMap)
            {
                if (acBaPhase != AcBaPhase.AwaitExit || map != AcBaOutdoorMap)
                { StopAcBa("rời phụ bản khi chưa xác nhận boss/cửa ra đúng map."); return false; }
            }
            if (map == AcBaDungeonMap)
            {
                state.WasInside = true;
                if (actor == this)
                {
                    acBaIndex = 0;
                    Array.Clear(acBaVisited, 0, acBaVisited.Length);
                    acBaReanchor = true;
                    acBaBestDistance = float.MaxValue;
                    acBaClearAt = acBaProgressAt = AcBaNow;
                }
            }
            state.Map = map;
            state.NpcId = -1;
            state.Selected = state.TalkSent = state.ClosedStale = false;
            state.Started = AcBaNow;
            state.LiveBossIds.Clear();
            return true;
        }

        private GameObject ReadAcBaNpc(Game actor)
        {
            GameObject found = null;
            float nearest = 100f;
            foreach (GameObject obj in actor.Objects.All)
            {
                if (obj == null || !obj.IsNPC || obj.Name == null || float.IsNaN(obj.X) || float.IsInfinity(obj.X)
                    || float.IsNaN(obj.Y) || float.IsInfinity(obj.Y)) continue;
                bool match = TINHKIEM.VietLien(obj.Name) == "gianghotieutieu";
                float distance = TINHKIEM.GetDistance(actor.CharX, actor.CharY, obj.X, obj.Y);
                if (match && distance < nearest) { nearest = distance; found = obj; }
            }
            return found;
        }

        private bool EnterAcBa(Game actor, AcBaActor state)
        {
            if (!CanControlAcBa(actor)) return false;
            if (state.Selected) return true;
            if ((state.TalkSent || state.ClosedStale)
                && AcBaTimeout(state.Started, AcBaDialogTimeout, "NPC/dialog vào cửa")) return true;
            GameObject npc = ReadAcBaNpc(actor);
            if (npc == null) return false;
            if (state.NpcId >= 0 && state.NpcId != npc.Id) { StopAcBa("NPC vào cửa đã thay đổi."); return true; }
            if (TINHKIEM.GetDistance(actor.CharX, actor.CharY, npc.X, npc.Y) > 3f)
            { if (CanControlAcBa(actor)) actor.Move(npc.X, npc.Y); return true; }
            if (!state.TalkSent)
            {
                if (actor.TLBB.IsQuestOpen)
                {
                    if (!state.ClosedStale && CanControlAcBa(actor))
                    { actor.CloseQuest(); state.ClosedStale = true; state.Started = AcBaNow; }
                    return true;
                }
                state.NpcId = npc.Id;
                if (CanControlAcBa(actor)) { actor.Talk(npc.Id); state.TalkSent = true; state.Started = AcBaNow; }
                return true;
            }
            if (!actor.TLBB.IsQuestOpen) return true;
            foreach (QuestFrame frame in QuestFrame.Enum(actor))
                if (frame != null && frame.StrOptionExtra1 == 50013 && frame.StrOptionExtra2 == -1)
                {
                    if (!CanControlAcBa(actor)) return true;
                    actor.QuestFrameOptionClicked(frame);
                    state.Selected = true;
                    state.Started = AcBaNow;
                    if (CanControlAcBa(actor)) actor.CloseQuest();
                    return true;
                }
            return true;
        }

        private void ObserveAcBaBoss(Game actor, AcBaActor state)
        {
            if (actor.TLBB.MapId != AcBaDungeonMap || acBaPhase == AcBaPhase.AwaitExit || !CanControlAcBa(actor)) return;
            foreach (GameObject obj in actor.Objects.All)
            {
                if (obj == null || obj.IsNPC || (obj.CleanName != "acba" && obj.CleanName != "tyrant") || float.IsNaN(obj.HP) || float.IsInfinity(obj.HP)) continue;
                if (obj.HP > 0) state.LiveBossIds.Add(obj.Id);
                else if (obj.HP == 0 && state.LiveBossIds.Contains(obj.Id))
                { acBaPhase = AcBaPhase.AwaitExit; acBaPhaseAt = AcBaNow; return; }
            }
        }

        private void PatrolAcBa(int[,] route, bool dungeon)
        {
            if (route == null || route.GetLength(0) == 0 || route.GetLength(1) != 2)
            { StopAcBa("không có tuyến hợp lệ."); return; }
            if (AcBaNow - acBaClearAt < 2000) return;
            if (acBaReanchor)
            {
                float best = float.MaxValue;
                int first = 0;
                for (int i = first; i < route.GetLength(0); i++)
                {
                    if (dungeon && acBaVisited[i]) continue;
                    float distance = TINHKIEM.GetDistance(CharX, CharY, route[i,0], route[i,1]);
                    if (distance < best) { best = distance; acBaIndex = i; }
                }
                acBaReanchor = false;
                acBaBestDistance = float.MaxValue;
                acBaProgressAt = AcBaNow;
            }
            if (acBaIndex < 0 || acBaIndex >= route.GetLength(0)) { StopAcBa("index tuyến không hợp lệ."); return; }
            float current = TINHKIEM.GetDistance(CharX, CharY, route[acBaIndex,0], route[acBaIndex,1]);
            if (float.IsNaN(current) || float.IsInfinity(current)) { StopAcBa("tọa độ nhân vật không hợp lệ."); return; }
            if (current <= 2f)
            {
                if (dungeon)
                {
                    acBaVisited[acBaIndex] = true;
                    int next = -1;
                    for (int offset = 1; offset <= route.GetLength(0); offset++)
                    {
                        int candidate = (acBaIndex + offset) % route.GetLength(0);
                        if (!acBaVisited[candidate]) { next = candidate; break; }
                    }
                    acBaIndex = next < 0 ? route.GetLength(0) : next;
                }
                else acBaIndex++;
                acBaProgressAt = AcBaNow;
                acBaBestDistance = float.MaxValue;
                if (acBaIndex == route.GetLength(0))
                {
                    if (dungeon) { acBaPhase = AcBaPhase.AwaitBoss; acBaPhaseAt = AcBaNow; return; }
                    acBaIndex = 0;
                }
                current = TINHKIEM.GetDistance(CharX, CharY, route[acBaIndex,0], route[acBaIndex,1]);
            }
            if (current < acBaBestDistance - 1f) { acBaBestDistance = current; acBaProgressAt = AcBaNow; }
            if (AcBaTimeout(acBaProgressAt, AcBaTravelTimeout, "tiến triển tuần tra") || !CanControlAcBa(this)) return;
            if (IsRide) { DownRide(); return; }
            if (TLBB.IsFollow) { StopFollow(); return; }
            Move(route[acBaIndex,0], route[acBaIndex,1]);
        }

        public void DatDoiAcBa()
        {
            if (!IsAcBa) return;
            lock (acBaLock)
            {
                try
                {
                    CheckAcBaSession();
                    if (acBaHandoff != null || !CanControlAcBa(this) || TickCount % 9 != 0) return;
                    if (!SelectAcBaTarget()) return;
                    if (IsMapPhuBan() && TLBB.MapId != AcBaDungeonMap)
                    { StopAcBa("đang ở phụ bản khác."); return; }
                    if (acBaSchool != TLBB.Menpai)
                    { TryAcBaHandoff(); return; }
                    var participants = new List<Game> { this };
                    foreach (Game member in Party.ToArray())
                        if (member != null && member != this && !participants.Contains(member) && member.IsAuto
                            && member.TLBB != null && member.TLBB.KeyId == acBaOwner && !member.TLBB.IsLeader) participants.Add(member);
                    foreach (Game tracked in new List<Game>(acBaActors.Keys))
                            if ((acBaPhase != AcBaPhase.Entry || acBaActors[tracked].WasInside) && !participants.Contains(tracked)) { StopAcBa("thành viên đã tắt Auto hoặc rời đội trong phụ bản."); return; }
                    bool ready = true, combat = false, loot = false, allInside = true, allExited = true;
                    foreach (Game actor in participants)
                    {
                        AcBaActor state;
                        if (!acBaActors.TryGetValue(actor, out state))
                        {
                            if (acBaPhase != AcBaPhase.Entry && actor != this) { StopAcBa("đội thay đổi trong phụ bản."); return; }
                            state = new AcBaActor { CharacterId = actor.TLBB.Id, Started = AcBaNow };
                            acBaActors.Add(actor, state);
                        }
                        if (state.CharacterId != actor.TLBB.Id) { StopAcBa("nhân vật trong đội đã thay đổi."); return; }
                        if (!CanControlAcBa(actor))
                        {
                            ready = false;
                            if (state.UnavailableAt < 0) state.UnavailableAt = AcBaNow;
                            if (AcBaTimeout(state.UnavailableAt, AcBaTravelTimeout, "thành viên sẵn sàng")) return;
                            continue;
                        }
                        state.UnavailableAt = -1;
                        if (!ObserveAcBaMap(actor, state)) return;
                        if (actor.IsMapPhuBan() && actor.TLBB.MapId != AcBaDungeonMap)
                        { StopAcBa("thành viên đang ở phụ bản khác."); return; }
                        if (acBaPhase == AcBaPhase.Entry && actor.TLBB.MapId != AcBaDungeonMap
                            && AcBaTimeout(state.Started, state.Selected ? AcBaDialogTimeout : AcBaTravelTimeout, "vào map phụ bản")) return;
                        allInside &= actor.TLBB.MapId == AcBaDungeonMap;
                        allExited &= state.WasInside && actor.TLBB.MapId == AcBaOutdoorMap;
                        ObserveAcBaBoss(actor, state);
                        if (actor.TLBB.MapId == AcBaDungeonMap && actor.Objects.NearMonter20m.Count > 0)
                        {
                            combat = true;
                            if (actor.TLBB.IsFollow && CanControlAcBa(actor)) actor.StopFollow();
                            if (actor.IsRide && CanControlAcBa(actor)) actor.DownRide();
                        }
                        if (CanControlAcBa(actor) && actor.PickItem()) loot = true;
                    }
                    if (!ready || !IsAcBa) return;
                    if (acBaPhase == AcBaPhase.AwaitExit)
                    {
                        if (allExited) { StopAcBa("đã về map ngoài; chưa xác nhận phần thưởng, cần chọn sự kiện mới."); return; }
                        AcBaTimeout(acBaPhaseAt, AcBaDialogTimeout, "map sau khi boss chết; chưa có protocol cửa ra");
                        return;
                    }
                    if (combat || loot)
                    { acBaClearAt = acBaProgressAt = AcBaNow; acBaReanchor = true; return; }
                    if (acBaPhase == AcBaPhase.Entry && allInside)
                    { acBaPhase = AcBaPhase.Patrol; acBaClearAt = acBaProgressAt = AcBaNow; }
                    if (acBaPhase == AcBaPhase.Entry)
                    {
                        foreach (Game actor in participants)
                        {
                            if (!CanControlAcBa(actor) || actor.TLBB.MapId == AcBaDungeonMap) continue;
                            AcBaActor state = acBaActors[actor];
                            if (AcBaTimeout(state.Started, state.Selected ? AcBaDialogTimeout : AcBaTravelTimeout, "vào map phụ bản")) return;
                            if (actor.TLBB.MapId != AcBaOutdoorMap)
                            {
                                int[,] route = AcBaOutdoorRoute();
                                if (route == null || route.GetLength(0) == 0) { StopAcBa("thiếu tuyến map ngoài."); return; }
                                if (CanControlAcBa(actor)) actor.GoTo(route[0,0], route[0,1], AcBaOutdoorMap);
                            }
                            else if (!EnterAcBa(actor, state))
                            {
                                if (actor == this) PatrolAcBa(AcBaOutdoorRoute(), false);
                                else if (CanControlAcBa(actor) && TLBB.MapId == AcBaOutdoorMap
                                    && TINHKIEM.GetDistance(actor.CharX, actor.CharY, CharX, CharY) > 4f)
                                    actor.GoTo(CharX, CharY, AcBaOutdoorMap);
                            }
                        }
                        return;
                    }
                    if (acBaPhase == AcBaPhase.AwaitBoss)
                    { AcBaTimeout(acBaPhaseAt, AcBaDialogTimeout, "boss sau khi hết tuyến; chưa xác nhận hoàn tất"); return; }
                    PatrolAcBa(acBaDungeonRoute, true);
                    foreach (Game actor in participants)
                        if (actor != this && CanControlAcBa(actor) && actor.TLBB.MapId == TLBB.MapId
                            && TINHKIEM.GetDistance(actor.CharX, actor.CharY, CharX, CharY) > 4f)
                            actor.GoTo(CharX, CharY, TLBB.MapId);
                }
                catch (Exception error) { StopAcBa("lỗi đọc/xử lý " + error.GetType().Name + "."); }
            }
        }
    }
}
