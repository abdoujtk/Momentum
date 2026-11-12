using Microsoft.UI.Xaml.Controls;
using Momentum.Desktop.ViewModels;
using Momentum.Desktop.ViewModels.Abstractions;
using Momentum.Desktop.Views;
using System;
using System.Collections.Generic;

namespace Momentum.Desktop.Services;

public sealed class NavigationService : INavigationService
{
    private Frame? _frame;
    private NavigationView? _navigationView;

   
    private readonly Dictionary<Type, Type> _viewModelToPageMap = new()
    {
        { typeof(TodayViewModel), typeof(TodayPage) },
        { typeof(HabitsViewModel), typeof(HabitsPage) },
         { typeof(HabitDetailsViewModel), typeof(HabitDetailsPage) }

    };

    
    public void Initialize(Frame frame, NavigationView navigationView)
    {
        _frame = frame;
        _navigationView = navigationView;
    }

    public void NavigateTo<TViewModel>(object? parameter = null) where TViewModel : class
        => NavigateTo(typeof(TViewModel), parameter);

  
    public void NavigateTo(Type viewModelType, object? parameter = null)
    {
        if (_frame is null) throw new InvalidOperationException("NavigationService is not initialized.");

        if (_viewModelToPageMap.TryGetValue(viewModelType, out var pageType))
        {
            // Allow navigation if parameter is provided (e.g., different habit details)
            if (_frame.CurrentSourcePageType == pageType && parameter is null) return;
            _frame.Navigate(pageType, parameter);
        }
    }

    public void GoBack()
    {
        if (CanGoBack) _frame?.GoBack();
    }

    public bool CanGoBack => _frame?.CanGoBack ?? false;

    public void NavigateToMainApp()
    {
        // Tell App.xaml.cs to switch windows
        App.CurrentApp.ShowMainShell();
    }
}