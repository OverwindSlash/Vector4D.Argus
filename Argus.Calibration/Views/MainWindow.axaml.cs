using Argus.Calibration.Config;
using Argus.Calibration.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Argus.Calibration.Views
{
    public class MainWindow : Window
    {
        [NotNull] private readonly StackPanel _workArea;
        [NotNull] private readonly MenuItem _toolTypeMenu;
        [NotNull] private readonly ComboBox _toolTypeCombo;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

#if DEBUG
            this.AttachDevTools();
#endif

            _workArea = this.FindControl<StackPanel>("WorkArea");
            _toolTypeMenu = this.FindControl<MenuItem>("SelectToolType");
            _toolTypeCombo = this.FindControl<ComboBox>("ToolType");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void ShowCheckStereoImagesControlAsync(object? sender, RoutedEventArgs e)
        {
            _workArea.Children.Clear();
            ScCheckPositionControl control = new ScCheckPositionControl();
            _workArea.Children.Add(control);

            control.InitDataContext(StereoTypes.BodyStereo);
            await control.CheckPositionAsync();
        }

        public void CloseWorkAreaControl()
        {
            _workArea.Children.Clear();
        }

        private void ShowCalibrateStereoControl(object? sender, RoutedEventArgs e)
        {
            _workArea.Children.Clear();
            ScCalibrateStereoControl control = new ScCalibrateStereoControl();
            _workArea.Children.Add(control);

            control.InitDataContext(StereoTypes.BodyStereo);
        }

        private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_toolTypeCombo == null)
            {
                return;
            }

            int selectedIndex = _toolTypeCombo.SelectedIndex;

            var viewModel = (MainWindowViewModel)DataContext!;
            viewModel.SelectedToolType = selectedIndex;
        }

        private void SelectToolType(int toolType)
        {
            var viewModel = (MainWindowViewModel)DataContext!;
            viewModel.SelectedToolType = toolType;
        }

        private void SelectToolMenu_OnClick(object? sender, RoutedEventArgs e)
        {
            SelectToolType(_toolTypeMenu.SelectedIndex);
        }

        private async void ShowCheckArmStereoImagesControl(object? sender, RoutedEventArgs e)
        {
            _workArea.Children.Clear();
            ScCheckPositionControl control = new ScCheckPositionControl();
            _workArea.Children.Add(control);

            control.InitDataContext((StereoTypes)_toolTypeCombo.SelectedIndex);
            await control.CheckPositionAsync();
        }

        private void ShowLeftArmHandEyeCalibControl(object? sender, RoutedEventArgs e)
        {
            _workArea.Children.Clear();

            HandEyeCalibrationControl control = new HandEyeCalibrationControl();
            control.SetArm(RobotArms.LeftArm);
            control.SetStereoTypes(StereoTypes.BodyStereo);

            _workArea.Children.Add(control);
        }

        private void ShowRightArmHandEyeCalibControl(object? sender, RoutedEventArgs e)
        {
            _workArea.Children.Clear();

            HandEyeCalibrationControl control = new HandEyeCalibrationControl();
            control.SetArm(RobotArms.RightArm);
            control.SetStereoTypes(StereoTypes.BodyStereo);

            _workArea.Children.Add(control);
        }

        private void ShowTurntableCalibControl(object? sender, RoutedEventArgs e)
        {
            _workArea.Children.Clear();

            CalibrateTurntableControl control = new CalibrateTurntableControl();            

            _workArea.Children.Add(control);

            control.CalibrateTurntable();
        }
        private void ShowRealSenseCalibControl(object? sender, RoutedEventArgs e)
        {
            _workArea.Children.Clear();

            CalibrateMultiSensorControl control = new CalibrateMultiSensorControl();

            _workArea.Children.Add(control);

            control.CalibrateRealSense();
        }
        private void ShowCalibrateArmStereoControl(object? sender, RoutedEventArgs e)
        {
            _workArea.Children.Clear();
            ScCalibrateStereoControl control = new ScCalibrateStereoControl();
            _workArea.Children.Add(control);

            control.InitDataContext((StereoTypes)_toolTypeCombo.SelectedIndex);
        }

        private void ShowArmHandEyeCalibControl(object? sender, RoutedEventArgs e)
        {
            _workArea.Children.Clear();

            HandEyeCalibrationControl control = new HandEyeCalibrationControl();
            control.SetArm(RobotArms.LeftArm);
            control.SetStereoTypes((StereoTypes) _toolTypeCombo.SelectedIndex);

            _workArea.Children.Add(control);
        }

        private void Window_OnClosing(object? sender, CancelEventArgs e)
        {
            var viewModel = (MainWindowViewModel)DataContext!;
            viewModel.CleanUp();
        }

        private void ShowLidarCalibControl(object? sender, RoutedEventArgs e)
        {
            _workArea.Children.Clear();

            LidarCalibrationControl control = new LidarCalibrationControl();

            _workArea.Children.Add(control);
        }
    }
}