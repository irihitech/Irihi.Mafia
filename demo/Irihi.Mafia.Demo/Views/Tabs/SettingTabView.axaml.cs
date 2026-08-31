using Avalonia.Controls;
using Avalonia.Interactivity;
using Irihi.Mafia.Demo.ViewModels;

namespace Irihi.Mafia.Demo.Views;

public partial class SettingTabView : ContentPage
{
    public SettingTabView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (this.DataContext is SettingTabViewModel vm)
        {
            vm.Launcher = TopLevel.GetTopLevel(this)?.Launcher;
        }
    }
}
