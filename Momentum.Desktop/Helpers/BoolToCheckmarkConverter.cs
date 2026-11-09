using Microsoft.UI.Xaml.Data;
using System;

namespace Momentum.Desktop.Helpers;

public class BoolToCheckmarkConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is true ? "\uE73E" : "\uE711"; // Checkmark or X

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}