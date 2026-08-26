using System.Reflection;
using Irihi.Mafia.Common;
using Irihi.Mafia.Controls;

namespace Irihi.Mafia.UnitTest;

public class CalendarDatePickerTest
{
    [Fact]
    public void Default_Values_Are_Correct()
    {
        var picker = new CalendarDatePicker();

        Assert.False(picker.IsDropDownOpen);
        Assert.Null(picker.SelectedDate);
        Assert.Null(picker.PopupSelectedDate);
        Assert.Null(picker.SelectedDateText);
        Assert.Equal("yyyy-MM-dd", picker.DateFormat);
        Assert.Equal(CalendarDisplayMode.Paged, picker.DisplayMode);
        Assert.Equal("Confirm", picker.ConfirmButtonText);
        Assert.IsAssignableFrom<ICell>(picker);
    }

    [Fact]
    public void SelectedDate_Formats_SelectedDateText()
    {
        var picker = new CalendarDatePicker
        {
            DateFormat = "yyyy/MM/dd"
        };

        picker.SelectedDate = new DateTime(2026, 8, 21);

        Assert.Equal("2026/08/21", picker.SelectedDateText);
    }

    [Fact]
    public void Changing_SelectedDate_Does_Not_Close_Popup()
    {
        var picker = new CalendarDatePicker
        {
            IsDropDownOpen = true
        };

        picker.SelectedDate = new DateTime(2026, 8, 21);

        Assert.True(picker.IsDropDownOpen);
    }

    [Fact]
    public void SelectedDate_Syncs_PopupSelectedDate_When_Closed()
    {
        var picker = new CalendarDatePicker();

        picker.SelectedDate = new DateTime(2026, 9, 1);

        Assert.Equal(new DateTime(2026, 9, 1), picker.PopupSelectedDate);
    }

    [Fact]
    public void Closing_Without_Confirm_Does_Not_Commit_Staged_Value()
    {
        var picker = new CalendarDatePicker
        {
            SelectedDate = new DateTime(2026, 8, 21)
        };

        picker.IsDropDownOpen = true;
        picker.PopupSelectedDate = new DateTime(2026, 9, 1);
        picker.IsDropDownOpen = false;

        Assert.Equal(new DateTime(2026, 8, 21), picker.SelectedDate);
    }

    [Fact]
    public void Confirm_Commits_Staged_Value_And_Closes_Popup()
    {
        var picker = new CalendarDatePicker
        {
            SelectedDate = new DateTime(2026, 8, 21),
            IsDropDownOpen = true,
            PopupSelectedDate = new DateTime(2026, 9, 1)
        };

        var onConfirmClick = typeof(CalendarDatePicker).GetMethod("OnConfirmClick", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(onConfirmClick);

        onConfirmClick!.Invoke(picker, new object?[] { null, null });

        Assert.Equal(new DateTime(2026, 9, 1), picker.SelectedDate);
        Assert.False(picker.IsDropDownOpen);
    }

    [Fact]
    public void Click_Opens_Popup_When_Enabled()
    {
        var picker = new CalendarDatePicker();
        var onClick = typeof(CalendarDatePicker).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.NotNull(onClick);

        onClick!.Invoke(picker, null);

        Assert.True(picker.IsDropDownOpen);
    }
}
