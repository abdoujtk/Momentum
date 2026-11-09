using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Momentum.Contracts.DTOs;
using Momentum.Desktop.Dialogs;
using Momentum.Desktop.ViewModels;
using System;

namespace Momentum.Desktop.Views;

public sealed partial class HabitsPage : Page
{
    public HabitsViewModel ViewModel { get; }

    public HabitsPage()
    {
        ViewModel = App.GetService<HabitsViewModel>();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadAsync();
    }

    private async void AddHabitButton_Click(object sender, RoutedEventArgs e)
    {
        var form = new CreateEditHabitForm(ViewModel.Categories);  // ← إنشاء
        var dialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = "إضافة عادة جديدة",
            Content = form,
            PrimaryButtonText = "حفظ",
            CloseButtonText = "إلغاء",
            DefaultButton = ContentDialogButton.Primary
        };

        dialog.PrimaryButtonClick += async (s, args) =>
        {
            var (isValid, data, error) = form.TryBuildRequest();
            if (!isValid) { args.Cancel = true; return; }

            var request = CreateEditHabitForm.ToCreateRequest(data!);
            var success = await ViewModel.CreateHabitAsync(request);
            if (!success) args.Cancel = true;
        };

        await dialog.ShowAsync();
    }
    private async void EditHabitButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.DataContext is not HabitDto habit)
            return;

        var form = new CreateEditHabitForm(ViewModel.Categories, habit);  // ← تعديل
        var dialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = $"تعديل: {habit.Title}",
            Content = form,
            PrimaryButtonText = "حفظ التعديلات",
            CloseButtonText = "إلغاء",
            DefaultButton = ContentDialogButton.Primary
        };

        dialog.PrimaryButtonClick += async (s, args) =>
        {
            var (isValid, data, error) = form.TryBuildRequest();
            if (!isValid) { args.Cancel = true; return; }

            var request = CreateEditHabitForm.ToUpdateRequest(data!, habit);
            var success = await ViewModel.UpdateHabitAsync(habit.HabitId, request);
            if (!success) args.Cancel = true;
        };

        await dialog.ShowAsync();
    }
    private async void DeleteHabitButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || button.DataContext is not HabitDto habit)
            return;

       
        var confirmDialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Title = "أرشفة العادة",
            Content = $"هل أنت متأكد من أرشفة \"{habit.Title}\"؟\n\nلن تفقد السجلات، يمكنك استرجاعها لاحقًا.",
            PrimaryButtonText = "أرشفة",
            CloseButtonText = "إلغاء",
            DefaultButton = ContentDialogButton.Close  
        };

        var result = await confirmDialog.ShowAsync();
        if (result != ContentDialogResult.Primary) return;

       
        var success = await ViewModel.DeleteHabitAsync(habit);
        if (!success)
        {
           
            var errorDialog = new ContentDialog
            {
                XamlRoot = this.XamlRoot,
                Title = "فشل الحذف",
                Content = "تعذر الاتصال بالخادم. يرجى المحاولة مرة أخرى.",
                CloseButtonText = "حسنًا"
            };
            await errorDialog.ShowAsync();
        }
    }

}