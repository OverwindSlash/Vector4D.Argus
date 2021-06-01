using System;
using System.Diagnostics;
using Argus.Calibration.Helper;
using Argus.Calibration.ViewModels;
using Argus.Calibration.Config;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RosSharp;
using RosSharp.Topic;

namespace Argus.Calibration.Views
{
    public partial class CalibrateTurntableControl : UserControl
    {
        public CalibrateTurntableControl()
        {
            InitializeComponent();
            DataContext = new CalibrateTurntableControlViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OpenRosCore_OnClick(object? sender, RoutedEventArgs e)
        {
            $"Scripts/init_roscore.sh".Bash();
        }

        private void CloseRosCore_OnClick(object? sender, RoutedEventArgs e)
        {
            $"Scripts/process_stopper.sh roscore".Bash();
        }

        public void CalibrateTurntable()
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            var windowViewModel = (MainWindowViewModel)window.DataContext!;

            var viewModel = (CalibrateTurntableControlViewModel)DataContext!;
            viewModel.CalibrateTurntable(windowViewModel);

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

        private void Visual_OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
        {
            var viewModel = (CalibrateTurntableControlViewModel)DataContext!;
            viewModel.Dispose();
        }
    }
}
