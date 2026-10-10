using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace ChickenAutoEx.Startup.Tests
{
    public sealed class UiSettingsRegressionTests
    {
        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void RecoveryCheckboxesSetIndependentGameFlags(bool hp, bool mp)
        {
            dynamic form = LegacyUiHarness.Create();
            form.checkregenhp.Checked = hp;
            form.checkrengenmp.Checked = mp;
            form.ChangeHp();
            form.ChangeMp();
            Assert.Equal(hp, (bool)form.CurGame.IsHP);
            Assert.Equal(mp, (bool)form.CurGame.IsMP);

            form.checkrengenmp.Checked = !mp;
            form.ChangeMp();
            Assert.Equal(hp, (bool)form.CurGame.IsHP);
            Assert.Equal(!mp, (bool)form.CurGame.IsMP);
            form.checkregenhp.Checked = !hp;
            form.ChangeHp();
            Assert.Equal(!hp, (bool)form.CurGame.IsHP);
            Assert.Equal(!mp, (bool)form.CurGame.IsMP);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ChangingRecoveryCheckboxesWithoutSelectedGameIsHarmless(bool suppressNotice)
        {
            dynamic form = LegacyUiHarness.Create();
            form.SuppressNotice = suppressNotice;
            form.CurGame = null;
            form.ChangeHp();
            form.ChangeMp();
            Assert.Equal(0, (int)form.notifyIcon1.Calls);
        }

        [Theory]
        [InlineData(false, "Tắt")]
        [InlineData(true, "Bật")]
        public void MpNoticeAndGameFlagAgree(bool enabled, string notice)
        {
            dynamic form = LegacyUiHarness.Create();
            form.SuppressNotice = false;
            form.checkregenhp.Checked = !enabled;
            form.checkrengenmp.Checked = enabled;
            form.ChangeMp();
            Assert.Equal(enabled, (bool)form.CurGame.IsMP);
            Assert.Contains(notice + " tự sử dụng MP", (string)form.notifyIcon1.LastText);
        }

        [Theory]
        [InlineData(61, 0, 0)]
        [InlineData(62, 7, 0)]
        [InlineData(63, 7, 9)]
        [InlineData(64, 7, 9)]
        public void LegacyConfigUsesOnlyPresentMapIndexes(int length, int map, int healMap)
        {
            dynamic form = LegacyUiHarness.Create();
            var values = ValidSettings(length);
            var errors = CaptureIndexErrors(() => form.Load(values));
            Assert.Equal(0, errors);
            Assert.Equal(map, (int)form.MapIndex);
            Assert.Equal(healMap, (int)form.HealMapIndex);
            // Fields before the optional tail must still be loaded.
            Assert.Equal(37m, (decimal)form.nudHP.Value);
            Assert.Equal(41m, (decimal)form.nudMP.Value);
            Assert.Equal(6m, (decimal)form.numbankinhtheosau.Value);
        }

        [Theory]
        [InlineData(61, 13, 17)]
        [InlineData(62, 7, 17)]
        [InlineData(63, 7, 9)]
        public void MissingConfigTailPreservesCurrentValues(int length, int map, int healMap)
        {
            dynamic form = LegacyUiHarness.Create();
            form.MapIndex = 13;
            form.HealMapIndex = 17;
            Assert.Equal(0, CaptureIndexErrors(() => form.Load(ValidSettings(length))));
            Assert.Equal(map, (int)form.MapIndex);
            Assert.Equal(healMap, (int)form.HealMapIndex);
        }

        [Theory]
        [InlineData(61)]
        [InlineData(62)]
        [InlineData(63)]
        public void MapOptionsRoundTripThroughExistingCsvPositions(int length)
        {
            dynamic form = LegacyUiHarness.Create();
            form.Load(ValidSettings(length));
            int expectedMap = form.MapIndex;
            int expectedHealMap = form.HealMapIndex;
            string saved = form.Save();
            string[] fields = saved.Split(',');
            Assert.Equal(64, fields.Length);
            Assert.Equal(expectedMap.ToString(), fields[61]);
            Assert.Equal(expectedHealMap.ToString(), fields[62]);
            form.MapIndex = -1;
            form.HealMapIndex = -1;
            Assert.Equal(0, CaptureIndexErrors(() => form.LoadSaved()));
            Assert.Equal(expectedMap, (int)form.MapIndex);
            Assert.Equal(expectedHealMap, (int)form.HealMapIndex);
        }

        [Fact]
        public void AbsentConfigKeepsMapDefaults()
        {
            dynamic form = LegacyUiHarness.Create();
            Assert.Equal(0, CaptureIndexErrors(() => form.Load(null)));
            Assert.Equal(0, (int)form.MapIndex);
            Assert.Equal(0, (int)form.HealMapIndex);
        }

        private static int[] ValidSettings(int length)
        {
            var values = new int[length];
            values[5] = 30;
            values[8] = 30;
            values[10] = 20;
            values[18] = 37;
            values[19] = 41;
            values[20] = 75;
            values[33] = 6;
            values[53] = 60;
            if (length > 61) values[61] = 7;
            if (length > 62) values[62] = 9;
            return values;
        }

        private static int CaptureIndexErrors(Action action)
        {
            int errors = 0;
            int thread = Environment.CurrentManagedThreadId;
            EventHandler<FirstChanceExceptionEventArgs> observer = (_, args) =>
            {
                if (Environment.CurrentManagedThreadId == thread && args.Exception is IndexOutOfRangeException)
                    errors++;
            };
            AppDomain.CurrentDomain.FirstChanceException += observer;
            try { action(); }
            finally { AppDomain.CurrentDomain.FirstChanceException -= observer; }
            // LoadSetting's legacy catch hides the exception; final values alone miss the bug.
            return errors;
        }
    }

    // Execute actual method bodies with inert dependencies. No form/game constructors,
    // licensing code, filesystem encryption, Win32 calls or automation assembly is loaded.
    internal static class LegacyUiHarness
    {
        private static readonly Lazy<byte[]> Image = new Lazy<byte[]>(Compile);

        internal static object Create()
        {
            // Load(byte[]) creates an isolated assembly instance, so static settings are
            // fresh for each test without sharing Global/Option state across theories.
            return Activator.CreateInstance(Assembly.Load(Image.Value).GetType("FixtureMain", true));
        }

        internal static ClassDeclarationSyntax SourceClass(string file, string name)
        {
            var source = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "RegressionSources", file));
            return CSharpSyntaxTree.ParseText(source).GetRoot().DescendantNodes()
                .OfType<ClassDeclarationSyntax>().Single(c => c.Identifier.Text == name);
        }

        private static string Methods(ClassDeclarationSyntax type, params string[] names)
        {
            return string.Join("\n", names.Select(name => type.Members.OfType<MethodDeclarationSyntax>()
                .Single(method => method.Identifier.Text == name).ToFullString()));
        }

        private static string ScalarDependencies(string file, string type, string methods)
        {
            var names = CSharpSyntaxTree.ParseText("class Selected {" + methods + "}").GetRoot()
                .DescendantNodes().OfType<MemberAccessExpressionSyntax>()
                .Where(m => m.Expression.ToString() == type).Select(m => m.Name.Identifier.Text).Distinct().ToArray();
            var members = SourceClass(file, type).Members.Where(member =>
                member is FieldDeclarationSyntax f && f.Declaration.Variables.Any(v => names.Contains(v.Identifier.Text)) ||
                member is PropertyDeclarationSyntax p && names.Contains(p.Identifier.Text)).ToArray();
            Assert.Equal(names.Length, members.Length);
            foreach (var member in members)
            {
                // Only inert scalar fields/auto-properties, never methods or constructors.
                var field = member as FieldDeclarationSyntax;
                var property = member as PropertyDeclarationSyntax;
                string scalarType = field?.Declaration.Type.ToString() ?? property.Type.ToString();
                Assert.Contains(scalarType, new[] { "bool", "int", "Keys", "TINHKIEM.Menpai" });
                if (property != null)
                    Assert.All(property.AccessorList.Accessors, a => Assert.Null(a.Body));
            }
            return "public static class " + type + " {\n" + string.Join("\n", members.Select(m => m.ToFullString())) + "\n}";
        }

        private static byte[] Compile()
        {
            var form = SourceClass("FrmMain.cs", "FrmMain");
            string methods = Methods(form, "Bool2Int", "SaveSetting", "LoadSetting",
                "checkregenhp_CheckedChanged", "checkrengenmp_CheckedChanged");
            methods += Methods(form, "ItemTrungAc_Click", "ItemAcBa_Click", "ItemThuyLao_Click",
                "itemTranLongKyCuoc_Click", "thoátToolStripMenuItem1_Click", "chươngTrìnhToolStripMenuItem_Click",
                "menuactac_Click", "UnCheckAllAcTac", "CallALLAction", "dUwngfToolStripMenuItem_Click",
                "tựĐộngToolStripMenuItem_Click", "vôLượngSơnToolStripMenuItem_Click", "kínhHồToolStripMenuItem_Click",
                "kiếmCácToolStripMenuItem_Click", "tháiHồToolStripMenuItem_Click", "tungSơnToolStripMenuItem_Click",
                "đônHoàngToolStripMenuItem_Click");
            // Optional for running the same behavioral tests against the pre-fix source.
            string[] menuHelpers = { "TryGetDungeonContext", "RequireDungeonContext", "UpdateDungeonMenu",
                "ResetDungeonActions", "SelectAcTacMap" };
            methods += string.Join("\n", form.Members.OfType<MethodDeclarationSyntax>()
                .Where(m => menuHelpers.Contains(m.Identifier.Text)).Select(m => m.ToFullString()));
            string settings = Methods(SourceClass("Setting.cs", "Setting"), "String2Arr", "String2Int");
            var keyboard = SourceClass("TINHKIEM.cs", "TINHKIEM");
            string keys = Methods(keyboard, "Int2Key", "Key2Int");
            string menpai = keyboard.Members.OfType<EnumDeclarationSyntax>().Single(e => e.Identifier.Text == "Menpai").ToFullString();
            var source = "using System; using System.Collections.Generic;\n" + Fixture
                + "\npublic class FixtureMain {\n" + Controls + DungeonControls + methods + "\n}\n"
                + ScalarDependencies("Global.cs", "Global", methods)
                + ScalarDependencies("Option.cs", "Option", methods)
                + "public static class TINHKIEM {" + menpai + keys + "public static int Bool2Int(bool value) => value ? 1 : 0;}"
                + "public static class Setting {" + settings + "public static int[] Input; public static string Saved;"
                + "public static int[] LoadSettingOffline(string name) => Input;"
                + "public static void SaveSettingOffline(string name, string value) { Saved = value; }}";
            var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))
                .Split(Path.PathSeparator).Select(path => MetadataReference.CreateFromFile(path));
            var compilation = CSharpCompilation.Create("LegacyUiRegression", new[] { CSharpSyntaxTree.ParseText(source) },
                references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using var output = new MemoryStream();
            var result = compilation.Emit(output);
            Assert.True(result.Success, string.Join("\n", result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)));
            return output.ToArray();
        }

        private const string Fixture = """
            public enum Keys { D0=48, D1, D2, D3, D4, D5, D6, D7, D8, D9, F1=112, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, F13 }
            public enum ToolTipIcon { Info }
            public class FakeControl { public bool Checked, Enabled=true; public decimal Value; public string Text; public object ForeColor; }
            public class FakeNotify {
                public int Calls; public string LastText;
                public void ShowBalloonTip(int ms, string title, string text, ToolTipIcon icon) { Calls++; LastText=text; }
            }
            public class FakePlayer { public string Name = "TestCharacter"; }
            public static class Color { public static string Green = "green"; }
            public static class SystemColors { public static string ControlText = "normal"; }
            public static class MAP { public const int VoLuongSon=1, KinhHo=2, KiemCac=3, ThaiHo=4, TungSon=5, DonHoang=6; }
            public static class CanhBao {
                public enum Kieu { Eror, OK }
                public static int Calls; public static string LastTitle, LastText;
                public static void Msg(string title, string text, Kieu type) { Calls++; LastTitle=title; LastText=text; }
            }
            public class Game {
                public bool IsHP=true, IsMP=true, IsAcBa, IsTrungAc, IsLauLanTamBao, IsKyCuoc, IsThuyLao, IsTrieuTap=true;
                public int MapAcTac;
                public FakePlayer TLBB=new FakePlayer(); public static int TrongHoaX; public static bool IsHoldPK;
                public Game LeaderValue; public Action OnLeaderRead;
                public Game Leader { get { var value=LeaderValue; OnLeaderRead?.Invoke(); return value; } set { LeaderValue=value; } }
            }
            """;

        private const string DungeonControls = """
            public Dictionary<int, Game> dicGame = new Dictionary<int, Game>();
            public Game Leader => CurGame?.Leader;
            public FakeControl menuactac=new FakeControl(), ItemAcBa=new FakeControl(), ItemLauLan=new FakeControl(),
                itemTranLongKyCuoc=new FakeControl(), ItemThuyLao=new FakeControl(), ItemTrungAc=new FakeControl(), itemchuacodoi=new FakeControl(),
                tựĐộngToolStripMenuItem=new FakeControl(), vôLượngSơnToolStripMenuItem=new FakeControl(), kínhHồToolStripMenuItem=new FakeControl(),
                kiếmCácToolStripMenuItem=new FakeControl(), tháiHồToolStripMenuItem=new FakeControl(), tungSơnToolStripMenuItem=new FakeControl(), đônHoàngToolStripMenuItem=new FakeControl();
            public void OpenDungeonMenu() => chươngTrìnhToolStripMenuItem_Click(null, EventArgs.Empty);
            public void ClickDungeon(string name) {
                // WinForms applies CheckOnClick before raising the Click event.
                string menuName = name == "thoátToolStripMenuItem1_Click" ? "ItemLauLan" : name.Substring(0, name.Length - 6);
                var field = GetType().GetField(menuName);
                if (field != null && menuName != "menuactac") {
                    var menu = (FakeControl)field.GetValue(this); menu.Checked = !menu.Checked;
                }
                try { GetType().GetMethod(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    .Invoke(this, new object[] { null, EventArgs.Empty }); }
                catch (System.Reflection.TargetInvocationException ex) {
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                }
            }
            public Game SelectWithLeader() {
                CurGame = new Game();
                var leader = new Game(); leader.TLBB.Name="Leader"; CurGame.Leader=leader;
                dicGame.Add(dicGame.Count, CurGame); dicGame.Add(dicGame.Count, leader);
                return leader;
            }
            public int WarningCount => CanhBao.Calls;
            public string WarningTitle => CanhBao.LastTitle;
            public string WarningText => CanhBao.LastText;
            public FakeControl Menu(string name) => (FakeControl)GetType().GetField(name).GetValue(this);
            """;

        private const string Controls = """
            public Game CurGame = new Game();
            public bool SuppressNotice = true, IsCalender, AlarmAcBa;
            public FakeNotify notifyIcon1 = new FakeNotify();
            public FakeControl checkregenhp=new FakeControl(), checkrengenmp=new FakeControl(),
                numrangerpickitem=new FakeControl(), numcanhbaohp=new FakeControl(), numuplevel=new FakeControl(),
                nudHP=new FakeControl(), nudMP=new FakeControl(), nudNM=new FakeControl(), numbankinhtheosau=new FakeControl(),
                checkBox7=new FakeControl(), checkauouplevel=new FakeControl(), checkBox10=new FakeControl(),
                CheckTriLieuComeback=new FakeControl(), checkBox9=new FakeControl(), checkdongytodoi=new FakeControl(), checkdongytoanbo=new FakeControl();
            public bool VuaBatXong() => SuppressNotice;
            private void SetHotKey() { }
            public void ChangeHp() => checkregenhp_CheckedChanged(null, EventArgs.Empty);
            public void ChangeMp() => checkrengenmp_CheckedChanged(null, EventArgs.Empty);
            public int MapIndex { get => Option.MapBanDoIndex; set => Option.MapBanDoIndex=value; }
            public int HealMapIndex { get => Option.MaptriLieuIndex; set => Option.MaptriLieuIndex=value; }
            public void Load(int[] values) { Setting.Input=values; LoadSetting(); }
            public string Save() { SaveSetting(); return Setting.Saved; }
            public void LoadSaved() { Setting.Input=Setting.String2Arr(Setting.Saved); LoadSetting(); }
            """;
    }
}
