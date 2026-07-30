using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Irihi.Mafia.Demo.ViewModels.ControlDetails;

public partial class PickerDetailViewModel : NavigationViewModelBase
{
    public ObservableCollection<string> Languages { get; } =
    [
        "简体中文",
        "English",
        "日本語",
        "한국어",
        "Français",
        "Deutsch"
    ];

    [ObservableProperty]
    public partial string? SelectedLanguage { get; set; } = "简体中文";

    [ObservableProperty]
    public partial int SelectedIndex { get; set; }

    public ObservableCollection<string> Sizes { get; } =
    [
        "小号",
        "中号",
        "大号",
        "加大号"
    ];
}
