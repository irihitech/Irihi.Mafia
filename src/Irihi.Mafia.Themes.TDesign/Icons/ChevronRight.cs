using Avalonia.Media;

namespace Irihi.Mafia.Themes.TDesign.Icons;

public class ChevronRight : TDesignIconBase
{
    private static readonly DrawingElement[]? StaticDrawingData =
    [
        new PathDrawingElement()
        {
            StrokeIndex = 0,
            InheritStrokeWidth = true,
            InheritStrokeCap = true,
            InheritStrokeJoin = true,
            Data = StreamGeometry.Parse(
                "M9.5 17.5L15 12L9.5 6.5"),
        },
    ];
    
    protected override DrawingElement[]? DrawingData => StaticDrawingData;
}