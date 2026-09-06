using System;
using System.Threading.Tasks;

namespace Irihi.Mafia.Demo.Services;

/// <summary>待检查/下载的更新包信息。</summary>
public sealed record UpdatePackageInfo(
    string Version,
    string DownloadUrl,
    string FileName,
    long FileSize,
    string? Sha256 = null);

/// <summary>检查更新的结果。</summary>
public sealed record UpdateCheckResult(
    bool UpdateFound,
    string? CurrentVersion = null,
    string? TargetVersion = null,
    string? Message = null);

/// <summary>下载进度信息。</summary>
public sealed record DownloadProgress(
    double ProgressPercentage,
    double SpeedBytesPerSecond,
    long DownloadedBytes,
    long TotalBytes,
    long RemainingBytes,
    string? StatusDescription = null)
{
    public double SpeedMbPerSecond => SpeedBytesPerSecond / 1024 / 1024;
}

/// <summary>
/// 应用更新服务抽象。共享 UI 项目只依赖此接口，
/// 由各平台提供实现（Android 使用 GeneralUpdate.Avalonia.Android，桌面/浏览器使用 Mock）。
/// </summary>
public interface IUpdateService : IDisposable
{
    /// <summary>当前应用版本号。</summary>
    string CurrentVersion { get; }

    /// <summary>当前平台是否支持自动更新。</summary>
    bool IsAvailable { get; }

    /// <summary>下载进度变化（UI 线程回调）。</summary>
    event Action<DownloadProgress>? ProgressChanged;

    /// <summary>下载并校验完成（UI 线程回调）。</summary>
    event Action? DownloadCompleted;

    /// <summary>更新流程失败（UI 线程回调），参数为错误消息。</summary>
    event Action<string>? UpdateFailed;

    /// <summary>检查是否有新版本。</summary>
    Task<UpdateCheckResult> CheckForUpdateAsync();

    /// <summary>下载更新包（含 SHA-256 校验），结果通过事件上报，不抛异常。</summary>
    Task DownloadAsync();

    /// <summary>启动安装。</summary>
    Task InstallAsync();

    /// <summary>取消下载。</summary>
    void Cancel();
}
