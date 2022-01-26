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

public class DataBox : TemplatedControl, IChildIndexProvider
{
    public static readonly DirectProperty<DataBox, AvaloniaList<DataBoxColumn>> ColumnsProperty = 
        AvaloniaProperty.RegisterDirect<DataBox, AvaloniaList<DataBoxColumn>>(
            nameof(Columns), 
            o => o.Columns);

    public static readonly DirectProperty<DataBox, IList?> ItemsProperty = 
        AvaloniaProperty.RegisterDirect<DataBox, IList?>(
            nameof(Items), 
            o => o.Items, 
            (o, v) => o.Items = v);

    private AvaloniaList<DataBoxColumn> _columns;
    private IList? _items;

    public AvaloniaList<DataBoxColumn> Columns
    {
        get => _columns;
        private set => SetAndRaise(ColumnsProperty, ref _columns, value);
    }

    public IList? Items
    {
        get => _items;
        set => SetAndRaise(ItemsProperty, ref _items, value);
    }

    internal List<DataBoxRow>? Rows { get; set; }

    public DataBox()
    {
        _columns = new AvaloniaList<DataBoxColumn>();
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

        Rows = new List<DataBoxRow>();

        foreach (var item in Items)
        {
            var row = new DataBoxRow()
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
        return DataBoxRowsLayout.Measure(availableSize, Columns, Rows);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return DataBoxRowsLayout.Arrange(finalSize, Columns, Rows);
    }

    int IChildIndexProvider.GetChildIndex(ILogical child)
    {
        if (Rows is { } && child is DataBoxRow row)
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
