using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Momentum.Desktop.Services;
using Momentum.Desktop.ViewModels;
using Momentum.Desktop.ViewModels.Abstractions;
using Momentum.Desktop.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

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
               

                // ─── ViewModels (Transient) ───
                services.AddTransient<TodayViewModel>();

                // ─── Pages ───
                services.AddTransient<TodayPage>();
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
