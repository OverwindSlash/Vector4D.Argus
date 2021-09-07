using Argus.Calibration.Helper;
using Argus.Calibration.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;

namespace Argus.Calibration.Views
{
    public partial class CalibrateMultiSensorControl : UserControl
    {
        public CalibrateMultiSensorControl()
        {
            InitializeComponent();
            DataContext = new CalibrateMultiSensorControlViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }


        public void CalibrateRealSense(string robotName)
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            var windowViewModel = (MainWindowViewModel)window.DataContext!;

            var viewModel = (CalibrateMultiSensorControlViewModel)DataContext!;
            viewModel.initRos();
            viewModel.RobotName = robotName;
            viewModel.Calibrate(windowViewModel);
        }

        private void Visual_OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
        {
            var viewModel = (CalibrateMultiSensorControlViewModel)DataContext!;
            viewModel.Dispose();
        }

        private void SaveResult_OnClick(object? sender, RoutedEventArgs e)
        {
            var viewModel = (CalibrateMultiSensorControlViewModel)DataContext!;
            viewModel.SaveResult();
        }
    }
}
