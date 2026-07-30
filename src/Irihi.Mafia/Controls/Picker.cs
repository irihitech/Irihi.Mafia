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
[TemplatePart("PART_ConfirmButton", typeof(Button), IsRequired = false)]
[TemplatePart("PART_CancelButton", typeof(Button), IsRequired = false)]
public class Picker : SelectingItemsControl, ICell
{

    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel());

    /// <summary>
    /// Defines the <see cref="IsConfirmable"/> property.
    /// When true, the user must press a confirm button in the dropdown to commit
    /// the selection; closing without confirming restores the previous value.
    /// </summary>
    public static readonly StyledProperty<bool> IsConfirmableProperty =
        AvaloniaProperty.Register<Picker, bool>(nameof(IsConfirmable));

    /// <summary>
    /// Defines the <see cref="PopupTitle"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> PopupTitleProperty =
        AvaloniaProperty.Register<Picker, string?>(nameof(PopupTitle));

    /// <summary>
    /// Defines the <see cref="IsDropDownOpen"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<Picker, bool>(nameof(IsDropDownOpen));

    /// <summary>
    /// Defines the <see cref="MaxDropDownHeight"/> property.
    /// </summary>
    public static readonly StyledProperty<double> MaxDropDownHeightProperty =
        AvaloniaProperty.Register<Picker, double>(nameof(MaxDropDownHeight), 200);

    /// <summary>
    /// Defines the <see cref="SelectionBoxItem"/> property.
    /// </summary>
    public static readonly DirectProperty<Picker, object?> SelectionBoxItemProperty =
        AvaloniaProperty.RegisterDirect<Picker, object?>(nameof(SelectionBoxItem), o => o.SelectionBoxItem);

    /// <summary>
    /// Defines the <see cref="PlaceholderText"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<Picker, string?>(nameof(PlaceholderText));

    /// <summary>
    /// Defines the <see cref="PlaceholderForeground"/> property.
    /// </summary>
    public static readonly StyledProperty<IBrush?> PlaceholderForegroundProperty =
        AvaloniaProperty.Register<Picker, IBrush?>(nameof(PlaceholderForeground));

    /// <summary>
    /// Defines the <see cref="HorizontalContentAlignment"/> property.
    /// </summary>
    public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
        ContentControl.HorizontalContentAlignmentProperty.AddOwner<Picker>();

    /// <summary>
    /// Defines the <see cref="VerticalContentAlignment"/> property.
    /// </summary>
    public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
        ContentControl.VerticalContentAlignmentProperty.AddOwner<Picker>();

    /// <summary>
    /// Defines the <see cref="SelectionBoxItemTemplate"/> property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> SelectionBoxItemTemplateProperty =
        AvaloniaProperty.Register<Picker, IDataTemplate?>(nameof(SelectionBoxItemTemplate));

    /// <summary>
    /// Defines the <see cref="Description"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> DescriptionProperty =
        AvaloniaProperty.Register<Picker, object?>(nameof(Description));

    /// <summary>
    /// Defines the <see cref="DescriptionTemplate"/> property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> DescriptionTemplateProperty =
        AvaloniaProperty.Register<Picker, IDataTemplate?>(nameof(DescriptionTemplate));

    /// <summary>
    /// Defines the <see cref="Note"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> NoteProperty =
        AvaloniaProperty.Register<Picker, object?>(nameof(Note));

    /// <summary>
    /// Defines the <see cref="NoteTemplate"/> property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> NoteTemplateProperty =
        AvaloniaProperty.Register<Picker, IDataTemplate?>(nameof(NoteTemplate));

    /// <summary>
    /// Defines the <see cref="InnerLeftContent"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> InnerLeftContentProperty =
        AvaloniaProperty.Register<Picker, object?>(nameof(InnerLeftContent));

    /// <summary>
    /// Defines the <see cref="InnerLeftContentTemplate"/> property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> InnerLeftContentTemplateProperty =
        AvaloniaProperty.Register<Picker, IDataTemplate?>(nameof(InnerLeftContentTemplate));

    /// <summary>
    /// Defines the <see cref="InnerRightContent"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> InnerRightContentProperty =
        AvaloniaProperty.Register<Picker, object?>(nameof(InnerRightContent));

    /// <summary>
    /// Defines the <see cref="InnerRightContentTemplate"/> property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> InnerRightContentTemplateProperty =
        AvaloniaProperty.Register<Picker, IDataTemplate?>(nameof(InnerRightContentTemplate));

    private Popup? _popup;
    private object? _selectionBoxItem;
    private Button? _confirmButton;
    private Button? _cancelButton;
    private int _savedSelectedIndex;
    private bool _isConfirmed;

    /// <summary>
    /// Initializes static members of the <see cref="Picker"/> class.
    /// </summary>
    static Picker()
    {
        ItemsPanelProperty.OverrideDefaultValue<Picker>(DefaultPanel);
        FocusableProperty.OverrideDefaultValue<Picker>(true);
    }

    /// <summary>
    /// Occurs after the drop-down list of the <see cref="Picker"/> closes.
    /// </summary>
    public event EventHandler? DropDownClosed;

    /// <summary>
    /// Occurs after the drop-down list of the <see cref="Picker"/> opens.
    /// </summary>
    public event EventHandler? DropDownOpened;

    /// <summary>
    /// Gets or sets a value indicating whether a confirm button is shown in the dropdown.
    /// When true, selection changes are only committed when the user presses confirm.
    /// </summary>
    public bool IsConfirmable
    {
        get => GetValue(IsConfirmableProperty);
        set => SetValue(IsConfirmableProperty, value);
    }

    /// <summary>
    /// Gets or sets the title text shown at the top of the dropdown popup.
    /// </summary>
    public string? PopupTitle
    {
        get => GetValue(PopupTitleProperty);
        set => SetValue(PopupTitleProperty, value);
    }

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

    /// <summary>
    /// Gets or sets the description text displayed below the selection box.
    /// </summary>
    public object? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>
    /// Gets or sets the data template used to display the description.
    /// </summary>
    public IDataTemplate? DescriptionTemplate
    {
        get => GetValue(DescriptionTemplateProperty);
        set => SetValue(DescriptionTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the note text displayed on the right side.
    /// </summary>
    public object? Note
    {
        get => GetValue(NoteProperty);
        set => SetValue(NoteProperty, value);
    }

    /// <summary>
    /// Gets or sets the data template used to display the note.
    /// </summary>
    public IDataTemplate? NoteTemplate
    {
        get => GetValue(NoteTemplateProperty);
        set => SetValue(NoteTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the content displayed on the inner left side.
    /// </summary>
    public object? InnerLeftContent
    {
        get => GetValue(InnerLeftContentProperty);
        set => SetValue(InnerLeftContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the data template used to display the inner left content.
    /// </summary>
    public IDataTemplate? InnerLeftContentTemplate
    {
        get => GetValue(InnerLeftContentTemplateProperty);
        set => SetValue(InnerLeftContentTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the content displayed on the inner right side.
    /// </summary>
    public object? InnerRightContent
    {
        get => GetValue(InnerRightContentProperty);
        set => SetValue(InnerRightContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the data template used to display the inner right content.
    /// </summary>
    public IDataTemplate? InnerRightContentTemplate
    {
        get => GetValue(InnerRightContentTemplateProperty);
        set => SetValue(InnerRightContentTemplateProperty, value);
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
        return new PickerItem();
    }

    /// <inheritdoc/>
    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<PickerItem>(item, out recycleKey);
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
    }

    /// <inheritdoc/>
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (!e.Handled && !IsDropDownOpen)
        {
            SetCurrentValue(IsDropDownOpenProperty, true);
            e.Handled = true;
        }

        base.OnPointerReleased(e);
    }

    /// <inheritdoc/>
    public override bool UpdateSelectionFromEvent(Control container, RoutedEventArgs eventArgs)
    {
        if (base.UpdateSelectionFromEvent(container, eventArgs))
        {
            if (!IsConfirmable)
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

        if (_confirmButton != null)
            _confirmButton.Click -= OnConfirmClick;

        if (_cancelButton != null)
            _cancelButton.Click -= OnCancelClick;

        _popup = e.NameScope.Get<Popup>("PART_Popup");
        _popup.Opened += PopupOpened;
        _popup.Closed += PopupClosed;

        _confirmButton = e.NameScope.Find<Button>("PART_ConfirmButton");
        if (_confirmButton != null)
            _confirmButton.Click += OnConfirmClick;

        _cancelButton = e.NameScope.Find<Button>("PART_CancelButton");
        if (_cancelButton != null)
            _cancelButton.Click += OnCancelClick;
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == SelectedItemProperty)
        {
            UpdateSelectionBoxItem(change.NewValue);
        }

        base.OnPropertyChanged(change);
    }

    private void PopupClosed(object? sender, EventArgs e)
    {
        if (IsConfirmable && !_isConfirmed)
        {
            var saved = _savedSelectedIndex;
            if (saved != -1)
                SelectedIndex = saved;
            else
                SelectedItem = null;
        }

        DropDownClosed?.Invoke(this, EventArgs.Empty);
    }

    private void PopupOpened(object? sender, EventArgs e)
    {
        _savedSelectedIndex = SelectedIndex;
        _isConfirmed = false;

        DropDownOpened?.Invoke(this, EventArgs.Empty);
    }

    private void OnConfirmClick(object? sender, RoutedEventArgs e)
    {
        _isConfirmed = true;
        _popup?.Close();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        _isConfirmed = false;
        _popup?.Close();
    }

    private void UpdateSelectionBoxItem(object? item)
    {
        // PickerItem/ContentControl must not be reused directly as SelectionBoxItem,
        // because they already have a visual parent in the ItemsPresenter tree.
        if (item is PickerItem comboBoxItem)
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
