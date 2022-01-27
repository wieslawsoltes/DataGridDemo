using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using DataBox.Layout;

namespace DataBox.Controls;

public class DataBoxPanel : VirtualizingStackPanel
{
    internal DataBox? DataBox { get; set; }

    public override void ApplyTemplate()
    {
        base.ApplyTemplate();

        DataBox = this.GetLogicalAncestors().FirstOrDefault(x => x is DataBox) as DataBox;
    }

    protected override Size MeasureOverride(Size availableSize)
    {  
        if (DataBox is null)
        {
            return availableSize;
        }

        var size = base.MeasureOverride(availableSize);

        DataBoxRowsLayout.Measure(availableSize, DataBox.Columns, Children);

        return size;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (DataBox is null)
        {
            return finalSize;
        }

        DataBoxRowsLayout.Arrange(finalSize, DataBox.Columns, Children);

        return base.ArrangeOverride(finalSize);
    }
}
