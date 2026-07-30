using Android.App;
using Android.Content.PM;
using Avalonia.Android;

namespace Irihi.Mafia.Demo.Android;

[Activity(
    Label = "Irihi.Mafia.Demo.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
    public override void OnBackPressed()
    {
        base.OnBackPressed();
        
    }
}
