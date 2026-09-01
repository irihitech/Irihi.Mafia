using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Irihi.Mafia.Demo.Models;

namespace Irihi.Mafia.Demo.ViewModels;

public partial class GalleryTabViewModel : NavigationViewModelBase
{
    private static readonly LanguageManager Manager = LanguageManager.Instance;
    private readonly List<ControlItem> _allInteractive;
    private readonly List<ControlItem> _allDisplay;

    public ObservableCollection<ControlItem> InteractiveItems { get; } = [];
    public ObservableCollection<ControlItem> DisplayItems { get; } = [];

    [ObservableProperty] public partial string SearchText { get; set; } = string.Empty;

    public GalleryTabViewModel()
    {
        _allInteractive =
        [
            CreateItem("Popup", nameof(LanguageManager.Keys.Gallery_Popup), nameof(LanguageManager.Keys.Gallery_Popup_Desc)),
            CreateItem("Button", nameof(LanguageManager.Keys.Gallery_Button), nameof(LanguageManager.Keys.Gallery_Button_Desc)),
            CreateItem("Calendar", nameof(LanguageManager.Keys.Gallery_Calendar), nameof(LanguageManager.Keys.Gallery_Calendar_Desc)),
            CreateItem("CalendarDatePicker", nameof(LanguageManager.Keys.Gallery_CalendarDatePicker), nameof(LanguageManager.Keys.Gallery_CalendarDatePicker_Desc)),
            CreateItem("Picker", nameof(LanguageManager.Keys.Gallery_Picker), nameof(LanguageManager.Keys.Gallery_Picker_Desc)),
            CreateItem("Switch", nameof(LanguageManager.Keys.Gallery_Switch), nameof(LanguageManager.Keys.Gallery_Switch_Desc)),
            CreateItem("Slider", nameof(LanguageManager.Keys.Gallery_Slider), nameof(LanguageManager.Keys.Gallery_Slider_Desc)),
            CreateItem("Progress", nameof(LanguageManager.Keys.Gallery_Progress), nameof(LanguageManager.Keys.Gallery_Progress_Desc)),
            CreateItem("CheckBox", nameof(LanguageManager.Keys.Gallery_CheckBox), nameof(LanguageManager.Keys.Gallery_CheckBox_Desc)),
            CreateItem("Input", nameof(LanguageManager.Keys.Gallery_Input), nameof(LanguageManager.Keys.Gallery_Input_Desc)),
            CreateItem("RadioButton", nameof(LanguageManager.Keys.Gallery_RadioButton), nameof(LanguageManager.Keys.Gallery_RadioButton_Desc)),
        ];
        _allDisplay =
        [
            CreateItem("Cell", nameof(LanguageManager.Keys.Gallery_Cell), nameof(LanguageManager.Keys.Gallery_Cell_Desc)),
            CreateItem("Avatar", nameof(LanguageManager.Keys.Gallery_Avatar), nameof(LanguageManager.Keys.Gallery_Avatar_Desc)),
            CreateItem("IconButton", nameof(LanguageManager.Keys.Gallery_IconButton), nameof(LanguageManager.Keys.Gallery_IconButton_Desc)),
            CreateItem("HyperlinkButton", nameof(LanguageManager.Keys.Gallery_HyperlinkButton), nameof(LanguageManager.Keys.Gallery_HyperlinkButton_Desc)),
            CreateItem("Divider", nameof(LanguageManager.Keys.Gallery_Divider), nameof(LanguageManager.Keys.Gallery_Divider_Desc)),
            CreateItem("TabControl", nameof(LanguageManager.Keys.Gallery_TabControl), nameof(LanguageManager.Keys.Gallery_TabControl_Desc)),
            CreateItem("TabbedPage", nameof(LanguageManager.Keys.Gallery_TabbedPage), nameof(LanguageManager.Keys.Gallery_TabbedPage_Desc)),
            CreateItem("Icon", nameof(LanguageManager.Keys.Gallery_Icon), nameof(LanguageManager.Keys.Gallery_Icon_Desc)),
            CreateItem("StickyPanel", nameof(LanguageManager.Keys.Gallery_StickyPanel), nameof(LanguageManager.Keys.Gallery_StickyPanel_Desc)),
        ];

        ApplyFilter();
        return;

        ControlItem CreateItem(string name, string titleKey, string descriptionKey) => new(Manager, name, titleKey, descriptionKey, ShowControlCommand);
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter(value);

    private void ApplyFilter(string value = "")
    {
        InteractiveItems.Clear();
        foreach (var item in _allInteractive.Where(i => i.Matches(value)))
            InteractiveItems.Add(item);
        DisplayItems.Clear();
        foreach (var item in _allDisplay.Where(i => i.Matches(value)))
            DisplayItems.Add(item);
    }

    [RelayCommand]
    private async Task ShowControlAsync(string controlName)
    {
        if (Navigator is null) return;
        await Navigator.NavigateToAsync(controlName);
    }
}