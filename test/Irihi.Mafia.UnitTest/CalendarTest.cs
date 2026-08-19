using Irihi.Mafia.Common;
using Irihi.Mafia.Controls;

namespace Irihi.Mafia.UnitTest;

public class CalendarTest
{
    [Fact]
    public void Default_Mode_And_Grid_Are_Initialized()
    {
        var calendar = new Calendar();

        Assert.Equal(CalendarDisplayMode.Paged, calendar.DisplayMode);
        Assert.Equal(CalendarSelectionMode.Single, calendar.SelectionMode);
        Assert.Equal(42, calendar.PagedDays.Count);
        Assert.Equal(25, calendar.ScrollMonths.Count);
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
        Assert.Single(calendar.PagedDays, x => x.IsSelected);
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
        Assert.Single(calendar.PagedDays, x => x.IsSelected);
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
        Assert.Equal(5, calendar.PagedDays.Count(x => x.IsInRange));

        calendar.SelectDateCommand.Execute(restart);

        Assert.Equal(restart, calendar.RangeStart);
        Assert.Null(calendar.RangeEnd);
        Assert.Single(calendar.PagedDays, x => x.IsSelected);
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
}
