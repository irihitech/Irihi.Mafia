using Avalonia;
using Avalonia.Controls;

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

        if (CurrentPage is null)
            await PushAsync(new HomeView());
    }
}