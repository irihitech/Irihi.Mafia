using Android.App;
using GeneralUpdate.Avalonia.Android.Abstractions;

namespace Irihi.Mafia.Demo.Android.Services;

/// <summary>向 GeneralUpdate 提供当前 Activity，用于启动系统安装器。</summary>
internal sealed class MainActivityProvider : IAndroidActivityProvider
{
    public Activity? GetCurrentActivity() => MainActivity.Current;
}
