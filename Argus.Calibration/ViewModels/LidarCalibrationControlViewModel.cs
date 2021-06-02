using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Argus.Calibration.ViewModels
{
    public class LidarCalibrationControlViewModel : ViewModelBase
    {
        public int CurrentDataCollectIndex { get; private set; }

        public int CurrentAnnoPhotoIndex { get; private set; }

        public int CurrentAnnoLidarIndex { get; private set; }

        public LidarCalibrationControlViewModel()
        {
            CurrentDataCollectIndex = 0;
            CurrentAnnoPhotoIndex = 0;
            CurrentAnnoLidarIndex = 0;
        }

        public void IncreaceDataCollectIndex()
        {
            CurrentDataCollectIndex++;
            this.RaisePropertyChanged(nameof(CurrentDataCollectIndex));
        }

        public void IncreaceAnnoPhotoIndex()
        {
            CurrentAnnoPhotoIndex++;
            this.RaisePropertyChanged(nameof(CurrentAnnoPhotoIndex));
        }

        public void IncreaceAnnoLidarIndex()
        {
            CurrentAnnoLidarIndex++;
            this.RaisePropertyChanged(nameof(CurrentAnnoLidarIndex));
        }
    }
}
