using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Irihi.Mafia.Demo.ViewModels.ControlDetails;

public partial class CalendarDetailViewModel : NavigationViewModelBase
{
    private static readonly DateTime Today = DateTime.Today;

    [ObservableProperty]
    public partial DateTime? PagedSingleDate { get; set; } = Today;

    [ObservableProperty]
    public partial IReadOnlyList<DateTime>? MultipleDates { get; set; } =
    [
        Today,
        Today.AddDays(2),
        Today.AddDays(5)
    ];

    [ObservableProperty]
    public partial DateTime? RangeStart { get; set; } = Today.AddDays(-2);

    [ObservableProperty]
    public partial DateTime? RangeEnd { get; set; } = Today.AddDays(3);

    [ObservableProperty]
    public partial DateTime? ScrollRangeStart { get; set; } = Today.AddDays(12);

    [ObservableProperty]
    public partial DateTime? ScrollRangeEnd { get; set; } = Today.AddDays(18);

    public string PagedSingleDateText => PagedSingleDate?.ToString("yyyy-MM-dd") ?? "None";

    public string MultipleDatesText => MultipleDates is { Count: > 0 }
        ? string.Join(", ", MultipleDates.Select(x => x.ToString("MM-dd")))
        : "None";

    public string RangeText => FormatRange(RangeStart, RangeEnd);

    public string ScrollRangeText => FormatRange(ScrollRangeStart, ScrollRangeEnd);

    partial void OnPagedSingleDateChanged(DateTime? value) => OnPropertyChanged(nameof(PagedSingleDateText));

    partial void OnMultipleDatesChanged(IReadOnlyList<DateTime>? value) => OnPropertyChanged(nameof(MultipleDatesText));

    partial void OnRangeStartChanged(DateTime? value) => OnPropertyChanged(nameof(RangeText));

    partial void OnRangeEndChanged(DateTime? value) => OnPropertyChanged(nameof(RangeText));

    partial void OnScrollRangeStartChanged(DateTime? value) => OnPropertyChanged(nameof(ScrollRangeText));

    partial void OnScrollRangeEndChanged(DateTime? value) => OnPropertyChanged(nameof(ScrollRangeText));

    private static string FormatRange(DateTime? start, DateTime? end)
    {
        return start switch
        {
            null => "None",
            _ when end is null => $"{start:yyyy-MM-dd} - ...",
            _ => $"{start:yyyy-MM-dd} - {end:yyyy-MM-dd}"
        };
    }
}
