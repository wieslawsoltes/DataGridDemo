using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;

namespace DataGridDemo.Controls.Layout;

internal class DataGridLayout
{
    private readonly DataGrid _dataGrid;

    public DataGridLayout(DataGrid dataGrid)
    {
        _dataGrid = dataGrid;
    }

    private bool HasStarColumn(IList<DataGridColumn> columns)
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

    private double GetTotalMeasureWidth(IList<DataGridColumn> columns)
    {
        var totalMeasureWidth = 0.0;

        for (var c = 0; c < columns.Count; c++)
        {
            var column = columns[c];

            totalMeasureWidth += column.MeasureWidth;
        }

        return totalMeasureWidth;
    }

    private void SetMeasureWidths(IList<DataGridColumn> columns, double finalWidth)
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

    private double GetTotalHeight(IList<DataGridRow> rows)
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

    public Size MeasureRows(Size availableSize)
    {
        if (_dataGrid.Rows is null || _dataGrid.Columns.Count <= 0)
        {
            return availableSize;
        }

        foreach (var row in _dataGrid.Rows)
        {
            row.Measure(availableSize);
        }

        var totalWidth = GetTotalMeasureWidth(_dataGrid.Columns);
        var totalHeight = GetTotalHeight(_dataGrid.Rows);
        var hasStarColumn = HasStarColumn(_dataGrid.Columns);

        return new Size(hasStarColumn ? 0 : totalWidth, totalHeight);
    }

    public Size ArrangeRows(Size finalSize)
    {
        if (_dataGrid.Rows is null || _dataGrid.Columns.Count <= 0)
        {
            return finalSize;
        }

        SetMeasureWidths(_dataGrid.Columns, finalSize.Width);

        var totalWidth = GetTotalMeasureWidth(_dataGrid.Columns);
        var totalHeight = 0.0;
        var maxWidth = 0.0;

        foreach (var row in _dataGrid.Rows)
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
