using ReactiveUI;

namespace Argus.Calibration.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _imagesCaptured;
        private bool _stereoCalibrated;

        public bool ImagesCaptured
        {
            get => _imagesCaptured;
            set => this.RaiseAndSetIfChanged(ref _imagesCaptured, value);
        }

        public bool StereoCalibrated
        {
            get => _stereoCalibrated;
            set => this.RaiseAndSetIfChanged(ref _stereoCalibrated, value);
        }

        public MainWindowViewModel()
        {
            ImagesCaptured = false;
            StereoCalibrated = false;
        }
    }
}