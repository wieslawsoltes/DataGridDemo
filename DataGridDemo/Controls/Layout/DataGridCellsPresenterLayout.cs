using System;
using System.Collections.Generic;
using Avalonia;

namespace DataGridDemo.Controls.Layout;

internal static class DataGridCellsPresenterLayout
{
    public static Size MeasureCells(Size availableSize, IList<DataGridCell>? cells)
    {
        if (cells is null)
        {
            return availableSize;
        }

        var totalWidth = 0.0;
        var maxHeight = 0.0;

        foreach (var cell in cells)
        {
            cell.Measure(availableSize);
            totalWidth += cell.DesiredSize.Width;
            maxHeight = Math.Max(maxHeight, cell.DesiredSize.Height);
        }

        return new Size(totalWidth, maxHeight);
    }

    public static Size ArrangeCells(Size finalSize, IList<DataGridCell>? cells)
    {
        if (cells is null)
        {
            return finalSize;
        }

        var totalWidth = 0.0;
        var maxHeight = 0.0;

        for (var c = 0; c < cells.Count; c++)
        {
            var cell = cells[c];
            var column = cell.Column;
            if (column is null)
            {
                continue;
            }
            var width = column.MeasureWidth;
            var height = cell.DesiredSize.Height;
            cell.Arrange(new Rect(totalWidth, 0.0, width, height));
            totalWidth += width;
            maxHeight = Math.Max(maxHeight, height);
        }

        return new Size(totalWidth, maxHeight);
    }
}
