using CommunityToolkit.Mvvm.ComponentModel;
using Irihi.Mafia.Demo.Services;

namespace Irihi.Mafia.Demo.ViewModels;

public class NavigationViewModelBase : ObservableObject
{
    public static INavigationService? Navigator => AppServices.Navigator;
}