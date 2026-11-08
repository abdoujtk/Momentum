using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Momentum.Desktop.ViewModels;

namespace Momentum.Desktop.Views;

public sealed partial class HabitsPage : Page
{
    public HabitsViewModel ViewModel { get; }

    public HabitsPage()
    {
        ViewModel = App.GetService<HabitsViewModel>();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadAsync();
    }
}