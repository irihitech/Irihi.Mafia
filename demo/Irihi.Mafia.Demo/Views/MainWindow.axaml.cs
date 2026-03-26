using Avalonia.Controls;
using Irihi.Mafia.Demo.ViewModels;
using System.ComponentModel;

namespace Irihi.Mafia.Demo.Views;

public partial class MainWindow : Window
{
    private MainViewModel? _currentViewModel;

    public MainWindow()
    {
        InitializeComponent();
        
        // Subscribe to ViewModel property changes
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        // Unsubscribe from previous ViewModel if exists
        if (_currentViewModel != null)
        {
            _currentViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }
        
        if (DataContext is MainViewModel viewModel)
        {
            _currentViewModel = viewModel;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            
            // Apply initial resolution
            if (viewModel.SelectedResolution != null)
            {
                Width = viewModel.SelectedResolution.Width;
                Height = viewModel.SelectedResolution.Height;
            }
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.SelectedResolution) && sender is MainViewModel viewModel)
        {
            if (viewModel.SelectedResolution != null)
            {
                Width = viewModel.SelectedResolution.Width;
                Height = viewModel.SelectedResolution.Height;
            }
        }
    }
}