# Drawer Component

A drawer control that displays content in an overlay layer, sliding in from a screen edge. This component is similar to Avalonia's Popup but specifically designed for drawer-style UI patterns commonly used in mobile applications.

## Overview

The drawer system consists of three main components:

1. **DrawerOverlayLayer** - A canvas-based container that hosts drawer overlays
2. **DrawerOverlayHost** - The host control that manages drawer position, animation, and lifecycle
3. **Drawer** - The main control that users interact with

## Features

- **Multiple Placement Options**: Bottom, Top, Left, Right
- **Smooth Animations**: Slide-in and slide-out animations with easing
- **Light Dismiss**: Click outside to close (configurable)
- **Modal Support**: Blocks interaction with content behind the drawer
- **Customizable Content**: Any Avalonia control can be placed inside
- **Backdrop/Scrim**: Semi-transparent overlay when drawer is open

## Usage

### Basic Example

```xml
<Panel>
    <!-- Your main content -->
    <StackPanel>
        <Button Name="OpenButton" Content="Open Drawer" />
    </StackPanel>
    
    <!-- Drawer Overlay Layer - Required to host drawers -->
    <m:DrawerOverlayLayer Name="DrawerOverlay" />
    
    <!-- Drawer Definition -->
    <m:Drawer Name="MyDrawer" Placement="Bottom">
        <Border
            Background="White"
            Padding="24"
            MaxHeight="400"
            CornerRadius="16,16,0,0">
            <StackPanel Spacing="16">
                <TextBlock Text="Drawer Content" FontSize="20" FontWeight="Bold" />
                <TextBlock Text="Your content here..." />
                <Button Name="CloseButton" Content="Close" />
            </StackPanel>
        </Border>
    </m:Drawer>
</Panel>
```

### Code-Behind

```csharp
public class MyView : UserControl
{
    public MyView()
    {
        InitializeComponent();
        
        var openButton = this.FindControl<Button>("OpenButton");
        var closeButton = this.FindControl<Button>("CloseButton");
        var drawer = this.FindControl<Drawer>("MyDrawer");
        
        openButton.Click += (s, e) => drawer.IsOpen = true;
        closeButton.Click += (s, e) => drawer.IsOpen = false;
    }
}
```

## Properties

### Drawer

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Child` | `Control?` | `null` | The control to display in the drawer |
| `IsOpen` | `bool` | `false` | Whether the drawer is currently open |
| `Placement` | `DrawerPlacement` | `Bottom` | Which screen edge the drawer slides from |
| `IsLightDismissEnabled` | `bool` | `true` | Whether clicking outside closes the drawer |
| `IsModal` | `bool` | `true` | Whether the drawer blocks interaction with content behind it |

### DrawerPlacement Enum

- `Bottom` - Slides in from the bottom (most common for mobile)
- `Top` - Slides in from the top
- `Left` - Slides in from the left (navigation drawer style)
- `Right` - Slides in from the right (action drawer style)

## Events

| Event | Description |
|-------|-------------|
| `Opened` | Raised when the drawer is opened |
| `Closed` | Raised when the drawer is closed |

## Examples

### Bottom Drawer (Default)

Perfect for action sheets, filters, or options menus:

```xml
<m:Drawer Placement="Bottom">
    <Border MaxHeight="400" CornerRadius="16,16,0,0">
        <!-- Your content -->
    </Border>
</m:Drawer>
```

### Left Navigation Drawer

Ideal for navigation menus:

```xml
<m:Drawer Placement="Left">
    <Border MaxWidth="300" CornerRadius="0,16,16,0">
        <StackPanel>
            <Button Content="Home" />
            <Button Content="Profile" />
            <Button Content="Settings" />
        </StackPanel>
    </Border>
</m:Drawer>
```

### Right Action Drawer

Good for contextual actions or settings:

```xml
<m:Drawer Placement="Right">
    <Border MaxWidth="300" CornerRadius="16,0,0,16">
        <!-- Actions or settings -->
    </Border>
</m:Drawer>
```

### Custom Content Drawer

A drawer with rich content like filters:

```xml
<m:Drawer Placement="Bottom">
    <Border MaxHeight="500" CornerRadius="16,16,0,0">
        <StackPanel Spacing="16">
            <TextBlock Text="Filters" FontSize="20" />
            <ComboBox />
            <Slider />
            <CheckBox Content="Option 1" />
            <Button Content="Apply" />
        </StackPanel>
    </Border>
</m:Drawer>
```

## Styling

The drawer uses TDesign theme tokens for consistent styling:

- `TDBackgroundColor1` - Drawer background
- `TDComponentBorder` - Border color
- `TDRadiusLarge` - Corner radius
- `TDShadow3` - Box shadow

You can override these by applying styles to the drawer content:

```xml
<m:Drawer Placement="Bottom">
    <Border
        Background="Red"
        CornerRadius="20,20,0,0"
        BoxShadow="0 0 20 5 #000000">
        <!-- Your content -->
    </Border>
</m:Drawer>
```

## Best Practices

1. **Always include a DrawerOverlayLayer** in your view to host drawers
2. **Provide a way to close** the drawer (button or light dismiss)
3. **Limit drawer height/width** to ensure usability (use MaxHeight/MaxWidth)
4. **Use appropriate corner radius** based on placement:
   - Bottom: Round top corners only
   - Top: Round bottom corners only
   - Left: Round right corners only
   - Right: Round left corners only
5. **Consider mobile screen sizes** when designing drawer content
6. **Use IsModal=true** for important actions that require user attention

## Architecture

The drawer implementation is based on Avalonia's overlay popup architecture:

- **DrawerOverlayLayer** is similar to `OverlayLayer` - provides the canvas for hosting
- **DrawerOverlayHost** is similar to `OverlayPopupHost` - manages lifecycle and positioning
- **Drawer** is similar to `Popup` - the public API users interact with

The drawer system uses:
- Canvas positioning for precise placement
- Avalonia Animations for smooth transitions
- Event routing for light dismiss behavior
- Logical tree for proper control lifetime management

## Known Limitations

- Requires Avalonia 11.3+ or 12.0+ (nightly builds)
- DrawerOverlayLayer must be present in the visual tree
- Only one drawer can be open at a time per overlay layer
- Animations use fixed durations (250ms in, 200ms out)

## Future Enhancements

Potential improvements for future versions:

- Configurable animation duration and easing
- Swipe-to-dismiss gesture support
- Multiple drawer layers
- Nested drawer support
- Custom backdrop styles
- Accessibility improvements (ARIA roles, keyboard navigation)
