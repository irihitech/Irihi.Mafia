using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Irihi.Mafia.Demo.Views;

public partial class MainView : NavigationPage
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        if (CurrentPage == null)
            await PushAsync(new HomeView());
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (TopLevel.GetTopLevel(this) is { } toplevel)
        {
            toplevel.BackRequested += OnBackRequested;
        }
    }

    private void OnBackRequested(object? sender, RoutedEventArgs e)
    {
        if (MainNavigation.CanGoBack)
        {
            MainNavigation.PopAsync();
            e.Handled = true;
        }
    }
}