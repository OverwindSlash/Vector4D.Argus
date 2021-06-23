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
            if (CurrentDataCollectIndex < 9)
            {
                CurrentDataCollectIndex++;
                this.RaisePropertyChanged(nameof(CurrentDataCollectIndex));
            }
        }

        public void DecreaceDataCollectIndex()
        {
            if (CurrentDataCollectIndex > 0)
            {
                CurrentDataCollectIndex--;
                this.RaisePropertyChanged(nameof(CurrentDataCollectIndex));
            }
        }

        public void IncreaceAnnoPhotoIndex()
        {
            if (CurrentAnnoPhotoIndex < 9)
            {
                CurrentAnnoPhotoIndex++;
                this.RaisePropertyChanged(nameof(CurrentAnnoPhotoIndex));
            }
        }

        public void DecreaceAnnoPhotoIndex()
        {
            if (CurrentAnnoPhotoIndex > 0)
            {
                CurrentAnnoPhotoIndex--;
                this.RaisePropertyChanged(nameof(CurrentAnnoPhotoIndex));
            }
        }

        public void IncreaceAnnoLidarIndex()
        {
            if (CurrentAnnoLidarIndex < 9)
            {
                CurrentAnnoLidarIndex++;
                this.RaisePropertyChanged(nameof(CurrentAnnoLidarIndex));
            }
        }
        public void DecreaceAnnoLidarIndex()
        {
            if (CurrentAnnoLidarIndex > 0)
            {
                CurrentAnnoLidarIndex--;
                this.RaisePropertyChanged(nameof(CurrentAnnoLidarIndex));
            }
        }
    }
}
