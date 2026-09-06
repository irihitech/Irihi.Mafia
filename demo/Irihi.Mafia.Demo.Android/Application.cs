using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using Irihi.Mafia.Demo.Android.Services;
using Irihi.Mafia.Demo.Services;

namespace Irihi.Mafia.Demo.Android;

[Application]
public class Application : AvaloniaAndroidApplication<App>
{
    protected Application(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();
        AppServices.Updater = new GeneralUpdateService();
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder);
    }
}
