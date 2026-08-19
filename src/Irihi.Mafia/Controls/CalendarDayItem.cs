using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Irihi.Mafia.Controls;

public sealed class CalendarDayItem : INotifyPropertyChanged
{
    private bool _isSelected;
    private bool _isInRange;
    private bool _isRangeStart;
    private bool _isRangeEnd;

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
        _isSelected = isSelected;
        _isInRange = isInRange;
        _isRangeStart = isRangeStart;
        _isRangeEnd = isRangeEnd;
        IsToday = isToday;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public DateTime Date { get; }

    public string Text { get; }

    public bool IsCurrentMonth { get; }

    public bool IsEnabled { get; }

    public bool IsSelected
    {
        get => _isSelected;
        private set => SetField(ref _isSelected, value);
    }

    public bool IsInRange
    {
        get => _isInRange;
        private set => SetField(ref _isInRange, value);
    }

    public bool IsRangeStart
    {
        get => _isRangeStart;
        private set => SetField(ref _isRangeStart, value);
    }

    public bool IsRangeEnd
    {
        get => _isRangeEnd;
        private set => SetField(ref _isRangeEnd, value);
    }

    public bool IsToday { get; }

    public void UpdateSelectionState(bool isSelected, bool isInRange, bool isRangeStart, bool isRangeEnd)
    {
        IsSelected = isSelected;
        IsInRange = isInRange;
        IsRangeStart = isRangeStart;
        IsRangeEnd = isRangeEnd;
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
