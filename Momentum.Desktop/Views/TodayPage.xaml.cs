using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Momentum.Desktop.ViewModels;

namespace Momentum.Desktop.Views;

public sealed partial class TodayPage : Page
{
    public TodayViewModel ViewModel { get; }

    public TodayPage()
    {
        ViewModel = App.GetService<TodayViewModel>();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadAsync();
    }
}