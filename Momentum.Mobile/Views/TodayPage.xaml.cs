using Momentum.Contracts.DTOs;
using Momentum.Desktop.ViewModels;

namespace Momentum.Mobile.Views;

public partial class TodayPage : ContentPage
{
    public TodayViewModel ViewModel { get; }

    public TodayPage(TodayViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.LoadAsync();
    }

    private async void OnCheckInClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.BindingContext is not HabitDto habit)
            return;

        if (habit.IsCompletedToday) return;

        // 1. اختيار المزاج (DisplayActionSheet)
        string action = await DisplayActionSheet("كيف تشعر بعد إنجاز هذه العادة؟", "إلغاء", null,
            "ممتاز 😄", "جيد 🙂", "عادي 😐", "متعب 😴", "مرهق 😫");

        if (action == "إلغاء") return;

        int? moodId = action switch
        {
            "ممتاز 😄" => 1,
            "جيد 🙂" => 2,
            "عادي 😐" => 3,
            "متعب 😴" => 4,
            "مرهق 😫" => 5,
            _ => null
        };

        // 2. إدخال ملاحظة (DisplayPromptAsync)
        string note = await DisplayPromptAsync("ملاحظة", "هل هناك شيء تود تدوينه؟ (اختياري)", "حفظ", "تخطي");

        // 3. تنفيذ الإنجاز
        var request = new CheckInRequest(habit.TargetValue, true, moodId, string.IsNullOrWhiteSpace(note) ? null : note);

        var success = await ViewModel.CheckInAsync(habit, request);

        if (!success)
        {
            await DisplayAlert("خطأ", "تعذر تسجيل الإنجاز. يرجى المحاولة مرة أخرى.", "حسناً");
        }
    }
}