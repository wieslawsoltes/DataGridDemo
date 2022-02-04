using System.Collections.Generic;

namespace DataGridDemo.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public IList<object>? Items { get; set; }

    public MainWindowViewModel()
    {
        Items = new List<object>();

        for (var i = 0; i < 5_000; i++)
        {
            var item = new ItemViewModel()
            {
                Column0 = $"Pixel Column {i}-0",
                Column1 = $"Very long text Column {i}-1",
                Column2 = $"Auto Column {i}-2",
            };

            Items.Add(item);
        }
    }
}
