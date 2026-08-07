using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Mafia.Demo.ViewModels.ControlDetails;
using Irihi.Mafia.Demo.Views.Pages.ControlDetails;

namespace Irihi.Mafia.Demo.ViewModels;

public partial class ControlGalleryViewModel : NavigationViewModelBase
{
    private static readonly LanguageManager Manager = LanguageManager.Instance;
    private readonly List<ControlItem> _allInteractive = [];
    private readonly List<ControlItem> _allDisplay = [];

    public ObservableCollection<ControlItem> InteractiveItems { get; } = [];
    public ObservableCollection<ControlItem> DisplayItems { get; } = [];

    [ObservableProperty] public partial string SearchText { get; set; } = "";

    public ControlGalleryViewModel()
    {
        _allInteractive.AddRange(
        [
            new(Manager, "Popup", nameof(LanguageManager.Keys.Gallery_Popup), nameof(LanguageManager.Keys.Gallery_Popup_Desc), ShowControlCommand),
            new(Manager, "Button", nameof(LanguageManager.Keys.Gallery_Button), nameof(LanguageManager.Keys.Gallery_Button_Desc), ShowControlCommand),
            new(Manager, "Picker", nameof(LanguageManager.Keys.Gallery_Picker), nameof(LanguageManager.Keys.Gallery_Picker_Desc), ShowControlCommand),
            new(Manager, "Switch", nameof(LanguageManager.Keys.Gallery_Switch), nameof(LanguageManager.Keys.Gallery_Switch_Desc), ShowControlCommand),
            new(Manager, "Slider", nameof(LanguageManager.Keys.Gallery_Slider), nameof(LanguageManager.Keys.Gallery_Slider_Desc), ShowControlCommand),
            new(Manager, "Progress", nameof(LanguageManager.Keys.Gallery_Progress), nameof(LanguageManager.Keys.Gallery_Progress_Desc), ShowControlCommand),
            new(Manager, "CheckBox", nameof(LanguageManager.Keys.Gallery_CheckBox), nameof(LanguageManager.Keys.Gallery_CheckBox_Desc), ShowControlCommand),
            new(Manager, "Input", nameof(LanguageManager.Keys.Gallery_Input), nameof(LanguageManager.Keys.Gallery_Input_Desc), ShowControlCommand),
            new(Manager, "RadioButton", nameof(LanguageManager.Keys.Gallery_RadioButton), nameof(LanguageManager.Keys.Gallery_RadioButton_Desc), ShowControlCommand),
        ]);
        _allDisplay.AddRange(
        [
            new(Manager, "Cell", nameof(LanguageManager.Keys.Gallery_Cell), nameof(LanguageManager.Keys.Gallery_Cell_Desc), ShowControlCommand),
            new(Manager, "Avatar", nameof(LanguageManager.Keys.Gallery_Avatar), nameof(LanguageManager.Keys.Gallery_Avatar_Desc), ShowControlCommand),
            new(Manager, "IconButton", nameof(LanguageManager.Keys.Gallery_IconButton), nameof(LanguageManager.Keys.Gallery_IconButton_Desc), ShowControlCommand),
            new(Manager, "Divider", nameof(LanguageManager.Keys.Gallery_Divider), nameof(LanguageManager.Keys.Gallery_Divider_Desc), ShowControlCommand),
            new(Manager, "TabControl", nameof(LanguageManager.Keys.Gallery_TabControl), nameof(LanguageManager.Keys.Gallery_TabControl_Desc), ShowControlCommand),
            new(Manager, "TabbedPage", nameof(LanguageManager.Keys.Gallery_TabbedPage), nameof(LanguageManager.Keys.Gallery_TabbedPage_Desc), ShowControlCommand),
        ]);
        ApplyFilter();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        InteractiveItems.Clear();
        foreach (var item in _allInteractive.Where(i => i.Matches(SearchText)))
            InteractiveItems.Add(item);
        DisplayItems.Clear();
        foreach (var item in _allDisplay.Where(i => i.Matches(SearchText)))
            DisplayItems.Add(item);
    }

    [RelayCommand]
    private void ShowControl(string controlName)
    {
        UserControl content = controlName switch
        {
            "Popup" => new PopupDetailView(),
            "Button" => new ButtonDetailView(),
            "Picker" => new PickerDetailView(),
            "Switch" => new SwitchDetailView(),
            "Input" => new InputDetailView(),
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
        NavigationRoot?.PushAsync(new ContentPage() { Header = controlName, Content = content, DataContext = datacontext, });
    }
}
