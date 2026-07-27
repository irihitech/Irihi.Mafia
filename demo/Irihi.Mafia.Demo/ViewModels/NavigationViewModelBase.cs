using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Irihi.Mafia.Demo.ViewModels;

public class NavigationViewModelBase: ObservableObject
{
    public NavigationPage? NavigationRoot { get; set; }
}
