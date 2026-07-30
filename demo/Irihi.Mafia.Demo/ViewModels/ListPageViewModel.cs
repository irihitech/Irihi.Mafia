using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Irihi.Mafia.Demo.ViewModels;

public partial class ListPageViewModel : ViewModelBase
{
    public ObservableCollection<string> ThemeOptions { get; } = ["浅色", "深色", "跟随系统"];

    public ObservableCollection<string> LanguageOptions { get; } = ["中文", "English"];

    [ObservableProperty]
    public partial int SelectedThemeIndex { get; set; } = GetInitialThemeIndex();

    [ObservableProperty]
    public partial int SelectedLanguageIndex { get; set; }

    [ObservableProperty] public partial bool IsNotificationEnabled { get; set; } = true;

    partial void OnSelectedThemeIndexChanged(int value)
    {
        var app = Application.Current;
        if (app is null) return;

        app.RequestedThemeVariant = value switch
        {
            0 => ThemeVariant.Light,
            1 => ThemeVariant.Dark,
            _ => ThemeVariant.Default,
        };
    }

    private static int GetInitialThemeIndex()
    {
        var variant = Application.Current?.RequestedThemeVariant;
        if (variant == ThemeVariant.Light) return 0;
        if (variant == ThemeVariant.Dark) return 1;
        return 2; // Default (跟随系统)
    }

    [RelayCommand]
    private void OpenProjectUrl()
    {
        Process.Start(new ProcessStartInfo("https://github.com/irihitech/Irihi.Mafia")
        {
            UseShellExecute = true
        });
    }
}
