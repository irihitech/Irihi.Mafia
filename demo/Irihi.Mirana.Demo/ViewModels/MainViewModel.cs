using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Irihi.Mirana.Demo.Models;

namespace Irihi.Mirana.Demo.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] public partial string? Greeting { get; set; } = "Welcome to Avalonia!";
    
    [ObservableProperty] 
    private PhoneResolution? _selectedResolution;

    public List<PhoneResolution> PhoneResolutions { get; } = new()
    {
        new PhoneResolution("iPhone X", 375, 812),
        new PhoneResolution("iPhone 16", 393, 852),
        new PhoneResolution("iPhone 16 Pro", 402, 874),
        new PhoneResolution("iPhone 16 Pro Max", 440, 956),
        new PhoneResolution("Google Pixel 9", 412, 915),
        new PhoneResolution("Google Pixel 9 Pro", 412, 915),
        new PhoneResolution("Google Pixel 9 Pro XL", 412, 918),
        new PhoneResolution("OPPO Find X8", 412, 918),
        new PhoneResolution("OPPO Find X8 Pro", 450, 1000),
        new PhoneResolution("Samsung Galaxy S24", 412, 915),
        new PhoneResolution("Samsung Galaxy S24 Ultra", 450, 1000)
    };

    public MainViewModel()
    {
        // Set default resolution to iPhone X
        SelectedResolution = PhoneResolutions[0];
    }
}