using Microsoft.Extensions.DependencyInjection;
using Momentum.Desktop.ViewModels;
using Momentum.Desktop.ViewModels.Abstractions;

namespace Momentum.Mobile.Services;

public class MauiNavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public MauiNavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public bool CanGoBack => Shell.Current.Navigation.NavigationStack.Count > 1;

    public void GoBack() => Shell.Current.GoToAsync("..");

    public void NavigateTo<TViewModel>(object? parameter = null) where TViewModel : class
    {
        // في MAUI، ننتقل عبر المسارات (Routes) المسجلة مسبقاً
        if (typeof(TViewModel) == typeof(TodayViewModel))
        {
            Shell.Current.GoToAsync(nameof(Views.TodayPage));
        }
    }

    public void NavigateTo(Type viewModelType, object? parameter = null)
    {
        if (viewModelType == typeof(TodayViewModel))
        {
            Shell.Current.GoToAsync(nameof(Views.TodayPage));
        }
    }

    public void NavigateToLogin()
    {
        Shell.Current.GoToAsync("//LoginPage");
    }

    public void NavigateToRegister()
    {
        // Will implement later
    }

    public void NavigateToMainApp()
    {
        NavigateTo<TodayViewModel>();
    }

    public void NavigateToLoginShell()
    {
        throw new NotImplementedException();
    }
}