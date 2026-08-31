using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Mafia.Common;

namespace Irihi.Mafia.Demo.ViewModels;

public partial class PopupPageViewModel: NavigationViewModelBase
{
    [ObservableProperty] public partial PopupPlacement PopupPlacement { get; set; }
    
    [ObservableProperty] public partial string PopupTitle { get; set; } = string.Empty;
    
    [ObservableProperty] public partial bool IsPopupOpen { get; set; }
    
    [ObservableProperty] public partial bool IsInheritedPopupOpen { get; set; }
    
    [ObservableProperty] public partial bool IsModal { get; set; } = true;

    [ObservableProperty] public partial bool IsLightDismiss { get; set; } = true;
    
    // ===== Popup 演示命令 =====
    [RelayCommand]
    private void OpenPopup(string placement)
    {
        PopupPlacement = placement switch
        {
            "Bottom" => PopupPlacement.Bottom,
            "Center" => PopupPlacement.Center,
            "Top" => PopupPlacement.Top,
            "Left" => PopupPlacement.Left,
            "Right" => PopupPlacement.Right,
            "FullScreen" => PopupPlacement.FullScreen,
            _ => PopupPlacement.Bottom
        };
        PopupTitle = $"{placement} Placement";
        IsPopupOpen = true;
    }

    [RelayCommand]
    private void ClosePopup()
    {
        IsPopupOpen = false;
    }

    [RelayCommand]
    private void OpenInheritedPopup()
    {
        IsInheritedPopupOpen = true;
    }

    [RelayCommand]
    private void CloseInheritedPopup()
    {
        IsInheritedPopupOpen = false;
    }
}
