using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Momentum.Desktop.Views;

namespace Momentum.Desktop;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
      
        RootFrame.Navigate(typeof(ShellPage));
    }
}