using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace Irihi.Mirana.Controls;

/// <summary>
/// A drawer control that displays content in an overlay layer, sliding in from a screen edge.
/// Similar to Avalonia's Popup but specifically designed for drawer-style UI patterns common in mobile applications.
/// </summary>
public class Drawer : Control
{
    /// <summary>
    /// Defines the <see cref="Child"/> property.
    /// </summary>
    public static readonly StyledProperty<Control?> ChildProperty =
        AvaloniaProperty.Register<Drawer, Control?>(nameof(Child));
    
    /// <summary>
    /// Defines the <see cref="IsOpen"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<Drawer, bool>(nameof(IsOpen), defaultValue: false);
    
    /// <summary>
    /// Defines the <see cref="Placement"/> property.
    /// </summary>
    public static readonly StyledProperty<DrawerPlacement> PlacementProperty =
        AvaloniaProperty.Register<Drawer, DrawerPlacement>(
            nameof(Placement), 
            defaultValue: DrawerPlacement.Bottom);
    
    /// <summary>
    /// Defines the <see cref="IsLightDismissEnabled"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsLightDismissEnabledProperty =
        AvaloniaProperty.Register<Drawer, bool>(
            nameof(IsLightDismissEnabled), 
            defaultValue: true);
    
    /// <summary>
    /// Defines the <see cref="IsModal"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsModalProperty =
        AvaloniaProperty.Register<Drawer, bool>(
            nameof(IsModal), 
            defaultValue: true);
    
    private DrawerOverlayHost? _host;
    private bool _isOpenRequested;
    
    static Drawer()
    {
        IsOpenProperty.Changed.AddClassHandler<Drawer>((x, e) => x.OnIsOpenChanged(e));
        IsHitTestVisibleProperty.OverrideDefaultValue<Drawer>(false);
    }
    
    /// <summary>
    /// Raised when the drawer is opened.
    /// </summary>
    public event EventHandler? Opened;
    
    /// <summary>
    /// Raised when the drawer is closed.
    /// </summary>
    public event EventHandler<EventArgs>? Closed;
    
    /// <summary>
    /// Gets or sets the control to display in the drawer.
    /// </summary>
    [Content]
    public Control? Child
    {
        get => GetValue(ChildProperty);
        set => SetValue(ChildProperty, value);
    }
    
    /// <summary>
    /// Gets or sets a value indicating whether the drawer is currently open.
    /// </summary>
    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the placement of the drawer (which screen edge it slides from).
    /// </summary>
    public DrawerPlacement Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }
    
    /// <summary>
    /// Gets or sets whether the drawer can be dismissed by clicking outside of it.
    /// </summary>
    public bool IsLightDismissEnabled
    {
        get => GetValue(IsLightDismissEnabledProperty);
        set => SetValue(IsLightDismissEnabledProperty, value);
    }
    
    /// <summary>
    /// Gets or sets whether the drawer is modal (blocks interaction with content behind it).
    /// </summary>
    public bool IsModal
    {
        get => GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }
    
    /// <summary>
    /// Gets the current drawer overlay host, if the drawer is open.
    /// </summary>
    public DrawerOverlayHost? Host => _host;
    
    /// <summary>
    /// Opens the drawer.
    /// </summary>
    public void Open()
    {
        if (_host != null)
        {
            return; // Already open
        }
        
        // Find the overlay layer
        var overlayLayer = FindOverlayLayer();
        if (overlayLayer == null)
        {
            _isOpenRequested = true;
            return;
        }
        
        _isOpenRequested = false;
        
        // Create the host
        _host = new DrawerOverlayHost(overlayLayer)
        {
            Placement = Placement,
            IsLightDismissEnabled = IsLightDismissEnabled
        };
        
        _host.SetChild(Child);
        ((ISetLogicalParent)_host).SetParent(this);
        
        // Subscribe to closed event
        _host.Closed += OnHostClosed;
        
        // Show the drawer
        _host.Show();
        
        Opened?.Invoke(this, EventArgs.Empty);
    }
    
    /// <summary>
    /// Closes the drawer.
    /// </summary>
    public void Close()
    {
        if (_host == null)
        {
            return; // Already closed
        }
        
        _host.Hide();
    }
    
    /// <inheritdoc />
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        
        // If IsOpen was requested before we were attached, open now
        if (_isOpenRequested && IsOpen)
        {
            Open();
        }
    }
    
    /// <inheritdoc />
    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        
        // Close the drawer if it's open
        if (_host != null)
        {
            _host.Dispose();
            _host = null;
        }
    }
    
    private void OnIsOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var isOpen = (bool)e.NewValue!;
        
        if (isOpen)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
    
    private void OnHostClosed(object? sender, EventArgs e)
    {
        if (_host != null)
        {
            _host.Closed -= OnHostClosed;
            ((ISetLogicalParent)_host).SetParent(null);
            _host = null;
        }
        
        // Update IsOpen property
        IsOpen = false;
        
        Closed?.Invoke(this, EventArgs.Empty);
    }
    
    private OverlayLayer? FindOverlayLayer()
    {
        // Try to find Avalonia's built-in overlay layer from this control
        if (this.FindLogicalAncestorOfType<Control>() is { } parent)
        {
            if (TopLevel.GetTopLevel(parent) is { } topLevel)
            {
                return OverlayLayer.GetOverlayLayer(topLevel);
            }
        }
        
        return null;
    }
}
