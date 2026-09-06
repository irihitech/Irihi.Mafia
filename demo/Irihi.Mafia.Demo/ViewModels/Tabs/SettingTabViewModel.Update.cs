using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Mafia.Demo.Services;

namespace Irihi.Mafia.Demo.ViewModels;

public enum UpdatePopupState
{
    /// <summary>无弹窗。</summary>
    None,

    /// <summary>正在检查更新。</summary>
    Checking,

    /// <summary>发现新版本，询问是否下载。</summary>
    NewVersion,

    /// <summary>正在下载更新。</summary>
    Downloading,

    /// <summary>下载完成，询问是否安装。</summary>
    ReadyToInstall,

    /// <summary>提示信息（已是最新 / 更新失败），正文见 MessageText。</summary>
    Notice,
}

public partial class SettingTabViewModel
{
    private readonly IUpdateService _updateService;

    public string CurrentVersion => _updateService.CurrentVersion;

    /// <summary>更新弹窗状态</summary>
    [ObservableProperty]
    public partial UpdatePopupState UpdatePopupState { get; set; }

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
        _updateService = updateService ?? AppServices.Updater!;

        _updateService.ProgressChanged += p => Progress = p;
        _updateService.DownloadCompleted += OnDownloadCompleted;
        _updateService.UpdateFailed += OnUpdateFailed;
    }

    [RelayCommand]
    private async Task CheckUpdateAsync()
    {
        UpdatePopupState = UpdatePopupState.Checking;
        try
        {
            var result = await _updateService.CheckForUpdateAsync();

            if (result.UpdateFound)
            {
                TargetVersion = result.TargetVersion;
                UpdatePopupState = UpdatePopupState.NewVersion;
            }
            else
            {
                IsLatestVersion = true;
                UpdatePopupState = UpdatePopupState.Notice;
            }
        }
        catch (Exception ex)
        {
            IsLatestVersion = false;
            MessageText = ex.Message;
            UpdatePopupState = UpdatePopupState.Notice;
        }
    }

    [RelayCommand]
    private void ConfirmUpdate()
    {
        UpdatePopupState = UpdatePopupState.Downloading;
        _ = _updateService.DownloadAsync();
    }

    [RelayCommand]
    private void CancelUpdate()
    {
        _updateService.Cancel();
        UpdatePopupState = UpdatePopupState.None;
    }

    [RelayCommand]
    private void InstallConfirm()
    {
        UpdatePopupState = UpdatePopupState.None;
        _ = _updateService.InstallAsync();
    }

    [RelayCommand]
    private void InstallCancel() => UpdatePopupState = UpdatePopupState.None;

    [RelayCommand]
    private void CloseMessage() => UpdatePopupState = UpdatePopupState.None;

    private void OnDownloadCompleted() => UpdatePopupState = UpdatePopupState.ReadyToInstall;

    private void OnUpdateFailed(string message)
    {
        IsLatestVersion = false;
        MessageText = message;
        UpdatePopupState = UpdatePopupState.Notice;
    }
}
