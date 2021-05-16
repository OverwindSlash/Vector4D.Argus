using Argus.Calibration.Config;
using Argus.Calibration.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Argus.Calibration.Views
{
    public class ScCalibrateStereoControl : UserControl
    {
        public ScCalibrateStereoControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void InitDataContext(StereoTypes stereoType)
        {
            DataContext = new CalibrateStereoControlViewModel(stereoType);
        }

        private async void Capture_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            var windowViewModel = (MainWindowViewModel)window.DataContext!;

            var viewModel = (CalibrateStereoControlViewModel)DataContext!;
            await viewModel.CaptureStereoImages(windowViewModel);
        }

        private void CalibrateStereo_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            var windowViewModel = (MainWindowViewModel)window.DataContext!;

            var viewModel = (CalibrateStereoControlViewModel)DataContext!;
            viewModel.CalibrateStereo(windowViewModel);
        }

        private void Cancel_OnClick(object? sender, RoutedEventArgs e)
        {
            var viewModel = (CalibrateStereoControlViewModel)DataContext!;
            viewModel.CancelOperation();
        }
    }
}