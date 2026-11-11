using CommunityToolkit.WinUI.Notifications;
using System;

namespace Momentum.Desktop.Services;

public sealed class NotificationService
{
    private bool _initialized;
    private const string AppUserModelId = "Momentum.HabitTracker.App";

    public void Initialize()
    {
        if (_initialized) return;

        // 1. Register AUMID and COM Server for unpackaged apps
        // Note: We use DesktopNotificationManagerCompat, not ToastNotificationManagerCompat
        DesktopNotificationManagerCompat.RegisterAumidAndComServer<NotificationActivatorPlaceholder>(AppUserModelId);

        // 2. Register the activator type
        DesktopNotificationManagerCompat.RegisterActivator<NotificationActivatorPlaceholder>();

        // 3. Subscribe to the activation event
        ToastNotificationManagerCompat.OnActivated += OnNotificationActivated;

        _initialized = true;
    }

    public void ShowNotification(string title, string body)
    {
        if (!_initialized) return;

        // Use ToastContentBuilder to construct the notification
        new ToastContentBuilder()
            .AddText(title)
            .AddText(body)
            .Show();
    }

    public void ShowDailyReminder()
    {
        if (!_initialized) return;

        new ToastContentBuilder()
            .AddText("وقت العادات! 🌱")
            .AddText("لا تنسَ تسجيل إنجازاتك اليومية. كل خطوة صغيرة تُحدث فرقًا.")
            .AddButton(new ToastButton()
                .SetContent("فتح Momentum")
                .AddArgument("action", "open"))
            .Show();
    }

    private void OnNotificationActivated(ToastNotificationActivatedEventArgsCompat e)
    {
        NotificationActivated?.Invoke(this, e);
    }

    public event EventHandler<ToastNotificationActivatedEventArgsCompat>? NotificationActivated;
}

// A minimal class to satisfy the generic constraint for RegisterAumidAndComServer.
// The actual activation is handled by the OnActivated event.
public class NotificationActivatorPlaceholder : NotificationActivator
{
    public override void OnActivated(string invokedArgs, NotificationUserInput userInput, string appUserModelId)
    {
        // Handled by ToastNotificationManagerCompat.OnActivated
    }
}