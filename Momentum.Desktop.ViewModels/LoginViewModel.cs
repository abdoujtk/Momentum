using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Momentum.Desktop.ViewModels.Abstractions;

namespace Momentum.Desktop.ViewModels;

public sealed partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly ITokenStore _tokenStore;
    private readonly INavigationService _navigation;

    public LoginViewModel(IAuthService authService, ITokenStore tokenStore, INavigationService navigation)
    {
        _authService = authService;
        _tokenStore = tokenStore;
        _navigation = navigation;
    }

    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isBusy;

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter email and password.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var result = await _authService.LoginAsync(Email, Password);
            if (result is null)
            {
                ErrorMessage = "Invalid email or password.";
                return;
            }

            // Save token
            await _tokenStore.SetTokenAsync(result.Token, result.DisplayName);

            // Navigate to main app (ShellPage)
            // We need a way to tell App.xaml.cs to switch the main window content
            _navigation.NavigateToMainApp();
        }
        catch
        {
            ErrorMessage = "Connection failed. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void GoToRegister()
    {
        _navigation.NavigateToRegister();
    }
}