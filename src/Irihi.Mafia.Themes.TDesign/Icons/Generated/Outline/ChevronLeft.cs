using Avalonia.Media;

namespace Irihi.Mafia.Themes.TDesign.Icons;

public class ChevronLeft : TDesignIconBase
{
    private static readonly DrawingElement[]? StaticDrawingData =
    [
        new PathDrawingElement()
        {
            StrokeIndex = 0,
            Data = StreamGeometry.Parse(
                "M14.5 17.5L9 12L14.5 6.5"),
        },
    ];

    protected override DrawingElement[]? DrawingData => StaticDrawingData;
}
