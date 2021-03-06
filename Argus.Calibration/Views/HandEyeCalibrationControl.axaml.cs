using Argus.Calibration.Config;
using Argus.Calibration.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using Avalonia.Interactivity;

namespace Argus.Calibration.Views
{
    public partial class HandEyeCalibrationControl : UserControl
    {
        public HandEyeCalibrationControl()
        {
            InitializeComponent();
            DataContext = new HandEyeCalibrationControlViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Visual_OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
        {
            var viewModel = (HandEyeCalibrationControlViewModel)DataContext!;
            viewModel.Dispose();
        }

        public void SetArm(RobotArms arm)
        {
            var viewModel = (HandEyeCalibrationControlViewModel)DataContext!;
            viewModel.SetArm(arm);
        }

        public void SetStereoTypes(StereoTypes stereoType)
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            MainWindowViewModel windowDataContext = (MainWindowViewModel)window.DataContext!;
            
            var viewModel = (HandEyeCalibrationControlViewModel)DataContext!;
            viewModel.SetStereoTypes(stereoType, windowDataContext);
        }

        private void MoveTurntablePrepos_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            var windowViewModel = (MainWindowViewModel)window.DataContext!;

            var viewModel = (HandEyeCalibrationControlViewModel)DataContext!;

            ComboBox combo = this.FindControl<ComboBox>("PrePosition");            

            viewModel.MoveTurntable(combo.SelectedIndex, windowViewModel);
        }

        private void HandEyeCalibrate_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            var windowViewModel = (MainWindowViewModel)window.DataContext!;

            var viewModel = (HandEyeCalibrationControlViewModel)DataContext!;
            viewModel.CalibrateHandEye(windowViewModel);
        }

        private void SaveResult_OnClick(object? sender, RoutedEventArgs e)
        {
            var viewModel = (HandEyeCalibrationControlViewModel)DataContext!;
            viewModel.SaveResult();
        }

        internal void SetRobotName(string robotName)
        {
            var viewModel = (HandEyeCalibrationControlViewModel)DataContext!;
            viewModel.RobotName = robotName;
        }

        internal void SetPresetPos(int presetPosIndex)
        {
            var viewModel = (HandEyeCalibrationControlViewModel)DataContext!;
            viewModel.PresetPos = presetPosIndex;
        }
    }
}
