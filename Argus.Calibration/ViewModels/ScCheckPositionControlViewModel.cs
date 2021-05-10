﻿using Argus.Calibration.Config;
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
        public Bitmap LeftImage => new Bitmap(_leftImagePath);

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
        public Bitmap RightImage => new Bitmap(_rightImagePath);

        public bool FoundCornersInLeftImage { get; private set; }
        public bool FoundCornersInRightImage { get; private set; }

        public ScCheckPositionControlViewModel(StereoTypes stereoType)
        {
            if (stereoType == StereoTypes.BodyStereo)
            {
                string filepath = Path.Combine(CalibConfig.MovementFileDir, CalibConfig.BodyStereoArmPositionFile);

                string[] positions = File.ReadAllText(filepath).Split("\n");

                // Move left arm to initial position
                Task<int> moveLeftArmTask = "ls".Bash();
                moveLeftArmTask.Wait();

                // Take snapshot
                FsHelper.PurgeDirectory(SnapshotsDir);

                // TODO：Change to real script
                Task<int> snapshotTask = "ls".Bash();
                SimulateSnapshot();
            }
            else
            {
                string filepath = Path.Combine(CalibConfig.MovementFileDir, CalibConfig.ArmToolsPositionFiles[(int)stereoType]);

                string[] positions = File.ReadAllText(filepath).Split("\n");

                // Move left and right arm to initial position
                // TODO：Change to real script
                Task<int> moveLeftArmTask = "ls".Bash();
                Task<int> moveRightArmTask = "ls".Bash();

                moveLeftArmTask.Wait();
                moveRightArmTask.Wait();

                // Take snapshot
                FsHelper.PurgeDirectory(SnapshotsDir);

                // TODO：Change to real script
                Task<int> snapshotTask = "ls".Bash();
                SimulateSnapshot();
            }


            LeftImagePath = FsHelper.GetFirstFileByNameFromDirectory(SnapshotsDir, "left");
            RightImagePath = FsHelper.GetFirstFileByNameFromDirectory(SnapshotsDir, "right");

            FoundCornersInLeftImage = CameraCalibrator.CheckAndDrawConCorners(LeftImagePath);
            FoundCornersInRightImage = CameraCalibrator.CheckAndDrawConCorners(RightImagePath);
        }

        private void SimulateSnapshot()
        {
            _leftImagePath = "PositionCheckSnapshots/Left.jpg";
            _rightImagePath = "PositionCheckSnapshots/Right.jpg";

            Task<int> bash1Task = "cp Images/Left.jpg PositionCheckSnapshots/".Bash();
            Task<int> bash2Task = "cp Images/Right.jpg PositionCheckSnapshots/".Bash();

            bash1Task.Wait();
            bash2Task.Wait();
        }
    }
}