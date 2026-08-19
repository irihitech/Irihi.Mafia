using System.Globalization;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Irihi.Mafia.Common;
using CalendarDisplayMode = Irihi.Mafia.Common.CalendarDisplayMode;
using CalendarSelectionMode = Irihi.Mafia.Common.CalendarSelectionMode;

namespace Irihi.Mafia.Controls;

[TemplatePart(PART_PagedHost, typeof(Control))]
[TemplatePart(PART_ScrollViewer, typeof(ScrollViewer))]
[PseudoClasses(PC_Paged, PC_Scroll, PC_Single, PC_Multiple, PC_Range)]
public class Calendar : TemplatedControl
{
    public const string PART_PagedHost = "PART_PagedHost";
    public const string PART_ScrollViewer = "PART_ScrollViewer";

    public const string PC_Paged = ":paged";
    public const string PC_Scroll = ":scroll";
    public const string PC_Single = ":single";
    public const string PC_Multiple = ":multiple";
    public const string PC_Range = ":range";

    private const double SwipeThreshold = 56;

    public static readonly StyledProperty<DateTime> DisplayDateProperty =
        AvaloniaProperty.Register<Calendar, DateTime>(
            nameof(DisplayDate),
            NormalizeMonth(DateTime.Today),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<CalendarDisplayMode> DisplayModeProperty =
        AvaloniaProperty.Register<Calendar, CalendarDisplayMode>(nameof(DisplayMode), CalendarDisplayMode.Paged);

    public static readonly StyledProperty<CalendarSelectionMode> SelectionModeProperty =
        AvaloniaProperty.Register<Calendar, CalendarSelectionMode>(nameof(SelectionMode), CalendarSelectionMode.Single);

    public static readonly StyledProperty<DateTime?> SelectedDateProperty =
        AvaloniaProperty.Register<Calendar, DateTime?>(
            nameof(SelectedDate),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<IReadOnlyList<DateTime>?> SelectedDatesProperty =
        AvaloniaProperty.Register<Calendar, IReadOnlyList<DateTime>?>(
            nameof(SelectedDates),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<DateTime?> RangeStartProperty =
        AvaloniaProperty.Register<Calendar, DateTime?>(
            nameof(RangeStart),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<DateTime?> RangeEndProperty =
        AvaloniaProperty.Register<Calendar, DateTime?>(
            nameof(RangeEnd),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<DayOfWeek> FirstDayOfWeekProperty =
        AvaloniaProperty.Register<Calendar, DayOfWeek>(
            nameof(FirstDayOfWeek),
            CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);

    public static readonly StyledProperty<DateTime?> MinDateProperty =
        AvaloniaProperty.Register<Calendar, DateTime?>(nameof(MinDate));

    public static readonly StyledProperty<DateTime?> MaxDateProperty =
        AvaloniaProperty.Register<Calendar, DateTime?>(nameof(MaxDate));

    public static readonly StyledProperty<int> ScrollMonthBufferProperty =
        AvaloniaProperty.Register<Calendar, int>(nameof(ScrollMonthBuffer), 12);

    public static readonly DirectProperty<Calendar, string> DisplayedMonthTitleProperty =
        AvaloniaProperty.RegisterDirect<Calendar, string>(
            nameof(DisplayedMonthTitle),
            o => o.DisplayedMonthTitle);

    public static readonly DirectProperty<Calendar, IReadOnlyList<string>> DayOfWeekHeadersProperty =
        AvaloniaProperty.RegisterDirect<Calendar, IReadOnlyList<string>>(
            nameof(DayOfWeekHeaders),
            o => o.DayOfWeekHeaders);

    public static readonly DirectProperty<Calendar, IReadOnlyList<CalendarDayItem>> PagedDaysProperty =
        AvaloniaProperty.RegisterDirect<Calendar, IReadOnlyList<CalendarDayItem>>(
            nameof(PagedDays),
            o => o.PagedDays);

    public static readonly DirectProperty<Calendar, IReadOnlyList<CalendarMonthView>> ScrollMonthsProperty =
        AvaloniaProperty.RegisterDirect<Calendar, IReadOnlyList<CalendarMonthView>>(
            nameof(ScrollMonths),
            o => o.ScrollMonths);

    private readonly ICommand _previousMonthCommand;
    private readonly ICommand _nextMonthCommand;
    private readonly ICommand _selectDateCommand;

    private string _displayedMonthTitle = string.Empty;
    private IReadOnlyList<string> _dayOfWeekHeaders = Array.Empty<string>();
    private IReadOnlyList<CalendarDayItem> _pagedDays = Array.Empty<CalendarDayItem>();
    private IReadOnlyList<CalendarMonthView> _scrollMonths = Array.Empty<CalendarMonthView>();

    private bool _isUpdatingSelection;
    private Control? _pagedHost;
    private ScrollViewer? _scrollViewer;
    private Point? _swipeStart;
    private DateTime? _rangeAnchor;

    static Calendar()
    {
        FocusableProperty.OverrideDefaultValue<Calendar>(false);

        DisplayDateProperty.Changed.AddClassHandler<Calendar, DateTime>((o, e) => o.OnDisplayDateChanged(e.NewValue.Value));
        DisplayModeProperty.Changed.AddClassHandler<Calendar, CalendarDisplayMode>((o, e) => o.OnDisplayModeChanged(e.NewValue.Value));
        SelectionModeProperty.Changed.AddClassHandler<Calendar, CalendarSelectionMode>((o, e) => o.OnSelectionModeChanged(e.NewValue.Value));
        SelectedDateProperty.Changed.AddClassHandler<Calendar, DateTime?>((o, e) => o.OnSelectedDateChanged(e.NewValue.Value));
        SelectedDatesProperty.Changed.AddClassHandler<Calendar, IReadOnlyList<DateTime>?>((o, e) => o.OnSelectedDatesChanged(e.NewValue.Value));
        RangeStartProperty.Changed.AddClassHandler<Calendar, DateTime?>((o, e) => o.OnRangeBoundaryChanged(e.Property, e.NewValue.Value));
        RangeEndProperty.Changed.AddClassHandler<Calendar, DateTime?>((o, e) => o.OnRangeBoundaryChanged(e.Property, e.NewValue.Value));
        FirstDayOfWeekProperty.Changed.AddClassHandler<Calendar, DayOfWeek>((o, _) => o.RefreshCalendar());
        MinDateProperty.Changed.AddClassHandler<Calendar, DateTime?>((o, _) => o.RefreshCalendar());
        MaxDateProperty.Changed.AddClassHandler<Calendar, DateTime?>((o, _) => o.RefreshCalendar());
        ScrollMonthBufferProperty.Changed.AddClassHandler<Calendar, int>((o, e) => o.OnScrollMonthBufferChanged(e.NewValue.Value));
    }

    public Calendar()
    {
        _previousMonthCommand = new ActionCommand(_ => MoveMonth(-1));
        _nextMonthCommand = new ActionCommand(_ => MoveMonth(1));
        _selectDateCommand = new ActionCommand(SelectDateFromCommand);

        UpdateDisplayModePseudoClasses(DisplayMode);
        UpdateSelectionModePseudoClasses(SelectionMode);
        RefreshCalendar();
    }

    public DateTime DisplayDate
    {
        get => GetValue(DisplayDateProperty);
        set => SetValue(DisplayDateProperty, value);
    }

    public CalendarDisplayMode DisplayMode
    {
        get => GetValue(DisplayModeProperty);
        set => SetValue(DisplayModeProperty, value);
    }

    public CalendarSelectionMode SelectionMode
    {
        get => GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    public DateTime? SelectedDate
    {
        get => GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public IReadOnlyList<DateTime>? SelectedDates
    {
        get => GetValue(SelectedDatesProperty);
        set => SetValue(SelectedDatesProperty, value);
    }

    public DateTime? RangeStart
    {
        get => GetValue(RangeStartProperty);
        set => SetValue(RangeStartProperty, value);
    }

    public DateTime? RangeEnd
    {
        get => GetValue(RangeEndProperty);
        set => SetValue(RangeEndProperty, value);
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

    public int ScrollMonthBuffer
    {
        get => GetValue(ScrollMonthBufferProperty);
        set => SetValue(ScrollMonthBufferProperty, value);
    }

    public string DisplayedMonthTitle
    {
        get => _displayedMonthTitle;
        private set => SetAndRaise(DisplayedMonthTitleProperty, ref _displayedMonthTitle, value);
    }

    public IReadOnlyList<string> DayOfWeekHeaders
    {
        get => _dayOfWeekHeaders;
        private set => SetAndRaise(DayOfWeekHeadersProperty, ref _dayOfWeekHeaders, value);
    }

    public IReadOnlyList<CalendarDayItem> PagedDays
    {
        get => _pagedDays;
        private set => SetAndRaise(PagedDaysProperty, ref _pagedDays, value);
    }

    public IReadOnlyList<CalendarMonthView> ScrollMonths
    {
        get => _scrollMonths;
        private set => SetAndRaise(ScrollMonthsProperty, ref _scrollMonths, value);
    }

    public ICommand PreviousMonthCommand => _previousMonthCommand;

    public ICommand NextMonthCommand => _nextMonthCommand;

    public ICommand SelectDateCommand => _selectDateCommand;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_pagedHost is not null)
        {
            _pagedHost.PointerPressed -= OnPagedHostPointerPressed;
            _pagedHost.PointerReleased -= OnPagedHostPointerReleased;
            _pagedHost.PointerCaptureLost -= OnPagedHostPointerCaptureLost;
        }

        _pagedHost = e.NameScope.Find<Control>(PART_PagedHost);
        if (_pagedHost is not null)
        {
            _pagedHost.PointerPressed += OnPagedHostPointerPressed;
            _pagedHost.PointerReleased += OnPagedHostPointerReleased;
            _pagedHost.PointerCaptureLost += OnPagedHostPointerCaptureLost;
        }

        _scrollViewer = e.NameScope.Find<ScrollViewer>(PART_ScrollViewer);
        ScrollHomeMonthIntoView();
    }

    private void OnDisplayDateChanged(DateTime value)
    {
        var normalized = NormalizeMonth(value);
        if (normalized != value)
        {
            SetCurrentValue(DisplayDateProperty, normalized);
            return;
        }

        RefreshCalendar();
        ScrollHomeMonthIntoView();
    }

    private void OnDisplayModeChanged(CalendarDisplayMode value)
    {
        UpdateDisplayModePseudoClasses(value);
        RefreshCalendar();
        ScrollHomeMonthIntoView();
    }

    private void OnSelectionModeChanged(CalendarSelectionMode value)
    {
        UpdateSelectionModePseudoClasses(value);
        CoerceSelectionForMode();
        RefreshCalendar();
    }

    private void OnSelectedDateChanged(DateTime? value)
    {
        if (_isUpdatingSelection)
        {
            return;
        }

        var normalized = value?.Date;
        if (normalized != value)
        {
            SetCurrentValue(SelectedDateProperty, normalized);
            return;
        }

        RefreshCalendar();
    }

    private void OnSelectedDatesChanged(IReadOnlyList<DateTime>? value)
    {
        if (_isUpdatingSelection)
        {
            return;
        }

        var normalized = NormalizeDateList(value);
        if (!AreDatesEqual(value, normalized))
        {
            SetCurrentValue(SelectedDatesProperty, normalized);
            return;
        }

        RefreshCalendar();
    }

    private void OnRangeBoundaryChanged(AvaloniaProperty property, DateTime? value)
    {
        if (_isUpdatingSelection)
        {
            return;
        }

        var normalized = value?.Date;
        if (normalized != value)
        {
            SetCurrentValue(property, normalized);
            return;
        }

        var start = RangeStart?.Date;
        var end = RangeEnd?.Date;
        if (start.HasValue && end.HasValue && end.Value < start.Value)
        {
            using (BeginSelectionUpdate())
            {
                SetCurrentValue(RangeStartProperty, end);
                SetCurrentValue(RangeEndProperty, start);
            }

            _rangeAnchor = end;
        }
        else
        {
            _rangeAnchor = start;
        }

        RefreshCalendar();
    }

    private void OnScrollMonthBufferChanged(int value)
    {
        if (value < 0)
        {
            SetCurrentValue(ScrollMonthBufferProperty, 0);
            return;
        }

        RefreshCalendar();
        ScrollHomeMonthIntoView();
    }

    private void MoveMonth(int offset)
    {
        SetCurrentValue(DisplayDateProperty, NormalizeMonth(DisplayDate.AddMonths(offset)));
    }

    private void SelectDateFromCommand(object? parameter)
    {
        if (parameter is DateTime date)
        {
            SelectDate(date);
        }
        else if (parameter is CalendarDayItem day)
        {
            SelectDate(day.Date);
        }
    }

    private void SelectDate(DateTime date)
    {
        date = date.Date;
        if (!IsDateEnabled(date))
        {
            return;
        }

        switch (SelectionMode)
        {
            case CalendarSelectionMode.Single:
                ApplySingleSelection(date);
                break;
            case CalendarSelectionMode.Multiple:
                ApplyMultipleSelection(date);
                break;
            case CalendarSelectionMode.Range:
                ApplyRangeSelection(date);
                break;
        }

        if (DisplayMode == CalendarDisplayMode.Paged && NormalizeMonth(DisplayDate) != NormalizeMonth(date))
        {
            SetCurrentValue(DisplayDateProperty, NormalizeMonth(date));
        }
        else
        {
            RefreshCalendar();
        }
    }

    private void ApplySingleSelection(DateTime date)
    {
        _rangeAnchor = date;

        using (BeginSelectionUpdate())
        {
            SetCurrentValue(SelectedDateProperty, date);
            SetCurrentValue(SelectedDatesProperty, Array.Empty<DateTime>());
            SetCurrentValue(RangeStartProperty, null);
            SetCurrentValue(RangeEndProperty, null);
        }
    }

    private void ApplyMultipleSelection(DateTime date)
    {
        var dates = NormalizeDateList(SelectedDates)?.ToList() ?? [];
        if (dates.Contains(date))
        {
            dates.Remove(date);
        }
        else
        {
            dates.Add(date);
            dates.Sort();
        }

        _rangeAnchor = dates.FirstOrDefault();

        using (BeginSelectionUpdate())
        {
            SetCurrentValue(SelectedDateProperty, null);
            SetCurrentValue(SelectedDatesProperty, dates.ToArray());
            SetCurrentValue(RangeStartProperty, null);
            SetCurrentValue(RangeEndProperty, null);
        }
    }

    private void ApplyRangeSelection(DateTime date)
    {
        var start = RangeStart?.Date;
        var end = RangeEnd?.Date;

        if (!start.HasValue || end.HasValue)
        {
            _rangeAnchor = date;

            using (BeginSelectionUpdate())
            {
                SetCurrentValue(SelectedDateProperty, null);
                SetCurrentValue(SelectedDatesProperty, Array.Empty<DateTime>());
                SetCurrentValue(RangeStartProperty, date);
                SetCurrentValue(RangeEndProperty, null);
            }

            return;
        }

        var anchor = _rangeAnchor ?? start.Value;
        var rangeStart = date < anchor ? date : anchor;
        var rangeEnd = date < anchor ? anchor : date;
        _rangeAnchor = rangeStart;

        using (BeginSelectionUpdate())
        {
            SetCurrentValue(SelectedDateProperty, null);
            SetCurrentValue(SelectedDatesProperty, Array.Empty<DateTime>());
            SetCurrentValue(RangeStartProperty, rangeStart);
            SetCurrentValue(RangeEndProperty, rangeEnd);
        }
    }

    private void CoerceSelectionForMode()
    {
        switch (SelectionMode)
        {
            case CalendarSelectionMode.Single:
            {
                var selected = SelectedDate?.Date
                               ?? NormalizeDateList(SelectedDates)?.FirstOrDefault()
                               ?? RangeStart?.Date
                               ?? RangeEnd?.Date;
                _rangeAnchor = selected;
                using (BeginSelectionUpdate())
                {
                    SetCurrentValue(SelectedDateProperty, selected);
                    SetCurrentValue(SelectedDatesProperty, Array.Empty<DateTime>());
                    SetCurrentValue(RangeStartProperty, null);
                    SetCurrentValue(RangeEndProperty, null);
                }
                break;
            }

            case CalendarSelectionMode.Multiple:
            {
                var dates = NormalizeDateList(SelectedDates)?.ToArray();
                if (dates is null or { Length: 0 })
                {
                    if (SelectedDate.HasValue)
                    {
                        dates = [SelectedDate.Value.Date];
                    }
                    else if (RangeStart.HasValue && RangeEnd.HasValue)
                    {
                        dates = EnumerateRange(RangeStart.Value.Date, RangeEnd.Value.Date).ToArray();
                    }
                    else if (RangeStart.HasValue)
                    {
                        dates = [RangeStart.Value.Date];
                    }
                }

                _rangeAnchor = dates?.FirstOrDefault();
                using (BeginSelectionUpdate())
                {
                    SetCurrentValue(SelectedDateProperty, null);
                    SetCurrentValue(SelectedDatesProperty, dates ?? Array.Empty<DateTime>());
                    SetCurrentValue(RangeStartProperty, null);
                    SetCurrentValue(RangeEndProperty, null);
                }
                break;
            }

            case CalendarSelectionMode.Range:
            {
                var start = RangeStart?.Date;
                var end = RangeEnd?.Date;
                if (!start.HasValue && !end.HasValue)
                {
                    var dates = NormalizeDateList(SelectedDates)?.ToArray();
                    if (dates is { Length: > 0 })
                    {
                        start = dates[0];
                        end = dates[^1];
                    }
                    else if (SelectedDate.HasValue)
                    {
                        start = SelectedDate.Value.Date;
                    }
                }

                if (start.HasValue && end.HasValue && end.Value < start.Value)
                {
                    (start, end) = (end, start);
                }

                _rangeAnchor = start;
                using (BeginSelectionUpdate())
                {
                    SetCurrentValue(SelectedDateProperty, null);
                    SetCurrentValue(SelectedDatesProperty, Array.Empty<DateTime>());
                    SetCurrentValue(RangeStartProperty, start);
                    SetCurrentValue(RangeEndProperty, end);
                }
                break;
            }
        }
    }

    private void RefreshCalendar()
    {
        var normalizedDisplayDate = NormalizeMonth(DisplayDate);
        var selection = CaptureSelection();

        DayOfWeekHeaders = BuildDayOfWeekHeaders();
        DisplayedMonthTitle = normalizedDisplayDate.ToString("Y", CultureInfo.CurrentCulture);
        PagedDays = BuildMonthDays(normalizedDisplayDate, selection);
        ScrollMonths = BuildScrollMonths(normalizedDisplayDate, selection);
    }

    private IReadOnlyList<string> BuildDayOfWeekHeaders()
    {
        var shortestNames = CultureInfo.CurrentCulture.DateTimeFormat.ShortestDayNames;
        var headers = new string[7];
        for (var index = 0; index < 7; index++)
        {
            var dayOfWeek = (int)FirstDayOfWeek + index;
            headers[index] = shortestNames[dayOfWeek % 7];
        }

        return headers;
    }

    private IReadOnlyList<CalendarDayItem> BuildMonthDays(DateTime month, SelectionState selection)
    {
        var monthStart = NormalizeMonth(month);
        var gridStart = GetGridStart(monthStart);
        var today = DateTime.Today;
        var days = new CalendarDayItem[42];

        for (var index = 0; index < days.Length; index++)
        {
            var current = gridStart.AddDays(index);
            days[index] = BuildDayItem(monthStart, current, today, selection);
        }

        return days;
    }

    private IReadOnlyList<CalendarMonthView> BuildScrollMonths(DateTime displayDate, SelectionState selection)
    {
        var buffer = Math.Max(0, ScrollMonthBuffer);
        var firstMonth = NormalizeMonth(displayDate).AddMonths(-buffer);
        var months = new List<CalendarMonthView>(buffer * 2 + 1);

        for (var index = 0; index <= buffer * 2; index++)
        {
            var month = firstMonth.AddMonths(index);
            var title = month.ToString("Y", CultureInfo.CurrentCulture);
            months.Add(new CalendarMonthView(month, title, BuildMonthDays(month, selection)));
        }

        return months;
    }

    private CalendarDayItem BuildDayItem(DateTime activeMonth, DateTime date, DateTime today, SelectionState selection)
    {
        var isCurrentMonth = date.Month == activeMonth.Month && date.Year == activeMonth.Year;
        var isToday = date == today;
        var isEnabled = IsDateEnabled(date);

        var isSelected = false;
        var isInRange = false;
        var isRangeStart = false;
        var isRangeEnd = false;

        switch (selection.Mode)
        {
            case CalendarSelectionMode.Single:
                isSelected = selection.SingleDate == date;
                break;

            case CalendarSelectionMode.Multiple:
                isSelected = selection.MultipleDates.Contains(date);
                break;

            case CalendarSelectionMode.Range:
                isRangeStart = selection.RangeStart == date;
                isRangeEnd = selection.RangeEnd == date;
                isInRange = selection.RangeStart.HasValue
                            && selection.RangeEnd.HasValue
                            && date >= selection.RangeStart.Value
                            && date <= selection.RangeEnd.Value;
                isSelected = isRangeStart || isRangeEnd || (selection.RangeStart == date && !selection.RangeEnd.HasValue);
                break;
        }

        return new CalendarDayItem(
            date,
            isCurrentMonth,
            isEnabled,
            isSelected,
            isInRange,
            isRangeStart,
            isRangeEnd,
            isToday);
    }

    private SelectionState CaptureSelection()
    {
        return SelectionMode switch
        {
            CalendarSelectionMode.Single => new SelectionState(
                SelectionMode,
                SelectedDate?.Date,
                [],
                null,
                null),

            CalendarSelectionMode.Multiple => new SelectionState(
                SelectionMode,
                null,
                NormalizeDateList(SelectedDates) is { } dates ? new HashSet<DateTime>(dates) : [],
                null,
                null),

            CalendarSelectionMode.Range => new SelectionState(
                SelectionMode,
                null,
                [],
                RangeStart?.Date,
                RangeEnd?.Date),

            _ => new SelectionState(SelectionMode, null, [], null, null)
        };
    }

    private DateTime GetGridStart(DateTime monthStart)
    {
        var current = monthStart;
        while (current.DayOfWeek != FirstDayOfWeek)
        {
            current = current.AddDays(-1);
        }

        return current;
    }

    private bool IsDateEnabled(DateTime date)
    {
        if (MinDate is { } minDate && date.Date < minDate.Date)
        {
            return false;
        }

        if (MaxDate is { } maxDate && date.Date > maxDate.Date)
        {
            return false;
        }

        return true;
    }

    private void UpdateDisplayModePseudoClasses(CalendarDisplayMode mode)
    {
        PseudoClasses.Set(PC_Paged, mode == CalendarDisplayMode.Paged);
        PseudoClasses.Set(PC_Scroll, mode == CalendarDisplayMode.Scroll);
    }

    private void UpdateSelectionModePseudoClasses(CalendarSelectionMode mode)
    {
        PseudoClasses.Set(PC_Single, mode == CalendarSelectionMode.Single);
        PseudoClasses.Set(PC_Multiple, mode == CalendarSelectionMode.Multiple);
        PseudoClasses.Set(PC_Range, mode == CalendarSelectionMode.Range);
    }

    private void ScrollHomeMonthIntoView()
    {
        if (_scrollViewer is null || DisplayMode != CalendarDisplayMode.Scroll || ScrollMonths.Count == 0)
        {
            return;
        }

        var targetMonth = ScrollMonths.FirstOrDefault(x => x.Month == NormalizeMonth(DisplayDate));
        if (targetMonth is null)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            if (_scrollViewer is null || DisplayMode != CalendarDisplayMode.Scroll)
            {
                return;
            }

            var target = _scrollViewer
                .GetVisualDescendants()
                .OfType<Control>()
                .FirstOrDefault(control => ReferenceEquals(control.DataContext, targetMonth));

            target?.BringIntoView();
        }, DispatcherPriority.Loaded);
    }

    private void OnPagedHostPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DisplayMode != CalendarDisplayMode.Paged || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        _swipeStart = e.GetPosition(_pagedHost);
    }

    private void OnPagedHostPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_swipeStart is null || _pagedHost is null || DisplayMode != CalendarDisplayMode.Paged)
        {
            _swipeStart = null;
            return;
        }

        var end = e.GetPosition(_pagedHost);
        var delta = end - _swipeStart.Value;
        _swipeStart = null;

        if (Math.Abs(delta.X) < SwipeThreshold || Math.Abs(delta.X) <= Math.Abs(delta.Y))
        {
            return;
        }

        if (delta.X < 0)
        {
            MoveMonth(1);
        }
        else
        {
            MoveMonth(-1);
        }

        e.Handled = true;
    }

    private void OnPagedHostPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        _swipeStart = null;
    }

