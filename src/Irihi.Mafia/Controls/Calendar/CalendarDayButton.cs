using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace Irihi.Mafia.Controls;

[PseudoClasses(PC_Selected, PC_Inactive, PC_Today, PC_InRange, PC_RangeStart, PC_RangeEnd)]
public class CalendarDayButton : Button
{
    public const string PC_Selected = ":selected";
    public const string PC_Inactive = ":inactive";
    public const string PC_Today = ":today";
    public const string PC_InRange = ":in-range";
    public const string PC_RangeStart = ":range-start";
    public const string PC_RangeEnd = ":range-end";

    public static readonly StyledProperty<DateTime> DateProperty =
        AvaloniaProperty.Register<CalendarDayButton, DateTime>(nameof(Date));

    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<CalendarDayButton, bool>(nameof(IsSelected));

    public static readonly StyledProperty<bool> IsInactiveProperty =
        AvaloniaProperty.Register<CalendarDayButton, bool>(nameof(IsInactive));

    public static readonly StyledProperty<bool> IsTodayProperty =
        AvaloniaProperty.Register<CalendarDayButton, bool>(nameof(IsToday));

    public static readonly StyledProperty<bool> IsInRangeProperty =
        AvaloniaProperty.Register<CalendarDayButton, bool>(nameof(IsInRange));

    public static readonly StyledProperty<bool> IsRangeStartProperty =
        AvaloniaProperty.Register<CalendarDayButton, bool>(nameof(IsRangeStart));

    public static readonly StyledProperty<bool> IsRangeEndProperty =
        AvaloniaProperty.Register<CalendarDayButton, bool>(nameof(IsRangeEnd));

    static CalendarDayButton()
    {
        IsSelectedProperty.Changed.AddClassHandler<CalendarDayButton, bool>((o, e) => o.UpdatePseudoClass(PC_Selected, e.NewValue.Value));
        IsInactiveProperty.Changed.AddClassHandler<CalendarDayButton, bool>((o, e) => o.UpdatePseudoClass(PC_Inactive, e.NewValue.Value));
        IsTodayProperty.Changed.AddClassHandler<CalendarDayButton, bool>((o, e) => o.UpdatePseudoClass(PC_Today, e.NewValue.Value));
        IsInRangeProperty.Changed.AddClassHandler<CalendarDayButton, bool>((o, e) => o.UpdatePseudoClass(PC_InRange, e.NewValue.Value));
        IsRangeStartProperty.Changed.AddClassHandler<CalendarDayButton, bool>((o, e) => o.UpdatePseudoClass(PC_RangeStart, e.NewValue.Value));
        IsRangeEndProperty.Changed.AddClassHandler<CalendarDayButton, bool>((o, e) => o.UpdatePseudoClass(PC_RangeEnd, e.NewValue.Value));
        FocusableProperty.OverrideDefaultValue<CalendarDayButton>(false);
    }

    public DateTime Date
    {
        get => GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public bool IsInactive
    {
        get => GetValue(IsInactiveProperty);
        set => SetValue(IsInactiveProperty, value);
    }

    public bool IsToday
    {
        get => GetValue(IsTodayProperty);
        set => SetValue(IsTodayProperty, value);
    }

    public bool IsInRange
    {
        get => GetValue(IsInRangeProperty);
        set => SetValue(IsInRangeProperty, value);
    }

    public bool IsRangeStart
    {
        get => GetValue(IsRangeStartProperty);
        set => SetValue(IsRangeStartProperty, value);
    }

    public bool IsRangeEnd
    {
        get => GetValue(IsRangeEndProperty);
        set => SetValue(IsRangeEndProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
    }

    private void UpdatePseudoClass(string pseudoClass, bool value)
    {
        PseudoClasses.Set(pseudoClass, value);
    }

    private void UpdatePseudoClasses()
    {
        UpdatePseudoClass(PC_Selected, IsSelected);
        UpdatePseudoClass(PC_Inactive, IsInactive);
        UpdatePseudoClass(PC_Today, IsToday);
        UpdatePseudoClass(PC_InRange, IsInRange);
        UpdatePseudoClass(PC_RangeStart, IsRangeStart);
        UpdatePseudoClass(PC_RangeEnd, IsRangeEnd);
    }
}
