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

        private void OpenLidarMsg_OnClick(object? sender, RoutedEventArgs e)
        {
            "Scripts/open_lidar_msg.sh".Bash();
        }

        private void TakePhoto_OnClick(object? sender, RoutedEventArgs e)
        {
            "Scripts/take_photo_for_lidar.sh".Bash();

            var viewModel = (LidarCalibrationControlViewModel)DataContext!;
        }

        private void RecordLidarBag_OnClick(object? sender, RoutedEventArgs e)
        {
            var viewModel = (LidarCalibrationControlViewModel)DataContext!;

            $"Scripts/take_photo_for_lidar.sh {viewModel.CurrentDataCollectIndex}".Bash();

            viewModel.IncreaceDataCollectIndex();
        }

        private void AnnotatePhoto_OnClick(object? sender, RoutedEventArgs e)
        {
            var viewModel = (LidarCalibrationControlViewModel)DataContext!;

            $"Scripts/annotate_photo.sh {viewModel.CurrentAnnoPhotoIndex}".Bash();

            viewModel.IncreaceAnnoPhotoIndex();
        }

        private void Convert2Pcd_OnClick(object? sender, RoutedEventArgs e)
        {
            $"Scripts/convert_rosbag_pcd.sh".Bash();
        }

        private void OpenFile_OnClick(object? sender, RoutedEventArgs e)
        {
            $"Scripts/open_lidar_corner_file.sh".Bash();
        }

        private void AnnotateLidar_OnClick(object? sender, RoutedEventArgs e)
        {
            var viewModel = (LidarCalibrationControlViewModel)DataContext!;

            $"Scripts/annotate_lidar.sh {viewModel.CurrentAnnoPhotoIndex}".Bash();

            viewModel.IncreaceAnnoLidarIndex();
        }

        private void GetExt1_OnClick(object? sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void GetExt2_OnClick(object? sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
