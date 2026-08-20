using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Irihi.Mafia.Themes.TDesign;

namespace Irihi.Mafia.HeadlessTest;

/// <summary>
/// Default test application with TDesign theme.
/// Used by AvaloniaFact and AvaloniaTheory attributes.
/// </summary>
public class TestApp : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Apply TDesign theme
        Styles.Add(new TDesignTheme());
        base.OnFrameworkInitializationCompleted();
    }
}
