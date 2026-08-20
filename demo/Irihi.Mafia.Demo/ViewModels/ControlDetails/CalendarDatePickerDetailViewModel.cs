using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Irihi.Mafia.Demo.ViewModels.ControlDetails;

public partial class CalendarDatePickerDetailViewModel : NavigationViewModelBase
{
    [ObservableProperty]
    public partial DateTime? SelectedDate { get; set; } = DateTime.Today;

    [ObservableProperty]
    public partial DateTime? ScrollSelectedDate { get; set; } = DateTime.Today.AddDays(7);

    public string SelectedDateText => FormatDate(SelectedDate);

    public string ScrollSelectedDateText => FormatDate(ScrollSelectedDate);

    partial void OnSelectedDateChanged(DateTime? value) => OnPropertyChanged(nameof(SelectedDateText));

    partial void OnScrollSelectedDateChanged(DateTime? value) => OnPropertyChanged(nameof(ScrollSelectedDateText));

    private static string FormatDate(DateTime? date) => date?.ToString("yyyy-MM-dd") ?? "None";
}
