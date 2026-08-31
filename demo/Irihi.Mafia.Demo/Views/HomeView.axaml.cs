using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
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
        GalleryTabContent.DataContext = new GalleryTabViewModel { NavigationRoot = nav };
        HomeTabContent.DataContext = new HomeTabViewModel { NavigationRoot = nav };
        SettingTabContent.DataContext = new SettingTabViewModel { NavigationRoot = nav };
    }
}