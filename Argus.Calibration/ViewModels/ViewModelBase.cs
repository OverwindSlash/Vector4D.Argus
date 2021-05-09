using Argus.Calibration.Config;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace Argus.Calibration.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        public CalibrationConfig CalibConfig { get; set; }

        public ViewModelBase()
        {
            CalibConfig = new CalibrationConfig();
            CalibConfig.LoadFromJson();
        }
    }
}