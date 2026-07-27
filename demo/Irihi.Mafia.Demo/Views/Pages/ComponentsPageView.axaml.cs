using Avalonia.Controls;
using Irihi.Mafia.Demo.ViewModels.ControlDetails;
using Irihi.Mafia.Demo.Views.Pages.ControlDetails;

namespace Irihi.Mafia.Demo.Views.Pages;

public partial class ComponentsPageView : UserControl
{
    public ComponentsPageView()
    {
        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainNavigation.PushAsync(new ContentPage()
        {
            Content = new ControlGalleryView(),
            Header = "Control Gallery",
            DataContext = new ControlGalleryViewModel() { NavigationRoot = MainNavigation, }
        });
    }
}
