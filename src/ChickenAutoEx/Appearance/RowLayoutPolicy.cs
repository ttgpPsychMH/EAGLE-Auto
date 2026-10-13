using System;
using System.Collections.Generic;
using System.Linq;

namespace TinhKiemAuto.Appearance
{
    internal sealed class RowCell
    {
        internal int Row, Column, Width, Height;
    }
    internal sealed class RowPlan
    {
        internal int[] X, Y, RowHeights;
        internal int Width, Height;
    }
    // Column constraints keep a caption before its input. A long cell can span
    // unused columns; it does not widen every other row's numeric-input column.
    internal static class RowLayoutPolicy
    {
        internal static RowPlan Arrange(IList<RowCell> cells, int columns, int rows, int padding, int top, int gap)
        {
            if (columns < 1 || rows < 1 || columns > 128 || rows > 128) throw new ArgumentOutOfRangeException();
            var x = Enumerable.Repeat(padding, columns).ToArray();
            var heights = new int[rows];
            for (int column = 0; column < columns; column++)
            {
                foreach (var row in cells.GroupBy(c => c.Row))
                {
                    var sorted = row.OrderBy(c => c.Column).ToArray();
                    for (int i = 0; i + 1 < sorted.Length; i++)
                        if (sorted[i].Column == column)
                            x[sorted[i + 1].Column] = Math.Max(x[sorted[i + 1].Column], x[column] + sorted[i].Width + gap);
                }
            }
            int width = padding;
            foreach (var cell in cells)
            {
                heights[cell.Row] = Math.Max(heights[cell.Row], cell.Height);
                width = Math.Max(width, x[cell.Column] + cell.Width + padding);
            }
            var y = new int[rows]; int height = top;
            for (int row = 0; row < rows; row++) { y[row] = height; height += heights[row] + gap; }
            return new RowPlan { X = x, Y = y, RowHeights = heights, Width = width, Height = height - gap + padding };
        }
    }
}
