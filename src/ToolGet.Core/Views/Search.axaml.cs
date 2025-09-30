using Avalonia.Controls;
using Avalonia.Input;
using ToolGet.Core.ViewModels;

namespace ToolGet.Core.Views;

public partial class Search : UserControl
{
    public Search()
    {
        InitializeComponent();
    }

    private void SearchBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (DataContext is SearchViewModel vm && vm.SearchCommand.CanExecute(null))
            {
                vm.SearchCommand.Execute(null);
                e.Handled = true;
            }
        }
    }
}