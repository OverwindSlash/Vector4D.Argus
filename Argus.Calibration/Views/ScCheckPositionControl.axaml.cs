using System;
using Argus.Calibration.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Argus.Calibration.Views
{
    public class ScCheckPositionControl : UserControl
    {
        public ScCheckPositionControl()
        {
            InitializeComponent();
            DataContext = new ScCheckPositionControlViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        // private void OK_OnClick(object? sender, RoutedEventArgs e)
        // {
        //     var controlViewModel = (ScCheckPositionControlViewModel)DataContext!;
        //     bool result = controlViewModel is {FoundCornersInLeftImage: true, FoundCornersInRightImage: true};
        //
        //     var window = (MainWindow) this.Parent.Parent.Parent.Parent;
        //     var windowViewModel = (MainWindowViewModel)window.DataContext!;
        //     
        //     window.CloseWorkAreaControl();
        // }
        //
        // private void Cancel_OnClick(object? sender, RoutedEventArgs e)
        // {
        //     var window = (MainWindow) this.Parent.Parent.Parent.Parent;
        //     var windowViewModel = (MainWindowViewModel)window.DataContext!;
        //     
        //     window.CloseWorkAreaControl();
        // }

        private void StyledElement_OnInitialized(object? sender, EventArgs e)
        {
            
        }
    }
}