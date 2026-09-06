using Irihi.Mafia.Demo.Services;

namespace Irihi.Mafia.Demo.Android.Services;

/// <summary>
/// 更新相关的配置。接入真实更新服务器时，把下面的占位值替换成你自己的信息。
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

    /// <summary>服务器上的最新版本信息（占位示例，请替换）。</summary>
    public static readonly UpdatePackageInfo Latest = new(
        Version: "0.0.2",
        DownloadUrl: "https://example.com/app-release.apk",
        FileName: "app-release.apk",
        FileSize: 0,
        Sha256: null);
}
