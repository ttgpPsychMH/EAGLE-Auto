using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TinhKiemAuto.Dashboard;
using Xunit;

namespace ChickenAutoEx.Startup.Tests
{
    public sealed class DungeonDashboardTests
    {
        private sealed class Port : IDungeonDashboardPort
        {
            internal DungeonContext Context = Ready();
            internal int Writes, Reads;
            internal Action<int, DungeonContext> OnRead;
            public DungeonContext Capture(DungeonKind kind) { OnRead?.Invoke(++Reads, Context); return Context; }
            public void Apply(DungeonKind kind, int option, bool start, DungeonBinding binding) { Writes++; Context.Active = start; }
        }
        private static DungeonContext Ready() => new DungeonContext {
            Binding = new DungeonBinding { Selected = new object(), Controller = new object(), SelectedId = "fake-a", ControllerId = "fake-b", TeamId = "fake-b" },
            SelectedName = "Mẫu A", ControllerName = "Mẫu B", Phase = "Chờ NPC",
            SelectedPresent = true, ControllerPresent = true, SameTeam = true, Online = true, Initialized = true, Auto = true
        };
        [Theory]
        [InlineData("selection")][InlineData("controller")][InlineData("team")][InlineData("online")]
        [InlineData("init")][InlineData("auto")][InlineData("dead")][InlineData("pause")]
        [InlineData("map")][InlineData("busy")][InlineData("conflict")]
        public void UnsafeContextBlocksStartButRetainsStopWhereIdentityIsKnown(string condition)
        {
            var p = new Port(); var c = p.Context;
            switch (condition) {
                case "selection": c.SelectedPresent = false; break;
                case "controller": c.ControllerPresent = false; break;
                case "team": c.SameTeam = false; break;
                case "online": c.Online = false; break;
                case "init": c.Initialized = false; break;
                case "auto": c.Auto = false; break;
                case "dead": c.Dead = true; break;
                case "pause": c.Paused = true; break;
                case "map": c.ChangingMap = true; break;
                case "busy": c.Busy = true; break;
                case "conflict": c.Conflict = true; break;
            }
            var a = new DungeonDashboardAdapter(p);
            Assert.False(a.Read(DungeonKind.KyCuoc).CanStart);
            Assert.False(string.IsNullOrEmpty(a.Execute(DungeonKind.KyCuoc, 0, true, c.Binding)));
            Assert.Equal(0, p.Writes);
            c.Active = true;
            bool stoppable = condition != "selection" && condition != "controller" && condition != "team";
            Assert.Equal(stoppable, a.Read(DungeonKind.KyCuoc).CanStop);
            a.Execute(DungeonKind.KyCuoc, 0, false, c.Binding);
            Assert.Equal(stoppable ? 1 : 0, p.Writes);
        }
        [Theory][InlineData("selected")][InlineData("controller")][InlineData("login")][InlineData("team")]
        public void StaleDisplayAndSecondReadNeverWriteToNewActor(string change)
        {
            foreach (int at in new[] { 1, 2 }) {
                var p = new Port(); var expected = p.Context.Binding;
                p.OnRead = (n, c) => {
                    if (n != at) return;
                    c.Binding = new DungeonBinding { Selected = expected.Selected, Controller = expected.Controller,
                        SelectedId = expected.SelectedId, ControllerId = expected.ControllerId, TeamId = expected.TeamId };
                    switch (change) {
                        case "selected": c.Binding.Selected = new object(); break;
                        case "controller": c.Binding.Controller = new object(); break;
                        case "login": c.Binding.SelectedId = "fake-relogin"; break;
                        case "team": c.Binding.TeamId = "fake-new-team"; break;
                    }
                };
                new DungeonDashboardAdapter(p).Execute(DungeonKind.KyCuoc, 0, true, expected);
                Assert.Equal(0, p.Writes);
            }
        }
        [Fact] public void StateChangeDuringSecondReadBlocksStart()
        {
            var p = new Port { OnRead = (n,c) => { if(n == 2) c.Paused = true; } };
            new DungeonDashboardAdapter(p).Execute(DungeonKind.KyCuoc, 0, true, p.Context.Binding);
            Assert.Equal(0,p.Writes);
        }
        [Fact] public void StartIsIdempotentAndStopOnlyTargetsActiveTask()
        {
            var p = new Port(); var a = new DungeonDashboardAdapter(p);
            a.Execute(DungeonKind.KyCuoc,0,true,p.Context.Binding);
            a.Execute(DungeonKind.KyCuoc,0,true,p.Context.Binding);
            Assert.Equal(1,p.Writes);
            a.Execute(DungeonKind.KyCuoc,0,false,p.Context.Binding);
            a.Execute(DungeonKind.KyCuoc,0,false,p.Context.Binding);
            Assert.Equal(2,p.Writes);
        }
        [Fact] public void FollowingBlocksPersonalQuestOnly()
        {
            var c = Ready(); c.Following = true;
            Assert.False(DungeonDashboardPolicy.Evaluate(DungeonKind.TrungAc,c).CanStart);
            Assert.True(DungeonDashboardPolicy.Evaluate(DungeonKind.KyCuoc,c).CanStart);
        }
        [Fact] public void ThuyLaoUsesTheSameBoundStartStopGuards()
        {
            var p = new Port(); var a = new DungeonDashboardAdapter(p);
            Assert.True(a.Read(DungeonKind.ThuyLao).CanStart);
            a.Execute(DungeonKind.ThuyLao,0,true,p.Context.Binding);
            Assert.Equal(1,p.Writes);
            p.Context.Active = true;
            Assert.True(a.Read(DungeonKind.ThuyLao).CanStop);
            a.Execute(DungeonKind.ThuyLao,0,false,p.Context.Binding);
            Assert.Equal(2,p.Writes);
        }
        [Theory][InlineData(2,-1)][InlineData(2,170)]
        [InlineData(3,0)][InlineData(3,10)][InlineData(1,50013)]
        public void InvalidSettingsNeverDispatch(int kind, int option)
        {
            var p = new Port(); new DungeonDashboardAdapter(p).Execute((DungeonKind)kind,option,true,p.Context.Binding);
            Assert.Equal(0,p.Writes);
        }
        [Fact] public void InactiveStateDoesNotClaimCompletionOrInventStopReason()
        {
            var v = DungeonDashboardPolicy.Evaluate(DungeonKind.KyCuoc, Ready());
            Assert.Contains("không suy đoán hoàn tất",v.Reason);
            Assert.Contains("Chưa bật",v.State);
        }
        [Theory][InlineData(0)][InlineData(1)][InlineData(2)][InlineData(3)]
        public void RealPortRoutesOnlyChosenTaskToCorrectController(int kind)
        {
            dynamic h = DashboardHarness.Create();
            h.Run(kind, kind == 2 ? 7 : kind == 3 ? 5 : 0, true);
            Assert.Equal(kind == 0,(bool)h.Actor.IsTrungAc);
            Assert.Equal(kind == 1,(bool)h.Leader.IsKyCuoc);
            Assert.Equal(kind == 2 ? 7 : 0,(int)h.Leader.MapAcTac);
            Assert.Equal(kind == 3,(bool)h.Leader.IsAcBa);
            Assert.False((bool)h.Actor.IsKyCuoc); Assert.False((bool)h.Actor.IsAcBa);
            h.Run(kind,0,false);
            Assert.False((bool)h.Actor.IsTrungAc); Assert.False((bool)h.Leader.IsKyCuoc);
            Assert.Equal(0,(int)h.Leader.MapAcTac); Assert.False((bool)h.Leader.IsAcBa);
        }
        [Fact] public void PersonalQuestWorksWithoutTeamButTeamTaskNeedsLeader()
        {
            dynamic h = DashboardHarness.Create(); h.Actor.Leader = null;
            Assert.True((bool)h.CanStart(0)); Assert.False((bool)h.CanStart(1));
            h.Run(0,0,true); Assert.True((bool)h.Actor.IsTrungAc);
        }
        [Fact] public void RealPortRevalidatesAtMutationBoundary()
        {
            dynamic h = DashboardHarness.Create(); h.ApplyAfterTeamChange();
            Assert.False((bool)h.Leader.IsKyCuoc);
        }
        [Fact] public void RealPortDoesNotCancelConflictingTasks()
        {
            dynamic h = DashboardHarness.Create(); h.Leader.IsAcBa = true;
            Assert.False((bool)h.CanStart(1)); h.Run(1,0,true);
            Assert.True((bool)h.Leader.IsAcBa); Assert.False((bool)h.Leader.IsKyCuoc);
        }
        [Fact] public void PersonalQuestCannotInterfereWithLeadersTeamTask()
        {
            dynamic h = DashboardHarness.Create(); h.Leader.IsKyCuoc = true;
            Assert.False((bool)h.CanStart(0)); h.Run(0,0,true);
            Assert.False((bool)h.Actor.IsTrungAc); Assert.True((bool)h.Leader.IsKyCuoc);
        }
        [Fact] public void StartingDoesNotClearAnOngoingSummon()
        {
            dynamic h = DashboardHarness.Create(); h.Actor.IsTrieuTap = true;
            Assert.False((bool)h.CanStart(1)); h.Run(1,0,true);
            Assert.True((bool)h.Actor.IsTrieuTap); Assert.False((bool)h.Leader.IsKyCuoc);
        }
        [Fact] public void AutoAcBaAndRandomAcTacUseExistingEntryPoints()
        {
            dynamic h = DashboardHarness.Create(); h.Run(3,-1,true);
            Assert.True((bool)h.Leader.IsAcBa); Assert.Equal(-1,(int)h.Leader.ManualSchool);
            h.Run(3,0,false); h.Run(2,0,true); Assert.Equal(5,(int)h.Leader.MapAcTac);
        }
        [Fact] public void ReadPhaseProjectionSendsNoCommandsAndKeepsFlags()
        {
            dynamic h = DashboardHarness.Create(); h.Leader.IsKyCuoc = true;
            for(int i=0;i<20;i++) Assert.Contains("cửa vào",(string)h.Reason(1));
            Assert.True((bool)h.Leader.IsKyCuoc); Assert.Equal(0,(int)h.Leader.ManualCalls);
        }
        [Fact] public void LegacyTabIndexesAndComponentLifetimeAreProtected()
        {
            var source = DashboardHarness.Read("FrmMain.Dashboard.cs");
            Assert.Contains("TablControl.TabPages.Add(dungeonDashboardTab)",source);
            Assert.DoesNotContain("TabPages.Insert",source);
            Assert.Contains("new Timer(components)",source);
            Assert.Contains("SelectedItems.Count != 1",source);
            var legacy = DashboardHarness.Read("FrmMain.cs");
            Assert.Contains("TablControl.TabPages[6]",legacy); Assert.Contains("TablControl.TabPages[5]",legacy);
            var tree = CSharpSyntaxTree.ParseText(legacy).GetRoot();
            var init = tree.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="InitializeComponent");
            var tabs = init.DescendantNodes().OfType<InvocationExpressionSyntax>().Where(n=>n.Expression.ToString()=="this.TablControl.Controls.Add")
                .Select(n=>n.ArgumentList.Arguments[0].ToString()).ToArray();
            Assert.Equal(new[]{"this.tabtophop","this.tabkynang","this.tabvatpham","this.tabPage2","this.tabtienich","this.tabchedo","this.tabautologin"},tabs);
        }
    }

    internal static class DashboardHarness
    {
        private static readonly Lazy<Assembly> assembly = new Lazy<Assembly>(Compile);
        internal static dynamic Create() => Activator.CreateInstance(assembly.Value.GetType("TinhKiemAuto.FakeDashboard"));
        internal static string Read(string name) => File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"RegressionSources",name));
        private static Assembly Compile()
        {
            var refs = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator).Select(p=>MetadataReference.CreateFromFile(p));
            var trees = new[]{Read("DungeonDashboard.cs"),Read("Game.DungeonDashboard.cs"),Read("DungeonMovementPolicy.cs"),Fixture}.Select(s=>CSharpSyntaxTree.ParseText(s));
            var c = CSharpCompilation.Create("DashboardFakeIntegration",trees,refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var stream = new MemoryStream(); var result = c.Emit(stream);
            Assert.True(result.Success,string.Join("\n",result.Diagnostics.Where(d=>d.Severity==DiagnosticSeverity.Error)));
            return Assembly.Load(stream.ToArray());
        }
        private const string Fixture = @"
