using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using DataGridDemo.Controls.Layout;

namespace DataGridDemo.Controls;

public class DataBoxCellsPresenter : Control
{
    internal List<DataBoxCell>? Cells { get; set; }

    internal DataBox? DataGrid { get; set; }

    internal object? Content { get; set; }

    internal void GenerateCells()
    {
        if (DataGrid?.Columns is null)
        {
            return;
        }

        Cells = new List<DataBoxCell>();

        foreach (var column in DataGrid.Columns)
        {
            var cell = new DataBoxCell()
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
        return DataBoxCellsLayout.Measure(availableSize, Cells);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return DataBoxCellsLayout.Arrange(finalSize, Cells);
    }
}
