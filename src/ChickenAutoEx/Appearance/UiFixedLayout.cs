using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace TinhKiemAuto.Appearance
{
    // Only the main form's legacy settings containers are registered. Controls,
    // radio parents, tab indexes, values and handlers remain the same objects.
    internal static class UiFixedLayout
    {
        private sealed class Slot
        {
            internal Control Control;
            internal int Row, Column, Width, Height;
            internal bool Right, Stretch;
        }
        private sealed class State
        {
            internal Slot[] Slots;
            internal int Rows, Columns, Dpi;
            internal bool Applying;
        }
        private static readonly ConditionalWeakTable<Control, State> States = new ConditionalWeakTable<Control, State>();
        internal static bool Owns(Control control) { State state; return control != null && States.TryGetValue(control, out state); }
        internal static void RegisterTree(Control root)
        {
            if (root == null || root.Name == "eagleDungeonDashboard" || Equals(root.Tag, "compactGroup")) return;
            foreach (Control child in root.Controls) RegisterTree(child);
            if (!(root is GroupBox || root is TabPage || root.Name == "panel3")) return;
            var children = root.Controls.Cast<Control>().ToArray();
            if (children.Length == 0 || children.Any(c => c.Dock != DockStyle.None) || Owns(root)) return;
            var rows = Guides(children.Select(c => c.Top), 8);
            var columns = Guides(children.Select(c => c.Left), 7);
            var state = new State { Dpi = Math.Max(96, root.DeviceDpi), Rows = rows.Count, Columns = columns.Count,
                Slots = children.Select(c => new Slot { Control = c, Row = Closest(rows, c.Top), Column = Closest(columns, c.Left),
                    Width = c.Width, Height = c.Height, Right = (c.Anchor & AnchorStyles.Right) != 0,
                    Stretch = (c.Anchor & (AnchorStyles.Left | AnchorStyles.Right)) == (AnchorStyles.Left | AnchorStyles.Right) }).ToArray() };
            States.Add(root, state);
            foreach (var slot in state.Slots) slot.Control.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            root.Layout += (s, e) => Refresh(root);
            root.FontChanged += (s, e) => Refresh(root);
            foreach (var slot in state.Slots)
            {
                slot.Control.TextChanged += (s, e) => AppAppearance.Apply(root);
                slot.Control.FontChanged += (s, e) => AppAppearance.Apply(root);
            }
        }
        private static List<int> Guides(IEnumerable<int> coordinates, int tolerance)
        {
            var result = new List<int>();
            foreach (int value in coordinates.OrderBy(v => v))
                if (result.Count == 0 || value - result[result.Count - 1] > tolerance) result.Add(value);
            return result;
        }
        private static int Closest(IList<int> guides, int value)
        {
            int best = 0;
            for (int i = 1; i < guides.Count; i++) if (Math.Abs(guides[i] - value) < Math.Abs(guides[best] - value)) best = i;
            return best;
        }
        internal static void Refresh(Control parent)
        {
            State state;
            if (parent == null || parent.IsDisposed || !States.TryGetValue(parent, out state) || state.Applying) return;
            if (parent.IsHandleCreated && parent.InvokeRequired) { AppAppearance.Apply(parent); return; }
            state.Applying = true;
            try
            {
                float scale = Math.Max(96, parent.DeviceDpi) / (float)state.Dpi;
                int gap = Math.Max(6, (int)Math.Ceiling(8 * scale)), padding = gap + 4;
                int top = parent is GroupBox ? Math.Max(28, (int)Math.Ceiling(parent.Font.GetHeight(parent.DeviceDpi)) + gap) : padding;
                var cells = state.Slots.Select(slot => Measure(slot, scale)).ToArray();
                var plan = RowLayoutPolicy.Arrange(cells, state.Columns, state.Rows, padding, top, gap);
                // A right-aligned action column leaves the adjacent combobox room to stretch.
                for (int column = 0; column < state.Columns; column++)
                {
                    var fixedRight = state.Slots.Select((slot, i) => new { slot, i }).Where(v => v.slot.Column == column && v.slot.Right && !v.slot.Stretch).ToArray();
                    if (fixedRight.Length > 0 && parent.ClientSize.Width >= plan.Width)
                        plan.X[column] = Math.Max(plan.X[column], parent.ClientSize.Width - padding - fixedRight.Max(v => cells[v.i].Width));
                }
                var scroll = parent as ScrollableControl;
                Point origin = scroll == null ? Point.Empty : scroll.AutoScrollPosition;
                for (int i = 0; i < state.Slots.Length; i++)
                {
                    var slot = state.Slots[i]; var cell = cells[i];
                    if (slot.Control.IsDisposed || slot.Control.Parent != parent) continue;
                    int width = cell.Width;
                    int next = state.Slots.Where(s => s.Row == slot.Row && s.Column > slot.Column).Select(s => s.Column).DefaultIfEmpty(-1).Min();
                    if (slot.Stretch && next >= 0) width = Math.Max(width, plan.X[next] - plan.X[slot.Column] - gap);
                    if (slot.Control is GroupBox || slot.Control is TabControl)
                        width = Math.Max(width, parent.ClientSize.Width - padding * 2);
                    slot.Control.SetBounds(origin.X + plan.X[slot.Column], origin.Y + plan.Y[slot.Row], width, cell.Height);
                    var button = slot.Control as ButtonBase;
                    if (button != null) button.AutoEllipsis = false;
                }
                if (parent is GroupBox)
                {
                    parent.MinimumSize = new Size(plan.Width, plan.Height);
                    if (parent.Height != plan.Height) parent.Height = plan.Height;
                    var table = parent.Parent as TableLayoutPanel;
                    if (table != null)
                        table.MinimumSize = new Size(table.Controls.Cast<Control>().Max(c => c.MinimumSize.Width) + table.Padding.Horizontal, 0);
                }
                else if (scroll != null)
                {
                    scroll.AutoScroll = true;
                    scroll.AutoScrollMinSize = new Size(plan.Width, plan.Height);
                }
            }
            finally { state.Applying = false; }
        }
        private static RowCell Measure(Slot slot, float scale)
        {
            Control control = slot.Control;
            int width = (int)Math.Ceiling(slot.Width * scale), height = (int)Math.Ceiling(slot.Height * scale);
            if (control is ButtonBase || control is Label)
            {
                bool multiline = control is Label && control.Text.IndexOf('\n') >= 0;
                Size text = TextRenderer.MeasureText(control.Text, control.Font,
                    new Size(multiline ? Math.Max(60, width) : int.MaxValue, int.MaxValue),
                    TextFormatFlags.NoPrefix | (multiline ? TextFormatFlags.WordBreak : TextFormatFlags.SingleLine));
                int adornment = control is CheckBox || control is RadioButton ? 24 : control is Button ? 24 : 4;
                width = Math.Max(width, text.Width + (int)Math.Ceiling(adornment * scale));
                height = Math.Max(height, text.Height + (int)Math.Ceiling(6 * scale));
                if (control is Button) height = Math.Max(height, (int)Math.Ceiling(32 * scale));
                var label = control as Label;
                if (label != null) label.AutoSize = false;
            }
            else if (control is NumericUpDown || control is ComboBox || control is TextBoxBase)
            {
                width = Math.Max(width, (int)Math.Ceiling(60 * scale));
                height = Math.Max(height, (int)Math.Ceiling(control.Font.GetHeight(control.DeviceDpi)) + (int)Math.Ceiling(12 * scale));
            }
            else if (control is GroupBox || control is TabControl)
            {
                width = Math.Max(width, control.MinimumSize.Width);
                height = Math.Max(height, control.MinimumSize.Height);
            }
            return new RowCell { Row = slot.Row, Column = slot.Column, Width = width, Height = height };
        }
    }
}
