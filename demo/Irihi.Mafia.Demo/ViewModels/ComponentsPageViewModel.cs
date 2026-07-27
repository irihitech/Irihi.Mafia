using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Mafia.Common;
using Irihi.Mafia.Demo.Views.Pages.ControlDetails;

namespace Irihi.Mafia.Demo.ViewModels;

public partial class ComponentsPageViewModel : ViewModelBase
{
    [ObservableProperty] public partial bool IsDetailOpen { get; set; }

    [ObservableProperty] public partial string SelectedControlName { get; set; } = "";

    [ObservableProperty] public partial UserControl? DetailContent { get; set; }

    [ObservableProperty] public partial bool IsPopupOpen { get; set; }

    [ObservableProperty] public partial bool IsModal { get; set; } = true;

    [ObservableProperty] public partial bool IsLightDismiss { get; set; } = true;

    [ObservableProperty] public partial PopupPlacement PopupPlacement { get; set; }

    [ObservableProperty] public partial string PopupTitle { get; set; } = "";

    [ObservableProperty] public partial bool IsInheritedPopupOpen { get; set; }

    [RelayCommand]
    private void ShowControl(string controlName)
    {
        SelectedControlName = controlName;
        DetailContent = controlName switch
        {
            "Popup" => new PopupDetailView { DataContext = this },
            "Button" => new ButtonDetailView(),
            "Switch" => new SwitchDetailView(),
            "Slider" => new SliderDetailView(),
            "Progress" => new ProgressDetailView(),
            _ => new PlaceholderDetailView(),
        };
        IsDetailOpen = true;
    }

    [RelayCommand]
    private void GoBack()
    {
        IsDetailOpen = false;
        DetailContent = null;
    }

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
