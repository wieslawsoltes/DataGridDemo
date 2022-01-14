﻿using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;

namespace DataGridDemo.Controls;

public class DataGridCellsPresenter : Control
{
    internal List<DataGridCell>? Cells { get; set; }

    internal DataGrid? DataGrid { get; set; }

    internal object? Content { get; set; }

    internal void CreateCells()
    {
        if (DataGrid?.Columns is { })
        {
            Cells = new List<DataGridCell>();

            foreach (var column in DataGrid.Columns)
            {
                var cell = new DataGridCell()
                {
                    Column = column,
                    Child = Content is { } ? column.CellTemplate?.Build(Content) : null,
                    DataGrid = DataGrid
                };

                Cells.Add(cell);
            }
        }

        if (Cells is { })
        {
            foreach (var cell in Cells)
            {
                if (cell.Parent is null)
                {
                    ((ISetLogicalParent)cell).SetParent(this);
                    VisualChildren.Add(cell);
                    LogicalChildren.Add(cell);
                }
            }
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Cells is { })
        {
            var width = 0.0;
            var height = 0.0;
            foreach (var cell in Cells)
            {
                cell.Measure(availableSize);
                width += cell.DesiredSize.Width;
                height = Math.Max(height, cell.DesiredSize.Height);
            }

            return new Size(width, height);
        }

        return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Cells is { } && DataGrid?.ColumnWidths is { })
        {
            var offset = 0.0;
            var height = 0.0;

            for (var c = 0; c < Cells.Count; c++)
            {
                var cell = Cells[c];
                var width = DataGrid.ColumnWidths[c];
                cell.Arrange(new Rect(offset, 0.0, width, cell.DesiredSize.Height));
                offset += width;
                height = Math.Max(height, cell.DesiredSize.Height);
            }

            return new Size(offset, height);
        }

        return base.ArrangeOverride(finalSize);
    }
}