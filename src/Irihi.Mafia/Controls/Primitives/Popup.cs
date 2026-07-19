using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;
using Irihi.Mafia.Common;

namespace Irihi.Mafia.Controls.Primitives;

public class Popup : Control
{
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<Popup, bool>(nameof(IsOpen));

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public static readonly StyledProperty<Control?> ChildProperty =
        AvaloniaProperty.Register<Popup, Control?>(nameof(Child));

    public Control? Child
    {
        get => GetValue(ChildProperty);
        set => SetValue(ChildProperty, value);
    }

    public static readonly StyledProperty<bool> IsModalProperty =
        AvaloniaProperty.Register<Popup, bool>(nameof(IsModal), defaultValue: true);

    public bool IsModal
    {
        get => GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }

    public static readonly StyledProperty<IBrush?> MaskBrushProperty =
        AvaloniaProperty.Register<Popup, IBrush?>(nameof(MaskBrush));

    public IBrush? MaskBrush
    {
        get => GetValue(MaskBrushProperty);
        set => SetValue(MaskBrushProperty, value);
    }

    public static readonly StyledProperty<PopupPlacement> PlacementProperty =
        AvaloniaProperty.Register<Popup, PopupPlacement>(nameof(Placement), defaultValue: PopupPlacement.Bottom);

    public PopupPlacement Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    public static readonly StyledProperty<bool> IsLightDismissEnabledProperty =
        AvaloniaProperty.Register<Popup, bool>(nameof(IsLightDismissEnabled), defaultValue: true);

    public bool IsLightDismissEnabled
    {
        get => GetValue(IsLightDismissEnabledProperty);
        set => SetValue(IsLightDismissEnabledProperty, value);
    }

    public event EventHandler? Opened;
    public event EventHandler? Closed;
    public event EventHandler<CancelEventArgs>? Closing;

    private OverlayPopupHost? _host;
    private OverlayLayer? _overlayLayer;
    private IDisposable? _sizeSubscription;
    private int _zIndex;
    private bool _isOpenRequested;
    private bool _ignoreIsOpenChanged;

    static Popup()
    {
        IsHitTestVisibleProperty.OverrideDefaultValue<Popup>(false);
        IsOpenProperty.Changed.AddClassHandler<Popup, bool>((o, e) => o.OnIsOpenChanged(e));
        ChildProperty.Changed.AddClassHandler<Popup, Control?>((o, e) => o.OnChildChanged(e));
    }

    protected override Size MeasureCore(Size availableSize) => new();

    public void Open()
    {
        if (_host is not null) return;

        var overlayLayer = OverlayLayer.GetOverlayLayer(this);
        if (overlayLayer is null)
        {
            _isOpenRequested = true;
            return;
        }

        _overlayLayer = overlayLayer;

        var host = new OverlayPopupHost
        {
            Content = Child,
            [~IsModalProperty] = this[~IsModalProperty],
            [~MaskBrushProperty] = this[~MaskBrushProperty],
            [~OverlayPopupHost.PlacementProperty] = this[~PlacementProperty],
        };

        host.MaskPointerPressed += OnMaskPointerPressed;
        _host = host;

        UpdateHostSize();
        _zIndex = Interlocked.Increment(ref s_zIndexBase);
        Canvas.SetLeft(host, 0);
        Canvas.SetTop(host, 0);
        host.ZIndex = _zIndex;

        _overlayLayer.Children.Add(host);
        _overlayLayer.PropertyChanged += OnOverlayLayerBoundsChanged;

        _isOpenRequested = true;

        using (BeginIgnoringIsOpen())
        {
            SetCurrentValue(IsOpenProperty, true);
        }

        Opened?.Invoke(this, EventArgs.Empty);
    }

    public void Close()
    {
        if (_host is null) return;

        var closingArgs = new CancelEventArgs();
        Closing?.Invoke(this, closingArgs);
        if (closingArgs.Cancel) return;

        CleanupHost();

        _isOpenRequested = false;

        using (BeginIgnoringIsOpen())
        {
            SetCurrentValue(IsOpenProperty, false);
        }

        Closed?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_isOpenRequested) Open();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_host is not null) CleanupHost();
    }

    private void OnIsOpenChanged(AvaloniaPropertyChangedEventArgs<bool> e)
    {
        if (_ignoreIsOpenChanged) return;
        if (e.NewValue.Value)
            Open();
        else
            Close();
    }

    private void OnChildChanged(AvaloniaPropertyChangedEventArgs<Control?> e)
    {
        var oldValue = e.OldValue;
        var newValue = e.NewValue;

        if (oldValue.HasValue && oldValue.Value is { } oldChild)
        {
            ((ISetLogicalParent?)oldChild)?.SetParent(null);
            if (oldChild is ILogical logical)
                LogicalChildren.Remove(logical);
        }

        if (newValue.HasValue && newValue.Value is { } newChild)
        {
            ((ISetLogicalParent)newChild).SetParent(this);
            LogicalChildren.Add((ILogical)newChild);
        }

        if (_host is not null)
        {
            _host.Content = newValue.HasValue ? newValue.Value : null;
        }
    }

    private void OnMaskPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (IsLightDismissEnabled) Close();
    }

    private void CleanupHost()
    {
        if (_host is null) return;

        _host.MaskPointerPressed -= OnMaskPointerPressed;
        _sizeSubscription?.Dispose();
        _sizeSubscription = null;
        if (_overlayLayer is not null)
            _overlayLayer.PropertyChanged -= OnOverlayLayerBoundsChanged;

        _overlayLayer?.Children.Remove(_host);
        _host = null;
        _overlayLayer = null;
    }

    private void UpdateHostSize()
    {
        if (_host is null || _overlayLayer is null) return;
        _host.Width = _overlayLayer.Bounds.Width;
        _host.Height = _overlayLayer.Bounds.Height;
    }

    private IgnoreIsOpenScope BeginIgnoringIsOpen() => new(this);

    private void OnOverlayLayerBoundsChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == BoundsProperty) UpdateHostSize();
    }

    private static int s_zIndexBase = 1000;

    private readonly struct IgnoreIsOpenScope : IDisposable
    {
        private readonly Popup _owner;

        public IgnoreIsOpenScope(Popup owner)
        {
            _owner = owner;
            _owner._ignoreIsOpenChanged = true;
        }

        public void Dispose()
        {
            _owner._ignoreIsOpenChanged = false;
        }
    }
}
