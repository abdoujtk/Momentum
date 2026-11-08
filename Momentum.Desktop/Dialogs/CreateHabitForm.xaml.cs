using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Momentum.Contracts.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace Momentum.Desktop.Dialogs;

public sealed partial class CreateHabitForm : UserControl
{
    public IReadOnlyList<CategoryDto> Categories { get; }

    public CreateHabitForm(IReadOnlyList<CategoryDto> categories)
    {
        InitializeComponent();
        Categories = categories;

      
        CategoryBox.ItemsSource = Categories;
        if (Categories.Count > 0) CategoryBox.SelectedIndex = 0;

        
        FrequencyBox.SelectedIndex = 0;
    }

    private void FrequencyBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
       
        if (DaysBox is null) return;

        if (FrequencyBox.SelectedItem is ComboBoxItem item && item.Tag.ToString() == "SpecificDays")
            DaysBox.Visibility = Visibility.Visible;
        else
            DaysBox.Visibility = Visibility.Collapsed;
    }

    public (bool IsValid, CreateHabitRequest? Request, string? ErrorMessage) TryBuildRequest()
    {
        if (string.IsNullOrWhiteSpace(TitleBox.Text))
            return (false, null, "يرجى إدخال اسم العادة.");

        if (CategoryBox.SelectedItem is not CategoryDto category)
            return (false, null, "يرجى اختيار الفئة.");

        var isSpecificDays = (FrequencyBox.SelectedItem as ComboBoxItem)?.Tag.ToString() == "SpecificDays";
        if (isSpecificDays && string.IsNullOrWhiteSpace(DaysBox.Text))
            return (false, null, "يرجى إدخال الأيام المحددة.");

        var request = new CreateHabitRequest(
            Title: TitleBox.Text.Trim(),
            Description: string.IsNullOrWhiteSpace(DescBox.Text) ? null : DescBox.Text.Trim(),
            TargetValue: null,
            TargetUnit: null,
            FrequencyType: isSpecificDays ? "SpecificDays" : "Daily",
            DaysOfWeek: isSpecificDays ? DaysBox.Text.Trim() : null,
            ColorHex: category.ColorHex,
            IconName: "circle",
            CategoryId: category.CategoryId
        );

        return (true, request, null);
    }
}