    private IDisposable BeginSelectionUpdate() => new SelectionUpdateScope(this);

    private static DateTime NormalizeMonth(DateTime date) => new(date.Year, date.Month, 1);

    private static IReadOnlyList<DateTime>? NormalizeDateList(IReadOnlyList<DateTime>? dates)
    {
        if (dates is null)
        {
            return null;
        }

        return dates
            .Select(x => x.Date)
            .Distinct()
            .Order()
            .ToArray();
    }

    private static bool AreDatesEqual(IReadOnlyList<DateTime>? left, IReadOnlyList<DateTime>? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return left is null && right is null;
        }

        return left.Count == right.Count && !left.Where((t, i) => t.Date != right[i].Date).Any();
    }

    private static IEnumerable<DateTime> EnumerateRange(DateTime start, DateTime end)
    {
        start = start.Date;
        end = end.Date;

        if (end < start)
        {
            (start, end) = (end, start);
        }

        for (var current = start; current <= end; current = current.AddDays(1))
        {
            yield return current;
        }
    }

    private readonly record struct SelectionState(
        CalendarSelectionMode Mode,
        DateTime? SingleDate,
        HashSet<DateTime> MultipleDates,
        DateTime? RangeStart,
        DateTime? RangeEnd);

    private sealed class ActionCommand(Action<object?> execute) : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => execute(parameter);
    }

    private readonly struct SelectionUpdateScope : IDisposable
    {
        private readonly Calendar _owner;

        public SelectionUpdateScope(Calendar owner)
        {
            _owner = owner;
            _owner._isUpdatingSelection = true;
        }

        public void Dispose()
        {
            _owner._isUpdatingSelection = false;
        }
    }
}
