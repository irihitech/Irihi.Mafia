namespace Irihi.Mafia.Controls;

public sealed class CalendarMonthView
{
    public CalendarMonthView(DateTime month, string title, IReadOnlyList<CalendarDayItem> days)
    {
        Month = new DateTime(month.Year, month.Month, 1);
        Title = title;
        Days = days;
    }

    public DateTime Month { get; }

    public string Title { get; }

    public IReadOnlyList<CalendarDayItem> Days { get; }
}
