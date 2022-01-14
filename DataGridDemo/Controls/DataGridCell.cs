using Avalonia;
using Avalonia.Controls;

namespace DataGridDemo.Controls;

public class DataGridCell : Control
{
    internal DataGridColumn? Column { get; set; }
    internal IControl? Child { get; set; }

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
        else
        {
            return base.MeasureOverride(availableSize);
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Child is { })
        {
            var rect = new Rect(0, 0, Child.DesiredSize.Width, Child.DesiredSize.Height);
            Child.Arrange(rect);
            return rect.Size;
        }
        else
        {
            return base.ArrangeOverride(finalSize);
        }
    }
}
