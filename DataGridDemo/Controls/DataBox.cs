using System;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Styling;

namespace DataGridDemo.Controls;

public class DataBox : ListBox, IStyleable
{
    public static readonly DirectProperty<DataBox, AvaloniaList<DataBoxColumn>> ColumnsProperty = 
        AvaloniaProperty.RegisterDirect<DataBox, AvaloniaList<DataBoxColumn>>(
            nameof(Columns), 
            o => o.Columns);

    private AvaloniaList<DataBoxColumn> _columns;

    Type IStyleable.StyleKey => typeof(DataBox);

    public AvaloniaList<DataBoxColumn> Columns
    {
        get => _columns;
        private set => SetAndRaise(ColumnsProperty, ref _columns, value);
    }

    public DataBox()
    {
        _columns = new AvaloniaList<DataBoxColumn>();
    }

    protected override IItemContainerGenerator CreateItemContainerGenerator()
    {
        var generator = new ItemContainerGenerator<DataBoxRow>(
            this,
            ContentControl.ContentProperty,
            ContentControl.ContentTemplateProperty);

        generator.Materialized += (_, args) =>
        {
            foreach (var container in args.Containers)
            {
                if (container.ContainerControl is DataBoxRow row)
                {
                    row.DataBox = this;
                }
            }
        };

        generator.Dematerialized += (_, args) =>
        {
            foreach (var container in args.Containers)
            {
                if (container.ContainerControl is DataBoxRow row)
                {
                    row.DataBox = null;
                }
            }
        };

        generator.Recycled += (_, args) =>
        {
            foreach (var container in args.Containers)
            {
                if (container.ContainerControl is DataBoxRow row)
                {
                    row.DataBox = this;
                }
            }
        };

        return generator;
    }
}
