using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace Irihi.Mafia.Controls.Primitives;

/// <summary>
/// A vertical panel with sticky headers.
/// </summary>
/// <remarks>
/// Set <see cref="StickyLevelProperty"/> on a child to pin it while scrolling.
/// Same-level elements replace each other; different levels stack.
/// Use inside a <see cref="ScrollViewer"/>.
/// </remarks>
public class StickyPanel : Panel
{
    // ─────────────────────────────────────────────────────────────
    //  Attached property
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Defines <c>StickyPanel.StickyLevel</c>. <see langword="null"/> disables sticky behavior.
    /// </summary>
    public static readonly AttachedProperty<int?> StickyLevelProperty =
        AvaloniaProperty.RegisterAttached<StickyPanel, Control, int?>("StickyLevel");

    /// <summary>Gets the value of <see cref="StickyLevelProperty"/> on a control.</summary>
    public static int? GetStickyLevel(Control control) => control.GetValue(StickyLevelProperty);

    /// <summary>Sets the value of <see cref="StickyLevelProperty"/> on a control.</summary>
    public static void SetStickyLevel(Control control, int? value) => control.SetValue(StickyLevelProperty, value);

    // ─────────────────────────────────────────────────────────────
    //  Internals
    // ─────────────────────────────────────────────────────────────

    /// <summary>Z-order baseline for pinned elements.</summary>
    private const int StickyZIndex = 1000;

    /// <summary>Cached sticky elements and levels.</summary>
    private readonly List<StickyElementInfo> _stickyElements = new();

    /// <summary>Sticky entries sorted by layout position.</summary>
    private readonly List<StickyEntry> _stickyEntries = new();

    /// <summary>Scratch buffer for computed pinned Y positions.</summary>
    private double[] _pinned = Array.Empty<double>();

    private double _maxStickyHeight;
    private Rect _viewport;
    private double _lastOffset = double.NaN;
    private bool _discoveryDirty = true;

    static StickyPanel()
    {
        // If a StickyLevel is (re)assigned at runtime, re-scan the owning panel's subtree.
        StickyLevelProperty.Changed.AddClassHandler<Control>(static (control, _) =>
        {
            control.FindAncestorOfType<StickyPanel>()?.OnStickyLevelChanged();
        });
    }

    public StickyPanel()
    {
        EffectiveViewportChanged += OnEffectiveViewportChanged;
        Children.CollectionChanged += (_, _) => _discoveryDirty = true;
    }

    private void OnStickyLevelChanged() => _discoveryDirty = true;

    // ─────────────────────────────────────────────────────────────
    //  Measure / Arrange
    // ─────────────────────────────────────────────────────────────

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        double width = 0;
        double height = 0;

        foreach (var child in Children)
        {
            child.Measure(new Size(availableSize.Width, double.PositiveInfinity));
            width = Math.Max(width, child.DesiredSize.Width);
            height += child.DesiredSize.Height;
        }

        return new Size(
            double.IsInfinity(availableSize.Width) ? width : availableSize.Width,
            height);
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        double y = 0;
        foreach (var child in Children)
        {
            child.Arrange(new Rect(0, y, finalSize.Width, child.DesiredSize.Height));
            y += child.DesiredSize.Height;
        }

        RebuildStickyEntries();

