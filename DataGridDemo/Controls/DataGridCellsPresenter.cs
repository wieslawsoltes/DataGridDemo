using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using DataGridDemo.Controls.Layout;

namespace DataGridDemo.Controls;

public class DataGridCellsPresenter : Control
{
    internal List<DataGridCell>? Cells { get; set; }

    internal DataGrid? DataGrid { get; set; }

    internal object? Content { get; set; }

    internal void GenerateCells()
    {
        if (DataGrid?.Columns is null)
        {
            return;
        }

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

        foreach (var cell in Cells)
        {
            if (cell.Parent is not null)
            {
                continue;
            }

            ((ISetLogicalParent)cell).SetParent(this);
            VisualChildren.Add(cell);
            LogicalChildren.Add(cell);
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return DataGridCellsLayout.Measure(availableSize, Cells);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return DataGridCellsLayout.Arrange(finalSize, Cells);
    }
}
