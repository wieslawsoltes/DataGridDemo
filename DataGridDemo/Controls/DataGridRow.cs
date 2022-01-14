using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;

namespace DataGridDemo.Controls;

public class DataGridRow : Control
{
    internal List<DataGridCell>? Cells { get; set; }
        
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
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

        base.OnAttachedToVisualTree(e);
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
        else
        {
            return base.MeasureOverride(availableSize);
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Cells is { })
        {
            var offset = 0.0;
            var height = 0.0;

            foreach (var cell in Cells)
            {
                cell.Arrange(new Rect(offset, 0.0, cell.DesiredSize.Width, cell.DesiredSize.Height));
                offset += cell.DesiredSize.Width;
                height = Math.Max(height, cell.DesiredSize.Height);
            }

            return new Size(offset, height);
        }
        else
        {
            return base.ArrangeOverride(finalSize);    
        }
    }
}
