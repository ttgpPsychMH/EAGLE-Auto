using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TinhKiemAuto.Appearance;
using Xunit;

namespace ChickenAutoEx.Startup.Tests
{
    public sealed class TypographyTests
    {
        [Theory]
        [InlineData("Nhân vật", 1, "NHÂN VẬT")]
        [InlineData("Kỹ Năng", 2, "Kỹ Năng")]
        [InlineData("Lên Bãi", 0, "Lên bãi")]
        [InlineData("Phân Cứng :", 0, "Phần cứng:")]
        [InlineData("Rao Chát", 0, "Rao chat")]
        [InlineData("Sử dụng thổ linh châu để lên bãi nhanh", 0, "Dùng Thổ Linh Châu để lên bãi nhanh")]
        [InlineData("Phục Hồi HP", 2, "Phục Hồi HP")]
        [InlineData("Bật Tắt Nhặt Đồ :  Ctrl + Shift + C", 0, "Bật/tắt nhặt đồ: Ctrl + Shift + C")]
        [InlineData("Buff NM HP", 2, "Buff NM HP")]
        public void FixedCopyUsesRequestedRole(string input, int role, string expected)
        {
            Assert.True(UiCopyPolicy.TryFormat(input, (CopyRole)role, out var output));
            Assert.Equal(expected, output);
        }

        [Fact]
        public void EveryReviewedCaptionIsIdempotentInEveryRole()
        {
            foreach (string original in UiCopyCatalog.Captions.Keys)
                foreach (CopyRole role in Enum.GetValues<CopyRole>())
                {
                    Assert.True(UiCopyPolicy.TryFormat(original, role, out var once));
                    Assert.True(UiCopyPolicy.TryFormat(once, role, out var twice), original + " => " + once);
                    Assert.Equal(once, twice);
                }
        }

        [Theory]
        [InlineData("KhiêmCung")]
        [InlineData("Nga Mi Sơn")]
        [InlineData("Thiên Long Bát Bộ")]
        [InlineData("NPC riêng của client")]
        [InlineData("password_DoNotFormat_123")]
        [InlineData("Atk")]
        [InlineData(null)]
        public void UnknownDataAndDebugCommandsAreNeverCaseConverted(string input)
        {
            Assert.False(UiCopyPolicy.TryFormat(input, CopyRole.Heading1, out var output));
            Assert.Equal(input, output);
        }

        [Theory]
        [InlineData("Trân Long Kỳ Cuộc")]
        [InlineData("Trừng Ác")]
        [InlineData("Ác Bá")]
        [InlineData("Thủy Lao")]
        public void DungeonNamesKeepTheirOfficialSpelling(string input)
        {
            foreach (CopyRole role in Enum.GetValues<CopyRole>())
            {
                UiCopyPolicy.TryFormat(input, role, out var output);
                Assert.Equal(input, output);
            }
        }

        [Fact]
        public void ActualTypographyHandlesHeadingAndLateButtonCopyWithoutGrowingFonts()
        {
            dynamic f = TypographyHarness.Create();
            f.ApplyHeading();
            Assert.Equal("NHÂN VẬT", (string)f.Heading.Text);
            Assert.Equal(11f, (float)f.Heading.Font.SizeInPoints);
            Assert.Equal(1, (int)f.Heading.Font.Style);
            f.ApplyButton();
            Assert.Equal("Lên bãi", (string)f.Button.Text);
            f.Button.Text = "Tìm Bãi";
            Assert.Equal("Tìm bãi", (string)f.Button.Text);
            f.ApplyHeading(); f.ApplyButton();
            Assert.Equal(11f, (float)f.Heading.Font.SizeInPoints);
            Assert.Equal(10f, (float)f.Button.Font.SizeInPoints);
            Assert.Equal(1, (int)f.Button.Listeners);
        }

        [Fact]
        public void ActualTypographyChangesGroupsTabsAndMenusByRole()
        {
            dynamic f = TypographyHarness.Create(); f.ApplyGroup(); f.ApplyTab(); f.ApplyMenu();
            Assert.Equal("Hồi Phục", (string)f.Group.Text);
            Assert.Equal("NHIỆM VỤ – PHÓ BẢN", (string)f.Tab.Text);
            Assert.Equal(10.5f, (float)f.Group.Font.SizeInPoints);
            Assert.Equal("Tiện Ích", (string)f.Menu.Text);
            Assert.Equal("Lên bãi", (string)f.Leaf.Text);
            f.Leaf.Text = "Tìm Bãi";
            Assert.Equal("Tìm bãi", (string)f.Leaf.Text);
        }

