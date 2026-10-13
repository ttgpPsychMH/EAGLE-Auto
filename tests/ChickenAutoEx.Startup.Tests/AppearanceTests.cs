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
    public sealed class AppearanceTests
    {
        [Theory]
        [InlineData(true, true, 0)]
        [InlineData(true, false, 0)]
        [InlineData(false, true, 1)]
        [InlineData(false, false, 2)]
        public void FaceAvailabilityChoosesPrivateThenInstalledThenFallback(bool privateFace, bool installedFace, int expectedValue)
        {
            FontSource expected = (FontSource)expectedValue;
            Assert.Equal(expected, FontStylePolicy.ResolveSource(privateFace, installedFace));
            Assert.Equal(expected == FontSource.SegoeUi ? "Segoe UI" : "Aptos", FontStylePolicy.ResolveFamilyName(privateFace, installedFace));
        }

        [Theory]
        [InlineData(8.25f, 9f)] [InlineData(9f, 9f)] [InlineData(14f, 14f)]
        public void ReadableFloorPreservesLargerSizes(float size, float expected) => Assert.Equal(expected, FontStylePolicy.PreservePointSize(size));

        [Theory]
        [InlineData(0f)] [InlineData(-1f)] [InlineData(float.NaN)]
        [InlineData(float.PositiveInfinity)] [InlineData(float.NegativeInfinity)]
        public void InvalidPointSizeUsesReadableDefault(float size) => Assert.Equal(9f, FontStylePolicy.PreservePointSize(size));

        [Theory]
        [InlineData(0, 0, 0)] [InlineData(1, 1, 1)] [InlineData(2, 2, 2)]
        [InlineData(3, 3, 3)] [InlineData(4, 4, 0)] [InlineData(8, 8, 0)]
        [InlineData(15, 15, 3)] [InlineData(255, 15, 3)]
        public void FaceSelectionPreservesDecorationsButRequestsOnlyBoldAndItalicFace(int input, int style, int face)
        {
            Assert.Equal(style, FontStylePolicy.PreserveStyleBits(input));
            Assert.Equal(face, FontStylePolicy.FaceStyleBits(input));
        }

        [Theory]
        [InlineData("Aptos", true)] [InlineData("APTOS", true)]
        [InlineData("aptos", true)] [InlineData("Aptos Display", false)]
        [InlineData("Aptos Narrow", false)] [InlineData(" Aptos", false)]
        [InlineData(null, false)]
        public void OnlyExactBodyFamilyMatchesIgnoringCase(string name, bool expected) => Assert.Equal(expected, FontStylePolicy.IsPreferredFamily(name));

        [Fact]
        public void ActualApplyVisitsControlTreeNestedMenusAndContextMenus()
        {
            dynamic f = AppearanceHarness.Create();
            f.Apply();
            Assert.True((bool)f.AllInitialFontsChanged);
            Assert.Equal(9f, (float)f.Root.Font.SizeInPoints);
            Assert.Equal(11f, (float)f.NestedControl.Font.SizeInPoints);
            Assert.Equal(3, (int)f.NestedControl.Font.Style);
            Assert.Equal(10.5f, (float)f.NestedItem.Font.SizeInPoints);
            Assert.Equal(7, (int)f.NestedItem.Font.Style);
        }

        [Fact]
        public void ActualApplyReplacesIconWithoutChangingHiddenWindowDecisions()
        {
            dynamic f = AppearanceHarness.Create();
            f.Root.ShowIcon = false; f.Root.ShowInTaskbar = false;
            f.Apply();
            Assert.True((bool)f.HasApplicationIcon);
            Assert.False((bool)f.Root.ShowIcon);
            Assert.False((bool)f.Root.ShowInTaskbar);
        }

        [Fact]
        public void ActualApplyPreservesSizesAndStyleAfterLaterControlFontAssignment()
        {
            dynamic f = AppearanceHarness.Create(); f.Apply();
            f.ChangeControlFont(f.NestedControl, 14f, 11);
            Assert.Equal("Aptos", (string)f.NestedControl.Font.FamilyName);
            Assert.Equal(14f, (float)f.NestedControl.Font.SizeInPoints);
            Assert.Equal(11, (int)f.NestedControl.Font.Style);
        }

        [Fact]
        public void ActualControlAddedStylesNewControlAndItsExistingChildren()
        {
            dynamic f = AppearanceHarness.Create(); f.Apply();
            dynamic late = f.AddLateControlTree();
            Assert.Equal("Aptos", (string)late.Font.FamilyName);
            Assert.Equal("Aptos", (string)late.Controls[0].Font.FamilyName);
            Assert.Equal(13f, (float)late.Controls[0].Font.SizeInPoints);
            Assert.Equal(2, (int)late.Controls[0].Font.Style);
        }

        [Theory]
        [InlineData(false)] [InlineData(true)]
        public void ActualItemAddedStylesTopLevelAndNestedLateMenuItems(bool nested)
        {
            dynamic f = AppearanceHarness.Create(); f.Apply();
            dynamic item = f.AddLateItem(nested);
            Assert.Equal("Aptos", (string)item.Font.FamilyName);
            Assert.Equal(12f, (float)item.Font.SizeInPoints);
            Assert.Equal(1, (int)item.Font.Style);
        }

        [Fact]
        public void ActualContextMenuChangedStylesReplacementAndItsItems()
        {
            dynamic f = AppearanceHarness.Create(); f.Apply();
            dynamic context = f.ReplaceContextMenu();
            Assert.Equal("Aptos", (string)context.Font.FamilyName);
            Assert.Equal("Aptos", (string)context.Items[0].Font.FamilyName);
            Assert.Equal(9.5f, (float)context.Items[0].Font.SizeInPoints);
        }

        [Fact]
        public void ActualDropDownOpeningRefreshesExplicitlyChangedNestedItemFonts()
        {
            dynamic f = AppearanceHarness.Create(); f.Apply();
            f.ChangeItemFont(f.NestedItem, 13f, 3);
            f.SubMenu.Open();
            Assert.Equal("Aptos", (string)f.NestedItem.Font.FamilyName);
            Assert.Equal(13f, (float)f.NestedItem.Font.SizeInPoints);
            Assert.Equal(3, (int)f.NestedItem.Font.Style);
        }

        [Fact]
        public void ActualToolStripFontChangedRefreshesItemsWithExplicitFonts()
        {
            dynamic f = AppearanceHarness.Create(); f.Apply();
            f.ChangeItemFont(f.MenuItem, 12.5f, 2);
            f.ChangeControlFont(f.Menu, 10f, 0);
            Assert.Equal("Aptos", (string)f.MenuItem.Font.FamilyName);
            Assert.Equal(12.5f, (float)f.MenuItem.Font.SizeInPoints);
            Assert.Equal(2, (int)f.MenuItem.Font.Style);
        }

        [Fact]
        public void ActualApplyRegistersListenersOnceAndReusesFonts()
        {
            dynamic f = AppearanceHarness.Create(); f.Apply();
            int writes = f.TotalFontWrites;
            f.Apply(); f.Apply();
            Assert.Equal(writes, (int)f.TotalFontWrites);
            Assert.Equal(1, (int)f.Root.ControlAddedListeners);
            Assert.Equal(1, (int)f.Root.FontChangedListeners);
            Assert.Equal(1, (int)f.Root.ContextMenuListeners);
            Assert.Equal(1, (int)f.Menu.ItemAddedListeners);
            Assert.Equal(1, (int)f.MenuItem.OpeningListeners);
            Assert.Equal(1, (int)f.SubMenu.OpeningListeners);
            Assert.InRange((int)f.MenuItem.OwnerChangedListeners, 0, 1);
        }

        [Fact]
        public void ActualApplyNeverWritesLegacyCheckedOrNumericValues()
        {
            // Every fixture control/item setter throws on a Checked or Value write.
            dynamic f = AppearanceHarness.Create(); f.Apply(); f.Apply();
            f.AddLateControlTree(); f.AddLateItem(true); f.ReplaceContextMenu();
            f.ChangeControlFont(f.NestedControl, 12f, 1); f.SubMenu.Open();
            Assert.True((bool)f.AllInitialFontsChanged);
        }

        [Fact]
        public void DisposedControlDoesNotAcquireFontOrListeners()
        {
            dynamic f = AppearanceHarness.Create(); f.Root.IsDisposed = true; f.Apply();
            Assert.Equal("Legacy", (string)f.Root.Font.FamilyName);
            Assert.Equal(0, (int)f.Root.ControlAddedListeners);
            Assert.False((bool)f.HasApplicationIcon);
        }

        [Fact]
        public void DisposedDescendantDoesNotAcquireFontOrListeners()
        {
            dynamic f = AppearanceHarness.Create(); f.NestedControl.IsDisposed = true; f.Apply();
            Assert.Equal("Legacy", (string)f.NestedControl.Font.FamilyName);
            Assert.Equal(0, (int)f.NestedControl.FontChangedListeners);
            Assert.Equal("Aptos", (string)f.Root.Font.FamilyName);
        }

        [Fact]
        public void NullRootAndDisposedMenuItemAreIgnored()
        {
            dynamic f = AppearanceHarness.Create(); f.ApplyNull();
            f.NestedItem.IsDisposed = true; f.Apply();
            Assert.Equal("Legacy", (string)f.NestedItem.Font.FamilyName);
            Assert.Equal("Aptos", (string)f.SubMenu.Font.FamilyName);
        }

        [Fact]
        public void WrongThreadQueuesUiUpdateBeforeTouchingControls()
        {
            dynamic f = AppearanceHarness.Create();
            f.Root.IsHandleCreated = true; f.Root.InvokeRequired = true;
            f.Apply();
            Assert.Equal(1, (int)f.Root.BeginInvokeCalls);
            Assert.Equal("Legacy", (string)f.Root.Font.FamilyName);
            Assert.Equal(0, (int)f.Root.ControlAddedListeners);
            f.Root.DrainInvocation();
            Assert.Equal("Aptos", (string)f.Root.Font.FamilyName);
            Assert.True((bool)f.AllInitialFontsChanged);
        }

        [Fact]
        public void QueuedUpdateSafelySkipsControlDisposedBeforeDispatch()
        {
            dynamic f = AppearanceHarness.Create();
            f.Root.IsHandleCreated = true; f.Root.InvokeRequired = true; f.Apply();
            f.Root.IsDisposed = true; f.Root.DrainInvocation();
            Assert.Equal("Legacy", (string)f.Root.Font.FamilyName);
            Assert.Equal(0, (int)f.Root.FontChangedListeners);
        }

        [Fact]
        public void BeginInvokeFailureDuringHandleTeardownDoesNotEscape()
        {
            dynamic f = AppearanceHarness.Create();
            f.Root.IsHandleCreated = true; f.Root.InvokeRequired = true;
            f.Root.ThrowBeginInvoke = true; f.Apply();
            Assert.Equal(1, (int)f.Root.BeginInvokeCalls);
            Assert.Equal("Legacy", (string)f.Root.Font.FamilyName);
            Assert.Equal(0, (int)f.Root.ControlAddedListeners);
        }

        [Theory]
        [InlineData(0)] [InlineData(4 * 1024 * 1024 + 1)]
        public void ActualBoundedReadRejectsEmptyAndOversizedInput(int length)
        {
            string path = Path.GetTempFileName();
            try
            {
                using (var file = new FileStream(path, FileMode.Open, FileAccess.Write)) file.SetLength(length);
                Assert.Null(AppearanceHarness.ReadFontBytes(path));
            }
            finally { File.Delete(path); }
        }

        [Theory]
        [InlineData(1)] [InlineData(257)] [InlineData(4 * 1024 * 1024)]
        public void ActualBoundedReadReturnsExactSyntheticBytesAtAllowedLengths(int length)
        {
            // Synthetic bytes never go through GDI, a font parser, or the user's TTFs.
            byte[] bytes = Enumerable.Range(0, length).Select(i => (byte)(i % 251)).ToArray();
            string path = Path.GetTempFileName();
            try { File.WriteAllBytes(path, bytes); Assert.Equal(bytes, AppearanceHarness.ReadFontBytes(path)); }
            finally { File.Delete(path); }
        }
    }

    internal static class AppearanceHarness
    {
        private static readonly Lazy<Assembly> Assembly = new Lazy<Assembly>(Compile);
        internal static dynamic Create() => Activator.CreateInstance(Assembly.Value.GetType("TinhKiemAuto.Appearance.AppearanceFixture"));
        internal static byte[] ReadFontBytes(string path)
        {
            var method = Assembly.Value.GetType("TinhKiemAuto.Appearance.AppAppearance").GetMethod("TestReadFont", BindingFlags.Static | BindingFlags.Public);
            return (byte[])method.Invoke(null, new object[] { path });
        }

        private static Assembly Compile()
        {
            var tree = CSharpSyntaxTree.ParseText(DashboardHarness.Read("AppAppearance.cs")).GetRoot();
            string[] names = { "Apply", "ApplyControl", "ApplyControlFont", "ApplyItem", "OnControlAdded", "OnControlFontChanged", "OnContextMenuStripChanged", "OnItemAdded", "OnDropDownOpening", "ReadBoundedFont" };
            var declared = tree.DescendantNodes().OfType<MethodDeclarationSyntax>().ToArray();
            var methods = names.Select(name => declared.Single(m => m.Identifier.Text == name).ToFullString()).ToList();
            // Public WinForms events differ between controls and menu items. Include
            // any reviewed traversal helper without inventing a FontChanged event
            // on ToolStripItem (that event is not public in .NET Framework).
            foreach (string optional in new[] { "OnItemOwnerChanged", "RefreshStripItems" })
            {
                var method = declared.SingleOrDefault(m => m.Identifier.Text == optional);
                if (method != null) methods.Add(method.ToFullString());
            }
            var states = new[] { "ControlState", "ItemState" }.Select(name => tree.DescendantNodes().OfType<ClassDeclarationSyntax>().Single(c => c.Identifier.Text == name).ToFullString());
            string fontBound = tree.DescendantNodes().OfType<FieldDeclarationSyntax>().Single(f => f.Declaration.Variables.Any(v => v.Identifier.Text == "MaximumFontBytes")).ToFullString();
            string source = "using System;using System.IO;using System.Collections.Generic;using System.Runtime.CompilerServices;namespace TinhKiemAuto.Appearance {internal static class AppAppearance {" + Boundary + fontBound + string.Join("\n", methods.Concat(states)) + "}}";
            var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator).Select(p => MetadataReference.CreateFromFile(p));
            var texts = new[] { DashboardHarness.Read("FontStylePolicy.cs"), source, Fixture }.Select(s => CSharpSyntaxTree.ParseText(s));
            var compilation = CSharpCompilation.Create("AppearanceTraversalFakeUi", texts, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var bytes = new MemoryStream(); var result = compilation.Emit(bytes);
            Assert.True(result.Success, string.Join("\n", result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)));
            return System.Reflection.Assembly.Load(bytes.ToArray());
        }

        // This explicit boundary replaces process resource initialization and GDI
        // font creation; the traversal/event/read methods above are real source.
        private const string Boundary = @"
 private static readonly ConditionalWeakTable<Control,ControlState> Controls=new ConditionalWeakTable<Control,ControlState>();
 private static readonly ConditionalWeakTable<ToolStripItem,ItemState> Items=new ConditionalWeakTable<ToolStripItem,ItemState>();
 private static readonly Dictionary<Tuple<float,FontStyle>,Font> Fonts=new Dictionary<Tuple<float,FontStyle>,Font>();
 internal static readonly object Icon=new object();
 private static void EnsureInitialized(){}
 private static Font GetFont(float size,FontStyle style){
  var key=Tuple.Create(FontStylePolicy.PreservePointSize(size),(FontStyle)FontStylePolicy.PreserveStyleBits((int)style));
  Font font;if(!Fonts.TryGetValue(key,out font)){font=new Font(""Aptos"",key.Item1,key.Item2);Fonts.Add(key,font);}return font;
 }
 public static byte[] TestReadFont(string path){return ReadBoundedFont(path);}
