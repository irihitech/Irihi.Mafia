using CommunityToolkit.Mvvm.ComponentModel;

namespace Irihi.Mirana.Demo.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}
