using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace ChickenAutoEx.Startup.Tests
{
    public sealed class KyCuocSimulationTests
    {
        [Fact, Trait("KyCuocBaseline", "yes")]
        public void NewRunResetsCompletion()
        {
            dynamic g=KyCuocHarness.Create(); g.IsKyCuoc=false; g.Completed=true; g.IsKyCuoc=true;
            g.Step(); Assert.True((bool)g.IsKyCuoc); Assert.False((bool)g.Completed);
        }
        [Fact, Trait("KyCuocBaseline", "yes")]
        public void DisabledMenuDoesNotMoveAfterBossDeath()
        {
            dynamic g=KyCuocHarness.Create(); g.TLBB.MapId=61; g.IsBossDie=true;
            g.Advance(31000); g.IsKyCuoc=false; g.FinishTick();
            Assert.Empty((List<string>)g.Commands); Assert.False((bool)g.IsP);
        }
        [Fact, Trait("KyCuocBaseline", "yes")]
        public void AutoOffMemberReceivesNoCommands()
        {
            dynamic g=KyCuocHarness.Create(),m=KyCuocHarness.Create(); m.TLBB.IsLeader=false;
            m.TLBB.MapId=1; m.IsAuto=false; g.Party.Add(m); g.Step();
            Assert.Empty((List<string>)m.Commands);
        }
        [Fact, Trait("KyCuocBaseline", "yes")]
        public void ExitTravelDoesNotMeanCompleted()
        {
            dynamic g=KyCuocHarness.Create(); g.TLBB.MapId=61; g.IsBossDie=true; g.AtNpc=false;
            g.Advance(31000); g.FinishTick(); Assert.False((bool)g.Completed);
        }
        [Fact, Trait("KyCuocBaseline", "yes")]
        public void CombatResumesNearCurrentPositionInsteadOfOldWaypoint()
        {
            dynamic g=KyCuocHarness.Create(); g.TLBB.MapId=61; g.SetPending(2);
            g.CharX=82; g.CharY=50; g.AddMonster(); g.Advance(5000); g.Step();
            g.ClearMonsters(); g.Advance(4001); g.Commands.Clear(); g.Step();
            Assert.Contains("Move:71:50",(List<string>)g.Commands);
        }
        [Fact, Trait("KyCuocBaseline", "yes")]
        public void PetAoeWithEmptyMonstersDoesNotThrow()
        {
            dynamic g=KyCuocHarness.Create(); g.SetTick(150); g.AOE();
            Assert.Empty((List<string>)g.Commands);
        }

        [Theory]
        [InlineData("off")][InlineData("offline")][InlineData("dead")][InlineData("leaderchanged")]
        [InlineData("teamchanged")][InlineData("nullmetadata")][InlineData("othermodule")]
        public void InvalidLeaderCancelsBeforeCommands(string fault)
        {
            dynamic g=KyCuocHarness.Create();
            switch(fault) {
                case "off":g.IsAuto=false;break; case "offline":g.TLBB.Online=false;break;
                case "dead":g.TLBB.PlayerState=2;break;case "leaderchanged":g.TLBB.IsLeader=false;break;
                case "teamchanged":g.TLBB.KeyId="other";break;case "nullmetadata":g.TLBB=null;break;
                case "othermodule":g.IsAcBa=true;break;
            }
            g.Validate(); Assert.False((bool)g.IsKyCuoc); Assert.Empty((List<string>)g.Commands);
        }
        [Theory]
        [InlineData("off")][InlineData("offline")][InlineData("dead")][InlineData("zoning")]
        [InlineData("scene")][InlineData("mapchange")][InlineData("otherteam")][InlineData("nullmetadata")]
        public void InvalidMemberReceivesNoCommands(string fault)
        {
            dynamic g=KyCuocHarness.Create(),m=KyCuocHarness.Create(); m.TLBB.IsLeader=false;
            switch(fault) {
                case "off":m.IsAuto=false;break;case "offline":m.TLBB.Online=false;break;
                case "dead":m.TLBB.PlayerState=2;break;case "zoning":m.TLBB.PlayerState=7;break;
                case "scene":m.ON_SCENE_TRANSING=true;break;case "mapchange":m.IsChangeMap=true;break;
                case "otherteam":m.TLBB.KeyId="other";break;case "nullmetadata":m.TLBB=null;break;
            }
            g.Party.Add(m);g.Step();Assert.Empty((List<string>)m.Commands);
        }
        [Theory][InlineData("zoning")][InlineData("scene")][InlineData("mapchange")]
        public void SceneTransitionPausesWithoutSendingCommands(string fault)
        {
            dynamic g=KyCuocHarness.Create();
            if(fault=="zoning")g.TLBB.PlayerState=7;
            if(fault=="scene")g.ON_SCENE_TRANSING=true;
            if(fault=="mapchange")g.IsChangeMap=true;
            g.Step();Assert.Empty((List<string>)g.Commands);Assert.True((bool)g.IsKyCuoc);
        }
        [Fact] public void EntrySelectsOnceThenWaitsForMapAck()
        {
            dynamic g=KyCuocHarness.Create();g.TLBB.IsQuestOpen=true;g.AddOption(401001,-1);
            for(int i=0;i<5;i++)g.Step();
            Assert.Single((List<string>)g.Commands,c=>c=="Select:401001:-1");
            Assert.False((bool)g.Completed);g.Advance(60001);g.Step();Assert.False((bool)g.IsKyCuoc);
        }
        [Theory][InlineData("wrongoption")][InlineData("wrongextra")][InlineData("missingnpc")]
        public void EntryRejectsUnverifiedNpcOrOption(string fault)
        {
            dynamic g=KyCuocHarness.Create();g.TLBB.IsQuestOpen=true;
            g.AddOption(fault=="wrongoption"?99:401001,fault=="wrongextra"?1:-1);
            if(fault=="missingnpc")g.Objects.All.Clear();
            g.Step();Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Select:")||c.StartsWith("Talk:"));
            g.Advance(180001);g.Step();Assert.False((bool)g.IsKyCuoc);
        }
        [Fact] public void TalkIsNotRepeatedEveryTick()
        {
            dynamic g=KyCuocHarness.Create();for(int i=0;i<5;i++)g.Step();
            Assert.Single((List<string>)g.Commands,c=>c=="Talk:142");
        }
        [Fact] public void EntryWaitsForActiveMembersAtDoor()
        {
            dynamic g=KyCuocHarness.Create(),m=KyCuocHarness.Create();m.TLBB.IsLeader=false;m.AtNpc=false;
            g.TLBB.IsQuestOpen=true;g.AddOption(401001,-1);g.Party.Add(m);g.Step();
            Assert.DoesNotContain("Select:401001:-1",(List<string>)g.Commands);
        }
        [Fact] public void MemberInsideNeverReturnsToEntryNpc()
        {
            dynamic g=KyCuocHarness.Create(),m=KyCuocHarness.Create();m.TLBB.IsLeader=false;m.TLBB.MapId=61;
            g.Party.Add(m);g.Step();Assert.Empty((List<string>)m.Commands);
        }
        [Fact] public void LeaderInsideWaitsForMemberOutsideBeforePatrol()
        {
            dynamic g=KyCuocHarness.Create(),m=KyCuocHarness.Create();g.TLBB.MapId=61;m.TLBB.IsLeader=false;
            g.Party.Add(m);g.Step();g.Advance(5000);g.Step();
            Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Move:"));
            Assert.Contains("GotoNpc:0:142",(List<string>)m.Commands);
        }
        [Fact] public void CancellingDuringGotoPreventsLaterDialogCommands()
        {
            dynamic g=KyCuocHarness.Create();g.CancelOnGoto=true;g.TLBB.IsQuestOpen=true;g.AddOption(401001,-1);
            g.Step();Assert.False((bool)g.IsKyCuoc);Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Select:"));
        }
        [Fact] public void ActiveBossSessionStopsImmediatelyWhenMenuIsDisabled()
        {
            dynamic g=KyCuocHarness.Create();PrepareBossExit(g);g.IsKyCuoc=false;g.Commands.Clear();g.Advance(31000);g.Step();
            Assert.Empty((List<string>)g.Commands);Assert.False((bool)g.IsP);
        }
        [Fact] public void ExitWaitsForObservedDepartureAndCanStartSecondRun()
        {
            dynamic g=KyCuocHarness.Create();PrepareBossExit(g);
            g.Advance(30000);g.TLBB.IsQuestOpen=true;g.AddOption(44000,0);g.Step();
            Assert.Contains("Select:44000:0",(List<string>)g.Commands);Assert.False((bool)g.Completed);
            g.Commands.Clear();g.Step();Assert.Empty((List<string>)g.Commands);
            g.TLBB.MapId=0;g.Step();Assert.True((bool)g.Completed);Assert.False((bool)g.IsKyCuoc);
            g.IsKyCuoc=true;Assert.False((bool)g.Completed);g.Step();Assert.True((bool)g.IsKyCuoc);
        }
        [Fact] public void FalseGotoDoesNotSelectExitOrComplete()
        {
            dynamic g=KyCuocHarness.Create();PrepareBossExit(g);g.Advance(31000);g.AtNpc=false;
            g.TLBB.IsQuestOpen=true;g.AddOption(44000,0);g.Step();
            Assert.Contains("GotoNpc:61:12349",(List<string>)g.Commands);
            Assert.False((bool)g.Completed);Assert.DoesNotContain("Select:44000:0",(List<string>)g.Commands);
        }
        [Fact] public void ExitWithoutMapAckTimesOutAndReportsOnce()
        {
            dynamic g=KyCuocHarness.Create();PrepareBossExit(g);g.Advance(31000);g.TLBB.IsQuestOpen=true;g.AddOption(44000,0);g.Step();
            g.Advance(60001);g.Step();Assert.False((bool)g.IsKyCuoc);Assert.False((bool)g.Completed);
            int count=g.Notices.Count;g.Step();Assert.Equal(count,(int)g.Notices.Count);
        }
        [Fact] public void UnexpectedExitDestinationIsNotCompleted()
        {
            dynamic g=KyCuocHarness.Create();PrepareBossExit(g);g.Advance(31000);g.TLBB.IsQuestOpen=true;g.AddOption(44000,0);g.Step();
            g.TLBB.MapId=2;g.Step();Assert.False((bool)g.Completed);Assert.False((bool)g.IsKyCuoc);
        }
        [Fact] public void MapDepartureWithoutSelectingExitCancelsSession()
        {
            dynamic g=KyCuocHarness.Create();g.TLBB.MapId=61;g.Step();g.TLBB.MapId=0;g.Step();
            Assert.False((bool)g.IsKyCuoc);Assert.False((bool)g.Completed);
        }
        [Fact] public void DeadBossWithoutAliveObservationDoesNotTriggerExit()
        {
            dynamic g=KyCuocHarness.Create();g.TLBB.MapId=61;g.SetBoss(0);g.Step();g.Advance(31000);g.Step();
            Assert.DoesNotContain("GotoNpc:61:12349",(List<string>)g.Commands);Assert.False((bool)g.Completed);
        }
        [Fact] public void RemainingMonstersPreventExitAfterBossDeath()
        {
            dynamic g=KyCuocHarness.Create();PrepareBossExit(g);g.Advance(31000);g.AddMonster();g.Step();
            Assert.DoesNotContain("GotoNpc:61:12349",(List<string>)g.Commands);Assert.False((bool)g.Completed);
        }
        [Fact] public void ExitCoordinatesKnownMembersEvenWhenLeaderLeavesFirst()
        {
            dynamic g=KyCuocHarness.Create(),m=KyCuocHarness.Create();g.TLBB.MapId=m.TLBB.MapId=61;m.TLBB.IsLeader=false;
            g.Party.Add(m);PrepareBossExit(g);g.Advance(31000);
            g.TLBB.IsQuestOpen=m.TLBB.IsQuestOpen=true;g.AddOption(44000,0);m.AddOption(44000,0);g.Step();
            Assert.Contains("Select:44000:0",(List<string>)m.Commands);
            g.TLBB.MapId=0;g.Step();Assert.False((bool)g.Completed);
            m.TLBB.MapId=0;g.Step();Assert.True((bool)g.Completed);
        }
        [Fact] public void MemberBossObservationCanCoordinateExit()
        {
            dynamic g=KyCuocHarness.Create(),m=KyCuocHarness.Create();g.TLBB.MapId=m.TLBB.MapId=61;m.TLBB.IsLeader=false;
            g.Party.Add(m);m.SetBoss(1);g.Step();m.SetBoss(0);g.Step();g.Advance(31000);g.Step();
            Assert.Contains("GotoNpc:61:12349",(List<string>)g.Commands);
        }
        [Fact] public void ResumeTargetRemainsStableUntilAnotherCombat()
        {
            dynamic g=KyCuocHarness.Create();g.TLBB.MapId=61;g.SetPending(2);g.CharX=82;g.CharY=50;g.AddMonster();g.Step();
            g.ClearMonsters();g.Advance(4001);g.Step();Assert.Equal(5,(int)g.PatrolIndex);
            g.CharX=60;g.CharY=40;g.Commands.Clear();g.Step();
            Assert.Equal(5,(int)g.PatrolIndex);Assert.Contains("Move:71:50",(List<string>)g.Commands);
        }
        [Fact] public void ReanchoringKeepsSkippedPointsAndOwnsIndependentIndex()
        {
            dynamic g=KyCuocHarness.Create();g.TLBB.MapId=61;g.SetPending(2);g.MoveIndex=99;g.CharX=82;g.CharY=50;
            g.AddMonster();g.Step();g.ClearMonsters();g.Advance(4001);g.Step();
            Assert.False(((bool[])g.Visited)[2]);Assert.False(((bool[])g.Visited)[3]);Assert.Equal(99,(int)g.MoveIndex);
            g.CharX=71;g.CharY=50;g.Step();Assert.Equal(2,(int)g.PatrolIndex);
            g.CharX=81;g.CharY=81;g.Step();Assert.Equal(3,(int)g.PatrolIndex);
        }
        [Fact] public void PatrolWaitsAfterArrivalAndStopsWhenStuck()
        {
            dynamic g=KyCuocHarness.Create();g.TLBB.MapId=61;g.CharX=0;g.CharY=0;g.Step();
            Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Move:"));
            g.Advance(4001);g.Step();Assert.Contains((List<string>)g.Commands,c=>c.StartsWith("Move:"));
            g.Advance(180001);g.Step();Assert.False((bool)g.IsKyCuoc);
        }
        [Fact] public void CombatOverridesMovementBeforeClearWaitExpires()
        {
            dynamic g=KyCuocHarness.Create();g.TLBB.MapId=61;g.IsRide=g.TLBB.IsRide=true;g.TLBB.IsFollow=true;g.AddMonster();g.Step();
            Assert.Contains("DownRide",(List<string>)g.Commands);Assert.Contains("StopFollow",(List<string>)g.Commands);
            Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Move:"));
        }
        [Fact] public void ErrorStopsModuleWithoutLoggingExceptionSecrets()
        {
            dynamic g=KyCuocHarness.Create();g.ThrowRead=true;g.TLBB.IsQuestOpen=true;g.AddOption(401001,-1);g.Step();
            Assert.False((bool)g.IsKyCuoc);
            Assert.Contains((List<string>)g.Notices,m=>m.Contains("InvalidOperationException"));
            Assert.DoesNotContain((List<string>)g.Notices,m=>m.Contains("SECRET-DO-NOT-LOG"));
        }
        [Fact] public void PetAoeSelectsNearestLivingMonsterInsideRadius()
        {
            dynamic g=KyCuocHarness.Create();g.SetTick(150);
            g.Objects.Monter.Add(newObject(g,40,0,1));
            g.Objects.NearMonter20m.Add(newObject(g,12,0,1));g.Objects.NearMonter20m.Add(newObject(g,3,4,1));
            g.Objects.NearMonter20m.Add(newObject(g,1,1,0));g.AOE();
            Assert.Equal(new[]{"Pet:674:3:4"},(List<string>)g.Commands);
        }
        [Theory][InlineData("off")][InlineData("offline")][InlineData("dead")][InlineData("zoning")][InlineData("noskill")][InlineData("disabled")][InlineData("nullskill")][InlineData("scene")][InlineData("mapchange")]
        public void PetAoeRespectsControlAndSkillConditions(string fault)
        {
            dynamic g=KyCuocHarness.Create();g.Objects.NearMonter20m.Add(newObject(g,3,4,1));g.SetTick(150);
            if(fault=="off")g.IsAuto=false;if(fault=="offline")g.TLBB.Online=false;if(fault=="dead")g.TLBB.PlayerState=2;
            if(fault=="zoning")g.TLBB.PlayerState=7;if(fault=="noskill")g.PetSkill=-1;if(fault=="disabled")g.DisablePet();
            if(fault=="nullskill")g.TLBB.SkillPetType=null;if(fault=="scene")g.ON_SCENE_TRANSING=true;if(fault=="mapchange")g.IsChangeMap=true;
            g.AOE();Assert.Empty((List<string>)g.Commands);
        }
        [Fact] public void CountdownUsesThirtySecondsAndNeverBecomesNegative()
        {
            dynamic g=KyCuocHarness.Create();PrepareBossExit(g);g.Advance(21000);g.Step();
            Assert.Contains("Debug:Di chuyển sau 9s",(List<string>)g.Commands);
            Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Debug:Di chuyển sau -"));
        }
        [Fact] public void PatrolStillVisitsAllSkippedPointsAndDoesNotCompleteAtRouteEnd()
        {
            dynamic g=KyCuocHarness.Create();g.TLBB.MapId=61;g.SetPending(2);g.CharX=82;g.CharY=50;
            g.AddMonster();g.Step();g.ClearMonsters();g.Advance(4001);
            var seen=new HashSet<string>();
            for(int i=0;i<9;i++)
            {
                g.Commands.Clear();g.Step();string move=((List<string>)g.Commands).Single(c=>c.StartsWith("Move:"));
                seen.Add(move);string[] parts=move.Split(':');g.CharX=float.Parse(parts[1]);g.CharY=float.Parse(parts[2]);
            }
            foreach(string point in new[]{"Move:42:42","Move:84:42","Move:81:81","Move:42:85","Move:50:49","Move:71:50"})
                Assert.Contains(point,seen);
            Assert.False((bool)g.Completed);
        }
        [Theory][InlineData("off")][InlineData("otherteam")]
        public void MemberLeavingDuringExitCancelsWithoutFurtherCommands(string fault)
        {
            dynamic g=KyCuocHarness.Create(),m=KyCuocHarness.Create();g.TLBB.MapId=m.TLBB.MapId=61;m.TLBB.IsLeader=false;
            g.Party.Add(m);PrepareBossExit(g);g.Advance(31000);g.Step();
            if(fault=="off")m.IsAuto=false;else m.TLBB.KeyId="other";
            g.Commands.Clear();m.Commands.Clear();g.Step();
            Assert.False((bool)g.IsKyCuoc);Assert.False((bool)g.Completed);Assert.Empty((List<string>)m.Commands);
        }
        [Fact] public void UninitializedMemberGetsNoCommands()
        {
            dynamic g=KyCuocHarness.Create(),m=KyCuocHarness.Create();m.TLBB.IsLeader=false;m.IsInit=false;
            g.Party.Add(m);g.Step();Assert.Empty((List<string>)m.Commands);
        }
        [Fact] public void PetAoeGuardAlsoWorksOutsideDungeon()
        {
            dynamic g=KyCuocHarness.Create();g.TLBB.MapId=4;g.SetTick(150);g.AOE();Assert.Empty((List<string>)g.Commands);
            g.Objects.NearMonter20m.Add(newObject(g,3,4,1));g.AOE();Assert.Contains("Pet:674:3:4",(List<string>)g.Commands);
        }
        [Fact] public void ActualAutoOffGuardCancelsSession()
        {
            dynamic g=KyCuocHarness.Create();g.IsAuto=false;g.AutoGuard();
            Assert.False((bool)g.IsKyCuoc);Assert.Empty((List<string>)g.Commands);
        }
        [Fact] public void ClearMissionResetsKyCuocForNextRun()
        {
            dynamic g=KyCuocHarness.Create();g.Completed=true;g.ClearMission();
            Assert.False((bool)g.IsKyCuoc);Assert.False((bool)g.Completed);
            g.IsKyCuoc=true;g.Step();Assert.True((bool)g.IsKyCuoc);
        }
        [Fact] public void ActiveKyCuocIsBusyEvenWhileTravellingToDoor()
        {
            dynamic g=KyCuocHarness.Create();g.TLBB.MapId=0;Assert.True((bool)g.Busy);
            g.IsKyCuoc=false;Assert.False((bool)g.Busy);
        }
        [Fact] public void MemberWaitsForPostTransitionDelay()
        {
            dynamic g=KyCuocHarness.Create(),m=KyCuocHarness.Create();m.TLBB.IsLeader=false;m.ResetTransition();
            g.Party.Add(m);g.Step();Assert.Empty((List<string>)m.Commands);
            g.Advance(2001);g.Step();Assert.Contains("GotoNpc:0:142",(List<string>)m.Commands);
        }
        [Fact] public void MemberRecoveryTimerStartsAtLossInsteadOfSessionStart()
        {
            dynamic g=KyCuocHarness.Create(),m=KyCuocHarness.Create();g.TLBB.MapId=m.TLBB.MapId=61;m.TLBB.IsLeader=false;
            g.Party.Add(m);g.Step();g.Advance(200000);m.TLBB.PlayerState=7;g.Step();Assert.True((bool)g.IsKyCuoc);
            g.Advance(2001);m.TLBB.PlayerState=0;g.Step();Assert.True((bool)g.IsKyCuoc);
        }
        [Fact] public void LongLootPauseIsNotReportedAsPatrolStuck()
        {
            dynamic g=KyCuocHarness.Create();g.TLBB.MapId=61;g.Step();g.Advance(4001);g.Step();
            g.Advance(200000);g.Loot=true;g.Step();g.Loot=false;g.Step();Assert.True((bool)g.IsKyCuoc);
        }
        [Fact] public void MemberOffBeforeExitIsExcludedWithoutBlockingLeader()
        {
            dynamic g=KyCuocHarness.Create(),m=KyCuocHarness.Create();g.TLBB.MapId=m.TLBB.MapId=61;m.TLBB.IsLeader=false;
            g.Party.Add(m);g.Step();m.IsAuto=false;m.Commands.Clear();g.Step();PrepareBossExit(g);g.Advance(31000);
            g.TLBB.IsQuestOpen=true;g.AddOption(44000,0);g.Step();g.TLBB.MapId=0;g.Step();
            Assert.True((bool)g.Completed);Assert.Empty((List<string>)m.Commands);
        }
        [Fact] public void AlreadyAcknowledgedMemberExitStaysValidAfterAutoOff()
        {
            dynamic g=KyCuocHarness.Create(),m=KyCuocHarness.Create();g.TLBB.MapId=m.TLBB.MapId=61;m.TLBB.IsLeader=false;
            g.Party.Add(m);PrepareBossExit(g);g.Advance(31000);
            g.TLBB.IsQuestOpen=m.TLBB.IsQuestOpen=true;g.AddOption(44000,0);m.AddOption(44000,0);g.Step();
            m.TLBB.MapId=0;g.Step();Assert.False((bool)g.Completed);
            m.IsAuto=false;m.Commands.Clear();g.TLBB.MapId=0;g.Step();
            Assert.True((bool)g.Completed);Assert.Empty((List<string>)m.Commands);
        }
        private static dynamic newObject(dynamic g,float x,float y,float hp)=>g.MakeObject(x,y,hp);
        private static void PrepareBossExit(dynamic g)
        {
            g.TLBB.MapId=61;g.SetBoss(1);g.Step();g.SetBoss(0);g.Step();g.Commands.Clear();
        }
    }

    internal static class KyCuocHarness
    {
        private static readonly Lazy<Assembly> Compiled=new Lazy<Assembly>(Compile);
        public static dynamic Create()=>Activator.CreateInstance(Compiled.Value.GetType("TinhKiemAuto.Game"));
        private static Assembly Compile()
        {
            bool baseline=Environment.GetEnvironmentVariable("KY_CUOC_BASELINE")=="1";
            string text=File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"RegressionSources",baseline?"LegacyGame.cs":"KyCuocReviewedGame.cs"));
            var root=CSharpSyntaxTree.ParseText(text).GetRoot();
            var game=root.DescendantNodes().OfType<ClassDeclarationSyntax>().Single(c=>c.Identifier.Text=="Game");
            string methods=baseline?string.Join("\n",game.Members.OfType<MethodDeclarationSyntax>().Where(m=>
                new[]{"DatDoiKyCuoc","TrieuTap","AOE","ClearMission"}.Contains(m.Identifier.Text)
                ||m.Identifier.Text=="MoveNext"&&m.ParameterList.Parameters.Count==1).Select(m=>m.ToFullString())):game.Members.OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="ClearMission").ToFullString();
            if(!baseline) methods+="public void MoveNext(int[,] p){throw new InvalidOperationException(\"Legacy movement must not be called\");}";
            methods+=game.Members.OfType<PropertyDeclarationSyntax>().Single(p=>p.Identifier.Text=="IsKyCuoc").ToFullString();
            methods+=game.Members.OfType<PropertyDeclarationSyntax>().Single(p=>p.Identifier.Text=="IsBusy").ToFullString();
            string autoGuard=game.Members.OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="Auto")
                .DescendantNodes().OfType<IfStatementSyntax>().Single(i=>i.Condition.ToString()=="!IsAuto").ToFullString();
            methods+="public bool Busy=>IsBusy; public void AutoGuard(){"+autoGuard+"}";
            string completion=baseline?game.DescendantNodes().OfType<IfStatementSyntax>().Single(i=>
                i.Condition.ToString()=="TLBB.MapId == MAP.TranLongKyCuoc && IsBossDie").ToFullString():"DatDoiKyCuoc();";
            string controls=baseline?"public void SetPending(int i) { MoveIndex=i; }":"public void SetPending(int i) { ResetKyCuocSession(); InitializeKyCuocSession(); kyCuocLeader.RouteIndex=i; for(int j=0;j<i;j++) kyCuocLeader.Visited[j]=true; } public int PatrolIndex=>kyCuocLeader.RouteIndex; public bool[] Visited=>kyCuocLeader.Visited; public string Phase=>kyCuocPhase.ToString(); public void Validate()=>CheckKyCuocSession();";
            string source="using System; using System.Collections.Generic; using Stopwatch= TinhKiemAuto.FakeStopwatch; namespace TinhKiemAuto {"
                +Fixture+"public partial class Game {"+Fields+methods+controls+"public void FinishTick(){"+completion+"} } }";
            var trees=new List<SyntaxTree>{CSharpSyntaxTree.ParseText(source)};
            if(!baseline)
                foreach(string name in new[]{"Game.KyCuoc.cs","Game.PetAoe.cs"})
                    trees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"RegressionSources",name))
                        .Replace("using System.Diagnostics;","using Stopwatch= TinhKiemAuto.FakeStopwatch;")));
            var references=((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator).Select(p=>MetadataReference.CreateFromFile(p));
            var compilation=CSharpCompilation.Create("KyCuocSimulation",trees,references,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var output=new MemoryStream(); var result=compilation.Emit(output);
            Assert.True(result.Success,string.Join("\n",result.Diagnostics.Where(d=>d.Severity==DiagnosticSeverity.Error)));
            return System.Reflection.Assembly.Load(output.ToArray());
        }
        private const string Fixture="""
            public class FakeStopwatch {
                public static long Now; private long start=Now;
                public static FakeStopwatch StartNew()=>new FakeStopwatch();
                public long ElapsedMilliseconds=>Now-start; public TimeSpan Elapsed=>TimeSpan.FromMilliseconds(ElapsedMilliseconds);
            }
            public class TLBB {
                public int MapId=0,PlayerState=0,MapAcBa; public string Id="leader",KeyId="leader",SkillPetType="PetSkill7_8";
                public bool IsLeader=true,Online=true,IsQuestOpen,IsRide,IsFollow,HaveRide;
            }
            public class NPC { public int Id,X,Y,Map; }
            public static class LACDUONG { public const int Id=0; public static NPC VuongTichTan=new NPC{Id=142,X=366,Y=228,Map=0}; }
            public static class TRANLONGKYCUOC { public static NPC TeThanh=new NPC{Id=12349,X=40,Y=40,Map=61}; }
            public static class MAP {
                public const int TranLongKyCuoc=61,ThuyLao=66,PhungHoangCoThanh=10,ViemMaSon=11,TamTaiHiepCoc=12,
                TangKinhCac=13,YenTuO=14,TacKhauDoanhDia=15,SinhTuLoiDai=16,PhungHoangCoThanhPhuBan=17,
                HuyenVuDaoPhuBan=18,ThanhThuSonPhuBan=19,PhieuMieuPhong=20;
            }
            public class GameObject { public int Id; public bool IsNPC; public float HP=1,X,Y; public string CleanName=""; }
            public class ObjectsFixture {
                public List<GameObject> All=new List<GameObject>{new GameObject{Id=142,IsNPC=true},new GameObject{Id=12349,IsNPC=true}};
                public List<GameObject> NearMonter12m=new List<GameObject>(),NearMonter18m=new List<GameObject>(),NearMonter20m=new List<GameObject>(),Monter=new List<GameObject>();
            }
            public class QuestFrame { public int StrOptionExtra1,StrOptionExtra2;
                public static List<QuestFrame> Enum(Game g) { if(g.ThrowRead) throw new InvalidOperationException("SECRET-DO-NOT-LOG"); return g.Dialog; } }
            public static class Global { public static bool UseSkillPet=true; }
            public static class TINHKIEM { public static float GetDistance(float x,float y,float a,float b)=>(float)Math.Sqrt((x-a)*(x-a)+(y-b)*(y-b)); }
            public static class CanhBao { public enum Kieu { Eror } public static List<string> Messages=new List<string>();
                public static void Msg(string title,string message,Kieu kind){ Messages.Add(message); } }
            """;
        private const string Fields="""
            public static int TickCount=18;
            public TLBB TLBB=new TLBB(); public ObjectsFixture Objects=new ObjectsFixture(); public List<Game> Party=new List<Game>();
            public List<QuestFrame> Dialog=new List<QuestFrame>(); public List<string> Commands=new List<string>();
            public bool IsAuto=true,IsInit=true,IsRide,ON_SCENE_TRANSING,IsChangeMap,AtNpc=true,Loot,ThrowRead;
            public bool IsThuyLao,IsXongThuyLao,DaNhanThuyLao,IsTrieuTap,IsTheoQ,IsQ123ToChau,IsYenTuO,IsBossDie,IsNhamBinhSinhDie,IsMapAcBa,IsQ123LauLan,IsP,TraQ,NhanQ,IsClick,IsContinute;
            public bool IsLauLanTamBao,IsAcBa,IsTrungAc,IsPhungHoangLangMo,IsPMP,IsTuBaoBon,IsHuyetChien,IsLuyenKim;
            public int PetSkill=674; public bool CancelOnGoto;
            public int MapAcTac,MapTKC,MoveIndex=-1; public float CharX,CharY,RoundX,RoundY;
            private bool IsXongKyCuoc; private Stopwatch ClearTime=Stopwatch.StartNew(); public Stopwatch BossDieTime=Stopwatch.StartNew(),tranTime=Stopwatch.StartNew();
            public Game(){ TickCount=18; Global.UseSkillPet=true; FakeStopwatch.Now+=3000; IsKyCuoc=true; }
            public bool Completed { get=>IsXongKyCuoc; set=>IsXongKyCuoc=value; }
            public List<string> Notices=>CanhBao.Messages;
            public void Step()=>DatDoiKyCuoc(); public void Advance(long ms){FakeStopwatch.Now+=ms;} public void SetTick(int tick){TickCount=tick;}
            public void AddOption(int a,int b){Dialog.Add(new QuestFrame{StrOptionExtra1=a,StrOptionExtra2=b});}
            public void AddMonster(){var m=new GameObject(); Objects.NearMonter18m.Add(m); Objects.NearMonter20m.Add(m); Objects.Monter.Add(m);}
            public void ClearMonsters(){Objects.NearMonter18m.Clear(); Objects.NearMonter20m.Clear(); Objects.Monter.Clear();}
            public void SetBoss(float hp){Objects.All.RemoveAll(o=>o.CleanName=="viencokyhon"); Objects.All.Add(new GameObject{Id=700,CleanName="viencokyhon",HP=hp});}
            public void MoveNext(){MoveNext(new int[,]{{42,42},{84,42},{81,81},{42,85},{50,49},{71,50}});}
            public bool GoTo(NPC n){Commands.Add($"GotoNpc:{n.Map}:{n.Id}"); if(CancelOnGoto)IsKyCuoc=false; return AtNpc;}
            public bool GoTo(float x,float y,int map=-1,bool force=false){Commands.Add($"Goto:{map}:{x}:{y}"); return false;}
            public void Move(float x,float y){Commands.Add($"Move:{x}:{y}");}
            public bool PickItem(){if(Loot)Commands.Add("Loot"); return Loot;}
            public void StopFollow(){Commands.Add("StopFollow"); TLBB.IsFollow=false;}
            public void UpRide(){Commands.Add("UpRide"); IsRide=TLBB.IsRide=true;}
            public void DownRide(){Commands.Add("DownRide"); IsRide=TLBB.IsRide=false;}
            public void Ride(){Commands.Add("Ride");}
            public bool IsMapPhuBan()=>TLBB.MapId==61;
            public void NhanThuyLao(){} public void DiThuyLao(){} public void ResetThuyLaoProgress(){}
            public void ResetTransition(){tranTime=Stopwatch.StartNew();}
            public void PostMessage(int a,int b){Commands.Add($"Post:{a}:{b}");}
            public void AskTeamFollow(){Commands.Add("FollowTeam");}
            public void Talk(NPC n){Commands.Add("Talk:"+n.Id);} public void Talk(int id){Commands.Add("Talk:"+id);}
            public void QuestFrameOptionClicked(int a,int b){Commands.Add($"Select:{a}:{b}");}
            public void CloseQuest(){Commands.Add("CloseQuest");} public void PushDebugMessage(string m){Commands.Add("Debug:"+m);}
            public int SkillPetId(string type)=>PetSkill;
            public GameObject MakeObject(float x,float y,float hp)=>new GameObject{X=x,Y=y,HP=hp};
            public void DisablePet(){Global.UseSkillPet=false;}
            public void UseSkillPet(int id,float x,float y){Commands.Add($"Pet:{id}:{x}:{y}");}
            """;
    }
}
