using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Irihi.Mafia.Controls;

public class CellGroup : HeaderedItemsControl
{
    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        recycleKey = null;
        return item is not Cell;
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) => new Cell();

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is Cell cell && !cell.IsSet(ContentControl.ContentTemplateProperty))
        {
            cell.ContentTemplate = ItemTemplate;
        }
    }
}