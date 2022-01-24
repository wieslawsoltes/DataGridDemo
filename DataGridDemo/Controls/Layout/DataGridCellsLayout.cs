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

        var width = 0.0;
        var height = 0.0;
        foreach (var cell in _dataGridCellsPresenter.Cells)
        {
            cell.Measure(availableSize);
            width += cell.DesiredSize.Width;
            height = Math.Max(height, cell.DesiredSize.Height);
        }

        return new Size(width, height);
    }

    public Size ArrangeCells(Size finalSize)
    {
        if (_dataGridCellsPresenter.Cells is null)
        {
            return finalSize;
        }

        var offset = 0.0;
        var height = 0.0;

        for (var c = 0; c < _dataGridCellsPresenter.Cells.Count; c++)
        {
            var cell = _dataGridCellsPresenter.Cells[c];
            var column = cell.Column;
            if (column is null)
            {
                continue;
            }
            var width = column.MeasureWidth;
            cell.Arrange(new Rect(offset, 0.0, width, cell.DesiredSize.Height));
            offset += width;
            height = Math.Max(height, cell.DesiredSize.Height);
        }

        return new Size(offset, height);
    }
}
