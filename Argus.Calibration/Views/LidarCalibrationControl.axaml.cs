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
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            MainWindowViewModel windowDataContext = (MainWindowViewModel)window.DataContext!;

            windowDataContext.AddOperationLog("检查激光雷达，确认后请关闭终端窗口，请勿直接关闭 rviz 窗口");
            "Scripts/check_lidar.sh".Bash();
        }

        private void OpenLidarMsg_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            MainWindowViewModel windowDataContext = (MainWindowViewModel)window.DataContext!;

            windowDataContext.AddOperationLog("开启激光雷达录制，请勿关闭终端窗口");

            "Scripts/open_lidar_msg.sh".Bash();
        }

        private void TakePhoto_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            MainWindowViewModel windowDataContext = (MainWindowViewModel)window.DataContext!;

            var viewModel = (LidarCalibrationControlViewModel)DataContext!;

            windowDataContext.AddOperationLog($"抓拍机载左目图像:{viewModel.CurrentDataCollectIndex}.png ......");            
            $"Scripts/take_photo_for_lidar.sh {viewModel.CurrentDataCollectIndex}".Bash();

            windowDataContext.AddOperationLog($"机载左目图像:{viewModel.CurrentDataCollectIndex}.png 抓拍完毕");
        }

        private void RecordLidarBag_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            MainWindowViewModel windowDataContext = (MainWindowViewModel)window.DataContext!;            

            var viewModel = (LidarCalibrationControlViewModel)DataContext!;

            windowDataContext.AddOperationLog($"录制激光雷达数据:{viewModel.CurrentDataCollectIndex}.bag ......");
            $"Scripts/record_lidar_bag.sh {viewModel.CurrentDataCollectIndex}".Bash();
            viewModel.IncreaceDataCollectIndex();
            windowDataContext.AddOperationLog($"激光雷达数据:{viewModel.CurrentDataCollectIndex}.bag 录制完毕");
        }

        private void AnnotatePhoto_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            MainWindowViewModel windowDataContext = (MainWindowViewModel)window.DataContext!;

            var viewModel = (LidarCalibrationControlViewModel)DataContext!;

            windowDataContext.AddOperationLog($"标注机载左目图像:{viewModel.CurrentAnnoPhotoIndex}.png ......");
            $"Scripts/annotate_photo.sh {viewModel.CurrentAnnoPhotoIndex}".Bash();
            viewModel.IncreaceAnnoPhotoIndex();
            windowDataContext.AddOperationLog($"机载左目图像:{viewModel.CurrentAnnoPhotoIndex}.png 标注完毕");
        }

        private void Convert2Pcd_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            MainWindowViewModel windowDataContext = (MainWindowViewModel)window.DataContext!;

            windowDataContext.AddOperationLog("将rosbag转换为pcd点云文件 ......");
            $"Scripts/convert_rosbag_pcd.sh".Bash();
            windowDataContext.AddOperationLog("pcd点云文件转换完毕");
        }

        private void OpenFile_OnClick(object? sender, RoutedEventArgs e)
        {
            $"Scripts/open_lidar_corner_file.sh".Bash();
        }

        private void AnnotateLidar_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            MainWindowViewModel windowDataContext = (MainWindowViewModel)window.DataContext!;

            var viewModel = (LidarCalibrationControlViewModel)DataContext!;
            windowDataContext.AddOperationLog($"标注点云文件:{viewModel.CurrentAnnoLidarIndex}.pcd ......");
            $"Scripts/annotate_lidar.sh {viewModel.CurrentAnnoLidarIndex}".Bash();
            viewModel.IncreaceAnnoLidarIndex();
            windowDataContext.AddOperationLog($"点云文件:{viewModel.CurrentAnnoLidarIndex}.pcd标注完毕");
        }

        private void GetExt1_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            MainWindowViewModel windowDataContext = (MainWindowViewModel)window.DataContext!;

            windowDataContext.AddOperationLog("计算激光雷达与机载相机外参 ......");
            $"Scripts/get_lidar_ext1.sh".Bash();
            windowDataContext.AddOperationLog("激光雷达与机载相机外参计算完毕");
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

        private void ProjectCloud_OnClick(object? sender, RoutedEventArgs e)
        {
            $"Scripts/project_cloud.sh".Bash();
        }
    }
}
