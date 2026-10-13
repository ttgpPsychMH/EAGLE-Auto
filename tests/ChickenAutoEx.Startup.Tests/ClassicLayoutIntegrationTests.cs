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
    public sealed class ClassicLayoutIntegrationTests
    {
        private static readonly Lazy<Assembly> Compiled=new Lazy<Assembly>(Compile);
        [Theory][InlineData(96,350)][InlineData(120,350)][InlineData(144,350)][InlineData(192,350)][InlineData(96,320)]
        public void ActualCompactRecoveryLayoutKeepsCheckboxesThresholdsAndValuesSeparate(int dpi,int logicalWidth)
        {
            dynamic f=Activator.CreateInstance(Compiled.Value.GetType("TinhKiemAuto.FrmMain"));
            f.DeviceDpi=dpi;f.Apply(3,logicalWidth*dpi/96);
            Array controls=f.Cells(3);
            foreach(dynamic c in controls){Assert.True((int)c.Width>0);Assert.True((int)c.Height>0);Assert.Equal(50,(int)c.Value);Assert.True((int)c.Right<=logicalWidth*dpi/96);Assert.True((int)c.Bottom<=(int)f.GroupHeight(3));}
            var items=controls.Cast<object>().ToArray();
            for(int i=0;i<items.Length;i++)for(int j=i+1;j<items.Length;j++)
            {dynamic a=items[i],b=items[j];Assert.False((bool)(a.Left<b.Right&&a.Right>b.Left&&a.Top<b.Bottom&&a.Bottom>b.Top));}
        }
        [Theory][InlineData(96)][InlineData(120)][InlineData(144)][InlineData(192)]
        public void ActualCompactAttackLayoutPreservesControlParentsAndScales(int dpi)
        {
            dynamic f=Activator.CreateInstance(Compiled.Value.GetType("TinhKiemAuto.FrmMain"));f.DeviceDpi=dpi;f.Apply(1,350*dpi/96);
            foreach(dynamic c in (Array)f.Cells(1)){Assert.True((int)c.Right<=350*dpi/96);Assert.True((int)c.Bottom<=(int)f.GroupHeight(1));Assert.Equal(50,(int)c.Value);}
            Assert.Equal(82*dpi/96,(int)f.GroupHeight(1));
        }
        private static Assembly Compile()
        {
            var root=CSharpSyntaxTree.ParseText(DashboardHarness.Read("FrmMain.ClassicLayout.cs")).GetRoot();
            var method=root.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m=>m.Identifier.Text=="ArrangeClassicGroup");
            string source="using System;namespace TinhKiemAuto {"+Fixture+"public partial class FrmMain {private bool arrangingClassicGroup;"+method+Fields+"}}";
            var refs=((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator).Select(p=>MetadataReference.CreateFromFile(p));
            var c=CSharpCompilation.Create("ClassicLayoutFakeUi",new[]{CSharpSyntaxTree.ParseText(source)},refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var bytes=new MemoryStream();var result=c.Emit(bytes);Assert.True(result.Success,string.Join("\n",result.Diagnostics.Where(d=>d.Severity==DiagnosticSeverity.Error)));return Assembly.Load(bytes.ToArray());
        }
        private const string Fixture="""
 public struct Size {public int Width;}
 public class Control {public bool AutoSize;public int Left,Top,Width,Height;public int Right=>Left+Width;public int Bottom=>Top+Height;public int Value{get=>50;set=>throw new Exception("setting write");}public void SetBounds(int x,int y,int w,int h){Left=x;Top=y;Width=w;Height=h;}}
 public class GroupBox:Control {public Size ClientSize;}
""";
        private const string Fields="""
 public int DeviceDpi=96;private GroupBox groupBox1=new GroupBox(),groupBox3=new GroupBox();
 private Control chekcdanhquai=new Control(),butboqua=new Control(),checkradius=new Control(),numberdanhquanh=new Control(),rad11=new Control(),radgom=new Control();
 private Control checkregenhp=new Control(),label14=new Control(),nudHP=new Control(),checkrengenmp=new Control(),label13=new Control(),nudMP=new Control(),checkcongsinh=new Control(),label15=new Control(),numbercongsinhhp=new Control(),checkhuyette=new Control(),label16=new Control(),numhuyettemp=new Control(),checkisNM=new Control(),label17=new Control(),nudNM=new Control(),checkBox7=new Control(),label18=new Control(),numcanhbaohp=new Control(),CheckAutoHoiSinh=new Control(),CheckTriLieuComeback=new Control();
 public void Apply(int group,int width){var g=group==1?groupBox1:groupBox3;g.ClientSize=new Size{Width=width};ArrangeClassicGroup(g);}
 public int GroupHeight(int group)=>(group==1?groupBox1:groupBox3).Height;
 public Control[] Cells(int group)=>group==1?new[]{chekcdanhquai,butboqua,checkradius,numberdanhquanh,rad11,radgom}:new[]{checkregenhp,label14,nudHP,checkrengenmp,label13,nudMP,checkcongsinh,label15,numbercongsinhhp,checkhuyette,label16,numhuyettemp,checkisNM,label17,nudNM,checkBox7,label18,numcanhbaohp,CheckAutoHoiSinh,CheckTriLieuComeback};
""";
    }
}
