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
        bool hasStarColumn = false;

        for (var c = 0; c < columns.Count; c++)
        {
            var column = columns[c];

            switch (column.Width.GridUnitType)
            {
                case GridUnitType.Star:
                    hasStarColumn = true;
                    break;
            }
        }

        return hasStarColumn;
    }

    private void MeasureColumnCells(DataGridColumn column, List<DataGridRow> rows)
    {
        int columnIndex = column.Index;

        for (var r = 0; r < rows.Count; r++)
        {
            var row = rows[r];
            if (row.CellsPresenter?.Cells is { })
            {
                var cell = row.CellsPresenter.Cells[columnIndex];
                var width = cell.DesiredSize.Width;
                column.MeasureWidth = Math.Max(column.MeasureWidth, width);
            }
        }
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

    private void CalculateColumnWidths(IList<DataGridColumn> columns, double finalWidth)
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

        // Debug.WriteLine($"starColumnsWidth='{starColumnsWidth}', finalWidth='{finalWidth}', totalPixelSize='{totalPixelSize}', totalStarSize='{totalStarSize}'");

        for (var c = 0; c < columns.Count; c++)
        {
            var column = columns[c];

            switch (column.Width.GridUnitType)
            {
                case GridUnitType.Star:
                    var percentage = column.Width.Value / totalStarSize;
                    var width = starColumnsWidth * percentage;
                    // Debug.WriteLine($"[{c}] width='{width}', percentage='{percentage}', finalWidth='{finalWidth}'");
                    column.MeasureWidth = width;
                    totalPixelSize += column.MeasureWidth;
                    break;
            }
        }
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

        var totalWidth = 0.0;
        var totalHeight = 0.0;

        for (var c = 0; c < _dataGrid.Columns.Count; c++)
        {
            var column = _dataGrid.Columns[c];

            MeasureColumnCells(column, _dataGrid.Rows);

            totalWidth += column.MeasureWidth;
        }

        for (var r = 0; r < _dataGrid.Rows.Count; r++)
        {
            var row = _dataGrid.Rows[r];
            var height = row.DesiredSize.Height;
            totalHeight += height;
        }

        var hasStarColumn = HasStarColumn(_dataGrid.Columns);

        var size = new Size(hasStarColumn ? 0 : totalWidth, totalHeight);
        // Debug.WriteLine($"[MeasureRows] size='{size}'");
        return size;
    }

    private Size ArrangeRows(List<DataGridRow> rows, double totalWidth)
    {
        var offset = 0.0;
        var finalSizeWidth = 0.0;

        foreach (var row in rows)
        {
            var rect = new Rect(
                0.0, 
                offset, 
                totalWidth, 
                row.DesiredSize.Height);

            row.Arrange(rect);

            offset += row.DesiredSize.Height;
            finalSizeWidth = Math.Max(finalSizeWidth, totalWidth);
        }

        return new Size(finalSizeWidth, offset);
    }

    public Size ArrangeRows(Size finalSize)
    {
        if (_dataGrid.Rows is null || _dataGrid.Columns.Count <= 0)
        {
            return finalSize;
        }

        CalculateColumnWidths(_dataGrid.Columns, finalSize.Width);

        var totalWidth = GetTotalMeasureWidth(_dataGrid.Columns);

        var size = ArrangeRows(_dataGrid.Rows, totalWidth);

        // Debug.WriteLine($"[ArrangeRows] size='{size}'");
        return size;
    }
}
