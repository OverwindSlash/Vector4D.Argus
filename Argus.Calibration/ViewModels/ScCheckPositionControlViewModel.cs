using System.Diagnostics;
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
    public class ScCheckPositionControlViewModel : ViewModelBase
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

        public ScCheckPositionControlViewModel(StereoTypes stereoType, MainWindowViewModel mainWindowVm)
        {
            _stereoType = stereoType;
            _mainWindowVm = mainWindowVm;
        }

        public async Task CheckPositionAsync()
        {
            _mainWindowVm.AddOperationLog("请等待机械臂移动至抓拍位置......");

            if (_stereoType == StereoTypes.BodyStereo)
            {
                string filepath = Path.Combine(CalibConfig.MovementFileDir, CalibConfig.BodyStereoArmPositionFile);
                string[] positions = File.ReadAllText(filepath).Split("\n");

                // Move left arm to initial position
                await Task.Run(() =>
                {
                    // TODO：Change to real script
                    "Mock/fake_cmd.sh".RunSync();   // Move left arm
                });
            }
            else
            {
                string filepath = Path.Combine(CalibConfig.MovementFileDir, CalibConfig.ArmToolsPositionFiles[(int)_stereoType]);
                string[] positions = File.ReadAllText(filepath).Split("\n");

                // Move left and right arm to initial position
                await Task.Run(() =>
                {
                    // TODO：Change to real script
                    "Mock/fake_cmd.sh".RunSync();   // Move left arm
                    "Mock/fake_cmd.sh".RunSync();   // Move right arm
                });
            }

            // Take snapshot
            FsHelper.PurgeDirectory(SnapshotsDir);

            _mainWindowVm.AddOperationLog("请等待抓拍完成......");
            await Task.Run(() =>
            {
                // TODO：Change to real script
                "Mock/fake_cmd.sh".RunSync();   // Snapshot
            });

            await SimulateSnapshotAsync();

            LeftImagePath = FsHelper.GetFirstFileByNameFromDirectory(SnapshotsDir, "left");
            RightImagePath = FsHelper.GetFirstFileByNameFromDirectory(SnapshotsDir, "right");

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
                "cp Images/Left.jpg PositionCheckSnapshots/".RunSync();
                "cp Images/Right.jpg PositionCheckSnapshots/".RunSync();
            });

            _leftImagePath = "PositionCheckSnapshots/Left.jpg";
            _rightImagePath = "PositionCheckSnapshots/Right.jpg";
        }
    }
}