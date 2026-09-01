using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Irihi.Mafia.Demo.Services;
using Irihi.Mafia.Demo.ViewModels;

namespace Irihi.Mafia.Demo.Views;

public partial class HomeView : TabbedPage
{
    public HomeView()
    {
        InitializeComponent();
    }

    protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var nav = this.FindAncestorOfType<NavigationPage>();
        if (nav is null) return;

        AppServices.Navigator = new NavigationService(nav);
        GalleryTabContent.DataContext = new GalleryTabViewModel();
        HomeTabContent.DataContext = new HomeTabViewModel();
        SettingTabContent.DataContext = new SettingTabViewModel();
    }
}
