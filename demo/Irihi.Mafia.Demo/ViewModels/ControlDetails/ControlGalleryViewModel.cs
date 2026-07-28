using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Irihi.Mafia.Demo.Views.Pages.ControlDetails;

namespace Irihi.Mafia.Demo.ViewModels.ControlDetails;

public partial class ControlGalleryViewModel: NavigationViewModelBase
{
    [RelayCommand]
    private void ShowControl(string controlName)
    {
        var name = controlName;
        UserControl content = controlName switch
        {
            "Popup" => new PopupDetailView(),
            "Button" => new ButtonDetailView(),
            "Picker" => new PickerDetailView(),
            "Switch" => new SwitchDetailView(),
            "Slider" => new SliderDetailView(),
            "Progress" => new ProgressDetailView(),
            _ => new PlaceholderDetailView(),
        };
        NavigationViewModelBase? datacontext = controlName switch
        {
            "Popup" => new PopupDetailViewModel() { NavigationRoot = NavigationRoot },
            "Button" => new ButtonDetailViewModel() { NavigationRoot = NavigationRoot },
            "Picker" => new PickerDetailViewModel() { NavigationRoot = NavigationRoot },
            _ => null,
        };
        NavigationRoot?.PushAsync(new ContentPage()
        {
            Header = name, 
            Content = content,
            DataContext = datacontext,
        });
    }
}
