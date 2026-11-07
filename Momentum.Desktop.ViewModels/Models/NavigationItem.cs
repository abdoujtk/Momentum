namespace Momentum.Desktop.ViewModels.Models;

public sealed class NavigationItem
{
    public string Title { get; }
    public Type ViewModelType { get; }
    public string IconGlyph { get; } 

    public NavigationItem(string title, Type viewModelType, string iconGlyph)
    {
        Title = title;
        ViewModelType = viewModelType;
        IconGlyph = iconGlyph;
    }
}