using System;
using System.Collections.Generic;
using System.Text;
using Argus.Calibration.Config;
using ReactiveUI;

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