# Drawer Component Implementation Summary

## Overview

This implementation provides a general-purpose popup mechanism similar to Avalonia's overlay popup, specifically designed for drawer-style UI patterns commonly used in mobile applications. The drawer slides in from screen edges (Bottom, Top, Left, or Right) with smooth animations.

## Architecture

The implementation follows Avalonia's overlay popup architecture pattern:

```
┌─────────────────────────────────────────────────────────────┐
│  Application Window / TopLevel (with VisualLayerManager)   │
│                                                               │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  Main Content Area                                     │  │
│  │                                                         │  │
│  │  ┌─────────────────────────────────────────────────┐  │  │
│  │  │  OverlayLayer (via VisualLayerManager)          │  │  │
│  │  │  (Avalonia built-in, automatic)                  │  │  │
│  │  │                                                   │  │  │
│  │  │  ┌─────────────────────────────────────────┐    │  │  │
│  │  │  │ DrawerOverlayHost (ContentControl)      │    │  │  │
│  │  │  │                                           │    │  │  │
│  │  │  │  ┌─────────────────────────────────┐    │    │  │  │
│  │  │  │  │ Backdrop/Scrim (Border)          │    │    │  │  │
│  │  │  │  └─────────────────────────────────┘    │    │  │  │
│  │  │  │                                           │    │  │  │
│  │  │  │  ┌─────────────────────────────────┐    │    │  │  │
│  │  │  │  │ Content Border (with animation)  │    │    │  │  │
│  │  │  │  │                                   │    │    │  │  │
│  │  │  │  │  User's Child Control Content    │    │    │  │  │
│  │  │  │  │                                   │    │    │  │  │
│  │  │  │  └─────────────────────────────────┘    │    │  │  │
│  │  │  └─────────────────────────────────────────┘    │  │  │
│  │  └─────────────────────────────────────────────────┘  │  │
│  └───────────────────────────────────────────────────────┘  │
│                                                               │
│  Drawer (Control) - Logical parent, manages IsOpen property │
└─────────────────────────────────────────────────────────────┘
```

## Component Responsibilities

### 1. Drawer (Main API)
- **Purpose**: Public-facing control that users interact with
- **Responsibilities**:
  - Manages `IsOpen` property
  - Provides placement options via `DrawerPlacement`
  - Creates and manages `DrawerOverlayHost` lifecycle
  - Finds Avalonia's OverlayLayer via `OverlayLayer.GetOverlayLayer()`
  - Handles logical tree attachment/detachment
  - Raises `Opened` and `Closed` events

### 2. DrawerOverlayHost
- **Purpose**: Visual host for the drawer in the overlay layer
- **Responsibilities**:
  - Manages drawer positioning based on placement
  - Handles slide-in/out animations
  - Implements light dismiss (click outside to close)
  - Renders backdrop/scrim overlay
  - Manages content layout and sizing

### 3. OverlayLayer (Avalonia Built-in)
- **Purpose**: Canvas-based container for hosting overlays
- **How It Works**:
  - Automatically available in all Avalonia windows via `VisualLayerManager`
  - Accessed using `OverlayLayer.GetOverlayLayer(visual)`
  - No manual setup required
  - Provides the overlay surface for drawers
  - Manages available size for positioning

### 4. DrawerPlacement
- **Purpose**: Enum defining placement options
- **Values**:
  - `Bottom` - Slides from bottom (default, most common)
  - `Top` - Slides from top
  - `Left` - Slides from left (navigation drawer)
  - `Right` - Slides from right (action drawer)

## Key Features

### Animation System
- **Slide-in**: 250ms with `CubicEaseOut` easing
- **Slide-out**: 200ms with `CubicEaseIn` easing
- **Properties animated**:
  - Opacity (0 → 1 on show, 1 → 0 on hide)
  - RenderTransform (TranslateTransform based on placement)

### Light Dismiss
- Enabled by default (`IsLightDismissEnabled = true`)
- Clicking on the backdrop closes the drawer
- Uses `PointerPressedEvent` with tunneling routing
- Checks if click is outside content bounds

### Positioning Logic
Each placement has specific positioning rules:

