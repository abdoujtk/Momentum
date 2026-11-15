using Microsoft.Extensions.DependencyInjection;
using Momentum.Client.Http;
using Momentum.Desktop.ViewModels;
using Momentum.Desktop.ViewModels.Abstractions;
using Momentum.Mobile.Service;
using Momentum.Mobile.Services;
using Momentum.Mobile.Views;

namespace Momentum.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services
        builder.Services.AddSingleton<INavigationService, MauiNavigationService>();
        builder.Services.AddSingleton<ITokenStore, MauiTokenStore>();

        // API Clients
        builder.Services.AddTransient<AuthHeaderHandler>();
        builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
        {
            client.BaseAddress = new Uri("http://192.168.100.6:5071/");
        });
        builder.Services.AddHttpClient<IHabitApiClient, HabitApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://192.168.100.6:5071/");
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<TodayViewModel>();

        // Views & Shell
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<TodayPage>();

        return builder.Build();
    }
}