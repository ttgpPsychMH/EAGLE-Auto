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
    public sealed class AcTacSimulationTests
    {
        [Fact, Trait("AcTacBaseline","yes")] public void SelectedMapDoesNotChangeWithCurrentMap()
        { dynamic g=AcTacHarness.Create();g.MapAcTac=6;g.TLBB.MapId=4;Assert.Equal(6,(int)g.MapAcTac); }
        [Fact, Trait("AcTacBaseline","yes")] public void CalendarIsBusyOutsideDungeon()
        { dynamic g=AcTacHarness.Create();Assert.True((bool)g.Busy); }
        [Fact, Trait("AcTacBaseline","yes")] public void RandomPoolIncludesAllSixMaps()
        { dynamic g=AcTacHarness.Create();var maps=new HashSet<int>();for(int i=0;i<12;i++){g.RandomMap();maps.Add((int)g.MapAcTac);}Assert.Equal(6,maps.Count); }
        [Fact, Trait("AcTacBaseline","yes")] public void RouteEndAndQuietTimeDoNotProveBossDeath()
        { dynamic g=AcTacHarness.Create();g.TLBB.MapId=170;g.MarkRouteDone();g.Advance(11000);g.Step();Assert.False((bool)g.IsBossDie); }
        [Fact, Trait("AcTacBaseline","yes")] public void AutoOffMemberGetsNoCommands()
        { dynamic g=AcTacHarness.Create(),m=AcTacHarness.Create();m.MapAcTac=0;m.TLBB.IsLeader=false;m.IsAuto=false;m.TLBB.MapId=4;g.Party.Add(m);g.Step();Assert.Empty((List<string>)m.Commands); }
        [Fact, Trait("AcTacBaseline","yes")] public void PauseSendsNoCommands()
        { dynamic g=AcTacHarness.Create();g.Pause(true);g.Step();Assert.Empty((List<string>)g.Commands); }
        [Fact, Trait("AcTacBaseline","yes")] public void AutoOffGuardCancelsMap()
        { dynamic g=AcTacHarness.Create();g.IsAuto=false;g.AutoGuard();Assert.Equal(0,(int)g.MapAcTac); }
        [Fact, Trait("AcTacBaseline","yes")] public void NegativeLegacyIndexCannotCrashEngine()
        { dynamic g=AcTacHarness.Create();g.TLBB.MapId=170;g.LegacyIndex=-2;Assert.Null(Record.Exception(()=>{g.Step();})); }
        [Fact, Trait("AcTacBaseline","yes")] public void WrongDialogNeverClicksAll()
        { dynamic g=AcTacHarness.Create();g.TLBB.MapId=6;g.AddNpc();g.SetDialog(99,-1);g.TryEntry();Assert.DoesNotContain("ClickAll",(List<string>)g.Commands); }
        [Fact, Trait("AcTacBaseline","yes")] public void MonsterNamedAsEntryNpcIsNotTalkedTo()
        { dynamic g=AcTacHarness.Create();g.TLBB.MapId=6;g.AddNpc(false);g.TryEntry();Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Talk:")); }
        [Fact, Trait("AcTacBaseline","yes")] public void Generic272DoesNothingWhenGlobalFlagOff()
        { dynamic g=AcTacHarness.Create();g.TLBB.MapId=272;g.Objects.All.Add(g.Make272());g.Generic272();Assert.Empty((List<string>)g.Commands); }

        [Fact, Trait("AcTacBaseline","yes")] public void AcTacSelectionDoesNotDisableOtherTeams()
        { dynamic f=LegacyUiHarness.Create();dynamic leader=f.SelectWithLeader();dynamic other=Activator.CreateInstance(leader.GetType());other.TLBB.KeyId="another-team";f.dicGame.Add(9,other);f.ClickDungeon("vôLượngSơnToolStripMenuItem_Click");Assert.True((bool)other.IsTrieuTap); }
        [Fact, Trait("AcTacBaseline","yes")] public void AcTacSelectionStopsConflictingLeaderQuest()
        { dynamic f=LegacyUiHarness.Create();dynamic leader=f.SelectWithLeader();leader.IsTrungAc=true;f.ClickDungeon("vôLượngSơnToolStripMenuItem_Click");Assert.False((bool)leader.IsTrungAc); }

        [Theory][InlineData("off")][InlineData("offline")][InlineData("init")][InlineData("dead")][InlineData("dead9")]
        [InlineData("null")][InlineData("leader")][InlineData("team")][InlineData("trung")][InlineData("thuy")][InlineData("ky")][InlineData("jail")]
        public void InvalidLeaderStopsBeforeCommands(string fault)
        {
            dynamic g=AcTacHarness.Create();g.Validate();
            switch(fault){case "off":g.IsAuto=false;break;case "offline":g.TLBB.Online=false;break;case "init":g.IsInit=false;break;
                case "dead":g.TLBB.PlayerState=2;break;case "dead9":g.TLBB.PlayerState=9;break;case "null":g.TLBB=null;break;
                case "leader":g.TLBB.IsLeader=false;break;case "team":g.TLBB.KeyId="other";break;case "trung":g.IsTrungAc=true;break;
                case "thuy":g.IsThuyLao=true;break;case "ky":g.IsKyCuoc=true;break;case "jail":g.TLBB.MapId=194;break;}
            g.Step();Assert.Equal(0,(int)g.MapAcTac);Assert.Empty((List<string>)g.Commands);
        }
        [Theory][InlineData("offline")][InlineData("init")][InlineData("dead")][InlineData("scene")][InlineData("team")][InlineData("trung")]
        public void UnavailableMemberGetsNoCommands(string fault)
        {
            dynamic g=AcTacHarness.Create(),m=AcTacHarness.Create();m.MapAcTac=0;m.TLBB.IsLeader=false;
            switch(fault){case "offline":m.TLBB.Online=false;break;case "init":m.IsInit=false;break;case "dead":m.TLBB.PlayerState=2;break;
                case "scene":m.ON_SCENE_TRANSING=true;break;case "team":m.TLBB.KeyId="other";break;case "trung":m.IsTrungAc=true;break;}
            g.Party.Add(m);g.Step();Assert.Empty((List<string>)m.Commands);
        }
        [Theory][InlineData("pause")][InlineData("scene")][InlineData("mapchange")][InlineData("combat")]
        public void PausePreservesRemainingTimeout(string kind)
        {
            dynamic g=AcTacHarness.Create();g.Step();g.Commands.Clear();g.Advance(10000);g.SetPause(kind,true);g.Step();g.Advance(200000);g.Step();
            Assert.Empty((List<string>)g.Commands);Assert.Equal(6,(int)g.MapAcTac);g.SetPause(kind,false);g.Step();Assert.Equal(6,(int)g.MapAcTac);
            g.Advance(170001);g.Step();Assert.Equal(0,(int)g.MapAcTac);
        }
        [Fact] public void ClearMissionCancelsAcTac()
        { dynamic g=AcTacHarness.Create();g.Step();g.ClearMission();Assert.Equal(0,(int)g.MapAcTac); }
        [Fact] public void ChangedCharacterCancelsSession()
        { dynamic g=AcTacHarness.Create();g.Step();g.Commands.Clear();g.TLBB.Id="other";g.Step();Assert.Equal(0,(int)g.MapAcTac);Assert.Empty((List<string>)g.Commands); }
        [Fact] public void InvalidMapIsRejectedWithoutReplacingCurrentSelection()
        { dynamic g=AcTacHarness.Create();Assert.Throws<ArgumentOutOfRangeException>(()=>{g.MapAcTac=999;});Assert.Equal(6,(int)g.MapAcTac); }
        [Fact] public void RestartDiscardsDialogAndBossState()
        { dynamic g=AcTacHarness.Create();g.TLBB.MapId=170;g.SetBoss(1);g.Step();g.SetBoss(0);g.Step();Assert.Equal("AwaitExit",(string)g.Phase);g.MapAcTac=0;g.MapAcTac=6;Assert.Equal("Entry",(string)g.Phase);g.Step();Assert.Equal("Patrol",(string)g.Phase); }
        [Fact] public void EntryUsesOutdoorRouteInsteadOfDungeonCoordinates()
        { dynamic g=AcTacHarness.Create();g.Step();Assert.Contains("Goto:6:58:148",(List<string>)g.Commands);Assert.DoesNotContain("Goto:6:96:84",(List<string>)g.Commands); }
        [Fact] public void NPCDialogWaitsAndSelectsOnlyOnce()
        {
            dynamic g=AcTacHarness.Create();g.TLBB.MapId=6;g.AddNpc();g.TryEntry();g.TryEntry();Assert.Single((List<string>)g.Commands,c=>c.StartsWith("Talk:"));
            g.SetDialog(50013,-1);g.TryEntry();g.TryEntry();Assert.Single((List<string>)g.Commands,c=>c.StartsWith("Option:"));
            Assert.Equal(6,(int)g.MapAcTac);g.Advance(30000);g.TryEntry();Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Move:"));g.Advance(30001);g.Step();Assert.Equal(0,(int)g.MapAcTac);
        }
        [Theory][InlineData("name")][InlineData("missing")][InlineData("monster")][InlineData("extra")]
        public void InvalidNpcOrOptionCannotEnter(string fault)
        {
            dynamic g=AcTacHarness.Create();g.TLBB.MapId=6;g.AddNpc(fault!="monster");
            if(fault=="name")g.Objects.All[0].Name="other";if(fault=="missing")g.Objects.All.Clear();
            g.TryEntry();g.SetDialog(50013,fault=="extra"?0:-1);g.TryEntry();Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Option:"));
        }
        [Fact] public void StaleOpenDialogIsClosedOnceBeforeTalk()
        { dynamic g=AcTacHarness.Create();g.TLBB.MapId=6;g.AddNpc();g.SetDialog(50013,-1);g.KeepOpen=true;g.TryEntry();g.TryEntry();Assert.Equal(new[]{"Close"},(List<string>)g.Commands); }
        [Fact] public void LateStaleDialogReceivesItsOwnWaitBudget()
        {
            dynamic g=AcTacHarness.Create();g.TLBB.MapId=6;g.Step();g.Advance(120000);g.AddNpc();g.SetDialog(99,-1);g.KeepOpen=true;
            g.Step();g.Advance(59000);g.Step();Assert.Equal(6,(int)g.MapAcTac);Assert.Single((List<string>)g.Commands,c=>c=="Close");
            g.Advance(1001);g.Step();Assert.Equal(0,(int)g.MapAcTac);Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Option:"));
        }
        [Fact] public void CancellationDuringTalkPreventsSelection()
        { dynamic g=AcTacHarness.Create();g.TLBB.MapId=6;g.AddNpc();g.CancelOnTalk=true;g.TryEntry();g.SetDialog(50013,-1);g.TryEntry();Assert.Equal(0,(int)g.MapAcTac);Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Option:")); }
        [Fact] public void CancellationDuringSelectionPreventsClose()
        { dynamic g=AcTacHarness.Create();g.TLBB.MapId=6;g.AddNpc();g.TryEntry();g.SetDialog(50013,-1);g.Commands.Clear();g.CancelOnSelect=true;g.TryEntry();Assert.Equal(new[]{"Option:50013:-1"},(List<string>)g.Commands); }
        [Fact] public void ParentOffDuringMemberGotoDoesNotCommandNextMember()
        { dynamic g=AcTacHarness.Create(),m=AcTacHarness.Create(),n=AcTacHarness.Create();m.MapAcTac=n.MapAcTac=0;m.TLBB.IsLeader=n.TLBB.IsLeader=false;m.CancelController=g;g.Party.Add(m);g.Party.Add(n);g.Step();Assert.Empty((List<string>)n.Commands); }
        [Theory][InlineData(0f)][InlineData(float.NaN)][InlineData(float.PositiveInfinity)]
        public void UnobservedOrInvalidBossDoesNotProveCompletion(float hp)
        { dynamic g=AcTacHarness.Create();g.TLBB.MapId=170;g.SetBoss(hp);g.Step();Assert.Equal("Patrol",(string)g.Phase);Assert.False((bool)g.IsBossDie); }
        [Fact] public void BossMustMatchPreviouslyObservedLiveId()
        { dynamic g=AcTacHarness.Create();g.TLBB.MapId=170;g.SetBoss(1);g.Step();g.SetBoss(0,701);g.Step();Assert.Equal("Patrol",(string)g.Phase);g.SetBoss(0,700);g.Step();Assert.Equal("AwaitExit",(string)g.Phase); }
        [Fact] public void BossDeathWaitsForActualMapThenResetsForNextRun()
        { dynamic g=AcTacHarness.Create();g.TLBB.MapId=170;g.SetBoss(1);g.Step();g.SetBoss(0);g.Step();g.Commands.Clear();g.Advance(30000);g.Step();Assert.Equal("AwaitExit",(string)g.Phase);Assert.Empty((List<string>)g.Commands);g.TLBB.MapId=6;g.Step();Assert.Equal("Entry",(string)g.Phase);Assert.Equal(6,(int)g.MapAcTac); }
        [Fact] public void MissingExitTimesOutWithoutRestartingDungeonRoute()
        { dynamic g=AcTacHarness.Create();g.TLBB.MapId=170;g.SetBoss(1);g.Step();g.SetBoss(0);g.Step();g.Commands.Clear();g.Advance(60001);g.Step();Assert.Equal(0,(int)g.MapAcTac);Assert.Empty((List<string>)g.Commands); }
        [Fact] public void UnexpectedDungeonDepartureStopsInsteadOfGuessingSuccess()
        { dynamic g=AcTacHarness.Create();g.TLBB.MapId=170;g.Step();g.TLBB.MapId=6;g.Commands.Clear();g.Step();Assert.Equal(0,(int)g.MapAcTac);Assert.Empty((List<string>)g.Commands); }
        [Fact] public void MemberIdentityChangeStopsBeforeCommands()
        { dynamic g=AcTacHarness.Create(),m=AcTacHarness.Create();m.MapAcTac=0;m.TLBB.IsLeader=false;g.Party.Add(m);g.Step();m.TLBB.Id="new";g.Commands.Clear();m.Commands.Clear();g.Step();Assert.Equal(0,(int)g.MapAcTac);Assert.Empty((List<string>)m.Commands); }
        [Fact] public void DuplicatePartyDoesNotDuplicateCommands()
        { dynamic g=AcTacHarness.Create(),m=AcTacHarness.Create();m.MapAcTac=0;m.TLBB.IsLeader=false;g.Party.Add(g);g.Party.Add(m);g.Party.Add(m);g.Step();Assert.Single((List<string>)m.Commands); }
        [Fact] public void ReadExceptionStopsWithoutSensitiveMessage()
        { dynamic g=AcTacHarness.Create();g.TLBB.MapId=6;g.AddNpc();g.TryEntry();g.SetDialog(50013,-1);g.ThrowRead=true;g.Step();Assert.Equal(0,(int)g.MapAcTac);Assert.Contains("InvalidOperationException",(string)g.Warning);Assert.DoesNotContain("SECRET",(string)g.Warning); }
        [Fact] public void CombatReanchorKeepsUnvisitedWaypoints()
        { dynamic g=AcTacHarness.Create();g.TLBB.MapId=170;g.CharX=21;g.CharY=94;g.Step();g.Advance(2001);g.Step();Assert.Equal("Patrol",(string)g.Phase);Assert.Equal(1,((bool[])g.Visited).Count(x=>x)); }
        [Fact] public void DungeonRoutePreservesAllFourteenLegacyPoints()
        { dynamic g=AcTacHarness.Create();int[,] original=g.AcTacPoint;int[,] repaired=g.DungeonRoute;Assert.Equal(14,original.GetLength(0));Assert.Equal(original.Cast<int>(),repaired.Cast<int>()); }
        [Fact] public void MemberCanObserveBossForLeaderSession()
        { dynamic g=AcTacHarness.Create(),m=AcTacHarness.Create();m.MapAcTac=0;m.TLBB.IsLeader=false;m.TLBB.Id="member";g.TLBB.MapId=m.TLBB.MapId=170;g.Party.Add(m);m.SetBoss(1);g.Step();m.SetBoss(0);g.Step();Assert.Equal("AwaitExit",(string)g.Phase); }
        [Fact] public void MemberOffDuringExitStopsWithoutClaimingCompletion()
        { dynamic g=AcTacHarness.Create(),m=AcTacHarness.Create();m.MapAcTac=0;m.TLBB.IsLeader=false;g.TLBB.MapId=m.TLBB.MapId=170;g.Party.Add(m);g.SetBoss(1);g.Step();g.SetBoss(0);g.Step();m.IsAuto=false;g.Commands.Clear();m.Commands.Clear();g.Step();Assert.Equal(0,(int)g.MapAcTac);Assert.Empty((List<string>)m.Commands); }
        [Fact] public void EmptyRouteStopsWithoutIndexing()
        { dynamic g=AcTacHarness.Create();g.Validate();g.EmptyRoute();Assert.Equal(0,(int)g.MapAcTac);Assert.Empty((List<string>)g.Commands); }
        [Fact] public void InvalidCharacterCoordinatesStopWithoutMoving()
        { dynamic g=AcTacHarness.Create();g.TLBB.MapId=170;g.CharX=float.NaN;g.Step();g.Advance(2001);g.Step();Assert.Equal(0,(int)g.MapAcTac);Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Move:")); }

    }

    internal static class AcTacHarness
    {
        private static readonly Lazy<Assembly> Compiled=new Lazy<Assembly>(Compile);
        internal static dynamic Create()=>Activator.CreateInstance(Compiled.Value.GetType("TinhKiemAuto.Game"));
        private static Assembly Compile()
        {
            bool baseline=Environment.GetEnvironmentVariable("AC_TAC_BASELINE")=="1";
            var game=CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"RegressionSources",baseline?"AcTacLegacyGame.cs":"AcTacReviewedGame.cs")))
                .GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single(c=>c.Identifier.Text=="Game");
            string[] selected=baseline?new[]{"DatDoiAcTac","TrieuTap","TalkNPCPhuBan","RandomAcTac","ClearMission"}:new[]{"RandomAcTac","ClearMission"};
            string methods=string.Join("\n",game.Members.OfType<MethodDeclarationSyntax>().Where(m=>selected.Contains(m.Identifier.Text)).Select(m=>m.ToFullString()));
            methods+=string.Join("\n",game.Members.OfType<PropertyDeclarationSyntax>().Where(p=>new[]{"MapAcTac","AcTacPoint","IsBusy"}.Contains(p.Identifier.Text)).Select(p=>p.ToFullString()));
            string auto=game.Members.OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="Auto").DescendantNodes().OfType<IfStatementSyntax>().Single(i=>i.Condition.ToString()=="!IsAuto").ToFullString();
            var generic=game.Members.OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="AcTac").Body.Statements;
            string genericGuard="";foreach(var statement in generic){genericGuard+=statement.ToFullString();if(statement is IfStatementSyntax i&&i.Condition.ToString()=="!Global.IsAcTac")break;}
            string control=baseline?"public void MarkRouteDone(){MapATIndex=AcTacPoint.GetLength(0);}":
                "public void MarkRouteDone(){CheckAcTacSession();acTacPhase=AcTacPhase.AwaitBoss;acTacPhaseAt=AcTacNow;} public string Phase=>acTacPhase.ToString();public int[,] DungeonRoute=>AcTacDungeonRoute;public void EmptyRoute(){PatrolAcTac(new int[0,2],true);}public bool[] Visited=>acTacVisited;public void Validate()=>CheckAcTacSession();";
            string entry=baseline?"TalkNPCPhuBan();":"DatDoiAcTac();";
            var points=CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"RegressionSources","POINT.cs"))).GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single();
            string pointFields=string.Join("\n",points.Members.OfType<FieldDeclarationSyntax>().Where(f=>f.Declaration.Variables.Any(v=>new[]{"VoLuongSon","KinhHo","KiemCac","ThaiHo","TungSon","DonHoang"}.Contains(v.Identifier.Text))).Select(f=>f.ToFullString()));
            var maps=CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"RegressionSources","MAP.cs"))).GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single();
            string mapFields=string.Join("\n",maps.Members.OfType<FieldDeclarationSyntax>().Where(f=>f.Declaration.Type.ToString()=="int").Select(f=>f.ToFullString()));
            string source="using System;using System.Collections.Generic;using Stopwatch=TinhKiemAuto.FakeStopwatch;using Random=TinhKiemAuto.FakeRandom;namespace TinhKiemAuto {"+Fixture+"public static class POINT {"+pointFields+"} public static class MAP {"+mapFields+"} public partial class Game {"+Fields+methods+control+
                "public void TryEntry(){"+entry+"} public void AutoGuard(){"+auto+"} public void Generic272(){"+genericGuard+"} } }";
            var trees=new List<SyntaxTree>{CSharpSyntaxTree.ParseText(source)};
            if(!baseline) trees.Add(CSharpSyntaxTree.ParseText(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"RegressionSources","Game.AcTac.cs"))
                .Replace("using System.Diagnostics;","using Stopwatch=TinhKiemAuto.FakeStopwatch;using Random=TinhKiemAuto.FakeRandom;")));
            var refs=((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator).Select(p=>MetadataReference.CreateFromFile(p));
            var c=CSharpCompilation.Create("AcTacSimulation",trees,refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var bytes=new MemoryStream();var result=c.Emit(bytes);Assert.True(result.Success,string.Join("\n",result.Diagnostics.Where(d=>d.Severity==DiagnosticSeverity.Error)));return Assembly.Load(bytes.ToArray());
        }
        private const string Fixture="""
            public class FakeStopwatch {public static long Now;private long start=Now;public static FakeStopwatch StartNew()=>new FakeStopwatch();public long ElapsedMilliseconds=>Now-start;public TimeSpan Elapsed=>TimeSpan.FromMilliseconds(ElapsedMilliseconds);}
            public class FakeRandom {private static int next;public int Next(int min,int max)=>min+next++%(max-min);}
            public class TLBB {public string Id="leader",KeyId="leader";public int MapId=1,PlayerState,MapAcBa=50;public bool IsLeader=true,Online=true,IsFollow,HaveRide,IsQuestOpen;}
            public class GameObject {public int Id=100;public string Name="therebels",Title="";public bool IsNPC=true;public float X,Y,HP=1;public string CleanName=>Name.Replace(" ","").ToLowerInvariant();}
            public class ObjectsFixture {public List<GameObject> All=new List<GameObject>(),NearMonter20m=new List<GameObject>(),NearMonter12m=new List<GameObject>();public List<GameObject> NearMonter(float x,float y,float r)=>NearMonter20m;}
            public class QuestFrame {private Game owner;public QuestFrame(){}public QuestFrame(Game g){owner=g;}public void ClickAll(){owner.Commands.Add("ClickAll");}public int StrOptionExtra1,StrOptionExtra2;public string Name="";public static List<QuestFrame> Enum(Game g){if(g.ThrowRead)throw new InvalidOperationException("SECRET");return g.Dialog;}}
            public static class Global {public static bool Paused,IsAcTac;}
            public static class CanhBao {public enum Kieu {Eror}public static string Last="";public static void Msg(string title,string text,Kieu kind){Last=text;}}
            public static class TINHKIEM {public static float GetDistance(float x,float y,float a,float b)=>(float)Math.Sqrt((x-a)*(x-a)+(y-b)*(y-b));public static int ParseInt(string s)=>int.Parse(s);}
            """;
        private const string Fields="""
            public TLBB TLBB=new TLBB();public ObjectsFixture Objects=new ObjectsFixture();public List<Game> Party=new List<Game>();public List<QuestFrame> Dialog=new List<QuestFrame>();public List<string> Commands=new List<string>();
            public bool IsAuto=true,IsInit=true,IsRide,ON_SCENE_TRANSING,IsChangeMap,IsBossDie,IsTrieuTap,IsTheoQ,IsP,TraQ,NhanQ,IsClick,IsContinute,IsNhamBinhSinhDie,IsMapAcBa,Talked,ThrowRead,KeepOpen,CancelOnTalk,CancelOnSelect;
            public bool IsThuyLao,IsKyCuoc,IsTrungAc,IsAcBa,IsLauLanTamBao,IsQ123LauLan,IsQ123ToChau,IsYenTuO,IsPhungHoangLangMo,IsPMP,IsTuBaoBon,IsLuyenKim,IsHuyetChien,IsBachHoaDuyen,IsSuMon,IsXayDung,IsTuDuong,IsNhiemVuCoBan;
            public int MapTKC,MoveIndex,MapATIndex=-1,CurMapATIndex=-1;private int mapAcTac;public float CharX,CharY,RoundX,RoundY;public static int TickCount=9;public Game CancelController;
            public FakeStopwatch tranTime=FakeStopwatch.StartNew(),ClearTime=FakeStopwatch.StartNew(),TimeStand=FakeStopwatch.StartNew();public QuestFrame QuestFrame;
            public Game(){Global.Paused=Global.IsAcTac=false;FakeStopwatch.Now+=3000;QuestFrame=new QuestFrame(this);MapAcTac=6;CanhBao.Last="";}
            public bool Busy=>IsBusy;public int LegacyIndex{set=>MapATIndex=value;}public bool IsMoveEx=>true;public string Warning=>CanhBao.Last;
            public void Step()=>DatDoiAcTac();public void RandomMap()=>RandomAcTac();public void Advance(long ms){FakeStopwatch.Now+=ms;}
            public void Pause(bool on){Global.Paused=on;}public void SetPause(string kind,bool on){if(kind=="pause")Global.Paused=on;if(kind=="scene")ON_SCENE_TRANSING=on;if(kind=="mapchange")IsChangeMap=on;if(kind=="combat")TLBB.PlayerState=on?7:0;}
            public void AddNpc(bool npc=true){Objects.All.Add(new GameObject{IsNPC=npc});}public void SetDialog(int a,int b){TLBB.IsQuestOpen=true;Dialog.Clear();Dialog.Add(new QuestFrame{StrOptionExtra1=a,StrOptionExtra2=b});}
            public GameObject Make272()=>new GameObject{Name="n Du V"+new string('x',18)};
            public void SetBoss(float hp,int id=700){Objects.All.RemoveAll(o=>o.Name=="tacbinhdaumuc");Objects.All.Add(new GameObject{Name="tacbinhdaumuc",IsNPC=false,HP=hp,Id=id});}
            public bool GoTo(float x,float y,int map){Commands.Add($"Goto:{map}:{x}:{y}");if(CancelController!=null)CancelController.MapAcTac=0;return false;}
            public void Move(float x,float y){Commands.Add($"Move:{x}:{y}");}public void Talk(int id){Commands.Add("Talk:"+id);if(CancelOnTalk)MapAcTac=0;}
            public void QuestFrameOptionClicked(QuestFrame f){Commands.Add($"Option:{f.StrOptionExtra1}:{f.StrOptionExtra2}");if(CancelOnSelect)MapAcTac=0;}
            public void CloseQuest(){Commands.Add("Close");if(!KeepOpen)TLBB.IsQuestOpen=false;}public bool PickItem()=>false;
            public void StopFollow(){Commands.Add("StopFollow");TLBB.IsFollow=false;}public void DownRide(){Commands.Add("DownRide");IsRide=false;}public void UpRide(){Commands.Add("UpRide");IsRide=true;}
            public void AskTeamFollow(){Commands.Add("FollowAll");}public void Ride(){Commands.Add("Ride");}public void MoveNext(){Commands.Add("LegacyMoveNext");}public void FixKetMap(){Commands.Add("FixKet");}
            public bool IsMapPhuBan()=>TLBB!=null&&TLBB.MapId==170;public bool CanActThuyLao()=>false;public void RunThuyLaoMember(Game m){}public void StopThuyLao(string s){}public void ResetThuyLaoProgress(){}
            """;
    }
}
