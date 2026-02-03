using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace Irihi.Mirana.Controls;

/// <summary>
/// A canvas-based overlay layer that hosts drawer controls.
/// Similar to Avalonia's OverlayLayer but specifically designed for drawers.
/// </summary>
public class DrawerOverlayLayer : Canvas
{
    /// <summary>
    /// Gets the available size for drawer placement.
    /// </summary>
    public Size AvailableSize { get; private set; }
    
    /// <summary>
    /// Gets the DrawerOverlayLayer for the specified visual element by traversing the visual tree.
    /// </summary>
    /// <param name="visual">The visual element to start searching from.</param>
    /// <returns>The DrawerOverlayLayer if found; otherwise, null.</returns>
    public static DrawerOverlayLayer? GetDrawerOverlayLayer(Visual visual)
    {
        // First, try to find it in visual ancestors
        foreach (var ancestor in visual.GetVisualAncestors())
        {
            if (ancestor is DrawerOverlayLayer layer)
            {
                return layer;
            }
        }
        
        // If not found in ancestors, check the TopLevel's visual descendants
        if (TopLevel.GetTopLevel(visual) is { } topLevel)
        {
            var descendants = topLevel.GetVisualDescendants();
            foreach (var descendant in descendants)
            {
                if (descendant is DrawerOverlayLayer layer)
                {
                    return layer;
                }
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Measures all child drawers with the available size.
    /// </summary>
    protected override Size MeasureOverride(Size availableSize)
    {
        foreach (Control child in Children)
        {
            child.Measure(availableSize);
        }
        return availableSize;
    }
    
    /// <summary>
    /// Arranges all child drawers and saves the available size.
    /// </summary>
    protected override Size ArrangeOverride(Size finalSize)
    {
        // Save the available size for child controls to reference
        AvailableSize = finalSize;
        return base.ArrangeOverride(finalSize);
    }
}
