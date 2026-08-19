using System.Collections;
using Irihi.Mafia.Common;
using Irihi.Mafia.Controls;

namespace Irihi.Mafia.UnitTest;

public class CalendarTest
{
    [Fact]
    public void Default_Mode_And_Grid_Are_Initialized()
    {
        var calendar = new Calendar();
        var visibleMonths = calendar.VisibleMonths.Cast<CalendarMonthView>().ToArray();

        Assert.Equal(CalendarDisplayMode.Paged, calendar.DisplayMode);
        Assert.Equal(CalendarSelectionMode.Single, calendar.SelectionMode);
        Assert.Single(visibleMonths);
        Assert.Equal(42, visibleMonths[0].Days.Count);
    }

    [Fact]
    public void Single_Selection_Updates_SelectedDate()
    {
        var calendar = new Calendar
        {
            SelectionMode = CalendarSelectionMode.Single
        };

        calendar.SelectDateCommand.Execute(new DateTime(2026, 8, 19, 21, 45, 0));

        Assert.Equal(new DateTime(2026, 8, 19), calendar.SelectedDate);
        Assert.Empty(calendar.SelectedDates ?? []);
        Assert.Null(calendar.RangeStart);
        Assert.Null(calendar.RangeEnd);
        Assert.Single(calendar.VisibleMonths.Cast<CalendarMonthView>().Single().Days, x => x.IsSelected);
    }

    [Fact]
    public void Multiple_Selection_Toggles_Dates()
    {
        var calendar = new Calendar
        {
            SelectionMode = CalendarSelectionMode.Multiple
        };

        var first = new DateTime(2026, 8, 10);
        var second = new DateTime(2026, 8, 12);

        calendar.SelectDateCommand.Execute(first);
        calendar.SelectDateCommand.Execute(second);
        calendar.SelectDateCommand.Execute(first);

        Assert.Null(calendar.SelectedDate);
        Assert.Null(calendar.RangeStart);
        Assert.Null(calendar.RangeEnd);
        Assert.Equal([second], calendar.SelectedDates);
        Assert.Single(calendar.VisibleMonths.Cast<CalendarMonthView>().Single().Days, x => x.IsSelected);
    }

    [Fact]
    public void Range_Selection_Completes_And_Restarts()
    {
        var calendar = new Calendar
        {
            SelectionMode = CalendarSelectionMode.Range
        };

        var start = new DateTime(2026, 8, 8);
        var end = new DateTime(2026, 8, 12);
        var restart = new DateTime(2026, 8, 20);

        calendar.SelectDateCommand.Execute(start);
        calendar.SelectDateCommand.Execute(end);

        Assert.Equal(start, calendar.RangeStart);
        Assert.Equal(end, calendar.RangeEnd);
        Assert.Equal(5, calendar.VisibleMonths.Cast<CalendarMonthView>().Single().Days.Count(x => x.IsInRange));

        calendar.SelectDateCommand.Execute(restart);

        Assert.Equal(restart, calendar.RangeStart);
        Assert.Null(calendar.RangeEnd);
        Assert.Single(calendar.VisibleMonths.Cast<CalendarMonthView>().Single().Days, x => x.IsSelected);
    }

    [Fact]
    public void Month_Commands_Update_DisplayDate()
    {
        var calendar = new Calendar
        {
            DisplayDate = new DateTime(2026, 8, 15)
        };

        calendar.NextMonthCommand.Execute(null);
        Assert.Equal(new DateTime(2026, 9, 1), calendar.DisplayDate);

        calendar.PreviousMonthCommand.Execute(null);
        Assert.Equal(new DateTime(2026, 8, 1), calendar.DisplayDate);
    }

    [Fact]
    public void Scroll_Mode_Uses_Virtualized_Month_Source()
    {
        var calendar = new Calendar
        {
            DisplayMode = CalendarDisplayMode.Scroll,
            ScrollMonthBuffer = 12
        };

        var visibleMonths = calendar.VisibleMonths.Cast<CalendarMonthView>().ToArray();

        Assert.Equal(25, visibleMonths.Length);
        Assert.Equal(new DateTime(calendar.DisplayDate.Year, calendar.DisplayDate.Month, 1), visibleMonths[12].Month);
        Assert.Equal(42, visibleMonths[12].Days.Count);
    }

    [Fact]
    public void Scroll_Mode_Keeps_List_Source_When_Selecting()
    {
        var calendar = new Calendar
        {
            DisplayMode = CalendarDisplayMode.Scroll,
            SelectionMode = CalendarSelectionMode.Range
        };

        var source = calendar.VisibleMonths;

        calendar.SelectDateCommand.Execute(new DateTime(2026, 8, 10));
        calendar.SelectDateCommand.Execute(new DateTime(2026, 8, 15));

        Assert.Same(source, calendar.VisibleMonths);
        Assert.IsAssignableFrom<IList>(calendar.VisibleMonths);
        Assert.Contains(
            calendar.VisibleMonths.Cast<CalendarMonthView>().SelectMany(x => x.Days),
            x => x.Date == new DateTime(2026, 8, 10) && x.IsRangeStart);
    }
}
