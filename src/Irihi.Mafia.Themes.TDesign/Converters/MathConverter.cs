using System.Globalization;
using Irihi.Avalonia.Shared.Converters;

namespace Irihi.Mafia.Themes.TDesign.Converters;

public enum MathOperation
{
    Add,
    Subtract,
    Multiply,
    Divide,
}

public class MathConverter(MathOperation operation) : MarkupValueConverter
{
    public MathOperation Operation { get; set; } = operation;

    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double doubleValue && parameter is string parameterString && double.TryParse(parameterString, out var parameterValue))
        {
            return Operation switch
            {
                MathOperation.Add => doubleValue + parameterValue,
                MathOperation.Subtract => doubleValue - parameterValue,
                MathOperation.Multiply => doubleValue * parameterValue,
                MathOperation.Divide => parameterValue != 0 ? doubleValue / parameterValue : double.NaN,
                _ => doubleValue,
            };
        }
        return value;
    }
}