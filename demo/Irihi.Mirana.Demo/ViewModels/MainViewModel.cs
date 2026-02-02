using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Irihi.Mirana.Demo.Models;

namespace Irihi.Mirana.Demo.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] public partial PhoneResolution? SelectedResolution { get; set; }

    public List<PhoneResolution> PhoneResolutions { get; } =
    [
        new("iPhone X", 375, 812),
        new("iPhone 16", 393, 852),
        new("iPhone 16 Pro", 402, 874),
        new("iPhone 16 Pro Max", 440, 956),
        new("Google Pixel 9", 412, 915),
        new("Google Pixel 9 Pro", 412, 915),
        new("Google Pixel 9 Pro XL", 412, 918),
        new("OPPO Find X8", 412, 918),
        new("OPPO Find X8 Pro", 450, 1000),
        new("Samsung Galaxy S24", 412, 915),
        new("Samsung Galaxy S24 Ultra", 450, 1000)
    ];

    public MainViewModel()
    {
        SelectedResolution = PhoneResolutions[0];
    }
}