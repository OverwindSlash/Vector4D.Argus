using Argus.Calibration.Helper;
using Argus.Calibration.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RosSharp;
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
            viewModel.Calibrate(windowViewModel);
            Ros.MasterUri = new Uri(viewModel.CalibConfig.RosMasterUri);
            Ros.HostName = viewModel.CalibConfig.HostName;
            Ros.TopicTimeout = viewModel.CalibConfig.TopicTimeout;
            Ros.XmlRpcTimeout = viewModel.CalibConfig.XmlRpcTimeout;

            var node = Ros.InitNodeAsync(viewModel.CalibConfig.NodeName).Result;
            var subscriber = node.SubscriberAsync<RosSharp.std_msgs.String>(@"/qt_echo_topic").Result;

            subscriber.Subscribe(x =>
            {
                viewModel.Result = x.data;
            });

        }
    }
}
