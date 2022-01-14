using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace DataGridDemo.Controls;

public class DataGridColumn
{
    public GridLength Width { get; set; }

    [Content]
    public IDataTemplate? CellTemplate { get; set; }

    internal int Index { get; set; }
}
