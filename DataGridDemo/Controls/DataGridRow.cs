using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace DataGridDemo.Controls;

public class DataGridRow : TemplatedControl
{
    internal DataGrid? DataGrid { get; set; }

    internal object? Content { get; set; }

    internal DataGridCellsPresenter? CellsPresenter { get; set; }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        CellsPresenter = e.NameScope.Find<DataGridCellsPresenter>("PART_CellsPresenter");

        if (CellsPresenter is { })
        {
            CellsPresenter.Content = Content;
            CellsPresenter.DataGrid = DataGrid;
            CellsPresenter.GenerateCells();
        }
        
        base.OnApplyTemplate(e);
    }
}
