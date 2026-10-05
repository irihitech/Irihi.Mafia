using GeneralUpdate.Avalonia.Android.Models;

namespace Irihi.Mafia.Demo.Android.Services;

/// <summary>
/// 更新相关的配置。接入真实更新服务器时，把下面的占位值替换成你自己的信息。
/// 最新包信息由组件在运行时向更新服务器查询，客户端不再内置。
/// </summary>
public static class UpdateConfig
{
    /// <summary>
    /// 当前应用版本兜底值。正常情况下 GeneralUpdateService 会从 APK 的
    /// PackageInfo.VersionName（即 csproj 的 ApplicationDisplayVersion）读取，
    /// 只有读取失败时才回退到这里。
    /// </summary>
    public const string CurrentVersion = "0.0.1";

    /// <summary>FileProvider authority，必须与 AndroidManifest.xml 中 provider 的 android:authorities 一致。</summary>
    public const string FileProviderAuthority = "tech.irihi.Mafia.Demo.generalupdate.fileprovider";

    /// <summary>更新服务器（占位示例，请替换）。ValidateAsync 会用下面的标识信息向服务器查询最新包。</summary>
    public static UpdateServerOptions UpdateServer { get; } = new()
    {
        // GeneralUpdate 参考协议的校验接口；改用自建 JSON 接口时设 UseJsonEndpoint = true。
        RequestUrl = "https://example.com/Upgrade/Verification",
        AppKey = "your-app-key",
        ProductId = "your-product-id",
        // AppType / Platform 为服务器约定的整型标识，需与服务端配置一致。
        AppType = 1,
        Platform = 1,
        UseJsonEndpoint = false,
    };
}
