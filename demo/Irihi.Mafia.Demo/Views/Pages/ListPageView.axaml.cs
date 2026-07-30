using Avalonia.Controls;
using Avalonia.Interactivity;
using Irihi.Mafia.Demo.ViewModels;

namespace Irihi.Mafia.Demo.Views.Pages;

public partial class ListPageView : UserControl
{
    public ListPageView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (this.DataContext is ListPageViewModel vm)
        {
            vm.Launcher = TopLevel.GetTopLevel(this)?.Launcher;
        }
    }
}
