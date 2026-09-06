using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Mafia.Demo.Services;

namespace Irihi.Mafia.Demo.ViewModels;

/// <summary>
/// 设置页当前展示的弹窗状态。整个页面同一时刻只有一个弹窗，
/// 用单个枚举表达当前状态，避免多个 IsXxxPopupOpen 布尔互相矛盾。
/// </summary>
public enum UpdatePopupState
{
    /// <summary>无弹窗。</summary>
    None,

    /// <summary>正在检查更新。</summary>
    Checking,

    /// <summary>发现新版本，询问是否下载。</summary>
    UpdateAvailable,

    /// <summary>正在下载更新。</summary>
    Downloading,

    /// <summary>下载完成，询问是否安装。</summary>
    ReadyToInstall,

    /// <summary>提示信息（已是最新 / 更新失败），正文见 MessageText。</summary>
    Notice,
}

public partial class SettingTabViewModel
{
    private IUpdateService _updateService;

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

    #region MockPicker

    [ObservableProperty] public partial int SelectedScenarioIndex { get; set; }

    /// <summary>模拟场景与选项文案。</summary>
    private static readonly List<(MockUpdateScenario Scenario, IObservable<string?> Label)> MockScenarios =
    [
        (MockUpdateScenario.Success, LanguageManager.Instance.Update_Scenario_Success),
        (MockUpdateScenario.AlreadyLatest, LanguageManager.Instance.Update_Scenario_AlreadyLatest),
        (MockUpdateScenario.CheckFailed, LanguageManager.Instance.Update_Scenario_CheckFailed),
        (MockUpdateScenario.DownloadFailed, LanguageManager.Instance.Update_Scenario_DownloadFailed),
        (MockUpdateScenario.HashMismatch, LanguageManager.Instance.Update_Scenario_HashMismatch),
        (MockUpdateScenario.InstallFailed, LanguageManager.Instance.Update_Scenario_InstallFailed),
    ];

    /// <summary>模拟场景选项。</summary>
    public ObservableCollection<IObservable<string?>> ScenarioOptions { get; } = new(MockScenarios.Select(x => x.Label));

    public SettingTabViewModel(IUpdateService? updateService = null)
    {
        _updateService = updateService ?? AppServices.Updater!;
        Subscribe(_updateService);
    }

    partial void OnSelectedScenarioIndexChanged(int value)
    {
        if (value < 0 || value >= MockScenarios.Count) return;
        if (OperatingSystem.IsAndroid()) return;
        SwitchTo(MockScenarios[value].Scenario);
    }

    private void SwitchTo(MockUpdateScenario scenario)
    {
        Unsubscribe(_updateService);
        if (_updateService is MockUpdateService mock) mock.Dispose();

        _updateService = new MockUpdateService(scenario);
        Subscribe(_updateService);

        UpdatePopupState = UpdatePopupState.None;
        IsLatestVersion = false;
        MessageText = string.Empty;
        TargetVersion = null;
        Progress = new DownloadProgress(0, 0, 0, 0, 0);
    }

    #endregion

    #region Update Event Sub/Unsub

    private void Subscribe(IUpdateService service)
    {
        service.ProgressChanged += HandleProgress;
        service.DownloadCompleted += OnDownloadCompleted;
        service.UpdateFailed += OnUpdateFailed;
    }

    private void Unsubscribe(IUpdateService service)
    {
        service.ProgressChanged -= HandleProgress;
        service.DownloadCompleted -= OnDownloadCompleted;
        service.UpdateFailed -= OnUpdateFailed;
    }

    private void HandleProgress(DownloadProgress progress) => Progress = progress;

    private void OnDownloadCompleted() => UpdatePopupState = UpdatePopupState.ReadyToInstall;

    private void OnUpdateFailed(string message)
    {
        IsLatestVersion = false;
        MessageText = message;
        UpdatePopupState = UpdatePopupState.Notice;
    }

    #endregion

    #region Command

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
                UpdatePopupState = UpdatePopupState.UpdateAvailable;
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

    #endregion
}