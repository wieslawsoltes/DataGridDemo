using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using DataGridDemo.ViewModels;

namespace DataGridDemo.Controls;

public class DataGrid : Control
{
    public List<DataGridColumn> Columns { get; set; }
    public IList<object> Items { get; set; }
    internal List<DataGridRow>? Rows;

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
            var item = new ItemViewModel()
            {
                Column0 = $"Column {i}-0",
                Column1 = $"Column {i}-1",
                Column2 = $"Column {i}-2",
            };
                
            Items.Add(item);
        }
    }

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
