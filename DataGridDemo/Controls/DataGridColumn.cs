using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace DataGridDemo.Controls;

public class DataGridColumn : AvaloniaObject
{
    public static readonly StyledProperty<IDataTemplate?> CellTemplateProperty = 
        AvaloniaProperty.Register<DataGridColumn, IDataTemplate?>(nameof(CellTemplate));

    public static readonly StyledProperty<GridLength> WidthProperty = 
        AvaloniaProperty.Register<DataGridColumn, GridLength>(nameof(Width));

    [Content]
    public IDataTemplate? CellTemplate
    {
        get => GetValue(CellTemplateProperty);
        set => SetValue(CellTemplateProperty, value);
    }

    public GridLength Width
    {
        get => GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }

    internal int Index { get; set; }

    internal double MeasureWidth { get; set; }
}
