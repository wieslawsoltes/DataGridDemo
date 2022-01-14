using Avalonia;
using Avalonia.Controls;

namespace DataGridDemo.Controls;

public class DataGridCell : Control
{
    internal DataGridColumn? Column { get; set; }

    internal IControl? Child { get; set; }

    internal DataGrid? DataGrid { get; set; }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (Child is { } && Child.Parent is null)
        {
            ((ISetLogicalParent)Child).SetParent(this);
            VisualChildren.Add(Child);
            LogicalChildren.Add(Child);
        }

        base.OnAttachedToVisualTree(e);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Child is { })
        {
            Child.Measure(availableSize);
            return Child.DesiredSize;
        }

        return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Child is { } && Column is { } &&  DataGrid?.ColumnWidths is { })
        {
            var width = DataGrid.ColumnWidths[Column.Index];
            var rect = new Rect(0, 0, width, Child.DesiredSize.Height);
            Child.Arrange(rect);
            return rect.Size;
        }

        return base.ArrangeOverride(finalSize);
    }
}
