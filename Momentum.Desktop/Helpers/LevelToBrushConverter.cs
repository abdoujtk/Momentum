using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace Momentum.Desktop.Helpers;

public class LevelToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not int level) return new SolidColorBrush(Colors.Transparent);

        // GitHub-style colors (light green to dark green)
        return level switch
        {
            0 => new SolidColorBrush(Color.FromArgb(255, 235, 237, 240)), // Light Gray
            1 => new SolidColorBrush(Color.FromArgb(255, 155, 233, 168)), // Light Green
            2 => new SolidColorBrush(Color.FromArgb(255, 64, 196, 99)),   // Medium Green
            3 => new SolidColorBrush(Color.FromArgb(255, 48, 161, 78)),   // Dark Green
            4 => new SolidColorBrush(Color.FromArgb(255, 33, 110, 57)),   // Darkest Green
            _ => new SolidColorBrush(Colors.Transparent)
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}