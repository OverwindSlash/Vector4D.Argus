using Argus.Calibration.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;

namespace Argus.Calibration.Views
{
    public class ScCalibrateStereoControl : UserControl
    {
        public ScCalibrateStereoControl()
        {
            InitializeComponent();
            DataContext = new CalibrateStereoControlViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
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

        private void Result_OnClick(object? sender, RoutedEventArgs e)
        {
            var viewModel = (CalibrateStereoControlViewModel)DataContext!;
            viewModel.ShowStereoCalibrationResult();
        }
    }
}