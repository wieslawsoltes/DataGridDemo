using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;

namespace DataGridDemo.Controls;

public class DataBoxRow : ListBoxItem, IStyleable
{
    Type IStyleable.StyleKey => typeof(DataBoxRow);

    internal DataBox? DataBox { get; set; }

    internal double MeasureWidth { get; set; }

    internal double MeasureHeight { get; set; }

    internal DataBoxCellsPresenter? CellsPresenter { get; set; }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        CellsPresenter = e.NameScope.Find<DataBoxCellsPresenter>("PART_CellsPresenter");

        if (CellsPresenter is { })
        {
            CellsPresenter.Content = DataContext;
            CellsPresenter.DataBox = DataBox;
            CellsPresenter.GenerateCells();
        }
        
        base.OnApplyTemplate(e);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = new Size(MeasureWidth, MeasureHeight);

        return base.ArrangeOverride(size);
    }
}
