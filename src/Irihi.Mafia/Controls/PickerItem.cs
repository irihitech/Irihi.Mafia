using Avalonia.Controls;

namespace Irihi.Mafia.Controls;

/// <summary>
/// A selectable item in a <see cref="Picker"/>.
/// </summary>
public class PickerItem : ListBoxItem
{
    static PickerItem()
    {
        FocusableProperty.OverrideDefaultValue<PickerItem>(true);
    }
}
