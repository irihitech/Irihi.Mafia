using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Templates;
using Irihi.Avalonia.Shared.Contracts;

namespace Irihi.Mirana.Controls;

[TemplatePart(PART_RootPanel, typeof(Panel))]
[PseudoClasses(PC_Bordered, PC_Arrow, PC_Required)]
public class Cell : Button, IInnerContentControl
{
    public const string PC_Bordered = ":bordered";
    public const string PC_Arrow = ":arrow";
    public const string PC_Required = ":required";
    public const string PART_RootPanel = "PART_RootPanel";

    public static readonly StyledProperty<object?> DescriptionProperty =
        AvaloniaProperty.Register<Cell, object?>(nameof(Description));

    public object? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> DescriptionTemplateProperty =
        AvaloniaProperty.Register<Cell, IDataTemplate?>(nameof(DescriptionTemplate));

    public IDataTemplate? DescriptionTemplate
    {
        get => GetValue(DescriptionTemplateProperty);
        set => SetValue(DescriptionTemplateProperty, value);
    }

    public static readonly StyledProperty<object?> NoteProperty =
        AvaloniaProperty.Register<Cell, object?>(nameof(Note));

    public object? Note
    {
        get => GetValue(NoteProperty);
        set => SetValue(NoteProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> NoteTemplateProperty =
        AvaloniaProperty.Register<Cell, IDataTemplate?>(nameof(NoteTemplate));

    public IDataTemplate? NoteTemplate
    {
        get => GetValue(NoteTemplateProperty);
        set => SetValue(NoteTemplateProperty, value);
    }

    public static readonly StyledProperty<object?> ImageProperty =
        AvaloniaProperty.Register<Cell, object?>(nameof(Image));

    public object? Image
    {
        get => GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> ImageTemplateProperty =
        AvaloniaProperty.Register<Cell, IDataTemplate?>(nameof(ImageTemplate));

    public IDataTemplate? ImageTemplate
    {
        get => GetValue(ImageTemplateProperty);
        set => SetValue(ImageTemplateProperty, value);
    }

    public static readonly StyledProperty<object?> InnerLeftContentProperty =
        AvaloniaProperty.Register<Cell, object?>(nameof(InnerLeftContent));


    public object? InnerLeftContent
    {
        get => GetValue(InnerLeftContentProperty);
        set => SetValue(InnerLeftContentProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> InnerLeftContentTemplateProperty =
        AvaloniaProperty.Register<Cell, IDataTemplate?>(nameof(InnerLeftContentTemplate));

    public IDataTemplate? InnerLeftContentTemplate
    {
        get => GetValue(InnerLeftContentTemplateProperty);
        set => SetValue(InnerLeftContentTemplateProperty, value);
    }

    public static readonly StyledProperty<object?> InnerRightContentProperty =
        AvaloniaProperty.Register<Cell, object?>(nameof(InnerRightContent));

    public object? InnerRightContent
    {
        get => GetValue(InnerRightContentProperty);
        set => SetValue(InnerRightContentProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> InnerRightContentTemplateProperty =
        AvaloniaProperty.Register<Cell, IDataTemplate?>(nameof(InnerRightContentTemplate));

    public IDataTemplate? InnerRightContentTemplate
    {
        get => GetValue(InnerRightContentTemplateProperty);
        set => SetValue(InnerRightContentTemplateProperty, value);
    }
}