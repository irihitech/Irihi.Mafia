using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Android.Content.PM;
using Avalonia.Threading;
using GeneralUpdate.Avalonia.Android;
using GeneralUpdate.Avalonia.Android.Abstractions;
using GeneralUpdate.Avalonia.Android.Models;
using Irihi.Mafia.Demo.Services;
using UpdateCheckResult = Irihi.Mafia.Demo.Services.UpdateCheckResult;
using UpdatePackageInfo = GeneralUpdate.Avalonia.Android.Models.UpdatePackageInfo;

namespace Irihi.Mafia.Demo.Android.Services;

/// <summary>
/// 基于 GeneralUpdate.Avalonia.Android 的真实更新服务。
/// 流程：CheckForUpdateAsync（向更新服务器查询最新包并比较版本）→ DownloadAsync（断点续传 + SHA-256 校验）→ InstallAsync（安装）。
/// 最新包信息由组件从 UpdateConfig.UpdateServer 查询得到，客户端不内置。
/// 所有事件都会调度到 UI 线程后触发。
/// </summary>
public sealed class GeneralUpdateService : IUpdateService
{
    private readonly IAndroidBootstrap _bootstrap;
    private UpdatePackageInfo? _packageInfo;
    private CancellationTokenSource? _cts;
    private string? _apkFilePath;

    public string CurrentVersion =>
        Application.Context.PackageManager
            ?.GetPackageInfo(Application.Context.PackageName ?? string.Empty, default(PackageInfoFlags))
            ?.VersionName
        ?? UpdateConfig.CurrentVersion;

    public bool IsAvailable => true;

    public event Action<DownloadProgress>? ProgressChanged;
    public event Action? DownloadCompleted;
    public event Action<string>? UpdateFailed;

    public GeneralUpdateService()
    {
        var options = new AndroidUpdateOptions
        {
            DownloadDirectoryPath = Path.Combine(Application.Context.CacheDir!.AbsolutePath!, "update"),
            FileProviderAuthority = UpdateConfig.FileProviderAuthority,
            UpdateServer = UpdateConfig.UpdateServer,
            // 更新服务器当前为 HTTP，组件默认强制 HTTPS 下载，需显式放行。
            AllowInsecureHttpDownloads = true,
        };

        _bootstrap = GeneralUpdateBootstrap.CreateDefault(
            options,
            activityProvider: new MainActivityProvider());

        _bootstrap.AddListenerDownloadProgressChanged += (_, e) =>
            Dispatcher.UIThread.Post(() => ProgressChanged?.Invoke(new DownloadProgress(
                e.ProgressPercentage,
                e.DownloadSpeedBytesPerSecond,
                e.DownloadedBytes,
                e.TotalBytes,
                e.RemainingBytes,
                e.StatusDescription)));

        _bootstrap.AddListenerUpdateCompleted += (_, _) =>
            Dispatcher.UIThread.Post(() => DownloadCompleted?.Invoke());

        _bootstrap.AddListenerUpdateFailed += (_, e) =>
            Dispatcher.UIThread.Post(() => UpdateFailed?.Invoke(e.Result?.Message ?? "Update failed."));
    }

    public async Task<UpdateCheckResult> CheckForUpdateAsync()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        var result = await _bootstrap.ValidateAsync(CurrentVersion, _cts.Token);
        if (!result.Success)
            throw new InvalidOperationException(result.Message ?? "Check for update failed.");

        _packageInfo = result.PackageInfo;
        return new UpdateCheckResult(
            result.UpdateFound,
            result.CurrentVersion,
            result.TargetVersion,
            result.Message);
    }

    public async Task DownloadAsync()
    {
        try
        {
            if (_packageInfo is null)
            {
                UpdateFailed?.Invoke("No update package info. Check for updates first.");
                return;
            }

            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            var result = await _bootstrap.DownloadAndVerifyAsync(_packageInfo, _cts.Token);
            if (!result.Success)
                return; // 失败由 AddListenerUpdateFailed 事件上报

            _apkFilePath = result.FilePath;
            // 成功时由 AddListenerUpdateCompleted 事件通知 DownloadCompleted
        }
        catch (OperationCanceledException)
        {
            // 用户主动取消，不提示
        }
        catch (Exception ex)
        {
            UpdateFailed?.Invoke(ex.Message);
        }
    }

    public async Task InstallAsync()
    {
        if (string.IsNullOrEmpty(_apkFilePath) || _packageInfo is null)
        {
            UpdateFailed?.Invoke("APK file not found.");
            return;
        }

        try
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            var result = await _bootstrap.LaunchInstallerAsync(_packageInfo, _apkFilePath, _cts.Token);
            if (!result.Success)
                return; // 失败由 AddListenerUpdateFailed 事件上报
        }
        catch (Exception ex)
        {
            UpdateFailed?.Invoke(ex.Message);
        }
    }

    public void Cancel() => _cts?.Cancel();

    public void Dispose() => _bootstrap.Dispose();
}