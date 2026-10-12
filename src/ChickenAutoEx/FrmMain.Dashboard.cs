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
            MinimumSize = new Size(820, 640);
            ClientSize = new Size(880, 760);
            BackColor = Color.FromArgb(241, 244, 248);

            // Keep all seven legacy tabs in their original order: handlers use indexes 5 and 6.
            TablControl.Multiline = true;
            foreach (TabPage page in TablControl.TabPages) page.AutoScroll = true;
            TabKyNangControl.Dock = DockStyle.Fill;
            txtlogs.BackColor = Color.White;
            txtlogs.Height = 100;
            paneltop.BackColor = panel2.BackColor = panel3.BackColor = Color.FromArgb(241, 244, 248);
            button1.Text = "Bật/tắt tất cả";
            button1.Size = new Size(112, 25);
            button1.BackColor = Color.FromArgb(209, 77, 69);
            toolTip1.SetToolTip(button1, "Bật/tắt Auto cho toàn bộ nhân vật (handler cũ).");
            buttrieutap.Size = new Size(112, 25);
            var scope = DashboardLabel("Điều khiển chung cho các cửa sổ Auto");
            scope.Location = new Point(250, 8);
            scope.Size = new Size(330, 28);
            scope.Dock = DockStyle.None;
            scope.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel3.Controls.Add(scope);

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            var body = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1,
                Padding = new Padding(6), Margin = Padding.Empty };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 246));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            var left = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3,
                BackColor = Color.White, Margin = new Padding(0, 0, 6, 0) };
            left.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            left.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            left.Controls.Add(DashboardLabel("NHÂN VẬT"), 0, 0);
            panellistview.Dock = DockStyle.Fill;
            panellistview.Margin = Padding.Empty;
            ListViewNhanVat.BackColor = Color.White;
            ListViewNhanVat.Resize += (sender, args) => txtNhanVat.Width = Math.Max(100, ListViewNhanVat.ClientSize.Width - 24);
            left.Controls.Add(panellistview, 0, 1);
            left.Controls.Add(DashboardLabel("Chọn dòng để cấu hình.\nTick ô để bật Auto của nhân vật."), 0, 2);

            var right = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3,
                Margin = Padding.Empty };
            right.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            right.RowStyles.Add(new RowStyle(SizeType.Absolute, 144));
            right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            dashboardSelection = DashboardLabel("EAGLE Auto — chọn nhân vật để cấu hình");
            right.Controls.Add(dashboardSelection, 0, 0);
            paneltop.Dock = DockStyle.Fill;
            paneltop.Margin = Padding.Empty;
            right.Controls.Add(paneltop, 0, 1);
            right.Controls.Add(TablControl, 0, 2);
            body.Controls.Add(left, 0, 0);
            body.Controls.Add(right, 1, 0);
            menuStrip1.Dock = DockStyle.Fill;
            menuStrip1.Margin = Padding.Empty;
            root.Controls.Add(menuStrip1, 0, 0);
            root.Controls.Add(body, 0, 1);
            txtlogs.Dock = DockStyle.Fill;
            root.Controls.Add(txtlogs, 0, 2);
            statusStrip1.Dock = DockStyle.Fill;
            root.Controls.Add(statusStrip1, 0, 3);
            // Empty old host only; never dispose the reused controls, bindings or components.
            Controls.Remove(panel1);
            Controls.Add(root);

            dungeonAdapter = new DungeonDashboardAdapter(new GameDungeonDashboardPort(
                DashboardSelectedGame, game => game != null && dicGame.ContainsValue(game)));
            CreateDungeonDashboardTab();
            // Owned by the existing component container; disposed with the form.
            var refresh = new Timer(components) { Interval = 500 };
            refresh.Tick += (sender, args) => {
                if (Visible && WindowState != FormWindowState.Minimized) RefreshDungeonDashboard();
            };
            ListViewNhanVat.SelectedIndexChanged += (sender, args) => RefreshDungeonDashboard();
            TablControl.SelectedIndexChanged += (sender, args) => RefreshDungeonDashboard();
            dashboardReady = true;
            refresh.Start();
            ResumeLayout(true);
        }

        private void CreateDungeonDashboardTab()
        {
            dungeonDashboardTab = new TabPage("Nhiệm Vụ – Phó Bản") { AutoScroll = true,
                BackColor = Color.White, Font = new Font("Segoe UI", 9f), Name = "eagleDungeonDashboard" };
            var table = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true,
                ColumnCount = 1, RowCount = 9, Padding = new Padding(10), MinimumSize = new Size(440, 0) };
            foreach (int height in new[] { 28, 32, 32, 42, 32, 58, 40, 40, 54 })
                table.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
            table.Controls.Add(DashboardLabel("NHIỆM VỤ – PHÓ BẢN"), 0, 0);
            dungeonTask = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "dungeonTask", AccessibleName = "Chọn nhiệm vụ phó bản" };
            foreach (DungeonKind kind in Enum.GetValues(typeof(DungeonKind)))
                dungeonTask.Items.Add(new DashboardChoice(DungeonDashboardPolicy.Name(kind), (int)kind));
            table.Controls.Add(dungeonTask, 0, 1);
            dungeonOption = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList,
                Name = "dungeonOption", AccessibleName = "Map hoặc môn phái" };
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
            dungeonStart = new Button { Text = "Bắt đầu", Size = new Size(132, 32), Name = "dungeonStart",
                BackColor = Color.FromArgb(31, 105, 181), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            dungeonStop = new Button { Text = "Dừng nhiệm vụ", Size = new Size(148, 32), Name = "dungeonStop" };
            buttons.Controls.Add(dungeonStart); buttons.Controls.Add(dungeonStop);
            table.Controls.Add(buttons, 0, 6);
            table.Controls.Add(dungeonFeedback, 0, 7);
            table.Controls.Add(DashboardLabel("Bật Auto bằng ô tick trong danh sách trước khi bắt đầu.\nCác điều kiện quyền/VIP do engine 107 kiểm tra như cũ.\nCác nhiệm vụ khác vẫn nằm trong menu cũ."), 0, 8);
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
                Game actor = DashboardSelectedGame();
                dashboardSelection.Text = actor == null || actor.TLBB == null ? "EAGLE Auto — chọn nhân vật để cấu hình"
                    : "Đang chọn: " + actor.TLBB.Name;
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
                dashboardSelection.Text = "EAGLE Auto — chờ nhận diện lại nhân vật";
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