        [Fact]
        public void TabHeadersAndSharedToggleUseHeadingOneWithoutBoldingPageContent()
        {
            dynamic f = TypographyHarness.Create(); f.ApplyTabHeader(); f.ApplySharedToggle();
            Assert.Equal(11f, (float)f.Tabs.Font.SizeInPoints); Assert.Equal(1, (int)f.Tabs.Font.Style);
            Assert.Equal(10f, (float)f.Tab.Font.SizeInPoints); Assert.Equal(0, (int)f.Tab.Font.Style);
            Assert.Equal("ĐIỀU KHIỂN CHUNG", (string)f.Shared.Text);
            Assert.Equal(11f, (float)f.Shared.Font.SizeInPoints); Assert.Equal(1, (int)f.Shared.Font.Style);
        }

        [Fact]
        public void ActualTypographyPreservesInputRowsAndDynamicLabelsEvenWhenTheyMatchUiCopy()
        {
            dynamic f = TypographyHarness.Create(); f.ApplyData();
            Assert.Equal("Lên Bãi", (string)f.Input.Text);
            Assert.Equal("Kỹ Năng", (string)f.Combo.Text);
            Assert.Equal("Nhân vật", (string)f.DynamicLabel.Text);
            Assert.Equal("Lên Bãi", (string)f.List.Row);
            Assert.Equal("Kỹ Năng", (string)f.List.Subitem);
            Assert.Equal("Tên nhân vật", (string)f.List.Columns[0].Text);
            Assert.Equal("Nhân vật", (string)f.Status.Text);
            Assert.Equal("Lên Bãi", (string)f.MenuInput.Text);
        }

        [Fact] public void CompactMainUsesNinePointNormalTabHeadersAndMenus()
        {
            dynamic f=TypographyHarness.Create();f.EnableCompact();f.ApplyTabHeader();f.ApplyMenu();
            Assert.Equal(9f,(float)f.Tabs.Font.SizeInPoints);Assert.Equal(0,(int)f.Tabs.Font.Style);
            Assert.Equal(9f,(float)f.Menu.Font.SizeInPoints);
        }
        [Fact]
        public void AuditedStaticLabelIsFormattedWhileUnidentifiedLabelIsUntouched()
        {
            dynamic f = TypographyHarness.Create(); f.ApplyStaticLabel();
            Assert.Equal("Phần cứng:", (string)f.StaticLabel.Text);
            Assert.Equal("Nhân vật", (string)f.DynamicLabel.Text);
        }

        [Theory]
        [InlineData("Không Xuất", 0, "Không xuất")]
        [InlineData("Không Xuất", 42, "Không Xuất")]
        [InlineData("Tên Pet Riêng", 0, "Tên Pet Riêng")]
        public void PetDisplayFormattingPreservesLegacyItemAndCommandText(string original, int value, string expected)
        {
            dynamic f = TypographyHarness.Create();
            string[] result = f.PetDisplay(original, value);
            Assert.Equal(expected, result[0]);
            Assert.Equal(original, result[1]); // Original ToString remains the command discriminator.
            Assert.Equal(value.ToString(), result[2]);
        }

        [Fact]
        public void ActualTypographyExpandsWithinNeighbourGapWithoutMovingControlsOrValues()
        {
            dynamic f = TypographyHarness.Create(); f.ApplyButton();
            Assert.InRange((int)f.Button.Width, 50, 76);
            Assert.InRange((int)f.Button.Height, 20, 38);
            Assert.Equal(10, (int)f.Button.Left);
            Assert.Equal(10, (int)f.Button.Top);
            Assert.True((bool)f.HasOriginalParent);
            // Checked, Value and SelectedIndex setters on all fake controls throw.
        }

        [Fact]
        public void ActualTypographyRetainsLargerFontDecorationsAndSkipsDisposedControls()
        {
            dynamic f = TypographyHarness.Create();
            f.MakeLargeButton(); f.ApplyButton();
            Assert.Equal(14f, (float)f.Button.Font.SizeInPoints);
            Assert.Equal(6, (int)f.Button.Font.Style);
            Assert.True((bool)f.Button.AutoEllipsis);
            f.Disposed.IsDisposed = true; f.ApplyDisposed();
            Assert.Equal("Lên Bãi", (string)f.Disposed.Text);
            Assert.Equal(0, (int)f.Disposed.Listeners);
        }
    }

