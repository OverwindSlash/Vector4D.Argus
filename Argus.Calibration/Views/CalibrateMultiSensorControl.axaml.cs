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


        public void CalibrateRealSense()
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            var windowViewModel = (MainWindowViewModel)window.DataContext!;

            var viewModel = (CalibrateMultiSensorControlViewModel)DataContext!;
            viewModel.initRos();
            viewModel.Calibrate(windowViewModel);
        }

        private void SaveResult_OnClick()
        {
            var viewModel = (CalibrateMultiSensorControlViewModel)DataContext!;
            viewModel.SaveResult();
        }

        private void Visual_OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
        {
            var viewModel = (CalibrateMultiSensorControlViewModel)DataContext!;
            viewModel.Dispose();
        }
    }
}
