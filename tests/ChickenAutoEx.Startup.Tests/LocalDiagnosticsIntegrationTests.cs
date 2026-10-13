using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace ChickenAutoEx.Startup.Tests
{
    public sealed class LocalDiagnosticsIntegrationTests
    {
        private static Assembly Compile(string source, string fixture)
        {
            var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator)
                .Append(typeof(TinhKiemAuto.Diagnostics.DiagnosticRecorder).Assembly.Location).Distinct().Select(p => MetadataReference.CreateFromFile(p));
            var compilation = CSharpCompilation.Create("OwnedDiagnosticsUi" + Guid.NewGuid().ToString("N"),
                new[] { CSharpSyntaxTree.ParseText(source), CSharpSyntaxTree.ParseText(fixture) }, references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var bytes = new MemoryStream(); var result = compilation.Emit(bytes);
            Assert.True(result.Success, string.Join("\n", result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)));
            return Assembly.Load(bytes.ToArray());
        }
        [Theory] [InlineData(6, true)] [InlineData(7, false)] [InlineData(2, false)] [InlineData(0, false)]
        public void ActualConsentDialogRequiresExplicitYesAndDefaultsToNo(int returned, bool expected)
        {
            var root = CSharpSyntaxTree.ParseText(DashboardHarness.Read("DiagnosticsHost.cs")).GetRoot();
            var ask = root.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text == "Ask");
            var consent = root.DescendantNodes().OfType<FieldDeclarationSyntax>().Single(f => f.Declaration.Variables.Any(v => v.Identifier.Text == "ConsentText"));
            string source = "using System;using System.Drawing;namespace TinhKiemAuto.Diagnostics {public class Probe {private Form owner=new Form();"
                + consent + ask + "public bool Run(){return Ask();}}}";
            var assembly = Compile(source, DialogFixture);
            var form = assembly.GetType("TinhKiemAuto.Diagnostics.Form"); form.GetField("ReturnValue").SetValue(null, returned);
            dynamic probe = Activator.CreateInstance(assembly.GetType("TinhKiemAuto.Diagnostics.Probe"));
            Assert.Equal(expected, (bool)probe.Run()); dynamic dialog = form.GetField("LastShown").GetValue(null);
            Assert.Equal(7, (int)dialog.AcceptButton.DialogResult); Assert.Equal(7, (int)dialog.CancelButton.DialogResult);
            Assert.Equal("Không", (string)dialog.AcceptButton.Text); Assert.True((bool)dialog.Disposed);
        }
        [Fact] public void GameProjectionReadsCachedFlagsWithoutTouchingMemoryOrChangingAutomation()
        {
            var assembly = Compile(DashboardHarness.Read("Game.LocalDiagnostics.cs"), GameFixture);
            dynamic game = Activator.CreateInstance(assembly.GetType("TinhKiemAuto.Game"));
            var type = ((object)game).GetType();
            var flags = (bool[])type.GetMethod("DiagnosticFlags", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(game, null);
            var phases = (int[])type.GetMethod("DiagnosticPhases", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(game, null);
            var acTac = (int[])type.GetMethod("DiagnosticAcTac", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(game, null);
            Assert.Equal(new[] {1, 1, 3, 2, 1, 0, 0, 0}, acTac);
            Assert.Equal(13, flags.Length); Assert.Equal(new[] { 3, 0, 2, 4, 1, 3 }, phases);
            Assert.True(flags[0]); Assert.True(flags[2]); Assert.True(flags[3]); Assert.True(flags[7]); Assert.True(flags[12]);
            Assert.Equal(0, (int)game.MemoryReads); Assert.Equal(0, (int)game.Actions);
            Assert.True((bool)game.IsAuto); Assert.True((bool)game.IsCheDo);
        }
        [Fact] public void SamplingAndErrorObservationDoNotReplaceWinFormsExceptionHandlingOrUploadData()
        {
            string host = DashboardHarness.Read("DiagnosticsHost.cs");
            Assert.Contains("FirstChanceException +=", host); Assert.Contains("UnhandledException +=", host);
            Assert.DoesNotContain("ThreadException +=", host); Assert.DoesNotContain("SetUnhandledExceptionMode", host);
            Assert.DoesNotContain("Exception.Message", host); Assert.DoesNotContain("Exception.ToString", host);
            Assert.DoesNotContain("HttpClient", host); Assert.DoesNotContain("WebRequest", host); Assert.DoesNotContain("SendMessage", host);
            Assert.Contains("consent.Decision == DiagnosticConsent.Accepted", host);
        }
        [Fact] public void ActualAcTacSamplerRecordsAnonymousCountsAndPreservesStopReason()
        {
            var root=CSharpSyntaxTree.ParseText(DashboardHarness.Read("DiagnosticsHost.cs")).GetRoot();
            var method=root.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="SampleAcTac");
            string source="using System;using System.Collections.Generic;namespace TinhKiemAuto.Diagnostics {public class Probe {"
                +"private Dictionary<int,string> previousAcTac=new Dictionary<int,string>();"+method
                +"public void Run(DiagnosticRecorder r,int[] values){SampleAcTac(r,new DiagnosticWindow{Alias=3,AcTac=values});}}"
                +"public class DiagnosticWindow {public int Alias;public int[] AcTac;} }";
            var assembly=Compile(source,"");dynamic probe=Activator.CreateInstance(assembly.GetType("TinhKiemAuto.Diagnostics.Probe"));
            var store=new TaskStore();using var recorder=new TinhKiemAuto.Diagnostics.DiagnosticRecorder("0.14",store,false);
            var reason=TinhKiemAuto.Diagnostics.DiagnosticReason.CharacterUnavailable;
            int[] values={1,1,3,2,1,0,1,(int)reason};probe.Run(recorder,values);probe.Run(recorder,values);recorder.Drain();
            Assert.Equal(5,store.Rows.Count);Assert.All(store.Rows,row=>Assert.Contains("window_0003",row));
            Assert.Contains(store.Rows,row=>row.Contains("TaskState")&&row.Contains("On,1,3,CharacterUnavailable"));
            values=new[]{1,0,3,2,1,0,1,(int)TinhKiemAuto.Diagnostics.DiagnosticReason.TeamChanged};probe.Run(recorder,values);recorder.Drain();
            Assert.Equal(10,store.Rows.Count);Assert.Contains(store.Rows,row=>row.Contains("Off,1,3,TeamChanged"));
        }
        private sealed class TaskStore:TinhKiemAuto.Diagnostics.IDiagnosticStore
        { public System.Collections.Generic.List<string> Rows=new System.Collections.Generic.List<string>();public void Write(string row)=>Rows.Add(row);public void Dispose(){} }
        private const string GameFixture = @"
using System;using System.Collections.Generic;
namespace TinhKiemAuto.Diagnostics {public class Placeholder{}}
namespace TinhKiemAuto {public partial class Game {
 public bool IsAuto=true,IsAttack,IsCheDo=true,IsPet,IsPickItem,IsHP,IsMP,IsNM,IsTrungAc=true,IsHooked=true;
 public List<int> Skills=new List<int>{1};private bool thuyLaoEnabled,kyCuocEnabled,isAcBa;
 private int mapAcTac,trungAcPhase=3,kyCuocPhase=2,acTacPhase=4,acBaPhase=1;
 private int[] acTacDiagnostic=new[]{1,1,3,2,1,0,0,0};
 private FakeHook hookDiagnostics=new FakeHook();public int MemoryReads,Actions;
 public object TLBB {get{MemoryReads++;throw new Exception(""must not read game memory"");}}
 public void Move(){Actions++;}public int MapAcTac {get{MemoryReads++;return 999;}}
} public class FakeHook {public int Reader=>3;} }
";
        private const string DialogFixture = @"
using System;using System.Collections.Generic;using System.Drawing;
namespace TinhKiemAuto.Appearance {public static class AppAppearance {public static void Apply(object form){}}}
namespace TinhKiemAuto.Diagnostics {
 public enum FormStartPosition {CenterParent}public enum FormBorderStyle{FixedDialog} public enum AutoScaleMode{Font}
 public enum SizeType{Percent,Absolute}public enum FlowDirection{RightToLeft}public enum ScrollBars{Vertical}
 public enum DockStyle{Fill}public enum DialogResult{None=0,Cancel=2,Yes=6,No=7}
 public class Items:List<object>{public void Add(object control,int x,int y){Add(control);}}
 public class Control {public DockStyle Dock;public string Text;public Items Controls=new Items();}
 public class Form:Control,IDisposable {public static int ReturnValue;public static Form LastShown;
  public FormStartPosition StartPosition;public Size ClientSize;public bool MinimizeBox,MaximizeBox,Disposed;
  public FormBorderStyle FormBorderStyle;public AutoScaleMode AutoScaleMode;public Button AcceptButton,CancelButton;
  public DialogResult ShowDialog(Form owner){LastShown=this;return (DialogResult)ReturnValue;}public void Dispose(){Disposed=true;}}
 public class Padding{public Padding(int size){}}
 public class RowStyle{public RowStyle(SizeType type,int value){}}
 public class TableLayoutPanel:Control {public Padding Padding;public int RowCount,ColumnCount;public List<RowStyle> RowStyles=new List<RowStyle>();}
 public class TextBox:Control {public bool Multiline,ReadOnly,WordWrap;public ScrollBars ScrollBars;public Color BackColor;}
 public class FlowLayoutPanel:Control{public FlowDirection FlowDirection;}
 public class Button:Control{public DialogResult DialogResult;public int Width,Height;}
}
";
    }
}
