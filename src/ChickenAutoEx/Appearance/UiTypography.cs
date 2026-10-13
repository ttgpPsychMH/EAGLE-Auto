using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace TinhKiemAuto.Appearance
{
    internal static class UiTypography
    {
        private sealed class State { internal bool Hooked, Updating; }
        private static readonly ConditionalWeakTable<Control, State> Controls = new ConditionalWeakTable<Control, State>();
        private static readonly ConditionalWeakTable<ToolStripItem, State> Items = new ConditionalWeakTable<ToolStripItem, State>();
        internal static void Apply(Control control)
        {
            if (control == null || control.IsDisposed) return;
            var tabs = control as TabControl;
            if (tabs != null)
            {
                // Header uses TabControl.Font; page fonts keep content regular.
                foreach (TabPage page in tabs.TabPages)
                    page.Font = AppAppearance.GetFont(10f, FontStyle.Regular);
                tabs.Font = AppAppearance.GetFont(11f, FontStyle.Bold);
                return;
            }
            if (control is TextBoxBase || control is ComboBox || control is ListControl) return;
            var list = control as ListView;
            if (list != null)
            {
                foreach (ColumnHeader column in list.Columns)
                {
                    string caption;
                    if (UiCopyPolicy.TryFormat(column.Text, CopyRole.Body, out caption)) column.Text = caption;
                }
                return; // Never change row/subitem names or values.
            }
            var state = Controls.GetValue(control, key => new State());
            if (state.Updating) return;
            if (!state.Hooked) { state.Hooked = true; control.TextChanged += (s, e) => AppAppearance.Apply(control); }
            CopyRole role = control is TabPage || control.Name == "sectionCharacters" || control.Name == "sectionDungeons" || control.Name == "sectionSharedControls" ? CopyRole.Heading1
                : control is GroupBox || control is TabPage || control is Form ? CopyRole.Heading2 : CopyRole.Body;
            // Labels may contain arbitrary character/NPC/skill values. Only audited static labels are eligible.
            var label = control as Label;
            if (label != null && role != CopyRole.Heading1)
            {
                Form form = control.FindForm();
                if (form == null || !UiCopyCatalog.StaticLabels.Contains(form.GetType().Name + ":" + control.Name)) return;
            }
            string rendered;
            if (!UiCopyPolicy.TryFormat(control.Text, role, out rendered)) return;
            state.Updating = true;
            try
            {
                if (control.Text != rendered) control.Text = rendered;
                float minimum = role == CopyRole.Heading1 ? 11f : role == CopyRole.Heading2 ? 10.5f : 10f;
                FontStyle style = control.Font.Style;
                if (role == CopyRole.Heading1) style |= FontStyle.Bold;
                if (!(control is TabPage))
                    control.Font = AppAppearance.GetFont(Math.Max(control.Font.SizeInPoints, minimum), style);
                EnsureRoom(control);
                UiFixedLayout.Refresh(control.Parent);
            }
            finally { state.Updating = false; }
        }
        internal static void Apply(ToolStripItem item)
        {
            if (item == null || item.IsDisposed || item is ToolStripTextBox || item is ToolStripComboBox || item is ToolStripStatusLabel) return;
            var state = Items.GetValue(item, key => new State());
            if (state.Updating) return;
            if (!state.Hooked)
            {
                state.Hooked = true;
                item.TextChanged += (s, e) => {
                    if (item.Owner != null) AppAppearance.Apply(item.Owner);
                    else Apply(item); // An unowned item is not displayed yet.
                };
            }
            var menu = item as ToolStripMenuItem;
            CopyRole role = item.Name == "sectionSharedControls" ? CopyRole.Heading1 : menu != null && (menu.HasDropDownItems || menu.Owner is MenuStrip) ? CopyRole.Heading2 : CopyRole.Body;
            string rendered;
            if (!UiCopyPolicy.TryFormat(item.Text, role, out rendered)) return;
            state.Updating = true;
            try {
                if (item.Text != rendered) item.Text = rendered;
                item.Font = AppAppearance.GetFont(Math.Max(item.Font.SizeInPoints, role == CopyRole.Heading1 ? 11f : role == CopyRole.Heading2 ? 10.5f : 10f),
                    role == CopyRole.Heading1 ? item.Font.Style | FontStyle.Bold : item.Font.Style);
            } finally { state.Updating = false; }
        }
        private static void EnsureRoom(Control control)
        {
            if (UiFixedLayout.Owns(control.Parent)) return;
            if (!(control is ButtonBase || control is Label) || control.Parent == null || control.Dock != DockStyle.None) return;
            Size text = TextRenderer.MeasureText(control.Text, control.Font, new Size(int.MaxValue, int.MaxValue),
                TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
            int width = text.Width + (control is CheckBox || control is RadioButton ? 24 : control is Button ? 18 : 2);
            int height = text.Height + (control is ButtonBase ? 6 : 2);
            int right = control.Parent.ClientSize.Width - control.Parent.Padding.Right;
            int bottom = control.Parent.ClientSize.Height - control.Parent.Padding.Bottom;
            foreach (Control sibling in control.Parent.Controls)
            {
                if (ReferenceEquals(control, sibling)) continue;
                if (sibling.Left > control.Left && sibling.Top < control.Bottom && sibling.Bottom > control.Top) right = Math.Min(right, sibling.Left - 4);
                if (sibling.Top >= control.Bottom && sibling.Left < control.Right && sibling.Right > control.Left) bottom = Math.Min(bottom, sibling.Top - 2);
            }
            // Never expand through a neighbouring input/button or change parents, values or selection.
            control.Width = Math.Max(control.Width, Math.Min(width, Math.Max(0, right - control.Left)));
            control.Height = Math.Max(control.Height, Math.Min(height, Math.Max(0, bottom - control.Top)));
            var label = control as Label;
            if (label != null && !label.AutoSize) label.AutoEllipsis = true;
            var button = control as ButtonBase;
            if (button != null && !button.AutoSize) button.AutoEllipsis = width > control.Width;
        }
    }
}
