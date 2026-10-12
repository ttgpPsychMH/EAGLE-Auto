using System;
using System.Drawing;
using System.Windows.Forms;
using TinhKiemAuto.Dashboard;

namespace TinhKiemAuto
{
    public partial class FrmMain
    {
        private TabPage dungeonDashboardTab;
        private ComboBox dungeonTask, dungeonOption;
        private Label dashboardSelection, dungeonController, dungeonState, dungeonReason, dungeonFeedback;
        private Button dungeonStart, dungeonStop;
        private DungeonDashboardAdapter dungeonAdapter;
        private DungeonBinding displayedDungeonBinding, feedbackBinding;
        private DungeonKind feedbackKind;
        private bool dashboardReady;
        private TableLayoutPanel compactRoot;
        private GroupBox sharedControls;
        private ToolStripButton sharedToggle, logToggle;
        private Label headerHP, headerMP, headerPet, headerPosition, headerOnline;
        private ToolStripStatusLabel compactStatus;

        private sealed class DashboardChoice
        {
            internal string Text;
            internal int Value;
            internal DashboardChoice(string text, int value) { Text = text; Value = value; }
            public override string ToString() { return Text; }
        }

        private static Label DashboardLabel(string text)
        {
            return new Label { Text = text, Dock = DockStyle.Fill, AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(36, 48, 64),
                Padding = new Padding(8, 0, 8, 0), UseMnemonic = false };
        }

        private Game DashboardSelectedGame()
        {
            // Unlike legacy SelectedGames, no selection never means all characters.
            if (ListViewNhanVat.SelectedItems.Count != 1) return null;
            var game = ListViewNhanVat.SelectedItems[0].Tag as Game;
            return ReferenceEquals(game, CurGame) ? game : null;
        }

