using System;
using Avalonia.Controls.Primitives;

namespace Irihi.Mirana.Controls;

/// <summary>
/// A canvas-based overlay layer that hosts drawer controls.
/// This class is provided for backward compatibility. 
/// Drawers now use Avalonia's built-in OverlayLayer automatically.
/// </summary>
[Obsolete("DrawerOverlayLayer is no longer needed. Drawers automatically use Avalonia's built-in OverlayLayer.")]
public class DrawerOverlayLayer : OverlayLayer
{
    // This class now simply extends Avalonia's OverlayLayer for backward compatibility
    // No additional functionality is needed as drawers use the built-in overlay system
}
