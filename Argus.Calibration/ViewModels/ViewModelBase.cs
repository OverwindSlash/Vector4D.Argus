using Argus.Calibration.Config;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace Argus.Calibration.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        public CalibrationConfig CalibConfig { get; set; }

        private bool _stereoCalibrated;

        public bool StereoCalibrated
        {
            get => _stereoCalibrated;
            set => this.RaiseAndSetIfChanged(ref _stereoCalibrated, value);
        }

        public ViewModelBase()
        {
            CalibConfig = new CalibrationConfig();
            CalibConfig.LoadFromJson();

            StereoCalibrated = false;
        }
    }
}