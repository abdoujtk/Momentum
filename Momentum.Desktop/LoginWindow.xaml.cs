using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Momentum.Desktop.Views;

namespace Momentum.Desktop;

public sealed partial class LoginWindow : Window
{
    public Frame RootFrameControl => RootFrame;
    public LoginWindow()
    {
        InitializeComponent();
        // Set a reasonable size for the login window
        AppWindow.Resize(new Windows.Graphics.SizeInt32(600, 700));
        RootFrame.Navigate(typeof(LoginPage));
    }
}