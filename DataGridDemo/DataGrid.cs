using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Metadata;

namespace DataGridDemo;

public class Item
{
    public string? Column0 { get; set; }

    public string? Column1 { get; set; }

    public string? Column2 { get; set; }
}
    
public class DataGridColumn
{
    public GridLength Width { get; set; }

    [Content]
    public IDataTemplate? CellTemplate { get; set; }
}

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

        return base.ArrangeOverride(finalSize);
    }
}
    
public class DataGrid : Control
{
    public List<DataGridColumn> Columns { get; set; }

    public IList<object> Items { get; set; }

    public DataGrid()
    {
        Columns = new List<DataGridColumn>()
        {
            new DataGridColumn()
            {
                Width = new GridLength(1, GridUnitType.Star),
                CellTemplate = new FuncDataTemplate(
                    (o) => true, 
                    (o, scope) => new TextBlock
                    {
                        [!TextBlock.TextProperty] = new Binding("Column0"),
                        DataContext = o
                    }, 
                    true)
            },
            new DataGridColumn()
            {
                Width = new GridLength(1, GridUnitType.Auto),
                CellTemplate = new FuncDataTemplate(
                    (o) => true, 
                    (o, scope) => new TextBlock
                    {
                        [!TextBlock.TextProperty] = new Binding("Column1"),
                        DataContext = o
                    }, 
                    true)
            },
            new DataGridColumn()
            {
                Width = new GridLength(100, GridUnitType.Pixel),
                CellTemplate = new FuncDataTemplate(
                    (o) => true, 
                    (o, scope) => new TextBlock
                    {
                        [!TextBlock.TextProperty] = new Binding("Column2"),
                        DataContext = o
                    }, 
                    true)
            },
        };

        Items = new List<object>();

        for (var i = 0; i < 10; i++)
        {
            var item = new Item()
            {
                Column0 = $"Column {i}-0",
                Column1 = $"Column {i}-1",
                Column2 = $"Column {i}-2",
            };
                
            Items.Add(item);
        }
    }

    private List<DataGridRow>? Rows;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {            
        if (Rows == null)
        {
            Rows = new List<DataGridRow>();

            foreach (var item in Items)
            {
                var row = new DataGridRow()
                {
                    Cells = new List<DataGridCell>()
                };

                foreach (var column in Columns)
                {
                    var cell = new DataGridCell()
                    {
                        Column = column,
                        Child = column.CellTemplate?.Build(item)
                    };
                    row.Cells.Add(cell);
                }
                    
                Rows.Add(row);
            }
        }
            
        if (Rows is { })
        {
            foreach (var row in Rows)
            {
                ((ISetLogicalParent)row).SetParent(this);
                VisualChildren.Add(row);
                LogicalChildren.Add(row);
            }
        }

        base.OnAttachedToVisualTree(e);
    }
        
    protected override Size MeasureOverride(Size availableSize)
    {
        if (Rows is { })
        {
            foreach (var row in Rows)
            {
                row.Measure(availableSize);
            }
        }

        return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Rows is { })
        {
            var offset = 0.0;
            var width = 0.0;

            foreach (var row in Rows)
            {
                row.Arrange(new Rect(0.0, offset, row.DesiredSize.Width, row.DesiredSize.Height));
                offset += row.DesiredSize.Height;
                width = Math.Max(width, row.DesiredSize.Width);
            }

            return new Size(width, offset);
        }
        else
        {
            return base.ArrangeOverride(finalSize);
        }
    }
}
