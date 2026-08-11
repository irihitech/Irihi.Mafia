using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Irihi.Avalonia.Shared.Contracts;

namespace Irihi.Mafia.Controls;

public class Input : TextBox, IInnerContentControl, ICell
{
    public static readonly StyledProperty<bool> IsRequiredProperty =
        Cell.IsRequiredProperty.AddOwner<Input>();

    public bool IsRequired
    {
        get => GetValue(IsRequiredProperty);
        set => SetValue(IsRequiredProperty, value);
    }

    public static readonly StyledProperty<object?> DescriptionProperty =
        Cell.DescriptionProperty.AddOwner<Input>();

    public object? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> DescriptionTemplateProperty =
        Cell.DescriptionTemplateProperty.AddOwner<Input>();

    public IDataTemplate? DescriptionTemplate
    {
        get => GetValue(DescriptionTemplateProperty);
        set => SetValue(DescriptionTemplateProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> InnerLeftContentTemplateProperty =
        Cell.InnerLeftContentTemplateProperty.AddOwner<Input>();

    public IDataTemplate? InnerLeftContentTemplate
    {
        get => GetValue(InnerLeftContentTemplateProperty);
        set => SetValue(InnerLeftContentTemplateProperty, value);
    }

    public static readonly StyledProperty<IDataTemplate?> InnerRightContentTemplateProperty =
        Cell.InnerRightContentTemplateProperty.AddOwner<Input>();

    public IDataTemplate? InnerRightContentTemplate
    {
        get => GetValue(InnerRightContentTemplateProperty);
        set => SetValue(InnerRightContentTemplateProperty, value);
    }
}
