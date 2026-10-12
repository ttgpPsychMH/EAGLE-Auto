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
    public sealed class TrayAppearanceTests
    {
        [Fact]
        public void ThemedTrayRetainsOriginalShowAndExitHandlers()
        {
            var tree=CSharpSyntaxTree.ParseText(DashboardHarness.Read("FrmMain.cs")).GetRoot();
            var methods=tree.DescendantNodes().OfType<MethodDeclarationSyntax>().ToArray();
            var startup=methods.Single(m=>m.Identifier.Text=="InitializeStartup");
            var statements=startup.Body.Statements.Where(s=>s.ToString().StartsWith("notifyIcon1.ContextMenuStrip")
                || s.ToString()=="TinhKiemAuto.Appearance.AppAppearance.Apply(notifyIcon1.ContextMenuStrip);").ToArray();
            Assert.Equal(4,statements.Length);
            var handlers=methods.Where(m=>m.Identifier.Text=="HienAuto"||m.Identifier.Text=="Thoat");
            string source="using System;namespace TinhKiemAuto { public partial class FrmMain {public void Initialize(){"+string.Join("\n",statements.Select(s=>s.ToString()))+"}"+string.Join("\n",handlers.Select(m=>m.ToFullString()))+"}}";
            var references=((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator).Select(p=>MetadataReference.CreateFromFile(p));
            var c=CSharpCompilation.Create("OwnedTrayUiSimulation",new[]{CSharpSyntaxTree.ParseText(source),CSharpSyntaxTree.ParseText(Fixture)},references,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var bytes=new MemoryStream();var result=c.Emit(bytes);
            Assert.True(result.Success,string.Join("\n",result.Diagnostics.Where(d=>d.Severity==DiagnosticSeverity.Error)));
            var assembly=Assembly.Load(bytes.ToArray());dynamic form=Activator.CreateInstance(assembly.GetType("TinhKiemAuto.FrmMain"));form.Initialize();
            Assert.Equal(1,(int)form.Applies);Assert.Equal(0,(int)form.Shows);Assert.Equal(0,(int)form.Exits);
            Assert.Equal("Hiện Auto",(string)form.notifyIcon1.ContextMenuStrip.Items[0].Text);
            Assert.Equal("Thoát Auto",(string)form.notifyIcon1.ContextMenuStrip.Items[1].Text);
            form.notifyIcon1.ContextMenuStrip.Items[0].Click();Assert.Equal(1,(int)form.Shows);Assert.Equal(0,(int)form.Exits);
            form.notifyIcon1.ContextMenuStrip.Items[1].Click();Assert.Equal(1,(int)form.Exits);
        }
        private const string Fixture=@"
namespace TinhKiemAuto {
 public sealed class FakeNotifyIcon {public ContextMenuStrip ContextMenuStrip;}
 public sealed class ContextMenuStrip {public System.Collections.Generic.List<ToolStripMenuItem> Items=new System.Collections.Generic.List<ToolStripMenuItem>();public ContextMenuStrip(object container){} }
 public sealed class ToolStripMenuItem {public string Text;private System.EventHandler handler;public ToolStripMenuItem(string text,object image,System.EventHandler click){Text=text;handler=click;}public void Click(){handler(this,System.EventArgs.Empty);} }
 public static class Application {public static int Exits;public static void Exit(){Exits++;}}
 public partial class FrmMain {private object components=new object();public FakeNotifyIcon notifyIcon1=new FakeNotifyIcon();public int Shows;public void Show(){Shows++;}public int Exits=>Application.Exits;public int Applies=>Appearance.AppAppearance.Applies;}
}
namespace TinhKiemAuto.Appearance {public static class AppAppearance {public static int Applies;public static void Apply(object menu){Applies++;}}}
";
    }
}
