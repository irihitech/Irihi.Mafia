using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace Irihi.Mirana.Controls;

[PseudoClasses(":expanded")]
[TemplatePart(PART_HeaderButton, typeof(Button))]
public class CollapsePanel : HeaderedContentControl
{
    public const string PART_HeaderButton = "PART_HeaderButton";

    private Button? _headerButton;

    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<CollapsePanel, bool>(nameof(IsExpanded));

    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    static CollapsePanel()
    {
        IsExpandedProperty.Changed.AddClassHandler<CollapsePanel>((panel, args) =>
        {
            panel.PseudoClasses.Set(":expanded", args.GetNewValue<bool>());
        });
    }

    public static readonly StyledProperty<object?> HeaderLeftContentProperty =
        AvaloniaProperty.Register<CollapsePanel, object?>(nameof(HeaderLeftContent));

    public object? HeaderLeftContent
    {
        get => GetValue(HeaderLeftContentProperty);
        set => SetValue(HeaderLeftContentProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> HeaderLeftContentTemplateProperty =
        AvaloniaProperty.Register<CollapsePanel, IDataTemplate?>(nameof(HeaderLeftContentTemplate));

    public IDataTemplate? HeaderLeftContentTemplate
    {
        get => GetValue(HeaderLeftContentTemplateProperty);
        set => SetValue(HeaderLeftContentTemplateProperty, value);
    }

    public static readonly StyledProperty<object?> HeaderRightContentProperty =
        AvaloniaProperty.Register<CollapsePanel, object?>(nameof(HeaderRightContent));

    public object? HeaderRightContent
    {
        get => GetValue(HeaderRightContentProperty);
        set => SetValue(HeaderRightContentProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> HeaderRightContentTemplateProperty =
        AvaloniaProperty.Register<CollapsePanel, IDataTemplate?>(nameof(HeaderRightContentTemplate));

    public IDataTemplate? HeaderRightContentTemplate
    {
        get => GetValue(HeaderRightContentTemplateProperty);
        set => SetValue(HeaderRightContentTemplateProperty, value);
    }

    public static readonly StyledProperty<bool> ShowExpandIconProperty =
        AvaloniaProperty.Register<CollapsePanel, bool>(nameof(ShowExpandIcon), defaultValue: true);

    public bool ShowExpandIcon
    {
        get => GetValue(ShowExpandIconProperty);
        set => SetValue(ShowExpandIconProperty, value);
    }

    public static readonly StyledProperty<object?> ValueProperty =
        AvaloniaProperty.Register<CollapsePanel, object?>(nameof(Value));

    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        // Unsubscribe from previous button if it exists
        if (_headerButton != null)
        {
            _headerButton.Click -= OnHeaderButtonClick;
        }

        if (e.NameScope.Find<Button>(PART_HeaderButton) is { } button)
        {
            _headerButton = button;
            _headerButton.Click += OnHeaderButtonClick;
        }
    }

    private void OnHeaderButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        IsExpanded = !IsExpanded;
    }
}
