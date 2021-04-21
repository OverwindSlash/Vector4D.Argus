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
            DataContext = new ScCalibrateStereoControlViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Calibrate_OnClick(object? sender, RoutedEventArgs e)
        {
            var viewModel = (ScCalibrateStereoControlViewModel)DataContext!;
            viewModel.CalibrateStereo();
        }

        private void Close_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = (MainWindow) this.Parent.Parent.Parent.Parent;
            window.CloseWorkAreaControl();
        }
    }
}