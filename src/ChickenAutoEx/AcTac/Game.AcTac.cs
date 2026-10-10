using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TinhKiemAuto
{
    public partial class Game
    {
        private readonly object acTacLock = new object();
        private readonly Stopwatch acTacClock = Stopwatch.StartNew();
        private static readonly Random acTacRandom = new Random();
        private static readonly int[] AcTacMaps = { MAP.VoLuongSon, MAP.KinhHo, MAP.KiemCac, MAP.ThaiHo, MAP.TungSon, MAP.DonHoang };
        private static readonly int[,] AcTacDungeonRoute = { {96,84},{109,52},{107,43},{108,21},{95,20},{76,21},{63,28},
            {47,20},{32,25},{23,21},{20,31},{42,40},{42,72},{21,94} };
        private readonly Dictionary<Game, AcTacActor> acTacActors = new Dictionary<Game, AcTacActor>();
        private string acTacOwner, acTacTeam;
        private AcTacPhase acTacPhase;
        private long acTacPausedAt = -1, acTacPauseOffset, acTacPhaseAt, acTacClearAt, acTacProgressAt;
        private int acTacIndex;
        private readonly bool[] acTacVisited = new bool[14];
        private bool acTacReanchor = true;
        private float acTacBestDistance = float.MaxValue;
        private const long AcTacDialogTimeout = 60000, AcTacTravelTimeout = 180000;
        private enum AcTacPhase { Entry, Patrol, AwaitBoss, AwaitExit }
        private sealed class AcTacActor
        {
            internal string CharacterId;
            internal int Map = -1, NpcId = -1;
            internal bool TalkSent, Selected, ClosedStale, WasInside;
            internal long Started, UnavailableAt = -1;
            internal readonly HashSet<int> LiveBossIds = new HashSet<int>();
        }
        private long AcTacNow { get { return acTacClock.ElapsedMilliseconds - acTacPauseOffset; } }

        public static int ChooseAcTacMap()
        {
            lock (acTacRandom) return AcTacMaps[acTacRandom.Next(0, AcTacMaps.Length)];
        }

        private static bool ValidAcTacMap(int map)
        {
            return map == 0 || Array.IndexOf(AcTacMaps, map) >= 0;
        }

        private void ResetAcTacSession()
        {
            acTacActors.Clear();
            acTacOwner = acTacTeam = null;
            acTacPhase = AcTacPhase.Entry;
            acTacPausedAt = -1;
            acTacPauseOffset = 0;
            acTacIndex = 0;
            Array.Clear(acTacVisited, 0, acTacVisited.Length);
            acTacReanchor = true;
            acTacBestDistance = float.MaxValue;
            acTacPhaseAt = acTacClearAt = acTacProgressAt = AcTacNow;
        }

        private void StopAcTac(string reason)
        {
            MapAcTac = 0;
            if (reason != null) CanhBao.Msg("Ác Tặc", "Đã dừng Ác Tặc: " + reason, CanhBao.Kieu.Eror);
        }

        private bool AcTacConflict(Game actor)
        {
            return actor.IsThuyLao || actor.IsKyCuoc || actor.IsTrungAc || actor.IsAcBa || actor.IsLauLanTamBao
                || actor.MapTKC != 0 || actor.IsQ123LauLan || actor.IsQ123ToChau || actor.IsYenTuO
                || actor.IsPhungHoangLangMo || actor.IsPMP || actor.IsTuBaoBon || actor.IsLuyenKim
                || actor.IsBachHoaDuyen || actor.IsSuMon || actor.IsXayDung || actor.IsTuDuong || actor.IsNhiemVuCoBan;
        }

        private bool CanControlAcTac(Game actor)
        {
            if (MapAcTac == 0 || !IsAuto || !IsInit || TLBB == null || !TLBB.Online || !TLBB.IsLeader
                || TLBB.Id != acTacOwner || TLBB.KeyId != acTacTeam || acTacTeam != acTacOwner
                || TLBB.PlayerState != 0 || Global.Paused || ON_SCENE_TRANSING || IsChangeMap || AcTacConflict(this)) return false;
            if (actor == null || !actor.IsAuto || !actor.IsInit || actor.TLBB == null || !actor.TLBB.Online
                || actor.TLBB.PlayerState != 0 || actor.ON_SCENE_TRANSING || actor.IsChangeMap || AcTacConflict(actor)) return false;
            AcTacActor state;
            if (acTacActors.TryGetValue(actor, out state) && state.CharacterId != actor.TLBB.Id) return false;
            return actor == this || (!actor.TLBB.IsLeader && actor.TLBB.KeyId == acTacOwner
                && actor.MapAcTac == 0 && actor.tranTime.Elapsed.TotalSeconds >= 2.0);
        }

        // Called before Auto's scene/leader dispatcher; session cancellation cannot depend on that dispatcher.
        private void CheckAcTacSession()
        {
            if (MapAcTac == 0) return;
            lock (acTacLock)
            {
                try
                {
                    if (!IsAuto || !IsInit || TLBB == null || !TLBB.Online || !TLBB.IsLeader
                        || string.IsNullOrEmpty(TLBB.Id) || TLBB.KeyId != TLBB.Id
                        || TLBB.PlayerState == 2 || TLBB.PlayerState == 9 || TLBB.MapId == MAP.GiamNguc)
                    { StopAcTac(null); return; }
                    if (acTacOwner == null) { acTacOwner = TLBB.Id; acTacTeam = TLBB.KeyId; }
                    if (acTacOwner != TLBB.Id || acTacTeam != TLBB.KeyId)
                    { StopAcTac("nhân vật hoặc đội trưởng đã thay đổi."); return; }
                    if (AcTacConflict(this)) { StopAcTac("đang có nhiệm vụ khác."); return; }
                    bool pause = Global.Paused || ON_SCENE_TRANSING || IsChangeMap || TLBB.PlayerState == 7;
                    long raw = acTacClock.ElapsedMilliseconds;
                    if (pause && acTacPausedAt < 0) acTacPausedAt = raw;
                    if (!pause && acTacPausedAt >= 0)
                    { acTacPauseOffset += raw - acTacPausedAt; acTacPausedAt = -1; }
                }
                catch (Exception error) { StopAcTac("lỗi đọc/xử lý " + error.GetType().Name + "."); }
            }
        }

        private bool AcTacTimeout(long started, long limit, string step)
        {
            if (AcTacNow - started < limit) return false;
            StopAcTac("hết thời gian chờ " + step + "; hãy kiểm tra bằng tay.");
            return true;
        }

        private int[,] AcTacOutdoorRoute()
        {
            if (MapAcTac == MAP.VoLuongSon) return POINT.VoLuongSon;
            if (MapAcTac == MAP.KinhHo) return POINT.KinhHo;
            if (MapAcTac == MAP.KiemCac) return POINT.KiemCac;
            if (MapAcTac == MAP.ThaiHo) return POINT.ThaiHo;
            if (MapAcTac == MAP.TungSon) return POINT.TungSon;
            return POINT.DonHoang;
        }

        private bool ObserveAcTacMap(Game actor, AcTacActor state)
        {
            if (state.CharacterId != actor.TLBB.Id) { StopAcTac("nhân vật trong đội đã thay đổi."); return false; }
            int map = actor.TLBB.MapId;
            if (map == state.Map) return true;
            if (state.WasInside && map != MAP.TacKhauDoanhDia)
            {
                if (acTacPhase != AcTacPhase.AwaitExit || map != MapAcTac)
                { StopAcTac("rời phụ bản khi chưa xác nhận boss/cửa ra đúng map."); return false; }
            }
            if (map == MAP.TacKhauDoanhDia)
            {
                state.WasInside = true;
                if (actor == this)
                {
                    acTacIndex = 0;
                    Array.Clear(acTacVisited, 0, acTacVisited.Length);
                    acTacReanchor = true;
                    acTacBestDistance = float.MaxValue;
                    acTacClearAt = acTacProgressAt = AcTacNow;
                }
            }
            state.Map = map;
            state.NpcId = -1;
            state.Selected = state.TalkSent = state.ClosedStale = false;
            state.Started = AcTacNow;
            state.LiveBossIds.Clear();
            return true;
        }

        private GameObject ReadAcTacNpc(Game actor)
        {
            GameObject found = null;
            float nearest = 100f;
            foreach (GameObject obj in actor.Objects.All)
            {
                if (obj == null || !obj.IsNPC || obj.Name == null || float.IsNaN(obj.X) || float.IsInfinity(obj.X)
                    || float.IsNaN(obj.Y) || float.IsInfinity(obj.Y)) continue;
                bool match = obj.CleanName == "therebels" || obj.CleanName == "thiefraid"
                    || (obj.Name.Contains("c T") && obj.Name.Contains("o Ph"));
                float distance = TINHKIEM.GetDistance(actor.CharX, actor.CharY, obj.X, obj.Y);
                if (match && distance < nearest) { nearest = distance; found = obj; }
            }
            return found;
        }

        private bool EnterAcTac(Game actor, AcTacActor state)
        {
            if (!CanControlAcTac(actor)) return false;
            if (state.Selected) return true;
            if ((state.TalkSent || state.ClosedStale)
                && AcTacTimeout(state.Started, AcTacDialogTimeout, "NPC/dialog vào cửa")) return true;
            GameObject npc = ReadAcTacNpc(actor);
            if (npc == null) return false;
            if (state.NpcId >= 0 && state.NpcId != npc.Id) { StopAcTac("NPC vào cửa đã thay đổi."); return true; }
            if (TINHKIEM.GetDistance(actor.CharX, actor.CharY, npc.X, npc.Y) > 3f)
            { if (CanControlAcTac(actor)) actor.Move(npc.X, npc.Y); return true; }
            if (!state.TalkSent)
            {
                if (actor.TLBB.IsQuestOpen)
                {
                    if (!state.ClosedStale && CanControlAcTac(actor))
                    { actor.CloseQuest(); state.ClosedStale = true; state.Started = AcTacNow; }
                    return true;
                }
                state.NpcId = npc.Id;
                if (CanControlAcTac(actor)) { actor.Talk(npc.Id); state.TalkSent = true; state.Started = AcTacNow; }
                return true;
            }
            if (!actor.TLBB.IsQuestOpen) return true;
            foreach (QuestFrame frame in QuestFrame.Enum(actor))
                if (frame != null && frame.StrOptionExtra1 == 50013 && frame.StrOptionExtra2 == -1)
                {
                    if (!CanControlAcTac(actor)) return true;
                    actor.QuestFrameOptionClicked(frame);
                    state.Selected = true;
                    state.Started = AcTacNow;
                    if (CanControlAcTac(actor)) actor.CloseQuest();
                    return true;
                }
            return true;
        }

        private void ObserveAcTacBoss(Game actor, AcTacActor state)
        {
            if (actor.TLBB.MapId != MAP.TacKhauDoanhDia || acTacPhase == AcTacPhase.AwaitExit || !CanControlAcTac(actor)) return;
            foreach (GameObject obj in actor.Objects.All)
            {
                if (obj == null || obj.IsNPC || obj.CleanName != "tacbinhdaumuc" || float.IsNaN(obj.HP) || float.IsInfinity(obj.HP)) continue;
                if (obj.HP > 0) state.LiveBossIds.Add(obj.Id);
                else if (obj.HP == 0 && state.LiveBossIds.Contains(obj.Id))
                { acTacPhase = AcTacPhase.AwaitExit; acTacPhaseAt = AcTacNow; return; }
            }
        }

        private void PatrolAcTac(int[,] route, bool dungeon)
        {
            if (route == null || route.GetLength(0) == 0 || route.GetLength(1) != 2)
            { StopAcTac("không có tuyến hợp lệ."); return; }
            if (AcTacNow - acTacClearAt < 2000) return;
            if (acTacReanchor)
            {
                float best = float.MaxValue;
                int first = 0;
                for (int i = first; i < route.GetLength(0); i++)
                {
                    if (dungeon && acTacVisited[i]) continue;
                    float distance = TINHKIEM.GetDistance(CharX, CharY, route[i,0], route[i,1]);
                    if (distance < best) { best = distance; acTacIndex = i; }
                }
                acTacReanchor = false;
                acTacBestDistance = float.MaxValue;
                acTacProgressAt = AcTacNow;
            }
            if (acTacIndex < 0 || acTacIndex >= route.GetLength(0)) { StopAcTac("index tuyến không hợp lệ."); return; }
            float current = TINHKIEM.GetDistance(CharX, CharY, route[acTacIndex,0], route[acTacIndex,1]);
            if (float.IsNaN(current) || float.IsInfinity(current)) { StopAcTac("tọa độ nhân vật không hợp lệ."); return; }
            if (current <= 2f)
            {
                if (dungeon)
                {
                    acTacVisited[acTacIndex] = true;
                    int next = -1;
                    for (int offset = 1; offset <= route.GetLength(0); offset++)
                    {
                        int candidate = (acTacIndex + offset) % route.GetLength(0);
                        if (!acTacVisited[candidate]) { next = candidate; break; }
                    }
                    acTacIndex = next < 0 ? route.GetLength(0) : next;
                }
                else acTacIndex++;
                acTacProgressAt = AcTacNow;
                acTacBestDistance = float.MaxValue;
                if (acTacIndex == route.GetLength(0))
                {
                    if (dungeon) { acTacPhase = AcTacPhase.AwaitBoss; acTacPhaseAt = AcTacNow; return; }
                    acTacIndex = 0;
                }
                current = TINHKIEM.GetDistance(CharX, CharY, route[acTacIndex,0], route[acTacIndex,1]);
            }
            if (current < acTacBestDistance - 1f) { acTacBestDistance = current; acTacProgressAt = AcTacNow; }
            if (AcTacTimeout(acTacProgressAt, AcTacTravelTimeout, "tiến triển tuần tra") || !CanControlAcTac(this)) return;
            if (IsRide) { DownRide(); return; }
            if (TLBB.IsFollow) { StopFollow(); return; }
            Move(route[acTacIndex,0], route[acTacIndex,1]);
        }

        public void DatDoiAcTac()
        {
            if (MapAcTac == 0) return;
            lock (acTacLock)
            {
                try
                {
                    CheckAcTacSession();
                    if (!CanControlAcTac(this) || TickCount % 9 != 0) return;
                    var participants = new List<Game> { this };
                    foreach (Game member in Party.ToArray())
                        if (member != null && member != this && !participants.Contains(member) && member.IsAuto
                            && member.TLBB != null && member.TLBB.KeyId == acTacOwner && !member.TLBB.IsLeader) participants.Add(member);
                    if (acTacPhase != AcTacPhase.Entry)
                        foreach (Game tracked in acTacActors.Keys)
                            if (!participants.Contains(tracked)) { StopAcTac("thành viên đã tắt Auto hoặc rời đội trong phụ bản."); return; }
                    bool ready = true, combat = false, loot = false, allInside = true, allExited = true;
                    foreach (Game actor in participants)
                    {
                        AcTacActor state;
                        if (!acTacActors.TryGetValue(actor, out state))
                        {
                            if (acTacPhase != AcTacPhase.Entry && actor != this) { StopAcTac("đội thay đổi trong phụ bản."); return; }
                            state = new AcTacActor { CharacterId = actor.TLBB.Id, Started = AcTacNow };
                            acTacActors.Add(actor, state);
                        }
                        if (state.CharacterId != actor.TLBB.Id) { StopAcTac("nhân vật trong đội đã thay đổi."); return; }
                        if (!CanControlAcTac(actor))
                        {
                            ready = false;
                            if (state.UnavailableAt < 0) state.UnavailableAt = AcTacNow;
                            if (AcTacTimeout(state.UnavailableAt, AcTacTravelTimeout, "thành viên sẵn sàng")) return;
                            continue;
                        }
                        state.UnavailableAt = -1;
                        if (!ObserveAcTacMap(actor, state)) return;
                        allInside &= actor.TLBB.MapId == MAP.TacKhauDoanhDia;
                        allExited &= state.WasInside && actor.TLBB.MapId == MapAcTac;
                        ObserveAcTacBoss(actor, state);
                        if (actor.TLBB.MapId == MAP.TacKhauDoanhDia && actor.Objects.NearMonter20m.Count > 0)
                        {
                            combat = true;
                            if (actor.TLBB.IsFollow && CanControlAcTac(actor)) actor.StopFollow();
                            if (actor.IsRide && CanControlAcTac(actor)) actor.DownRide();
                        }
                        if (CanControlAcTac(actor) && actor.PickItem()) loot = true;
                    }
                    if (!ready || MapAcTac == 0) return;
                    if (acTacPhase == AcTacPhase.AwaitExit)
                    {
                        if (allExited) { ResetAcTacSession(); return; }
                        AcTacTimeout(acTacPhaseAt, AcTacDialogTimeout, "map sau khi boss chết; chưa có protocol cửa ra");
                        return;
                    }
                    if (combat || loot)
                    { acTacClearAt = acTacProgressAt = AcTacNow; acTacReanchor = true; return; }
                    if (acTacPhase == AcTacPhase.Entry && allInside)
                    { acTacPhase = AcTacPhase.Patrol; acTacClearAt = acTacProgressAt = AcTacNow; }
                    if (acTacPhase == AcTacPhase.Entry)
                    {
                        foreach (Game actor in participants)
                        {
                            if (!CanControlAcTac(actor) || actor.TLBB.MapId == MAP.TacKhauDoanhDia) continue;
                            AcTacActor state = acTacActors[actor];
                            if (AcTacTimeout(state.Started, state.Selected ? AcTacDialogTimeout : AcTacTravelTimeout, "vào map phụ bản")) return;
                            if (actor.TLBB.MapId != MapAcTac)
                            {
                                int[,] route = AcTacOutdoorRoute();
                                if (CanControlAcTac(actor)) actor.GoTo(route[0,0], route[0,1], MapAcTac);
                            }
                            else if (!EnterAcTac(actor, state))
                            {
                                if (actor == this) PatrolAcTac(AcTacOutdoorRoute(), false);
                                else if (CanControlAcTac(actor) && TLBB.MapId == MapAcTac
                                    && TINHKIEM.GetDistance(actor.CharX, actor.CharY, CharX, CharY) > 4f)
                                    actor.GoTo(CharX, CharY, MapAcTac);
                            }
                        }
                        return;
                    }
                    if (acTacPhase == AcTacPhase.AwaitBoss)
                    { AcTacTimeout(acTacPhaseAt, AcTacDialogTimeout, "boss sau khi hết tuyến; chưa xác nhận hoàn tất"); return; }
                    PatrolAcTac(AcTacDungeonRoute, true);
                    foreach (Game actor in participants)
                        if (actor != this && CanControlAcTac(actor) && actor.TLBB.MapId == TLBB.MapId
                            && TINHKIEM.GetDistance(actor.CharX, actor.CharY, CharX, CharY) > 4f)
                            actor.GoTo(CharX, CharY, TLBB.MapId);
                }
                catch (Exception error) { StopAcTac("lỗi đọc/xử lý " + error.GetType().Name + "."); }
            }
        }
    }
}
