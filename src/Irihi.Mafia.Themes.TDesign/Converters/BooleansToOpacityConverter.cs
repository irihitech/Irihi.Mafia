using System.Globalization;
using Irihi.Avalonia.Shared.Converters;

namespace Irihi.Mafia.Themes.TDesign.Converters;

public class BooleansToOpacityConverter : MarkupValueConverter
{
    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return b ? 1 : 0;
        }

        return 1;
    }
}