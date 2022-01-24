using System;
using Avalonia;

namespace DataGridDemo.Controls.Layout;

internal class DataGridCellsLayout
{
    private readonly DataGridCellsPresenter _dataGridCellsPresenter;

    public DataGridCellsLayout(DataGridCellsPresenter dataGridCellsPresenter)
    {
        _dataGridCellsPresenter = dataGridCellsPresenter;
    }

    public Size MeasureCells(Size availableSize)
    {
        if (_dataGridCellsPresenter.Cells is null)
        {
            return availableSize;
        }

        var totalWidth = 0.0;
        var maxHeight = 0.0;

        foreach (var cell in _dataGridCellsPresenter.Cells)
        {
            cell.Measure(availableSize);
            totalWidth += cell.DesiredSize.Width;
            maxHeight = Math.Max(maxHeight, cell.DesiredSize.Height);
        }

        return new Size(totalWidth, maxHeight);
    }

    public Size ArrangeCells(Size finalSize)
    {
        if (_dataGridCellsPresenter.Cells is null)
        {
            return finalSize;
        }

        var totalWidth = 0.0;
        var maxHeight = 0.0;

        for (var c = 0; c < _dataGridCellsPresenter.Cells.Count; c++)
        {
            var cell = _dataGridCellsPresenter.Cells[c];
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