        private void InitializeDungeonDashboard()
        {
            SuspendLayout();
            MaximumSize = Size.Empty;
            MinimumSize = new Size(440, 520);
            ClientSize = new Size(560, 700);
            BackColor = Color.FromArgb(245, 247, 250);

            // Keep the seven legacy tabs in their original order (crafting/login use indexes).
            string[] titles = { "Cơ bản", "Kỹ năng", "Vật phẩm", "Pet", "Tiện ích", "Chế đồ", "Đăng nhập" };
            TablControl.Multiline = true;
            TablControl.ItemSize = new Size(54, 24);
            for (int i = 0; i < 7; i++)
            {
                TablControl.TabPages[i].Text = titles[i];
                TablControl.TabPages[i].AutoScroll = true;
                TablControl.TabPages[i].BackColor = Color.White;
            }
            TabKyNangControl.Dock = DockStyle.Fill;
            StyleLegacyControls(TablControl);
            ArrangeBasicGroups();
            menuStrip1.RenderMode = ToolStripRenderMode.System;
            menuStrip1.BackColor = Color.FromArgb(245, 247, 250);
            tùyChọnToolStripMenuItem.Text = "Tiện ích";
            chươngTrìnhToolStripMenuItem.Text = "Nhiệm vụ";
            txtlogs.BackColor = Color.White;
            txtlogs.BorderStyle = BorderStyle.FixedSingle;

            compactRoot = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 7,
                Padding = Padding.Empty, Margin = Padding.Empty };
            compactRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            compactRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            compactRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 140));
            compactRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            compactRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 0)); // Shared controls, initially folded.
            compactRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            compactRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
            compactRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));

            var header = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1,
                Padding = new Padding(6, 4, 6, 4), Margin = Padding.Empty };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
            panellistview.Dock = DockStyle.Fill;
            panellistview.Margin = Padding.Empty;
            ListViewNhanVat.BackColor = Color.White;
            ListViewNhanVat.HeaderStyle = ColumnHeaderStyle.None;
            txtNhanVat.Text = "Nhân vật";
            ListViewNhanVat.Resize += (sender, args) => txtNhanVat.Width = Math.Max(100, ListViewNhanVat.ClientSize.Width - 24);
            var listHost = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3,
                Margin = new Padding(0, 0, 6, 0), BackColor = Color.White };
            listHost.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
            listHost.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            listHost.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            var listTitle = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty };
            listTitle.Controls.Add(DashboardLabel("Nhân vật"));
            var clearSelection = new LinkLabel { Text = "Bỏ chọn", Dock = DockStyle.Right, Width = 64,
                TextAlign = ContentAlignment.MiddleCenter, AccessibleName = "Bỏ chọn nhân vật, giữ trạng thái Auto" };
            clearSelection.LinkClicked += (sender, args) => {
                for (int i = ListViewNhanVat.SelectedItems.Count - 1; i >= 0; i--)
                    ListViewNhanVat.SelectedItems[i].Selected = false;
                RefreshDungeonDashboard();
            };
            toolTip1.SetToolTip(clearSelection, "Bỏ lựa chọn cấu hình; không tắt Auto của nhân vật.");
            listTitle.Controls.Add(clearSelection); clearSelection.BringToFront();
            listHost.Controls.Add(listTitle, 0, 0);
            listHost.Controls.Add(panellistview, 0, 1);
            listHost.Controls.Add(DashboardLabel("Chọn dòng để cấu hình • Tick để bật Auto"), 0, 2);
            header.Controls.Add(listHost, 0, 0);
            var metrics = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 6, Margin = Padding.Empty };
            foreach (float height in new[] { 20f, 14f, 14f, 14f, 20f, 18f }) metrics.RowStyles.Add(new RowStyle(SizeType.Percent, height));
            dashboardSelection = DashboardLabel("Chưa chọn nhân vật");
            dashboardSelection.AutoEllipsis = true;
            metrics.Controls.Add(dashboardSelection, 0, 0);
            headerHP = DashboardLabel("HP —"); headerMP = DashboardLabel("MP —"); headerPet = DashboardLabel("Pet —");
            metrics.Controls.Add(headerHP, 0, 1); metrics.Controls.Add(headerMP, 0, 2); metrics.Controls.Add(headerPet, 0, 3);
            headerPosition = DashboardLabel("Vị trí: —"); headerPosition.AutoEllipsis = true;
            headerOnline = DashboardLabel("Online: —");
            metrics.Controls.Add(headerPosition, 0, 4); metrics.Controls.Add(headerOnline, 0, 5);
            header.Controls.Add(metrics, 1, 0);

            var toolbar = new ToolStrip { Dock = DockStyle.Fill, GripStyle = ToolStripGripStyle.Hidden,
                BackColor = BackColor, RenderMode = ToolStripRenderMode.System, Padding = new Padding(6, 0, 6, 0) };
            sharedToggle = new ToolStripButton("Điều khiển chung") { CheckOnClick = true, ToolTipText = "Mở các thao tác dùng chung cho nhiều cửa sổ Auto." };
            logToggle = new ToolStripButton("Nhật ký") { CheckOnClick = true, Checked = true, Alignment = ToolStripItemAlignment.Right };
            toolbar.Items.Add(sharedToggle); toolbar.Items.Add(logToggle);
            sharedControls = new GroupBox { Text = "Điều khiển chung • áp dụng cho nhiều cửa sổ", Dock = DockStyle.Fill,
                Visible = false, Margin = new Padding(6, 0, 6, 4), Padding = new Padding(4), BackColor = Color.White };
            panel3.Dock = DockStyle.Fill;
            panel3.BackColor = Color.White;
            panel3.AutoScroll = true;
            panel3.Controls.Add(new Label { Text = "Nhặt: tất cả nhân vật\nPet / Theo / Skill: cài đặt chung\nAuto: đảo trạng thái từng nhân vật",
                Location = new Point(286, 10), Size = new Size(238, 62), Anchor = AnchorStyles.Top | AnchorStyles.Left, UseMnemonic = false });
            button1.Text = "Đảo bật/tắt Auto"; button1.Size = new Size(148, 26);
            toolTip1.SetToolTip(button1, "Đổi trạng thái Auto của từng nhân vật: đang bật thành tắt, đang tắt thành bật.");
            buttrieutap.Text = "Triệu tập các đội"; buttrieutap.Size = new Size(148, 26);
            toolTip1.SetToolTip(buttrieutap, "Triệu tập các tổ đội về từng đội trưởng; áp dụng cho nhiều cửa sổ.");
            StyleLegacyControls(panel3);
            sharedControls.Controls.Add(panel3);
            sharedToggle.CheckedChanged += (sender, args) => UpdateCompactPanelBudget();
            logToggle.CheckedChanged += (sender, args) => UpdateCompactPanelBudget();
            compactRoot.SizeChanged += (sender, args) => UpdateCompactPanelBudget();
            menuStrip1.Dock = txtlogs.Dock = statusStrip1.Dock = DockStyle.Fill;
            txttrangthai.Visible = false; // The old timer may update this with CurGame after selection is cleared.
            compactStatus = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleLeft };
            statusStrip1.Items.Add(compactStatus);
            statusStrip1.ShowItemToolTips = true;
            menuStrip1.Margin = txtlogs.Margin = statusStrip1.Margin = Padding.Empty;
            TablControl.Margin = new Padding(6, 0, 6, 4);
            compactRoot.Controls.Add(menuStrip1, 0, 0); compactRoot.Controls.Add(header, 0, 1);
            compactRoot.Controls.Add(toolbar, 0, 2); compactRoot.Controls.Add(sharedControls, 0, 3);
            compactRoot.Controls.Add(TablControl, 0, 4); compactRoot.Controls.Add(txtlogs, 0, 5);
            compactRoot.Controls.Add(statusStrip1, 0, 6);
            // The old empty hosts retain their fields; no reused control/handler is disposed.
            Controls.Remove(panel1); Controls.Remove(paneltop);
            Controls.Add(compactRoot);

            dungeonAdapter = new DungeonDashboardAdapter(new GameDungeonDashboardPort(
                DashboardSelectedGame, game => game != null && dicGame.ContainsValue(game)));
            CreateDungeonDashboardTab();
            var refresh = new Timer(components) { Interval = 500 };
            refresh.Tick += (sender, args) => {
                if (Visible && WindowState != FormWindowState.Minimized) RefreshDungeonDashboard();
            };
            ListViewNhanVat.SelectedIndexChanged += (sender, args) => RefreshDungeonDashboard();
            TablControl.SelectedIndexChanged += (sender, args) => RefreshDungeonDashboard();
            Shown += (sender, args) => FitCompactWindow();
            dashboardReady = true;
            ApplyCharacterHeader(CharacterHeader.Empty(0));
            refresh.Start();
            ResumeLayout(true);
        }

        private float UiScale { get { return Math.Max(1f, Font.GetHeight() / 13f); } }
        private float SharedControlsHeight() { return 132 * UiScale; }
        private float LogPanelHeight() { return 76 * UiScale; }

        private void UpdateCompactPanelBudget()
        {
            float fixedHeight = compactRoot.RowStyles[0].Height + compactRoot.RowStyles[1].Height
                + compactRoot.RowStyles[2].Height + compactRoot.RowStyles[6].Height;
            var budget = CompactPanelBudget.Allocate(compactRoot.ClientSize.Height, fixedHeight, 180 * UiScale,
                sharedToggle.Checked ? SharedControlsHeight() : 0, logToggle.Checked ? LogPanelHeight() : 0);
            compactRoot.RowStyles[3].Height = budget.Shared;
            compactRoot.RowStyles[5].Height = budget.Log;
            sharedControls.Visible = sharedToggle.Checked && budget.Shared > 0;
            txtlogs.Visible = logToggle.Checked && budget.Log > 0;
            logToggle.Text = logToggle.Checked && budget.Log == 0 ? "Nhật ký (thiếu chỗ)" : "Nhật ký";
            logToggle.ToolTipText = "Gập/mở nhật ký. Nhật ký tự thu khi thiếu chỗ và hiện lại khi kéo cửa sổ cao hơn.";
        }

        private void FitCompactWindow()
        {
            var area = Screen.FromControl(this).WorkingArea;
            int width = Math.Max(1, area.Width - 16), height = Math.Max(1, area.Height - 16);
            MinimumSize = new Size(Math.Min(MinimumSize.Width, width), Math.Min(MinimumSize.Height, height));
            Size = new Size(Math.Min(Width, width), Math.Min(Height, height));
            Location = new Point(Math.Max(area.Left, Math.Min(Left, area.Right - Width)),
                Math.Max(area.Top, Math.Min(Top, area.Bottom - Height)));
            UpdateCompactPanelBudget();
        }

        private static void StyleLegacyControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                var button = control as Button;
                if (button != null)
                {
                    button.UseVisualStyleBackColor = true;
                    button.BackColor = SystemColors.Control;
                    button.ForeColor = SystemColors.ControlText;
                    button.FlatStyle = FlatStyle.Standard;
                }
                StyleLegacyControls(control);
            }
        }

        private void ArrangeBasicGroups()
        {
            int buttonGap = Math.Max(6, groupBox2.Width - butlenbai.Right);
            var groups = new[] { groupBox1, groupBox2, groupBox3 };
            string[] names = { "Đánh quái", "Di chuyển", "Hồi phục" };
            var stack = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, RowCount = 3,
                Padding = new Padding(4), Margin = Padding.Empty };
            stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int i = 0; i < groups.Length; i++)
            {
                var group = groups[i];
                // Keep each group and its radio parent intact; move controls only vertically for the legend.
                foreach (Control child in group.Controls) child.Top += 12;
                group.Text = names[i];
                group.Height += 14;
                group.MinimumSize = new Size(358, group.Height);
                group.Dock = DockStyle.Fill;
                group.Margin = new Padding(2, 2, 2, 6);
                stack.RowStyles.Add(new RowStyle(SizeType.Absolute, group.Height + group.Margin.Vertical));
                stack.Controls.Add(group, 0, i);
            }
            tabtophop.Controls.Add(stack);
            // Stretch input fields with their group, keep the action buttons on its right.
            foreach (var button in new[] { butlenbai, buttrilieu, buttimbai }) button.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comdanhsachbando.Anchor = comlenbai.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Resize += (sender, args) => {
                foreach (var button in new[] { butlenbai, buttrilieu, buttimbai })
                    button.Left = groupBox2.ClientSize.Width - button.Width - (int)(buttonGap * UiScale);
                int right = butlenbai.Left;
                comdanhsachbando.Width = Math.Max(100, right - comdanhsachbando.Left - 12);
                comlenbai.Width = Math.Max(100, right - comlenbai.Left - 12);
            };
        }

        private void ApplyCharacterHeader(CharacterHeader header)
        {
            dashboardSelection.Text = header.Title;
            headerHP.Text = "HP " + header.HP; headerMP.Text = "MP " + header.MP; headerPet.Text = "Pet " + header.Pet;
            headerPosition.Text = "Vị trí: " + header.Position; headerOnline.Text = "Online: " + header.Online;
            compactStatus.Text = header.Title + " • " + header.Hint;
            compactStatus.ToolTipText = header.Hint;
            toolTip1.SetToolTip(dashboardSelection, header.Title);
            toolTip1.SetToolTip(headerPosition, header.Position);
            // Never reset Checked/Value/Text of legacy settings: those events write configuration.
            for (int i = 0; i < 6; i++) TablControl.TabPages[i].Enabled = header.CanConfigure;
            buttrieutap.Enabled = header.CanConfigure;
        }

        private CharacterHeader ReadCharacterHeader()
        {
            Game actor = DashboardSelectedGame();
            if (actor == null || !dicGame.ContainsValue(actor) || actor.TLBB == null)
                return CharacterHeader.Empty(ListViewNhanVat.SelectedItems.Count);
            var character = actor.TLBB;
            string id = character.Id;
            bool online = character.Online;
            var header = !online ? CharacterHeader.Unavailable(character.Name, false)
                : !actor.DashboardInitialized || string.IsNullOrEmpty(id) ? CharacterHeader.Unavailable(character.Name, true)
                : CharacterHeader.Live(character.Name, character.HPPercent, character.MPPercent, character.PetHPPercent,
                    CharacterHeader.Location(character.MapName, actor.CharX, actor.CharY), character.OnlineTimeSec, actor.IsAuto);
            if (!ReferenceEquals(actor, DashboardSelectedGame()) || !dicGame.ContainsValue(actor)
                || actor.TLBB == null || actor.TLBB.Id != id || actor.TLBB.Online != online
                || (header.CanConfigure && !actor.DashboardInitialized))
                return CharacterHeader.Empty(ListViewNhanVat.SelectedItems.Count);
            return header;
        }

        private void CreateDungeonDashboardTab()
        {
            dungeonDashboardTab = new TabPage("Nhiệm vụ") { AutoScroll = true,
                BackColor = Color.White, Font = new Font("Segoe UI", 9f), Name = "eagleDungeonDashboard" };
            var table = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true,
                ColumnCount = 1, RowCount = 9, Padding = new Padding(8), MinimumSize = new Size(380, 0) };
            foreach (int height in new[] { 20, 28, 28, 32, 28, 58, 36, 38, 28 })
                table.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
            table.Controls.Add(DashboardLabel("NHIỆM VỤ – PHÓ BẢN"), 0, 0);
            dungeonTask = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "dungeonTask", AccessibleName = "Chọn nhiệm vụ phó bản" };
            foreach (DungeonKind kind in Enum.GetValues(typeof(DungeonKind)))
                dungeonTask.Items.Add(new DashboardChoice(DungeonDashboardPolicy.Name(kind), (int)kind));
            table.Controls.Add(dungeonTask, 0, 1);
            dungeonOption = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "dungeonOption", AccessibleName = "Map hoặc môn phái cho lần bắt đầu" };
            toolTip1.SetToolTip(dungeonOption, "Lựa chọn áp dụng khi bấm Bắt đầu; không đổi nhiệm vụ đang chạy.");
            table.Controls.Add(dungeonOption, 0, 2);
            dungeonController = DashboardLabel("");
            dungeonState = DashboardLabel("");
            dungeonState.BackColor = Color.FromArgb(232, 241, 252);
            dungeonReason = DashboardLabel("");
            dungeonFeedback = DashboardLabel("");
            table.Controls.Add(dungeonController, 0, 3);
            table.Controls.Add(dungeonState, 0, 4);
            table.Controls.Add(dungeonReason, 0, 5);
            var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
            dungeonStart = new Button { Text = "Bắt đầu", Size = new Size(112, 30), Name = "dungeonStart",
                BackColor = Color.FromArgb(31, 105, 181), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            dungeonStop = new Button { Text = "Dừng nhiệm vụ", Size = new Size(124, 30), Name = "dungeonStop" };
            buttons.Controls.Add(dungeonStart); buttons.Controls.Add(dungeonStop);
            table.Controls.Add(buttons, 0, 6);
            table.Controls.Add(dungeonFeedback, 0, 7);
            table.Controls.Add(DashboardLabel("Chọn nhân vật, bật Auto rồi bắt đầu. Map/môn phái áp dụng khi bắt đầu."), 0, 8);
            dungeonDashboardTab.Controls.Add(table);
            // Append, never insert: preserve the index-based legacy login/crafting handlers.
            TablControl.TabPages.Add(dungeonDashboardTab);
            dungeonTask.SelectedIndexChanged += (sender, args) => { UpdateDungeonOptions(); RefreshDungeonDashboard(); };
            dungeonOption.SelectedIndexChanged += (sender, args) => dungeonFeedback.Text = "";
            dungeonStart.Click += (sender, args) => ExecuteDungeonDashboard(true);
            dungeonStop.Click += (sender, args) => ExecuteDungeonDashboard(false);
            dungeonTask.SelectedIndex = 0;
        }

        private DungeonKind DashboardKind { get { return (DungeonKind)((DashboardChoice)dungeonTask.SelectedItem).Value; } }

        private void UpdateDungeonOptions()
        {
            displayedDungeonBinding = feedbackBinding = null;
            dungeonFeedback.Text = "";
            dungeonOption.Items.Clear();
            if (DashboardKind == DungeonKind.AcTac)
            {
                dungeonOption.Items.Add(new DashboardChoice("Map ngẫu nhiên (cơ chế cũ)", 0));
                dungeonOption.Items.Add(new DashboardChoice("Vô Lượng Sơn", MAP.VoLuongSon));
                dungeonOption.Items.Add(new DashboardChoice("Kính Hồ", MAP.KinhHo));
                dungeonOption.Items.Add(new DashboardChoice("Kiếm Các", MAP.KiemCac));
                dungeonOption.Items.Add(new DashboardChoice("Thái Hồ", MAP.ThaiHo));
                dungeonOption.Items.Add(new DashboardChoice("Tung Sơn", MAP.TungSon));
                dungeonOption.Items.Add(new DashboardChoice("Đôn Hoàng", MAP.DonHoang));
            }
            else if (DashboardKind == DungeonKind.AcBa)
            {
                dungeonOption.Items.Add(new DashboardChoice("Nhận diện từ thông báo hệ thống", -1));
                foreach (int school in AcBaEvents.Schools)
                    dungeonOption.Items.Add(new DashboardChoice("Thủ công: " + AcBaEvents.SchoolName(school), school));
            }
            else dungeonOption.Items.Add(new DashboardChoice("Thiết lập theo engine 107 hiện tại", 0));
            dungeonOption.SelectedIndex = 0;
            dungeonOption.Enabled = DashboardKind == DungeonKind.AcTac || DashboardKind == DungeonKind.AcBa;
        }

        private void RefreshDungeonDashboard()
        {
            if (!dashboardReady || IsDisposed) return;
            try
            {
                ApplyCharacterHeader(ReadCharacterHeader());
                if (TablControl.SelectedTab != dungeonDashboardTab) return;
                var view = dungeonAdapter.Read(DashboardKind);
                displayedDungeonBinding = view.Context == null ? null : view.Context.Binding;
                dungeonStart.Enabled = view.CanStart;
                dungeonStop.Enabled = view.CanStop;
                dungeonState.Text = view.State;
                dungeonReason.Text = view.Reason;
                string who = view.Context == null ? null : view.Context.ControllerName;
                dungeonController.Text = (DungeonDashboardPolicy.IsPersonal(DashboardKind) ? "Nhân vật thực hiện: " : "Đội trưởng điều khiển: ")
                    + (string.IsNullOrEmpty(who) ? "chưa có" : who);
                if (feedbackBinding != null && (feedbackKind != DashboardKind || !feedbackBinding.Matches(displayedDungeonBinding)))
                { dungeonFeedback.Text = ""; feedbackBinding = null; }
            }
            catch (Exception error)
            {
                ApplyCharacterHeader(CharacterHeader.Unavailable(null, true));
                displayedDungeonBinding = null;
                dungeonStart.Enabled = dungeonStop.Enabled = false;
                dungeonState.Text = "Chưa đọc được trạng thái";
                dungeonReason.Text = "Chờ nhận diện lại nhân vật / tổ đội. Loại lỗi: " + error.GetType().Name;
                dungeonController.Text = "Điều khiển: chưa xác nhận";
                dungeonFeedback.Text = ""; feedbackBinding = null;
            }
        }

        private void ExecuteDungeonDashboard(bool start)
        {
            try
            {
                var binding = displayedDungeonBinding;
                string result = dungeonAdapter.Execute(DashboardKind, ((DashboardChoice)dungeonOption.SelectedItem).Value, start, binding);
                RefreshDungeonDashboard();
                if (binding != null && binding.Matches(displayedDungeonBinding))
                { dungeonFeedback.Text = result; feedbackBinding = binding; feedbackKind = DashboardKind; }
                else dungeonFeedback.Text = "Ngữ cảnh đã thay đổi; xem lại nhân vật và đội trưởng.";
                AddLog("Bảng nhiệm vụ — " + DungeonDashboardPolicy.Name(DashboardKind) + ": " + result);
            }
            catch (Exception error)
            {
                RefreshDungeonDashboard();
                dungeonFeedback.Text = "Lỗi " + error.GetType().Name + "; kiểm tra trạng thái trước khi thử lại.";
                // Never log exception messages, native data, account IDs or credentials.
                AddLog("Bảng nhiệm vụ: lỗi " + error.GetType().Name + "; kiểm tra trạng thái engine.");
            }
        }
    }
}
