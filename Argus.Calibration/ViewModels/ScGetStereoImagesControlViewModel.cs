using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Argus.Calibration.Helper;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace Argus.Calibration.ViewModels
{
    public class ScGetStereoImagesControlViewModel : ViewModelBase
    {
        private bool _isBusy;
        private ObservableCollection<string> _stereoImagePairFiles;
        private int _selectedStereoImagePairIndex;
        private List<string> _leftImageFiles;
        private List<string> _rightImageFiles;
        private string _selectedLeftImagePath;
        private string _selectedRightImagePath;

        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }
        
        public ObservableCollection<string> StereoImagePairFiles
        {
            get => _stereoImagePairFiles;
            //set => this.RaiseAndSetIfChanged(ref _stereoImagePairFiles, value);
        }
        
        public int SelectedImagePareIndex
        {
            get => _selectedStereoImagePairIndex;
            set
            {
                _selectedStereoImagePairIndex = value;
                _selectedLeftImagePath = _leftImageFiles[value];
                _selectedRightImagePath = _rightImageFiles[value];
                this.RaisePropertyChanged(nameof(SelectedLeftImage));
                this.RaisePropertyChanged(nameof(SelectedRightImage));
            }
        }

        public Bitmap SelectedLeftImage
        {
            get
            {
                if (_selectedLeftImagePath != null)
                {
                    new Bitmap(_selectedLeftImagePath);
                }

                return null;
            }
        }

        public Bitmap SelectedRightImage
        {
            get
            {
                if (_selectedRightImagePath != null)
                {
                    new Bitmap(_selectedRightImagePath);
                }

                return null;
            }
        }


        public ScGetStereoImagesControlViewModel()
        {
            _leftImageFiles = new List<string>();
            _rightImageFiles = new List<string>();
            _stereoImagePairFiles = new ObservableCollection<string>();
            
            IsBusy = false;
        }

        public void CaptureStereoImages()
        {
            Task.Run(() => 
            {
                IsBusy = true;
            
                string imageBaseDir = CalibConfig.StereoImagesDir;
                string leftImgDir = Path.Combine(imageBaseDir, "left");
                string rightImgDir = Path.Combine(imageBaseDir, "right");

                FsHelper.PurgeDirectory(leftImgDir);
                FsHelper.PurgeDirectory(rightImgDir);
            
                string[] positions = File.ReadAllText(CalibConfig.ArmPositionFile).Split("\n");
                for (int i = 1; i <= positions.Length; i++)
                {
                    // TODO: Change to real script
                    Thread.Sleep(2000);
                    string curDir = System.AppDomain.CurrentDomain.BaseDirectory;
                    string leftSrc = Path.Combine(curDir, "Assets", "left", $"Left{i}.jpg");
                    string leftDest = Path.Combine(curDir, leftImgDir, $"Left{i:D2}.jpg");
                    string rightSrc = Path.Combine(curDir, "Assets", "right", $"Right{i}.jpg");
                    string rightDest = Path.Combine(curDir, rightImgDir, $"Right{i:D2}.jpg");

                    var moveArmTask = $"cp {leftSrc} {leftDest}".Bash();
                    var snapshotTask = $"cp {rightSrc} {rightDest}".Bash();

                    moveArmTask.Wait();
                    snapshotTask.Wait();

                    FileInfo leftFi = new FileInfo($"{leftImgDir}/Left{i:D2}.jpg");
                    FileInfo rightFi = new FileInfo($"{rightImgDir}/Right{i:D2}.jpg");
                
                    string content = $"{i}: {leftFi.Name} <--> {rightFi.Name}";
                    _stereoImagePairFiles.Add(content);

                    _leftImageFiles.Add(leftFi.FullName);
                    _rightImageFiles.Add(rightFi.FullName);

                    SelectedImagePareIndex = i - 1;
                }
            
                IsBusy = false;
            });
        }
    }
}