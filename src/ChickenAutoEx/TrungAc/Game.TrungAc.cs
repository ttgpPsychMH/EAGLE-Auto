using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TinhKiemAuto
{
    public partial class Game
    {
        private readonly object trungAcLock = new object();
        private readonly Stopwatch trungAcClock = Stopwatch.StartNew();
        private volatile bool trungAcEnabled;
        private string trungAcOwner, trungAcLoginOwner;
        private bool trungAcLoginAttempted, trungAcLoginSuppressed;
        private int trungAcLastOnlineSeconds, trungAcQuestId, trungAcNpcId, trungAcTargetId = -1;
        private bool trungAcSent, trungAcDelivered, trungAcArrived;
        private int trungAcAbsentReads;
        private long trungAcStarted, trungAcPausedAt = -1, trungAcPauseOffset;
        private float trungAcBestDistance = float.MaxValue;
        private float trungAcTargetHP = float.MaxValue;
        private readonly HashSet<int> trungAcBeforeSpawn = new HashSet<int>();
        private TrungAcPhase trungAcPhase;
        private const string TrungAcQuestName = "#{CXDT_090304_01}";
        private const long TrungAcDialogTimeout = 60000, TrungAcTravelTimeout = 180000;
        private enum TrungAcPhase { Scan, Info, TravelTarget, Spawn, Combat, AcceptNpc, ReturnNpc,
            AcceptDialog, ReturnDialog, AcceptAck, ReturnContinue, ReturnComplete, ReturnAck }

        private long TrungAcNow { get { return trungAcClock.ElapsedMilliseconds - trungAcPauseOffset; } }

        private void ResetTrungAcSession()
        {
            trungAcOwner = null;
            trungAcPhase = TrungAcPhase.Scan;
            trungAcQuestId = trungAcNpcId = 0;
            trungAcTargetId = -1;
            trungAcSent = trungAcDelivered = false;
            trungAcArrived = false;
            trungAcAbsentReads = 0;
            trungAcPausedAt = -1;
            trungAcPauseOffset = 0;
            trungAcStarted = TrungAcNow;
            trungAcBestDistance = float.MaxValue;
            trungAcTargetHP = float.MaxValue;
            trungAcBeforeSpawn.Clear();
            State = STATE.None;
            MissionMap = -1;
            MissionX = MissionY = 0;
            TrungAcInfo = string.Empty;
            Extra1 = 0;
            comeTime = doneTime = HongTrungAcTime = null;
            IsHong = IsXongTrungAc = false;
        }

        private void StopTrungAc(string reason)
        {
            IsTrungAc = false;
            if (reason != null)
                CanhBao.Msg("Trừng Ác", "Đã dừng Trừng Ác: " + reason, CanhBao.Kieu.Eror);
        }

        // Called by Auto before its scene/leader dispatcher. UI alarms never send task commands.
        private void CheckTrungAcSession()
        {
            if (!IsTrungAc) return;
            lock (trungAcLock)
            {
                try
                {
                    if (!IsAuto || !IsInit || TLBB == null || !TLBB.Online || string.IsNullOrEmpty(TLBB.Id))
                    { StopTrungAc(null); return; }
                    if (trungAcOwner == null) trungAcOwner = TLBB.Id;
                    if (trungAcOwner != TLBB.Id) { StopTrungAc("nhân vật đã thay đổi."); return; }
                    if (IsThuyLao || IsKyCuoc || IsAcBa || IsLauLanTamBao || MapAcTac != 0 || MapTKC != 0
                        || IsQ123LauLan || IsQ123ToChau || IsYenTuO || IsPhungHoangLangMo || IsPMP
                        || IsTuBaoBon || IsLuyenKim || IsBachHoaDuyen || IsSuMon || IsXayDung || IsTuDuong || IsNhiemVuCoBan)
                    { StopTrungAc("đang có nhiệm vụ khác; hãy dừng nhiệm vụ đó trước."); return; }
                    if (TLBB.PlayerState == 2 || TLBB.PlayerState == 9 || TLBB.MapId == MAP.GiamNguc)
                    { StopTrungAc("nhân vật chết hoặc đang ở Giám Ngục."); return; }
                    bool pause = Global.Paused || ON_SCENE_TRANSING || IsChangeMap
                        || (TLBB.PlayerState == 7 && trungAcPhase != TrungAcPhase.Combat && trungAcPhase != TrungAcPhase.Spawn);
                    long raw = trungAcClock.ElapsedMilliseconds;
                    if (pause && trungAcPausedAt < 0) trungAcPausedAt = raw;
                    if (!pause && trungAcPausedAt >= 0)
                    { trungAcPauseOffset += raw - trungAcPausedAt; trungAcPausedAt = -1; }
                }
                catch (Exception error) { StopTrungAc("lỗi đọc/xử lý " + error.GetType().Name + "."); }
            }
        }

        private bool CanActTrungAc()
        {
            return IsTrungAc && IsAuto && IsInit && TLBB != null && TLBB.Online && TLBB.Id == trungAcOwner
                && TLBB.PlayerState == 0 && !Global.Paused && !ON_SCENE_TRANSING && !IsChangeMap;
        }

        public void StartTrungAcFromLogin()
        {
            lock (trungAcLock)
            {
                if (TLBB == null || !TLBB.Online) return;
                if (trungAcLoginOwner != TLBB.Id || (trungAcLastOnlineSeconds >= 60 && TLBB.OnlineTimeSec < 60))
                {
                    trungAcLoginOwner = TLBB.Id;
                    trungAcLoginAttempted = trungAcLoginSuppressed = false;
                }
                trungAcLastOnlineSeconds = TLBB.OnlineTimeSec;
                if (TLBB.OnlineTimeSec >= 60 || !IsAuto || !IsInit || trungAcLoginAttempted || trungAcLoginSuppressed) return;
                trungAcLoginAttempted = true;
                IsTrungAc = true;
                CheckTrungAcSession();
                if (IsTrungAc) IsBTDByLogin = true;
            }
        }

        private void SetTrungAcPhase(TrungAcPhase phase)
        {
            trungAcPhase = phase;
            trungAcStarted = TrungAcNow;
            trungAcSent = false;
            trungAcArrived = false;
            trungAcAbsentReads = 0;
            trungAcBestDistance = float.MaxValue;
        }

        private bool WaitTrungAc(long timeout, string step)
        {
            if (TrungAcNow - trungAcStarted < timeout) return true;
            StopTrungAc("hết thời gian chờ " + step + "; quest được giữ để kiểm tra bằng tay.");
            return false;
        }

        private Task ReadTrungAcTask()
        {
            Task found = null;
            foreach (Task task in Task.Enum(this))
                if (task != null && task.Name != null && task.Name.Contains(TrungAcQuestName))
                {
                    if (found != null && found.Id != task.Id) throw new InvalidOperationException();
                    found = task;
                }
            return found;
        }

        private PacketItem ReadTrungAcItem()
        {
            foreach (PacketItem item in PacketItem.Enum(this))
                if (item != null && item.Name != null && item.Count > 0 && TINHKIEM.VietLien(item.Name).Contains("trungaclenh"))
                    return item;
            return null;
        }

        private bool TravelTrungAc(float x, float y, int map)
        {
            if (!CanActTrungAc()) return false;
            if (TLBB.MapId != map)
            {
                if (WaitTrungAc(TrungAcTravelTimeout, "đổi map")) TimDuong(x, y, map);
                return false;
            }
            float distance = TINHKIEM.GetDistance(CharX, CharY, x, y);
            if (float.IsNaN(distance) || float.IsInfinity(distance)) { StopTrungAc("tọa độ nhân vật không hợp lệ."); return false; }
            if (distance <= 3f)
            {
                if (!trungAcArrived) { trungAcArrived = true; trungAcStarted = TrungAcNow; }
                return true;
            }
            trungAcArrived = false;
            if (distance < trungAcBestDistance - 1f)
            { trungAcBestDistance = distance; trungAcStarted = TrungAcNow; }
            if (WaitTrungAc(TrungAcTravelTimeout, "tiến triển di chuyển") && CanActTrungAc()) Move(x, y);
            return false;
        }

        private GameObject ReadTrungAcNpc()
        {
            foreach (GameObject obj in Objects.AllNpc)
                if (obj != null && obj.IsNPC && obj.Name != null && TINHKIEM.VietLien(obj.Name) == "ngogioi"
                    && TINHKIEM.GetDistance(obj.X, obj.Y, 224f, 226f) <= 5f) return obj;
            return null;
        }

        private bool AtTrungAcNpc()
        {
            if (!CanActTrungAc() || TLBB.MapId != 1 || TINHKIEM.GetDistance(CharX, CharY, 224f, 226f) > 3f) return false;
            GameObject npc = ReadTrungAcNpc();
            return npc != null && npc.Id == trungAcNpcId;
        }

        private bool ValidTrungAcTarget(GameObject obj)
        {
            return obj != null && !obj.IsNPC && obj.Menpai == 28 && obj.HP > 0f && !float.IsInfinity(obj.HP)
                && !float.IsNaN(obj.X) && !float.IsNaN(obj.Y) && !float.IsInfinity(obj.X) && !float.IsInfinity(obj.Y)
                && Math.Abs((long)obj.Lvl - TLBB.Lvl) <= 5 && !trungAcBeforeSpawn.Contains(obj.Id)
                && TINHKIEM.GetDistance(obj.X, obj.Y, MissionX, MissionY) <= 20f
                && TINHKIEM.GetDistance(obj.X, obj.Y, CharX, CharY) < 20f
                && (TLBB.MapId != MAP.ThaoNguyen || !string.IsNullOrEmpty(obj.Title));
        }

        public void TrungAc()
        {
            if (!IsTrungAc) return;
            lock (trungAcLock)
            {
                try
                {
                    CheckTrungAcSession();
                    if (!CanActTrungAc() || TickCount % 12 != 0) return;
                    if (TLBB.IsFollow) { StopTrungAc("hãy dừng theo đội trước khi làm quest cá nhân."); return; }
                    Task task = ReadTrungAcTask();
                    if (task != null && trungAcQuestId != 0 && task.Id != trungAcQuestId)
                    { StopTrungAc("quest hiện tại đã thay đổi."); return; }
                    if (task != null) trungAcQuestId = task.Id;
                    bool completed = task != null && (task.Completed || task.Complete >= 256);
                    if (completed && (trungAcPhase == TrungAcPhase.Scan || trungAcPhase == TrungAcPhase.Info
                        || trungAcPhase == TrungAcPhase.TravelTarget || trungAcPhase == TrungAcPhase.Spawn || trungAcPhase == TrungAcPhase.Combat))
                        SetTrungAcPhase(TrungAcPhase.ReturnNpc);
                    PacketItem item = ReadTrungAcItem();
                    if (trungAcPhase == TrungAcPhase.Scan)
                    {
                        if (item != null)
                        {
                            if (!CanActTrungAc()) return;
                            if (TLBB.IsQuestOpen)
                            {
                                if (!WaitTrungAc(TrungAcDialogTimeout, "đóng dialog cũ")) return;
                                if (!trungAcSent) { CloseQuest(); trungAcSent = true; }
                                return;
                            }
                            PlayerPackageUseItem(item.Index);
                            SetTrungAcPhase(TrungAcPhase.Info);
                        }
                        else if (task == null) SetTrungAcPhase(TrungAcPhase.AcceptNpc);
                        else StopTrungAc("quest còn active nhưng thiếu lệnh/thông tin mục tiêu; cần kiểm tra bằng tay.");
                        return;
                    }
                    if (trungAcPhase == TrungAcPhase.Info)
                    {
                        if (!WaitTrungAc(TrungAcDialogTimeout, "thông tin lệnh")) return;
                        if (!TLBB.IsQuestOpen) return;
                        string info = QuestFrame.All(this);
                        int map, x, y;
                        if (!TryParseTrungAcDestination(info, out map, out x, out y)) return;
                        MissionMap = map; MissionX = x; MissionY = y; TrungAcInfo = info;
                        if (CanActTrungAc()) CloseQuest();
                        SetTrungAcPhase(TrungAcPhase.TravelTarget);
                        return;
                    }
                    if (trungAcPhase == TrungAcPhase.TravelTarget)
                    {
                        if (!TravelTrungAc(MissionX, MissionY, MissionMap)) return;
                        if (task == null || item == null)
                        { if (WaitTrungAc(TrungAcDialogTimeout, "quest/lệnh trước triệu hồi")) return; return; }
                        trungAcBeforeSpawn.Clear();
                        foreach (GameObject obj in Objects.Near20m) if (obj != null) trungAcBeforeSpawn.Add(obj.Id);
                        if (!CanActTrungAc()) return;
                        PlayerPackageUseItem(item.Index);
                        SetTrungAcPhase(TrungAcPhase.Spawn);
                        return;
                    }
                    if (trungAcPhase == TrungAcPhase.Spawn || trungAcPhase == TrungAcPhase.Combat)
                    {
                        if (TLBB.MapId != MissionMap) { StopTrungAc("rời map mục tiêu trước khi quest hoàn tất."); return; }
                        GameObject target = null;
                        float nearest = float.MaxValue;
                        foreach (GameObject obj in Objects.Near20m)
                        {
                            if (!ValidTrungAcTarget(obj)) continue;
                            if (obj.Id == trungAcTargetId) { target = obj; break; }
                            float distance = TINHKIEM.GetDistance(obj.X, obj.Y, CharX, CharY);
                            if (distance < nearest) { nearest = distance; target = obj; }
                        }
                        if (target == null)
                        { WaitTrungAc(TrungAcTravelTimeout, "quái hoặc quest hoàn tất"); return; }
                        if (trungAcTargetId != target.Id || target.HP < trungAcTargetHP - 0.01f)
                        { trungAcStarted = TrungAcNow; trungAcTargetHP = target.HP; }
                        if (!WaitTrungAc(TrungAcTravelTimeout, "tiến triển combat")) return;
                        trungAcTargetId = target.Id;
                        trungAcPhase = TrungAcPhase.Combat;
                        if (!CanActTrungAc()) return;
                        if (IsRide) { DownRide(); return; }
                        SelectTarget(target.Id);
                        if (CanActTrungAc()) SendKey(Global.BaseSkill);
                        return;
                    }
                    if (trungAcPhase == TrungAcPhase.AcceptNpc || trungAcPhase == TrungAcPhase.ReturnNpc)
                    {
                        bool returning = trungAcPhase == TrungAcPhase.ReturnNpc;
                        if (!TravelTrungAc(224f, 226f, 1)) return;
                        if (!WaitTrungAc(TrungAcDialogTimeout, "Ngô Giới")) return;
                        GameObject npc = ReadTrungAcNpc();
                        if (npc == null) return;
                        if (TLBB.IsQuestOpen)
                        { if (!trungAcSent && CanActTrungAc()) { CloseQuest(); trungAcSent = true; } return; }
                        trungAcNpcId = npc.Id;
                        if (!CanActTrungAc()) return;
                        Talk(npc.Id);
                        SetTrungAcPhase(returning ? TrungAcPhase.ReturnDialog : TrungAcPhase.AcceptDialog);
                        return;
                    }
                    if (!AtTrungAcNpc()) { StopTrungAc("context NPC nhận/trả quest đã thay đổi."); return; }
                    if (!WaitTrungAc(TrungAcDialogTimeout, "dialog/xác nhận quest")) return;
                    if (trungAcPhase == TrungAcPhase.AcceptDialog || trungAcPhase == TrungAcPhase.ReturnDialog)
                    {
                        if (!TLBB.IsQuestOpen) return;
                        var frames = QuestFrame.Enum(this);
                        string dialog = string.Empty;
                        foreach (QuestFrame frame in frames) if (frame != null) dialog += frame.Name;
                        bool limit = dialog.Contains("#{CXDY_090423_01}") && dialog.Contains("#{CXDY_090423_02}");
                        if (limit && task == null && item == null)
                        {
                            if (++trungAcAbsentReads < 2) return;
                            bool delivered = trungAcDelivered;
                            StopTrungAc(delivered ? null : "dialog báo giới hạn lượt theo token legacy; cần xác minh bằng tay.");
                            IsXongTrungAc = delivered;
                            return;
                        }
                        trungAcAbsentReads = 0;
                        foreach (QuestFrame frame in frames)
                            if (frame != null && frame.Name == TrungAcQuestName && frame.StrOptionExtra1 > 0)
                            {
                                if (!CanActTrungAc()) return;
                                QuestFrameOptionClicked(frame);
                                SetTrungAcPhase(trungAcPhase == TrungAcPhase.ReturnDialog ? TrungAcPhase.ReturnContinue : TrungAcPhase.AcceptAck);
                                return;
                            }
                        return;
                    }
                    if (trungAcPhase == TrungAcPhase.AcceptAck)
                    {
                        if (task == null || item == null) return;
                        if (CanActTrungAc()) CloseQuest();
                        SetTrungAcPhase(TrungAcPhase.Scan);
                        return;
                    }
                    if (trungAcPhase == TrungAcPhase.ReturnContinue || trungAcPhase == TrungAcPhase.ReturnComplete)
                    {
                        if (!completed || !TLBB.IsQuestOpen || !QuestFrame.All(this).Contains(TrungAcQuestName)) return;
                        if (!CanActTrungAc()) return;
                        if (trungAcPhase == TrungAcPhase.ReturnContinue)
                        { PostMessage(14, 105); SetTrungAcPhase(TrungAcPhase.ReturnComplete); }
                        else { QuestFrameMissionComplete(); SetTrungAcPhase(TrungAcPhase.ReturnAck); }
                        return;
                    }
                    if (trungAcPhase == TrungAcPhase.ReturnAck)
                    {
                        if (task != null) { trungAcAbsentReads = 0; return; }
                        if (++trungAcAbsentReads < 2) return;
                        trungAcDelivered = true;
                        trungAcQuestId = 0;
                        trungAcTargetId = -1;
                        if (CanActTrungAc()) CloseQuest();
                        SetTrungAcPhase(TrungAcPhase.Scan);
                        return;
                    }
                    StopTrungAc("trạng thái không được hỗ trợ.");
                }
                catch (Exception error) { StopTrungAc("lỗi đọc/xử lý " + error.GetType().Name + "."); }
            }
        }
    }
}
