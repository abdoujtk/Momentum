using CommunityToolkit.Mvvm.Input;
using H.NotifyIcon;
using H.NotifyIcon.Core;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace Momentum.Desktop.Services;

public sealed class TrayService : IDisposable
{
    private TaskbarIcon? _trayIcon;
    private Window? _window;
    private bool _isExitRequested;

   
    private readonly DispatcherQueue _dispatcherQueue;

    public event EventHandler? OpenRequested;
    public event EventHandler? CheckInRequested;
    public event EventHandler? ExitRequested;

    // Inject NotificationService directly to avoid App.GetService threading issues
    public TrayService()
    {
        
        // Capture the UI thread's dispatcher queue
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
    }

    public void Initialize(Window window)
    {
        _window = window;

        _trayIcon = new TaskbarIcon
        {
            ToolTipText = "Momentum — Habit Tracker",
            IconSource = new GeneratedIconSource
            {
                Text = "✓",
                Foreground = new SolidColorBrush(Color.FromArgb(255, 33, 150, 243)),
                FontSize = 36,
                FontWeight = Microsoft.UI.Text.FontWeights.Bold,
                Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255))
            },
            MenuActivation = PopupActivationMode.RightClick,
            // Add this line to fix menu item click events
            ContextMenuMode = ContextMenuMode.SecondWindow,
            LeftClickCommand = new RelayCommand(() => ExecuteOnUiThread(() => OpenRequested?.Invoke(this, EventArgs.Empty))),
            ContextFlyout = BuildContextMenu()
        };

        _trayIcon.ForceCreate();
        window.AppWindow.Closing += OnWindowClosing;
    }

    private MenuFlyout BuildContextMenu()
    {
        var menu = new MenuFlyout();

        var openItem = new MenuFlyoutItem { Text = "فتح Momentum", Icon = new FontIcon { Glyph = "\uE8A7" } };
        openItem.Click += (_, _) => ExecuteOnUiThread(() => OpenRequested?.Invoke(this, EventArgs.Empty));

        var checkInItem = new MenuFlyoutItem { Text = "تسجيل إنجاز اليوم", Icon = new FontIcon { Glyph = "\uE73E" } };
        checkInItem.Click += (_, _) => ExecuteOnUiThread(() => CheckInRequested?.Invoke(this, EventArgs.Empty));

        var testNotifItem = new MenuFlyoutItem { Text = "🔔 اختبار الإشعار", Icon = new FontIcon { Glyph = "\uEA8F" } };
        testNotifItem.Click += (_, _) =>
        {
            System.Diagnostics.Debug.WriteLine("Test notification clicked!");
            // Use the built-in balloon notification from H.NotifyIcon
            _trayIcon?.ShowNotification(
                "اختبار ناجح! ✓",
                "هذا إشعار مدمج من Momentum يعمل بشكل مثالي.",
                NotificationIcon.Info);
        };

        var exitItem = new MenuFlyoutItem { Text = "خروج", Icon = new FontIcon { Glyph = "\uE711" } };
        exitItem.Click += (_, _) =>
        {
            System.Diagnostics.Debug.WriteLine("Exit clicked!");
            _isExitRequested = true;
            ExecuteOnUiThread(() =>
            {
                ExitRequested?.Invoke(this, EventArgs.Empty);
                _window?.Close();
            });
        };

        menu.Items.Add(openItem);
        menu.Items.Add(checkInItem);
        menu.Items.Add(testNotifItem);
        menu.Items.Add(new MenuFlyoutSeparator());
        menu.Items.Add(exitItem);

        return menu;
    }

    private void OnWindowClosing(Microsoft.UI.Windowing.AppWindow sender, Microsoft.UI.Windowing.AppWindowClosingEventArgs args)
    {
        if (_isExitRequested) return;
        args.Cancel = true;
        _window?.AppWindow.Hide();
    }

    public void ShowWindow()
    {
        ExecuteOnUiThread(() =>
        {
            if (_window is null) return;
            _window.AppWindow.Show();
            _window.Activate();

            if (_window.AppWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
                presenter.Restore();
        });
    }

    // Helper to force execution on the UI thread
    private void ExecuteOnUiThread(Action action)
    {
        if (_dispatcherQueue.HasThreadAccess)
            action();
        else
            _dispatcherQueue.TryEnqueue(() => action());
    }

    public void Dispose()
    {
        if (_window is not null)
            _window.AppWindow.Closing -= OnWindowClosing;

        _trayIcon?.Dispose();
        _trayIcon = null;
    }
}