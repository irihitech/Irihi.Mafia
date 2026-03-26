using System.Globalization;
using Irihi.Avalonia.Shared.Converters;

namespace Irihi.Mafia.Themes.TDesign.Converters;

public class SwitchMovingLengthConverter : MarkupMultiValueConverter
{
    public override object? Convert(IList<object?>? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || value.Count != 3) return 0;
        if (value[0] is double borderWidth && value[1] is double knobWidth && value[2] is double knobSpacing)
        {
            return borderWidth - knobWidth - knobSpacing * 2;
        }

        return 0;
    }
}