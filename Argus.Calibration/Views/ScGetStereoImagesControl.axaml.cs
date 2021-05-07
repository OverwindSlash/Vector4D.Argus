using Argus.Calibration.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Argus.Calibration.Views
{
    public class ScGetStereoImagesControl : UserControl
    {
        public ScGetStereoImagesControl()
        {
            InitializeComponent();
            DataContext = new ScGetStereoImagesControlViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Capture_OnClick(object? sender, RoutedEventArgs e)
        {
            var viewModel = (ScGetStereoImagesControlViewModel)DataContext!;
            viewModel.CaptureStereoImages();
        }

        private void OK_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = (MainWindow) this.Parent.Parent.Parent.Parent;
            var windowViewModel = (MainWindowViewModel)window.DataContext!;
            windowViewModel.ImagesCaptured = true;
        }

        private void Cancel_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = (MainWindow) this.Parent.Parent.Parent.Parent;
            window.CloseWorkAreaControl();
        }
    }
}