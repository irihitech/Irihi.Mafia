using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Mafia.Common;

namespace Irihi.Mafia.Demo.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isPopupOpen;

    [ObservableProperty]
    private bool _isModal = true;

    [ObservableProperty]
    private bool _isLightDismiss = true;

    [ObservableProperty]
    private PopupPlacement _popupPlacement;

    [ObservableProperty]
    private string _popupTitle = "";

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
}
