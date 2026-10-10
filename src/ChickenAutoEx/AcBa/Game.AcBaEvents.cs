using System;
using TinhKiemAuto.Models;

namespace TinhKiemAuto
{
    public partial class Game
    {
        private bool isAcBa;
        private int acBaNoticeSchool = -1, acBaManualSchool = -1;
        private string acBaNoticeCharacter;
        private long acBaNoticeAt, acBaWaitAt;
        private const long AcBaNoticeFreshness = 600000; // Freshness budget, not the server event duration.
        private Game acBaHandoff;
        private string acBaHandoffId;
        private long acBaHandoffAt;
        public bool IsAcBa
        {
            get { return isAcBa; }
            set
            {
                lock (acBaLock)
                {
                    if (isAcBa == value) return;
                    isAcBa = value;
                    ResetAcBaSession();
                    acBaWaitAt = AcBaNow;
                    acBaHandoff = null;
                    if (!value) ClearAcBaNotice();
                }
            }
        }
        private void ClearAcBaNotice()
        {
            AcBa = acBaNoticeSchool = acBaManualSchool = -1;
            acBaNoticeCharacter = null;
            IsAlarmAcBa = false;
        }
        private void CheckAcBaNoticeIdentity()
        {
            if (!IsInit || TLBB == null || !TLBB.Online || (acBaNoticeCharacter != null && TLBB.Id != acBaNoticeCharacter)) ClearAcBaNotice();
            if (acBaNoticeSchool >= 0 && acBaClock.ElapsedMilliseconds - acBaNoticeAt > AcBaNoticeFreshness)
            { AcBa = acBaNoticeSchool = -1; IsAlarmAcBa = false; }
        }
        public void ReceiveAcBaNotice(string text)
        {
            lock (acBaLock)
            {
                if (TLBB == null || !IsInit || !TLBB.Online || string.IsNullOrEmpty(TLBB.Id)) return;
                CheckAcBaNoticeIdentity();
                int school; bool ended;
                if (!AcBaEvents.TryParse(text, out school, out ended)) return;
                if (ended)
                {
                    if (school == acBaNoticeSchool) { AcBa = acBaNoticeSchool = -1; IsAlarmAcBa = false; }
                    if (IsAcBa && (school == acBaSchool || school == acBaManualSchool)) StopAcBa("thông báo sự kiện đã kết thúc; chưa xác nhận kết quả.");
                    return;
                }
                // Duplicate broadcasts do not refresh the freshness budget or reset an active run.
                if (acBaNoticeSchool == school) return;
                acBaNoticeSchool = AcBa = school;
                acBaNoticeCharacter = TLBB.Id;
                acBaNoticeAt = acBaClock.ElapsedMilliseconds;
                IsAlarmAcBa = false;
            }
        }
        public void SelectManualAcBaSchool(int school)
        {
            if (AcBaEvents.DungeonMap(school) < 0) throw new ArgumentOutOfRangeException("school");
            lock (acBaLock)
            {
                IsAcBa = false;
                IsAcBa = true;
                acBaManualSchool = school;
                acBaNoticeCharacter = TLBB == null ? null : TLBB.Id;
            }
        }
        public string AcBaStatus
        {
            get
            {
                lock (acBaLock)
                {
                    if (acBaHandoff != null) return "Ác Bá: chờ xác nhận chuyển đội trưởng";
                    if (acBaSchool < 0) return "Ác Bá: chờ thông báo hệ thống / chọn môn phái";
                    if (TLBB != null && TLBB.Menpai != acBaSchool)
                        return "Ác Bá: " + AcBaEvents.SchoolName(acBaSchool) + " — chờ thành viên đúng phái làm đội trưởng";
                    string phase = acBaPhase == AcBaPhase.Entry ? "đến cửa vào" : acBaPhase == AcBaPhase.Patrol ? "tuần tra"
                        : acBaPhase == AcBaPhase.AwaitBoss ? "chờ boss" : "chờ map ra";
                    return "Ác Bá: " + AcBaEvents.SchoolName(acBaSchool) + " — " + phase;
                }
            }
        }
        private void ReleaseAcBaNoticeHook()
        {
            if (!IsHooked || Option.AlarmChat || AlarmChat || FrmMain.AlarmAcBa) return;
            try { UnHookRecv(); }
            catch (Exception) { } // Native cleanup is also retried by the existing close/dispose path.
        }
        private bool RequiresAcBaNotices { get { return IsAcBa && acBaManualSchool < 0; } }
        private bool AcBaControlsCurrentActor()
        {
            if (IsAcBa) return true;
            if (!IsAuto || TLBB == null || !TLBB.Online) return false;
            try
            {
                foreach (Game actor in Party.ToArray())
                    if (actor != null && actor != this && actor.IsAcBa && actor.IsAuto && actor.TLBB != null
                        && actor.TLBB.Online && actor.TLBB.IsLeader && actor.TLBB.Id == TLBB.KeyId) return true;
            }
            catch (Exception) { return true; } // Unknown team snapshot must not enable legacy patrol.
            return false;
        }
        private bool SelectAcBaTarget()
        {
            CheckAcBaNoticeIdentity();
            if (acBaSchool >= 0) return true; // Target stays fixed once a run starts.
            int target = acBaManualSchool >= 0 ? acBaManualSchool : acBaNoticeSchool;
            if (target < 0) { AcBaTimeout(acBaWaitAt, AcBaNoticeFreshness, "thông báo Ác Bá; có thể chọn môn phái thủ công"); return false; }
            if (AcBaEvents.DungeonMap(target) < 0) { StopAcBa("môn phái không được hỗ trợ."); return false; }
            acBaSchool = target;
            acBaDungeonRoute = AcBaEvents.ReadRoute(Setting.LoadMAP(AcBaDungeonMap.ToString()), target);
            if (acBaDungeonRoute == null || acBaDungeonRoute.GetLength(0) < 2)
            { StopAcBa("thiếu tuyến phụ bản."); return false; }
            acBaVisited = new bool[acBaDungeonRoute.GetLength(0)];
            acBaProgressAt = acBaPhaseAt = AcBaNow;
            return true;
        }
        private bool EligibleAcBaHandoff(Game member, bool acknowledged)
        {
            return member != null && member != this && member.IsAuto && member.IsInit && member.TLBB != null
                && member.TLBB.Online && member.TLBB.OnlineTimeSec > 3 && member.TLBB.Menpai == acBaSchool
                && member.TLBB.PlayerState == 0 && !member.ON_SCENE_TRANSING && !member.IsChangeMap
                && !member.IsAcBa && !AcBaConflict(member) && !member.IsMapPhuBan()
                && !string.IsNullOrEmpty(member.TLBB.Id) && !string.IsNullOrEmpty(member.TLBB.Name)
                && (acknowledged ? member.TLBB.IsLeader && member.TLBB.KeyId == member.TLBB.Id
                                 : !member.TLBB.IsLeader && member.TLBB.KeyId == acBaOwner);
        }
        private void TryAcBaHandoff()
        {
            if (acBaHandoff != null) return;
            if (AcBaTimeout(acBaPhaseAt, AcBaTravelTimeout, "thành viên đúng môn phái sẵn sàng làm đội trưởng")) return;
            foreach (Game member in Party.ToArray())
            {
                if (!CanControlAcBa(this) || !EligibleAcBaHandoff(member, false)) continue;
                acBaHandoff = member; acBaHandoffId = member.TLBB.Id; acBaHandoffAt = AcBaNow;
                AppointLeader(member.TLBB.Name); // Once; completion requires server state on both actors.
                return;
            }
        }
        private bool TryAcceptAcBaHandoff(int school, string expectedId)
        {
            lock (acBaLock)
            {
                if (IsAcBa || !IsAuto || !IsInit || TLBB == null || !TLBB.Online || TLBB.Id != expectedId
                    || !TLBB.IsLeader || TLBB.KeyId != expectedId || TLBB.Menpai != school || TLBB.PlayerState != 0
                    || Global.Paused || ON_SCENE_TRANSING || IsChangeMap || AcBaConflict(this) || IsMapPhuBan()) return false;
                SelectManualAcBaSchool(school);
                return true;
            }
        }
        private bool CheckAcBaHandoff()
        {
            if (acBaHandoff == null) return false;
            if (Global.Paused || ON_SCENE_TRANSING || IsChangeMap || TLBB.PlayerState == 7) return true;
            if (acBaHandoff.TLBB == null || acBaHandoff.TLBB.Id != acBaHandoffId || !acBaHandoff.IsAuto
                || !acBaHandoff.IsInit || !acBaHandoff.TLBB.Online || AcBaConflict(acBaHandoff)
                || !Party.Contains(acBaHandoff)) { StopAcBa("thành viên nhận đội trưởng không còn sẵn sàng."); return true; }
            if (!TLBB.IsLeader)
            {
                if (TLBB.KeyId != acBaHandoffId || !EligibleAcBaHandoff(acBaHandoff, true))
                { StopAcBa("đội trưởng thay đổi ngoài yêu cầu Ác Bá."); return true; }
                Game next = acBaHandoff; int school = acBaSchool; string nextId = acBaHandoffId;
                IsAcBa = false;
                next.TryAcceptAcBaHandoff(school, nextId); // Explicit team transfer, not a global cache.
                return true;
            }
            AcBaTimeout(acBaHandoffAt, AcBaDialogTimeout, "xác nhận đội trưởng từ server");
            return true;
        }
    }
}