        return finalSize;
    }

    // ─────────────────────────────────────────────────────────────
    //  Sticky discovery
    // ─────────────────────────────────────────────────────────────

    private void RebuildStickyEntries()
    {
        // Only walk the whole visual subtree when the set of sticky elements may have
        // changed.  On ordinary arrange passes we just re-read the cached elements' bounds.
        if (_discoveryDirty)
        {
            RediscoverStickyElements();
            _discoveryDirty = false;
        }

        _stickyEntries.Clear();

        foreach (var info in _stickyElements)
        {
            double top = ComputeTopRelativeToPanel(info.Element);
            if (double.IsNaN(top))
                continue; // element left the subtree — will be dropped on the next rescan

            _stickyEntries.Add(new StickyEntry
            {
                Element = info.Element,
                Level = info.Level,
                Top = top,
                Height = info.Element.Bounds.Height,
            });
        }

        // Document order == ascending top, which is what the push-out algorithm needs.
        _stickyEntries.Sort(static (a, b) => a.Top.CompareTo(b.Top));

        ComputeRelations();

        if (!double.IsNaN(_lastOffset))
            ApplyStickyOffsets();
    }

    /// <summary>
    /// Rebuilds the cached list of elements with <see cref="StickyLevelProperty"/>.
    /// </summary>
    private void RediscoverStickyElements()
    {
        _stickyElements.Clear();
        foreach (var child in Children)
            CollectStickyInSubtree(child);
    }

    private void CollectStickyInSubtree(Visual root)
    {
        if (root is Control c && GetStickyLevel(c) is { } level)
        {
            _stickyElements.Add(new StickyElementInfo { Element = c, Level = level });

            // Keep a pinned header above the siblings that scroll underneath it.  Lower
            // levels get a higher z-order so a level-1 header stays above a level-2 header
            // it may overlap while the latter is being pushed out.
            c.SetValue(Panel.ZIndexProperty, StickyZIndex - level);
        }

        foreach (var child in root.GetVisualChildren())
            CollectStickyInSubtree(child);
    }

    /// <summary>
    /// Returns the Y position of <paramref name="element"/> relative to this panel.
    /// </summary>
    private double ComputeTopRelativeToPanel(Control element)
    {
        double y = element.Bounds.Top;

        Visual? parent = element.GetVisualParent();
        while (parent is not null && !ReferenceEquals(parent, this))
        {
            if (parent is Control pc)
                y += pc.Bounds.Top;
            parent = parent.GetVisualParent();
        }

        return ReferenceEquals(parent, this) ? y : double.NaN;
    }

    /// <summary>
    /// Computes per-entry indices used for stacking and same-level push-out.
    /// </summary>
    private void ComputeRelations()
    {
        int n = _stickyEntries.Count;
        _maxStickyHeight = 0;

        // Next same-level index — scan backwards, remembering the nearest one per level.
        var nextByLevel = new Dictionary<int, int>();
        for (int i = n - 1; i >= 0; i--)
        {
            var entry = _stickyEntries[i];
            entry.NextSameIndex = nextByLevel.TryGetValue(entry.Level, out int next) ? next : -1;
            nextByLevel[entry.Level] = i;
        }

        // Nearest lower-level index — scan forwards, remembering the latest index per level.
        var lastByLevel = new Dictionary<int, int>();
        for (int i = 0; i < n; i++)
        {
            var entry = _stickyEntries[i];

            int above = -1;
            foreach (var (level, index) in lastByLevel)
            {
                if (level < entry.Level && index > above)
                    above = index;
            }
            entry.AboveIndex = above;

            lastByLevel[entry.Level] = i;
            _maxStickyHeight = Math.Max(_maxStickyHeight, entry.Height);
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  Scroll handling
    // ─────────────────────────────────────────────────────────────

    private void OnEffectiveViewportChanged(object? sender, EffectiveViewportChangedEventArgs e)
    {
        // e.EffectiveViewport is expressed in this panel's coordinate space, so its Top is
        // exactly the scroll offset (the viewport's top edge in content coordinates).
        _viewport = e.EffectiveViewport;
        _lastOffset = e.EffectiveViewport.Top;
        ApplyStickyOffsets();
    }

    private void ApplyStickyOffsets()
    {
        double offset = _lastOffset;
        int n = _stickyEntries.Count;
        if (n == 0)
            return;

        if (_pinned.Length < n)
            _pinned = new double[n];

        // Compute every pinned position.  A header's stack base depends on the nearest
        // lower-level header, which always precedes it, so one forward pass suffices.  This
        // is cheap scalar math and is kept over the full range for simplicity.
        for (int i = 0; i < n; i++)
        {
            var entry = _stickyEntries[i];

            double basePos = entry.AboveIndex >= 0
                ? _pinned[entry.AboveIndex] + _stickyEntries[entry.AboveIndex].Height
                : offset;

            double pushTop = entry.NextSameIndex >= 0
                ? _stickyEntries[entry.NextSameIndex].Top
                : double.PositiveInfinity;

            double p;
            if (entry.Top >= offset && entry.Top >= basePos)
            {
                // Still at its natural position — not yet at the top or the stack.
                p = entry.Top;
            }
            else
            {
                // Pin at the stack bottom, clamped by how far the next same-level header
                // can push it up.  When the stack already sits past that limit, the header
                // gives way and the next same-level header simply slides over it.
                p = Math.Min(basePos, Math.Max(basePos, pushTop) - entry.Height);
            }

            _pinned[i] = p;
        }

        // Only touch transforms whose visual bounds intersect the visible window (plus a
        // buffer so headers slide in and out smoothly).  Applying a RenderTransform
        // invalidates render, so skipping off-screen elements is the real win; transforms
        // are also reused per entry instead of being reallocated on every scroll tick.
        double viewportHeight = _viewport.Height > 0 ? _viewport.Height : double.PositiveInfinity;
        double buffer = double.IsInfinity(viewportHeight) ? 0 : Math.Max(_maxStickyHeight, viewportHeight * 0.5);

        for (int i = 0; i < n; i++)
        {
            var entry = _stickyEntries[i];
            double visualTop = _pinned[i] - offset;

            if (visualTop + entry.Height < -buffer || visualTop > viewportHeight + buffer)
                continue;

            ApplyTranslate(entry, _pinned[i] - entry.Top);
        }
    }

    private static void ApplyTranslate(StickyEntry entry, double translate)
    {
        if (translate <= 0.0001)
        {
            if (entry.Element.RenderTransform is not null)
                entry.Element.RenderTransform = null;
            return;
        }

        var transform = entry.Transform;
        if (transform is null)
        {
            transform = new TranslateTransform(0, translate);
            entry.Transform = transform;
        }
        else
        {
            transform.Y = translate;
        }

        if (!ReferenceEquals(entry.Element.RenderTransform, transform))
            entry.Element.RenderTransform = transform;
    }

    // ─────────────────────────────────────────────────────────────
    //  Records
    // ─────────────────────────────────────────────────────────────

    /// <summary>Cached sticky element metadata.</summary>
    private sealed class StickyElementInfo
    {
        public required Control Element { get; init; }
        public required int Level { get; init; }
    }

    /// <summary>Sticky element state for one layout pass.</summary>
    private sealed class StickyEntry
    {
        public required Control Element { get; init; }
        public required int Level { get; init; }
        public double Top { get; init; }
        public double Height { get; init; }
        public int AboveIndex { get; set; } = -1;
        public int NextSameIndex { get; set; } = -1;
        public TranslateTransform? Transform { get; set; }
    }
}
