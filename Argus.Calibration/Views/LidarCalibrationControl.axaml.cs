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
            var viewModel = (LidarCalibrationControlViewModel)DataContext!;

            $"Scripts/take_photo_for_lidar.sh {viewModel.CurrentDataCollectIndex}".Bash();
        }

        private void RecordLidarBag_OnClick(object? sender, RoutedEventArgs e)
        {
            var viewModel = (LidarCalibrationControlViewModel)DataContext!;

            $"Scripts/record_lidar_bag.sh {viewModel.CurrentDataCollectIndex}".Bash();

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

            $"Scripts/annotate_lidar.sh {viewModel.CurrentAnnoLidarIndex}".Bash();

            viewModel.IncreaceAnnoLidarIndex();
        }

        private void GetExt1_OnClick(object? sender, RoutedEventArgs e)
        {
            $"Scripts/get_lidar_ext1.sh".Bash();
        }

        private void GetExt2_OnClick(object? sender, RoutedEventArgs e)
        {
            $"Scripts/get_lidar_ext2.sh".Bash();
        }

        private void IncreaseCollectIndex(object? sender, RoutedEventArgs e)
        {
            var viewModel = (LidarCalibrationControlViewModel)DataContext!;
            viewModel.IncreaceDataCollectIndex();
        }

        private void DecreaseCollectIndex(object? sender, RoutedEventArgs e)
        {
            var viewModel = (LidarCalibrationControlViewModel)DataContext!;
            viewModel.DecreaceDataCollectIndex();
        }

        private void IncreaseAnnoPhotoIndex(object? sender, RoutedEventArgs e)
        {
            var viewModel = (LidarCalibrationControlViewModel)DataContext!;
            viewModel.IncreaceAnnoPhotoIndex();
        }

        private void DecreaseAnnoPhotoIndex(object? sender, RoutedEventArgs e)
        {
            var viewModel = (LidarCalibrationControlViewModel)DataContext!;
            viewModel.DecreaceAnnoPhotoIndex();
        }

        private void IncreaseAnnoLidarIndex(object? sender, RoutedEventArgs e)
        {
            var viewModel = (LidarCalibrationControlViewModel)DataContext!;
            viewModel.IncreaceAnnoLidarIndex();
        }

        private void DecreaseAnnoLidarIndex(object? sender, RoutedEventArgs e)
        {
            var viewModel = (LidarCalibrationControlViewModel)DataContext!;
            viewModel.DecreaceAnnoLidarIndex();
        }

        private void ColorLidar_OnClick(object? sender, RoutedEventArgs e)
        {
            $"Scripts/color_lidar.sh".Bash();
        }
    }
}
