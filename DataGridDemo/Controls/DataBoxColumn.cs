using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace DataGridDemo.Controls;

public class DataBoxColumn : AvaloniaObject
{
    public static readonly StyledProperty<IDataTemplate?> CellTemplateProperty = 
        AvaloniaProperty.Register<DataBoxColumn, IDataTemplate?>(nameof(CellTemplate));

    public static readonly StyledProperty<GridLength> WidthProperty = 
        AvaloniaProperty.Register<DataBoxColumn, GridLength>(nameof(Width));

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

    internal double MeasureWidth { get; set; }
}
