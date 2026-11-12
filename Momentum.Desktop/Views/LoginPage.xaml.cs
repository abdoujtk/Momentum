using Microsoft.UI.Xaml.Controls;
using Momentum.Desktop.ViewModels;

namespace Momentum.Desktop.Views;

public sealed partial class LoginPage : Page
{
   
    public LoginViewModel ViewModel { get; }

    public LoginPage()
    {
        ViewModel = App.GetService<LoginViewModel>();
        InitializeComponent();
    }
}