";

        private const string Fixture = @"
using System;
using System.Collections.Generic;
using System.Linq;
namespace TinhKiemAuto.Appearance {
 // Traversal fixture isolates typography, which has its own actual-source tests.
 internal static class UiFixedLayout {internal static void Refresh(Control c){}}
 internal static class UiTypography {internal static void Apply(Control c){} internal static void Apply(ToolStripItem i){}}
 [Flags] public enum FontStyle { Regular=0,Bold=1,Italic=2,Underline=4,Strikeout=8 }
 public sealed class Font {
  public readonly string FamilyName;public readonly float SizeInPoints;public readonly FontStyle Style;
  public Font(string family,float size,FontStyle style){FamilyName=family;SizeInPoints=size;Style=style;}
 }
 public sealed class ControlEventArgs:EventArgs {public Control Control;public ControlEventArgs(Control c){Control=c;}}
 public sealed class ToolStripItemEventArgs:EventArgs {public ToolStripItem Item;public ToolStripItemEventArgs(ToolStripItem i){Item=i;}}
 public class Control {
  private Font font=new Font(""Legacy"",8.25f,FontStyle.Regular);private ToolStrip context;
  private EventHandler<ControlEventArgs> controlAdded;private EventHandler fontChanged,contextChanged;
  public int FontWrites,ControlAddedListeners,FontChangedListeners,ContextMenuListeners,BeginInvokeCalls;
  public bool IsDisposed,IsHandleCreated,InvokeRequired,ThrowBeginInvoke;
  public readonly ControlCollection Controls;
  public Control(){Controls=new ControlCollection(this);}
  public Font Font {get{return font;}set{font=value;FontWrites++;fontChanged?.Invoke(this,EventArgs.Empty);}}
  public bool Checked {get{return false;}set{throw new InvalidOperationException(""Unexpected Checked write"");}}
  public int Value {get{return 50;}set{throw new InvalidOperationException(""Unexpected Value write"");}}
  public ToolStrip ContextMenuStrip {get{return context;}set{context=value;contextChanged?.Invoke(this,EventArgs.Empty);}}
  public event EventHandler<ControlEventArgs> ControlAdded {add{controlAdded+=value;ControlAddedListeners++;}remove{controlAdded-=value;ControlAddedListeners--;}}
  public event EventHandler FontChanged {add{fontChanged+=value;FontChangedListeners++;}remove{fontChanged-=value;FontChangedListeners--;}}
  public event EventHandler ContextMenuStripChanged {add{contextChanged+=value;ContextMenuListeners++;}remove{contextChanged-=value;ContextMenuListeners--;}}
  internal void Added(Control c){controlAdded?.Invoke(this,new ControlEventArgs(c));}
  private Delegate queued;private object[] queuedArgs;
  public object BeginInvoke(Delegate method,params object[] args){BeginInvokeCalls++;if(ThrowBeginInvoke)throw new InvalidOperationException(""Handle teardown"");queued=method;queuedArgs=args;return null;}
  public void DrainInvocation(){InvokeRequired=false;var action=queued;queued=null;action?.DynamicInvoke(queuedArgs);}
 }
 public sealed class ControlCollection:List<Control> {
  private readonly Control owner;internal ControlCollection(Control c){owner=c;}
  public new void Add(Control c){base.Add(c);owner.Added(c);}
 }
 public sealed class Form:Control {public object Icon;public bool ShowIcon=true,ShowInTaskbar=true;}
 public class ToolStrip:Control {
  private EventHandler<ToolStripItemEventArgs> itemAdded;public int ItemAddedListeners;
  public readonly ItemCollection Items;
  public ToolStrip(){Items=new ItemCollection(this);}
  public event EventHandler<ToolStripItemEventArgs> ItemAdded {add{itemAdded+=value;ItemAddedListeners++;}remove{itemAdded-=value;ItemAddedListeners--;}}
  internal void Added(ToolStripItem item){itemAdded?.Invoke(this,new ToolStripItemEventArgs(item));}
 }
 public sealed class ItemCollection:List<ToolStripItem> {
  private readonly ToolStrip owner;internal ItemCollection(ToolStrip s){owner=s;}
  public new void Add(ToolStripItem item){base.Add(item);item.ChangeOwner(owner);owner.Added(item);}
 }
 public class ToolStripItem {
  private Font font=new Font(""Legacy"",8.25f,FontStyle.Regular);private EventHandler ownerChanged;
  public int FontWrites,OwnerChangedListeners;public bool IsDisposed;public ToolStrip Owner;
  public Font Font {get{return font;}set{font=value;FontWrites++;}}
  public bool Checked {get{return false;}set{throw new InvalidOperationException(""Unexpected Checked write"");}}
  public int Value {get{return 50;}set{throw new InvalidOperationException(""Unexpected Value write"");}}
  public event EventHandler OwnerChanged {add{ownerChanged+=value;OwnerChangedListeners++;}remove{ownerChanged-=value;OwnerChangedListeners--;}}
  internal void ChangeOwner(ToolStrip owner){Owner=owner;ownerChanged?.Invoke(this,EventArgs.Empty);}
 }
 public sealed class ToolStripDropDownItem:ToolStripItem {
  private EventHandler opening;public int OpeningListeners;public readonly ToolStrip DropDown=new ToolStrip();
  public event EventHandler DropDownOpening {add{opening+=value;OpeningListeners++;}remove{opening-=value;OpeningListeners--;}}
  public void Open(){opening?.Invoke(this,EventArgs.Empty);}
 }
 public sealed class AppearanceFixture {
  public readonly Form Root=new Form();public readonly Control Panel=new Control(),NestedControl=new Control();
  public readonly ToolStrip Menu=new ToolStrip(),Context=new ToolStrip();
  public readonly ToolStripDropDownItem MenuItem=new ToolStripDropDownItem(),SubMenu=new ToolStripDropDownItem();
  public readonly ToolStripItem NestedItem=new ToolStripItem(),ContextItem=new ToolStripItem();
  public AppearanceFixture(){
   NestedControl.Font=new Font(""Legacy"",11f,(FontStyle)3);NestedItem.Font=new Font(""Legacy"",10.5f,(FontStyle)7);
   Root.Controls.Add(Panel);Panel.Controls.Add(NestedControl);Root.Controls.Add(Menu);
   Menu.Items.Add(MenuItem);MenuItem.DropDown.Items.Add(SubMenu);SubMenu.DropDown.Items.Add(NestedItem);
   Context.Items.Add(ContextItem);Panel.ContextMenuStrip=Context;
  }
  public bool HasApplicationIcon=>ReferenceEquals(Root.Icon,AppAppearance.Icon);
  public bool AllInitialFontsChanged=>new[]{Root.Font,Panel.Font,NestedControl.Font,Menu.Font,MenuItem.Font,MenuItem.DropDown.Font,SubMenu.Font,SubMenu.DropDown.Font,NestedItem.Font,Context.Font,ContextItem.Font}.All(f=>f.FamilyName==""Aptos"");
  public int TotalFontWrites=>Root.FontWrites+Panel.FontWrites+NestedControl.FontWrites+Menu.FontWrites+MenuItem.FontWrites+MenuItem.DropDown.FontWrites+SubMenu.FontWrites+SubMenu.DropDown.FontWrites+NestedItem.FontWrites+Context.FontWrites+ContextItem.FontWrites;
  public void Apply(){AppAppearance.Apply(Root);}
  public void ApplyNull(){AppAppearance.Apply(null);}
  public void ChangeControlFont(Control control,float size,int style){control.Font=new Font(""Legacy"",size,(FontStyle)style);}
  public void ChangeItemFont(ToolStripItem item,float size,int style){item.Font=new Font(""Legacy"",size,(FontStyle)style);}
  public Control AddLateControlTree(){var parent=new Control();var child=new Control();child.Font=new Font(""Legacy"",13f,FontStyle.Italic);parent.Controls.Add(child);Panel.Controls.Add(parent);return parent;}
  public ToolStripItem AddLateItem(bool nested){var item=new ToolStripItem();item.Font=new Font(""Legacy"",12f,FontStyle.Bold);(nested?SubMenu.DropDown:Menu).Items.Add(item);return item;}
  public ToolStrip ReplaceContextMenu(){var strip=new ToolStrip();var item=new ToolStripItem();item.Font=new Font(""Legacy"",9.5f,FontStyle.Regular);strip.Items.Add(item);Panel.ContextMenuStrip=strip;return strip;}
 }
}";
    }
}
