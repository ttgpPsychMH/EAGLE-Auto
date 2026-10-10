using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace ChickenAutoEx.Startup.Tests
{
    public sealed class DungeonMenuRegressionTests
    {
        private static readonly string[] Menus = { "menuactac", "ItemAcBa", "ItemLauLan",
            "itemTranLongKyCuoc", "ItemThuyLao", "ItemTrungAc" };
        private static readonly string[] MapMenus = { "vôLượngSơnToolStripMenuItem", "kínhHồToolStripMenuItem",
            "kiếmCácToolStripMenuItem", "tháiHồToolStripMenuItem", "tungSơnToolStripMenuItem", "đônHoàngToolStripMenuItem" };

        public static IEnumerable<object[]> Handlers()
        {
            foreach (var name in new[] { "ItemTrungAc_Click", "ItemAcBa_Click", "ItemThuyLao_Click", "itemTranLongKyCuoc_Click",
                "thoátToolStripMenuItem1_Click", "tựĐộngToolStripMenuItem_Click", "dUwngfToolStripMenuItem_Click" }
                .Concat(MapMenus.Select(name => name + "_Click")))
                yield return new object[] { name };
        }

        [Theory]
        [MemberData(nameof(Handlers))]
        public void ClickWithMissingSelectionOrLeaderDoesNotCrashOrChangeGame(string handler)
        {
            foreach (bool removeSelection in new[] { true, false })
            {
                dynamic form = LegacyUiHarness.Create();
                dynamic leader = form.SelectWithLeader();
                leader.IsTrungAc = true;
                leader.MapAcTac = 3;
                form.OpenDungeonMenu();
                // Simulate selection/team disappearing after opening the menu, including
                // WinForms CheckOnClick having set a stale checkmark before the handler.
                foreach (string name in Menus.Concat(MapMenus)) form.Menu(name).Checked = true;
                if (removeSelection) form.CurGame = null;
                else form.CurGame.Leader = null;
                Assert.Null(Record.Exception(() => { form.ClickDungeon(handler); }));
                Assert.True((bool)leader.IsTrungAc);
                Assert.Equal(3, (int)leader.MapAcTac);
                Assert.True((bool)leader.IsTrieuTap);
                if (handler == "ItemTrungAc_Click" && !removeSelection)
                {
                    Assert.True((bool)form.CurGame.IsTrungAc);
                    Assert.True((bool)form.ItemTrungAc.Checked);
                    Assert.Equal(1, (int)form.notifyIcon1.Calls);
                    Assert.Equal(0, (int)form.WarningCount);
                    Assert.True((bool)form.ItemTrungAc.Enabled);
                    continue;
                }
                Assert.Equal(0, (int)form.notifyIcon1.Calls);
                Assert.Equal(1, (int)form.WarningCount);
                AssertUnavailable(form);
                if (handler == "ItemTrungAc_Click") Assert.Equal("Lỗi Trừng Ác", (string)form.WarningTitle);
            }
        }

        [Theory]
        [InlineData("ItemTrungAc_Click", "IsTrungAc", "ItemTrungAc", "Auto Trừng Ác", false)]
        [InlineData("ItemTrungAc_Click", "IsTrungAc", "ItemTrungAc", "Auto Trừng Ác", true)]
        [InlineData("itemTranLongKyCuoc_Click", "IsKyCuoc", "itemTranLongKyCuoc", "Auto Trân Long Kỳ Cuộc", false)]
        [InlineData("itemTranLongKyCuoc_Click", "IsKyCuoc", "itemTranLongKyCuoc", "Auto Trân Long Kỳ Cuộc", true)]
        [InlineData("thoátToolStripMenuItem1_Click", "IsLauLanTamBao", "ItemLauLan", "Auto Lâu Lan Tầm Bảo", false)]
        [InlineData("thoátToolStripMenuItem1_Click", "IsLauLanTamBao", "ItemLauLan", "Auto Lâu Lan Tầm Bảo", true)]
        public void ToggleNoticeAndCheckmarkMatchTheSelectedQuest(string handler, string field, string menu, string text, bool initial)
        {
            dynamic form = LegacyUiHarness.Create();
            object leader = form.SelectWithLeader();
            object controlled = handler == "ItemTrungAc_Click" ? form.CurGame : leader;
            var flag = controlled.GetType().GetField(field);
            // Lâu Lan's value deliberately differs from the toggled Trừng Ác/Kỳ Cuộc value.
            leader.GetType().GetField("IsLauLanTamBao").SetValue(leader, initial);
            flag.SetValue(controlled, initial);
            form.OpenDungeonMenu();
            foreach (bool expected in new[] { !initial, initial })
            {
                form.ClickDungeon(handler);
                Assert.Equal(expected, (bool)flag.GetValue(controlled));
                Assert.Equal(expected, (bool)form.Menu(menu).Checked);
                Assert.Contains((expected ? " Bật " : " Tắt ") + text, (string)form.notifyIcon1.LastText);
                if (field != "IsLauLanTamBao")
                    Assert.Equal(initial, (bool)leader.GetType().GetField("IsLauLanTamBao").GetValue(leader));
                Assert.Equal(0, (int)form.WarningCount);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void OpeningMenuReflectsSelectionAndTeamWithoutMutatingQuestState(int context)
        {
            dynamic form = LegacyUiHarness.Create();
            dynamic leader = form.SelectWithLeader();
            leader.IsTrungAc = true;
            leader.MapAcTac = 3;
            if (context == 0) form.CurGame = null;
            if (context == 1) form.CurGame.Leader = null;
            form.OpenDungeonMenu();
            Assert.True((bool)leader.IsTrungAc);
            Assert.Equal(3, (int)leader.MapAcTac);
            Assert.Equal(0, (int)form.WarningCount);
            if (context != 2) AssertUnavailable(form);
            else
            {
                foreach (string name in Menus) Assert.True((bool)form.Menu(name).Enabled);
                Assert.False((bool)form.ItemTrungAc.Checked); // Selected member's personal quest is independent of leader.
                Assert.True((bool)form.kiếmCácToolStripMenuItem.Checked);
                Assert.Contains("Leader", (string)form.itemchuacodoi.Text);
            }
        }

        [Fact]
        public void LosingSelectionWhileResolvingLeaderRejectsTheClick()
        {
            dynamic form = LegacyUiHarness.Create();
            dynamic leader = form.SelectWithLeader();
            form.CurGame.OnLeaderRead = (Action)(() => form.CurGame = null);
            Assert.Null(Record.Exception(() => { form.ClickDungeon("ItemAcBa_Click"); }));
            Assert.False((bool)leader.IsTrungAc);
            Assert.Equal(0, (int)form.notifyIcon1.Calls);
            AssertUnavailable(form);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void MissingCharacterMetadataRejectsTheClick(bool missingLeaderMetadata)
        {
            dynamic form = LegacyUiHarness.Create();
            dynamic leader = form.SelectWithLeader();
            if (missingLeaderMetadata) leader.TLBB = null;
            else form.CurGame.TLBB = null;
            Assert.Null(Record.Exception(() => { form.ClickDungeon("ItemAcBa_Click"); }));
            Assert.False((bool)leader.IsTrungAc);
            AssertUnavailable(form);
        }

        [Fact]
        public void ThuyLaoKeepsItsExistingDisabledEngineBehaviorAndClearsCheckmark()
        {
            dynamic form = LegacyUiHarness.Create();
            dynamic leader = form.SelectWithLeader();
            leader.IsThuyLao = true;
            leader.IsTrungAc = true;
            form.ItemThuyLao.Checked = true;
            form.ClickDungeon("ItemThuyLao_Click");
            Assert.False((bool)leader.IsThuyLao);
            Assert.False((bool)form.ItemThuyLao.Checked);
            Assert.True((bool)leader.IsTrungAc);
            Assert.Contains("Not work", (string)form.WarningText);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void AcBaToggleKeepsResetPolicyAndRefreshesOtherCheckmarks(bool initial)
        {
            dynamic form = LegacyUiHarness.Create();
            dynamic leader = form.SelectWithLeader();
            leader.IsAcBa = initial;
            leader.IsTrungAc = leader.IsKyCuoc = leader.IsLauLanTamBao = leader.IsThuyLao = true;
            leader.MapAcTac = 3;
            form.OpenDungeonMenu();
            form.ClickDungeon("ItemAcBa_Click");
            Assert.Equal(!initial, (bool)leader.IsAcBa);
            Assert.Equal(!initial, (bool)form.ItemAcBa.Checked);
            Assert.Contains((initial ? " Tắt " : " Bật ") + "ÁC BÁ", (string)form.notifyIcon1.LastText);
            foreach (string name in Menus.Where(n => n != "ItemAcBa" && n != "menuactac"))
                Assert.False((bool)form.Menu(name).Checked);
            Assert.Equal(0, (int)leader.MapAcTac);
            foreach (string name in MapMenus) Assert.False((bool)form.Menu(name).Checked);
        }

        [Theory]
        [InlineData("vôLượngSơnToolStripMenuItem_Click", 1)]
        [InlineData("kínhHồToolStripMenuItem_Click", 2)]
        [InlineData("kiếmCácToolStripMenuItem_Click", 3)]
        [InlineData("tháiHồToolStripMenuItem_Click", 4)]
        [InlineData("tungSơnToolStripMenuItem_Click", 5)]
        [InlineData("đônHoàngToolStripMenuItem_Click", 6)]
        [InlineData("tựĐộngToolStripMenuItem_Click", 0)]
        public void AcTacSelectionShowsOneMapAndPreservesOtherQuestFlags(string handler, int expectedMap)
        {
            dynamic form = LegacyUiHarness.Create();
            dynamic leader = form.SelectWithLeader();
            leader.IsTrungAc = true;
            foreach (string name in MapMenus) form.Menu(name).Checked = true;
            form.ClickDungeon(handler);
            int actualMap = leader.MapAcTac;
            if (expectedMap == 0) Assert.InRange(actualMap, 1, 6);
            else Assert.Equal(expectedMap, actualMap);
            Assert.True((bool)leader.IsTrungAc);
            Assert.False((bool)leader.IsTrieuTap);
            for (int i = 0; i < MapMenus.Length; i++)
                Assert.Equal(actualMap == i + 1, (bool)form.Menu(MapMenus[i]).Checked);
        }

        [Fact]
        public void StoppingAcTacClearsMapCheckmarksWithoutStoppingOtherQuests()
        {
            dynamic form = LegacyUiHarness.Create();
            dynamic leader = form.SelectWithLeader();
            leader.MapAcTac = 3;
            leader.IsTrungAc = true;
            form.OpenDungeonMenu();
            form.ClickDungeon("dUwngfToolStripMenuItem_Click");
            Assert.Equal(0, (int)leader.MapAcTac);
            Assert.True((bool)leader.IsTrungAc);
            foreach (string name in MapMenus) Assert.False((bool)form.Menu(name).Checked);
        }

        [Fact]
        public void SwitchingCharacterRefreshesTheMenuFromItsOwnLeader()
        {
            dynamic form = LegacyUiHarness.Create();
            dynamic first = form.SelectWithLeader();
            first.IsTrungAc = true;
            form.OpenDungeonMenu();
            dynamic second = form.SelectWithLeader();
            second.IsKyCuoc = true;
            form.OpenDungeonMenu();
            Assert.False((bool)form.ItemTrungAc.Checked);
            Assert.True((bool)form.itemTranLongKyCuoc.Checked);
            Assert.True((bool)first.IsTrungAc);
            Assert.False((bool)second.IsTrungAc);
        }

        [Fact]
        public void DesignerRefreshesTheMenuOnDropDownOpeningIncludingKeyboardNavigation()
        {
            var form = LegacyUiHarness.SourceClass("FrmMain.cs", "FrmMain");
            var initialize = form.Members.OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Text == "InitializeComponent");
            Assert.Contains(initialize.DescendantNodes().OfType<AssignmentExpressionSyntax>(), a =>
                a.Left.ToString() == "this.chươngTrìnhToolStripMenuItem.DropDownOpening" &&
                a.Right.ToString().Contains("chươngTrìnhToolStripMenuItem_Click"));
        }

        private static void AssertUnavailable(dynamic form)
        {
            foreach (string name in Menus)
            {
                if (name == "ItemTrungAc")
                {
                    bool personalAvailable = form.CurGame != null && form.CurGame.TLBB != null;
                    Assert.Equal(personalAvailable, (bool)form.Menu(name).Enabled);
                    Assert.Equal(personalAvailable && (bool)form.CurGame.IsTrungAc, (bool)form.Menu(name).Checked);
                    continue;
                }
                Assert.False((bool)form.Menu(name).Enabled);
                Assert.False((bool)form.Menu(name).Checked);
            }
            foreach (string name in MapMenus) Assert.False((bool)form.Menu(name).Checked);
        }

        [Theory][InlineData(false)][InlineData(true)]
        public void TrungAcControlsSelectedCharacterWithOrWithoutTeam(bool withoutTeam)
        {
            dynamic form=LegacyUiHarness.Create();dynamic leader=form.SelectWithLeader();
            if(withoutTeam)form.CurGame.Leader=null;
            form.OpenDungeonMenu();form.ClickDungeon("ItemTrungAc_Click");
            Assert.True((bool)form.CurGame.IsTrungAc);Assert.False((bool)leader.IsTrungAc);
            Assert.True((bool)form.ItemTrungAc.Checked);Assert.Contains("TESTCHARACTER",(string)form.notifyIcon1.LastText);
            form.ClickDungeon("ItemTrungAc_Click");Assert.False((bool)form.CurGame.IsTrungAc);
        }

        [Fact] public void TrungAcMissingSelectedMetadataCannotToggleAnotherCharacter()
        {
            dynamic form=LegacyUiHarness.Create();dynamic leader=form.SelectWithLeader();form.CurGame.TLBB=null;
            form.ClickDungeon("ItemTrungAc_Click");Assert.False((bool)leader.IsTrungAc);Assert.Equal(1,(int)form.WarningCount);
            AssertUnavailable(form);
        }
    }
}
