using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.LogicalTree;
using DataGridDemo.ViewModels;

namespace DataGridDemo.Controls;

public class DataGrid : TemplatedControl, IChildIndexProvider
{
    public List<DataGridColumn>? Columns { get; set; }

    public IList<object>? Items { get; set; }

    internal List<DataGridRow>? Rows { get; set; }

    internal double[]? ColumnWidths { get; set; }

    public DataGrid()
    {
        CreateColumns();
        CreateItems();
    }

    private void CreateItems()
    {
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

    private void CreateColumns()
    {
        Columns = new List<DataGridColumn>()
        {
            new DataGridColumn()
            {
                Width = new GridLength(1, GridUnitType.Star),
                //Width = new GridLength(1, GridUnitType.Auto),
                //Width = new GridLength(200, GridUnitType.Pixel),
                CellTemplate = new FuncDataTemplate(
                    (_) => true,
                    (o, _) =>
                        new Border()
                        {
                            //Background = Brushes.Red,
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
                Width = new GridLength(1, GridUnitType.Star),
                //Width = new GridLength(1, GridUnitType.Auto),
                //Width = new GridLength(200, GridUnitType.Pixel),
                CellTemplate = new FuncDataTemplate(
                    (_) => true,
                    (o, _) =>
                        new Border()
                        {
                            //Background = Brushes.Green,
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
                Width = new GridLength(1, GridUnitType.Star),
                //Width = new GridLength(1, GridUnitType.Auto),
                //Width = new GridLength(200, GridUnitType.Pixel),
                CellTemplate = new FuncDataTemplate(
                    (_) => true,
                    (o, _) =>
                        new Border()
                        {
                            //Background = Brushes.Blue,
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
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        CreateRows();

        base.OnAttachedToVisualTree(e);
    }

    private void CreateRows()
    {
        if (Rows != null || Items is null || Columns is null)
        {
            return;
        }

        Rows = new List<DataGridRow>();

        foreach (var item in Items)
        {
            var row = new DataGridRow()
            {
                Item = item,
                DataGrid = this
            };

            Rows.Add(row);
        }

        AddRows();
    }

    private void AddRows()
    {
        if (Rows is null)
        {
            return;
        }

        foreach (var row in Rows)
        {
            ((ISetLogicalParent)row).SetParent(this);
            VisualChildren.Add(row);
            LogicalChildren.Add(row);
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (ColumnWidths is null && Columns is { })
        {
            ColumnWidths = new double[Columns.Count];
        }

        if (Rows is { } && Columns is { } && ColumnWidths is { })
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
                    if (row.CellsPresenter?.Cells is { })
                    {
                        var cell = row.CellsPresenter.Cells[c];
                        var width = cell.DesiredSize.Width;
                        ColumnWidths[c] = Math.Max(ColumnWidths[c], width);
                    }
                }

                totalWidth += ColumnWidths[c];
            }

            for (var r = 0; r < Rows.Count; r++)
            {
                var row = Rows[r];
                var height = row.DesiredSize.Height;
                totalHeight += height;
            }

            bool hasStarColumn = false;
            
            for (var c = 0; c < Columns.Count; c++)
            {
                var column = Columns[c];

                switch (column.Width.GridUnitType)
                {
                    case GridUnitType.Star:
                        hasStarColumn = true;
                        break;
                }
            }
            
            // var size = new Size(totalWidth, totalHeight);
            var size = new Size(hasStarColumn ? 0 : totalWidth, totalHeight);
            // Debug.WriteLine($"[MeasureOverride] size='{size}'");
            return size;
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
                offset += row.DesiredSize.Height;
                finalSizeWidth = Math.Max(finalSizeWidth, totalWidth);
            }

            var size = new Size(finalSizeWidth, offset);
            // Debug.WriteLine($"[ArrangeOverride] size='{size}'");
            return size;
        }

        return base.ArrangeOverride(finalSize);
    }

    private double GetTotalWidth()
    {
        var totalWidth = 0.0;

        if (Columns is { } && ColumnWidths is { })
        {
            for (var c = 0; c < Columns.Count; c++)
            {
                totalWidth += ColumnWidths[c];
            }
        }

        return totalWidth;
    }

    private void SetFinalColumnWidths(double finalWidth)
    {
        if (Columns is null || ColumnWidths is null)
        {
            return;
        }
        
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

        // Debug.WriteLine($"starColumnsWidth='{starColumnsWidth}', finalWidth='{finalWidth}', totalPixelSize='{totalPixelSize}', totalStarSize='{totalStarSize}'");

        for (var c = 0; c < Columns.Count; c++)
        {
            var column = Columns[c];

            switch (column.Width.GridUnitType)
            {
                case GridUnitType.Star:
                    var percentage = column.Width.Value / totalStarSize;
                    var width = starColumnsWidth * percentage;
                    // Debug.WriteLine($"[{c}] width='{width}', percentage='{percentage}', finalWidth='{finalWidth}'");
                    ColumnWidths[c] = width;
                    totalPixelSize += ColumnWidths[c];
                    break;
            }
        }
    }

    public int GetChildIndex(ILogical child)
    {
        if (Rows is { } && child is DataGridRow row)
        {
            return Rows.IndexOf(row);
        }

        return -1;
    }

    public bool TryGetTotalCount(out int count)
    {
        count = Rows?.Count ?? 0;
        return true;
    }

    public event EventHandler<ChildIndexChangedEventArgs>? ChildIndexChanged;
}
