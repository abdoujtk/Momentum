using Momentum.Desktop.ViewModels;

namespace Momentum.Mobile.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // Inject the shared ViewModel
    }
}