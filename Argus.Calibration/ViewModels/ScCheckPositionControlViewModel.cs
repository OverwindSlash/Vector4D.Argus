using System.Threading;
using System.Threading.Tasks;
using Argus.Calibration.Helper;
using Argus.StereoCalibration;
using Avalonia.Media.Imaging;
using JetBrains.Annotations;
using ReactiveUI;

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

        public ScCheckPositionControlViewModel()
        {
            _leftImagePath = "PositionCheckSnapshots/Left.jpg";
            _rightImagePath = "PositionCheckSnapshots/Right.jpg";
            
            FsHelper.PurgeDirectory(SnapshotsDir);

            Task<int> bash1Task = "cp Assets/Left.jpg PositionCheckSnapshots/".Bash();
            Task<int> bash2Task = "cp Assets/Right.jpg PositionCheckSnapshots/".Bash();

            bash1Task.Wait();
            bash2Task.Wait();

            LeftImagePath = FsHelper.GetFirstFileByNameFromDirectory(SnapshotsDir, "left");
            RightImagePath = FsHelper.GetFirstFileByNameFromDirectory(SnapshotsDir, "right");

            FoundCornersInLeftImage = CameraCalibrator.CheckAndDrawConCorners(LeftImagePath);
            FoundCornersInRightImage = CameraCalibrator.CheckAndDrawConCorners(RightImagePath);
        }
    }
}