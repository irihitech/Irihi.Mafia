using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace Irihi.Mirana.Controls;

[TemplatePart(PART_RootPanel, typeof(Panel))]
[PseudoClasses(PC_AlignTop, PC_AlignMiddle, PC_AlignBottom, PC_Bordered, PC_Arrow, PC_Required)]
public class Cell : Button
{
    public const string PC_AlignTop = ":align-top";
    public const string PC_AlignMiddle = ":align-middle";
    public const string PC_AlignBottom = ":align-bottom";
    public const string PC_Bordered = ":bordered";
    public const string PC_Arrow = ":arrow";
    public const string PC_Required = ":required";
    public const string PART_RootPanel = "PART_RootPanel";

    // Title property
    public static readonly StyledProperty<object?> TitleProperty =
        AvaloniaProperty.Register<Cell, object?>(nameof(Title));

    public object? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> TitleTemplateProperty =
        AvaloniaProperty.Register<Cell, IDataTemplate?>(nameof(TitleTemplate));

    public IDataTemplate? TitleTemplate
    {
        get => GetValue(TitleTemplateProperty);
        set => SetValue(TitleTemplateProperty, value);
    }

    // Description property
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

    // Note property (appears on same line as title)
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

    // Image property
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

    // Left Icon property
    public static readonly StyledProperty<object?> LeftIconProperty =
        AvaloniaProperty.Register<Cell, object?>(nameof(LeftIcon));

    public object? LeftIcon
    {
        get => GetValue(LeftIconProperty);
        set => SetValue(LeftIconProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> LeftIconTemplateProperty =
        AvaloniaProperty.Register<Cell, IDataTemplate?>(nameof(LeftIconTemplate));

    public IDataTemplate? LeftIconTemplate
    {
        get => GetValue(LeftIconTemplateProperty);
        set => SetValue(LeftIconTemplateProperty, value);
    }

    // Right Icon property
    public static readonly StyledProperty<object?> RightIconProperty =
        AvaloniaProperty.Register<Cell, object?>(nameof(RightIcon));

    public object? RightIcon
    {
        get => GetValue(RightIconProperty);
        set => SetValue(RightIconProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> RightIconTemplateProperty =
        AvaloniaProperty.Register<Cell, IDataTemplate?>(nameof(RightIconTemplate));

    public IDataTemplate? RightIconTemplate
    {
        get => GetValue(RightIconTemplateProperty);
        set => SetValue(RightIconTemplateProperty, value);
    }

    // Align property - controls right content alignment (using VerticalAlignment)
    public static readonly StyledProperty<VerticalAlignment> AlignProperty =
        AvaloniaProperty.Register<Cell, VerticalAlignment>(nameof(Align), defaultValue: VerticalAlignment.Center);

    public VerticalAlignment Align
    {
        get => GetValue(AlignProperty);
        set => SetValue(AlignProperty, value);
    }

    static Cell()
    {
        AlignProperty.Changed.AddClassHandler<Cell, VerticalAlignment>((cell, e) => UpdateAlignPseudoClasses(cell, e.NewValue.Value));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdateAlignPseudoClasses(this, Align);
    }

    private static void UpdateAlignPseudoClasses(Cell cell, VerticalAlignment align)
    {
        IPseudoClasses pseudo = cell.Classes;
        pseudo.Set(PC_AlignTop, align == VerticalAlignment.Top);
        pseudo.Set(PC_AlignMiddle, align == VerticalAlignment.Center);
        pseudo.Set(PC_AlignBottom, align == VerticalAlignment.Bottom);
    }
}
