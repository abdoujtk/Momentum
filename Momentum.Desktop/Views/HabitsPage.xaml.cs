using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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
        // 1. إنشاء النموذج (UserControl)
        var form = new CreateHabitForm(ViewModel.Categories);

        // 2. إنشاء ContentDialog قياسي ووضع النموذج في Content
        var dialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot, // ⚠️ ضروري جداً في WinUI 3
            Title = "إضافة عادة جديدة",
            Content = form,
            PrimaryButtonText = "حفظ",
            CloseButtonText = "إلغاء",
            DefaultButton = ContentDialogButton.Primary
        };

        // 3. التعامل مع زر الحفظ للتحقق من الصحة قبل الإغلاق
        dialog.PrimaryButtonClick += async (s, args) =>
        {
            var (isValid, request, errorMessage) = form.TryBuildRequest();
            if (!isValid)
            {
                // إلغاء الإغلاق وعرض الخطأ
                args.Cancel = true;
                var errorText = new TextBlock
                {
                    Text = errorMessage,
                    Foreground = new SolidColorBrush(Colors.Red),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 8, 0, 0)
                };
                // يمكن إضافة الخطأ إلى النموذج أو عرضه في الحوار
                // (لتبسيط الأمر، سنطبع في الـ Output)
                System.Diagnostics.Debug.WriteLine(errorMessage);
                return;
            }

            // إذا كانت البيانات صحيحة، أرسلها إلى ViewModel
            var success = await ViewModel.CreateHabitAsync(request!);
            if (!success)
            {
                args.Cancel = true; // فشل الحفظ، أبقِ الحوار مفتوحاً
            }
        };

        // 4. عرض الحوار وانتظار النتيجة
        var result = await dialog.ShowAsync();
    }
}