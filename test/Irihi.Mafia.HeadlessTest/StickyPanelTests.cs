using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Irihi.Mafia.Controls.Primitives;
using Irihi.Mafia.HeadlessTest.StickyPanelTest;

namespace Irihi.Mafia.HeadlessTest;

/// <summary>
/// Headless tests for <see cref="StickyPanel"/>.
/// </summary>
public class StickyPanelTests
{
    // ── Attached property ────────────────────────────────────────────────────

    [AvaloniaFact]
    public void StickyLevel_Defaults_To_Null()
    {
        var border = new Border();
        Assert.Null(StickyPanel.GetStickyLevel(border));
    }

    [AvaloniaFact]
    public void StickyLevel_RoundTrips()
    {
        var border = new Border();
        StickyPanel.SetStickyLevel(border, 2);
        Assert.Equal(2, StickyPanel.GetStickyLevel(border));

        StickyPanel.SetStickyLevel(border, null);
        Assert.Null(StickyPanel.GetStickyLevel(border));
    }

    // ── Layout ───────────────────────────────────────────────────────────────

    [AvaloniaFact]
    public void Measure_Stacks_Children_Vertically()
    {
        var panel = new StickyPanel();
        panel.Children.Add(new Border { Width = 100, Height = 20 });
        panel.Children.Add(new Border { Width = 100, Height = 30 });

        panel.Measure(new Size(200, double.PositiveInfinity));

        Assert.Equal(50, panel.DesiredSize.Height);
        Assert.Equal(200, panel.DesiredSize.Width);
    }

    [AvaloniaFact]
    public void Arrange_Positions_Children_Vertically()
    {
        var panel = new StickyPanel();
        var b0 = new Border { Height = 20 };
        var b1 = new Border { Height = 30 };
        panel.Children.Add(b0);
        panel.Children.Add(b1);

        panel.Measure(new Size(200, double.PositiveInfinity));
        panel.Arrange(new Rect(0, 0, 200, 50));

        Assert.Equal(new Rect(0, 0, 200, 20), b0.Bounds);
        Assert.Equal(new Rect(0, 20, 200, 30), b1.Bounds);
    }

    // ── Pinning ──────────────────────────────────────────────────────────────

    [AvaloniaFact]
    public void Sticky_Child_Pins_To_Top_When_Scrolled()
    {
        var (header, _) = BuildScrollingPanel(out var scroll, out var content);

        scroll.Offset = new Vector(0, 100);
        Dispatcher.UIThread.RunJobs();

        var transform = Assert.IsType<TranslateTransform>(header.RenderTransform);
        Assert.Equal(100, transform.Y, 3);
    }

    [AvaloniaFact]
    public void NonSticky_Child_Is_Not_Transformed()
    {
        var (_, content) = BuildScrollingPanel(out var scroll, out _);

        scroll.Offset = new Vector(0, 100);
        Dispatcher.UIThread.RunJobs();

        Assert.Null(content.RenderTransform);
    }

    // ── Push-out ─────────────────────────────────────────────────────────────

    [AvaloniaFact]
    public void Next_Sticky_Pushes_Previous_Out()
    {
        var panel = new StickyPanel();

        var header0 = new Border { Height = 20 };
        StickyPanel.SetStickyLevel(header0, 1);
        var content0 = new Border { Height = 100 };
        var header1 = new Border { Height = 20 };
        StickyPanel.SetStickyLevel(header1, 1);
        var content1 = new Border { Height = 100 };

        panel.Children.Add(header0);   // top = 0
        panel.Children.Add(content0);  // top = 20
        panel.Children.Add(header1);   // top = 120
        panel.Children.Add(content1);  // top = 140

        var window = new Window { Width = 200, Height = 100 };
        var scroll = new ScrollViewer { Content = panel };
        window.Content = scroll;
        window.Show();
        Dispatcher.UIThread.RunJobs();

        // header1 reaches header0's bottom at offset 100 (header1.Top - header0.Height);
        // header0 is still pinned at the top.
        scroll.Offset = new Vector(0, 100);
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(100, Assert.IsType<TranslateTransform>(header0.RenderTransform).Y, 3);
        Assert.Null(header1.RenderTransform);

        // header1 now pins at the top; it has pushed header0 up by header0's own height.
        scroll.Offset = new Vector(0, 140);
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(120, Assert.IsType<TranslateTransform>(header0.RenderTransform).Y, 3);
        Assert.Equal(20, Assert.IsType<TranslateTransform>(header1.RenderTransform).Y, 3);
    }

    // ── Multi-level stacking ─────────────────────────────────────────────────

