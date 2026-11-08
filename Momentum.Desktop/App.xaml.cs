using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Momentum.Client.Http;
using Momentum.Desktop.Services;
using Momentum.Desktop.ViewModels;
using Momentum.Desktop.ViewModels.Abstractions;
using Momentum.Desktop.Views;
using System;
using System.Net.Http;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Momentum.Desktop
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public IHost Host { get; }
        private Window? _window;

        public static T GetService<T>() where T : class
        => ((App)Current).Host.Services.GetRequiredService<T>();


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            Host = Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                // ─── HTTP + API Client ───
                services.AddHttpClient<IHabitApiClient, HabitApiClient>(client =>
                {
                    client.BaseAddress = new Uri("https://localhost:7098/");
                    client.Timeout = TimeSpan.FromSeconds(30);
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new HttpClientHandler();
#if DEBUG
                    // ⚠️ DEV ONLY: bypass self-signed cert for localhost
                    handler.ServerCertificateCustomValidationCallback =
                        (msg, _, _, _) => msg.RequestUri?.IsLoopback == true;
#endif
                    return handler;
                });

                // ─── Services ───
                services.AddSingleton<INavigationService, NavigationService>();

                // ─── ViewModels (Transient) ───
                services.AddTransient<TodayViewModel>();
                services.AddTransient<ShellViewModel>();
                services.AddTransient<HabitsViewModel>();
                // ─── Pages ───
                services.AddTransient<TodayPage>();
                services.AddTransient<ShellPage>();
                services.AddTransient<HabitsPage>();
            })
            .Build();

        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }
    }
}
