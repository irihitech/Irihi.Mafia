using System.Globalization;
using Irihi.Avalonia.Shared.Converters;

namespace Irihi.Mirana.Themes.TDesign.Converters;

public class PositionToAngleConverter : MarkupValueConverter
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            return d * 3.6;
        }

        return 0;
    }

    public override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            return d / 3.6;
        }

        return 0;
    }
}