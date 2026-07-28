using System;
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using Irihi.Mafia.Controls.Primitives;
using Popup = Irihi.Mafia.Controls.Primitives.Popup;

namespace Irihi.Mafia.Controls;

/// <summary>
/// A drop-down list control optimized for touch interaction.
/// Uses Mafia's <see cref="Popup"/> for the dropdown overlay.
/// </summary>
[TemplatePart("PART_Popup", typeof(Popup), IsRequired = true)]
[PseudoClasses(pcDropdownOpen, pcPressed)]
public class ComboBox : SelectingItemsControl
{
    internal const string pcDropdownOpen = ":dropdownopen";
    internal const string pcPressed = ":pressed";

    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new VirtualizingStackPanel());

    /// <summary>
    /// Defines the <see cref="IsDropDownOpen"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<ComboBox, bool>(nameof(IsDropDownOpen));

    /// <summary>
    /// Defines the <see cref="MaxDropDownHeight"/> property.
    /// </summary>
    public static readonly StyledProperty<double> MaxDropDownHeightProperty =
        AvaloniaProperty.Register<ComboBox, double>(nameof(MaxDropDownHeight), 200);

    /// <summary>
    /// Defines the <see cref="SelectionBoxItem"/> property.
    /// </summary>
    public static readonly DirectProperty<ComboBox, object?> SelectionBoxItemProperty =
        AvaloniaProperty.RegisterDirect<ComboBox, object?>(nameof(SelectionBoxItem), o => o.SelectionBoxItem);

    /// <summary>
    /// Defines the <see cref="PlaceholderText"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<ComboBox, string?>(nameof(PlaceholderText));

    /// <summary>
    /// Defines the <see cref="PlaceholderForeground"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> PlaceholderForegroundProperty =
        AvaloniaProperty.Register<ComboBox, IBrush?>(nameof(PlaceholderForeground));

    /// <summary>
    /// Defines the <see cref="HorizontalContentAlignment"/> property.
    /// </summary>
    public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
        ContentControl.HorizontalContentAlignmentProperty.AddOwner<ComboBox>();

    /// <summary>
    /// Defines the <see cref="VerticalContentAlignment"/> property.
    /// </summary>
    public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
        ContentControl.VerticalContentAlignmentProperty.AddOwner<ComboBox>();

    /// <summary>
    /// Defines the <see cref="SelectionBoxItemTemplate"/> property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> SelectionBoxItemTemplateProperty =
        AvaloniaProperty.Register<ComboBox, IDataTemplate?>(nameof(SelectionBoxItemTemplate));

    private Popup? _popup;
    private object? _selectionBoxItem;

    /// <summary>
    /// Initializes static members of the <see cref="ComboBox"/> class.
    /// </summary>
    static ComboBox()
    {
        ItemsPanelProperty.OverrideDefaultValue<ComboBox>(DefaultPanel);
        FocusableProperty.OverrideDefaultValue<ComboBox>(true);
    }

    /// <summary>
    /// Occurs after the drop-down list of the <see cref="ComboBox"/> closes.
    /// </summary>
    public event EventHandler? DropDownClosed;

    /// <summary>
    /// Occurs after the drop-down list of the <see cref="ComboBox"/> opens.
    /// </summary>
    public event EventHandler? DropDownOpened;

    /// <summary>
    /// Gets or sets a value indicating whether the dropdown is currently open.
    /// </summary>
    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    /// <summary>
    /// Gets or sets the maximum height for the dropdown list.
    /// </summary>
    public double MaxDropDownHeight
    {
        get => GetValue(MaxDropDownHeightProperty);
        set => SetValue(MaxDropDownHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the item to display as the control's selected content.
    /// </summary>
    public object? SelectionBoxItem
    {
        get => _selectionBoxItem;
        protected set => SetAndRaise(SelectionBoxItemProperty, ref _selectionBoxItem, value);
    }

    /// <summary>
    /// Gets or sets the placeholder text displayed when no item is selected.
    /// </summary>
    public string? PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used to render the placeholder text.
    /// </summary>
    public IBrush? PlaceholderForeground
    {
        get => GetValue(PlaceholderForegroundProperty);
        set => SetValue(PlaceholderForegroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal alignment of the content within the control.
    /// </summary>
    public HorizontalAlignment HorizontalContentAlignment
    {
        get => GetValue(HorizontalContentAlignmentProperty);
        set => SetValue(HorizontalContentAlignmentProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical alignment of the content within the control.
    /// </summary>
    public VerticalAlignment VerticalContentAlignment
    {
        get => GetValue(VerticalContentAlignmentProperty);
        set => SetValue(VerticalContentAlignmentProperty, value);
    }

    /// <summary>
    /// Gets or sets the data template used to display the selected item in the selection box.
    /// </summary>
    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public IDataTemplate? SelectionBoxItemTemplate
    {
        get => GetValue(SelectionBoxItemTemplateProperty);
        set => SetValue(SelectionBoxItemTemplateProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateSelectionBoxItem(SelectedItem);
    }

    /// <inheritdoc/>
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new ComboBoxItem();
    }

    /// <inheritdoc/>
    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<ComboBoxItem>(item, out recycleKey);
    }

    /// <inheritdoc/>
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (IsDropDownOpen)
        {
            SetCurrentValue(IsDropDownOpenProperty, false);
            e.Handled = true;
        }
        else
        {
            PseudoClasses.Set(pcPressed, true);
        }
    }

    /// <inheritdoc/>
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (!e.Handled && PseudoClasses.Contains(pcPressed))
        {
            SetCurrentValue(IsDropDownOpenProperty, !IsDropDownOpen);
            e.Handled = true;
        }

        PseudoClasses.Set(pcPressed, false);
        base.OnPointerReleased(e);
    }

    /// <inheritdoc/>
    public override bool UpdateSelectionFromEvent(Control container, RoutedEventArgs eventArgs)
    {
        if (base.UpdateSelectionFromEvent(container, eventArgs))
        {
            _popup?.Close();
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_popup != null)
        {
            _popup.Opened -= PopupOpened;
            _popup.Closed -= PopupClosed;
        }

        _popup = e.NameScope.Get<Popup>("PART_Popup");
        _popup.Opened += PopupOpened;
        _popup.Closed += PopupClosed;
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == SelectedItemProperty)
        {
            UpdateSelectionBoxItem(change.NewValue);
        }
        else if (change.Property == IsDropDownOpenProperty)
        {
            PseudoClasses.Set(pcDropdownOpen, change.GetNewValue<bool>());
        }

        base.OnPropertyChanged(change);
    }

    private void PopupClosed(object? sender, EventArgs e)
    {
        DropDownClosed?.Invoke(this, EventArgs.Empty);
    }

    private void PopupOpened(object? sender, EventArgs e)
    {
        DropDownOpened?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateSelectionBoxItem(object? item)
    {
        // ComboBoxItem/ContentControl must not be reused directly as SelectionBoxItem,
        // because they already have a visual parent in the ItemsPresenter tree.
        if (item is ComboBoxItem comboBoxItem)
        {
            SelectionBoxItem = comboBoxItem.Content;
        }
        else if (item is ContentControl { Content: not null } cc)
        {
            SelectionBoxItem = cc.Content;
        }
        else
        {
            SelectionBoxItem = item;
        }
    }
}