    internal static class TypographyHarness
    {
        private static readonly Lazy<Assembly> Compiled = new Lazy<Assembly>(Compile);
        internal static dynamic Create() => Activator.CreateInstance(Compiled.Value.GetType("TinhKiemAuto.Appearance.TypographyFixture"));
        private static Assembly Compile()
        {
            var formTree = CSharpSyntaxTree.ParseText(DashboardHarness.Read("FrmMain.cs")).GetRoot();
            string choice = formTree.DescendantNodes().OfType<ClassDeclarationSyntax>().Single(c => c.Identifier.Text == "ComboboxItem").ToFullString();
            var dashboardTree = CSharpSyntaxTree.ParseText(DashboardHarness.Read("FrmMain.Dashboard.cs")).GetRoot();
            string formatter = dashboardTree.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text == "FormatPetChoice").ToFullString();
            string fake = FakeUi.Replace("__PET_DISPLAY_SOURCE__", choice + formatter);
            var texts = new[] { "UiTypography.cs", "UiCopyPolicy.cs", "UiCopyCatalog.cs", "FontStylePolicy.cs" }
                .Select(DashboardHarness.Read).Append(fake).Select(s => CSharpSyntaxTree.ParseText(s));
            var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator)
                .Select(p => MetadataReference.CreateFromFile(p));
            var compilation = CSharpCompilation.Create("OwnedTypographyFakeUi", texts, references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var bytes = new MemoryStream(); var result = compilation.Emit(bytes);
            Assert.True(result.Success, string.Join("\n", result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)));
            return Assembly.Load(bytes.ToArray());
        }

        // Real typography code above; only the WinForms/GDI boundary is synthetic.
        private const string FakeUi = @"
