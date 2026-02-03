using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Irihi.Mirana.Demo.Views.DrawerDemo;

public partial class DrawerDemoView : UserControl
{
    public DrawerDemoView()
    {
        InitializeComponent();
        
        // Wire up button click events
        var bottomButton = this.FindControl<Button>("BottomDrawerButton");
        var topButton = this.FindControl<Button>("TopDrawerButton");
        var leftButton = this.FindControl<Button>("LeftDrawerButton");
        var rightButton = this.FindControl<Button>("RightDrawerButton");
        var customButton = this.FindControl<Button>("CustomDrawerButton");
        
        var bottomDrawer = this.FindControl<Controls.Drawer>("BottomDrawer");
        var topDrawer = this.FindControl<Controls.Drawer>("TopDrawer");
        var leftDrawer = this.FindControl<Controls.Drawer>("LeftDrawer");
        var rightDrawer = this.FindControl<Controls.Drawer>("RightDrawer");
        var customDrawer = this.FindControl<Controls.Drawer>("CustomDrawer");
        
        if (bottomButton != null && bottomDrawer != null)
        {
            bottomButton.Click += (s, e) => bottomDrawer.IsOpen = true;
        }
        
        if (topButton != null && topDrawer != null)
        {
            topButton.Click += (s, e) => topDrawer.IsOpen = true;
        }
        
        if (leftButton != null && leftDrawer != null)
        {
            leftButton.Click += (s, e) => leftDrawer.IsOpen = true;
        }
        
        if (rightButton != null && rightDrawer != null)
        {
            rightButton.Click += (s, e) => rightDrawer.IsOpen = true;
        }
        
        if (customButton != null && customDrawer != null)
        {
            customButton.Click += (s, e) => customDrawer.IsOpen = true;
        }
        
        // Wire up close buttons
        var closeBottomButton = this.FindControl<Button>("CloseBottomButton");
        var closeTopButton = this.FindControl<Button>("CloseTopButton");
        var closeLeftButton = this.FindControl<Button>("CloseLeftButton");
        var closeRightButton = this.FindControl<Button>("CloseRightButton");
        var applyCustomButton = this.FindControl<Button>("ApplyCustomButton");
        var resetCustomButton = this.FindControl<Button>("ResetCustomButton");
        
        if (closeBottomButton != null && bottomDrawer != null)
        {
            closeBottomButton.Click += (s, e) => bottomDrawer.IsOpen = false;
        }
        
        if (closeTopButton != null && topDrawer != null)
        {
            closeTopButton.Click += (s, e) => topDrawer.IsOpen = false;
        }
        
        if (closeLeftButton != null && leftDrawer != null)
        {
            closeLeftButton.Click += (s, e) => leftDrawer.IsOpen = false;
        }
        
        if (closeRightButton != null && rightDrawer != null)
        {
            closeRightButton.Click += (s, e) => rightDrawer.IsOpen = false;
        }
        
        if (applyCustomButton != null && customDrawer != null)
        {
            applyCustomButton.Click += (s, e) => customDrawer.IsOpen = false;
        }
        
        if (resetCustomButton != null)
        {
            resetCustomButton.Click += (s, e) => 
            {
                // Reset the filter values - in a real app, this would reset actual filter state
                // For the demo, we're just showing the button handler structure
            };
        }
    }
}
