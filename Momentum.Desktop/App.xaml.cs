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

    // Main window (after login)
    private Window? _window;

    // Login window (shown first)
    private Window? _loginWindow;

    private TrayService? _trayService;

    // Helper to access App from ViewModels
    public static App CurrentApp => (App)Current;

    public static T GetService<T>() where T : class
        => ((App)Current).Host.Services.GetRequiredService<T>();

    
    public LoginWindow? LoginWindowInstance => _loginWindow as LoginWindow;

   
    public App()
    {
        InitializeComponent();
        
        Host = Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                // ═══════════════════════════════════════════════════════════
                // 1. Register TokenStore & AuthHeaderHandler
                // ═══════════════════════════════════════════════════════════
                services.AddSingleton<ISecureTokenStorage, SecureTokenStorage>();
                services.AddSingleton<ITokenStore, TokenStore>();
                services.AddTransient<AuthHeaderHandler>();
              

                // ═══════════════════════════════════════════════════════════
                // 2. Register API Clients
                // ═══════════════════════════════════════════════════════════
                // Auth Service (No handler needed, it's for logging in)
                services.AddHttpClient<IAuthService, Momentum.Client.Http.AuthService>(client =>
                {
                    client.BaseAddress = new Uri("https://localhost:7098/");
                })
                .ConfigurePrimaryHttpMessageHandler(() => GetDebugHttpClientHandler());

                // Habit API Client (With AuthHeaderHandler to attach JWT)
                services.AddHttpClient<IHabitApiClient, Momentum.Client.Http.HabitApiClient>(client =>
                {
                    client.BaseAddress = new Uri("https://localhost:7098/");
                    client.Timeout = TimeSpan.FromSeconds(30);
                })
                .AddHttpMessageHandler<AuthHeaderHandler>() // <-- Important
                .ConfigurePrimaryHttpMessageHandler(() => GetDebugHttpClientHandler());

                // ═══════════════════════════════════════════════════════════
                // 3. Services
                // ═══════════════════════════════════════════════════════════
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<TrayService>();
                
            

                // ═══════════════════════════════════════════════════════════
                // 4. ViewModels
                // ═══════════════════════════════════════════════════════════
                services.AddTransient<LoginViewModel>();
                services.AddTransient<RegisterViewModel>();

                services.AddTransient<ShellViewModel>();
                services.AddTransient<TodayViewModel>();
                services.AddTransient<HabitsViewModel>();
                services.AddTransient<HabitDetailsViewModel>();

                // ═══════════════════════════════════════════════════════════
                // 5. Pages & Windows
                // ═══════════════════════════════════════════════════════════
                services.AddTransient<LoginPage>();
                services.AddTransient<RegisterPage>();

                services.AddTransient<ShellPage>();
                services.AddTransient<TodayPage>();
                services.AddTransient<HabitsPage>();
                services.AddTransient<HabitDetailsPage>();
            })
            .Build();
    }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        var tokenStore = GetService<ITokenStore>();

        // Load the saved token
        await tokenStore.LoadTokenAsync();

        if (!string.IsNullOrEmpty(tokenStore.Token))
        {
            ShowMainShell();
        }
        else
        {
            _loginWindow = new LoginWindow();
            _loginWindow.Activate();
        }
    }

    /// <summary>
    /// This method is called by LoginViewModel after a successful login.
    /// It closes the login window, opens the main window, and initializes the tray.
    /// </summary>
    public void ShowMainShell()
    {
        // 1. Create Main Window
        _window = new MainWindow();

        // 2. Initialize TrayService (Now that we are authenticated)
        _trayService = GetService<TrayService>();
        _trayService.Initialize(_window);

        _trayService.OpenRequested += (_, _) => _trayService.ShowWindow();
        _trayService.CheckInRequested += (_, _) =>
        {
            _trayService.ShowWindow();
            var navService = GetService<INavigationService>();
            navService.NavigateTo<TodayViewModel>();
        };
        _trayService.ExitRequested += (_, _) =>
        {
            _trayService?.Dispose();
            Application.Current.Exit();
        };

        // 3. Show Main Window
        _window.Activate();

        // 4. Close Login Window
        _loginWindow?.Close();
        _loginWindow = null;
    }

    // Helper to bypass SSL for localhost in debug mode
    private static HttpClientHandler GetDebugHttpClientHandler()
    {
        var handler = new HttpClientHandler();
#if DEBUG
        handler.ServerCertificateCustomValidationCallback =
            (msg, _, _, _) => msg.RequestUri?.IsLoopback == true;
#endif
        return handler;
    }

    public void ShowLoginShell()
    {
        try
        {
        
            var tokenStore = GetService<ITokenStore>();
            tokenStore.Clear();

            _trayService?.Dispose();
            _trayService = null;

         
            var dispatcher = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
            dispatcher.TryEnqueue(() =>
            {
                try
                {
                   
                    _loginWindow = new LoginWindow();
                    _loginWindow.Activate();

                    
                    if (_window != null)
                    {
                        _window.Close();
                        _window = null;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error during window switch: {ex.Message}");
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in ShowLoginShell: {ex.Message}");
        }
    }
}