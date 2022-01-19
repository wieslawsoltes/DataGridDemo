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

    private bool HasStarColumn(List<DataGridColumn> columns)
    {
        bool hasStarColumn = false;

        for (var c = 0; c < columns.Count; c++)
        {
            var column = columns[c];

            switch (column.Width.GridUnitType)
            {
                case GridUnitType.Star:
                    hasStarColumn = true;
                    break;
            }
        }

        return hasStarColumn;
    }

    private void MeasureColumnCells(DataGridColumn column, List<DataGridRow> rows)
    {
        int columnIndex = column.Index;

        for (var r = 0; r < rows.Count; r++)
        {
            var row = rows[r];
            if (row.CellsPresenter?.Cells is { })
            {
                var cell = row.CellsPresenter.Cells[columnIndex];
                var width = cell.DesiredSize.Width;
                column.MeasureWidth = Math.Max(column.MeasureWidth, width);
            }
        }
    }

    private double GetTotalMeasureWidth(List<DataGridColumn> columns)
    {
        var totalMeasureWidth = 0.0;
        
        for (var c = 0; c < columns.Count; c++)
        {
            var column = columns[c];

            totalMeasureWidth += column.MeasureWidth;
        }

        return totalMeasureWidth;
    }

    private void CalculateColumnWidths(List<DataGridColumn> columns, double finalWidth)
    {
        var totalStarSize = 0.0;
        var totalPixelSize = 0.0;

        for (var c = 0; c < columns.Count; c++)
        {
            var column = columns[c];

            switch (column.Width.GridUnitType)
            {
                case GridUnitType.Auto:
                    totalPixelSize += column.MeasureWidth;
                    break;
                case GridUnitType.Pixel:
                    column.MeasureWidth = column.Width.Value;
                    totalPixelSize += column.MeasureWidth;
                    break;
                case GridUnitType.Star:
                    totalStarSize += column.Width.Value;
                    break;
            }
        }

        var starColumnsWidth = Math.Max(0, finalWidth - totalPixelSize);

        // Debug.WriteLine($"starColumnsWidth='{starColumnsWidth}', finalWidth='{finalWidth}', totalPixelSize='{totalPixelSize}', totalStarSize='{totalStarSize}'");

        for (var c = 0; c < columns.Count; c++)
        {
            var column = columns[c];

            switch (column.Width.GridUnitType)
            {
                case GridUnitType.Star:
                    var percentage = column.Width.Value / totalStarSize;
                    var width = starColumnsWidth * percentage;
                    // Debug.WriteLine($"[{c}] width='{width}', percentage='{percentage}', finalWidth='{finalWidth}'");
                    column.MeasureWidth = width;
                    totalPixelSize += column.MeasureWidth;
                    break;
            }
        }
    }

    private Size MeasureRows(Size availableSize)
    {
        if (Rows is null || Columns is null)
        {
            return availableSize;
        }

        foreach (var row in Rows)
        {
            row.Measure(availableSize);
        }

        var totalWidth = 0.0;
        var totalHeight = 0.0;

        for (var c = 0; c < Columns.Count; c++)
        {
            var column = Columns[c];

            MeasureColumnCells(column, Rows);

            totalWidth += column.MeasureWidth;
        }

        for (var r = 0; r < Rows.Count; r++)
        {
            var row = Rows[r];
            var height = row.DesiredSize.Height;
            totalHeight += height;
        }

        var hasStarColumn = HasStarColumn(Columns);

        var size = new Size(hasStarColumn ? 0 : totalWidth, totalHeight);
        // Debug.WriteLine($"[MeasureRows] size='{size}'");
        return size;
    }

    private Size ArrangeRows(List<DataGridRow> rows, double totalWidth)
    {
        var offset = 0.0;
        var finalSizeWidth = 0.0;

        foreach (var row in rows)
        {
            var rect = new Rect(
                0.0, 
                offset, 
                totalWidth, 
                row.DesiredSize.Height);

            row.Arrange(rect);

            offset += row.DesiredSize.Height;
            finalSizeWidth = Math.Max(finalSizeWidth, totalWidth);
        }

        return new Size(finalSizeWidth, offset);
    }

    private Size ArrangeRows(Size finalSize)
    {
        if (Rows is null || Columns is null)
        {
            return finalSize;
        }

        CalculateColumnWidths(Columns, finalSize.Width);

        var totalWidth = GetTotalMeasureWidth(Columns);

        var size = ArrangeRows(Rows, totalWidth);

        // Debug.WriteLine($"[ArrangeRows] size='{size}'");
        return size;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return MeasureRows(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return ArrangeRows(finalSize);
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
