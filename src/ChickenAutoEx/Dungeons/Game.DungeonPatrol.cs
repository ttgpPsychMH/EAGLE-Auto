using System;
using System.Collections.Generic;
using System.Linq;
using TinhKiemAuto.Diagnostics;

namespace TinhKiemAuto
{
    public partial class Game
    {
        private enum DungeonTask { KyCuoc, AcTac, AcBa, ThuyLao }
        private enum TeamPatrolPhase { Waiting, Mounting, Following, Travel, Unfollowing, Dismounting, Stopping, Combat, Loot, Stopped }
        private enum TeamCommand { Mount, Unfollow, Dismount, Stop, Regroup }
        private sealed class TeamActor
        {
            internal readonly Dictionary<TeamCommand, long> Last = new Dictionary<TeamCommand, long>();
        }
        private readonly object dungeonPatrolLock = new object();
        private readonly Dictionary<Game, TeamActor> dungeonActors = new Dictionary<Game, TeamActor>();
        private DungeonTask dungeonTask;
        private volatile bool dungeonTaskBound;
        private bool dungeonWasCombat, dungeonFollowConfirmed;
        private long dungeonFollowAt = -5000, dungeonPreparationAt = -1;
        private TeamPatrolPhase dungeonPreparation;
        private volatile Game dungeonPatrolOwner;
        private volatile int[] dungeonPatrolDiagnostic = new[] { -1,0,0,0,0,0,0,0,0 };

