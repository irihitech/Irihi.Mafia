using Avalonia.Controls;
using Irihi.Mirana.Demo.Views.DrawerDemo;

namespace Irihi.Mirana.Demo.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        
        var drawerDemoButton = this.FindControl<Button>("DrawerDemoButton");
        var demoContent = this.FindControl<ContentControl>("DemoContent");
        
        if (drawerDemoButton != null && demoContent != null)
        {
            drawerDemoButton.Click += (s, e) =>
            {
                demoContent.Content = new DrawerDemoView();
            };
        }
    }
}