    [AvaloniaFact]
    public void Different_Levels_Stack_While_Same_Level_Pushes_Out()
    {
        var panel = new StickyPanel();

        var h1 = new Border { Height = 40 };
        StickyPanel.SetStickyLevel(h1, 1);
        var c1 = new Border { Height = 200 }; // h2 lands at top = 240
        var h2 = new Border { Height = 30 };
        StickyPanel.SetStickyLevel(h2, 2);
        var c2 = new Border { Height = 100 }; // h3 lands at top = 370
        var h3 = new Border { Height = 30 };
        StickyPanel.SetStickyLevel(h3, 2);
        var c3 = new Border { Height = 200 };

        panel.Children.Add(h1);
        panel.Children.Add(c1);
        panel.Children.Add(h2);
        panel.Children.Add(c2);
        panel.Children.Add(h3);
        panel.Children.Add(c3);

        var window = new Window { Width = 200, Height = 100 };
        var scroll = new ScrollViewer { Content = panel };
        window.Content = scroll;
        window.Show();
        Dispatcher.UIThread.RunJobs();

        // offset 250: level-1 h1 pins at the top; level-2 h2 stacks below it at y=40;
        // h3 is still at its natural position.
        scroll.Offset = new Vector(0, 250);
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(250, Assert.IsType<TranslateTransform>(h1.RenderTransform).Y, 3);
        Assert.Equal(50, Assert.IsType<TranslateTransform>(h2.RenderTransform).Y, 3);
        Assert.Null(h3.RenderTransform);

        // offset 350: h3 (same level as h2) has pinned below h1 and pushed h2 up by h2's
        // own height; h2 now sits just above h3.
        scroll.Offset = new Vector(0, 350);
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(350, Assert.IsType<TranslateTransform>(h1.RenderTransform).Y, 3);
        Assert.Equal(120, Assert.IsType<TranslateTransform>(h2.RenderTransform).Y, 3);
        Assert.Equal(20, Assert.IsType<TranslateTransform>(h3.RenderTransform).Y, 3);
    }

    // ── ItemsControl / ItemsPanel ────────────────────────────────────────────

    [AvaloniaFact]
    public void ItemsControl_Template_Sticky_Is_Discovered_And_Pinned()
    {
        var view = new StickyPanelTestView();
        view.items.ItemsSource = Enumerable.Range(0, 10).Select(_ => new object()).ToList();

        var window = new Window { Width = 200, Height = 100, Content = view };
        window.Show();
        Dispatcher.UIThread.RunJobs();

        var header0 = FindSticky(view.items.ContainerFromIndex(0)!);
        var header1 = FindSticky(view.items.ContainerFromIndex(1)!);
        Assert.NotNull(header0);
        Assert.NotNull(header1);

        // Each item is 100 tall (header 20 + content 80).  At offset 50 the first header
        // is pinned at the top (translated +50); the second has not arrived yet.
        view.scroll.Offset = new Vector(0, 50);
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(50, Assert.IsType<TranslateTransform>(header0!.RenderTransform).Y, 3);
        Assert.Null(header1!.RenderTransform);

        // At offset 100 the second header reaches the top and the first is pushed out
        // (its push limit is nextTop - top - height = 100 - 0 - 20 = 80).
        view.scroll.Offset = new Vector(0, 100);
        Dispatcher.UIThread.RunJobs();
        Assert.Equal(80, Assert.IsType<TranslateTransform>(header0.RenderTransform).Y, 3);
        Assert.Null(header1.RenderTransform);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Finds the first control in a visual subtree carrying <see cref="StickyPanel.StickyLevelProperty"/>.
    /// </summary>
    private static Control? FindSticky(Visual root)
    {
        if (root is Control c && StickyPanel.GetStickyLevel(c).HasValue)
            return c;

        foreach (var child in root.GetVisualChildren())
        {
            var found = FindSticky(child);
            if (found is not null)
                return found;
        }

        return null;
    }

    /// <summary>
    /// Builds a windowed <see cref="ScrollViewer"/> hosting a <see cref="StickyPanel"/> with
    /// one sticky header (height 20) followed by a tall scrolling content block.
    /// </summary>
    private static (Border header, Border content) BuildScrollingPanel(
        out ScrollViewer scroll, out Border content)
    {
        var panel = new StickyPanel();
        var header = new Border { Height = 20 };
        StickyPanel.SetStickyLevel(header, 1);
        content = new Border { Height = 500 };
        panel.Children.Add(header);
        panel.Children.Add(content);

        var window = new Window { Width = 200, Height = 100 };
        scroll = new ScrollViewer { Content = panel };
        window.Content = scroll;
        window.Show();
        Dispatcher.UIThread.RunJobs();

        return (header, content);
    }
}
