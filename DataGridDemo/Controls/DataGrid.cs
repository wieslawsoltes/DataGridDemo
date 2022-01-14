using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;
using DataGridDemo.ViewModels;

namespace DataGridDemo.Controls;

public class DataGrid : Control
{
    public List<DataGridColumn> Columns { get; set; }
    public IList<object> Items { get; set; }
    internal List<DataGridRow>? Rows;
    internal double[] ColumnWidths;

    public DataGrid()
    {
        Columns = new List<DataGridColumn>()
        {
            new DataGridColumn()
            {
                Width = new GridLength(1, GridUnitType.Star),
                CellTemplate = new FuncDataTemplate(
                    (o) => true, 
                    (o, scope) => 
                        new Border()
                        {
                            Background = Brushes.Red,
                            Child = new TextBlock
                            {
                                [!TextBlock.TextProperty] = new Binding("Column0"),
                                Margin = new Thickness(5),
                                DataContext = o
                            }
                        }, 
                    true),
                Index = 0
            },
            new DataGridColumn()
            {
                //Width = new GridLength(1, GridUnitType.Auto),
                Width = new GridLength(1, GridUnitType.Star),
                CellTemplate = new FuncDataTemplate(
                    (o) => true, 
                    (o, scope) => 
                        new Border()
                        {
                            Background = Brushes.Green,
                            Child = new TextBlock
                            {
                                [!TextBlock.TextProperty] = new Binding("Column1"),
                                Margin = new Thickness(5),
                                DataContext = o
                            }
                        }, 
                    true),
                Index = 1
            },
            new DataGridColumn()
            {
                //Width = new GridLength(200, GridUnitType.Pixel),
                Width = new GridLength(1, GridUnitType.Star),
                CellTemplate = new FuncDataTemplate(
                    (o) => true, 
                    (o, scope) => 
                        new Border()
                        {
                            Background = Brushes.Blue,
                            Child = new TextBlock
                            {
                                [!TextBlock.TextProperty] = new Binding("Column2"),
                                Margin = new Thickness(5),
                                DataContext = o
                            }
                        }, 
                    true),
                Index = 2
            },
        };

        ColumnWidths = new double[Columns.Count];

        Items = new List<object>();

        for (var i = 0; i < 50; i++)
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
                    Cells = new List<DataGridCell>(),
                    DataGrid = this
                };

                foreach (var column in Columns)
                {
                    var cell = new DataGridCell()
                    {
                        Column = column,
                        Child = column.CellTemplate?.Build(item),
                        DataGrid = this
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

            var totalWidth = 0.0;
            var totalHeight = 0.0;

            for (var c = 0; c < Columns.Count; c++)
            {
                for (var r = 0; r < Rows.Count; r++)
                {
                    var row = Rows[r];
                    var cell = row.Cells[c];
                    var width = cell.DesiredSize.Width;
                    ColumnWidths[c] = Math.Max(ColumnWidths[c], width);
                }

                totalWidth += ColumnWidths[c];
            }

            for (var r = 0; r < Rows.Count; r++)
            {
                var row = Rows[r];
                var height = row.DesiredSize.Height;
                totalHeight += height;
            }

            return new Size(totalWidth, totalHeight);
            //return base.MeasureOverride(new Size(totalWidth, totalHeight));
        }

        return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Rows is { })
        {
            SetFinalColumnWidths(finalSize.Width);

            var offset = 0.0;
            var finalSizeWidth = 0.0;
            var totalWidth = GetTotalWidth();

            foreach (var row in Rows)
            {
                row.Arrange(new Rect(0.0, offset, totalWidth, row.DesiredSize.Height));
                //row.Arrange(new Rect(0.0, offset, row.DesiredSize.Width, row.DesiredSize.Height));
                offset += row.DesiredSize.Height;
                //width = Math.Max(width, row.DesiredSize.Width);
                finalSizeWidth = Math.Max(finalSizeWidth, totalWidth);
            }

            return new Size(finalSizeWidth, offset);
        }
        else
        {
            return base.ArrangeOverride(finalSize);
        }
    }

    private double GetTotalWidth()
    {
        var totalWidth = 0.0;

        for (var c = 0; c < Columns.Count; c++)
        {
            totalWidth += ColumnWidths[c];
        }

        return totalWidth;
    }

    private void SetFinalColumnWidths(double finalWidth)
    {
        var totalStarSize = 0.0;
        var totalPixelSize = 0.0;

        for (var c = 0; c < Columns.Count; c++)
        {
            var column = Columns[c];

            switch (column.Width.GridUnitType)
            {
                case GridUnitType.Auto:
                    totalPixelSize += ColumnWidths[c];
                    break;
                case GridUnitType.Pixel:
                    ColumnWidths[c] = column.Width.Value;
                    totalPixelSize += ColumnWidths[c];
                    break;
                case GridUnitType.Star:
                    totalStarSize += column.Width.Value;
                    break;
            }
        }

        var starColumnsWidth = Math.Max(0, finalWidth - totalPixelSize);

        for (var c = 0; c < Columns.Count; c++)
        {
            var column = Columns[c];

            switch (column.Width.GridUnitType)
            {
                case GridUnitType.Star:
                    var percentage = column.Width.Value / totalStarSize;
                    var width = starColumnsWidth * percentage;
                    ColumnWidths[c] = width;
                    totalPixelSize += ColumnWidths[c];
                    break;
            }
        }
    }
}
