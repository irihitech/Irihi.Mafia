using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
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

    public IBrush? MaskBrush
    {
        get => GetValue(MaskBrushProperty);
        set => SetValue(MaskBrushProperty, value);
    }

    public event EventHandler<PointerPressedEventArgs>? MaskPointerPressed;

    static OverlayPopupHost()
    {
        PlacementProperty.Changed.AddClassHandler<OverlayPopupHost, PopupPlacement>(
            (o, e) => UpdatePlacementPseudoClasses(o, e.NewValue.Value));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (e.NameScope.Find<Border>(PART_MaskBorder) is { } maskBorder)
        {
            maskBorder.PointerPressed += OnMaskPointerPressed;
        }

        UpdatePlacementPseudoClasses(this, Placement);
    }

    private void OnMaskPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Source == sender)
        {
            MaskPointerPressed?.Invoke(this, e);
        }
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
