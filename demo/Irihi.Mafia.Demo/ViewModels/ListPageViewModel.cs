using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Irihi.Mafia.Demo.ViewModels;

public partial class ListPageViewModel : ViewModelBase
{
    [ObservableProperty] public partial bool IsDarkMode { get; set; }

    [ObservableProperty] public partial bool IsNotificationEnabled { get; set; } = true;
}
