using Avalonia;
using Avalonia.Controls;

namespace Irihi.Mafia.Demo.Controls;

public class GalleryBlock : ContentControl
{
    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<GalleryBlock, string>(
        nameof(Title));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<string> SummaryProperty = AvaloniaProperty.Register<GalleryBlock, string>(
        nameof(Summary));

    public string Summary
    {
        get => GetValue(SummaryProperty);
        set => SetValue(SummaryProperty, value);
    }
}