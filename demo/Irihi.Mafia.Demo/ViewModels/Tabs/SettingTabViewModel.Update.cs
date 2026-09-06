using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Mafia.Demo.Services;

namespace Irihi.Mafia.Demo.ViewModels;

public partial class SettingTabViewModel
{
    private readonly IUpdateService? _updateService;

    public string CurrentVersion => _updateService.CurrentVersion;

    /// <summary>正在检查更新（转圈弹窗）。</summary>
    [ObservableProperty]
    public partial bool IsCheckingPopupOpen { get; set; }

    /// <summary>发现新版本确认弹窗。</summary>
    [ObservableProperty]
    public partial bool IsCheckUpdatePopupOpen { get; set; }

    /// <summary>下载进度弹窗。</summary>
    [ObservableProperty]
    public partial bool IsDownloadPopupOpen { get; set; }

    /// <summary>下载完成、确认安装弹窗。</summary>
    [ObservableProperty]
    public partial bool IsInstallPopupOpen { get; set; }

    /// <summary>提示弹窗（已是最新 / 更新失败）。</summary>
    [ObservableProperty]
    public partial bool IsMessagePopupOpen { get; set; }

    /// <summary>已是最新版本（决定提示弹窗显示的内容）。</summary>
    [ObservableProperty]
    public partial bool IsLatestVersion { get; set; }

    /// <summary>提示弹窗正文（失败原因等）。</summary>
    [ObservableProperty]
    public partial string MessageText { get; set; } = string.Empty;

    /// <summary>服务器上的新版本号。</summary>
    [ObservableProperty]
    public partial string? TargetVersion { get; set; }

    /// <summary>下载进度。</summary>
    [ObservableProperty]
    public partial DownloadProgress Progress { get; set; } = new(0, 0, 0, 0, 0);

    public SettingTabViewModel(IUpdateService? updateService = null)
    {
        _updateService = updateService ?? AppServices.Updater;
        if (_updateService is null) return;

        _updateService.ProgressChanged += p => Progress = p;
        _updateService.DownloadCompleted += OnDownloadCompleted;
        _updateService.UpdateFailed += OnUpdateFailed;
    }

    [RelayCommand]
    private async Task CheckUpdateAsync()
    {
        IsCheckingPopupOpen = true;
        try
        {
            var result = await _updateService.CheckForUpdateAsync();
            IsCheckingPopupOpen = false;

            if (result.UpdateFound)
            {
                TargetVersion = result.TargetVersion;
                IsCheckUpdatePopupOpen = true;
            }
            else
            {
                IsLatestVersion = true;
                IsMessagePopupOpen = true;
            }
        }
        catch (Exception ex)
        {
            IsCheckingPopupOpen = false;
            IsLatestVersion = false;
            MessageText = ex.Message;
            IsMessagePopupOpen = true;
        }
    }

    [RelayCommand]
    private void ConfirmUpdate()
    {
        IsCheckUpdatePopupOpen = false;
        IsDownloadPopupOpen = true;
        _ = _updateService.DownloadAsync();
    }

    [RelayCommand]
    private void CancelUpdate()
    {
        _updateService.Cancel();
        IsCheckUpdatePopupOpen = false;
        IsDownloadPopupOpen = false;
    }

    [RelayCommand]
    private void InstallConfirm()
    {
        IsInstallPopupOpen = false;
        _ = _updateService.InstallAsync();
    }

    [RelayCommand]
    private void InstallCancel() => IsInstallPopupOpen = false;

    [RelayCommand]
    private void CloseMessage() => IsMessagePopupOpen = false;

    private void OnDownloadCompleted()
    {
        IsDownloadPopupOpen = false;
        IsInstallPopupOpen = true;
    }

    private void OnUpdateFailed(string message)
    {
        IsCheckingPopupOpen = false;
        IsDownloadPopupOpen = false;
        IsLatestVersion = false;
        MessageText = message;
        IsMessagePopupOpen = true;
    }
}