using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
namespace ChickenAutoEx.Startup.Tests
{
    public sealed class FixedLayoutIntegrationTests
    {
        [Fact] public void ActualLayoutSeparatesCaptionsSymbolsInputsAndButtons()
        {
            dynamic f=FixedLayoutHarness.Create();f.Apply();Assert.True((bool)f.NoOverlaps);
            Assert.True((bool)f.AllContentsContained);Assert.True((bool)f.SameParents);
            Assert.True((bool)f.ValuesUntouched);
        }
        [Fact] public void ActualLayoutDoesNotGrowOnRepeatedRefreshOrReregister()
        {
            dynamic f=FixedLayoutHarness.Create();f.Apply();string before=f.Geometry();
            f.Apply();f.Apply();f.Register();f.Apply();Assert.Equal(before,(string)f.Geometry());
        }
        [Fact] public void ActualLayoutExpandsForChangedCaptionThenRecoversWhenShortened()
        {
            dynamic f=FixedLayoutHarness.Create();f.Apply();string before=f.Geometry();
            f.ChangeCaption(true);f.Apply();Assert.True((bool)f.NoOverlaps);Assert.True((bool)f.AllContentsContained);
            f.ChangeCaption(false);f.Apply();Assert.Equal(before,(string)f.Geometry());
        }
        [Theory][InlineData(120)][InlineData(144)][InlineData(192)]
        public void ActualLayoutKeepsFullTextAtHigherDpi(int dpi)
        {
            dynamic f=FixedLayoutHarness.Create();f.Apply();f.SetDpi(dpi);f.Apply();
            Assert.True((bool)f.NoOverlaps);Assert.True((bool)f.AllContentsContained);Assert.True((bool)f.ValuesUntouched);
        }
        [Fact] public void ActualScrollablePageGetsRoomWithoutChangingSelectionOrAuto()
        {
            dynamic f=FixedLayoutHarness.Create();f.ApplyPage();Assert.True((bool)f.Page.AutoScroll);
            Assert.True((bool)f.PageContentsContained);Assert.True((bool)f.ValuesUntouched);
        }
    }
    internal static class FixedLayoutHarness
    {
        private static readonly Lazy<Assembly> compiled=new Lazy<Assembly>(Compile);
        internal static dynamic Create()=>Activator.CreateInstance(compiled.Value.GetType("TinhKiemAuto.Appearance.FixedLayoutFixture"));
        private static Assembly Compile()
        {
            var texts=new[]{DashboardHarness.Read("UiFixedLayout.cs"),DashboardHarness.Read("RowLayoutPolicy.cs"),FakeUi}.Select(s=>CSharpSyntaxTree.ParseText(s));
            var refs=((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator).Select(p=>MetadataReference.CreateFromFile(p));
            var c=CSharpCompilation.Create("OwnedFixedLayoutFakeUi",texts,refs,new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var bytes=new MemoryStream();var result=c.Emit(bytes);
            Assert.True(result.Success,string.Join("\n",result.Diagnostics.Where(d=>d.Severity==DiagnosticSeverity.Error)));
            return Assembly.Load(bytes.ToArray());
        }
        private const string FakeUi=@"
using System;using System.Collections.Generic;using System.Linq;using System.Drawing;
namespace System.Windows.Forms {public class Placeholder {}}
namespace TinhKiemAuto.Appearance {
 [Flags]public enum AnchorStyles{None=0,Top=1,Left=2,Right=4,Bottom=8} public enum DockStyle{None,Fill}
 [Flags]public enum TextFormatFlags{NoPrefix=1,WordBreak=2,SingleLine=4}
 public class Font {public float Size=10;public float GetHeight()=>Size*1.5f;public float GetHeight(float dpi)=>GetHeight();}
 public static class TextRenderer {public static Size MeasureText(string text,Font font,Size limit,TextFormatFlags flags)=>new Size((int)Math.Ceiling(text.Length*font.Size*.7f),(int)Math.Ceiling(font.GetHeight()));}
 public class Control {
  public string Name="""",Text="""";public int Left,Top,Width=80,Height=20,DeviceDpi=96;public Font Font=new Font();
  public bool IsDisposed,IsHandleCreated,InvokeRequired;public DockStyle Dock;public AnchorStyles Anchor=AnchorStyles.Top|AnchorStyles.Left;
  public Control Parent;public List<Control> Controls=new List<Control>();public Size ClientSize=>new Size(Width,Height);public Size MinimumSize;
  public int Right=>Left+Width;public int Bottom=>Top+Height;public Rectangle Bounds=>new Rectangle(Left,Top,Width,Height);
  public event EventHandler Layout,FontChanged,TextChanged;public void RaiseLayout()=>Layout?.Invoke(this,EventArgs.Empty);
  public void SetBounds(int x,int y,int w,int h){Left=x;Top=y;Width=w;Height=h;}
  public bool Checked{get=>true;set=>throw new Exception(""Checked write"");}public int Value{get=>50;set=>throw new Exception(""Value write"");}
  public int SelectedIndex{get=>1;set=>throw new Exception(""Selection write"");}
 }
 public struct Padding{public int Horizontal;}public class TableLayoutPanel:Control{public Padding Padding;}
 public class ScrollableControl:Control{public Point AutoScrollPosition;public bool AutoScroll;public Size AutoScrollMinSize;}
 public class TabPage:ScrollableControl{}public class GroupBox:Control{}public class TabControl:Control{}
 public class Label:Control{public bool AutoSize;}public class ButtonBase:Control{public bool AutoEllipsis;}public class Button:ButtonBase{}public class CheckBox:ButtonBase{}public class RadioButton:ButtonBase{}
 public class NumericUpDown:Control{}public class ComboBox:Control{}public class TextBoxBase:Control{}
 internal static class AppAppearance{internal static void Apply(Control c)=>UiFixedLayout.Refresh(c);}
 public class FixedLayoutFixture {
  public GroupBox Group=new GroupBox{Width=360,Height=106,Name=""groupBox3""};public TabPage Page=new TabPage{Width=250,Height=180,Name=""tabvatpham""};
  public CheckBox Caption=new CheckBox{Text=""Phục hồi HP"",Left=11,Top=12,Width=91,Height=17};
  public Label Symbol=new Label{Text=""<"",Left=104,Top=13,Width=13,Height=13};
  public NumericUpDown Input=new NumericUpDown{Left=120,Top=11,Width=45,Height=20};
  public Button Action=new Button{Text=""Trị liệu"",Left=199,Top=12,Width=78,Height=23};
  public CheckBox LongCaption=new CheckBox{Text=""Tự hồi sinh khi chết"",Left=199,Top=34,Width=125,Height=17};
  public FixedLayoutFixture(){foreach(var c in new Control[]{Caption,Symbol,Input,Action,LongCaption}){c.Parent=Group;Group.Controls.Add(c);}var b=new Button{Text=""Một nút cấu hình khác"",Top=20,Left=20};b.Parent=Page;Page.Controls.Add(b);Register();}
  public void Register(){UiFixedLayout.RegisterTree(Group);UiFixedLayout.RegisterTree(Page);}
  public void Apply()=>UiFixedLayout.Refresh(Group);public void ApplyPage()=>UiFixedLayout.Refresh(Page);
  public bool NoOverlaps{get{var cs=Group.Controls;for(int i=0;i<cs.Count;i++)for(int j=i+1;j<cs.Count;j++)if(cs[i].Bounds.IntersectsWith(cs[j].Bounds))return false;return true;}}
  public bool AllContentsContained=>Group.Controls.All(c=>c.Right<=Group.MinimumSize.Width&&c.Bottom<=Group.MinimumSize.Height);
  public bool PageContentsContained=>Page.Controls.All(c=>c.Right<=Page.AutoScrollMinSize.Width&&c.Bottom<=Page.AutoScrollMinSize.Height);
  public bool SameParents=>Group.Controls.All(c=>ReferenceEquals(c.Parent,Group));public bool ValuesUntouched=>Input.Value==50&&Caption.Checked&&Input.SelectedIndex==1;
  public string Geometry()=>string.Join(""|"",Group.Controls.Select(c=>c.Bounds.ToString()))+Group.MinimumSize;
  public void ChangeCaption(bool longer)=>Caption.Text=longer?""Phục hồi HP với một nhãn dài hơn trước"":""Phục hồi HP"";
  public void SetDpi(int dpi){Group.DeviceDpi=dpi;foreach(var c in Group.Controls){c.DeviceDpi=dpi;c.Font.Size=10*dpi/96f;}}
 }
}
";
    }
}
