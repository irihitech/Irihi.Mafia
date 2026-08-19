namespace Irihi.Mafia.Controls;

public sealed class CalendarDayItem
{
    public CalendarDayItem(
        DateTime date,
        bool isCurrentMonth,
        bool isEnabled,
        bool isSelected,
        bool isInRange,
        bool isRangeStart,
        bool isRangeEnd,
        bool isToday)
    {
        Date = date.Date;
        Text = Date.Day.ToString();
        IsCurrentMonth = isCurrentMonth;
        IsEnabled = isEnabled;
        IsSelected = isSelected;
        IsInRange = isInRange;
        IsRangeStart = isRangeStart;
        IsRangeEnd = isRangeEnd;
        IsToday = isToday;
    }

    public DateTime Date { get; }

    public string Text { get; }

    public bool IsCurrentMonth { get; }

    public bool IsEnabled { get; }

    public bool IsSelected { get; }

    public bool IsInRange { get; }

    public bool IsRangeStart { get; }

    public bool IsRangeEnd { get; }

    public bool IsToday { get; }
}
