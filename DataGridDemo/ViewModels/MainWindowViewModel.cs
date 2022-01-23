using System.Collections.Generic;

namespace DataGridDemo.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public IList<object>? Items { get; set; }

    public MainWindowViewModel()
    {
        Items = new List<object>();

        for (var i = 0; i < 50; i++)
        {
            var item = new ItemViewModel()
            {
                Column0 = $"Column {i}-0",
                Column1 = $"Column {i}-1",
                Column2 = $"Column {i}-2",
            };

            Items.Add(item);
        }
    }
}
