using Avalonia.Controls;
using Irihi.Mirana.Demo.ViewModels;
using System.ComponentModel;

namespace Irihi.Mirana.Demo.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        // Subscribe to ViewModel property changes
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
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