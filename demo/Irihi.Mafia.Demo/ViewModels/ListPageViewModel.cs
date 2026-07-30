using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Irihi.Mafia.Demo.ViewModels;

public partial class ListPageViewModel : ViewModelBase
{
    public ObservableCollection<IObservable<string?>> ThemeOptions { get; } =
    [
        LanguageManager.Instance.Theme_Light, LanguageManager.Instance.Theme_Dark, LanguageManager.Instance.Theme_System
    ];

    public ObservableCollection<string> LanguageOptions { get; } = ["中文", "English"];

    [ObservableProperty]
    public partial int SelectedThemeIndex { get; set; } = GetInitialThemeIndex();

    [ObservableProperty]
    public partial int SelectedLanguageIndex { get; set; } = GetInitialLanguageIndex();

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

    partial void OnSelectedLanguageIndexChanged(int value)
    {
        var culture = value switch
        {
            0 => new CultureInfo("zh-Hans"),
            _ => CultureInfo.InvariantCulture,
        };
        LanguageManager.Instance.UpdateCulture(culture);
    }

    private static int GetInitialThemeIndex()
    {
        var variant = Application.Current?.RequestedThemeVariant;
        if (variant == ThemeVariant.Light) return 0;
        if (variant == ThemeVariant.Dark) return 1;
        return 2;
    }

    private static int GetInitialLanguageIndex()
    {
        var culture = LanguageManager.Instance.CurrentCulture;
        if (culture.Name.StartsWith("zh")) return 0;
        return 1;
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
