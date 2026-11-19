using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Irihi.Mafia.Common;

namespace Irihi.Mafia.Controls;

/// <summary>
/// A text link control styled after TDesign Mobile's link component,
/// built on the built-in HyperlinkButton.
/// </summary>
[PseudoClasses(PC_Small, PC_Medium, PC_Large)]
public class Link : global::Avalonia.Controls.HyperlinkButton
{
    public const string PC_Small = ":small";
    public const string PC_Medium = ":medium";
    public const string PC_Large = ":large";

    /// <summary>
    /// Defines the <see cref="Size"/> property.
    /// </summary>
    public static readonly StyledProperty<ControlSize> SizeProperty =
        AvaloniaProperty.Register<Link, ControlSize>(
            nameof(Size), defaultValue: ControlSize.Medium);

    public ControlSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    static Link()
    {
        SizeProperty.Changed.AddClassHandler<Link, ControlSize>((o, e) => { UpdateSizePseudoClasses(o, e.NewValue.Value); });
    }

    public Link()
    {
        UpdateSizePseudoClasses(this, Size);
    }

    private static void UpdateSizePseudoClasses(Link link, ControlSize size)
    {
        IPseudoClasses pseudo = link.Classes;
        pseudo.Set(PC_Small, size == ControlSize.Small);
        pseudo.Set(PC_Medium, size == ControlSize.Medium);
        pseudo.Set(PC_Large, size == ControlSize.Large);
    }
}
