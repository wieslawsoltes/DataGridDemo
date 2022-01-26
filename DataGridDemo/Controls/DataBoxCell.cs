using Avalonia;
using Avalonia.Controls;
using DataGridDemo.Controls.Layout;

namespace DataGridDemo.Controls;

public class DataBoxCell : Control
{
    internal DataBoxColumn? Column { get; set; }

    internal IControl? Child { get; set; }

    internal DataBox? DataBox { get; set; }

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
        if (Child is { } && Column is { })
        {
            return DataBoxCellLayout.Measure(Child, Column, availableSize);
        }

        return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Child is { } && Column is { })
        {
            return DataBoxCellLayout.Arrange(Child, Column, finalSize);
        }

        return base.ArrangeOverride(finalSize);
    }

}
