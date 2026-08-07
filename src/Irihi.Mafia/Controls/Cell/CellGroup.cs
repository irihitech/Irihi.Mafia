using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;

namespace Irihi.Mafia.Controls;

public class CellGroup : HeaderedItemsControl
{
    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        recycleKey = null;
        return item is not Control;
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return ItemTemplate is null ? new Cell() : new ContentPresenter();
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is ContentPresenter presenter)
        {
            presenter.Content = item;
            presenter.ContentTemplate = ItemTemplate;
        }
        else if (container is Cell cell && !cell.IsSet(ContentControl.ContentTemplateProperty))
        {
            cell.ContentTemplate = ItemTemplate;
        }
    }
}
