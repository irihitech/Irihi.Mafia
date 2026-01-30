using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Irihi.Mirana.Controls;

public class Collapse : ItemsControl
{
    public static readonly StyledProperty<bool> ExpandMutexProperty =
        AvaloniaProperty.Register<Collapse, bool>(nameof(ExpandMutex));

    public bool ExpandMutex
    {
        get => GetValue(ExpandMutexProperty);
        set => SetValue(ExpandMutexProperty, value);
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        recycleKey = null;
        return item is not CollapsePanel;
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new CollapsePanel();
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);

        if (container is CollapsePanel panel)
        {
            if (!panel.IsSet(ContentControl.ContentTemplateProperty))
            {
                panel.ContentTemplate = ItemTemplate;
            }

            // Set up panel value if not set
            if (panel.Value == null)
            {
                panel.Value = index;
            }

            // Handle expand mutex logic
            panel.PropertyChanged += (_, e) =>
            {
                if (e.Property == CollapsePanel.IsExpandedProperty && e.NewValue is true && ExpandMutex)
                {
                    // Collapse all other panels
                    foreach (var child in GetPanels())
                    {
                        if (child != panel)
                        {
                            child.IsExpanded = false;
                        }
                    }
                }
            };
        }
    }

    private IEnumerable<CollapsePanel> GetPanels()
    {
        return ItemsPanelRoot?.Children.OfType<CollapsePanel>() ?? Enumerable.Empty<CollapsePanel>();
    }
}
