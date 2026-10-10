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
    public sealed class ThuyLaoSimulationTests
    {
        [Fact, Trait("ThuyLaoBaseline", "yes")] public void EntryFromAnotherMapTargetsTheNpcMap()
        {
            dynamic g = ThuyLaoHarness.Create(); g.TLBB.MapId = 2; g.Receive = true;
            g.Enter(); Assert.Contains("Route:4:67:77", (List<string>)g.Commands);
        }
        [Fact, Trait("ThuyLaoBaseline", "yes")] public void AutoOffMemberReceivesNoCommands()
        {
            dynamic leader = ThuyLaoHarness.Create(), member = ThuyLaoHarness.Create();
            member.TLBB.IsLeader = false; member.IsAuto = false;
            leader.Party.Add(member); leader.TrieuTap(); Assert.Empty((List<string>)member.Commands);
        }
        [Fact, Trait("ThuyLaoBaseline", "yes")] public void MemberAlreadyInsideWaitsForLeaderOutside()
        {
            dynamic leader = ThuyLaoHarness.Create(), member = ThuyLaoHarness.Create();
            leader.TLBB.MapId = 4; member.TLBB.IsLeader = false; member.TLBB.MapId = 66; member.Receive = true;
            leader.Party.Add(member); leader.TrieuTap(); Assert.Empty((List<string>)member.Commands);
        }
        [Fact, Trait("ThuyLaoBaseline", "yes")] public void RouteEndDoesNotMeanQuestCompleted()
        {
            dynamic g = ThuyLaoHarness.Create(); g.TLBB.MapId = 66; g.MoveIndex = 2;
            g.MoveNext(new int[,] { { 71, 41 }, { 107, 40 } }); Assert.False((bool)g.Completed);
        }

        [Theory]
        [InlineData("off")][InlineData("offline")][InlineData("dead")][InlineData("zoning")]
        [InlineData("transition")][InlineData("mapchange")][InlineData("otherteam")][InlineData("nullmetadata")]
        public void UnavailableMemberReceivesNoCommands(string condition)
        {
            dynamic leader = ThuyLaoHarness.Create(), member = ThuyLaoHarness.Create();
            member.TLBB.IsLeader = false;
            switch (condition)
            {
                case "off": member.IsAuto=false; break;
                case "offline": member.TLBB.Online=false; break;
                case "dead": member.TLBB.PlayerState=2; break;
                case "zoning": member.TLBB.PlayerState=7; break;
                case "transition": member.ON_SCENE_TRANSING=true; break;
                case "mapchange": member.IsChangeMap=true; break;
                case "otherteam": member.TLBB.KeyId="another-leader"; break;
                case "nullmetadata": member.TLBB=null; break;
            }
            leader.Party.Add(member); leader.TrieuTap(); Assert.Empty((List<string>)member.Commands);
        }

        [Theory]
        [InlineData("off")][InlineData("offline")][InlineData("dead")][InlineData("transition")]
        [InlineData("mapchange")][InlineData("leaderchanged")][InlineData("nullmetadata")]
        public void InvalidLeaderStopsModuleBeforeAnyCommand(string condition)
        {
            dynamic g=ThuyLaoHarness.Create();
            switch (condition)
            {
                case "off": g.IsAuto=false; break;
                case "offline": g.TLBB.Online=false; break;
                case "dead": g.TLBB.PlayerState=2; break;
                case "transition": g.ON_SCENE_TRANSING=true; break;
                case "mapchange": g.IsChangeMap=true; break;
                case "leaderchanged": g.TLBB.IsLeader=false; break;
                case "nullmetadata": g.TLBB=null; break;
            }
            g.DatDoiThuyLao(); Assert.False((bool)g.IsThuyLao); Assert.Empty((List<string>)g.Commands);
        }

        [Fact] public void DisabledModuleDoesNotAffectOtherQuestFlags()
        {
            dynamic g=ThuyLaoHarness.Create(); g.IsThuyLao=false; g.IsAcBa=true; g.IsClick=true;
            g.DatDoiThuyLao(); Assert.Empty((List<string>)g.Commands);
            Assert.True((bool)g.IsAcBa); Assert.True((bool)g.IsClick);
        }

        [Fact] public void MissionOpeningWaitsForObservedWindowAndSendsOnce()
        {
            dynamic g=ThuyLaoHarness.Create();
            for(int i=0;i<5;i++) g.ReceiveQuest();
            Assert.Equal(new[]{"Post:18:105"}, (List<string>)g.Commands); Assert.Equal(0,(int)g.TaskReads);
            g.AdvanceClock(60001); g.ReceiveQuest(); Assert.False((bool)g.IsThuyLao);
            int count=g.Notices.Count; g.ReceiveQuest(); Assert.Equal(count,(int)g.Notices.Count);
        }

        [Fact] public void ReceiveRequiresQuestAppearanceAndDoesNotRepeatAccept()
        {
            dynamic g=ThuyLaoHarness.Create(); PrepareReceive(g);
            g.IsClick=true;
            g.ReceiveQuest(); g.ReceiveQuest();
            for(int i=0;i<5;i++) g.ReceiveQuest();
            Assert.False((bool)g.Receive); Assert.Equal("VerifyMission",(string)g.ReceiveStage);
            Assert.Equal(1,((List<string>)g.Commands).Count(c=>c=="Accept")); Assert.True((bool)g.IsClick);
            g.AddTask(false,0); g.ReceiveQuest(); Assert.True((bool)g.Receive);
        }

        [Theory][InlineData(true,0)][InlineData(false,256)]
        public void CompletedQuestIsNotAcceptedAsAnotherRun(bool completed,int status)
        {
            dynamic g=ThuyLaoHarness.Create(); g.AddTask(completed,status);
            g.ReceiveQuest(); g.TLBB.IsTogleMission=true; g.ReceiveQuest(); g.ReceiveQuest();
            Assert.False((bool)g.Receive); Assert.False((bool)g.IsThuyLao);
            Assert.DoesNotContain("Accept",(List<string>)g.Commands);
        }

        [Theory][InlineData("wrongoption")][InlineData("wrongextra")][InlineData("missingnpc")]
        public void UnverifiedNpcOrDialogIsNeverSelected(string fault)
        {
            dynamic g=ThuyLaoHarness.Create(); PrepareReceive(g);
            if(fault=="wrongoption") { g.Dialog.Clear(); g.AddOption(99,-1); }
            if(fault=="wrongextra") { g.Dialog.Clear(); g.AddOption(232000,4); }
            if(fault=="missingnpc") g.Objects.All.Clear();
            g.ReceiveQuest(); g.ReceiveQuest();
            Assert.DoesNotContain((List<string>)g.Commands,c=>c.StartsWith("Select:")||c=="Accept"||c.StartsWith("Talk:"));
            g.AdvanceClock(60001); g.ReceiveQuest(); Assert.False((bool)g.IsThuyLao);
        }

        [Fact] public void SceneChangeDiscardsPendingDialogAndMissionState()
        {
            dynamic g=ThuyLaoHarness.Create(); PrepareReceive(g); g.ReceiveQuest();
            Assert.Contains("Select:232000",(List<string>)g.Commands);
            g.TLBB.MapId=2; g.Commands.Clear(); g.ReceiveQuest();
            Assert.Equal("OpenMission",(string)g.ReceiveStage); Assert.False((bool)g.Receive);
            Assert.DoesNotContain("Accept",(List<string>)g.Commands);
        }

        [Fact] public void TeamChangeRevalidatesPreviouslyReceivedQuest()
        {
            dynamic g=ThuyLaoHarness.Create(); g.AddTask(false,0);
            g.ReceiveQuest(); g.TLBB.IsTogleMission=true; g.ReceiveQuest(); g.ReceiveQuest();
            Assert.True((bool)g.Receive);
            g.TLBB.KeyId="new-leader"; g.ReceiveQuest();
            Assert.False((bool)g.Receive); Assert.Equal("OpenMission",(string)g.ReceiveStage);
        }

        [Fact] public void EntryAcceptsOnceThenWaitsForActualMapChange()
        {
            dynamic g=ThuyLaoHarness.Create(); g.TLBB.MapId=4; g.TLBB.IsQuestOpen=true; g.AddOption(232002,-1);
            g.Enter(); g.Enter(); g.Enter(); g.Enter();
            Assert.Equal(1,((List<string>)g.Commands).Count(c=>c=="Accept"));
            g.AdvanceClock(60001); g.Enter(); Assert.False((bool)g.IsThuyLao);
            Assert.Contains((List<string>)g.Notices,c=>c.Contains("Entry"));
        }

        [Fact] public void EntryObservedInsideNeverRoutesBackToNpc()
        {
            dynamic g=ThuyLaoHarness.Create(); g.TLBB.MapId=66; g.Enter(); Assert.Empty((List<string>)g.Commands);
        }

        [Fact] public void RouteFinishedWaitsForTaskCompletionBeforeExitPoint()
        {
            dynamic g=ThuyLaoHarness.Create(); g.TLBB.MapId=66;
            g.DatDoiThuyLao(); g.MarkRouteDone(); g.Commands.Clear();
            g.AddTask(false,0); g.DatDoiThuyLao();
            Assert.False((bool)g.Completed); Assert.DoesNotContain("GotoPosition",(List<string>)g.Commands);
            g.Tasks.Clear(); g.AddTask(true,256); g.DatDoiThuyLao();
            Assert.True((bool)g.Completed); Assert.Contains("GotoPosition",(List<string>)g.Commands);
        }

        [Fact] public void RemainingMonstersPreventCompletionAndExitMovement()
        {
            dynamic g=ThuyLaoHarness.Create(); g.TLBB.MapId=66; g.DatDoiThuyLao(); g.MarkRouteDone();
            g.AddTask(true,256); g.AddMonster(); g.DatDoiThuyLao();
            Assert.False((bool)g.Completed); Assert.DoesNotContain("GotoPosition",(List<string>)g.Commands);
        }

        [Fact] public void MissingCompletionTimesOutInsteadOfLoopingOrGuessingSuccess()
        {
            dynamic g=ThuyLaoHarness.Create(); g.TLBB.MapId=66; g.DatDoiThuyLao(); g.MarkRouteDone();
            g.DatDoiThuyLao(); g.AdvanceClock(60001); g.DatDoiThuyLao();
            Assert.False((bool)g.IsThuyLao); Assert.False((bool)g.Completed);
            Assert.DoesNotContain("GotoPosition",(List<string>)g.Commands);
        }

        [Fact] public void PatrolUsesOwnIndexAndWaitsAfterMapArrival()
        {
            dynamic g=ThuyLaoHarness.Create(); g.TLBB.MapId=66; g.MoveIndex=99;
            g.DatDoiThuyLao(); Assert.DoesNotContain("Move",(List<string>)g.Commands);
            g.AdvanceClock(2001); g.DatDoiThuyLao();
            Assert.Contains("Move",(List<string>)g.Commands); Assert.Equal(99,(int)g.MoveIndex);
            g.AdvanceClock(60001); g.DatDoiThuyLao(); Assert.False((bool)g.IsThuyLao);
        }

        [Fact] public void ClearMissionResetsAllThuyLaoState()
        {
            dynamic g=ThuyLaoHarness.Create(); PrepareReceive(g); g.ReceiveQuest(); g.Receive=true; g.MarkRouteDone();
            g.ClearMission(); Assert.False((bool)g.IsThuyLao); Assert.False((bool)g.Receive);
            Assert.False((bool)g.RouteDone); Assert.False((bool)g.Completed); Assert.Equal("",(string)g.ReceiveStage);
        }

        [Fact] public void DisablingModuleDiscardsPendingReceiveState()
        {
            dynamic g=ThuyLaoHarness.Create(); PrepareReceive(g); g.ReceiveQuest();
            g.Receive=true; g.MarkRouteDone(); g.IsThuyLao=false;
            Assert.False((bool)g.Receive); Assert.False((bool)g.RouteDone); Assert.Equal("",(string)g.ReceiveStage);
        }

        [Fact] public void ExplicitRestartAfterTimeoutBeginsWithFreshState()
        {
            dynamic g=ThuyLaoHarness.Create(); g.ReceiveQuest(); g.AdvanceClock(60001); g.ReceiveQuest();
            Assert.False((bool)g.IsThuyLao);
            g.IsThuyLao=true; g.Commands.Clear(); g.ReceiveQuest();
            Assert.True((bool)g.IsThuyLao); Assert.Equal("OpenMission",(string)g.ReceiveStage);
            Assert.Equal(new[]{"Post:18:105"},(List<string>)g.Commands);
        }

        [Fact] public void ReadFailureStopsAndReportsTypeWithoutSensitiveMessage()
        {
            dynamic g=ThuyLaoHarness.Create(); g.DatDoiThuyLao(); g.TLBB.IsTogleMission=true; g.DatDoiThuyLao();
            g.ThrowRead=true; int before=g.Notices.Count; g.DatDoiThuyLao();
            Assert.False((bool)g.IsThuyLao);
            string notice=g.Notices[before]; Assert.Contains("InvalidOperationException",notice);
            Assert.DoesNotContain("SECRET-DO-NOT-LOG",notice);
            g.DatDoiThuyLao(); Assert.Equal(before+1,(int)g.Notices.Count);
        }

        [Fact] public void MemberReadFailureStopsLeaderBeforeItSendsAnotherCommand()
        {
            dynamic leader=ThuyLaoHarness.Create(),member=ThuyLaoHarness.Create();
            member.TLBB.IsLeader=false; member.ReceiveQuest(); member.TLBB.IsTogleMission=true; member.ReceiveQuest();
            member.ThrowRead=true; leader.Party.Add(member);
            leader.DatDoiThuyLao(); Assert.False((bool)leader.IsThuyLao);
            Assert.Empty((List<string>)leader.Commands);
        }

        [Fact] public void GenericSummonBehaviorRemainsAvailableWhenThuyLaoIsOff()
        {
            dynamic leader=ThuyLaoHarness.Create(),member=ThuyLaoHarness.Create();
            leader.IsThuyLao=false; leader.TLBB.MapId=4; member.TLBB.IsLeader=false;
            leader.Party.Add(member); bool summoned=leader.TrieuTap();
            Assert.True(summoned); Assert.Equal(new[]{"GotoMap:4"},(List<string>)member.Commands);
        }

        private static void PrepareReceive(dynamic g)
        {
            g.ReceiveQuest(); g.TLBB.IsTogleMission=true; g.ReceiveQuest(); g.ReceiveQuest();
            g.TLBB.IsQuestOpen=true; g.AddOption(232000,-1);
        }
    }

    internal static class ThuyLaoHarness
    {
        private static readonly Lazy<Assembly> Assembly = new Lazy<Assembly>(Compile);
        internal static dynamic Create() => Activator.CreateInstance(Assembly.Value.GetType("TinhKiemAuto.Game"));
        private static Assembly Compile()
        {
            bool baseline = Environment.GetEnvironmentVariable("THUY_LAO_BASELINE") == "1";
            string file = baseline ? "LegacyGame.cs" : "ReviewedGame.cs";
            string text = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "RegressionSources", file));
            var root = CSharpSyntaxTree.ParseText(text).GetRoot();
            var game = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Single(c => c.Identifier.Text == "Game");
            string[] names = baseline ? new[] { "DiThuyLao", "NhanThuyLao", "DatDoiThuyLao", "TrieuTap", "ClearMission", "MoveNext" }
                : new[] { "TrieuTap", "ClearMission", "MoveNext" };
            string methods = string.Join("\n", game.Members.OfType<MethodDeclarationSyntax>()
                .Where(m => names.Contains(m.Identifier.Text) && (m.Identifier.Text != "MoveNext" || m.ParameterList.Parameters.Count == 1))
                .Select(m => m.ToFullString()));
            methods += game.Members.OfType<PropertyDeclarationSyntax>().Single(p=>p.Identifier.Text=="IsThuyLao").ToFullString();
            string source = "using System; using System.Collections.Generic; using System.Diagnostics; namespace TinhKiemAuto {" + Fixture
                + "public partial class Game {" + Fields + methods
                + (baseline ? "" : "public void MarkRouteDone() { ThuyLaoRouteFinished=true; } public bool RouteDone=>ThuyLaoRouteFinished;") + "} }";
            var trees = new List<SyntaxTree> { CSharpSyntaxTree.ParseText(source) };
            if (!baseline)
            {
                string repair = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "RegressionSources", "Game.ThuyLao.cs"));
                trees.Add(CSharpSyntaxTree.ParseText(repair.Replace("using System.Diagnostics;", "using Stopwatch = TinhKiemAuto.FakeStopwatch;")));
            }
            var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator)
                .Select(p => MetadataReference.CreateFromFile(p));
            var compilation = CSharpCompilation.Create("ThuyLaoSimulation", trees, references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var output = new MemoryStream(); var result = compilation.Emit(output);
            Assert.True(result.Success, string.Join("\n", result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)));
            return System.Reflection.Assembly.Load(output.ToArray());
        }
        private const string Fixture = """
            public class FakeStopwatch {
                public static long Now; private long start=Now;
                public static FakeStopwatch StartNew() => new FakeStopwatch();
                public long ElapsedMilliseconds => Now-start;
                public TimeSpan Elapsed => TimeSpan.FromMilliseconds(ElapsedMilliseconds);
            }
            public class TLBB {
                public int MapId=1, PlayerState=0, MapAcBa; public string Id="leader", KeyId="leader";
                public bool IsLeader=true, Online=true, IsQuestOpen, IsTogleMission, IsFollow, IsRide, HaveRide;
            }
            public class NPC { public int Id,X,Y,Map; }
            public static class THAIHO { public static NPC HoDienKhanh=new NPC {Id=13,X=67,Y=77,Map=4}; }
            public static class TOCHAU { public static NPC HoDienBao=new NPC {Id=93,X=339,Y=310,Map=1}; }
            public static class MAP {
                public const int ThaiHo=4,ToChau=1,ThuyLao=66,PhungHoangCoThanh=10,ViemMaSon=11,TamTaiHiepCoc=12,
                TangKinhCac=13,YenTuO=14,TacKhauDoanhDia=15,SinhTuLoiDai=16,PhungHoangCoThanhPhuBan=17,
                HuyenVuDaoPhuBan=18,ThanhThuSonPhuBan=19,PhieuMieuPhong=20;
            }
            public class GameObject { public int Id; public bool IsNPC=true; }
            public class ObjectsFixture {
                public List<GameObject> All=new List<GameObject>{new GameObject{Id=13},new GameObject{Id=93}};
                public List<GameObject> NearMonter12m=new List<GameObject>(),NearMonter15m=new List<GameObject>(),NearMonter20m=new List<GameObject>();
            }
            public class Task {
                public string ClearName="binhdinhthuylao"; public bool Completed; public int Complete;
                public static List<Task> Enum(Game g) { g.TaskReads++; if(g.ThrowRead) throw new InvalidOperationException("SECRET-DO-NOT-LOG"); return g.Tasks; }
            }
            public class QuestFrame {
                public int StrOptionExtra1,StrOptionExtra2=-1;
                public static List<QuestFrame> Enum(Game g) => g.Dialog;
            }
            public static class TINHKIEM { public static float GetDistance(float x,float y,float a,float b) => (float)Math.Sqrt((x-a)*(x-a)+(y-b)*(y-b)); }
            public static class CanhBao {
                public enum Kieu { Eror }
                public static List<string> Messages=new List<string>();
                public static void Msg(string title,string message,Kieu kind) { Messages.Add(message); }
            }
            """;
        private const string Fields = """
            public static int TickCount=18;
            public TLBB TLBB=new TLBB(); public ObjectsFixture Objects=new ObjectsFixture(); public List<Game> Party=new List<Game>();
            public List<Task> Tasks=new List<Task>(); public List<QuestFrame> Dialog=new List<QuestFrame>();
            public List<string> Commands=new List<string>(); public bool ThrowRead,AtNpc=true,Loot;
            public bool IsAuto=true,IsRide,ON_SCENE_TRANSING,IsChangeMap,IsKyCuoc,IsTrieuTap,IsTheoQ;
            public bool IsQ123ToChau,IsYenTuO,IsBossDie,IsNhamBinhSinhDie,IsMapAcBa,IsQ123LauLan,IsP,TraQ,NhanQ,IsClick,IsContinute;
            public bool IsTuBaoBon,IsLuyenKim,IsHuyetChien,IsPMP,IsLauLanTamBao,IsPhungHoangLangMo,IsAcBa;
            public int MapTKC,MapAcTac,MoveIndex=-1; public float CharX,CharY,RoundX,RoundY;
            private bool DaNhanThuyLao,IsXongThuyLao; private string TrangThaiThuyLao="";
            private Stopwatch ClearTime=Stopwatch.StartNew();
            public FakeStopwatch tranTime=FakeStopwatch.StartNew();
            public Game() { FakeStopwatch.Now+=3000; IsThuyLao=true; }
            public bool Receive { get=>DaNhanThuyLao; set=>DaNhanThuyLao=value; }
            public int TaskReads;
            public List<string> Notices=>CanhBao.Messages;
            public void AddTask(bool done,int status) { Tasks.Add(new Task{Completed=done,Complete=status}); }
            public void AddOption(int option,int extra) { Dialog.Add(new QuestFrame{StrOptionExtra1=option,StrOptionExtra2=extra}); }
            public void AddMonster() { Objects.NearMonter15m.Add(new GameObject()); }
            public bool Completed=>IsXongThuyLao;
            public string ReceiveStage=>TrangThaiThuyLao;
            public void Enter()=>DiThuyLao(); public void ReceiveQuest()=>NhanThuyLao();
            public void AdvanceClock(long ms) { FakeStopwatch.Now+=ms; }
            public void MoveNext() { MoveNext(new int[,] {{71,41},{107,40},{41,50}}); }
            public void TimDuong(float x,float y,int map) { Commands.Add($"Route:{map}:{x}:{y}"); }
            public bool GoTo(NPC n) { Commands.Add($"GotoNpc:{n.Map}:{n.Id}"); return AtNpc; }
            public bool GoTo(float x,float y) { Commands.Add("GotoPosition"); return true; }
            public bool GoTo(float x,float y,int map) { Commands.Add("GotoMap:"+map); return true; }
            public void Move(float x,float y) { Commands.Add("Move"); }
            public bool PickItem() { if(Loot) Commands.Add("Loot"); return Loot; }
            public void StopFollow() { Commands.Add("StopFollow"); }
            public void UpRide() { Commands.Add("UpRide"); }
            public void DownRide() { Commands.Add("DownRide"); }
            public void Ride() { Commands.Add("Ride"); }
            public bool IsMapPhuBan()=>false;
            public void PostMessage(int a,int b) { Commands.Add($"Post:{a}:{b}"); }
            public void Talk(NPC npc) { Commands.Add("Talk:"+npc.Id); }
            public void QuestFrameOptionClicked(int a,int b) { Commands.Add("Select:"+a); }
            public void QuestFrameAccept() { Commands.Add("Accept"); }
            public void CloseQuest() { Commands.Add("CloseQuest"); }
            """;
    }
}
