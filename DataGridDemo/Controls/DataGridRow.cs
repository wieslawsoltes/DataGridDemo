using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace DataGridDemo.Controls;

public class DataGridRow : TemplatedControl
{
    internal DataGrid? DataGrid { get; set; }

    internal object? Item { get; set; }

    internal DataGridCellsPresenter? CellsPresenter { get; set; }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        CellsPresenter = e.NameScope.Find<DataGridCellsPresenter>("PART_CellsPresenter");

        if (CellsPresenter is { })
        {
            CellsPresenter.Content = Item;
            CellsPresenter.DataGrid = DataGrid;
            CellsPresenter.CreateCells();
        }
        
        base.OnApplyTemplate(e);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        //if (VisualChildren.Count == 1 && VisualChildren[0] is IControl control)
        //{
        //    control.Measure(availableSize);
        //    return control.DesiredSize;
        //}

        return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        //if (VisualChildren.Count == 1 && VisualChildren[0] is IControl control)
        //{
        //    var rect = new Rect(0, 0, finalSize.Width, finalSize.Height);
        //    control.Arrange(rect);
        //    return rect.Size;
        //}

        return base.ArrangeOverride(finalSize);
    }
}
