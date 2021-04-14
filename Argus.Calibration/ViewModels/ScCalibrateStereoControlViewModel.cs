using System.Collections.Generic;
using System.IO;
using System.Linq;
using Argus.StereoCalibration;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using JetBrains.Annotations;
using ReactiveUI;

namespace Argus.Calibration.ViewModels
{
    public class ScCalibrateStereoControlViewModel : ReactiveObject
    {
        private static string _imageBaseDir = "StereoImages";
        
        [NotNull] private List<string> _stereoImagePairFiles;
        private int _selectedStereoImagePairIndex;
        
        [NotNull] private readonly List<string> _leftImageFiles;
        [NotNull] private readonly List<string> _rightImageFiles;

        [CanBeNull] private string? _selectedLeftImagePath;
        [CanBeNull] private string? _selectedRightImagePath;
        
        public List<string> LeftImageFiles => _leftImageFiles;
        public List<string> RightImageFiles => _rightImageFiles;

        public List<string> StereoImageFiles
        {
            get => _stereoImagePairFiles;
        }

        public int SelectedImagePareIndex
        {
            get => _selectedStereoImagePairIndex;
            set
            {
                _selectedStereoImagePairIndex = value;
                _selectedLeftImagePath = LeftImageFiles[value];
                _selectedRightImagePath = RightImageFiles[value];
                this.RaisePropertyChanged(nameof(SelectedLeftImage));
                this.RaisePropertyChanged(nameof(SelectedRightImage));
            }
        }

        public Bitmap SelectedLeftImage => new Bitmap(_selectedLeftImagePath);
        public Bitmap SelectedRightImage => new Bitmap(_selectedRightImagePath);

        public ScCalibrateStereoControlViewModel()
        {
            string leftImgDir = Path.Combine(_imageBaseDir, "left");
            string rightImgDir = Path.Combine(_imageBaseDir, "right");
            
            var leftFis = StereoCalibrator.GetImageFilesInFolder(leftImgDir);
            var rightFis = StereoCalibrator.GetImageFilesInFolder(rightImgDir);

            _leftImageFiles = new List<string>();
            var leftFiles = leftFis.Select(fi => fi.FullName);
            LeftImageFiles.AddRange(leftFiles);

            _rightImageFiles = new List<string>();
            var rightFiles = rightFis.Select(fi => fi.FullName);
            RightImageFiles.AddRange(rightFiles);

            _stereoImagePairFiles = new List<string>();
            for (int i = 0; i < leftFis.Count; i++)
            {
                string content = $"{i}: {leftFis[i].Name} <--> {rightFis[i].Name}";
                _stereoImagePairFiles.Add(content);
            }
            this.RaisePropertyChanged(nameof(StereoImageFiles));

            SelectedImagePareIndex = 0;
        }
    }
}