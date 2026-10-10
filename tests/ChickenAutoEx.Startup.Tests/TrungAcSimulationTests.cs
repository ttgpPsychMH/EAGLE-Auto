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
    public sealed class TrungAcSimulationTests
    {
        [Fact, Trait("TrungAcBaseline", "yes")]
        public void EmptyDialogDoesNotThrow()
        { dynamic g=TrungAcHarness.Create();g.PrepareInfo();Assert.Null(Record.Exception(()=>{g.Step();})); }
        [Fact, Trait("TrungAcBaseline", "yes")]
        public void MissingYDoesNotThrow()
        { dynamic g=TrungAcHarness.Create();g.PrepareInfo();g.SetDialog("tochau [123]");Assert.Null(Record.Exception(()=>{g.Step();})); }
        [Fact, Trait("TrungAcBaseline", "yes")]
        public void DisabledTaskNeverAbandonsQuestFromUiAlarm()
        { dynamic g=TrungAcHarness.Create();g.IsTrungAc=false;g.SetOldTimers();g.Alarm();Assert.Empty((List<string>)g.Commands); }
        [Fact, Trait("TrungAcBaseline", "yes")]
        public void NewRunClearsOldCompletion()
        { dynamic g=TrungAcHarness.Create();g.IsTrungAc=false;g.IsXongTrungAc=true;g.IsTrungAc=true;Assert.False((bool)g.IsXongTrungAc); }
        [Fact, Trait("TrungAcBaseline", "yes")]
        public void DelayedAcceptDialogKeepsWaitingState()
        { dynamic g=TrungAcHarness.Create();g.PrepareAccept();g.Step();Assert.True((bool)g.WaitingAccept); }
        [Fact, Trait("TrungAcBaseline", "yes")]
        public void CorpseAloneDoesNotTriggerQuestReturn()
        { dynamic g=TrungAcHarness.Create();g.PrepareCombat(1);g.AddMonster(10,0,0,0);g.Step();Assert.False((bool)g.Returning); }
        [Fact, Trait("TrungAcBaseline", "yes")]
        public void MultipleLiveTargetsReceiveOneStableSelection()
        { dynamic g=TrungAcHarness.Create();g.PrepareCombat(3);g.AddMonster(10,2,0,1);g.AddMonster(11,8,0,1);g.Step();Assert.Single((List<string>)g.Commands,c=>c.StartsWith("Select:")); }
        [Fact, Trait("TrungAcBaseline", "yes")]
        public void SpawnUsesTokenOnceThenWaits()
        { dynamic g=TrungAcHarness.Create();g.PrepareSpawn();g.Step();g.Step();Assert.Single((List<string>)g.Commands,c=>c.StartsWith("Use:")); }
        [Fact, Trait("TrungAcBaseline", "yes")]
        public void ActiveTaskBlocksCalendarInOrdinaryMap()
        { dynamic g=TrungAcHarness.Create();Assert.True((bool)g.Busy); }
        [Fact, Trait("TrungAcBaseline", "yes")]
        public void ClearMissionCancelsTask()
        { dynamic g=TrungAcHarness.Create();g.ClearMission();Assert.False((bool)g.IsTrungAc); }
        [Fact, Trait("TrungAcBaseline", "yes")]
        public void LoginDoesNotRestartTaskAfterManualOff()
        { dynamic g=TrungAcHarness.Create();g.LoginTick();g.IsTrungAc=false;g.LoginTick();Assert.False((bool)g.IsTrungAc); }

        [Theory]
        [InlineData("")][InlineData("tochau")][InlineData("tochau [12]")][InlineData("tochau ]12,34[")]
        [InlineData("tochau [abc,xyz]")][InlineData("tochau [-1,2]")][InlineData("tochau [999999999999,1]")]
        [InlineData("tochau [1,2,3]")][InlineData("tochau [1,2] [3,4]")][InlineData("unknown [1,2]")]
        [InlineData("tochau daily [1,2]")][InlineData("tochau [4096,1]")]
        public void MalformedInfoNeverMovesOrCompletes(string info)
        {
            dynamic g=TrungAcHarness.Create();g.PrepareInfo();g.SetDialog(info);g.Step();
            Assert.Equal("Info",(string)g.Phase);Assert.Empty((List<string>)g.Commands);Assert.False((bool)g.IsXongTrungAc);
            g.Advance(60001);g.Step();Assert.False((bool)g.IsTrungAc);Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Move:")||c.StartsWith("Lua:"));
        }
        [Theory][InlineData("tochau [123,234]",1)][InlineData("caoxuongmecung [123,234]",252)]
        [InlineData("thanhnguyensondong [123,234]",283)][InlineData("lacduong [123,234]",0)]
        [InlineData("Tô Châu [123,234]",1)][InlineData("Cao Xương Mê Cung [123,234]",252)]
        [InlineData("Thanh Nguyên Sơn Động [123,234]",283)][InlineData("Lạc Dương [123,234]",0)]
        public void DestinationUsesStrictCoordinatesAndSpecificMap(string info,int map)
        {
            dynamic g=TrungAcHarness.Create();g.PrepareInfo();g.SetDialog(info);g.Step();
            Assert.Equal(map,(int)g.MissionMap);Assert.Equal(123,(int)g.MissionX);Assert.Equal(234,(int)g.MissionY);
            Assert.Equal("TravelTarget",(string)g.Phase);
        }
        [Theory][InlineData("off")][InlineData("offline")][InlineData("null")][InlineData("notinit")]
        [InlineData("dead")][InlineData("dead9")][InlineData("jail")][InlineData("identity")][InlineData("follow")]
        [InlineData("thuy")][InlineData("ky")][InlineData("actac")][InlineData("bhd")][InlineData("sumon")]
        public void LostControlOrConflictingMissionStopsBeforeCommands(string fault)
        {
            dynamic g=TrungAcHarness.Create();g.Step();g.Commands.Clear();
            switch(fault){case "off":g.IsAuto=false;break;case "offline":g.TLBB.Online=false;break;
                case "null":g.TLBB=null;break;case "notinit":g.IsInit=false;break;case "dead":g.TLBB.PlayerState=2;break;
                case "dead9":g.TLBB.PlayerState=9;break;case "jail":g.TLBB.MapId=77;break;case "identity":g.TLBB.Id="other";break;
                case "follow":g.TLBB.IsFollow=true;break;case "thuy":g.IsThuyLao=true;break;case "ky":g.IsKyCuoc=true;break;
                case "actac":g.MapAcTac=3;break;case "bhd":g.IsBachHoaDuyen=true;break;case "sumon":g.IsSuMon=true;break;}
            g.Step();Assert.False((bool)g.IsTrungAc);Assert.Empty((List<string>)g.Commands);
        }
        [Theory][InlineData("pause")][InlineData("scene")][InlineData("mapchange")][InlineData("attacking")]
        public void TemporaryPauseDoesNotSendCommandsOrConsumeTimeout(string pause)
        {
            dynamic g=TrungAcHarness.Create();g.PrepareInfo();g.Step();g.SetPause(pause,true);g.Step();g.Advance(200000);g.Step();
            Assert.True((bool)g.IsTrungAc);Assert.Empty((List<string>)g.Commands);
            g.SetPause(pause,false);g.Step();Assert.True((bool)g.IsTrungAc);
            g.Advance(60001);g.Step();Assert.False((bool)g.IsTrungAc);
        }
        [Fact] public void OldSharedTimersCannotCauseQuestAbandonEvenWhileEnabled()
        { dynamic g=TrungAcHarness.Create();g.SetOldTimers();g.Alarm();Assert.Empty((List<string>)g.Commands); }
        [Fact] public void StaleInfoDialogIsClosedOnceBeforeUsingToken()
        {
            dynamic g=TrungAcHarness.Create();g.AddItem();g.TLBB.IsQuestOpen=true;g.KeepDialogOpen=true;
            g.Step();g.Step();Assert.Single((List<string>)g.Commands,c=>c=="Close");Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Use:"));
            g.Advance(60001);g.Step();Assert.False((bool)g.IsTrungAc);
        }
        [Fact] public void InfoUseWaitsForDialogInsteadOfRetryingItem()
        {
            dynamic g=TrungAcHarness.Create();g.AddTask(false);g.AddItem();g.Step();g.Step();g.Step();
            Assert.Single((List<string>)g.Commands,c=>c.StartsWith("Use:"));Assert.Equal("Info",(string)g.Phase);
        }
        [Fact] public void TravelWithoutRouteTimesOutAndKeepsQuest()
        {
            dynamic g=TrungAcHarness.Create();g.PrepareInfo();g.SetDialog("tochau [123,234]");g.Step();g.TLBB.MapId=3;g.Commands.Clear();
            g.Step();g.Advance(180001);g.Step();Assert.False((bool)g.IsTrungAc);Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Lua:")||c=="OK");
        }
        [Fact] public void SteadyTravelProgressRefreshesStuckTimeout()
        {
            dynamic g=TrungAcHarness.Create();g.PrepareInfo();g.SetDialog("tochau [123,234]");g.Step();g.TLBB.MapId=1;g.CharX=0;g.CharY=234;
            g.Step();g.Advance(100000);g.CharX=20;g.Step();g.Advance(100000);g.CharX=40;g.Step();Assert.True((bool)g.IsTrungAc);
        }
        [Theory][InlineData("missing")][InlineData("wrongname")][InlineData("wrongposition")]
        public void NpcMustMatchLegacyIdentityAndPosition(string fault)
        {
            dynamic g=TrungAcHarness.Create();g.AtNgoGioi();g.Step();
            if(fault=="missing")g.Objects.AllNpc.Clear();if(fault=="wrongname")g.Objects.AllNpc[0].Name="other";
            if(fault=="wrongposition")g.Objects.AllNpc[0].X=100;
            g.Step();Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Talk:"));g.Advance(60001);g.Step();Assert.False((bool)g.IsTrungAc);
        }
        [Fact] public void NpcTalkAndAcceptOptionAreSentOnceThenAwaitRealTaskAndItem()
        {
            dynamic g=TrungAcHarness.Create();g.AtNgoGioi();g.Step();g.Step();g.Step();
            Assert.Single((List<string>)g.Commands,c=>c.StartsWith("Talk:"));
            g.SetDialog("#{CXDT_090304_01}");g.Step();g.Step();g.Step();
            Assert.Single((List<string>)g.Commands,c=>c.StartsWith("Option:"));Assert.Equal("AcceptAck",(string)g.Phase);
            g.AddTask(false);g.AddItem();g.Step();Assert.Equal("Scan",(string)g.Phase);
        }
        [Fact] public void WrongNpcContextAfterTalkingStops()
        { dynamic g=TrungAcHarness.Create();g.PrepareAccept();g.TLBB.MapId=3;g.SetDialog("#{CXDT_090304_01}");g.Step();Assert.False((bool)g.IsTrungAc);Assert.Empty((List<string>)g.Commands); }
        [Fact] public void RemainingTokenCannotCauseRepeatedSpawnUse()
        { dynamic g=TrungAcHarness.Create();g.PrepareSpawn();g.Step();g.Advance(1000);g.Step();g.Advance(180001);g.Step();Assert.Single((List<string>)g.Commands,c=>c.StartsWith("Use:"));Assert.False((bool)g.IsTrungAc); }
        [Fact] public void PreexistingMonsterIsNotMistakenForNewQuestSpawn()
        { dynamic g=TrungAcHarness.Create();g.PrepareSpawn();g.AddMonster(10,1,0,1);g.Step();g.Step();Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Select:")); }
        [Fact] public void NewSpawnCanBeTargetedWhileTokenSnapshotRemains()
        { dynamic g=TrungAcHarness.Create();g.PrepareSpawn();g.Step();g.AddMonster(10,1,0,1);g.Step();Assert.Contains("Select:10",(List<string>)g.Commands);Assert.Single((List<string>)g.Commands,c=>c.StartsWith("Use:")); }
        [Fact] public void TargetIsStableWhenAnotherMonsterBecomesCloser()
        {
            dynamic g=TrungAcHarness.Create();g.PrepareCombat(3);g.AddMonster(10,2,0,1);g.AddMonster(11,8,0,1);g.Step();
            g.Objects.Near20m[1].X=1;g.Commands.Clear();g.Step();Assert.Contains("Select:10",(List<string>)g.Commands);Assert.DoesNotContain("Select:11",(List<string>)g.Commands);
        }
        [Theory][InlineData("npc")][InlineData("dead")][InlineData("type")][InlineData("level")][InlineData("far")]
        [InlineData("nan")][InlineData("infinite")][InlineData("title")]
        public void InvalidTargetNeverReceivesSkill(string fault)
        {
            dynamic g=TrungAcHarness.Create();g.PrepareCombat(fault=="title"?20:3);g.AddMonster(10,1,0,1);dynamic m=g.Objects.Near20m[0];
            switch(fault){case "npc":m.IsNPC=true;break;case "dead":m.HP=0;break;case "type":m.Menpai=27;break;
                case "level":m.Lvl=200;break;case "far":m.X=100;break;case "nan":m.X=float.NaN;break;
                case "infinite":m.HP=float.PositiveInfinity;break;case "title":m.Title="";break;}
            g.Step();Assert.Empty((List<string>)g.Commands);Assert.False((bool)g.IsXongTrungAc);
        }
        [Fact] public void ReadFailureStopsWithTypeOnly()
        { dynamic g=TrungAcHarness.Create();g.ThrowRead=true;g.Step();Assert.False((bool)g.IsTrungAc);Assert.Contains("InvalidOperationException",(string)g.LastWarning);Assert.DoesNotContain("SECRET",(string)g.LastWarning); }
        [Fact] public void MissingItemWithActiveQuestStopsWithoutAbandoning()
        { dynamic g=TrungAcHarness.Create();g.AddTask(false);g.Step();Assert.False((bool)g.IsTrungAc);Assert.Empty((List<string>)g.Commands); }
        [Fact] public void ServerCompletedQuestOverridesCombatAndTravelsToReturnNpc()
        { dynamic g=TrungAcHarness.Create();g.PrepareCombat(3);g.AddTask(true);g.AddMonster(10,1,0,1);g.Step();Assert.True((bool)g.Returning);Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Select:")); }
        [Fact] public void CompleteCommandDoesNotMeanRewardAcknowledged()
        {
            dynamic g=TrungAcHarness.Create();g.PrepareReturn();g.Step();g.Step();g.Step();g.Step();
            Assert.Single((List<string>)g.Commands,c=>c=="Post:15:105");Assert.Equal("ReturnAck",(string)g.Phase);
            Assert.False((bool)g.IsXongTrungAc);g.Advance(60001);g.Step();Assert.False((bool)g.IsTrungAc);Assert.False((bool)g.IsXongTrungAc);
        }
        [Fact] public void ReturnRequiresTwoTaskAbsenceSnapshotsAndNextContextualLimitDialog()
        {
            dynamic g=TrungAcHarness.Create();g.PrepareReturn();g.Step();g.Step();g.Step();g.Tasks.Clear();g.Step();
            Assert.Equal("ReturnAck",(string)g.Phase);g.Step();Assert.Equal("Scan",(string)g.Phase);
            g.Step();g.Step();g.SetDialog("#{CXDY_090423_01}#{CXDY_090423_02}");g.Step();Assert.True((bool)g.IsTrungAc);
            g.Step();Assert.False((bool)g.IsTrungAc);Assert.True((bool)g.IsXongTrungAc);
        }
        [Fact] public void LimitTokensWithoutPriorDeliveryNeverClaimReward()
        { dynamic g=TrungAcHarness.Create();g.PrepareAccept();g.SetDialog("#{CXDY_090423_01}#{CXDY_090423_02}");g.Step();g.Step();Assert.False((bool)g.IsTrungAc);Assert.False((bool)g.IsXongTrungAc); }
        [Fact] public void QuestIdentityChangeStopsInsteadOfMixingCycles()
        { dynamic g=TrungAcHarness.Create();g.PrepareCombat(3);g.Step();g.Tasks[0].Id=99;g.Step();Assert.False((bool)g.IsTrungAc); }
        [Fact] public void AutoOffCallerCancelsSession()
        { dynamic g=TrungAcHarness.Create();g.IsAuto=false;g.AutoGuard();Assert.False((bool)g.IsTrungAc); }
        [Fact] public void ClearNhiemVuResetsLegacyAndSessionState()
        { dynamic g=TrungAcHarness.Create();g.PrepareCombat(3);g.SetOldTimers();g.ClearNhiemVu();g.Commands.Clear();Assert.False((bool)g.IsTrungAc);Assert.False((bool)g.IsXongTrungAc);Assert.False((bool)g.IsHong);g.Alarm();Assert.Empty((List<string>)g.Commands); }
        [Fact] public void AutoLoginDoesNotOverrideConflictOrAutoOff()
        { dynamic g=TrungAcHarness.Create();g.IsTrungAc=false;g.IsAuto=false;g.LoginTick();Assert.False((bool)g.IsTrungAc);g.IsAuto=true;g.IsKyCuoc=true;g.LoginTick();Assert.False((bool)g.IsTrungAc); }
        [Fact] public void LoginTracksLongOnlineSessionAndStartsAgainAfterReconnect()
        { dynamic g=TrungAcHarness.Create();g.LoginTick();g.IsTrungAc=false;g.TLBB.OnlineTimeSec=100;g.LoginTick();Assert.False((bool)g.IsTrungAc);g.TLBB.OnlineTimeSec=1;g.LoginTick();Assert.True((bool)g.IsTrungAc); }
        [Fact] public void NativeTaskReadExceptionCannotMarkTrungAcComplete()
        { dynamic g=TrungAcHarness.Create();g.PrepareReturn();g.ThrowRead=true;g.Step();Assert.False((bool)g.IsTrungAc);Assert.False((bool)g.IsXongTrungAc);Assert.Empty((List<string>)g.Commands); }

        [Fact] public void CombatHpProgressRefreshesStuckTimeout()
        { dynamic g=TrungAcHarness.Create();g.PrepareCombat(3);g.AddMonster(10,0,0,1);g.Step();g.Advance(100000);g.Objects.Near20m[0].HP=0.8f;g.Step();g.Advance(100000);g.Objects.Near20m[0].HP=0.6f;g.Step();Assert.True((bool)g.IsTrungAc); }
        [Fact] public void CancellationDuringTargetSelectionPreventsSkill()
        { dynamic g=TrungAcHarness.Create();g.PrepareCombat(3);g.AddMonster(10,0,0,1);g.CancelOnSelect=true;g.Step();Assert.False((bool)g.IsTrungAc);Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Skill:")); }

    }

    internal static class TrungAcHarness
    {
        private static readonly Lazy<Assembly> Compiled=new Lazy<Assembly>(Compile);
        public static dynamic Create()=>Activator.CreateInstance(Compiled.Value.GetType("TinhKiemAuto.Game"));
        private static Assembly Compile()
        {
            bool baseline=Environment.GetEnvironmentVariable("TRUNG_AC_BASELINE")=="1";
            string source=File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"RegressionSources",baseline?"LegacyGame.cs":"TrungAcReviewedGame.cs"));
            var game=CSharpSyntaxTree.ParseText(source).GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single(c=>c.Identifier.Text=="Game");
            string methods=string.Join("\n",game.Members.OfType<MethodDeclarationSyntax>().Where(m=>
                new[]{"ClearMission","ClearNhiemVu","QuestFrameMissionComplete"}.Contains(m.Identifier.Text)||baseline&&m.Identifier.Text=="TrungAc").Select(m=>m.ToFullString()));
            if(baseline) methods+="public bool IsTrungAc;";
            else methods+=game.Members.OfType<PropertyDeclarationSyntax>().Single(p=>p.Identifier.Text=="IsTrungAc").ToFullString();
            methods+=game.Members.OfType<PropertyDeclarationSyntax>().Single(p=>p.Identifier.Text=="IsBusy").ToFullString();
            string alarm=game.Members.OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="TheoDoiCanhBao").ToFullString();
            alarm=baseline?alarm.Substring(alarm.IndexOf("if (comeTime != null"),alarm.IndexOf("if (TLBB.Disconnected)")-alarm.IndexOf("if (comeTime != null")):"";
            string autoGuard=game.Members.OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="Auto").DescendantNodes().OfType<IfStatementSyntax>()
                .Single(i=>i.Condition.ToString()=="!IsAuto").ToFullString();
            string keyboard=File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"RegressionSources","TINHKIEM.cs"));
            var utility=CSharpSyntaxTree.ParseText(keyboard).GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single(c=>c.Identifier.Text=="TINHKIEM");
            string parsers=string.Join("\n",utility.Members.OfType<MethodDeclarationSyntax>().Where(m=>new[]{"GetMapId","GetTruyen","ParseInt","NumDiff","VietLien","ClearSign"}.Contains(m.Identifier.Text)).Select(m=>m.ToFullString()));
            parsers+=utility.Members.OfType<FieldDeclarationSyntax>().Single(f=>f.Declaration.Variables.Any(v=>v.Identifier.Text=="vietnameseSigns")).ToFullString();
            string state=File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"RegressionSources","STATE.cs"));
            string form=File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"RegressionSources",baseline?"LegacyFrmMain.cs":"FrmMain.cs"));
            string login=CSharpSyntaxTree.ParseText(form).GetRoot().DescendantNodes().OfType<IfStatementSyntax>()
                .Single(i=>i.Condition.ToString()==(baseline?"item.IsTrungAc && game.TLBB.OnlineTimeSec < 60":"item.IsTrungAc")).ToFullString();
            string controls=baseline?"public bool WaitingAccept=>State==STATE.TalkToAcceptMission; public bool Returning=>State==STATE.Done; public string Phase=>State.ToString(); public void PrepareInfo(){State=STATE.GetInfo;TLBB.IsQuestOpen=true;} public void PrepareAccept(){AtNgoGioi();State=STATE.TalkToAcceptMission;} public void PrepareCombat(int map){TLBB.MapId=MissionMap=map;State=STATE.Come;AddTask(false);} public void PrepareSpawn(){PrepareCombat(3);AddItem();}":
                "public bool WaitingAccept=>trungAcPhase==TrungAcPhase.AcceptDialog; public bool Returning=>trungAcPhase==TrungAcPhase.ReturnNpc; public string Phase=>trungAcPhase.ToString(); public void PrepareInfo(){SetTrungAcPhase(TrungAcPhase.Info);TLBB.IsQuestOpen=true;} public void PrepareAccept(){AtNgoGioi();trungAcNpcId=100;SetTrungAcPhase(TrungAcPhase.AcceptDialog);} public void PrepareCombat(int map){TLBB.MapId=MissionMap=map;SetTrungAcPhase(TrungAcPhase.Combat);AddTask(false);} public void PrepareSpawn(){PrepareCombat(3);SetTrungAcPhase(TrungAcPhase.TravelTarget);AddItem();} public void PrepareReturn(){AtNgoGioi();trungAcNpcId=100;AddTask(true);SetTrungAcPhase(TrungAcPhase.ReturnDialog);SetDialog(\"#{CXDT_090304_01}\");}";
            string fixture="using System; using System.Collections.Generic; using System.Text.RegularExpressions; using Stopwatch=TinhKiemAuto.FakeStopwatch; namespace TinhKiemAuto {"+Fixture+
                "public static class TINHKIEM { public static float GetDistance(float x,float y,float a,float b)=>(float)Math.Sqrt((x-a)*(x-a)+(y-b)*(y-b));"+parsers+"} public partial class Game {"+Fields+methods+controls+
                "public void Alarm(){"+alarm+"} public void AutoGuard(){"+autoGuard+"} public void LoginTick(){var item=new LoginItem();var game=this;"+login+"} } }";
            var trees=new List<SyntaxTree>{CSharpSyntaxTree.ParseText(fixture),CSharpSyntaxTree.ParseText(state)};
            if(!baseline)foreach(string name in new[]{"Game.TrungAc.cs","Game.TrungAcParser.cs"})trees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"RegressionSources",name)).Replace("using System.Diagnostics;","using Stopwatch=TinhKiemAuto.FakeStopwatch;")));
            var references=((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator).Select(p=>MetadataReference.CreateFromFile(p));
            var compilation=CSharpCompilation.Create("TrungAcSimulation",trees,references,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var output=new MemoryStream();var result=compilation.Emit(output);
            Assert.True(result.Success,string.Join("\n",result.Diagnostics.Where(d=>d.Severity==DiagnosticSeverity.Error)));
            return Assembly.Load(output.ToArray());
        }
        private const string Fixture="""
            public class FakeStopwatch { public static long Now; private long start=Now; public static FakeStopwatch StartNew()=>new FakeStopwatch();public long ElapsedMilliseconds=>Now-start;public TimeSpan Elapsed=>TimeSpan.FromMilliseconds(ElapsedMilliseconds); }
            public class TLBB { public int MapId=3,PlayerState,Lvl=72,OnlineTimeSec=10;public string Id="person"; public bool Online=true,IsQuestOpen,IsTogleMission,HaveRide,IsFollow; }
            public static class MAP { public const int GiamNguc=77,ThaoNguyen=20; }
            public static class LACDUONG { public const int Id=0; }
            public class GameObject { public int Id,Menpai=28,Lvl=72;public bool IsNPC; public float X,Y,HP=1;public string Name="",Title="quest"; }
            public class ObjectsFixture { public List<GameObject> Near20m=new List<GameObject>(),AllNpc=new List<GameObject>(); }
            public class Task { public int Id=5,Complete;public string Name="#{CXDT_090304_01}";public bool Completed;public static List<Task> Enum(Game g){if(g.ThrowRead)throw new InvalidOperationException("SECRET");return g.Tasks;} }
            public class PacketItem { public string Name="trungaclenh";public int Index=2,Count=1;public static List<PacketItem> Enum(Game g)=>g.Items; }
            public class QuestFrame {public string Name;public int StrOptionExtra1=1,StrOptionExtra2=-1;public static List<QuestFrame> Enum(Game g)=>g.Dialog;public static string All(Game g)=>string.Join("",g.Dialog.ConvertAll(f=>f.Name)); }
            public static class Global {public static bool Paused;public const int BaseSkill=1;}
            public static class CanhBao {public enum Kieu {Eror}public static string Warning="";public static void Msg(string title,string message,Kieu kind){Warning=message;} }
            public class LoginItem {public bool IsTrungAc=true;}
            """;
        private const string Fields="""
            public static int TickCount=12;public TLBB TLBB=new TLBB();public ObjectsFixture Objects=new ObjectsFixture();
            public List<Task> Tasks=new List<Task>();public List<PacketItem> Items=new List<PacketItem>();public List<QuestFrame> Dialog=new List<QuestFrame>();public List<string> Commands=new List<string>();
            public bool IsAuto=true,IsInit=true,IsRide,ON_SCENE_TRANSING,IsChangeMap,IsXongTrungAc,IsHong,IsBTDByLogin,ThrowRead,KeepDialogOpen,CancelOnSelect;
            public bool IsThuyLao,IsKyCuoc,IsAcBa,IsLauLanTamBao,IsQ123LauLan,IsQ123ToChau,IsYenTuO,IsPhungHoangLangMo,IsPMP,IsTuBaoBon,IsLuyenKim,IsHuyetChien;
            public bool IsBachHoaDuyen,IsSuMon,IsXayDung,IsTuDuong,IsNhiemVuCoBan;public int State,MissionMap,MissionX,MissionY,Extra1,MapAcTac,MapTKC;public float CharX,CharY;public string TrungAcInfo="";
            private Stopwatch comeTime,doneTime=Stopwatch.StartNew();public Stopwatch HongTrungAcTime;
            public Game(){Global.Paused=false;TickCount=12;IsTrungAc=true;CanhBao.Warning="";}
            public bool Busy=>IsBusy;public string LastWarning=>CanhBao.Warning;
            public void Step()=>TrungAc();public void Advance(long ms){FakeStopwatch.Now+=ms;}
            public void AddTask(bool complete){Tasks.Add(new Task{Completed=complete,Complete=complete?256:0});}
            public void AddItem(){Items.Add(new PacketItem());}
            public void AddMonster(int id,float x,float y,float hp){Objects.Near20m.Add(new GameObject{Id=id,X=x,Y=y,HP=hp});}
            public void AtNgoGioi(){TLBB.MapId=1;CharX=224;CharY=226;Objects.AllNpc.Add(new GameObject{Id=100,IsNPC=true,Name="ngogioi",X=224,Y=226});}
            public void SetDialog(string info){TLBB.IsQuestOpen=true;Dialog.Clear();Dialog.Add(new QuestFrame{Name=info});}
            public void SetOldTimers(){comeTime=Stopwatch.StartNew();HongTrungAcTime=Stopwatch.StartNew();IsHong=true;Advance(301000);}
            public void SetPause(string kind,bool value){if(kind=="pause")Global.Paused=value;if(kind=="scene")ON_SCENE_TRANSING=value;if(kind=="mapchange")IsChangeMap=value;if(kind=="attacking")TLBB.PlayerState=value?7:0;}
            public bool ForcePickItem()=>false;public bool DaDenNoi(int x,int y,int map)=>TLBB.MapId==map;public bool TrongPhamVi(int x,int y,int map)=>TLBB.MapId==map;
            public void PlayerPackageUseItem(int index){Commands.Add("Use:"+index);}
            public void Move(float x,float y){Commands.Add($"Move:{x}:{y}");}public void TimDuong(float x,float y,int map){Commands.Add($"Travel:{map}:{x}:{y}");}
            public void Talk(int id){Commands.Add("Talk:"+id);}public void QuestFrameOptionClicked(QuestFrame f){Commands.Add($"Option:{f.StrOptionExtra1}:{f.StrOptionExtra2}");}
            public void QuestFrameOptionClicked(int a,int b){Commands.Add($"Option:{a}:{b}");}public void CloseQuest(){Commands.Add("Close");if(!KeepDialogOpen)TLBB.IsQuestOpen=false;}
            public void PostMessage(int a,int b){Commands.Add($"Post:{a}:{b}");}public void SelectTarget(int id){Commands.Add("Select:"+id);if(CancelOnSelect)IsTrungAc=false;}public void SendKey(int key){Commands.Add("Skill:"+key);}
            public void DownRide(){Commands.Add("DownRide");IsRide=false;}public void MessageboxSelfOkClicked(){Commands.Add("OK");}
            public void LuaDoUnicodeString(string code){Commands.Add("Lua:"+code);}public void TogleMission(){TLBB.IsTogleMission=true;}
            public string LuaToString()=>"";public string LuaString()=>"";public bool IsMapPhuBan()=>false;public void ResetThuyLaoProgress(){}
            public string TrangThaiLuyenKim,TrangThaiSuMon,TrangThaiTuBaoBon,TrangThaiXayDung,TrangThaiTuDuong,TrangThaiQD;
            public bool IsNguyenVong,IsLPMH,IsNhanh,IsDauCo,DaNhanHoaHong,DaNhanHoaChung,IsKhoang,IsDuoc,IsTrongTrot,IsThuHoach;
            public bool IsVanMay,IsLyHoa,IsNguHanhPhap,OkNhanDa,IsTueHong,IsChucPhuc,IsNhatHop,BachHoaDuyenCompleted,IsCauOThuoc,IsDead,IsCheDo,IsNotClear;
            public bool IsNhanQuaBuiHoaHong,IsNhanHoaHongLo,NhanQuaHoaHongCompleted,IsNhatHopall,IsChayVong,IsNhatHopQDua,IsQDua,QDuaCompleted,IsMoBang;
            public void LuaDoOneLineString(string code){Commands.Add("Lua:"+code);} 
            """;
    }
}
