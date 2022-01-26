using System;
using Avalonia;
using Avalonia.Controls;

namespace DataGridDemo.Controls.Layout;

internal static class DataGridCellLayout
{
    public static Size Measure(IControl child, DataGridColumn column, Size availableSize)
    {
        child.Measure(availableSize);

        var width = child.DesiredSize.Width;

        column.MeasureWidth = Math.Max(column.MeasureWidth, width);

        return child.DesiredSize;
    }

    public static Size Arrange(IControl child, DataGridColumn column, Size finalSize)
    {
        var width = column.MeasureWidth;
        var height = child.DesiredSize.Height;
        var rect = new Rect(0, 0, width, height);
        child.Arrange(rect);
        return rect.Size;
    }
}
