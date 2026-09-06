using System;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace Irihi.Mafia.Demo.Services;

/// <summary>
/// 模拟更新服务：用于桌面/iOS/Browser 等非 Android 平台演示更新 UI 流程。
/// </summary>
public sealed class MockUpdateService : IUpdateService
{
    private const long TotalBytes = 10 * 1024 * 1024; // 10 MB

    private DispatcherTimer? _timer;
    private double _progress;

    public string CurrentVersion => "0.0.1";

    public bool IsAvailable => true;

    public event Action<DownloadProgress>? ProgressChanged;
    public event Action? DownloadCompleted;
    public event Action<string>? UpdateFailed;

    public async Task<UpdateCheckResult> CheckForUpdateAsync()
    {
        await Task.Delay(1200);
        return new UpdateCheckResult(UpdateFound: true, CurrentVersion, "0.0.2");
    }

    public Task DownloadAsync()
    {
        _progress = 0;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(120) };
        _timer.Tick += (_, _) =>
        {
            _progress = Math.Min(100, _progress + Random.Shared.Next(3, 7));
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
                DownloadCompleted?.Invoke();
            }
        };
        _timer.Start();
        return Task.CompletedTask;
    }

    public Task InstallAsync()
    {
        Cancel();
        return Task.CompletedTask;
    }

    public void Cancel() => _timer?.Stop();

    public void Dispose() => Cancel();
}
