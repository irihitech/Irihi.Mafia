using System.Globalization;
using Avalonia.Data;
using Irihi.Avalonia.Shared.Converters;

namespace Irihi.Mafia.Themes.TDesign.Converters;

public class TabbedPageTabItemFontSizeConverter : MarkupMultiValueConverter
{
    public override object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 3 && values[1] is not double && values[2] is not double)
        {
            return BindingOperations.DoNothing;
        }

        return values[0] is not null ? values[2] : values[1];
    }
}
