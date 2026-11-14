using Microsoft.Extensions.DependencyInjection; // <-- هذا هو السطر السحري
using Microsoft.Extensions.Logging;
using Momentum.Client.Http;
using Momentum.Desktop.ViewModels;
using Momentum.Desktop.ViewModels.Abstractions;

namespace Momentum.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // 1. Register the MAUI-specific TokenStore
        builder.Services.AddSingleton<ITokenStore, MauiTokenStore>();

        // 2. Register the shared AuthHeaderHandler
        builder.Services.AddTransient<AuthHeaderHandler>();

        // 3. Register API Clients
        builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
        {
            client.BaseAddress = new Uri("https://10.0.2.2:7098/"); // Localhost for Android Emulator
        });

        builder.Services.AddHttpClient<IHabitApiClient, HabitApiClient>(client =>
        {
            client.BaseAddress = new Uri("https://10.0.2.2:7098/");
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        // 4. Register ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<TodayViewModel>();    

        // 5. Register Pages
        builder.Services.AddTransient<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}