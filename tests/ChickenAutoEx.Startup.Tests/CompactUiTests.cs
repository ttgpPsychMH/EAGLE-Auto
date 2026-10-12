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
    public sealed class CompactUiTests
    {
        [Theory][InlineData(0)][InlineData(2)]
        public void NoExactSelectionNeverShowsExampleVitalsOrEnablesSettings(int count)
        {
            var h = CharacterHeader.Empty(count);
            Assert.False(h.CanConfigure);
            Assert.Equal("—",h.HP); Assert.Equal("—",h.MP); Assert.Equal("—",h.Pet);
            Assert.Equal("—",h.Position); Assert.Equal("—",h.Online);
        }
        [Theory][InlineData(false)][InlineData(true)]
        public void OfflineOrInitializingClearsAllPreviouslyObservedMetrics(bool online)
        {
            var h = CharacterHeader.Unavailable("Mẫu A",online);
            Assert.False(h.CanConfigure); Assert.Equal("Mẫu A",h.Title);
            Assert.Equal("—",h.HP); Assert.Equal("—",h.MP); Assert.Equal("—",h.Pet);
            Assert.Equal("—",h.Position); Assert.Equal("—",h.Online);
        }
        [Theory][InlineData(-1)][InlineData(101)][InlineData(int.MaxValue)]
        public void InvalidReadValuesAreUnknownInsteadOfInventedPercentages(int value)
        {
            var h = CharacterHeader.Live("Mẫu",value,value,value,null,-1,false);
            Assert.Equal("—",h.HP); Assert.Equal("—",h.MP); Assert.Equal("—",h.Pet);
            Assert.Equal("—",h.Position); Assert.Equal("—",h.Online);
        }
        [Theory][InlineData(true)][InlineData(false)]
        public void ReadyCharacterCanBeConfiguredBeforeOrAfterAutoIsEnabled(bool auto)
        {
            var h = CharacterHeader.Live("Mẫu",0,100,90,"Đại Lý [218,152]",3661,auto);
            Assert.True(h.CanConfigure); Assert.Equal("0%",h.HP); Assert.Equal("100%",h.MP);
            Assert.Equal("90%",h.Pet); Assert.Equal("01:01:01",h.Online);
            Assert.Contains(auto ? "đang bật" : "đang tắt",h.Hint);
        }
        [Fact] public void OnlineTimeDoesNotWrapAfterOneDay()
        { Assert.Equal("100:00:00",CharacterHeader.Live("Mẫu",1,2,3,"Map",360000,true).Online); }
        [Theory][InlineData(float.NaN,1f)][InlineData(1f,float.PositiveInfinity)][InlineData(float.MaxValue,1f)]
        public void InvalidCoordinatesAreNotDisplayedAsZeroOrIntMin(float x,float y)
        { Assert.Equal("—",CharacterHeader.Location("Đại Lý",x,y)); }
        [Fact] public void KnownZeroCoordinatesAreValid()
        { Assert.Equal("Đại Lý [0,0]",CharacterHeader.Location("Đại Lý",0,0)); }
        [Theory][InlineData(481f,220f,180f)][InlineData(650f,220f,180f)][InlineData(680f,275f,225f)]
        public void SharedPanelAndLogCannotConsumeMinimumContentHeight(float height,float reserved,float minimum)
        {
            var b = CompactPanelBudget.Allocate(height,reserved,minimum,132,76);
            Assert.True(height-reserved-b.Shared-b.Log >= minimum);
            Assert.InRange(b.Shared,0,132); Assert.InRange(b.Log,0,76);
        }
        [Fact] public void LogPreferenceRestoresAfterPanelClosesOrWindowGrows()
        {
            Assert.Equal(0,CompactPanelBudget.Allocate(481,220,180,132,76).Log);
            Assert.Equal(76,CompactPanelBudget.Allocate(481,220,180,0,76).Log);
            Assert.Equal(76,CompactPanelBudget.Allocate(700,220,180,132,76).Log);
        }
        [Fact] public void VeryShortWindowFoldsOptionalRegions()
        {
            var b = CompactPanelBudget.Allocate(350,220,180,132,76);
            Assert.Equal(0,b.Shared); Assert.Equal(0,b.Log);
        }
        [Theory][InlineData(0)][InlineData(2)]
        public void ActualUiMethodsDoNotUseRetainedCurGameWithoutOneSelectedRow(int count)
        {
            dynamic f = CompactHeaderHarness.Create(); f.SelectRows(count); f.Refresh();
            Assert.Equal("HP —",(string)f.HpText);
            Assert.True((bool)f.AllSettingsDisabled); Assert.False((bool)f.SummonEnabled);
            Assert.True((bool)f.LoginEnabled); Assert.True((bool)f.TasksEnabled);
            Assert.Equal(0,(int)f.Actor.NativeReads);
        }
        [Fact] public void ActualUiMethodsRecoverAfterSelectionAndOfflineTransition()
        {
            dynamic f = CompactHeaderHarness.Create(); f.SelectRows(1); f.Refresh();
            Assert.Equal("HP 99%",(string)f.HpText); Assert.True((bool)f.SettingsEnabled);
            f.Actor.TLBB.Online=false; f.Refresh();
            Assert.Equal("HP —",(string)f.HpText); Assert.False((bool)f.SettingsEnabled);
            f.Actor.TLBB.Online=true; f.Refresh();
            Assert.True((bool)f.SettingsEnabled);
        }
        [Theory][InlineData("registration")][InlineData("selection")][InlineData("init")][InlineData("identity")]
        public void ActualHeaderReadRejectsStaleCandidate(string change)
        {
            dynamic f = CompactHeaderHarness.Create(); f.SelectRows(1); f.Invalidate(change); f.Refresh();
            Assert.Equal("HP —",(string)f.HpText); Assert.False((bool)f.SettingsEnabled);
        }
        [Fact] public void HeaderEnablementNeverWritesLegacyCheckboxesOrNumericSettings()
        {
            // Fixture setters throw if the real extracted method writes any legacy setting.
            dynamic f = CompactHeaderHarness.Create(); f.Refresh(); f.SelectRows(1); f.Refresh();
            f.SelectRows(0); f.Refresh();
            Assert.True((bool)f.AllSettingsDisabled); Assert.True((bool)f.LoginEnabled); Assert.True((bool)f.TasksEnabled);
        }
    }

    internal static class CompactHeaderHarness
    {
        private static readonly Lazy<Assembly> assembly = new Lazy<Assembly>(Compile);
        internal static dynamic Create() => Activator.CreateInstance(assembly.Value.GetType("TinhKiemAuto.FrmMain"));
        private static Assembly Compile()
        {
            var tree = CSharpSyntaxTree.ParseText(DashboardHarness.Read("FrmMain.Dashboard.cs")).GetRoot();
            string[] names={"DashboardSelectedGame","ReadCharacterHeader","ApplyCharacterHeader"};
            var methods = names.Select(name=>tree.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text==name).ToFullString());
            var source = "using System;using TinhKiemAuto.Dashboard;namespace TinhKiemAuto { public partial class FrmMain {"+string.Join("\n",methods)+"}}";
            var refs=((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator).Select(p=>MetadataReference.CreateFromFile(p));
            var texts=new[]{DashboardHarness.Read("CharacterHeader.cs"),source,Fixture}.Select(s=>CSharpSyntaxTree.ParseText(s));
            var c=CSharpCompilation.Create("CompactHeaderFakeUi",texts,refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var bytes=new MemoryStream();var result=c.Emit(bytes);
            Assert.True(result.Success,string.Join("\n",result.Diagnostics.Where(d=>d.Severity==DiagnosticSeverity.Error)));
            return Assembly.Load(bytes.ToArray());
        }
        private const string Fixture=@"
using System.Linq;
namespace TinhKiemAuto {
 public sealed class FakeLabel { public string Text,ToolTipText; public bool Enabled=true; }
 public sealed class FakePage {
  public bool Enabled=true;
  public bool Checked {get=>true;set=>throw new System.Exception(""Unexpected setting write"");}
  public int Value {get=>50;set=>throw new System.Exception(""Unexpected setting write"");}
 }
 public sealed class FakeTabs { public FakePage[] TabPages=System.Linq.Enumerable.Range(0,8).Select(_=>new FakePage()).ToArray(); }
 public sealed class FakeItem { public object Tag; }
 public sealed class FakeList { public System.Collections.Generic.List<FakeItem> SelectedItems=new System.Collections.Generic.List<FakeItem>(); }
 public sealed class FakeTooltip { public void SetToolTip(FakeLabel label,string value){} }
 public sealed class FakeCharacter {
  public string Name=""Mẫu A"",MapName=""Đại Lý"";private string id=""fake-a"";private int reads;
  public bool Online=true;public int OnlineTimeSec=3661,HPPercent=99,MPPercent=86,PetHPPercent=90;
  public System.Action OnFinalIdentityRead;
  public string Id {get { if(++reads==2)OnFinalIdentityRead?.Invoke();return id; }}
  public void ChangeIdentity(){id=""fake-b"";}
 }
 public sealed class Game {
  public FakeCharacter TLBB=new FakeCharacter(); public bool IsAuto=true,DashboardInitialized=true;public int NativeReads;
  public float CharX {get{NativeReads++;return 218;}} public float CharY=>152;
 }
 public partial class FrmMain {
  private FakeList ListViewNhanVat=new FakeList();private Game CurGame;public Game Actor=new Game();
  private System.Collections.Generic.Dictionary<int,Game> dicGame=new System.Collections.Generic.Dictionary<int,Game>();
  private FakeTabs TablControl=new FakeTabs();private FakeTooltip toolTip1=new FakeTooltip();
  private FakeLabel dashboardSelection=new FakeLabel(),headerHP=new FakeLabel(),headerMP=new FakeLabel(),headerPet=new FakeLabel(),headerPosition=new FakeLabel(),headerOnline=new FakeLabel(),compactStatus=new FakeLabel(),buttrieutap=new FakeLabel();
  public FrmMain(){CurGame=Actor;dicGame.Add(1,Actor);}
  public void SelectRows(int count){ListViewNhanVat.SelectedItems.Clear();for(int i=0;i<count;i++)ListViewNhanVat.SelectedItems.Add(new FakeItem{Tag=Actor});}
  public void Refresh(){ApplyCharacterHeader(ReadCharacterHeader());}
  public string HpText=>headerHP.Text;public bool SettingsEnabled=>TablControl.TabPages.Take(6).All(p=>p.Enabled);public bool AllSettingsDisabled=>TablControl.TabPages.Take(6).All(p=>!p.Enabled);
  public bool LoginEnabled=>TablControl.TabPages[6].Enabled;public bool TasksEnabled=>TablControl.TabPages[7].Enabled;public bool SummonEnabled=>buttrieutap.Enabled;
  public void Invalidate(string kind){
   if(kind==""registration"")dicGame.Clear();
   if(kind==""selection"")CurGame=new Game();
   if(kind==""init"")Actor.TLBB.OnFinalIdentityRead=()=>Actor.DashboardInitialized=false;
   if(kind==""identity"")Actor.TLBB.OnFinalIdentityRead=()=>Actor.TLBB.ChangeIdentity();
  }
 }
}";
    }
}
