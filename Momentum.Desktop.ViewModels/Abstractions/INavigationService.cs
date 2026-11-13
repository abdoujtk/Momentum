namespace Momentum.Desktop.ViewModels.Abstractions;

public interface INavigationService
{
    
    void NavigateTo<TViewModel>(object? parameter = null) where TViewModel : class;

    
    void NavigateTo(Type viewModelType, object? parameter = null);

    
    void GoBack();

    
    bool CanGoBack { get; }

    void NavigateToMainApp();

    void NavigateToLogin();
    void NavigateToRegister();

    void NavigateToLoginShell();
}