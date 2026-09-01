using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Irihi.Mafia.Demo.ViewModels;
using Irihi.Mafia.Demo.Views;

namespace Irihi.Mafia.Demo.Services;

public sealed class NavigationService(INavigation navigation) : INavigationService
{
    private readonly IReadOnlyDictionary<string, Func<Page>> _routes = new Dictionary<string, Func<Page>>
    {
        ["Popup"] = () => new PopupPage { DataContext = new PopupPageViewModel() },
        ["Button"] = () => new ButtonPage { DataContext = new ButtonPageViewModel() },
        ["Calendar"] = () => new CalendarPage { DataContext = new CalendarPageViewModel() },
        ["CalendarDatePicker"] = () => new CalendarDatePickerPage { DataContext = new CalendarDatePickerPageViewModel() },
        ["Picker"] = () => new PickerPage { DataContext = new PickerPageViewModel() },
        ["Switch"] = () => new SwitchPage(),
        ["Input"] = () => new InputPage(),
        ["Slider"] = () => new SliderPage(),
        ["Progress"] = () => new ProgressPage(),
        ["CheckBox"] = () => new CheckBoxPage(),
        ["RadioButton"] = () => new RadioButtonPage(),
        ["Cell"] = () => new CellPage(),
        ["Avatar"] = () => new AvatarPage(),
        ["IconButton"] = () => new IconButtonPage(),
        ["HyperlinkButton"] = () => new HyperlinkButtonPage(),
        ["Divider"] = () => new DividerPage(),
        ["TabControl"] = () => new TabControlPage(),
        ["TabbedPage"] = () => new TabbedPagePage(),
        ["Icon"] = () => new IconGalleryPage(),
        ["StickyPanel"] = () => new StickyPanelPage(),
    };

    public bool CanGoBack => navigation.CanGoBack;

    public async Task NavigateToAsync(string route)
    {
        var page = _routes.TryGetValue(route, out var factory)
            ? factory()
            : new PlaceholderPage();

        page.Header = route;
        await navigation.PushAsync(page);
    }

    public Task GoBackAsync() => navigation.PopAsync();
}