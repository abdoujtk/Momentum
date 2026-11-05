using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Momentum.Desktop.Views;

namespace Momentum.Desktop;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ContentFrame.Navigate(typeof(TodayPage));
    }

    private void NavView_SelectionChanged(
        NavigationView sender,
        NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem item && item.Tag is string tag)
        {
            switch (tag)
            {
                case "today":
                    ContentFrame.Navigate(typeof(TodayPage));
                    break;
            }
        }
    }
}