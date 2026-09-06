using Android.App;
using Android.Content.PM;
using Android.OS;
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
    /// <summary>当前 Activity 实例，供 GeneralUpdate 启动安装器时使用。</summary>
    public static MainActivity? Current { get; private set; }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Current = this;
    }

    protected override void OnDestroy()
    {
        if (Current == this) Current = null;
        base.OnDestroy();
    }

    public override void OnBackPressed()
    {
        base.OnBackPressed();
    }
}
