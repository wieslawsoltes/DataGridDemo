using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;

namespace DataGridDemo.Controls.Layout;

internal static class DataGridLayout
{
    private static bool HasStarColumn(IList<DataGridColumn> columns)
    {
        for (var c = 0; c < columns.Count; c++)
        {
            var column = columns[c];
            if (column.Width.GridUnitType == GridUnitType.Star)
            {
                return true;
            }
        }

        return false;
    }

    private static double GetColumnsTotalMeasureWidth(IList<DataGridColumn> columns)
    {
        var totalMeasureWidth = 0.0;

        for (var c = 0; c < columns.Count; c++)
        {
            var column = columns[c];

            totalMeasureWidth += column.MeasureWidth;
        }

        return totalMeasureWidth;
    }

    private static void SetColumnsFinalMeasureWidth(IList<DataGridColumn> columns, double finalWidth)
    {
        var totalStarSize = 0.0;
        var totalPixelSize = 0.0;

        for (var c = 0; c < columns.Count; c++)
        {
            var column = columns[c];

            switch (column.Width.GridUnitType)
            {
                case GridUnitType.Auto:
                    totalPixelSize += column.MeasureWidth;
                    break;
                case GridUnitType.Pixel:
                    column.MeasureWidth = column.Width.Value;
                    totalPixelSize += column.MeasureWidth;
                    break;
                case GridUnitType.Star:
                    totalStarSize += column.Width.Value;
                    break;
            }
        }

        var starColumnsWidth = Math.Max(0, finalWidth - totalPixelSize);

        for (var c = 0; c < columns.Count; c++)
        {
            var column = columns[c];

            switch (column.Width.GridUnitType)
            {
                case GridUnitType.Star:
                    var percentage = column.Width.Value / totalStarSize;
                    var width = starColumnsWidth * percentage;
                    column.MeasureWidth = width;
                    totalPixelSize += column.MeasureWidth;
                    break;
            }
        }
    }

    private static double GetTotalRowsHeight(IList<DataGridRow> rows)
    {
        var totalHeight = 0.0;

        for (var r = 0; r < rows.Count; r++)
        {
            var row = rows[r];
            var height = row.DesiredSize.Height;
            totalHeight += height;
        }

        return totalHeight;
    }

    public static Size MeasureRows(Size availableSize, IList<DataGridColumn>? columns, IList<DataGridRow>? rows)
    {
        if (columns is null || columns.Count <= 0 || rows is null)
        {
            return availableSize;
        }

        foreach (var row in rows)
        {
            row.Measure(availableSize);
        }

        var totalWidth = GetColumnsTotalMeasureWidth(columns);
        var totalHeight = GetTotalRowsHeight(rows);
        var hasStarColumn = HasStarColumn(columns);

        return new Size(hasStarColumn ? 0 : totalWidth, totalHeight);
    }

    public static Size ArrangeRows(Size finalSize, IList<DataGridColumn>? columns, IList<DataGridRow>? rows)
    {
        if (columns is null || columns.Count <= 0 || rows is null)
        {
            return finalSize;
        }

        var finalWidth = finalSize.Width;

        SetColumnsFinalMeasureWidth(columns, finalWidth);

        var totalWidth = GetColumnsTotalMeasureWidth(columns);
        var totalHeight = 0.0;
        var maxWidth = 0.0;

        foreach (var row in rows)
        {
            var height = row.DesiredSize.Height;
            var rect = new Rect(0.0, totalHeight, totalWidth, height);
            row.Arrange(rect);
            totalHeight += height;
            maxWidth = Math.Max(maxWidth, totalWidth);
        }

        return new Size(maxWidth, totalHeight);
    }
}
