using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Forms;

namespace TinhKiemAuto.Diagnostics
{
    internal sealed class DiagnosticWindow
    {
        internal int Alias, Pid;
        internal bool[] Flags;
        internal int[] Phases;
        internal int[] AcTac;
    }
    internal sealed class DiagnosticsHost : IDisposable
    {
        internal const string ConsentText = "EAGLE Auto có thể tự ghi chẩn đoán để hỗ trợ tìm lỗi khi sử dụng.\r\n\r\n"
            + "Ghi: thời gian, phiên bản Auto, CPU/RAM/handles của Auto và cửa sổ game đã nhận, độ trễ giao diện, trạng thái tính năng/hook, nhóm lỗi và mã HRESULT.\r\n\r\n"
            + "Trong file chẩn đoán không ghi: tài khoản, mật khẩu, token, tên nhân vật, HWID, tọa độ, nội dung chat, gói tin, ảnh màn hình, nội dung exception hoặc đường dẫn trong stack trace.\r\n\r\n"
            + "File CSV chỉ lưu trên máy, tối đa 4 file × 2 MB; log chẩn đoán cũ có thể được thay thế. Không tự gửi lên mạng.\r\n\r\n"
            + "Ghi nhớ lựa chọn: nếu đồng ý, lần sau tự bật. Có thể tắt hoặc mở thư mục log trong Tiện ích → Chẩn đoán tự động. Anh có đồng ý không?";
        private readonly Form owner;
        private readonly Func<DiagnosticWindow[]> snapshot;
        private readonly string folder;
        private readonly ConsentController consent;
        private readonly System.Windows.Forms.Timer heartbeat;
        private System.Threading.Timer sampler;
        private readonly Stopwatch clock = Stopwatch.StartNew();
        private readonly object sampleLock = new object();
        private readonly int ownPid;
        private long lastBeat, completedLag;
        private long lastDropped, lastQueueDropped, lastErrorDropped;
        private int sampling;
        private bool disposed;
        private volatile DiagnosticWindow[] windows = new DiagnosticWindow[0];
        private readonly Dictionary<int, string> previousStates = new Dictionary<int, string>();
        private readonly Dictionary<int, string> previousAcTac = new Dictionary<int, string>();
        private readonly Dictionary<int, Tuple<long, double, double>> cpu = new Dictionary<int, Tuple<long, double, double>>();
        private ToolStripMenuItem status, enable, disable;
        internal DiagnosticsHost(Form owner, ToolStripMenuItem menu, Func<DiagnosticWindow[]> snapshot)
        {
            this.owner = owner; this.snapshot = snapshot;
            using (var process = Process.GetCurrentProcess()) ownPid = process.Id;
            folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EAGLE Auto", "Diagnostics");
            consent = new ConsentController(new ConsentSettings(folder), () => {
                var store = new RotatingDiagnosticStore(folder);
                return new DiagnosticRecorder(AppBranding.DisplayVersion.Substring(1), store);
            });
            var root = new ToolStripMenuItem("Chẩn đoán tự động");
            status = new ToolStripMenuItem("Đang tắt") { Enabled = false };
            enable = new ToolStripMenuItem("Bật ghi nhận…", null, (s, e) => { consent.Request(Ask); ApplyDecision(); });
            disable = new ToolStripMenuItem("Tắt ghi nhận", null, (s, e) => { Stop(); consent.Revoke(); RefreshMenu(); ReportSaveFailure(); });
            root.DropDownItems.AddRange(new ToolStripItem[] { status, enable, disable, new ToolStripSeparator(),
                new ToolStripMenuItem("Mở thư mục log", null, (s, e) => OpenFolder()),
                new ToolStripMenuItem("Dữ liệu được ghi", null, (s, e) => ShowInformation()) });
            menu.DropDownItems.Add(root); root.DropDownOpening += (s, e) => RefreshMenu();
            Appearance.AppAppearance.Apply(root.Owner);
            heartbeat = new System.Windows.Forms.Timer { Interval = 1000 };
            heartbeat.Tick += Beat;
            owner.Shown += OnShown;
            owner.FormClosed += (s, e) => Dispose();
            Application.ApplicationExit += OnExit;
            AppDomain.CurrentDomain.FirstChanceException += FirstChance;
            AppDomain.CurrentDomain.UnhandledException += Unhandled;
            // Previously accepted policy may start before FrmMain_Load; unknown policy waits for the dialog.
            if (consent.Decision == DiagnosticConsent.Accepted) { consent.Initialize(null); ApplyDecision(); }
            else RefreshMenu();
        }
        private void OnShown(object sender, EventArgs args)
        { if (consent.Decision == DiagnosticConsent.Unknown) { consent.Initialize(Ask); ApplyDecision(); } }
        private bool Ask()
        {
            using (var form = new Form { Text = "Cho phép chẩn đoán EAGLE Auto", StartPosition = FormStartPosition.CenterParent,
                ClientSize = new Size(560, 480), MinimizeBox = false, MaximizeBox = false, FormBorderStyle = FormBorderStyle.FixedDialog,
                AutoScaleMode = AutoScaleMode.Font })
            {
                var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(16), RowCount = 2, ColumnCount = 1 };
                layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
                layout.Controls.Add(new TextBox { Dock = DockStyle.Fill, Text = ConsentText, Multiline = true, ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical, BackColor = Color.White, WordWrap = true }, 0, 0);
                var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
                var no = new Button { Text = "Không", DialogResult = DialogResult.No, Width = 110, Height = 32 };
                var yes = new Button { Text = "Đồng ý", DialogResult = DialogResult.Yes, Width = 110, Height = 32 };
                buttons.Controls.Add(no); buttons.Controls.Add(yes); layout.Controls.Add(buttons, 0, 1); form.Controls.Add(layout);
                form.AcceptButton = no; form.CancelButton = no;
                Appearance.AppAppearance.Apply(form);
                return form.ShowDialog(owner) == DialogResult.Yes;
            }
        }
        private void ShowInformation()
        { MessageBox.Show(owner, ConsentText, "Dữ liệu chẩn đoán", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void ApplyDecision()
        {
            Stop(); var r = consent.Recorder;
            if (r != null && r.Enabled)
            {
                lock (sampleLock) { previousStates.Clear(); previousAcTac.Clear(); cpu.Clear(); windows = new DiagnosticWindow[0]; lastBeat = clock.ElapsedMilliseconds;
                    lastDropped = lastQueueDropped = lastErrorDropped = 0; }
                LocalDiagnostics.Attach(r, typeof(FrmMain).Assembly);
                r.Record(DiagnosticSchema.Row(DateTime.UtcNow, r.Version, DiagnosticEvent.SessionStarted, DiagnosticFeature.General));
                heartbeat.Start(); sampler = new System.Threading.Timer(Sample, null, 1000, 5000);
            }
            RefreshMenu(); ReportSaveFailure();
            if (consent.StartFailed) MessageBox.Show(owner, "Không bật được chẩn đoán. Auto vẫn hoạt động; kiểm tra quyền ghi thư mục người dùng.", AppBranding.Name);
        }
        private void ReportSaveFailure()
        {
            if (!consent.SaveSucceeded) MessageBox.Show(owner, "Không lưu được lựa chọn chẩn đoán cho lần mở sau. Lựa chọn hiện tại vẫn áp dụng trong phiên này.", AppBranding.Name);
        }
        private void RefreshMenu()
        {
            bool active = LocalDiagnostics.Enabled;
            status.Text = (consent.Recorder != null && consent.Recorder.Failed) || consent.StartFailed ? "Đã dừng: không ghi được file" : active ? "Đang ghi • chỉ lưu trên máy" : "Đang tắt";
            enable.Enabled = !active; disable.Enabled = active || consent.Decision == DiagnosticConsent.Accepted;
        }
        private void Beat(object sender, EventArgs args)
        {
            if (!LocalDiagnostics.Enabled) { heartbeat.Stop(); RefreshMenu(); return; }
            long now = clock.ElapsedMilliseconds;
            Interlocked.Exchange(ref completedLag, Math.Max(0, now - Interlocked.Exchange(ref lastBeat, now) - 1000));
            try { windows = snapshot(); } catch { windows = new DiagnosticWindow[0]; }
        }
        private void Sample(object ignored)
        {
            if (Interlocked.Exchange(ref sampling, 1) != 0) return;
            try
            {
                lock (sampleLock)
                {
                    var r = consent.Recorder;
                    if (disposed || r == null || !r.Enabled) return;
                    var current = windows;
                    long lag = Math.Max(Math.Max(0, clock.ElapsedMilliseconds - Interlocked.Read(ref lastBeat) - 1000), Interlocked.Exchange(ref completedLag, 0));
                    r.Record(DiagnosticSchema.Row(DateTime.UtcNow, r.Version, DiagnosticEvent.InterfaceSample,
                        DiagnosticFeature.Interface, uiDelay: lag, gameCount: current.Length));
                    long dropped = r.Dropped;
                    if (dropped != lastDropped) { lastDropped = dropped; r.Record(DiagnosticSchema.Row(DateTime.UtcNow, r.Version,
                        DiagnosticEvent.SamplesDropped, DiagnosticFeature.General, count: dropped)); }
                    long queueDropped = r.QueueDropped, errorDropped = r.ErrorDropped;
                    if (queueDropped != lastQueueDropped) { lastQueueDropped = queueDropped; r.Record(DiagnosticSchema.Row(DateTime.UtcNow, r.Version,
                        DiagnosticEvent.QueueSamplesDropped, DiagnosticFeature.General, count: queueDropped)); }
                    if (errorDropped != lastErrorDropped) { lastErrorDropped = errorDropped; r.Record(DiagnosticSchema.Row(DateTime.UtcNow, r.Version,
                        DiagnosticEvent.ErrorSamplesDropped, DiagnosticFeature.General, count: errorDropped)); }
                    SampleProcess(r, 0, ownPid);
                    var alive = new HashSet<int>();
                    foreach (var window in current)
                    {
                        alive.Add(window.Alias); SampleProcess(r, window.Alias, window.Pid);
                        SampleAcTac(r, window);
                        string state = string.Join("", window.Flags.Select(v => v ? "1" : "0")) + ":" + string.Join("/", window.Phases);
                        string previous;
                        if (previousStates.TryGetValue(window.Alias, out previous) && previous == state) continue;
                        previousStates[window.Alias] = state;
                        DiagnosticFeature[] features = { DiagnosticFeature.GameLoop, DiagnosticFeature.Combat, DiagnosticFeature.Skills,
                            DiagnosticFeature.Crafting, DiagnosticFeature.Pet, DiagnosticFeature.Loot, DiagnosticFeature.Recovery,
                            DiagnosticFeature.TrungAc, DiagnosticFeature.ThuyLao, DiagnosticFeature.KyCuoc, DiagnosticFeature.AcTac, DiagnosticFeature.AcBa, DiagnosticFeature.Hook };
                        for (int i = 0; i < features.Length; i++)
                            r.Record(DiagnosticSchema.Row(DateTime.UtcNow, r.Version, DiagnosticEvent.FeatureState, features[i],
                                window: window.Alias, state: window.Flags[i], phase: i >= 7 ? window.Phases[i - 7] : (int?)null, count: i));
                }
                foreach (int alias in previousStates.Keys.Where(key => !alive.Contains(key)).ToArray()) previousStates.Remove(alias);
                foreach (int alias in previousAcTac.Keys.Where(key => !alive.Contains(key)).ToArray()) previousAcTac.Remove(alias);
                foreach (int alias in cpu.Keys.Where(key => key != 0 && !alive.Contains(key)).ToArray()) cpu.Remove(alias);
                }
            }
            catch { /* Failure of diagnostics cannot control automation. */ }
            finally { Interlocked.Exchange(ref sampling, 0); }
        }
        private void SampleAcTac(DiagnosticRecorder r, DiagnosticWindow window)
        {
            int[] value = window.AcTac;
            if (value == null || value.Length != 8) return;
            string signature = string.Join("/", value), previous;
            if (previousAcTac.TryGetValue(window.Alias, out previous) && signature == previous) return;
            previousAcTac[window.Alias] = signature;
            var reason = (DiagnosticReason)value[7];
            r.Record(DiagnosticSchema.Row(DateTime.UtcNow, r.Version, DiagnosticEvent.TaskState, DiagnosticFeature.AcTac,
                window: window.Alias, state: value[1] != 0, phase: value[0], count: value[2], reason: reason));
            DiagnosticEvent[] codes = { DiagnosticEvent.TaskReady, DiagnosticEvent.TaskInside, DiagnosticEvent.TaskCombatHold, DiagnosticEvent.TaskProgress };
            for (int i = 0; i < codes.Length; i++)
                r.Record(DiagnosticSchema.Row(DateTime.UtcNow, r.Version, codes[i], DiagnosticFeature.AcTac,
                    window: window.Alias, count: value[i + 3], reason: reason));
        }
        private void SampleProcess(DiagnosticRecorder r, int alias, int pid)
        {
            try
            {
                using (var process = Process.GetProcessById(pid))
                {
                    process.Refresh(); long start = process.StartTime.ToUniversalTime().Ticks;
                    double value = process.TotalProcessorTime.TotalMilliseconds, now = clock.Elapsed.TotalSeconds;
                    Tuple<long, double, double> last; double? percent = null;
                    if (cpu.TryGetValue(alias, out last) && last.Item1 == start && now > last.Item3 && value >= last.Item2)
                        percent = (value - last.Item2) / ((now - last.Item3) * 1000 * Environment.ProcessorCount) * 100;
                    cpu[alias] = Tuple.Create(start, value, now);
                    r.Record(DiagnosticSchema.Row(DateTime.UtcNow, r.Version, DiagnosticEvent.ProcessSample, DiagnosticFeature.General,
                        window: alias, cpu: percent, working: process.WorkingSet64, privateBytes: process.PrivateMemorySize64, handles: process.HandleCount));
                }
            }
            catch (Exception error)
            { r.Record(DiagnosticSchema.Row(DateTime.UtcNow, r.Version, DiagnosticEvent.Warning, DiagnosticFeature.General, DiagnosticSchema.ErrorKind(error), error.HResult, alias)); }
        }
        private static void FirstChance(object sender, FirstChanceExceptionEventArgs e) { LocalDiagnostics.ObserveException(e.Exception); }
        private static void Unhandled(object sender, UnhandledExceptionEventArgs e) { LocalDiagnostics.ObserveException(e.ExceptionObject as Exception, true); }
        private void OpenFolder()
        {
            try
            {
                if (!Directory.Exists(folder)) { MessageBox.Show(owner, "Chưa có log chẩn đoán. Chọn Bật ghi nhận nếu anh muốn tạo log.", AppBranding.Name); return; }
                using (var opened = Process.Start(new ProcessStartInfo { FileName = folder, UseShellExecute = true })) { }
            }
            catch { MessageBox.Show(owner, "Không mở được thư mục log.", AppBranding.Name); }
        }
        private void Stop()
        {
            heartbeat.Stop(); var old = sampler; sampler = null; if (old != null) old.Dispose();
            LocalDiagnostics.Detach();
        }
        private void OnExit(object sender, EventArgs args) { Dispose(); }
        public void Dispose()
        {
            if (disposed) return; disposed = true; Stop(); consent.Dispose(); heartbeat.Dispose();
            owner.Shown -= OnShown; Application.ApplicationExit -= OnExit;
            AppDomain.CurrentDomain.FirstChanceException -= FirstChance;
            AppDomain.CurrentDomain.UnhandledException -= Unhandled;
        }
    }
}
