using Momentum.Mobile.Views;

namespace Momentum.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        
        Routing.RegisterRoute(nameof(TodayPage), typeof(TodayPage));
    }
}