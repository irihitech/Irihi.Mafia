using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;
using Irihi.Mafia.Common;

namespace Irihi.Mafia.Controls;

[TemplatePart(PART_Popup, typeof(Primitives.Popup), IsRequired = true)]
public class CalendarDatePicker : Cell, ICell
{
    public const string PART_Popup = "PART_Popup";

    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<CalendarDatePicker, bool>(
            nameof(IsDropDownOpen),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<DateTime?> SelectedDateProperty =
        AvaloniaProperty.Register<CalendarDatePicker, DateTime?>(
            nameof(SelectedDate),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<CalendarDatePicker, string?>(nameof(PlaceholderText));

    public static readonly StyledProperty<IBrush?> PlaceholderForegroundProperty =
        AvaloniaProperty.Register<CalendarDatePicker, IBrush?>(nameof(PlaceholderForeground));

    public static readonly StyledProperty<string> DateFormatProperty =
        AvaloniaProperty.Register<CalendarDatePicker, string>(nameof(DateFormat), "yyyy-MM-dd");

    public static readonly StyledProperty<string?> PopupTitleProperty =
        AvaloniaProperty.Register<CalendarDatePicker, string?>(nameof(PopupTitle));

    public static readonly StyledProperty<double> PopupMinHeightProperty =
        AvaloniaProperty.Register<CalendarDatePicker, double>(nameof(PopupMinHeight), 480d);

    public static readonly StyledProperty<double> PopupMaxHeightProperty =
        AvaloniaProperty.Register<CalendarDatePicker, double>(nameof(PopupMaxHeight), double.PositiveInfinity);

    public static readonly StyledProperty<CalendarDisplayMode> DisplayModeProperty =
        AvaloniaProperty.Register<CalendarDatePicker, CalendarDisplayMode>(nameof(DisplayMode), CalendarDisplayMode.Paged);

    public static readonly StyledProperty<DayOfWeek> FirstDayOfWeekProperty =
        AvaloniaProperty.Register<CalendarDatePicker, DayOfWeek>(
            nameof(FirstDayOfWeek),
            CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);

    public static readonly StyledProperty<DateTime?> MinDateProperty =
        AvaloniaProperty.Register<CalendarDatePicker, DateTime?>(nameof(MinDate));

    public static readonly StyledProperty<DateTime?> MaxDateProperty =
        AvaloniaProperty.Register<CalendarDatePicker, DateTime?>(nameof(MaxDate));

    public static readonly DirectProperty<CalendarDatePicker, string?> SelectedDateTextProperty =
        AvaloniaProperty.RegisterDirect<CalendarDatePicker, string?>(
            nameof(SelectedDateText),
            o => o.SelectedDateText);

    private string? _selectedDateText;

    static CalendarDatePicker()
    {
        SelectedDateProperty.Changed.AddClassHandler<CalendarDatePicker, DateTime?>(
            (o, e) => o.OnSelectedDateChanged(e.NewValue.Value));
        DateFormatProperty.Changed.AddClassHandler<CalendarDatePicker, string>((o, _) => o.UpdateSelectedDateText());
    }

    public CalendarDatePicker()
    {
        if (!Classes.Contains("Arrow"))
        {
            Classes.Add("Arrow");
        }

        UpdateSelectedDateText();
    }

    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    public DateTime? SelectedDate
    {
        get => GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public string? PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public IBrush? PlaceholderForeground
    {
        get => GetValue(PlaceholderForegroundProperty);
        set => SetValue(PlaceholderForegroundProperty, value);
    }

    public string DateFormat
    {
        get => GetValue(DateFormatProperty);
        set => SetValue(DateFormatProperty, value);
    }

    public string? PopupTitle
    {
        get => GetValue(PopupTitleProperty);
        set => SetValue(PopupTitleProperty, value);
    }

    public double PopupMinHeight
    {
        get => GetValue(PopupMinHeightProperty);
        set => SetValue(PopupMinHeightProperty, value);
    }

    public double PopupMaxHeight
    {
        get => GetValue(PopupMaxHeightProperty);
        set => SetValue(PopupMaxHeightProperty, value);
    }

    public CalendarDisplayMode DisplayMode
    {
        get => GetValue(DisplayModeProperty);
        set => SetValue(DisplayModeProperty, value);
    }

    public DayOfWeek FirstDayOfWeek
    {
        get => GetValue(FirstDayOfWeekProperty);
        set => SetValue(FirstDayOfWeekProperty, value);
    }

    public DateTime? MinDate
    {
        get => GetValue(MinDateProperty);
        set => SetValue(MinDateProperty, value);
    }

    public DateTime? MaxDate
    {
        get => GetValue(MaxDateProperty);
        set => SetValue(MaxDateProperty, value);
    }

    public string? SelectedDateText
    {
        get => _selectedDateText;
        private set => SetAndRaise(SelectedDateTextProperty, ref _selectedDateText, value);
    }

    protected override void OnClick()
    {
        base.OnClick();

        if (IsEnabled)
        {
            SetCurrentValue(IsDropDownOpenProperty, true);
        }
    }

    private void OnSelectedDateChanged(DateTime? date)
    {
        UpdateSelectedDateText();

        if (date is not null && IsDropDownOpen)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
        }
    }

    private void UpdateSelectedDateText()
    {
        if (SelectedDate is null)
        {
            SelectedDateText = null;
            return;
        }

        var format = string.IsNullOrWhiteSpace(DateFormat) ? "yyyy-MM-dd" : DateFormat;
        SelectedDateText = SelectedDate.Value.ToString(format, CultureInfo.CurrentCulture);
    }
}
