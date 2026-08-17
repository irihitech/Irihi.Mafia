using Avalonia;
using Avalonia.Headless;
using Irihi.Mafia.HeadlessTest;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

namespace Irihi.Mafia.HeadlessTest;

public class TestAppBuilder
{
    /// <summary>
    /// Builds the Avalonia test application with the headless platform and control library theme.
    /// </summary>
    /// <returns>The configured <see cref="AppBuilder"/>.</returns>
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<TestApp>()
                  .UseHarfBuzz()
                  .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
