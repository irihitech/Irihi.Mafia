using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Irihi.Mafia.Controls;

public class CalendarMonthsPresenter : ItemsControl, ILogicalScrollable
{
    private ILogicalScrollable? _logicalScrollable;

    protected override Type StyleKeyOverride => typeof(ItemsControl);

    public event EventHandler? ScrollInvalidated;

    public Size Extent => _logicalScrollable?.Extent ?? default;

    public Vector Offset
    {
        get => _logicalScrollable?.Offset ?? default;
        set
        {
            EnsureLogicalScrollable();
            if (_logicalScrollable is not null)
            {
                _logicalScrollable.Offset = value;
            }
        }
    }

    public Size Viewport => _logicalScrollable?.Viewport ?? default;

    public bool CanHorizontallyScroll
    {
        get => _logicalScrollable?.CanHorizontallyScroll ?? false;
        set
        {
            EnsureLogicalScrollable();
            if (_logicalScrollable is not null)
            {
                _logicalScrollable.CanHorizontallyScroll = value;
            }
        }
    }

    public bool CanVerticallyScroll
    {
        get => _logicalScrollable?.CanVerticallyScroll ?? false;
        set
        {
            EnsureLogicalScrollable();
            if (_logicalScrollable is not null)
            {
                _logicalScrollable.CanVerticallyScroll = value;
            }
        }
    }

    public bool IsLogicalScrollEnabled => _logicalScrollable?.IsLogicalScrollEnabled ?? false;

    public Size ScrollSize => _logicalScrollable?.ScrollSize ?? default;

    public Size PageScrollSize => _logicalScrollable?.PageScrollSize ?? default;

    protected override Size ArrangeOverride(Size finalSize)
    {
        EnsureLogicalScrollable();
        return base.ArrangeOverride(finalSize);
    }

    public bool BringIntoView(Control target, Rect targetRect)
    {
        EnsureLogicalScrollable();
        return _logicalScrollable?.BringIntoView(target, targetRect) ?? false;
    }

    public Control? GetControlInDirection(NavigationDirection direction, Control? from)
    {
        EnsureLogicalScrollable();
        return _logicalScrollable?.GetControlInDirection(direction, from!);
    }

    public void RaiseScrollInvalidated(EventArgs e)
    {
        ScrollInvalidated?.Invoke(this, e);
    }

    private void EnsureLogicalScrollable()
    {
        if (ItemsPanelRoot is ILogicalScrollable logicalScrollable)
        {
            if (ReferenceEquals(_logicalScrollable, logicalScrollable))
            {
                return;
            }

            DetachLogicalScrollable();
            _logicalScrollable = logicalScrollable;
            _logicalScrollable.ScrollInvalidated += OnScrollInvalidated;
        }
        else
        {
            DetachLogicalScrollable();
        }
    }

    private void DetachLogicalScrollable()
    {
        if (_logicalScrollable is null)
        {
            return;
        }

        _logicalScrollable.ScrollInvalidated -= OnScrollInvalidated;
        _logicalScrollable = null;
    }

    private void OnScrollInvalidated(object? sender, EventArgs e)
    {
        ScrollInvalidated?.Invoke(this, e);
    }
}