```csharp
Bottom: 
  - Canvas.Left = 0
  - Canvas.Top = overlayHeight - drawerHeight
  - Width = overlayWidth
  - Slides from: Y + drawerHeight

Top:
  - Canvas.Left = 0
  - Canvas.Top = 0
  - Width = overlayWidth
  - Slides from: Y - drawerHeight

Left:
  - Canvas.Left = 0
  - Canvas.Top = 0
  - Height = overlayHeight
  - Slides from: X - drawerWidth

Right:
  - Canvas.Left = overlayWidth - drawerWidth
  - Canvas.Top = 0
  - Height = overlayHeight
  - Slides from: X + drawerWidth
```

## Comparison with Avalonia's Popup

| Feature | Avalonia Popup | Mirana Drawer |
|---------|----------------|---------------|
| Base Class | Control | Control |
| Host Class | OverlayPopupHost | DrawerOverlayHost |
| Overlay Layer | OverlayLayer (built-in) | OverlayLayer (built-in) |
| Positioning | Relative to target | Fixed to screen edge |
| Animation | Optional | Built-in slide animation |
| Placement | Complex anchor/gravity | Simple edge-based |
| Use Case | Context menus, tooltips | Mobile-style drawers |
| Setup Required | None | None |

## Usage Pattern

### XAML Structure
```xml
<Panel>
    <!-- Main content -->
    <YourContent />
    
    <!-- No overlay layer needed - uses Avalonia's built-in! -->
    
    <!-- Drawer definitions -->
    <m:Drawer Placement="Bottom">
        <Border>
            <!-- Your drawer content -->
        </Border>
    </m:Drawer>
</Panel>
```

### Code Interaction
```csharp
// Opening
drawer.IsOpen = true;
// or
drawer.Open();

// Closing
drawer.IsOpen = false;
// or
drawer.Close();

// Events
drawer.Opened += (s, e) => { /* ... */ };
drawer.Closed += (s, e) => { /* ... */ };
```

## Theme Integration

The TDesign theme provides:
- `TDBackgroundColor1` - Drawer surface
- `TDComponentBorder` - Border color
- `TDRadiusLarge` - Corner radius
- `TDShadow3` - Drop shadow
- Backdrop with `#80000000` (50% black)

## Implementation Details

### File Structure
```
src/Irihi.Mirana/Controls/Drawer/
├── Drawer.cs                 # Main control (uses OverlayLayer)
├── DrawerOverlayHost.cs      # Host with animation (uses OverlayLayer)
├── DrawerOverlayLayer.cs     # Obsolete (extends OverlayLayer for compatibility)
├── DrawerPlacement.cs        # Enum (28 lines)
└── README.md                 # Documentation

src/Irihi.Mirana.Themes.TDesign/Controls/
└── Drawer.axaml              # Theme styles

demo/Irihi.Mirana.Demo/Views/DrawerDemo/
├── DrawerDemoView.axaml      # Demo UI (no manual overlay needed)
└── DrawerDemoView.axaml.cs   # Demo code-behind

test/Irihi.Mirana.UnitTest/Controls/
└── DrawerTests.cs            # Unit tests
```

### Dependencies
- Avalonia 12.0.999-cibuild0061905-alpha (nightly)
- Irihi.Avalonia.Shared 0.3.1

## Known Limitations

1. **Build Environment**: Requires access to Avalonia nightly feed
2. **Single Drawer**: Only one drawer per overlay layer at a time
3. **Fixed Animation**: Animation timing is hardcoded
4. **No Gestures**: Swipe-to-dismiss not implemented
5. **VisualLayerManager Required**: Needs standard Avalonia window infrastructure (always present)

## Future Enhancements

Potential improvements:
1. ✨ Configurable animation duration and easing
2. ✨ Touch gesture support (swipe to open/close)
3. ✨ Multiple simultaneous drawers with z-index management
4. ✨ Keyboard navigation and accessibility improvements
5. ✨ Partial drawer states (peek, half-open, full-open)
6. ✨ Custom backdrop styles and blur effects
7. ✨ Resize handle for adjustable drawer height

## Testing

Unit tests cover:
- Default property values
- Property changes
- Enum value integrity
- Basic open/close behavior

**Note**: Full integration tests require Avalonia.Headless.

## References

This implementation is inspired by:
- [Avalonia OverlayPopupHost.cs](https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Controls/Primitives/OverlayPopupHost.cs)
- [Avalonia OverlayLayer.cs](https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Controls/Primitives/OverlayLayer.cs)
- [Avalonia Popup.cs](https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Controls/Primitives/Popup.cs)
- [Avalonia VisualLayerManager](https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Controls/Primitives/VisualLayerManager.cs)
- Material Design Bottom Sheets
- iOS Action Sheets and Side Drawers
