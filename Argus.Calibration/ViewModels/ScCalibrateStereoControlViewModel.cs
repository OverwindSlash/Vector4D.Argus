using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Argus.Calibration.Helper;
using Argus.StereoCalibration;
using Avalonia.Media.Imaging;
using JetBrains.Annotations;
using ReactiveUI;

namespace Argus.Calibration.ViewModels
{
    public class ScCalibrateStereoControlViewModel : ViewModelBase
    {
        [NotNull] private List<string> _stereoImagePairFiles;
        private int _selectedStereoImagePairIndex;
        
        [NotNull] private List<string> _leftImageFiles;
        [NotNull] private List<string> _rightImageFiles;

        [CanBeNull] private string? _selectedLeftImagePath;
        [CanBeNull] private string? _selectedRightImagePath;

        public List<string> LeftImageFiles => _leftImageFiles;
        public List<string> RightImageFiles => _rightImageFiles;

        private bool _isBusy;
        private string _status;
        private string _leftRms;
        private string _rightRms;
        private string _stereoRms;
        private string _leftYaml;
        private string _rightYaml;
        private string _xml;
        private bool _canPerformCalibration;
        private bool _notInCalibration;

        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }
        
        public string Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }

        public string LeftRms
        {
            get => _leftRms;
            set => this.RaiseAndSetIfChanged(ref _leftRms, value);
        }

        public string RightRms
        {
            get => _rightRms;
            set => this.RaiseAndSetIfChanged(ref _rightRms, value);
        }

        public string StereoRms
        {
            get => _stereoRms;
            set => this.RaiseAndSetIfChanged(ref _stereoRms, value);
        }
        
        public string LeftYaml
        {
            get => _leftYaml;
            set => this.RaiseAndSetIfChanged(ref _leftYaml, value);
        }

        public string RightYaml
        {
            get => _rightYaml;
            set => this.RaiseAndSetIfChanged(ref _rightYaml, value);
        }

        public string Xml
        {
            get => _xml;
            set => this.RaiseAndSetIfChanged(ref _xml, value);
        }

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

        public bool CanPerformCalibration
        {
            get => _canPerformCalibration;
            set => this.RaiseAndSetIfChanged(ref _canPerformCalibration, value);
        }

        public bool NotInCalibration
        {
            get => _notInCalibration;
            set => this.RaiseAndSetIfChanged(ref _notInCalibration, value);
        }

        public ScCalibrateStereoControlViewModel()
        {
            CanPerformCalibration = false;
            NotInCalibration = true;

            LeftRms = "左目图像";
            RightRms = "右目图像";
            StereoRms = string.Empty;
            LeftYaml = string.Empty;
            LeftYaml = "_左目内参Yaml："; 
            RightYaml = "_右目内参Yaml：";
            Xml = "识别用Xml：";
        }
        
        public async void CaptureStereoImages()
        {
            string imageBaseDir = CalibConfig.StereoImagesDir;
            string leftImgDir = Path.Combine(imageBaseDir, "left");
            string rightImgDir = Path.Combine(imageBaseDir, "right");

            FsHelper.PurgeDirectory(leftImgDir);
            FsHelper.PurgeDirectory(rightImgDir);

            var bash1Task = $"cp Assets/left/* {leftImgDir}".Bash();
            var bash2Task = $"cp Assets/right/* {rightImgDir}".Bash();

            bash1Task.Wait();
            bash2Task.Wait();

            _leftImageFiles = FsHelper.GetImageFilesInFolder(leftImgDir);
            _rightImageFiles = FsHelper.GetImageFilesInFolder(rightImgDir);

            _stereoImagePairFiles = new List<string>();
            for (int i = 0; i < _leftImageFiles.Count; i++)
            {
                FileInfo leftFi = new FileInfo(_leftImageFiles[i]);
                FileInfo rightFi = new FileInfo(_rightImageFiles[i]);
                
                string content = $"{i}: {leftFi.Name} <--> {rightFi.Name}";
                _stereoImagePairFiles.Add(content);
            }
            this.RaisePropertyChanged(nameof(StereoImageFiles));

            SelectedImagePareIndex = 0;

            if (_leftImageFiles.Count > 10)
            {
                CanPerformCalibration = true;
            }
        }

        public async void CalibrateStereo()
        {
            IsBusy = true;
            Status = "双目标定中，请稍候……";
            LeftRms = "左目图像";
            RightRms = "右目图像";
            LeftYaml = "_左目内参Yaml："; 
            RightYaml = "_右目内参Yaml：";
            Xml = "识别用Xml：";

            CanPerformCalibration = false;
            NotInCalibration = false;

            await Task.Run(() =>
            {
                var result = CameraCalibrator.CalibrateStereoCamera(_leftImageFiles, _rightImageFiles);
                LeftRms = $"左目图像 重投影误差：{result.LeftRms}";
                RightRms = $"右目图像 重投影误差：{result.RightRms}";
                Status = $"双目标定完毕 重投影误差：{result.StereoRms}";

                string leftYamlFile = CameraCalibrator.GenerateYamlFile(CalibConfig.YamlFileDir, $"{CalibConfig.BodyCameraName}_left", result.ImageSize,
                    result.LeftCameraMatrix, result.LeftDistCoeffs, result.R1, result.P1);

                string rightYamlFile = CameraCalibrator.GenerateYamlFile(CalibConfig.YamlFileDir, $"{CalibConfig.BodyCameraName}_right", result.ImageSize,
                    result.RightCameraMatrix, result.RightDistCoeffs, result.R2, result.P2);

                string xmlFile = CameraCalibrator.GenerateXmlFile(CalibConfig.XmlFileDir, $"{CalibConfig.BodyCameraName}",
                    result.LeftCameraMatrix, result.LeftDistCoeffs, result.RightCameraMatrix, result.RightDistCoeffs,
                    result.Rotation, result.Translation, result.Essential, result.Fundamental);

                FileInfo leftYamlFi = new FileInfo(leftYamlFile);
                FileInfo rightYamlFi = new FileInfo(rightYamlFile);
                FileInfo xmlFi = new FileInfo(xmlFile);

                LeftYaml = $"_左目内参Yaml：{leftYamlFi.FullName}"; 
                RightYaml = $"_右目内参Yaml：{rightYamlFi.FullName}";
                Xml = $"识别用Xml：{xmlFi.FullName}";
            });
            
            IsBusy = false;
            NotInCalibration = true;
            CanPerformCalibration = true;
        }
    }
}