using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using DataGridDemo.Controls.Layout;

namespace DataGridDemo.Controls;

public class DataGrid : TemplatedControl, IChildIndexProvider
{
    public static readonly DirectProperty<DataGrid, AvaloniaList<DataGridColumn>> ColumnsProperty = 
        AvaloniaProperty.RegisterDirect<DataGrid, AvaloniaList<DataGridColumn>>(
            nameof(Columns), 
            o => o.Columns);

    public static readonly DirectProperty<DataGrid, IList?> ItemsProperty = 
        AvaloniaProperty.RegisterDirect<DataGrid, IList?>(
            nameof(Items), 
            o => o.Items, 
            (o, v) => o.Items = v);

    private AvaloniaList<DataGridColumn> _columns;
    private IList? _items;

    public AvaloniaList<DataGridColumn> Columns
    {
        get => _columns;
        private set => SetAndRaise(ColumnsProperty, ref _columns, value);
    }

    public IList? Items
    {
        get => _items;
        set => SetAndRaise(ItemsProperty, ref _items, value);
    }

    internal List<DataGridRow>? Rows { get; set; }

    public DataGrid()
    {
        _columns = new AvaloniaList<DataGridColumn>();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        GenerateRows();

        base.OnAttachedToVisualTree(e);
    }

    private void GenerateRows()
    {
        if (Rows != null || Items is null || Columns.Count <= 0)
        {
            return;
        }

        Rows = new List<DataGridRow>();

        foreach (var item in Items)
        {
            var row = new DataGridRow()
            {
                Content = item,
                DataGrid = this,
                DataContext = item
            };

            Rows.Add(row);
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
        return DataGridLayout.MeasureRows(availableSize, Columns, Rows);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return DataGridLayout.ArrangeRows(finalSize, Columns, Rows);
    }

    int IChildIndexProvider.GetChildIndex(ILogical child)
    {
        if (Rows is { } && child is DataGridRow row)
        {
            return Rows.IndexOf(row);
        }

        return -1;
    }

    bool IChildIndexProvider.TryGetTotalCount(out int count)
    {
        count = Rows?.Count ?? 0;
        return true;
    }

    private EventHandler<ChildIndexChangedEventArgs>? _childIndexChanged;

    event EventHandler<ChildIndexChangedEventArgs>? IChildIndexProvider.ChildIndexChanged
    {
        add => _childIndexChanged += value;
        remove => _childIndexChanged -= value;
    }

    private void RaiseChildIndexChanged()
    {
        _childIndexChanged?.Invoke(this, new ChildIndexChangedEventArgs());
    }
}
