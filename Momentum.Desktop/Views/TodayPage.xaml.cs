using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Momentum.Contracts.DTOs;
using Momentum.Desktop.Dialogs;
using Momentum.Desktop.ViewModels;
using System;

namespace Momentum.Desktop.Views;

public sealed partial class TodayPage : Page
{
    public TodayViewModel ViewModel { get; }

    public TodayPage()
    {
        ViewModel = App.GetService<TodayViewModel>();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadAsync();
    }

    private async void CheckInButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.DataContext is not HabitDto habit)
            return;

        // Build the form and dialog
        var form = new CheckInForm(habit, ViewModel.Moods);
        var dialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = $"تسجيل إنجاز: {habit.Title}",
            Content = form,
            PrimaryButtonText = "حفظ",
            CloseButtonText = "إلغاء",
            DefaultButton = ContentDialogButton.Primary
        };

        dialog.PrimaryButtonClick += async (s, args) =>
        {
            var (isValid, request, errorMessage) = form.TryBuildRequest();
            if (!isValid)
            {
                args.Cancel = true;
                System.Diagnostics.Debug.WriteLine(errorMessage);
                return;
            }

            var success = await ViewModel.CheckInAsync(habit, request!);
            if (!success)
            {
                args.Cancel = true;  // Keep dialog open on failure
            }
        };

        await dialog.ShowAsync();
    }
}