namespace TinhKiemAuto {
 public static class Global { public static bool Paused; }
 public static class MAP { public static int GiamNguc=77; }
 public class FakeTlbb { public string Name=""Fake"",Id=""fake-a"",KeyId=""fake-b""; public bool Online=true,IsLeader,IsFollow; public int PlayerState,MapId; }
 public partial class Game {
  public FakeTlbb TLBB = new FakeTlbb(); public Game Leader;
  public bool IsInit=true,IsAuto=true,ON_SCENE_TRANSING,IsChangeMap;
  private int[] dungeonPatrolDiagnostic=new[]{-1,0,0,0,0,0,0,0,0}; public string thuyLaoStep=""""; public bool IsTrungAc,IsKyCuoc,IsAcBa,IsThuyLao,IsLauLanTamBao,IsQ123LauLan,IsQ123ToChau,IsYenTuO,IsPhungHoangLangMo,IsPMP,IsTuBaoBon,IsLuyenKim,IsHuyetChien,IsBachHoaDuyen,IsSuMon,IsXayDung,IsTuDuong,IsNhiemVuCoBan,IsTrieuTap;
  public int MapAcTac,MapTKC,ManualSchool=-1,ManualCalls;
  private object trungAcLock=new object(),kyCuocLock=new object(),acTacLock=new object();
  private enum TrungAcPhase { Scan,Info,TravelTarget,Spawn,Combat,AcceptAck }
  private enum KyCuocPhase { Entry,Patrol,WaitingExit,Exit }
  private enum AcTacPhase { Entry,Patrol,AwaitBoss,AwaitExit }
  private TrungAcPhase trungAcPhase; private KyCuocPhase kyCuocPhase; private AcTacPhase acTacPhase;
  public string AcBaStatus => ""Chờ thông báo hệ thống"";
  public static int ChooseAcTacMap()=>5;
  public void SelectManualAcBaSchool(int school){ManualCalls++;ManualSchool=school;IsAcBa=true;}
 }
 public class FakeDashboard {
  public Game Actor=new Game(),Leader=new Game(); private GameDungeonDashboardPort port; private Dashboard.DungeonDashboardAdapter adapter;
  public FakeDashboard(){ Leader.TLBB.Id=Leader.TLBB.KeyId=""fake-b"";Leader.TLBB.IsLeader=true;Actor.Leader=Leader;Leader.Leader=Leader;port=new GameDungeonDashboardPort(()=>Actor,g=>g==Actor||g==Leader);adapter=new Dashboard.DungeonDashboardAdapter(port); }
  public bool CanStart(int kind)=>adapter.Read((Dashboard.DungeonKind)kind).CanStart;
  public string Reason(int kind)=>adapter.Read((Dashboard.DungeonKind)kind).Reason;
  public void Run(int kind,int option,bool start){var k=(Dashboard.DungeonKind)kind;adapter.Execute(k,option,start,port.Capture(k).Binding);}
  public void ApplyAfterTeamChange(){var binding=port.Capture(Dashboard.DungeonKind.KyCuoc).Binding;Actor.TLBB.KeyId=""fake-new-team"";try{port.Apply(Dashboard.DungeonKind.KyCuoc,0,true,binding);}catch(System.InvalidOperationException){} }
 }
}";
    }
}
