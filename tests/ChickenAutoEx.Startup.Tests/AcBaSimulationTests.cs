using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace ChickenAutoEx.Startup.Tests
{
    public sealed class AcBaSimulationTests
    {
        private static List<string> Commands(dynamic g)=>(List<string>)g.Commands;
        [Fact, Trait("AcBaBaseline","yes")] public void UnknownEventDoesNotWalkToOwnSchool(){dynamic g=AcBaHarness.Create();g.Step();Assert.Empty(Commands((object)g));}
        [Fact, Trait("AcBaBaseline","yes")] public void OutdoorCalendarIsBusy(){dynamic g=AcBaHarness.Create();Assert.True((bool)g.Busy);}
        [Fact, Trait("AcBaBaseline","yes")] public void AutoOffClearsAcBa(){dynamic g=AcBaHarness.Create();g.IsAuto=false;g.AutoOffGuard();Assert.False((bool)g.IsAcBa);}
        [Fact, Trait("AcBaBaseline","yes")] public void ActiveAcBaRequestsSystemHook(){dynamic g=AcBaHarness.Create();Assert.True((bool)g.ShouldHook());}
        [Fact, Trait("AcBaBaseline","yes")] public void PausedEngineSendsNoCommands(){dynamic g=AcBaHarness.Create();g.Choose(1);g.Pause(true);g.Step();Assert.Empty(Commands((object)g));}
        [Fact, Trait("AcBaBaseline","yes")] public void WrongDungeonIsNeverPatrolled(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=80;g.Step();Assert.Empty(Commands((object)g));}
        [Fact, Trait("AcBaBaseline","yes")] public void UnobservedDeadBossIsNotCompletion(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=173;g.SetBoss(0);g.Step();Assert.False((bool)g.IsBossDie);Assert.Equal("Patrol",(string)g.Phase);}
        [Fact, Trait("AcBaBaseline","yes")] public void WrongDialogNeverClicksAll(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=9;g.AddNpc();g.TryEntry();g.SetDialog(99,-1);g.TryEntry();Assert.DoesNotContain("ClickAll",Commands((object)g));}
        [Theory]
        [InlineData(1,9,173,"Thiếu Lâm")][InlineData(2,11,175,"Minh Giáo")][InlineData(3,10,174,"Cái Bang")][InlineData(4,12,176,"Võ Đang")][InlineData(5,15,179,"Nga Mi")][InlineData(6,16,180,"Tinh Túc")][InlineData(7,13,177,"Thiên Long")][InlineData(8,17,181,"Thiên Sơn")][InlineData(9,14,178,"Tiêu Dao")][InlineData(32,284,288,"Mộ Dung")][InlineData(37,615,618,"Đường Môn")]
        public void AllSchoolsRecognizeRepeatedNpcSchoolAndPreserveMaps(int school,int outdoor,int dungeon,string name)
        {dynamic h=AcBaHarness.Helpers(),g=AcBaHarness.Create();string text=$"NPC tại {name}: Ác Bá đã xuất hiện tại phái {name}.";Assert.Equal(school,(int)h.Parse(text));Assert.Equal(outdoor,(int)h.Outside(school));Assert.Equal(dungeon,(int)h.Inside(school));g.TLBB.Menpai=school;g.ReceiveAcBaNotice(text);g.Step();Assert.Equal(school,(int)g.Target);Assert.Contains(Commands((object)g),c=>c.StartsWith("Goto:"+outdoor+":"));Assert.NotNull((int[,])h.Route("",school));}
        [Theory][InlineData("Nga My")][InlineData("Nga Mi Sơn")][InlineData("NGA MI")]
        public void NgaMiAliasesAreAccepted(string school){dynamic h=AcBaHarness.Helpers();Assert.Equal(5,(int)h.Parse("NPC nói Ác Bá đã xuất hiện tại phái "+school));}
        [Theory][InlineData("Ác Bá xuất hiện tại Nga Mi và Võ Đang")][InlineData("Ác Bá chưa xuất hiện tại Nga Mi")][InlineData("Ác Bá sắp xuất hiện tại Nga Mi")][InlineData("Ác Bá sẽ xuất hiện tại Nga Mi")][InlineData("NPC Nga Mi: không có thông báo gì")][InlineData("Giang hồ tiểu tiểu tại Nga Mi")]
        public void AmbiguousOrNonActiveTextDoesNotSelectSchool(string text){dynamic h=AcBaHarness.Helpers();Assert.Equal(-1,(int)h.Parse(text));}
        [Theory][InlineData("VISCII")][InlineData("UTF-8")]
        public void ExplicitCodecDecodesActualAccentedText(string codec){dynamic h=AcBaHarness.Helpers();string text="NPC Nga Mi Sơn: Ác Bá đã xuất hiện tại phái Nga Mi.";Assert.Equal(text,(string)h.Decode(h.Packet(text,codec,4),codec));}
        [Theory][InlineData(3)][InlineData(2)][InlineData(5)] public void OtherChannelsCannotDispatchEvent(int channel){dynamic h=AcBaHarness.Helpers();Assert.Null((string)h.Decode(h.Packet("Ác Bá xuất hiện tại Nga Mi","VISCII",channel),"VISCII"));}
        [Fact] public void HeaderAndBufferBoundsAreFailClosed(){dynamic h=AcBaHarness.Helpers();Assert.Null((string)h.Decode(null,"VISCII"));for(int i=0;i<=15;i++)Assert.Null((string)h.Decode(new byte[i],"VISCII"));Assert.Null((string)h.Decode(new byte[65537],"VISCII"));}
        [Fact] public void UnknownCodecAndInvalidUtf8AreRejected(){dynamic h=AcBaHarness.Helpers();byte[] p=h.Packet("abcdef");p[15]=255;Assert.Null((string)h.Decode(p,"UTF-8"));Assert.Null((string)h.Decode(p,"guess"));}
        [Fact] public void EmbeddedTerminatorAndPrivatePayloadSignatureAreRejected(){dynamic h=AcBaHarness.Helpers();byte[] p=h.Packet("Ác Bá xuất hiện tại Nga Mi");p[10]=3;p[16]=218;p[17]=3;p[22]=4;Assert.Null((string)h.Decode(p,"VISCII"));p[10]=4;p[20]=0;Assert.Null((string)h.Decode(p,"VISCII"));}
        [Theory][InlineData("")][InlineData("bad")][InlineData("-2,3-4,5")][InlineData("2,3-4,x")][InlineData("0,3-4,5")][InlineData("2,3,4-5,6")][InlineData("10001,2-3,4")]
        public void InvalidSavedRouteFallsBackToRecoveredPoints(string saved){dynamic h=AcBaHarness.Helpers();int[,] expected=h.Route("",1),actual=h.Route(saved,1);Assert.Equal(expected.Cast<int>(),actual.Cast<int>());}
        [Fact] public void ValidSavedRouteIsRetained(){dynamic h=AcBaHarness.Helpers();int[,] actual=h.Route(" 2,3 - 4,5 ",1);Assert.Equal(new[]{2,3,4,5},actual.Cast<int>());}
        [Fact] public void DuplicateDoesNotRefreshFreshness(){dynamic g=AcBaHarness.Create();g.ReceiveAcBaNotice("Ác Bá xuất hiện tại Nga Mi");g.Advance(599999);g.ReceiveAcBaNotice("NPC khác Nga Mi: Ác Bá xuất hiện tại Nga Mi");g.Advance(2);g.Step();Assert.Equal(-1,(int)g.Target);Assert.Empty(Commands((object)g));}
        [Fact] public void ClearMissionDiscardsNotice(){dynamic g=AcBaHarness.Create();g.ReceiveAcBaNotice("Ác Bá xuất hiện tại Thiếu Lâm");g.ClearMission();Assert.Equal(-1,(int)g.AcBa);Assert.False((bool)g.IsAcBa);g.IsAcBa=true;g.Step();Assert.Empty(Commands((object)g));}
        [Fact] public void ChangedCharacterStopsBeforeCommands(){dynamic g=AcBaHarness.Create();g.Choose(1);g.Step();g.Commands.Clear();g.TLBB.Id="new";g.Step();Assert.False((bool)g.IsAcBa);Assert.Empty(Commands((object)g));}
        [Fact] public void EndedNoticeStopsMatchingRun(){dynamic g=AcBaHarness.Create();g.Choose(5);g.ReceiveAcBaNotice("Ác Bá tại Nga Mi đã kết thúc");Assert.False((bool)g.IsAcBa);}
        [Fact] public void EndedOtherSchoolDoesNotCancelActiveRun(){dynamic g=AcBaHarness.Create();g.Choose(1);g.Step();g.ReceiveAcBaNotice("Ác Bá tại Nga Mi đã kết thúc");Assert.True((bool)g.IsAcBa);}
        [Fact] public void NewNoticeCannotRetargetActiveRun(){dynamic g=AcBaHarness.Create();g.Choose(1);g.Step();g.ReceiveAcBaNotice("Ác Bá xuất hiện tại Nga Mi");g.Step();Assert.Equal(1,(int)g.Target);}
        [Theory][InlineData("offline")][InlineData("init")][InlineData("dead")][InlineData("leader")][InlineData("team")][InlineData("jail")]
        public void InvalidControllerStops(string kind){dynamic g=AcBaHarness.Create();g.Choose(1);switch(kind){case "offline":g.TLBB.Online=false;break;case "init":g.IsInit=false;break;case "dead":g.TLBB.PlayerState=2;break;case "leader":g.TLBB.IsLeader=false;break;case "team":g.TLBB.KeyId="other";break;case "jail":g.TLBB.MapId=194;break;}g.Step();Assert.False((bool)g.IsAcBa);Assert.Empty(Commands((object)g));}
        [Theory][InlineData("off")][InlineData("offline")][InlineData("init")][InlineData("dead")][InlineData("scene")][InlineData("otherteam")][InlineData("quest")][InlineData("identity")][InlineData("battle")]
        public void IneligibleMembersAreNotCommanded(string kind){dynamic g=AcBaHarness.Create(),m=AcBaHarness.Create();g.Choose(1);m.IsAcBa=false;m.TLBB.Id="member";m.TLBB.IsLeader=false;switch(kind){case "off":m.IsAuto=false;break;case "offline":m.TLBB.Online=false;break;case "init":m.IsInit=false;break;case "dead":m.TLBB.PlayerState=2;break;case "scene":m.ON_SCENE_TRANSING=true;break;case "otherteam":m.TLBB.KeyId="other";break;case "quest":m.IsTrungAc=true;break;case "identity":m.TLBB.Id="";break;case "battle":m.IsHuyetChien=true;break;}g.Party.Add(m);g.Step();Assert.Empty(Commands((object)m));}
        [Fact] public void DuplicateMembersGetOneCommand(){dynamic g=AcBaHarness.Create(),m=AcBaHarness.Create();g.Choose(1);m.IsAcBa=false;m.TLBB.Id="member";m.TLBB.IsLeader=false;g.Party.Add(m);g.Party.Add(m);g.Step();Assert.Single(Commands((object)m));}
        [Fact] public void ParentOffDuringMemberMovePreventsNextCommand(){dynamic g=AcBaHarness.Create(),m=AcBaHarness.Create(),n=AcBaHarness.Create();g.Choose(1);m.IsAcBa=n.IsAcBa=false;m.TLBB.Id="m";n.TLBB.Id="n";m.TLBB.IsLeader=n.TLBB.IsLeader=false;m.CancelController=g;g.Party.Add(m);g.Party.Add(n);g.Step();Assert.Empty(Commands((object)n));}
        [Fact] public void MonsterWithNpcNameNeverGetsTalkedTo(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=9;g.AddNpc(false);g.TryEntry();Assert.DoesNotContain(Commands((object)g),c=>c.StartsWith("Talk:"));}
        [Fact] public void NpcDialogTalkAndSelectOnlyOnce(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=9;g.AddNpc();g.TryEntry();g.TryEntry();Assert.Single(Commands((object)g),c=>c.StartsWith("Talk:"));g.SetDialog(50013,-1);g.TryEntry();g.TryEntry();Assert.Single(Commands((object)g),c=>c.StartsWith("Option:"));}
        [Fact] public void CancelDuringTalkCannotSelect(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=9;g.AddNpc();g.CancelOnTalk=true;g.TryEntry();g.SetDialog(50013,-1);g.TryEntry();Assert.DoesNotContain(Commands((object)g),c=>c.StartsWith("Option:"));}
        [Fact] public void DialogWithoutMapTransitionTimesOut(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=9;g.AddNpc();g.TryEntry();g.SetDialog(50013,-1);g.TryEntry();g.Advance(60001);g.Step();Assert.False((bool)g.IsAcBa);}
        [Fact] public void BossRequiresSameLiveIdAndCorrectMap(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=173;g.SetBoss(1);g.Step();g.SetBoss(0,701);g.Step();Assert.Equal("Patrol",(string)g.Phase);g.SetBoss(0,700);g.Step();Assert.Equal("AwaitExit",(string)g.Phase);}
        [Theory][InlineData(float.NaN)][InlineData(float.PositiveInfinity)] public void InvalidBossHealthDoesNotProveDeath(float hp){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=173;g.SetBoss(hp);g.Step();Assert.Equal("Patrol",(string)g.Phase);}
        [Fact] public void MissingExitStopsWithoutRewardsOrMovement(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=173;g.SetBoss(1);g.Step();g.SetBoss(0);g.Step();g.Commands.Clear();g.Advance(60001);g.Step();Assert.False((bool)g.IsAcBa);Assert.Empty(Commands((object)g));}
        [Fact] public void ActualExitEndsRunWithoutRepeatingOldEvent(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=173;g.SetBoss(1);g.Step();g.SetBoss(0);g.Step();g.TLBB.MapId=9;g.Step();Assert.False((bool)g.IsAcBa);Assert.Equal(-1,(int)g.AcBa);}
        [Fact] public void UnexpectedDepartureStops(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=173;g.Step();g.TLBB.MapId=9;g.Commands.Clear();g.Step();Assert.False((bool)g.IsAcBa);Assert.Empty(Commands((object)g));}
        [Fact] public void ReanchorUsesXAndYAndPreservesVisited(){dynamic g=AcBaHarness.Create();g.SavedRoute("95,105-69,79-80,120");g.Choose(1);g.TLBB.MapId=173;g.CharX=69;g.CharY=79;g.Step();g.Advance(2001);g.Step();Assert.True(((bool[])g.Visited)[1]);Assert.Contains("Move:80:120",Commands((object)g));}
        [Fact] public void NegativeSessionIndexStopsWithoutException(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=173;g.Step();g.Advance(2001);g.CorruptIndex(-2);g.Step();Assert.False((bool)g.IsAcBa);}
        [Fact] public void RouteEndDoesNotInferBossDeath(){dynamic g=AcBaHarness.Create();g.SavedRoute("1,1-2,2");g.Choose(1);g.TLBB.MapId=173;g.CharX=1;g.CharY=1;g.Step();g.Advance(2001);g.Step();g.CharX=2;g.CharY=2;g.Step();Assert.Equal("AwaitBoss",(string)g.Phase);Assert.False((bool)g.IsBossDie);g.Advance(60001);g.Step();Assert.False((bool)g.IsAcBa);}
        [Fact] public void ReadErrorStopsAndDoesNotExposeMessage(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=9;g.AddNpc();g.TryEntry();g.SetDialog(50013,-1);g.ThrowRead=true;g.Step();Assert.False((bool)g.IsAcBa);Assert.Contains("InvalidOperationException",(string)g.Warning);Assert.DoesNotContain("SECRET",(string)g.Warning);}
        [Theory][InlineData("pause")][InlineData("scene")][InlineData("mapchange")][InlineData("combat")]
        public void PauseDoesNotConsumeTravelTimeout(string kind){dynamic g=AcBaHarness.Create();g.Choose(1);g.Step();g.Commands.Clear();g.Advance(10000);g.SetPause(kind,true);g.Step();g.Advance(200000);g.Step();Assert.Empty(Commands((object)g));g.SetPause(kind,false);g.Step();Assert.True((bool)g.IsAcBa);g.Advance(170001);g.Step();Assert.False((bool)g.IsAcBa);}
        [Fact] public void HandoffRequiresReadyMatchingMemberAndServerAck(){dynamic g=AcBaHarness.Create(),m=AcBaHarness.Create();g.Choose(5);m.IsAcBa=false;m.TLBB.Id=m.TLBB.Name="member";m.TLBB.Menpai=5;m.TLBB.IsLeader=false;g.Party.Add(m);g.Step();g.Step();Assert.Single(Commands((object)g),c=>c.StartsWith("Appoint:"));Assert.False((bool)m.IsAcBa);g.TLBB.IsLeader=false;g.TLBB.KeyId="member";m.TLBB.IsLeader=true;m.TLBB.KeyId="member";g.Step();Assert.False((bool)g.IsAcBa);Assert.True((bool)m.IsAcBa);m.Step();Assert.Equal(5,(int)m.Target);}
        [Fact] public void NoMatchingMemberWaitsBoundedlyWithoutTravel(){dynamic g=AcBaHarness.Create();g.Choose(5);g.Step();Assert.Empty(Commands((object)g));g.Advance(180001);g.Step();Assert.False((bool)g.IsAcBa);}
        [Fact] public void HandoffTimeoutDoesNotEnableMember(){dynamic g=AcBaHarness.Create(),m=AcBaHarness.Create();g.Choose(5);m.IsAcBa=false;m.TLBB.Id=m.TLBB.Name="member";m.TLBB.Menpai=5;m.TLBB.IsLeader=false;g.Party.Add(m);g.Step();g.Advance(60001);g.Step();Assert.False((bool)g.IsAcBa);Assert.False((bool)m.IsAcBa);Assert.Single(Commands((object)g));}
        [Fact] public void AutomaticWaitingHasTimeout(){dynamic g=AcBaHarness.Create();g.Step();g.Advance(600001);g.Step();Assert.False((bool)g.IsAcBa);Assert.Empty(Commands((object)g));}
        [Fact] public void GenericEngineIsBlockedForActiveTeam(){dynamic g=AcBaHarness.Create(),m=AcBaHarness.Create();m.IsAcBa=false;m.TLBB.Id="member";m.TLBB.IsLeader=false;m.Party.Add(g);Assert.True((bool)m.GenericBlocked());m.TLBB.KeyId="other";Assert.False((bool)m.GenericBlocked());}
        [Fact] public void AutoOffReleasesOnlyUnneededReceiveHook(){dynamic g=AcBaHarness.Create();g.SetHookRead(123,10);g.HookStep();g.Commands.Clear();g.IsAuto=false;g.AutoOffGuard();Assert.False((bool)g.IsHooked);Assert.Equal(new[]{"UnHook"},Commands((object)g));}
        [Fact] public void SameMapSceneTransitionInvalidatesOldBossEvidence(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=173;g.SetBoss(1);g.Step();g.ON_SCENE_TRANSING=true;g.Validate();g.SetBoss(0);g.ON_SCENE_TRANSING=false;g.Step();Assert.Equal("Patrol",(string)g.Phase);}
        [Fact] public void RepeatedLootCannotSuppressEntryTimeout(){dynamic g=AcBaHarness.Create();g.Choose(1);g.Loot=true;g.Step();g.Advance(180001);g.Step();Assert.False((bool)g.IsAcBa);}
        [Fact] public void ManualSelectionDoesNotRequireReceiveHook(){dynamic g=AcBaHarness.Create();g.Choose(1);Assert.False((bool)g.ShouldHook());}
        [Theory][InlineData(0,10)][InlineData(123,0)][InlineData(123,9)]
        public void InvalidRecvAddressOrShortReadDoesNotPostHook(int address,int read){dynamic g=AcBaHarness.Create();g.SetHookRead(address,read);g.HookStep();Assert.False((bool)g.IsHooked);Assert.Empty(Commands((object)g));}
        [Fact] public void FullRecvReadPostsExistingAbiOnce(){dynamic g=AcBaHarness.Create();g.SetHookRead(123,10);g.HookStep();g.HookStep();Assert.True((bool)g.IsHooked);Assert.Equal(new[]{"Post:-10"},Commands((object)g));}
        [Fact] public void HookFailureStopsAutomaticButManualStillWorks(){dynamic g=AcBaHarness.Create();g.ThrowHook=true;g.HookStep();Assert.False((bool)g.IsAcBa);g.Choose(1);g.HookStep();Assert.True((bool)g.IsAcBa);Assert.Empty(Commands((object)g));}
        [Fact] public void GenericPatrolDoesNotRestartAfterAcBaStopsInDungeon(){dynamic g=AcBaHarness.Create();g.IsAcBa=false;g.TLBB.MapId=173;Assert.True((bool)g.SkipGeneric());}
        [Fact] public void EndedTextOverridesEarlierAppearance(){dynamic h=AcBaHarness.Helpers();Assert.True((bool)h.Ended("Ác Bá xuất hiện tại Nga Mi nhưng đã bị tiêu diệt"));}
        [Fact] public void FragmentedWhitespaceDoesNotBreakKnownText(){dynamic h=AcBaHarness.Helpers();Assert.Equal(5,(int)h.Parse("Ác\tBá đã xuất\nhiện tại Nga\tMi"));}
        [Fact] public void UnavailableHandoffCandidateIsNeverEnabled(){dynamic g=AcBaHarness.Create(),m=AcBaHarness.Create();g.Choose(5);m.IsAcBa=false;m.IsAuto=false;m.TLBB.Id=m.TLBB.Name="member";m.TLBB.Menpai=5;m.TLBB.IsLeader=false;g.Party.Add(m);g.Step();Assert.Empty(Commands((object)g));Assert.False((bool)m.IsAcBa);}
        [Fact] public void ChangedHandoffCandidateCancelsWithoutTransfer(){dynamic g=AcBaHarness.Create(),m=AcBaHarness.Create();g.Choose(5);m.IsAcBa=false;m.TLBB.Id=m.TLBB.Name="member";m.TLBB.Menpai=5;m.TLBB.IsLeader=false;g.Party.Add(m);g.Step();m.TLBB.Id="new";g.Step();Assert.False((bool)g.IsAcBa);Assert.False((bool)m.IsAcBa);}
        [Fact] public void ChangedNpcIdStopsBeforeDialogSelection(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=9;g.AddNpc();g.TryEntry();g.Objects.All[0].Id=101;g.SetDialog(50013,-1);g.TryEntry();Assert.False((bool)g.IsAcBa);Assert.DoesNotContain(Commands((object)g),c=>c.StartsWith("Option:"));}
        [Fact] public void StaleDialogClosesOnlyOnceAndIsNotSelected(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=9;g.AddNpc();g.SetDialog(50013,-1);g.KeepOpen=true;g.TryEntry();g.TryEntry();Assert.Equal(new[]{"Close"},Commands((object)g));}
        [Fact] public void CancelDuringOptionDoesNotCloseAnotherDialog(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=9;g.AddNpc();g.TryEntry();g.SetDialog(50013,-1);g.Commands.Clear();g.CancelOnSelect=true;g.TryEntry();Assert.Equal(new[]{"Option:50013:-1"},Commands((object)g));}
        [Fact] public void AutoOffTrackedMemberStopsDungeonRun(){dynamic g=AcBaHarness.Create(),m=AcBaHarness.Create();g.Choose(1);m.IsAcBa=false;m.TLBB.Id="m";m.TLBB.IsLeader=false;g.TLBB.MapId=m.TLBB.MapId=173;g.Party.Add(m);g.Step();m.IsAuto=false;g.Step();Assert.False((bool)g.IsAcBa);}
        [Fact] public void NonFiniteCoordinatesCannotMove(){dynamic g=AcBaHarness.Create();g.Choose(1);g.TLBB.MapId=173;g.CharX=float.NaN;g.Step();g.Advance(2001);g.Step();Assert.False((bool)g.IsAcBa);Assert.DoesNotContain(Commands((object)g),c=>c.StartsWith("Move:"));}

    }

    internal static class AcBaHarness
    {
        private static readonly Lazy<Assembly> Compiled = new Lazy<Assembly>(Compile);
        internal static dynamic Create()=>Activator.CreateInstance(Compiled.Value.GetType("TinhKiemAuto.Game"));
        internal static dynamic Helpers()=>Activator.CreateInstance(Compiled.Value.GetType("TinhKiemAuto.EventTests"));
        private static string Read(string path)=>File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"RegressionSources",path));
        private static ClassDeclarationSyntax Class(string path,string name)=>CSharpSyntaxTree.ParseText(Read(path)).GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single(c=>c.Identifier.Text==name);
        private static Assembly Compile()
        {
            bool baseline=Environment.GetEnvironmentVariable("AC_BA_BASELINE")=="1";
            var game=Class(baseline?"AcBaLegacyGame.cs":"AcBaReviewedGame.cs","Game");
            string[] selected=baseline?new[]{"DatDoiAcBa","TalkNPCPhuBan","TrieuTap","ClearMission"}:new[]{"ClearMission"};
            string methods=string.Join("\n",game.Members.OfType<MethodDeclarationSyntax>().Where(m=>selected.Contains(m.Identifier.Text)).Select(m=>m.ToFullString()));
            methods+=string.Join("\n",game.Members.OfType<PropertyDeclarationSyntax>().Where(p=>new[]{"AcBaPoint","IsBusy"}.Contains(p.Identifier.Text)).Select(p=>p.ToFullString()));
            string off=game.Members.OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="Auto").DescendantNodes().OfType<IfStatementSyntax>().Single(i=>i.Condition.ToString()=="!IsAuto").ToFullString();
            string hook=game.Members.OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="Auto").DescendantNodes().OfType<IfStatementSyntax>().Single(i=>i.Condition.ToString().StartsWith("!IsHooked &&")).Condition.ToString();
            var autoMethod=game.Members.OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="Auto");
            string hookBody=autoMethod.DescendantNodes().OfType<IfStatementSyntax>().Single(i=>i.Condition.ToString().StartsWith("!IsHooked &&")).ToFullString();
            var generic=game.Members.OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="AcTac").Body.Statements.OfType<IfStatementSyntax>().First().Condition.ToString();
            string control=baseline?"public bool IsAcBa; public void Choose(int s){AcBa=s;IsAcBa=true;} public string Phase=>IsBossDie?\"AwaitExit\":\"Patrol\"; public void TryEntry()=>TalkNPCPhuBan();":
                "public void Choose(int s)=>SelectManualAcBaSchool(s);public string Phase=>acBaPhase.ToString();public int Target=>acBaSchool;public void TryEntry()=>DatDoiAcBa();public void Validate()=>CheckAcBaSession();public void CorruptIndex(int i){CheckAcBaSession();SelectAcBaTarget();acBaIndex=i;acBaReanchor=false;}public bool GenericBlocked()=>AcBaControlsCurrentActor();public bool[] Visited=>acBaVisited;";
            var points=Class("POINT.cs","POINT");var maps=Class("MAP.cs","MAP");var tk=Class("TINHKIEM.cs","TINHKIEM");var conv=Class("ConverterEx.cs","ConverterEx");
            string pointFields=string.Join("\n",points.Members.OfType<FieldDeclarationSyntax>().Select(f=>f.ToFullString()));
            string mapFields=string.Join("\n",maps.Members.OfType<FieldDeclarationSyntax>().Where(f=>f.Declaration.Type.ToString()=="int").Select(f=>f.ToFullString()));
            string tkMethods=string.Join("\n",tk.Members.OfType<MethodDeclarationSyntax>().Where(m=>new[]{"VietLien","ClearSign","GetDistance","ParseInt"}.Contains(m.Identifier.Text)&& (m.Identifier.Text!="ParseInt"||m.ParameterList.Parameters.Count==1)).Select(m=>m.ToFullString()));
            string signs=tk.Members.OfType<FieldDeclarationSyntax>().Single(f=>f.Declaration.Variables.Any(v=>v.Identifier.Text=="vietnameseSigns")).ToFullString();
            string codes=conv.Members.OfType<FieldDeclarationSyntax>().Single(f=>f.Declaration.Variables.Any(v=>v.Identifier.Text=="Unicodes")).ToFullString()+conv.Members.OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="VISCII2UnicodeEx").ToFullString();
            string source="using System;using System.Collections.Generic;using System.Text;using System.Text.RegularExpressions;using System.Linq;using Process=TinhKiemAuto.FakeProcess;using Stopwatch=TinhKiemAuto.FakeStopwatch;namespace TinhKiemAuto.Models {} namespace TinhKiemAuto {"+Fixture+
                "public static class POINT {"+pointFields+"} public static class MAP {"+mapFields+"} public static class TINHKIEM {"+signs+tkMethods+"} public static class ConverterEx {"+codes+"} "+EventFixture+
                "public partial class Game {"+Fields+methods+control+"public void AutoOffGuard(){"+off+"} public bool ShouldHook()=>"+hook+";public void HookStep(){"+hookBody+"} public bool SkipGeneric()=>"+generic+";} }";
            var trees=new List<SyntaxTree>{CSharpSyntaxTree.ParseText(source),CSharpSyntaxTree.ParseText(Read("AcBaEvents.cs"))};
            if(!baseline)foreach(var path in new[]{"Game.AcBa.cs","Game.AcBaEvents.cs"})trees.Add(CSharpSyntaxTree.ParseText(Read(path).Replace("using System.Diagnostics;","using Stopwatch=TinhKiemAuto.FakeStopwatch;")));
            var refs=((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator).Select(p=>MetadataReference.CreateFromFile(p));
            var compilation=CSharpCompilation.Create("AcBaSimulation",trees,refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var bytes=new MemoryStream();var result=compilation.Emit(bytes);Assert.True(result.Success,string.Join("\n",result.Diagnostics.Where(d=>d.Severity==DiagnosticSeverity.Error)));return Assembly.Load(bytes.ToArray());
        }
        private const string EventFixture="""
            public class EventTests {
                public int Parse(string text){int school;bool ended;return AcBaEvents.TryParse(text,out school,out ended)?school:-1;}
                public bool Ended(string text){int school;bool ended;return AcBaEvents.TryParse(text,out school,out ended)&&ended;}
                public int[,] Route(string saved,int school)=>AcBaEvents.ReadRoute(saved,school);
                public int Outside(int school)=>AcBaEvents.OutdoorMap(school);public int Inside(int school)=>AcBaEvents.DungeonMap(school);
                public byte[] Packet(string text,string codec="VISCII",int channel=4){byte[] body=codec=="UTF-8"?System.Text.Encoding.UTF8.GetBytes(text):text.Select(c=>(byte)Array.IndexOf(ConverterEx.Unicodes,c)).ToArray();byte[] b=new byte[15+body.Length];b[4]=218;b[5]=3;b[10]=(byte)channel;Array.Copy(body,0,b,15,body.Length);return b;}
                public string Decode(byte[] b,string codec){string text;return AcBaEvents.TryDecodeSystem(b,codec,out text)?text:null;}
            }
            """;
        private const string Fixture="""
            public class FakeStopwatch {public static long Now;private long start=Now;public static FakeStopwatch StartNew()=>new FakeStopwatch();public long ElapsedMilliseconds=>Now-start;public TimeSpan Elapsed=>TimeSpan.FromMilliseconds(ElapsedMilliseconds);}
            public class FakeRandom {private static int next;public int Next(int min,int max)=>min+next++%(max-min);}
            public class TLBB {public string Id="leader",KeyId="leader";public string Name="leader";public int MapId=1,PlayerState,Menpai=1,OnlineTimeSec=4;public int MapAcBa=>AcBaEvents.DungeonMap(Menpai);public int MapMonPhai=>AcBaEvents.OutdoorMap(Menpai);public bool IsLeader=true,Online=true,IsFollow,HaveRide,IsQuestOpen;}
            public class GameObject {public int Id=100;public string Name="Giang ho tieu tieu",Title="";public bool IsNPC=true;public float X,Y,HP=1;public string CleanName=>TINHKIEM.VietLien(Name);}
            public class ObjectsFixture {public List<GameObject> All=new List<GameObject>(),NearMonter18m=new List<GameObject>(),NearMonter20m=new List<GameObject>(),NearMonter12m=new List<GameObject>();public List<GameObject> NearMonter(float x,float y,float r)=>NearMonter20m;}
            public class QuestFrame {private Game owner;public QuestFrame(){}public QuestFrame(Game g){owner=g;}public void ClickAll(){owner.Commands.Add("ClickAll");}public int StrOptionExtra1,StrOptionExtra2;public string Name="";public static List<QuestFrame> Enum(Game g){if(g.ThrowRead)throw new InvalidOperationException("SECRET");return g.Dialog;}}
            public static class Global {public static bool Paused,IsAcTac;}
            public static class CanhBao {public enum Kieu {Eror}public static string Last="";public static void Msg(string title,string text,Kieu kind){Last=text;}}
            public class FakeProcess {public static FakeProcess GetProcessById(int id)=>new FakeProcess();}
            public static class Memory {public static IntPtr Id;public static int ReadCount=10;public static bool ReadProcessMemory(IntPtr id,int address,byte[] buffer,int size,out int read){read=ReadCount;return true;}public static bool ReadProcessMemory(IntPtr id,int address,byte[] buffer,int size,int zero)=>true;}
            public static class Setting {public static string Route="";public static string LoadMAP(string map)=>Route;}
            public static class Option {public static bool AlarmChat;}
            public class FrmMain {public static bool AlarmAcBa;public static Dictionary<int,Game> dicGame=new Dictionary<int,Game>();}
""";
        private const string Fields="""
            public TLBB TLBB=new TLBB();public ObjectsFixture Objects=new ObjectsFixture();public List<Game> Party=new List<Game>();public List<QuestFrame> Dialog=new List<QuestFrame>();public List<string> Commands=new List<string>();
            public bool IsAuto=true,IsInit=true,IsRide,ON_SCENE_TRANSING,IsChangeMap,IsBossDie,IsTrieuTap,IsTheoQ,IsP,TraQ,NhanQ,IsClick,IsContinute,IsNhamBinhSinhDie,Talked,ThrowRead,KeepOpen,CancelOnTalk,CancelOnSelect;
            public bool IsThuyLao,IsKyCuoc,IsTrungAc,IsLauLanTamBao,IsQ123LauLan,IsQ123ToChau,IsYenTuO,IsPhungHoangLangMo,IsPMP,IsTuBaoBon,IsLuyenKim,IsHuyetChien,IsBachHoaDuyen,IsSuMon,IsXayDung,IsTuDuong,IsNhiemVuCoBan;
            public int ProcessId=100,RecvAddress,AcBa=-1,MapAcTac,MapTKC,MoveIndex,MapATIndex=-1,CurMapATIndex=-1;private int mapAcTac;public float CharX,CharY,RoundX,RoundY;public static int TickCount=9;public Game CancelController;
            public FakeStopwatch BossDieTime=FakeStopwatch.StartNew();public bool IsAlarmAcBa,IsHooked,AlarmChat;public FakeStopwatch tranTime=FakeStopwatch.StartNew(),ClearTime=FakeStopwatch.StartNew(),TimeStand=FakeStopwatch.StartNew();public QuestFrame QuestFrame;
            public Game(){Global.Paused=Global.IsAcTac=false;FakeStopwatch.Now+=3000;QuestFrame=new QuestFrame(this);MapAcTac=0;IsAcBa=true;Setting.Route="95,105-69,79";CanhBao.Last="";}
            public bool Busy=>IsBusy;public int LegacyIndex{set=>MapATIndex=value;}public bool IsMoveEx=>true;public string Warning=>CanhBao.Last;
            public void Step()=>DatDoiAcBa();public void Advance(long ms){FakeStopwatch.Now+=ms;}
            public void Pause(bool on){Global.Paused=on;}public void SetPause(string kind,bool on){if(kind=="pause")Global.Paused=on;if(kind=="scene")ON_SCENE_TRANSING=on;if(kind=="mapchange")IsChangeMap=on;if(kind=="combat")TLBB.PlayerState=on?7:0;}
            public void AddNpc(bool npc=true){Objects.All.Add(new GameObject{IsNPC=npc});}public void SetDialog(int a,int b){TLBB.IsQuestOpen=true;Dialog.Clear();Dialog.Add(new QuestFrame{StrOptionExtra1=a,StrOptionExtra2=b});}
            public GameObject Make272()=>new GameObject{Name="n Du V"+new string('x',18)};
            public void SetBoss(float hp,int id=700){Objects.All.RemoveAll(o=>o.Name=="acba");Objects.All.Add(new GameObject{Name="acba",IsNPC=false,HP=hp,Id=id});}
            public bool GoTo(float x,float y,int map){Commands.Add($"Goto:{map}:{x}:{y}");if(CancelController!=null)CancelController.IsAcBa=false;return false;}
            public void Move(float x,float y){Commands.Add($"Move:{x}:{y}");}public void Talk(int id){Commands.Add("Talk:"+id);if(CancelOnTalk)IsAcBa=false;}
            public void QuestFrameOptionClicked(QuestFrame f){Commands.Add($"Option:{f.StrOptionExtra1}:{f.StrOptionExtra2}");if(CancelOnSelect)IsAcBa=false;}
            public void CloseQuest(){Commands.Add("Close");if(!KeepOpen)TLBB.IsQuestOpen=false;}public bool Loot;public bool PickItem()=>Loot;
            public void StopFollow(){Commands.Add("StopFollow");TLBB.IsFollow=false;}public void DownRide(){Commands.Add("DownRide");IsRide=false;}public void UpRide(){Commands.Add("UpRide");IsRide=true;}
            public void AskTeamFollow(){Commands.Add("FollowAll");}public void Ride(){Commands.Add("Ride");}public void MoveNext(){Commands.Add("LegacyMoveNext");}public void FixKetMap(){Commands.Add("FixKet");}
            public bool IsMapPhuBan()=>TLBB!=null&&(TLBB.MapId==80||TLBB.MapId==170|| (TLBB.MapId>=173&&TLBB.MapId<=181)||TLBB.MapId==288||TLBB.MapId==618);public bool CanActThuyLao()=>false;public void RunThuyLaoMember(Game m){}public void StopThuyLao(string s){}public void ResetThuyLaoProgress(){}

            public bool IsMapAcBa=>TLBB!=null&&Array.IndexOf(new[]{173,174,175,176,177,178,179,180,181,288,618},TLBB.MapId)>=0;
            private byte[] bufferRecv=new byte[10];public bool ThrowHook;private int HookAddress=123;
            public void SetHookRead(int address,int read){HookAddress=address;Memory.ReadCount=read;}
            public IntPtr GetRemoteProcAddress(FakeProcess p,string module,string name){if(ThrowHook)throw new InvalidOperationException("SECRET");return new IntPtr(HookAddress);}
            public void PostMessage(int pid,int command){Commands.Add("Post:"+command);}public void UnHookRecv(){IsHooked=false;Commands.Add("UnHook");}
            public void AppointLeader(string name){Commands.Add("Appoint:"+name);}
            public void SavedRoute(string route){Setting.Route=route;}
""";
    }
}
