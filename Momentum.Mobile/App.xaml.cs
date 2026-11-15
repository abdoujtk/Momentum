using Microsoft.Extensions.DependencyInjection;
using Momentum.Mobile.Views;

namespace Momentum.Mobile;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        // Set the initial page
        MainPage = serviceProvider.GetRequiredService<AppShell>();
    }
}