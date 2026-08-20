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
        Assert.Null(picker.SelectedDateText);
        Assert.Equal("yyyy-MM-dd", picker.DateFormat);
        Assert.Equal(CalendarDisplayMode.Paged, picker.DisplayMode);
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
    public void Selecting_Date_Closes_Popup()
    {
        var picker = new CalendarDatePicker
        {
            IsDropDownOpen = true
        };

        picker.SelectedDate = new DateTime(2026, 8, 21);

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
