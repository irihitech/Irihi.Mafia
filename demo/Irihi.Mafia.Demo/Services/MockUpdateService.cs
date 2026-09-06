using System;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace Irihi.Mafia.Demo.Services;

/// <summary>Mock 更新服务的演示场景。</summary>
public enum MockUpdateScenario
{
    /// <summary>完整成功流程：检查发现新版本 → 下载 → 完成 → 安装。</summary>
    Success,

    /// <summary>检查结果为已是最新版本（绿色对勾提示弹窗）。</summary>
    AlreadyLatest,

    /// <summary>检查阶段抛异常（模拟无法连接更新服务器），走 ViewModel 的失败分支。</summary>
    CheckFailed,

    /// <summary>下载到中途触发 <see cref="MockUpdateService.UpdateFailed"/>，用于演示下载失败弹窗。</summary>
    DownloadFailed,

    /// <summary>下载到 100% 后校验失败（SHA-256 不匹配），区别于下载中断。</summary>
    HashMismatch,

    /// <summary>下载正常完成，但安装阶段触发 <see cref="MockUpdateService.UpdateFailed"/>（模拟未授权安装）。</summary>
    InstallFailed,
}

/// <summary>
/// 模拟更新服务：用于桌面/iOS/Browser 等非 Android 平台演示更新 UI 流程。
/// 通过 <see cref="MockUpdateScenario"/> 切换不同分支/失败场景。
/// </summary>
public sealed class MockUpdateService : IUpdateService
{
    private const long TotalBytes = 10 * 1024 * 1024; // 10 MB

    /// <summary>DownloadFailed 场景下触发失败的进度阈值（百分比）。</summary>
    private const double FailAtPercent = 60;

    private readonly MockUpdateScenario _scenario;

    private DispatcherTimer? _timer;
    private double _progress;

    public string CurrentVersion => "0.0.1";

    public bool IsAvailable => true;

    public event Action<DownloadProgress>? ProgressChanged;
    public event Action? DownloadCompleted;
    public event Action<string>? UpdateFailed;

    public MockUpdateService(MockUpdateScenario scenario = MockUpdateScenario.Success)
    {
        _scenario = scenario;
    }

    public async Task<UpdateCheckResult> CheckForUpdateAsync()
    {
        await Task.Delay(1200);

        switch (_scenario)
        {
            case MockUpdateScenario.AlreadyLatest:
                return new UpdateCheckResult(UpdateFound: false, CurrentVersion, CurrentVersion);

            case MockUpdateScenario.CheckFailed:
                throw new InvalidOperationException("检查更新失败（模拟）：无法连接更新服务器");

            default:
                return new UpdateCheckResult(UpdateFound: true, CurrentVersion, "0.0.2");
        }
    }

    public Task DownloadAsync()
    {
        _progress = 0;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(120) };
        _timer.Tick += (_, _) =>
        {
            _progress = Math.Min(100, _progress + Random.Shared.Next(3, 7));

            // 下载中断：进行到一半时报错并停止。
            if (_scenario == MockUpdateScenario.DownloadFailed && _progress >= FailAtPercent)
            {
                _timer.Stop();
                UpdateFailed?.Invoke("下载失败（模拟）：网络连接中断");
                return;
            }

            var downloaded = (long)(TotalBytes * _progress / 100);
            ProgressChanged?.Invoke(new DownloadProgress(
                _progress,
                SpeedBytesPerSecond: 2.5 * 1024 * 1024,
                downloaded,
                TotalBytes,
                TotalBytes - downloaded,
                "downloading"));

            if (_progress >= 100)
            {
                _timer.Stop();

                // 校验失败：进度走满但 SHA-256 不匹配，与"下载中断"区分。
                if (_scenario == MockUpdateScenario.HashMismatch)
                {
                    UpdateFailed?.Invoke("校验失败（模拟）：APK 的 SHA-256 与服务器不一致");
                    return;
                }

                DownloadCompleted?.Invoke();
            }
        };
        _timer.Start();
        return Task.CompletedTask;
    }

    public Task InstallAsync()
    {
        Cancel();

        // 安装失败：下载已完成，但模拟系统拒绝安装（如未开启"允许安装未知来源"）。
        if (_scenario == MockUpdateScenario.InstallFailed)
            UpdateFailed?.Invoke("安装失败（模拟）：未授予安装未知来源应用的权限");

        return Task.CompletedTask;
    }

    public void Cancel() => _timer?.Stop();

    public void Dispose() => Cancel();
}
