using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Momentum.Desktop.ViewModels;

namespace Momentum.Desktop.Views;

public sealed partial class HabitDetailsPage : Page
{
    public HabitDetailsViewModel ViewModel { get; }

    public HabitDetailsPage()
    {
        ViewModel = App.GetService<HabitDetailsViewModel>();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Read navigation parameter (HabitId)
        if (e.Parameter is int habitId)
        {
            await ViewModel.LoadAsync(habitId);
        }
    }
}