        private bool DungeonTaskActive(DungeonTask task)
        {
            switch (task)
            {
                case DungeonTask.KyCuoc: return IsKyCuoc;
                case DungeonTask.AcTac: return MapAcTac != 0;
                case DungeonTask.AcBa: return IsAcBa;
                default: return IsThuyLao;
            }
        }
        private bool DungeonControlsCurrentActor()
        {
            Game owner = dungeonPatrolOwner;
            return IsKyCuoc || MapAcTac != 0 || IsAcBa || IsThuyLao
                || (IsAuto && owner != null && owner.dungeonTaskBound && owner.DungeonTaskActive(owner.dungeonTask)
                    && TLBB != null && owner.TLBB != null && TLBB.KeyId == owner.TLBB.Id);
        }
        private bool DungeonCombatReady()
        {
            Game owner = dungeonPatrolOwner;
            if (!DungeonControlsCurrentActor() || owner == null || !owner.dungeonTaskBound
                || !owner.DungeonTaskActive(owner.dungeonTask) || !IsAuto || TLBB == null
                || IsRide || TLBB.IsFollow || DungeonMovementPolicy.IsMoving(TLBB.PlayerState)) return false;
            int[] state = owner.dungeonPatrolDiagnostic;
            return state[2] != 0 && state[1] == (int)TeamPatrolPhase.Combat;
        }
        private Game[] DungeonWindowSnapshot()
        {
            return FrmMain.dicGame.Values.ToArray();
        }
        private void ResetDungeonPatrol(DungeonTask task)
        {
            lock (dungeonPatrolLock)
            {
                if (!dungeonTaskBound || dungeonTask != task) return;
                foreach (Game actor in dungeonActors.Keys)
                    if (actor.dungeonPatrolOwner == this) actor.dungeonPatrolOwner = null;
                dungeonActors.Clear(); dungeonTaskBound = dungeonWasCombat = dungeonFollowConfirmed = false;
                dungeonFollowAt = -5000; dungeonPreparationAt = -1;
                int[] last = (int[])dungeonPatrolDiagnostic.Clone();
                last[1] = (int)TeamPatrolPhase.Stopped; last[2] = 0; last[8] = (int)DiagnosticReason.SessionCleared;
                dungeonPatrolDiagnostic = last;
            }
        }
        private int[] CaptureDungeonStop(DungeonTask task, string reason)
        {
            int[] last = dungeonPatrolDiagnostic[0] == (int)task ? (int[])dungeonPatrolDiagnostic.Clone() : new int[9];
            last[0] = (int)task; last[1] = (int)TeamPatrolPhase.Stopped; last[2] = 0;
            last[8] = (int)(reason == null ? DiagnosticReason.CharacterUnavailable : DiagnosticSchema.Reason(reason));
            return last;
        }
        private void BindDungeonParty(DungeonTask task, IList<Game> participants, Func<Game, bool> eligible)
        {
            lock (dungeonPatrolLock)
            {
                if (dungeonTaskBound && dungeonTask != task) ResetDungeonPatrol(dungeonTask);
                dungeonTask = task; dungeonTaskBound = true;
                foreach (Game absent in dungeonActors.Keys.Where(actor => !participants.Contains(actor)).ToArray())
                { if (absent.dungeonPatrolOwner == this) absent.dungeonPatrolOwner = null; dungeonActors.Remove(absent); }
                foreach (Game actor in participants)
                {
                    if (actor == null || !eligible(actor)) continue;
                    Game previous = actor.dungeonPatrolOwner;
                    if (previous != null && previous != this && previous.dungeonTaskBound && previous.DungeonTaskActive(previous.dungeonTask))
                        throw new InvalidOperationException("Dungeon ownership conflict.");
                    actor.dungeonPatrolOwner = this;
                    if (!dungeonActors.ContainsKey(actor)) dungeonActors.Add(actor, new TeamActor());
                }
            }
        }
        private void PublishTeamPatrol(TeamPatrolPhase phase, IList<Game> participants, Func<Game, bool> eligible, DiagnosticReason reason)
        {
            int ready = 0, follow = 0, ride = 0, moving = 0;
            foreach (Game actor in participants)
            {
                if (actor == null || !eligible(actor)) continue;
                ready++;
                if (actor != this && actor.TLBB.IsFollow) follow++;
                if (actor.IsRide) ride++;
                if (DungeonMovementPolicy.IsMoving(actor.TLBB.PlayerState)) moving++;
            }
            dungeonPatrolDiagnostic = new[] { (int)dungeonTask, (int)phase, DungeonTaskActive(dungeonTask) ? 1 : 0,
                participants.Count, ready, follow, ride, moving, (int)reason };
        }
        private bool IssueTeamCommand(Game actor, TeamCommand command, long now, Func<Game, bool> eligible, Action action)
        {
            if (!dungeonTaskBound || !DungeonTaskActive(dungeonTask) || !eligible(this) || !eligible(actor)) return false;
            lock (dungeonPatrolLock)
            {
                TeamActor state;
                if (!dungeonActors.TryGetValue(actor, out state)) return false;
                long last;
                if (state.Last.TryGetValue(command, out last) && now - last < 2000) return false;
                state.Last[command] = now;
            }
            action(); return true; // Queueing is not native ACK; the next observation decides readiness.
        }
        private void StopDungeonTravel(Game actor, Func<Game, bool> eligible)
        {
            // Reuse the existing Move coordinate/opcode contract to target the observed current position.
            // Game.Move short-circuits near the target, so it cannot itself cancel an already queued route.
            float x = actor.CharX, y = actor.CharY;
            if (float.IsNaN(x) || float.IsNaN(y) || float.IsInfinity(x) || float.IsInfinity(y)
                || x <= int.MinValue || x >= int.MaxValue || y <= int.MinValue || y >= int.MaxValue) return;
            if (!DungeonTaskActive(dungeonTask) || !eligible(this) || !eligible(actor)) return;
            actor.PostMessage((int)x, 50);
            if (!DungeonTaskActive(dungeonTask) || !eligible(this) || !eligible(actor)) return;
            actor.PostMessage((int)y, 51);
            if (!DungeonTaskActive(dungeonTask) || !eligible(this) || !eligible(actor)) return;
            actor.PostMessage(9, 105);
        }
        private bool PrepareDungeonNpc(Game actor, Func<Game, bool> eligible, long now)
        {
            if (!DungeonTaskActive(dungeonTask) || !eligible(this) || !eligible(actor)) return false;
            if (actor.TLBB.IsFollow)
            { IssueTeamCommand(actor, TeamCommand.Unfollow, now, eligible, actor.StopFollow); return false; }
            if (actor.IsRide)
            { IssueTeamCommand(actor, TeamCommand.Dismount, now, eligible, actor.DownRide); return false; }
            if (DungeonMovementPolicy.IsMoving(actor.TLBB.PlayerState))
            { IssueTeamCommand(actor, TeamCommand.Stop, now, eligible, () => StopDungeonTravel(actor, eligible)); return false; }
            return DungeonMovementPolicy.CanTalk(actor.TLBB.PlayerState);
        }
        private bool WaitTeamPreparation(TeamPatrolPhase phase, long now, IList<Game> participants,
            Func<Game, bool> eligible, Action<string> stop)
        {
            if (!DungeonTaskActive(dungeonTask)) return false;
            if (dungeonPreparationAt < 0 || dungeonPreparation != phase)
            { dungeonPreparation = phase; dungeonPreparationAt = now; }
            PublishTeamPatrol(phase, participants, eligible, DiagnosticReason.CommandPending);
            if (now - dungeonPreparationAt >= 10000)
                stop("chưa xác nhận bước cưỡi/theo/dừng sau 10 giây; hãy kiểm tra bằng tay.");
            return false;
        }
        private bool PrepareDungeonPatrol(DungeonTask task, IList<Game> participants, Func<Game, bool> eligible,
            long now, long clearAt, long quietWait, bool combat, bool loot, Action<string> stop)
        {
            BindDungeonParty(task, participants, eligible);
            if (!DungeonTaskActive(task) || !eligible(this)) return false;
            bool fighting = combat;
            foreach (Game actor in participants)
                if (actor != null && eligible(actor) && DungeonMovementPolicy.IsAttacking(actor.TLBB.PlayerState)) fighting = true;
            if (fighting || loot)
            {
                dungeonWasCombat = true; dungeonFollowAt = -5000; dungeonFollowConfirmed = false;
                bool pending = false;
                foreach (Game actor in participants)
                    if (actor != null && eligible(actor) && actor.TLBB.MapId == TLBB.MapId && actor.TLBB.IsFollow)
                    { pending = true; IssueTeamCommand(actor, TeamCommand.Unfollow, now, eligible, actor.StopFollow); }
                if (pending) return WaitTeamPreparation(TeamPatrolPhase.Unfollowing, now, participants, eligible, stop);
                foreach (Game actor in participants)
                    if (actor != null && eligible(actor) && actor.TLBB.MapId == TLBB.MapId && actor.IsRide)
                    { pending = true; IssueTeamCommand(actor, TeamCommand.Dismount, now, eligible, actor.DownRide); }
                if (pending) return WaitTeamPreparation(TeamPatrolPhase.Dismounting, now, participants, eligible, stop);
                foreach (Game actor in participants)
                    if (actor != null && eligible(actor) && actor.TLBB.MapId == TLBB.MapId && DungeonMovementPolicy.IsMoving(actor.TLBB.PlayerState))
                    { pending = true; IssueTeamCommand(actor, TeamCommand.Stop, now, eligible, () => StopDungeonTravel(actor, eligible)); }
                if (pending) return WaitTeamPreparation(TeamPatrolPhase.Stopping, now, participants, eligible, stop);
                dungeonPreparationAt = -1;
                bool attacksEnabled = true;
                foreach (Game actor in participants) if (actor != null && eligible(actor) && !actor.IsAttack) attacksEnabled = false;
                PublishTeamPatrol(loot ? TeamPatrolPhase.Loot : TeamPatrolPhase.Combat, participants, eligible,
                    attacksEnabled ? (loot ? DiagnosticReason.LootHold : DiagnosticReason.CombatHold) : DiagnosticReason.CombatDisabled);
                return false; // Existing combat/skills handle the actual attacks, preserving per-character settings.
            }
            if (dungeonWasCombat)
            { dungeonWasCombat = false; dungeonFollowAt = -5000; dungeonPreparationAt = -1; }
            if (now - clearAt < quietWait)
            { PublishTeamPatrol(TeamPatrolPhase.Waiting, participants, eligible, DiagnosticReason.None); return false; }
            if (TLBB.IsFollow)
            { IssueTeamCommand(this, TeamCommand.Unfollow, now, eligible, StopFollow);
                return WaitTeamPreparation(TeamPatrolPhase.Unfollowing, now, participants, eligible, stop); }
            bool unmountFollow = false;
            foreach (Game actor in participants)
                if (actor != null && eligible(actor) && actor.TLBB.MapId == TLBB.MapId
                    && actor.TLBB.HaveRide && !actor.IsRide && actor.TLBB.IsFollow)
                { unmountFollow = true; IssueTeamCommand(actor, TeamCommand.Unfollow, now, eligible, actor.StopFollow); }
            if (unmountFollow) return WaitTeamPreparation(TeamPatrolPhase.Unfollowing, now, participants, eligible, stop);
            bool mounting = false;
            foreach (Game actor in participants)
                if (actor != null && eligible(actor) && actor.TLBB.MapId == TLBB.MapId && actor.TLBB.HaveRide && !actor.IsRide)
                {
                    mounting = true;
                    if (!actor.TLBB.IsFollow) IssueTeamCommand(actor, TeamCommand.Mount, now, eligible, actor.UpRide);
                }
            if (mounting) return WaitTeamPreparation(TeamPatrolPhase.Mounting, now, participants, eligible, stop);
            dungeonPreparationAt = -1;
            bool missingFollow = false, mayRequest = participants.Count > 1;
            foreach (Game actor in participants)
            {
                if (actor == null || !eligible(actor) || actor.TLBB.MapId != TLBB.MapId || actor.TLBB.IsQuestOpen) mayRequest = false;
                if (actor != null && actor != this && eligible(actor) && !actor.TLBB.IsFollow) missingFollow = true;
            }
            foreach (Game known in DungeonWindowSnapshot())
                if (known != null && known != this && known.TLBB != null && known.TLBB.KeyId == TLBB.Id
                    && (!participants.Contains(known) || !eligible(known))) mayRequest = false;
            if (missingFollow && mayRequest)
            {
                if (dungeonFollowConfirmed) { dungeonFollowAt = -5000; dungeonFollowConfirmed = false; }
                if (dungeonFollowAt < 0)
                { dungeonFollowAt = now; if (DungeonTaskActive(task) && eligible(this)) AskTeamFollow(); }
                if (!DungeonTaskActive(task)) return false;
                bool confirmed = true;
                foreach (Game actor in participants)
                    if (actor != this && (!eligible(actor) || !actor.TLBB.IsFollow)) confirmed = false;
                if (!confirmed && now - dungeonFollowAt < 5000)
                { PublishTeamPatrol(TeamPatrolPhase.Following, participants, eligible, DiagnosticReason.CommandPending); return false; }
                if (confirmed) dungeonFollowConfirmed = true;
            }
            else if (!missingFollow) dungeonFollowConfirmed = true;
            PublishTeamPatrol(TeamPatrolPhase.Travel, participants, eligible, DiagnosticReason.None);
            RegroupDungeonPatrol(participants, eligible, now);
            return DungeonTaskActive(task) && eligible(this);
        }
        private void RegroupDungeonPatrol(IList<Game> participants, Func<Game, bool> eligible, long now)
        {
            foreach (Game actor in participants)
            {
                if (!DungeonTaskActive(dungeonTask) || !eligible(this)) return;
                if (actor == null || actor == this || !eligible(actor) || actor.TLBB.MapId != TLBB.MapId || actor.TLBB.IsFollow) continue;
                if (DungeonMovementPolicy.IsAttacking(actor.TLBB.PlayerState)) continue;
                if (TINHKIEM.GetDistance(actor.CharX, actor.CharY, CharX, CharY) > 4f)
                    IssueTeamCommand(actor, TeamCommand.Regroup, now, eligible, () => actor.Move(CharX, CharY));
            }
        }
    }
}
