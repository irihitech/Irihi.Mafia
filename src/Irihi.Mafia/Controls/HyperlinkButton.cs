using System;
using Avalonia;
using Avalonia.Controls;
using Irihi.Mafia.Common;

namespace Irihi.Mafia.Controls;

/// <summary>
/// A text link button styled after TDesign Mobile's link component.
/// Inherits the built-in <see cref="global::Avalonia.Controls.HyperlinkButton"/> theme via
/// <see cref="StyleKeyOverride"/>, adding a <see cref="Size"/> property that mirrors into
/// the Small/Medium/Large style classes.
/// </summary>
public class HyperlinkButton : global::Avalonia.Controls.HyperlinkButton
{
    public static readonly StyledProperty<ControlSize> SizeProperty =
        AvaloniaProperty.Register<HyperlinkButton, ControlSize>(
            nameof(Size), defaultValue: ControlSize.Medium);

    public ControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    static HyperlinkButton()
    {
        SizeProperty.Changed.AddClassHandler<HyperlinkButton, ControlSize>((o, e) => { UpdateSizeClasses(o, e.NewValue.Value); });
    }

    public HyperlinkButton()
    {
        UpdateSizeClasses(this, Size);
    }

    protected override Type StyleKeyOverride => typeof(global::Avalonia.Controls.HyperlinkButton);

    private static void UpdateSizeClasses(HyperlinkButton link, ControlSize size)
    {
        link.Classes.Set("Small", size == ControlSize.Small);
        link.Classes.Set("Medium", size == ControlSize.Medium);
        link.Classes.Set("Large", size == ControlSize.Large);
    }
}
