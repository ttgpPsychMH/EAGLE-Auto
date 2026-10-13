using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TinhKiemAuto.Appearance;
using TinhKiemAuto.Dashboard;

namespace TinhKiemAuto
{
    public partial class FrmMain
    {
        internal bool CompactUiReady { get; private set; }
        private TabControl classicNavigation;
        private Panel classicViewport;
        private bool classicSelecting;

        private void InitializeClassicLayout(TableLayoutPanel header, TableLayoutPanel list,
            TableLayoutPanel metrics, ToolStrip toolbar)
        {
            CompactUiReady = true;
            compactRoot.RowStyles[1].Height = 178;
            compactRoot.RowStyles[2].Height = 0;
            toolbar.Visible = false;
            header.Padding = Padding.Empty;
            list.Margin = Padding.Empty;
            list.RowStyles[0].Height = list.RowStyles[2].Height = 0;
            metrics.RowStyles[0].Height = 0;
            metrics.RowStyles[0].SizeType = SizeType.Absolute;
            dashboardSelection.Visible = false;
            BackColor = SystemColors.Control;
            menuStrip1.BackColor = SystemColors.Control;
            txtlogs.BackColor = SystemColors.Control;
            txtlogs.ScrollBars = RichTextBoxScrollBars.Vertical;
            TablControl.Multiline = false;
            TablControl.ItemSize = new Size(60, 24);
            TablControl.SizeMode = TabSizeMode.Fixed;
            TablControl.Dock = DockStyle.None;
            TablControl.Margin = Padding.Empty;
            classicNavigation = new TabControl { Dock = DockStyle.Top, Height = 27,
                Name = "classicNavigation", SizeMode = TabSizeMode.Normal };
            foreach (string caption in CompactNavigationPolicy.Captions)
                classicNavigation.TabPages.Add(new TabPage(caption));
            classicViewport = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty };
            classicViewport.Controls.Add(TablControl);
            var body = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty };
            body.Controls.Add(classicViewport); body.Controls.Add(classicNavigation);
            compactRoot.Controls.Add(body, 0, 4);
            classicViewport.SizeChanged += (s, e) => FitClassicContent();
            TablControl.FontChanged += (s, e) => FitClassicContent();
            classicNavigation.SelectedIndexChanged += (s, e) => {
                if (classicSelecting || classicNavigation.SelectedIndex < 0) return;
                SelectClassicPage(CompactNavigationPolicy.LegacyIndex(classicNavigation.SelectedIndex));
            };
            TablControl.SelectedIndexChanged += (s, e) => {
                classicSelecting = true;
                try { classicNavigation.SelectedIndex = CompactNavigationPolicy.FrontIndex(TablControl.SelectedIndex); }
                finally { classicSelecting = false; }
            };
            BuildClassicMenus();
            foreach (GroupBox group in new[] { groupBox1, groupBox2, groupBox3 })
            {
                group.Tag = "compactGroup";
                group.MinimumSize = Size.Empty;
                group.Layout += (s, e) => ArrangeClassicGroup(group);
                group.FontChanged += (s, e) => ArrangeClassicGroup(group);
                ArrangeClassicGroup(group);
            }
            groupBox2.Visible = false;
            checkregenhp.Text = "Item HP"; checkrengenmp.Text = "Item MP";
            checkcongsinh.Text = "Cộng sinh"; checkhuyette.Text = "Huyết tế";
            checkisNM.Text = "NM buff"; checkBox7.Text = "Báo HP";
            SelectClassicPage(0);
            FitClassicContent();
        }

        private void SelectClassicPage(int index)
        {
            if (index < 0 || index >= TablControl.TabPages.Count) return;
            TablControl.SelectedIndex = index; // Original index-based crafting/login handlers stay intact.
            FitClassicContent();
        }
        private void FitClassicContent()
        {
            if (classicViewport == null || classicViewport.IsDisposed) return;
            // Clip only the legacy tab header. All eight original pages and their event handlers stay in place.
            int top = TablControl.DisplayRectangle.Top;
            if (top <= 0) top = 28;
            TablControl.SetBounds(-2, -top, classicViewport.ClientSize.Width + 4,
                Math.Max(1, classicViewport.ClientSize.Height + top + 2));
        }
        private void BuildClassicMenus()
        {
            var help = new ToolStripMenuItem("Giúp đỡ");
            tùyChọnToolStripMenuItem.Text = "Tệp tin";
            foreach (ToolStripItem item in new ToolStripItem[] { thôngTinAUTOToolStripMenuItem,
                thôngTinCậpNhậtToolStripMenuItem, hookDiagnosticsToolStripMenuItem })
                help.DropDownItems.Add(item);
            menuStrip1.Items.Remove(chươngTrìnhToolStripMenuItem);
            chươngTrìnhToolStripMenuItem.Text = "Nhiệm vụ (menu cũ)";
            tùyChọnToolStripMenuItem.DropDownItems.Add(chươngTrìnhToolStripMenuItem);
            var advanced = new ToolStripMenuItem("Nâng cao");
            int[] indexes = CompactNavigationPolicy.AdvancedIndexes;
            foreach (int index in indexes)
            {
                int captured = index;
                advanced.DropDownItems.Add(TablControl.TabPages[index].Text, null, (s, e) => SelectClassicPage(captured));
            }
            var travel = new ToolStripMenuItem("Di chuyển / lên bãi") { CheckOnClick = true };
            travel.CheckedChanged += (s, e) => { groupBox2.Visible = travel.Checked; SelectClassicPage(0); };
            advanced.DropDownItems.Add(travel);
            var shared = new ToolStripMenuItem("Điều khiển chung") { CheckOnClick = true };
            shared.CheckedChanged += (s, e) => sharedToggle.Checked = shared.Checked;
            var log = new ToolStripMenuItem("Hiện nhật ký") { CheckOnClick = true, Checked = true };
            log.CheckedChanged += (s, e) => logToggle.Checked = log.Checked;
            tùyChọnToolStripMenuItem.DropDownItems.Add(advanced);
            tùyChọnToolStripMenuItem.DropDownItems.Add(shared);
            tùyChọnToolStripMenuItem.DropDownItems.Add(log);
            menuStrip1.Items.Add(help);
        }

        private bool arrangingClassicGroup;
        private void ArrangeClassicGroup(GroupBox group)
        {
            if (arrangingClassicGroup) return;
            arrangingClassicGroup = true;
            try
            {
                float scale = Math.Max(96, DeviceDpi) / 96f;
                int gap = (int)Math.Ceiling(5 * scale), rowHeight = (int)Math.Ceiling(28 * scale);
                if (group == groupBox1)
                {
                    Action<Control,int,int,int,int> place = (c,x,y,w,h) => c.SetBounds((int)(x*scale),(int)(y*scale),(int)(w*scale),(int)(h*scale));
                    place(chekcdanhquai,5,23,125,26); place(butboqua,135,23,24,24);
                    place(checkradius,5,51,125,26); place(numberdanhquanh,135,51,45,24);
                    place(rad11,195,23,55,26); place(radgom,250,23,70,26);
                    group.Height = (int)(82*scale);
                }
                else if (group == groupBox3)
                {
                    // Existing checkbox/threshold triples, with no value or event changes.
                    Control[][] rows = {
                        new Control[]{checkregenhp,label14,nudHP}, new Control[]{checkrengenmp,label13,nudMP},
                        new Control[]{checkcongsinh,label15,numbercongsinhhp}, new Control[]{checkhuyette,label16,numhuyettemp},
                        new Control[]{checkisNM,label17,nudNM}, new Control[]{checkBox7,label18,numcanhbaohp},
                        new Control[]{CheckAutoHoiSinh}, new Control[]{CheckTriLieuComeback}
                    };
                    int columns = group.ClientSize.Width >= 300 * scale ? 2 : 1;
                    int cellWidth = Math.Max(120, (group.ClientSize.Width - gap * 3) / columns);
                    for (int i = 0; i < rows.Length; i++)
                    {
                        int x = gap + (i / 4) % columns * (cellWidth + gap);
                        int y = (int)(23 * scale) + (columns == 2 ? i % 4 : i) * rowHeight;
                        var controls = rows[i];
                        if (controls.Length == 1) { controls[0].SetBounds(x, y, cellWidth, rowHeight-2); continue; }
                        int numericWidth = (int)(45 * scale), symbolWidth = (int)(12 * scale);
                        controls[0].AutoSize = false; controls[0].SetBounds(x,y,cellWidth-numericWidth-symbolWidth,rowHeight-2);
                        controls[1].SetBounds(x+cellWidth-numericWidth-symbolWidth,y+3,symbolWidth,rowHeight-4);
                        controls[2].SetBounds(x+cellWidth-numericWidth,y,numericWidth,rowHeight-2);
                    }
                    group.Height = (int)(27 * scale) + (columns == 2 ? 4 : 8) * rowHeight;
                }
                // Other fixed legacy groups remain scrollable when a narrow display needs more room.
            }
            finally { arrangingClassicGroup = false; }
        }
    }
}
