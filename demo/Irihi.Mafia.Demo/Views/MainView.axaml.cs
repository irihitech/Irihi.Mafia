using Avalonia.Controls;
using Irihi.Mafia.Demo.ViewModels;

namespace Irihi.Mafia.Demo.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        HomePageContent.DataContext = new HomePageViewModel();
        ComponentsPageContent.DataContext = new ComponentsPageViewModel();
        ListPageContent.DataContext = new ListPageViewModel();
    }
}
