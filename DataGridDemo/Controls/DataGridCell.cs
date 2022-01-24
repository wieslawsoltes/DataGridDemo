using System;
using Avalonia;
using Avalonia.Controls;

namespace DataGridDemo.Controls;

public class DataGridCell : Control
{
    internal DataGridColumn? Column { get; set; }

    internal IControl? Child { get; set; }

    internal DataGrid? DataGrid { get; set; }

    internal double MeasureWidth { get; set; }

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
            Child.Measure(availableSize);

            MeasureWidth = Child.DesiredSize.Width;

            Column.MeasureWidth = Math.Max(Column.MeasureWidth, MeasureWidth);

            return Child.DesiredSize;
        }

        return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Child is { } && Column is { })
        {
            var width = Column.MeasureWidth;
            var height = Child.DesiredSize.Height;
            var rect = new Rect(0, 0, width, height);
            Child.Arrange(rect);
            return rect.Size;
        }

        return base.ArrangeOverride(finalSize);
    }
}
