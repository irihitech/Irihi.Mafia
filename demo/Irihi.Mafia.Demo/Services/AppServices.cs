namespace Irihi.Mafia.Demo.Services;

public static class AppServices
{
    public static INavigationService? Navigator { get; set; }
    public static IUpdateService? Updater { get; set; } = new MockUpdateService();
}