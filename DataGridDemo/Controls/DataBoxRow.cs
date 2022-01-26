using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace DataGridDemo.Controls;

public class DataBoxRow : TemplatedControl
{
    internal DataBox? DataGrid { get; set; }

    internal object? Content { get; set; }

    internal DataBoxCellsPresenter? CellsPresenter { get; set; }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        CellsPresenter = e.NameScope.Find<DataBoxCellsPresenter>("PART_CellsPresenter");

        if (CellsPresenter is { })
        {
            CellsPresenter.Content = Content;
            CellsPresenter.DataGrid = DataGrid;
            CellsPresenter.GenerateCells();
        }
        
        base.OnApplyTemplate(e);
    }
}
