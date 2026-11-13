using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Momentum.Contracts.DTOs.Auth;
using Momentum.Desktop.ViewModels.Abstractions;
using System.Threading.Tasks;

namespace Momentum.Desktop.ViewModels;

public sealed partial class RegisterViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly ITokenStore _tokenStore;
    private readonly INavigationService _navigation;

    public RegisterViewModel(IAuthService authService, ITokenStore tokenStore, INavigationService navigation)
    {
        _authService = authService;
        _tokenStore = tokenStore;
        _navigation = navigation;
    }

    [ObservableProperty] private string _displayName = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isBusy;

    // Add this computed property to fix the XAML binding issue
    public bool IsNotBusy => !IsBusy;

    partial void OnIsBusyChanged(bool value) => OnPropertyChanged(nameof(IsNotBusy));

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(DisplayName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "All fields are required.";
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var request = new RegisterRequest(DisplayName, Email, Password);
            var result = await _authService.RegisterAsync(request);

            if (result is null)
            {
                ErrorMessage = "Registration failed. Email might already be in use.";
                return;
            }

            await _tokenStore.SetTokenAsync(result.Token, result.DisplayName);

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
    private void GoToLogin()
    {
        // Use the navigation service!
        _navigation.NavigateToLogin();
    }
}