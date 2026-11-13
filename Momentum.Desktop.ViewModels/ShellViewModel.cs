using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Momentum.Desktop.ViewModels.Abstractions;
using Momentum.Desktop.ViewModels.Models;
using System.Collections.ObjectModel;

namespace Momentum.Desktop.ViewModels;

public sealed partial class ShellViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    public ObservableCollection<NavigationItem> NavigationItems { get; } = new();

    [ObservableProperty]
    private NavigationItem? _selectedItem;

    public ShellViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;

        
        NavigationItems.Add(new NavigationItem("اليوم", typeof(TodayViewModel), "\uE787"));
        NavigationItems.Add(new NavigationItem("العادات", typeof(HabitsViewModel), "\uE8FD"));

    }

    partial void OnSelectedItemChanged(NavigationItem? value)
    {
        if (value is not null)
        {
            _navigationService.NavigateTo(value.ViewModelType);
        }
    }
    private bool _isLoggingOut;

    [RelayCommand]
    private void Logout()
    {
        if (_isLoggingOut) return; 
        _isLoggingOut = true;

        _navigationService.NavigateToLoginShell();
    }
}