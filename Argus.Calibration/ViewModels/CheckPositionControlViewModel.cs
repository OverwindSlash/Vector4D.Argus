using Argus.Calibration.Config;
using Argus.Calibration.Helper;
using Argus.StereoCalibration;
using Avalonia.Media.Imaging;
using JetBrains.Annotations;
using ReactiveUI;
using System.IO;
using System.Threading.Tasks;

namespace Argus.Calibration.ViewModels
{
    public class CheckPositionControlViewModel : ViewModelBase
    {
        private const string SnapshotsDir = "PositionCheckSnapshots";

        private StereoTypes _stereoType;
        private MainWindowViewModel _mainWindowVm;

        [NotNull] private string _leftImagePath;
        public string LeftImagePath
        {
            get => _leftImagePath;
            private set
            {
                this.RaiseAndSetIfChanged(ref _leftImagePath, value);
                this.RaisePropertyChanged(nameof(LeftImage));
            }
        }
        public Bitmap LeftImage
        {
            get
            {
                if (_leftImagePath != null)
                {
                    return new Bitmap(_leftImagePath);
                }

                return null;
            }
        }

        [NotNull] private string _rightImagePath;
        public string RightImagePath
        {
            get => _rightImagePath;
            private set
            {
                this.RaiseAndSetIfChanged(ref _rightImagePath, value);
                this.RaisePropertyChanged(nameof(RightImage));
            }
        }
        public Bitmap RightImage
        {
            get
            {
                if (_rightImagePath != null)
                {
                    return new Bitmap(_rightImagePath);
                }

                return null;
            }
        }

        public bool FoundCornersInLeftImage { get; private set; }
        public bool FoundCornersInRightImage { get; private set; }

        public CheckPositionControlViewModel(StereoTypes stereoType, MainWindowViewModel mainWindowVm)
        {
            _stereoType = stereoType;
            _mainWindowVm = mainWindowVm;
        }

        public async Task CheckPositionAsync()
        {
            _mainWindowVm.AddOperationLog("请等待机械臂移动至抓拍位置......");

            // 1. Move robot arms to snapshot positions.
            if (_stereoType == StereoTypes.BodyStereo)
            {
                // 1.1 Body stereo check only need move left arm to initial position
                string filepath = Path.Combine(CalibConfig.MovementFileDir, CalibConfig.BodyStereoArmPositionFile);
                string[] positions = File.ReadAllText(filepath).Split("\n");

                await Task.Run(() =>
                {
                    _mainWindowVm.AddOperationLog($"将左臂移动至 {positions[0]}");
                    string moveLeftCmd = $"Scripts/move_leftarm.sh '{positions[0]}'";
                    moveLeftCmd.RunSync();
                });
            }
            else
            {
                // 1.2 Arm stereo check need move left and right arm to initial position
                string filepath = Path.Combine(CalibConfig.MovementFileDir, CalibConfig.ArmToolsPositionFiles[(int)_stereoType]);
                string[] positions = File.ReadAllText(filepath).Split("\n");

                // Is left or right arm tool
                bool isLeftArmTool = (int) _stereoType % 2 == 0;
                string leftArmPosition = isLeftArmTool ? positions[0] : positions[1];
                string rightArmPosition = isLeftArmTool ? positions[1] : positions[0];

                // Move left and right arm to initial position
                await Task.Run(() =>
                {
                    _mainWindowVm.AddOperationLog($"将左臂移动至 {leftArmPosition}");
                    string moveLeftCmd = $"Scripts/move_leftarm.sh '{leftArmPosition}'";
                    moveLeftCmd.RunSync();

                    _mainWindowVm.AddOperationLog($"将右臂移动至 {rightArmPosition}");
                    string moveRightCmd = $"Scripts/move_rightarm.sh '{rightArmPosition}'";
                    moveRightCmd.RunSync();
                });
            }

            // 2. Take snapshot.
            FsHelper.PurgeDirectory(SnapshotsDir);

            _mainWindowVm.AddOperationLog("请等待抓拍完成......");
            await Task.Run(() =>
            {
                string snapshotCmd = $"Scripts/snapshot_body.sh '{SnapshotsDir}'";
                snapshotCmd.RunSync();
            });

            await SimulateSnapshotAsync();

            // 3. Find corner.
            string leftSnapshotDir = Path.Combine(SnapshotsDir, "left");
            string rightSnapshotDir = Path.Combine(SnapshotsDir, "right");
            LeftImagePath = FsHelper.GetFirstFileByNameFromDirectory(leftSnapshotDir, "left");
            RightImagePath = FsHelper.GetFirstFileByNameFromDirectory(rightSnapshotDir, "right");
            _mainWindowVm.AddOperationLog($"左目抓拍图像保存至: {LeftImagePath}");
            _mainWindowVm.AddOperationLog($"右目抓拍图像保存至: {RightImagePath}");

            _mainWindowVm.AddOperationLog("图像角点识别中......");
            await Task.Run(() =>
            {
                FoundCornersInLeftImage = CameraCalibrator.CheckAndDrawConCorners(LeftImagePath);
                FoundCornersInRightImage = CameraCalibrator.CheckAndDrawConCorners(RightImagePath);
            });

            this.RaisePropertyChanged(nameof(LeftImage));
            this.RaisePropertyChanged(nameof(RightImage));

            _mainWindowVm.AddOperationLog("抓拍完成");
        }

        private async Task SimulateSnapshotAsync()
        {
            await Task.Run(() =>
            {
                FileInfo srcLeftFi = new FileInfo("Images/left/Left1.jpg");
                FileInfo srcRightFi = new FileInfo("Images/right/Right1.jpg");

                FsHelper.EnsureDirectoryExist("PositionCheckSnapshots/left");
                FsHelper.EnsureDirectoryExist("PositionCheckSnapshots/right");

                srcLeftFi.CopyTo("PositionCheckSnapshots/left/Left.jpg", true);
                srcRightFi.CopyTo("PositionCheckSnapshots/right/Right.jpg", true);
            });
        }
    }
}