using System;using System.Collections.Generic;using System.Drawing;
namespace System.Windows.Forms {public class Placeholder {}}
namespace TinhKiemAuto.Appearance {
 [Flags]public enum FontStyle {Regular=0,Bold=1,Italic=2,Underline=4,Strikeout=8}
 public class Font {public float SizeInPoints;public FontStyle Style;public Font(float size,FontStyle style){SizeInPoints=size;Style=style;}}
 public enum DockStyle {None,Fill}
 [Flags]public enum TextFormatFlags {SingleLine=1,NoPrefix=2}
 public struct Padding {public int Right,Bottom;}
 public static class TextRenderer {public static Size MeasureText(string text,Font font,Size proposed,TextFormatFlags flags)=>new Size((int)(text.Length*font.SizeInPoints*.7f), (int)(font.SizeInPoints*1.5f));}
 public class Control {
  public string Name="""";private string text="""";private EventHandler changed;
  public string Text {get=>text;set{text=value;changed?.Invoke(this,EventArgs.Empty);}}
  public int Listeners;public event EventHandler TextChanged {add{changed+=value;Listeners++;}remove{changed-=value;Listeners--;}}
  public bool IsDisposed;public Font Font=new Font(9f,FontStyle.Regular);public Control Parent;
  public List<Control> Controls=new List<Control>();public DockStyle Dock;public int Left,Top,Width=50,Height=20;
  public int Right=>Left+Width;public int Bottom=>Top+Height;public Size ClientSize=new Size(400,200);public Padding Padding;
  public Form FindForm()=>this is Form?(Form)this:Parent?.FindForm();
  public bool Checked {get=>false;set=>throw new Exception(""Checked write"");}
  public int Value {get=>50;set=>throw new Exception(""Value write"");}
  public int SelectedIndex {get=>0;set=>throw new Exception(""Selection write"");}
 }
 public class Form:Control {} public class FrmMain:Form { public bool CompactUiReady; __PET_DISPLAY_SOURCE__ public static void TestPetFormat(ListControlConvertEventArgs e)=>FormatPetChoice(null,e); }
 public class ListControlConvertEventArgs:EventArgs {public object ListItem,Value;} public class Label:Control {public bool AutoSize,AutoEllipsis;}
 public class ButtonBase:Control {public bool AutoSize,AutoEllipsis;} public class Button:ButtonBase {} public class CheckBox:ButtonBase {} public class RadioButton:ButtonBase {}
 public class TabControl:Control {public List<TabPage> TabPages=new List<TabPage>();}public class GroupBox:Control {}public class TabPage:Control {}public class TextBoxBase:Control {}public class ListControl:Control {}public class ComboBox:ListControl {}
 public class ColumnHeader {public string Text;}
 public class ListView:Control {public List<ColumnHeader> Columns=new List<ColumnHeader>();public string Row=""Lên Bãi"",Subitem=""Kỹ Năng"";}
 public class MenuStrip:Control {}
 public class ToolStripItem {
  public bool IsDisposed;private string text="""";public string Text {get=>text;set{text=value;TextChanged?.Invoke(this,EventArgs.Empty);}}
  public string Name="""";public event EventHandler TextChanged;public Font Font=new Font(9f,FontStyle.Regular);public Control Owner;
 }
 public class ToolStripMenuItem:ToolStripItem {public bool HasDropDownItems;}
 public class ToolStripTextBox:ToolStripItem {}public class ToolStripComboBox:ToolStripItem {}public class ToolStripStatusLabel:ToolStripItem {}
 internal static class UiFixedLayout {internal static bool Owns(Control c)=>false;internal static void Refresh(Control c){}}
 internal static class AppAppearance {
  internal static void Apply(Control control)=>UiTypography.Apply(control);
  private static Dictionary<Tuple<float,FontStyle>,Font> cache=new Dictionary<Tuple<float,FontStyle>,Font>();
  internal static Font GetFont(float size,FontStyle style){var key=Tuple.Create(FontStylePolicy.PreservePointSize(size),style);if(!cache.TryGetValue(key,out var font)){font=new Font(key.Item1,key.Item2);cache.Add(key,font);}return font;}
 }
 public class TypographyFixture {
  public Form Root=new FrmMain();public Label Heading=new Label{ Name=""sectionCharacters"",Text=""Nhân vật""};
  public Button Button=new Button{Text=""Lên Bãi"",Left=10,Top=10};public Control Neighbour=new Control{Left=90,Top=10};
  public GroupBox Group=new GroupBox{Text=""Hồi Phục""};public TabPage Tab=new TabPage{Text=""Nhiệm Vụ - Phụ Bản""};
  public TabControl Tabs=new TabControl(); public ToolStripItem Shared=new ToolStripItem{Name=""sectionSharedControls"",Text=""Điều khiển chung""};
  public TextBoxBase Input=new TextBoxBase{Text=""Lên Bãi""};public ComboBox Combo=new ComboBox{Text=""Kỹ Năng""};
  public Label DynamicLabel=new Label{Text=""Nhân vật""};public Label StaticLabel=new Label{Name=""label10"",Text=""Phân Cứng :""};public ListView List=new ListView();
  public ToolStripMenuItem Menu=new ToolStripMenuItem{Text=""Tiện Ích"",HasDropDownItems=true},Leaf=new ToolStripMenuItem{Text=""Lên Bãi""};
  public ToolStripStatusLabel Status=new ToolStripStatusLabel{Text=""Nhân vật""};public ToolStripTextBox MenuInput=new ToolStripTextBox{Text=""Lên Bãi""};
  public Button Disposed=new Button{Text=""Lên Bãi""};
  public TypographyFixture(){foreach(var c in new Control[]{Heading,Button,Neighbour,Group,Tab,Input,Combo,DynamicLabel,StaticLabel,List}){c.Parent=Root;Root.Controls.Add(c);}List.Columns.Add(new ColumnHeader{Text=""Tên Nhân Vật""});Tabs.TabPages.Add(Tab);}
  public string[] PetDisplay(string original,int value){var c=new FrmMain.ComboboxItem{Text=original,Value=value};var e=new ListControlConvertEventArgs{ListItem=c,Value=original};FrmMain.TestPetFormat(e);return new[]{(string)e.Value,c.ToString(),c.Value.ToString()};}
  public void EnableCompact(){((FrmMain)Root).CompactUiReady=true;Tabs.Parent=Root;Menu.Owner=Root;}
  public bool HasOriginalParent=>ReferenceEquals(Button.Parent,Root);
  public void ApplyHeading()=>UiTypography.Apply(Heading);public void ApplyButton()=>UiTypography.Apply(Button);
  public void ApplyTabHeader()=>UiTypography.Apply(Tabs); public void ApplySharedToggle()=>UiTypography.Apply(Shared);
  public void ApplyGroup()=>UiTypography.Apply(Group);public void ApplyTab()=>UiTypography.Apply(Tab);
  public void ApplyMenu(){UiTypography.Apply(Menu);UiTypography.Apply(Leaf);}
  public void ApplyData(){foreach(var c in new Control[]{Input,Combo,DynamicLabel,List})UiTypography.Apply(c);UiTypography.Apply(Status);UiTypography.Apply(MenuInput);}
  public void ApplyStaticLabel(){UiTypography.Apply(StaticLabel);UiTypography.Apply(DynamicLabel);}
  public void MakeLargeButton()=>Button.Font=new Font(14f,(FontStyle)6);public void ApplyDisposed()=>UiTypography.Apply(Disposed);
 }
}
";
    }
}
