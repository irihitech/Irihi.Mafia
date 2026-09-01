using System.Threading.Tasks;

namespace Irihi.Mafia.Demo.Services;

public interface INavigationService
{
    bool CanGoBack { get; }

    Task NavigateToAsync(string route);

    Task GoBackAsync();
}