using Avalonia.Controls;

namespace Irihi.Mafia.Controls;

/// <summary>
/// A selectable item in a <see cref="ComboBox"/>.
/// </summary>
public class ComboBoxItem : ListBoxItem
{
    static ComboBoxItem()
    {
        FocusableProperty.OverrideDefaultValue<ComboBoxItem>(true);
    }
}
