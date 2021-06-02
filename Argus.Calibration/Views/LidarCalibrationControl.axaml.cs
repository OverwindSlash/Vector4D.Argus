using Argus.Calibration.Helper;
using Argus.Calibration.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Argus.Calibration.Views
{
    public partial class LidarCalibrationControl : UserControl
    {
        public LidarCalibrationControl()
        {
            InitializeComponent();
            DataContext = new LidarCalibrationControlViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void CheckLidar_OnClick(object? sender, RoutedEventArgs e)
        {
            "Scripts/check_lidar.sh".Bash();
        }
    }
}
