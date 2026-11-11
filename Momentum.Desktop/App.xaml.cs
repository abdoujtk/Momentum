using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Momentum.Desktop.Services;
using Momentum.Desktop.ViewModels;
using Momentum.Desktop.ViewModels.Abstractions;
using Momentum.Desktop.Views;
using System;
using System.Net.Http;

namespace Momentum.Desktop;

public partial class App : Application
{
    public IHost Host { get; }
    private Window? _window;
    private TrayService? _trayService;
    private NotificationService? _notificationService;

    public static T GetService<T>() where T : class
        => ((App)Current).Host.Services.GetRequiredService<T>();

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddHttpClient<IHabitApiClient, Momentum.Client.Http.HabitApiClient>(client =>
                {
                    client.BaseAddress = new Uri("https://localhost:7098/");
                    client.Timeout = TimeSpan.FromSeconds(30);
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new HttpClientHandler();
#if DEBUG
                    handler.ServerCertificateCustomValidationCallback =
                        (msg, _, _, _) => msg.RequestUri?.IsLoopback == true;
#endif
                    return handler;
                });

                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<NotificationService>();
                services.AddSingleton<TrayService>();
              

                services.AddTransient<ShellViewModel>();
                services.AddTransient<TodayViewModel>();
            
                services.AddTransient<HabitsViewModel>();
                services.AddTransient<HabitDetailsViewModel>();

                services.AddTransient<ShellPage>();
                services.AddTransient<TodayPage>();
             
                services.AddTransient<HabitsPage>();
                services.AddTransient<HabitDetailsPage>();
            })
            .Build();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();

        // Initialize services
       
      //  _notificationService = GetService<NotificationService>();
      //  _notificationService.Initialize();

        _trayService = GetService<TrayService>();
        _trayService.Initialize(_window);


        _trayService.OpenRequested += (_, _) => _trayService.ShowWindow();
        _trayService.CheckInRequested += (_, _) =>
        {
            // 1. Bring window to front
            _trayService.ShowWindow();

            // 2. Navigate to Today Page
            var navService = GetService<INavigationService>();
            navService.NavigateTo<TodayViewModel>();
        };
        _trayService.ExitRequested += (_, _) =>
        {
            // TrayService already calls _window.Close() internally.
            // We just need to ensure the application shuts down.
            _trayService?.Dispose();
            Application.Current.Exit(); // هذا هو الطريقة الصحيحة في WinUI 3 لإغلاق التطبيق بالكامل
        };


       

        _window.Activate();
    }
}