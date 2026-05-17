using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Irihi.Avalonia.Shared.Converters;

namespace Irihi.Mafia.Themes.TDesign.Converters;

public class TabStripBorderThicknessConverter : MarkupMultiValueConverter
{
    public override object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is not [Dock dock, Thickness thickness])
            return new Thickness(0);

        return dock switch
        {
            Dock.Left => new Thickness(0, 0, thickness.Right, 0),
            Dock.Top => new Thickness(0, 0, 0, thickness.Bottom),
            Dock.Right => new Thickness(thickness.Left, 0, 0, 0),
            Dock.Bottom => new Thickness(0, thickness.Top, 0, 0),
            _ => new Thickness(0)
        };
    }
}
