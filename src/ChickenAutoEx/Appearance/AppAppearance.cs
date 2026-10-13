using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TinhKiemAuto.Appearance
{
    // Presentation resources belong to this process only. Nothing is installed
    // in Windows and no font binary is embedded or distributed by the project.
    internal static class AppAppearance
    {
        private const int MaximumFontBytes = 4 * 1024 * 1024;
        private const string IconResourceName = "EagleAuto.Branding.Icon";
        private const string LogoResourceName = "EagleAuto.Branding.Logo";
        private static readonly object ResourceLock = new object();
        private static readonly ConditionalWeakTable<Control, ControlState> Controls = new ConditionalWeakTable<Control, ControlState>();
        private static readonly ConditionalWeakTable<ToolStripItem, ItemState> Items = new ConditionalWeakTable<ToolStripItem, ItemState>();
        private static readonly Dictionary<FontKey, Font> Fonts = new Dictionary<FontKey, Font>();
        private static readonly List<MemoryFont> MemoryFonts = new List<MemoryFont>();
        private static readonly HashSet<int> PrivateFaces = new HashSet<int>();
        private static PrivateFontCollection privateCollection;
        private static FontFamily privateFamily;
        private static FontFamily installedFamily;
        private static FontFamily fallbackFamily;
        private static Icon applicationIcon;
        private static Image logo;
        private static bool initialized;
        private static bool disposed;
        private static bool styleFallbackUsed;
        private static bool selectedFontDirectory;

        internal static string FontStatus
        {
            get
            {
                EnsureInitialized();
                lock (ResourceLock)
                {
                    if (privateFamily != null && PrivateFaces.Contains(0))
                        return "Aptos: " + PrivateFaces.Count + "/4 kiểu đã nạp "
                            + (selectedFontDirectory ? "từ thư mục đã chọn." : "từ thư mục Fonts.")
                            + (styleFallbackUsed ? " Một số kiểu chữ dùng Segoe UI." : "");
                    if (installedFamily != null)
                        return styleFallbackUsed ? "Aptos có trên Windows; một số kiểu chữ dùng Segoe UI." : "Aptos có sẵn trên Windows.";
                    return "Segoe UI dự phòng; chưa nạp được Aptos.";
                }
            }
        }

        internal static Icon Icon
        {
            get { EnsureInitialized(); return applicationIcon; }
        }

        internal static Image Logo
        {
            get { EnsureInitialized(); return logo; }
        }

        // Call after InitializeComponent on the owning UI thread. Added controls
        // and menu items subsequently inherit the same rule automatically.
        internal static void Apply(Control root)
        {
            if (root == null || root.IsDisposed) return;
            if (root.IsHandleCreated && root.InvokeRequired)
            {
                try { root.BeginInvoke(new Action<Control>(Apply), root); }
                catch (InvalidOperationException) { }
                return;
            }

            EnsureInitialized();
            ApplyControl(root);
        }

        internal static void ShowAbout(IWin32Window owner)
        {
            using (var form = new Form())
            {
                form.Text = AppBranding.AboutTitle;
                form.ClientSize = new Size(520, 320);
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.ShowInTaskbar = false;
                form.StartPosition = FormStartPosition.CenterParent;
                form.BackColor = Color.White;
                form.Padding = new Padding(18);
                form.Font = GetFont(FontStylePolicy.DefaultPointSize, FontStyle.Regular);

                var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 4 };
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
                layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

                var picture = new PictureBox { Image = Logo, SizeMode = PictureBoxSizeMode.Zoom, Dock = DockStyle.Fill, Margin = new Padding(0, 0, 14, 8) };
                var title = new Label { Text = AppBranding.WindowTitle, AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = GetFont(14f, FontStyle.Bold) };
                var attribution = new Label { Text = AppBranding.AboutText, AutoSize = false, Dock = DockStyle.Fill, Margin = new Padding(0, 4, 0, 4) };
                var status = new Label { Text = FontStatus, AutoSize = false, Dock = DockStyle.Fill, ForeColor = Color.DimGray, Margin = Padding.Empty };
                var close = new Button { Text = "Đóng", AutoSize = true, DialogResult = DialogResult.OK, Anchor = AnchorStyles.Right | AnchorStyles.Bottom, Margin = Padding.Empty };
                layout.Controls.Add(picture, 0, 0);
                layout.Controls.Add(title, 1, 0);
                layout.Controls.Add(attribution, 0, 1);
                layout.SetColumnSpan(attribution, 2);
                layout.Controls.Add(status, 0, 2);
                layout.SetColumnSpan(status, 2);
                var chooseFont = new Button { Text = "Chọn font…", AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Bottom, Margin = Padding.Empty };
                chooseFont.Click += (sender, args) => ChooseAptosFolder(form);
                layout.Controls.Add(chooseFont, 0, 3);
                layout.Controls.Add(close, 1, 3);
                form.Controls.Add(layout);
                form.AcceptButton = close;
                form.CancelButton = close;
                Apply(form);
                form.ShowDialog(owner);
            }
        }

        private static void ChooseAptosFolder(IWin32Window owner)
        {
            using (var picker = new FolderBrowserDialog { Description = "Chọn thư mục chứa bốn kiểu Aptos: Regular, Bold, Italic và Bold Italic.", ShowNewFolderButton = false })
            {
                if (picker.ShowDialog(owner) != DialogResult.OK) return;
                if (!FontDirectorySettings.HasFourFaces(picker.SelectedPath))
                {
                    MessageBox.Show(owner, "Chưa tìm thấy đủ bốn tệp Aptos TTF hợp lệ. Hãy giải nén bộ font rồi chọn thư mục chứa các tệp.", "Chọn Font Aptos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                bool saved = FontDirectorySettings.Save(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), picker.SelectedPath);
                MessageBox.Show(owner, saved ? "Đã lưu thư mục font riêng cho Auto. Đóng và mở lại Auto khi thuận tiện để nạp Aptos; không cần cài font vào Windows."
                    : "Không lưu được lựa chọn font. Bạn có thể chép bốn tệp vào thư mục Fonts cạnh EXE.", "Chọn Font Aptos", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static void ApplyControl(Control control)
        {
            if (control == null || control.IsDisposed) return;
            ControlState state = Controls.GetValue(control, key => new ControlState());
            if (!state.Hooked)
            {
                state.Hooked = true;
                control.ControlAdded += OnControlAdded;
                control.FontChanged += OnControlFontChanged;
                control.ContextMenuStripChanged += OnContextMenuStripChanged;
            }

            ApplyControlFont(control, state);
            UiTypography.Apply(control);
            var form = control as Form;
            if (form != null) form.Icon = Icon; // Preserve ShowIcon/ShowInTaskbar decisions.

            var strip = control as ToolStrip;
            if (strip != null)
            {
                if (!state.StripHooked)
                {
                    state.StripHooked = true;
                    strip.ItemAdded += OnItemAdded;
                }
                foreach (ToolStripItem item in strip.Items) ApplyItem(item);
            }

            if (control.ContextMenuStrip != null) ApplyControl(control.ContextMenuStrip);
            foreach (Control child in control.Controls) ApplyControl(child);
            UiFixedLayout.Refresh(control);
        }

        private static void ApplyControlFont(Control control, ControlState state)
        {
            if (state.Updating || control.IsDisposed) return;
            state.Updating = true;
            try
            {
                Font existing = control.Font;
                Font replacement = GetFont(existing.SizeInPoints, existing.Style);
                if (!ReferenceEquals(existing, replacement)) control.Font = replacement;
            }
            finally { state.Updating = false; }
        }

        private static void ApplyItem(ToolStripItem item)
        {
            if (item == null || item.IsDisposed) return;
            ItemState state = Items.GetValue(item, key => new ItemState());
            Font existing = item.Font;
            Font replacement = GetFont(existing.SizeInPoints, existing.Style);
            if (!ReferenceEquals(existing, replacement)) item.Font = replacement;
            UiTypography.Apply(item);
            var dropDown = item as ToolStripDropDownItem;
            if (dropDown == null) return;
            if (!state.Hooked)
            {
                state.Hooked = true;
                dropDown.DropDownOpening += OnDropDownOpening;
            }
            ApplyControl(dropDown.DropDown);
        }

        private static void OnControlAdded(object sender, ControlEventArgs e) { Apply(e.Control); }
        private static void OnControlFontChanged(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            var state = Controls.GetValue(control, key => new ControlState());
            if (!state.Updating) Apply(control); // Includes explicit ToolStripItem fonts, not only the strip itself.
        }
        private static void OnContextMenuStripChanged(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control != null && control.ContextMenuStrip != null) Apply(control.ContextMenuStrip);
        }
        private static void OnItemAdded(object sender, ToolStripItemEventArgs e) { ApplyItem(e.Item); }
        private static void OnDropDownOpening(object sender, EventArgs e)
        {
            var item = sender as ToolStripDropDownItem;
            if (item != null) ApplyItem(item); // Reapply an explicit item font before showing its menu.
        }

        internal static Font GetFont(float sizeInPoints, FontStyle style)
        {
            EnsureInitialized();
            float size = FontStylePolicy.PreservePointSize(sizeInPoints);
            style = (FontStyle)FontStylePolicy.PreserveStyleBits((int)style);
            int face = FontStylePolicy.FaceStyleBits((int)style);
            lock (ResourceLock)
            {
                bool privateAvailable = privateFamily != null && PrivateFaces.Contains(face) && privateFamily.IsStyleAvailable((FontStyle)face);
                bool installedAvailable = installedFamily != null && installedFamily.IsStyleAvailable((FontStyle)face);
                FontSource source = FontStylePolicy.ResolveSource(privateAvailable, installedAvailable);
                FontFamily family = source == FontSource.PrivateAptos ? privateFamily
                    : source == FontSource.InstalledAptos ? installedFamily : fallbackFamily;
                if (source == FontSource.SegoeUi)
                {
                    family = fallbackFamily;
                    if (privateFamily != null || installedFamily != null) styleFallbackUsed = true;
                }

                var key = new FontKey(family, size, style);
                Font font;
                if (Fonts.TryGetValue(key, out font)) return font;
                try { font = new Font(family, size, style, GraphicsUnit.Point); }
                catch (ArgumentException)
                {
                    styleFallbackUsed = true;
                    key = new FontKey(fallbackFamily, size, style);
                    if (Fonts.TryGetValue(key, out font)) return font;
                    font = new Font(fallbackFamily, size, style, GraphicsUnit.Point);
                }
                Fonts.Add(key, font);
                return font;
            }
        }

        private static void EnsureInitialized()
        {
            lock (ResourceLock)
            {
                if (initialized || disposed) return;
                initialized = true;
                Application.ApplicationExit += OnApplicationExit;
                installedFamily = FindInstalledAptos();
                fallbackFamily = new FontFamily(FontStylePolicy.FallbackFamily);
                LoadPrivateFonts();
                applicationIcon = LoadIcon();
                logo = LoadLogo();
            }
        }

        private static FontFamily FindInstalledAptos()
        {
            try
            {
                using (var collection = new InstalledFontCollection())
                {
                    FontFamily found = null;
                    foreach (FontFamily family in collection.Families)
                    {
                        if (found == null && FontStylePolicy.IsPreferredFamily(family.Name)) found = family;
                        else family.Dispose();
                    }
                    return found;
                }
            }
            catch (Exception) { return null; }
        }

        private static void LoadPrivateFonts()
        {
            try
            {
                string directory = FontDirectorySettings.Read(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
                selectedFontDirectory = directory != null;
                if (directory == null) directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts");
                if (!Directory.Exists(directory)) return;
                privateCollection = new PrivateFontCollection();
                LoadFace(directory, 0, "Aptos.ttf");
                LoadFace(directory, 1, "Aptos-Bold.ttf", "AptosBold.ttf");
                LoadFace(directory, 2, "Aptos-Italic.ttf", "AptosItalic.ttf");
                LoadFace(directory, 3, "Aptos-Bold-Italic.ttf", "Aptos-BoldItalic.ttf", "AptosBoldItalic.ttf");
                foreach (FontFamily family in privateCollection.Families)
                {
                    if (privateFamily == null && FontStylePolicy.IsPreferredFamily(family.Name)) privateFamily = family;
                    else family.Dispose();
                }
                if (privateFamily != null)
                    PrivateFaces.RemoveWhere(face => !privateFamily.IsStyleAvailable((FontStyle)face));
                // Do not treat a collection containing only another Aptos variant
                // or incomplete registration as the requested body-text family.
                if (privateFamily == null || !PrivateFaces.Contains(0))
                {
                    if (privateFamily != null) privateFamily.Dispose();
                    privateFamily = null;
                    PrivateFaces.Clear();
                }
            }
            catch (Exception) { PrivateFaces.Clear(); }
        }

        private static void LoadFace(string directory, int face, params string[] names)
        {
            foreach (string name in names)
            {
                IntPtr memory = IntPtr.Zero;
                IntPtr handle = IntPtr.Zero;
                try
                {
                    string path = Path.Combine(directory, name);
                    if (!File.Exists(path)) continue;
                    byte[] bytes = ReadBoundedFont(path);
                    if (bytes == null) continue;
                    memory = Marshal.AllocHGlobal(bytes.Length);
                    Marshal.Copy(bytes, 0, memory, bytes.Length);
                    uint count = 0;
                    handle = AddFontMemResourceEx(memory, (uint)bytes.Length, IntPtr.Zero, ref count);
                    if (handle == IntPtr.Zero || count == 0) continue;
                    privateCollection.AddMemoryFont(memory, bytes.Length);
                    MemoryFonts.Add(new MemoryFont(memory, handle));
                    memory = IntPtr.Zero;
                    handle = IntPtr.Zero;
                    PrivateFaces.Add(face);
                    return;
                }
                catch (Exception) { /* A failed face falls back without exposing paths or exception text. */ }
                finally
                {
                    if (handle != IntPtr.Zero) RemoveFontMemResourceEx(handle);
                    if (memory != IntPtr.Zero) Marshal.FreeHGlobal(memory);
                }
            }
        }

        private static byte[] ReadBoundedFont(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                long length = stream.Length;
                if (length <= 0 || length > MaximumFontBytes) return null;
                var bytes = new byte[(int)length];
                int offset = 0;
                while (offset < bytes.Length)
                {
                    int read = stream.Read(bytes, offset, bytes.Length - offset);
                    if (read == 0) return null;
                    offset += read;
                }
                return stream.ReadByte() == -1 ? bytes : null;
            }
        }

        private static Icon LoadIcon()
        {
            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(IconResourceName))
                {
                    if (stream != null)
                        using (var icon = new Icon(stream)) return (Icon)icon.Clone();
                }
            }
            catch (Exception) { }
            return (Icon)SystemIcons.Application.Clone();
        }

        private static Image LoadLogo()
        {
            try
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(LogoResourceName))
                {
                    if (stream != null)
                        using (Image source = Image.FromStream(stream)) return new Bitmap(source);
                }
            }
            catch (Exception) { }
            return null;
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            lock (ResourceLock)
            {
                if (disposed) return;
                disposed = true;
                Application.ApplicationExit -= OnApplicationExit;
                foreach (Font font in Fonts.Values) font.Dispose();
                Fonts.Clear();
                if (applicationIcon != null) applicationIcon.Dispose();
                if (logo != null) logo.Dispose();
                if (privateFamily != null) privateFamily.Dispose();
                if (installedFamily != null) installedFamily.Dispose();
                if (fallbackFamily != null) fallbackFamily.Dispose();
                if (privateCollection != null) privateCollection.Dispose();
                foreach (MemoryFont font in MemoryFonts)
                {
                    RemoveFontMemResourceEx(font.Handle);
                    Marshal.FreeHGlobal(font.Memory);
                }
                MemoryFonts.Clear();
            }
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr AddFontMemResourceEx(IntPtr font, uint length, IntPtr reserved, ref uint fontCount);

        [DllImport("gdi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RemoveFontMemResourceEx(IntPtr handle);

        private sealed class ControlState { internal bool Hooked; internal bool StripHooked; internal bool Updating; }
        private sealed class ItemState { internal bool Hooked; }
        private sealed class MemoryFont
        {
            internal readonly IntPtr Memory;
            internal readonly IntPtr Handle;
            internal MemoryFont(IntPtr memory, IntPtr handle) { Memory = memory; Handle = handle; }
        }

        private struct FontKey : IEquatable<FontKey>
        {
            private readonly FontFamily family;
            private readonly float size;
            private readonly FontStyle style;
            internal FontKey(FontFamily family, float size, FontStyle style) { this.family = family; this.size = size; this.style = style; }
            public bool Equals(FontKey other) { return ReferenceEquals(family, other.family) && size.Equals(other.size) && style == other.style; }
            public override bool Equals(object other) { return other is FontKey && Equals((FontKey)other); }
            public override int GetHashCode() { return RuntimeHelpers.GetHashCode(family) ^ size.GetHashCode() ^ (int)style; }
        }
    }
}
