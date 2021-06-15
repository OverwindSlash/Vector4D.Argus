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

            viewModel.initRos();
            viewModel.CalibrateTurntable(windowViewModel);

        }

        private void Visual_OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
        {
            var viewModel = (CalibrateTurntableControlViewModel)DataContext!;
            viewModel.Dispose();
        }
    }
}
