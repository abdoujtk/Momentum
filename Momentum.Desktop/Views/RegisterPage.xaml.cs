using Microsoft.UI.Xaml.Controls;
using Momentum.Desktop.ViewModels;

namespace Momentum.Desktop.Views;

public sealed partial class RegisterPage : Page
{
    public RegisterViewModel ViewModel { get; }

    public RegisterPage()
    {
        ViewModel = App.GetService<RegisterViewModel>();
        InitializeComponent();
    }
}