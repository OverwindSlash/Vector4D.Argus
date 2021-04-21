using ReactiveUI;

namespace Argus.Calibration.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _positionChecked;
        private bool _stereoCalibrated;

        public bool PositionChecked
        {
            get => _positionChecked;
            set => this.RaiseAndSetIfChanged(ref _positionChecked, value);
        }

        public bool StereoCalibrated
        {
            get => _stereoCalibrated;
            set => this.RaiseAndSetIfChanged(ref _stereoCalibrated, value);
        }

        public MainWindowViewModel()
        {
            PositionChecked = false;
            StereoCalibrated = false;
        }
    }
}