using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

namespace Irihi.Mirana.Controls;

/// <summary>
/// The host control for a drawer in the overlay layer.
/// This control manages the drawer's position, animation, and lifecycle.
/// </summary>
public class DrawerOverlayHost : ContentControl
{
    private readonly DrawerOverlayLayer _overlayLayer;
    private DrawerPlacement _placement = DrawerPlacement.Bottom;
    private Size _drawerSize;
    private bool _isAnimating;
    
    /// <summary>
    /// Defines the <see cref="Placement"/> property.
    /// </summary>
    public static readonly StyledProperty<DrawerPlacement> PlacementProperty =
        AvaloniaProperty.Register<DrawerOverlayHost, DrawerPlacement>(
            nameof(Placement), 
            DrawerPlacement.Bottom);
    
    /// <summary>
    /// Defines the <see cref="IsLightDismissEnabled"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsLightDismissEnabledProperty =
        AvaloniaProperty.Register<DrawerOverlayHost, bool>(
            nameof(IsLightDismissEnabled), 
            defaultValue: true);
    
    /// <summary>
    /// Gets or sets the placement of the drawer.
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
    /// Event raised when the drawer is closed.
    /// </summary>
    public event EventHandler<EventArgs>? Closed;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawerOverlayHost"/> class.
    /// </summary>
    /// <param name="overlayLayer">The overlay layer that hosts this drawer.</param>
    public DrawerOverlayHost(DrawerOverlayLayer overlayLayer)
    {
        _overlayLayer = overlayLayer ?? throw new ArgumentNullException(nameof(overlayLayer));
        
        // Set up event handlers for light dismiss
        this.AddHandler(PointerPressedEvent, OnOverlayPointerPressed, RoutingStrategies.Tunnel);
    }
    
    /// <summary>
    /// Sets the child control for the drawer.
    /// </summary>
    /// <param name="control">The control to display in the drawer.</param>
    public void SetChild(Control? control)
    {
        Content = control;
    }
    
    /// <summary>
    /// Shows the drawer with an animation.
    /// </summary>
    public void Show()
    {
        if (_overlayLayer.Children.Contains(this))
        {
            return;
        }
        
        _overlayLayer.Children.Add(this);
        
        // Force layout update to get the correct size
        if (Content is Visual { IsAttachedToVisualTree: false })
        {
            UpdateLayout();
        }
        
        // Start the slide-in animation
        AnimateIn();
    }
    
    /// <summary>
    /// Hides the drawer with an animation.
    /// </summary>
    public void Hide()
    {
        if (!_overlayLayer.Children.Contains(this))
        {
            return;
        }
        
        AnimateOut(() =>
        {
            _overlayLayer.Children.Remove(this);
            Closed?.Invoke(this, EventArgs.Empty);
        });
    }
    
    /// <summary>
    /// Disposes the drawer host.
    /// </summary>
    public void Dispose()
    {
        Hide();
    }
    
    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize)
    {
        _drawerSize = finalSize;
        UpdatePosition(finalSize);
        return base.ArrangeOverride(finalSize);
    }
    
    private void UpdatePosition(Size size)
    {
        var overlaySize = _overlayLayer.AvailableSize;
        
        switch (Placement)
        {
            case DrawerPlacement.Bottom:
                Canvas.SetLeft(this, 0);
                Canvas.SetTop(this, overlaySize.Height - size.Height);
                Width = overlaySize.Width;
                HorizontalAlignment = HorizontalAlignment.Stretch;
                VerticalAlignment = VerticalAlignment.Bottom;
                break;
                
            case DrawerPlacement.Top:
                Canvas.SetLeft(this, 0);
                Canvas.SetTop(this, 0);
                Width = overlaySize.Width;
                HorizontalAlignment = HorizontalAlignment.Stretch;
                VerticalAlignment = VerticalAlignment.Top;
                break;
                
            case DrawerPlacement.Left:
                Canvas.SetLeft(this, 0);
                Canvas.SetTop(this, 0);
                Height = overlaySize.Height;
                HorizontalAlignment = HorizontalAlignment.Left;
                VerticalAlignment = VerticalAlignment.Stretch;
                break;
                
            case DrawerPlacement.Right:
                Canvas.SetLeft(this, overlaySize.Width - size.Width);
                Canvas.SetTop(this, 0);
                Height = overlaySize.Height;
                HorizontalAlignment = HorizontalAlignment.Right;
                VerticalAlignment = VerticalAlignment.Stretch;
                break;
        }
    }
    
    private void AnimateIn()
    {
        if (_isAnimating) return;
        _isAnimating = true;
        
        var animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(250),
            Easing = new CubicEaseOut(),
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0),
                    Setters =
                    {
                        new Setter(OpacityProperty, 0.0),
                        new Setter(GetTransformProperty(), GetStartTransform())
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1),
                    Setters =
                    {
                        new Setter(OpacityProperty, 1.0),
                        new Setter(GetTransformProperty(), GetEndTransform())
                    }
                }
            }
        };
        
        animation.RunAsync(this).ContinueWith(_ => 
        {
            Dispatcher.UIThread.Post(() => _isAnimating = false);
        });
    }
    
    private void AnimateOut(Action onComplete)
    {
        if (_isAnimating) return;
        _isAnimating = true;
        
        var animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(200),
            Easing = new CubicEaseIn(),
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0),
                    Setters =
                    {
                        new Setter(OpacityProperty, 1.0),
                        new Setter(GetTransformProperty(), GetEndTransform())
                    }
                },
                new KeyFrame
                {
                    Cue = new Cue(1),
                    Setters =
                    {
                        new Setter(OpacityProperty, 0.0),
                        new Setter(GetTransformProperty(), GetStartTransform())
                    }
                }
            }
        };
        
        animation.RunAsync(this).ContinueWith(_ =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                _isAnimating = false;
                onComplete?.Invoke();
            });
        });
    }
    
    private Property GetTransformProperty()
    {
        return RenderTransformProperty;
    }
    
    private ITransform GetStartTransform()
    {
        var overlaySize = _overlayLayer.AvailableSize;
        
        return Placement switch
        {
            DrawerPlacement.Bottom => new TranslateTransform(0, _drawerSize.Height),
            DrawerPlacement.Top => new TranslateTransform(0, -_drawerSize.Height),
            DrawerPlacement.Left => new TranslateTransform(-_drawerSize.Width, 0),
            DrawerPlacement.Right => new TranslateTransform(_drawerSize.Width, 0),
            _ => new TranslateTransform(0, _drawerSize.Height)
        };
    }
    
    private ITransform GetEndTransform()
    {
        return new TranslateTransform(0, 0);
    }
    
    private void OnOverlayPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!IsLightDismissEnabled)
        {
            return;
        }
        
        // Check if the click is outside the content area
        var point = e.GetPosition(this);
        var contentBounds = Content?.Bounds;
        
        if (contentBounds.HasValue && !contentBounds.Value.Contains(point))
        {
            e.Handled = true;
            Hide();
        }
    }
}
