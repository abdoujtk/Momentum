using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Momentum.Contracts.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Momentum.Desktop.Dialogs;

public sealed partial class CreateEditHabitForm : UserControl
{
    private readonly HabitDto? _existing;
    public IReadOnlyList<CategoryDto> Categories { get; }
    public bool IsEditMode => _existing is not null;

    public CreateEditHabitForm(IReadOnlyList<CategoryDto> categories, HabitDto? existing = null)
    {
        InitializeComponent();
        _existing = existing;
        Categories = categories;

     
        CategoryBox.ItemsSource = Categories;

        if (_existing is not null)
        {
          
            TitleBox.Text = _existing.Title;
            DescBox.Text = _existing.Description ?? string.Empty;
            TargetValueBox.Text = _existing.TargetValue?.ToString() ?? string.Empty;
            TargetUnitBox.Text = _existing.TargetUnit ?? string.Empty;
            DaysBox.Text = _existing.DaysOfWeek ?? string.Empty;

            var matchedCategory = Categories.FirstOrDefault(c => c.CategoryId == _existing.CategoryId);
            CategoryBox.SelectedItem = matchedCategory;
            if (CategoryBox.SelectedIndex == -1 && Categories.Count > 0)
                CategoryBox.SelectedIndex = 0;

            FrequencyBox.SelectedIndex = _existing.FrequencyType == "SpecificDays" ? 1 : 0;
        }
        else
        {
           
            if (Categories.Count > 0)
                CategoryBox.SelectedIndex = 0;

            FrequencyBox.SelectedIndex = 0;
        }

        
        DaysBox.Visibility = FrequencyBox.SelectedIndex == 1
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void FrequencyBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
       
        if (DaysBox is null) return;

        if (FrequencyBox.SelectedItem is ComboBoxItem item
            && item.Tag?.ToString() == "SpecificDays")
            DaysBox.Visibility = Visibility.Visible;
        else
            DaysBox.Visibility = Visibility.Collapsed;
    }

   
    public (bool IsValid, HabitFormData? Data, string? ErrorMessage) TryBuildRequest()
    {
        if (string.IsNullOrWhiteSpace(TitleBox.Text))
            return (false, null, "يرجى إدخال اسم العادة.");

        if (CategoryBox.SelectedItem is not CategoryDto category)
            return (false, null, "يرجى اختيار الفئة.");

        var isSpecificDays = (FrequencyBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() == "SpecificDays";
        if (isSpecificDays && string.IsNullOrWhiteSpace(DaysBox.Text))
            return (false, null, "يرجى إدخال الأيام المحددة.");

        decimal? targetValue = null;
        if (!string.IsNullOrWhiteSpace(TargetValueBox.Text)
            && decimal.TryParse(TargetValueBox.Text, out var tv))
            targetValue = tv;

        var data = new HabitFormData(
            Title: TitleBox.Text.Trim(),
            Description: string.IsNullOrWhiteSpace(DescBox.Text) ? null : DescBox.Text.Trim(),
            TargetValue: targetValue,
            TargetUnit: string.IsNullOrWhiteSpace(TargetUnitBox.Text) ? null : TargetUnitBox.Text.Trim(),
            FrequencyType: isSpecificDays ? "SpecificDays" : "Daily",
            DaysOfWeek: isSpecificDays ? DaysBox.Text.Trim() : null,
            ColorHex: _existing?.ColorHex,
            IconName: _existing?.IconName,
            CategoryId: category.CategoryId
        );

        return (true, data, null);
    }

    public static CreateHabitRequest ToCreateRequest(HabitFormData data) =>
        new(data.Title, data.Description, data.TargetValue, data.TargetUnit,
            data.FrequencyType, data.DaysOfWeek,
            data.ColorHex ?? "#2196F3", data.IconName ?? "circle",
            data.CategoryId);

    public static UpdateHabitRequest ToUpdateRequest(HabitFormData data, HabitDto original) =>
        new(data.Title, data.Description, data.TargetValue, data.TargetUnit,
            data.FrequencyType, data.DaysOfWeek,
            data.ColorHex ?? original.ColorHex, data.IconName ?? original.IconName,
            data.CategoryId);
}

public sealed record HabitFormData(
    string Title,
    string? Description,
    decimal? TargetValue,
    string? TargetUnit,
    string FrequencyType,
    string? DaysOfWeek,
    string? ColorHex,
    string? IconName,
    int CategoryId);