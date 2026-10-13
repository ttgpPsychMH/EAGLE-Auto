using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TinhKiemAuto.Diagnostics;

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
        private const long AcTacReadGrace = 5000, AcTacFollowInterval = 5000;
        private long acTacUnconfirmedAt = -1, acTacFollowAt = -5000, acTacHoldAt = -1;
        private long acTacSnapshotAt = -1;
        private int acTacHoldCount = -1;
        private double acTacHoldHealth;
        private volatile Game acTacCoordinator;
        // Immutable cached projection. No identity, coordinates or native data are serialized.
        private volatile int[] acTacDiagnostic = new int[8];
        private int acTacParticipantCount, acTacReadyCount, acTacInsideCount, acTacCombatCount, acTacMovingCount;
        private enum AcTacPhase { Entry, Patrol, AwaitBoss, AwaitExit }
        private sealed class AcTacActor
        {
            internal string CharacterId;
            internal int Map = -1, NpcId = -1;
            internal bool TalkSent, Selected, ClosedStale, WasInside;
            internal long Started, UnavailableAt = -1;
            internal int OutdoorIndex = -1, TalkAttempts;
            internal long TalkAt, ProgressAt;
            internal float X, Y;
            internal bool PositionKnown;
            internal readonly HashSet<int> LiveBossIds = new HashSet<int>();
        }
        private long AcTacNow { get { return (acTacPausedAt >= 0 ? acTacPausedAt : acTacClock.ElapsedMilliseconds) - acTacPauseOffset; } }

        private bool AcTacControlsCurrentActor()
        {
            Game owner = acTacCoordinator;
            return mapAcTac != 0 || (owner != null && owner.mapAcTac != 0 && IsAuto);
        }

        private void PublishAcTac(DiagnosticReason reason)
        {
            acTacDiagnostic = new[] { (int)acTacPhase, MapAcTac != 0 ? 1 : 0, acTacParticipantCount,
                acTacReadyCount, acTacInsideCount, acTacCombatCount, acTacMovingCount, (int)reason };
        }

        private void ObserveAcTacSelection(int value)
        {
            if (value == 0) acTacDiagnostic = new[] { (int)acTacPhase, 0, acTacParticipantCount,
                acTacReadyCount, acTacInsideCount, acTacCombatCount, acTacMovingCount, (int)DiagnosticReason.SessionCleared };
        }

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
            foreach (Game actor in acTacActors.Keys)
                if (actor.acTacCoordinator == this) actor.acTacCoordinator = null;
            acTacActors.Clear();
            acTacOwner = acTacTeam = null;
            acTacPhase = AcTacPhase.Entry;
            acTacPausedAt = -1;
            acTacPauseOffset = 0;
            acTacIndex = 0;
            Array.Clear(acTacVisited, 0, acTacVisited.Length);
            acTacReanchor = true;
            acTacBestDistance = float.MaxValue;
            acTacUnconfirmedAt = acTacHoldAt = acTacSnapshotAt = -1;
            acTacFollowAt = -AcTacFollowInterval;
            acTacHoldCount = -1;
            acTacHoldHealth = 0;
            acTacParticipantCount = acTacReadyCount = acTacInsideCount = acTacCombatCount = acTacMovingCount = 0;
            acTacPhaseAt = acTacClearAt = acTacProgressAt = AcTacNow;
        }

        private void StopAcTac(string reason, DiagnosticReason code = DiagnosticReason.InvalidState)
        {
            int[] stopped = new[] { (int)acTacPhase, 0, acTacParticipantCount, acTacReadyCount,
                acTacInsideCount, acTacCombatCount, acTacMovingCount, (int)code };
            MapAcTac = 0;
            acTacDiagnostic = stopped;
            if (reason != null) CanhBao.Msg("Ác Tặc", "Đã dừng Ác Tặc: " + reason, CanhBao.Kieu.Eror);
        }

        private void PauseAcTacClock(bool pause)
        {
            long raw = acTacClock.ElapsedMilliseconds;
            if (pause && acTacPausedAt < 0) acTacPausedAt = raw;
            if (!pause && acTacPausedAt >= 0)
            { acTacPauseOffset += raw - acTacPausedAt; acTacPausedAt = -1; }
        }

        private void WaitAcTacRead(DiagnosticReason reason)
        {
            long raw = acTacClock.ElapsedMilliseconds;
            if (acTacUnconfirmedAt < 0) acTacUnconfirmedAt = raw;
            PauseAcTacClock(true);
            PublishAcTac(reason);
            if (raw - acTacUnconfirmedAt >= AcTacReadGrace)
                StopAcTac("không xác nhận được nhân vật/đội trong 5 giây; hãy kiểm tra bằng tay.", reason);
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
                || acTacUnconfirmedAt >= 0 || TLBB.PlayerState != 0 || Global.Paused || ON_SCENE_TRANSING || IsChangeMap || AcTacConflict(this)) return false;
            if (actor == null || !actor.IsAuto || !actor.IsInit || actor.TLBB == null || !actor.TLBB.Online
                || string.IsNullOrEmpty(actor.TLBB.Id)
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
                    if (!IsAuto) { StopAcTac(null, DiagnosticReason.AutoDisabled); return; }
                    // Scene changes may temporarily invalidate identity/online fields. Never command during them.
                    if (Global.Paused || ON_SCENE_TRANSING || IsChangeMap)
                    { PauseAcTacClock(true); PublishAcTac(DiagnosticReason.SceneTransition); return; }
                    if (!IsInit) { WaitAcTacRead(DiagnosticReason.NotInitialized); return; }
                    if (TLBB == null || !TLBB.Online) { WaitAcTacRead(DiagnosticReason.NotOnline); return; }
                    if (string.IsNullOrEmpty(TLBB.Id) || string.IsNullOrEmpty(TLBB.KeyId))
                    { WaitAcTacRead(DiagnosticReason.MissingIdentity); return; }
                    if (acTacOwner != null && (acTacOwner != TLBB.Id || acTacTeam != TLBB.KeyId))
                    { StopAcTac("nhân vật hoặc đội trưởng đã thay đổi.", DiagnosticReason.TeamChanged); return; }
                    if (!TLBB.IsLeader || TLBB.KeyId != TLBB.Id)
                    { WaitAcTacRead(DiagnosticReason.LeaderUnconfirmed); return; }
                    if (TLBB.PlayerState == 2 || TLBB.PlayerState == 9 || TLBB.MapId == MAP.GiamNguc)
                    { StopAcTac("nhân vật đang ở trạng thái không thể tiếp tục nhiệm vụ.", DiagnosticReason.CharacterUnavailable); return; }
                    if (acTacOwner == null) { acTacOwner = TLBB.Id; acTacTeam = TLBB.KeyId; }
                    acTacUnconfirmedAt = -1;
                    if (AcTacConflict(this)) { StopAcTac("đang có nhiệm vụ khác.", DiagnosticReason.OtherTask); return; }
                    PauseAcTacClock(TLBB.PlayerState == 7 || acTacSnapshotAt >= 0);
                    if (TLBB.PlayerState != 0) PublishAcTac(DiagnosticReason.CharacterUnavailable);
                }
                catch (Exception) { WaitAcTacRead(DiagnosticReason.ReadFailure); }
            }
        }

        private bool AcTacTimeout(long started, long limit, string step)
        {
            if (AcTacNow - started < limit) return false;
            StopAcTac("hết thời gian chờ " + step + "; hãy kiểm tra bằng tay.", DiagnosticReason.WaitingTimeout);
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
            state.TalkAttempts = 0;
            state.OutdoorIndex = -1;
            state.Started = AcTacNow;
            state.ProgressAt = AcTacNow;
            state.PositionKnown = false;
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
            if (state.Selected)
            { AcTacTimeout(state.Started, AcTacDialogTimeout, "map sau khi chọn cửa"); return true; }
            if ((state.TalkSent || state.ClosedStale)
                && AcTacTimeout(state.Started, AcTacDialogTimeout, "NPC/dialog vào cửa")) return true;
            GameObject npc = ReadAcTacNpc(actor);
            if (npc == null) { PublishAcTac(DiagnosticReason.MissingTarget); return false; }
            if (state.NpcId >= 0 && state.NpcId != npc.Id) { StopAcTac("NPC vào cửa đã thay đổi."); return true; }
            if (actor.TLBB.IsFollow)
            { actor.StopFollow(); return true; }
            if (actor.IsRide)
            { actor.DownRide(); return true; }
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
                if (CanControlAcTac(actor))
                { actor.Talk(npc.Id); state.TalkSent = true; state.Started = state.TalkAt = AcTacNow; state.TalkAttempts = 1; }
                return true;
            }
            if (!actor.TLBB.IsQuestOpen)
            {
                PublishAcTac(DiagnosticReason.DialogPending);
                // Opening a dialog can be retried; entry/quest selection must never be retried blindly.
                if (state.TalkAttempts < 3 && AcTacNow - state.TalkAt >= 5000 && CanControlAcTac(actor))
                { actor.Talk(npc.Id); state.TalkAt = AcTacNow; state.TalkAttempts++; }
                return true;
            }
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
            PublishAcTac(DiagnosticReason.OptionMissing);
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

        private void ObserveAcTacPosition(Game actor, AcTacActor state)
        {
            if (float.IsNaN(actor.CharX) || float.IsInfinity(actor.CharX)
                || float.IsNaN(actor.CharY) || float.IsInfinity(actor.CharY)) return;
            if (!state.PositionKnown || TINHKIEM.GetDistance(actor.CharX, actor.CharY, state.X, state.Y) >= 1f)
            {
                if (state.PositionKnown) acTacMovingCount++;
                state.X = actor.CharX; state.Y = actor.CharY;
                state.PositionKnown = true; state.ProgressAt = AcTacNow;
            }
        }

        private void RequestAcTacFollow(List<Game> participants, Game[] snapshot)
        {
            if (participants.Count < 2 || AcTacNow - acTacFollowAt < AcTacFollowInterval || !CanControlAcTac(this)) return;
            bool needed = false;
            // The native request affects the team. Do not send it across maps, dialogs or known opted-out windows.
            foreach (Game known in snapshot)
                if (known != null && known != this && known.TLBB != null && known.TLBB.KeyId == acTacOwner
                    && (!participants.Contains(known) || !CanControlAcTac(known))) return;
            foreach (Game actor in participants)
            {
                if (!CanControlAcTac(actor) || actor.TLBB.MapId != TLBB.MapId || actor.TLBB.IsQuestOpen) return;
                AcTacActor state = acTacActors[actor];
                if (state.TalkSent || state.Selected) return;
                if (actor != this && !actor.TLBB.IsFollow) needed = true;
            }
            if (needed && CanControlAcTac(this)) { acTacFollowAt = AcTacNow; AskTeamFollow(); }
        }

        private void PatrolAcTacOutdoor(Game actor, AcTacActor state)
        {
            if (!CanControlAcTac(actor)) return;
            int[,] route = AcTacOutdoorRoute();
            if (state.OutdoorIndex < 0)
            {
                float best = float.MaxValue;
                for (int i = 0; i < route.GetLength(0); i++)
                {
                    float distance = TINHKIEM.GetDistance(actor.CharX, actor.CharY, route[i,0], route[i,1]);
                    if (distance < best) { best = distance; state.OutdoorIndex = i; }
                }
                if (state.OutdoorIndex < 0) { StopAcTac("tọa độ tìm cửa không hợp lệ."); return; }
            }
            if (TINHKIEM.GetDistance(actor.CharX, actor.CharY, route[state.OutdoorIndex,0], route[state.OutdoorIndex,1]) <= 2f)
                state.OutdoorIndex = (state.OutdoorIndex + 1) % route.GetLength(0);
            if (actor.TLBB.IsFollow) return;
            // GoTo preserves the outdoor navigation/mount behavior; each actor searches even if the leader entered first.
            if (CanControlAcTac(actor)) actor.GoTo(route[state.OutdoorIndex,0], route[state.OutdoorIndex,1], MapAcTac);
        }

        private void RegroupAcTac(List<Game> participants)
        {
            foreach (Game actor in participants)
            {
                if (MapAcTac == 0) return;
                if (actor == this || !CanControlAcTac(actor) || actor.TLBB.MapId != TLBB.MapId) continue;
                if (actor.TLBB.IsFollow) continue;
                if (actor.IsRide) { actor.DownRide(); continue; }
                if (TINHKIEM.GetDistance(actor.CharX, actor.CharY, CharX, CharY) > 4f && CanControlAcTac(actor))
                    actor.Move(CharX, CharY); // Same-map fallback must not invoke GoTo's mount/travel branch in a dungeon.
            }
        }

        private bool HoldAcTac(bool combat, bool loot, int targets, double health)
        {
            if (!combat && !loot) { acTacHoldAt = -1; acTacHoldCount = -1; return false; }
            if (acTacHoldAt < 0 || acTacHoldCount != targets || health < acTacHoldHealth)
            { acTacHoldAt = AcTacNow; acTacHoldCount = targets; }
            acTacHoldHealth = health;
            acTacClearAt = acTacProgressAt = AcTacNow;
            acTacReanchor = true;
            if (acTacPhase == AcTacPhase.AwaitBoss) acTacPhaseAt = AcTacNow;
            DiagnosticReason reason = combat ? DiagnosticReason.CombatHold : DiagnosticReason.LootHold;
            PublishAcTac(reason);
            if (AcTacNow - acTacHoldAt >= AcTacTravelTimeout)
                StopAcTac("đánh/nhặt không có tiến triển trong 180 giây; hãy kiểm tra mục tiêu bằng tay.", DiagnosticReason.NoProgress);
            return true;
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
                    // The shared Party getter dereferences unrelated windows and cannot represent a temporarily missing member.
                    Game[] snapshot;
                    try { snapshot = FrmMain.dicGame.Values.ToArray(); }
                    catch (Exception error) when (error is InvalidOperationException || error is ArgumentException || error is IndexOutOfRangeException)
                    {
                        long raw = acTacClock.ElapsedMilliseconds;
                        if (acTacSnapshotAt < 0) acTacSnapshotAt = raw;
                        PauseAcTacClock(true); PublishAcTac(DiagnosticReason.SnapshotUnavailable);
                        if (raw - acTacSnapshotAt >= AcTacReadGrace)
                            StopAcTac("không đọc được danh sách cửa sổ trong 5 giây; hãy kiểm tra bằng tay.", DiagnosticReason.SnapshotUnavailable);
                        return;
                    }
                    acTacSnapshotAt = -1; PauseAcTacClock(false);
                    var participants = new List<Game> { this };
                    foreach (Game member in snapshot)
                        if (member != null && member != this && !participants.Contains(member) && member.IsAuto
                            && member.TLBB != null && member.TLBB.KeyId == acTacOwner && !member.TLBB.IsLeader) participants.Add(member);
                    foreach (Game tracked in acTacActors.Keys)
                    {
                        if (tracked == this || participants.Contains(tracked)) continue;
                        if (!tracked.IsAuto || !snapshot.Contains(tracked) || (tracked.TLBB != null
                            && !string.IsNullOrEmpty(tracked.TLBB.KeyId) && tracked.TLBB.KeyId != acTacOwner))
                        { StopAcTac("thành viên đã tắt Auto hoặc rời đội.", DiagnosticReason.TeamChanged); return; }
                        // A tracked actor with temporarily missing data remains part of the entry barrier.
                        participants.Add(tracked);
                    }
                    acTacParticipantCount = participants.Count;
                    acTacReadyCount = acTacInsideCount = acTacCombatCount = acTacMovingCount = 0;
                    bool ready = true, combat = false, loot = false, allInside = true, allExited = true;
                    int targets = 0;
                    double health = 0;
                    foreach (Game actor in participants)
                    {
                        AcTacActor state;
                        if (!acTacActors.TryGetValue(actor, out state))
                        {
                            if (acTacPhase != AcTacPhase.Entry && actor != this) { StopAcTac("đội thay đổi trong phụ bản."); return; }
                            state = new AcTacActor { CharacterId = actor.TLBB == null ? null : actor.TLBB.Id, Started = AcTacNow, ProgressAt = AcTacNow };
                            acTacActors.Add(actor, state);
                        }
                        if (actor.TLBB != null && !string.IsNullOrEmpty(actor.TLBB.Id))
                        {
                            if (string.IsNullOrEmpty(state.CharacterId)) state.CharacterId = actor.TLBB.Id;
                            else if (state.CharacterId != actor.TLBB.Id)
                            { StopAcTac("nhân vật trong đội đã thay đổi.", DiagnosticReason.TeamChanged); return; }
                        }
                        if (!CanControlAcTac(actor))
                        {
                            ready = allInside = allExited = false;
                            if (state.UnavailableAt < 0) state.UnavailableAt = AcTacNow;
                            if (AcTacTimeout(state.UnavailableAt, AcTacTravelTimeout, "thành viên sẵn sàng")) return;
                            continue;
                        }
                        if (actor.acTacCoordinator != null && actor.acTacCoordinator != this && actor.acTacCoordinator.mapAcTac != 0)
                        { StopAcTac("thành viên đang được đội khác điều phối.", DiagnosticReason.OtherTask); return; }
                        actor.acTacCoordinator = this;
                        acTacReadyCount++;
                        state.UnavailableAt = -1;
                        if (!ObserveAcTacMap(actor, state)) return;
                        ObserveAcTacPosition(actor, state);
                        allInside &= actor.TLBB.MapId == MAP.TacKhauDoanhDia;
                        allExited &= state.WasInside && actor.TLBB.MapId == MapAcTac;
                        if (actor.TLBB.MapId == MAP.TacKhauDoanhDia) acTacInsideCount++;
                        ObserveAcTacBoss(actor, state);
                        if (actor.TLBB.MapId == MAP.TacKhauDoanhDia && actor.Objects.NearMonter20m.Count > 0)
                        {
                            combat = true;
                            acTacCombatCount++;
                            foreach (GameObject target in actor.Objects.NearMonter20m)
                                if (target != null && target.HP > 0 && !float.IsNaN(target.HP) && !float.IsInfinity(target.HP))
                                { targets++; health += target.HP; }
                            if (actor.TLBB.IsFollow && CanControlAcTac(actor)) actor.StopFollow();
                            if (actor.IsRide && CanControlAcTac(actor)) actor.DownRide();
                        }
                        if (CanControlAcTac(actor) && actor.PickItem()) loot = true;
                    }
                    if (MapAcTac == 0) return;
                    if (acTacPhase == AcTacPhase.AwaitExit)
                    {
                        PublishAcTac(DiagnosticReason.ExitPending);
                        if (ready && allExited) { ResetAcTacSession(); PublishAcTac(DiagnosticReason.CompletedMapReturn); return; }
                        AcTacTimeout(acTacPhaseAt, AcTacDialogTimeout, "map sau khi boss chết; chưa có protocol cửa ra");
                        return;
                    }
                    if (acTacPhase == AcTacPhase.Entry && ready && allInside)
                    { acTacPhase = AcTacPhase.Patrol; acTacClearAt = acTacProgressAt = AcTacNow; }
                    if (acTacPhase == AcTacPhase.Entry)
                    {
                        PublishAcTac(ready ? DiagnosticReason.EntrancePending : DiagnosticReason.CharacterUnavailable);
                        // Combat of an already-entered actor must not starve entry commands for the others.
                        if (!combat && !loot) RequestAcTacFollow(participants, snapshot);
                        foreach (Game actor in participants)
                        {
                            if (MapAcTac == 0) return;
                            if (!CanControlAcTac(actor) || actor.TLBB.MapId == MAP.TacKhauDoanhDia) continue;
                            AcTacActor state = acTacActors[actor];
                            long started = actor.TLBB.MapId != MapAcTac ? state.ProgressAt : state.Started;
                            if (AcTacTimeout(started, state.Selected || state.TalkSent || state.ClosedStale
                                ? AcTacDialogTimeout : AcTacTravelTimeout, "vào map phụ bản")) return;
                            if (actor.TLBB.MapId != MapAcTac)
                            {
                                int[,] route = AcTacOutdoorRoute();
                                if (actor.TLBB.IsFollow && actor.TLBB.MapId == TLBB.MapId) continue;
                                if (CanControlAcTac(actor)) actor.GoTo(route[0,0], route[0,1], MapAcTac);
                            }
                            else if (!EnterAcTac(actor, state))
                                PatrolAcTacOutdoor(actor, state);
                        }
                        return;
                    }
                    if (HoldAcTac(combat, loot, targets, health)) return;
                    PublishAcTac(ready ? DiagnosticReason.None : DiagnosticReason.CharacterUnavailable);
                    if (!ready) { RegroupAcTac(participants); return; }
                    if (acTacPhase == AcTacPhase.AwaitBoss)
                    { PublishAcTac(DiagnosticReason.BossPending); AcTacTimeout(acTacPhaseAt, AcTacDialogTimeout, "boss sau khi hết tuyến; chưa xác nhận hoàn tất"); return; }
                    RequestAcTacFollow(participants, snapshot);
                    if (MapAcTac == 0) return;
                    PatrolAcTac(AcTacDungeonRoute, true);
                    if (MapAcTac != 0) RegroupAcTac(participants);
                }
                catch (Exception error) { StopAcTac("lỗi đọc/xử lý " + error.GetType().Name + ".", DiagnosticReason.ReadFailure); }
            }
        }
    }
}
