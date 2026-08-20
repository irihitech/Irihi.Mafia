using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Irihi.Mafia.Common;

namespace Irihi.Mafia.Controls.Primitives;

[TemplatePart(PART_MaskBorder, typeof(Border))]
[PseudoClasses(PC_Bottom, PC_Left, PC_Right, PC_Center, PC_Top, PC_FullScreen)]
public class OverlayPopupHost : ContentControl
{
    public const string PC_Bottom = ":bottom";
    public const string PC_Left = ":left";
    public const string PC_Right = ":right";
    public const string PC_Center = ":center";
    public const string PC_Top = ":top";
    public const string PC_FullScreen = ":fullscreen";
    public const string PART_MaskBorder = "PART_MaskBorder";

    public static readonly StyledProperty<Transform?> TransformProperty =
        PopupRoot.TransformProperty.AddOwner<OverlayPopupHost>();

    public static readonly StyledProperty<Thickness> SafeAreaPaddingProperty =
        AvaloniaProperty.Register<OverlayPopupHost, Thickness>(nameof(SafeAreaPadding));

    public static readonly StyledProperty<double> ContentOffsetXProperty =
        AvaloniaProperty.Register<OverlayPopupHost, double>(nameof(ContentOffsetX));

    public double ContentOffsetX
    {
        get => GetValue(ContentOffsetXProperty);
        set => SetValue(ContentOffsetXProperty, value);
    }

    public static readonly StyledProperty<double> ContentOffsetYProperty =
        AvaloniaProperty.Register<OverlayPopupHost, double>(nameof(ContentOffsetY));

    public double ContentOffsetY
    {
        get => GetValue(ContentOffsetYProperty);
        set => SetValue(ContentOffsetYProperty, value);
    }

    public static readonly StyledProperty<PopupPlacement> PlacementProperty =
        AvaloniaProperty.Register<OverlayPopupHost, PopupPlacement>(nameof(Placement), defaultValue: PopupPlacement.Bottom);

    public PopupPlacement Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    public static readonly StyledProperty<bool> IsModalProperty =
        AvaloniaProperty.Register<OverlayPopupHost, bool>(nameof(IsModal));

    public bool IsModal
    {
        get => GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }

    public static readonly StyledProperty<IBrush?> MaskBrushProperty =
        AvaloniaProperty.Register<OverlayPopupHost, IBrush?>(nameof(MaskBrush));

    private Border? _maskBorder;
    private readonly OverlayLayer? _overlayLayer;
    private Point _lastRequestedPosition;
    private Size _popupSize;
    private bool _needsPositionUpdate;

    public Transform? Transform
    {
        get => GetValue(TransformProperty);
        set => SetValue(TransformProperty, value);
    }

    public Thickness SafeAreaPadding
    {
        get => GetValue(SafeAreaPaddingProperty);
        set => SetValue(SafeAreaPaddingProperty, value);
    }

    public IBrush? MaskBrush
    {
        get => GetValue(MaskBrushProperty);
        set => SetValue(MaskBrushProperty, value);
    }

    public event EventHandler<PointerPressedEventArgs>? MaskPointerPressed;

    static OverlayPopupHost()
    {
        KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue<OverlayPopupHost>(KeyboardNavigationMode.Cycle);
        PlacementProperty.Changed.AddClassHandler<OverlayPopupHost, PopupPlacement>(
            (o, e) => UpdatePlacementPseudoClasses(o, e.NewValue.Value));
    }

    public OverlayPopupHost()
    {
    }

    internal OverlayPopupHost(OverlayLayer overlayLayer)
    {
        _overlayLayer = overlayLayer;
    }

    internal static OverlayPopupHost CreatePopupHost(Visual target)
    {
        if (OverlayLayer.GetOverlayLayer(target) is { } overlayLayer)
        {
            return new OverlayPopupHost(overlayLayer);
        }

        throw new InvalidOperationException("Unable to create overlay popup host: no overlay layer found for target.");
    }

    internal void SetChild(Control? control)
    {
        Content = control;
    }

    internal void Show()
    {
        if (_overlayLayer is null)
            return;

        if (Parent != _overlayLayer)
            _overlayLayer.Children.Add(this);
        _overlayLayer.PropertyChanged += OnOverlayLayerPropertyChanged;
        UpdateOverlayLayout();

        // Ensure descendants are built early so focus behavior matches popup root behavior.
        UpdateLayout();
    }

    internal void Hide()
    {
        if (_overlayLayer is null)
            return;

        _overlayLayer.PropertyChanged -= OnOverlayLayerPropertyChanged;
        _overlayLayer.Children.Remove(this);
    }

    internal void SetPosition(Point point)
    {
        _lastRequestedPosition = point;
        _needsPositionUpdate = true;
        UpdatePosition();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_maskBorder is not null)
            _maskBorder.PointerPressed -= OnMaskPointerPressed;

        _maskBorder = e.NameScope.Find<Border>(PART_MaskBorder);
        if (_maskBorder is not null)
        {
            _maskBorder.PointerPressed += OnMaskPointerPressed;
        }

        UpdatePlacementPseudoClasses(this, Placement);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateSafeAreaPadding();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (_overlayLayer is not null)
            _overlayLayer.PropertyChanged -= OnOverlayLayerPropertyChanged;

        base.OnDetachedFromVisualTree(e);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_popupSize != finalSize)
        {
            _popupSize = finalSize;
            UpdateSafeAreaPadding();
        }

        UpdatePosition();
        return base.ArrangeOverride(finalSize);
    }

    private void OnMaskPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Source == sender)
        {
            MaskPointerPressed?.Invoke(this, e);
        }
    }

    private void UpdatePosition()
    {
        if (!_needsPositionUpdate)
            return;

        _needsPositionUpdate = false;
        Dispatcher.UIThread.Post(() =>
        {
            Canvas.SetLeft(this, _lastRequestedPosition.X);
            Canvas.SetTop(this, _lastRequestedPosition.Y);
        }, DispatcherPriority.Render);
    }

    private void UpdateSafeAreaPadding()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        SetCurrentValue(SafeAreaPaddingProperty, topLevel?.InsetsManager?.SafeAreaPadding ?? default);
    }

    private void OnOverlayLayerPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == BoundsProperty)
            UpdateOverlayLayout();
    }

    private void UpdateOverlayLayout()
    {
        if (_overlayLayer is null)
            return;

        SetCurrentValue(WidthProperty, _overlayLayer.Bounds.Width);
        SetCurrentValue(HeightProperty, _overlayLayer.Bounds.Height);
        SetPosition(default);
    }

    private static void UpdatePlacementPseudoClasses(OverlayPopupHost host, PopupPlacement placement)
    {
        var pseudo = (IPseudoClasses)host.Classes;
        pseudo.Set(PC_Bottom, placement == PopupPlacement.Bottom);
        pseudo.Set(PC_Left, placement == PopupPlacement.Left);
        pseudo.Set(PC_Right, placement == PopupPlacement.Right);
        pseudo.Set(PC_Center, placement == PopupPlacement.Center);
        pseudo.Set(PC_Top, placement == PopupPlacement.Top);
        pseudo.Set(PC_FullScreen, placement == PopupPlacement.FullScreen);
    }
}
