using Argus.Calibration.Helper;
using Argus.StereoCalibration;
using Avalonia.Media.Imaging;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Argus.Calibration.ViewModels
{
    public class ScCalibrateStereoControlViewModel : ViewModelBase
    {
        private ObservableCollection<string> _stereoImagePairFiles;
        private int _selectedStereoImagePairIndex;
        private List<string> _leftImageFiles;
        private List<string> _rightImageFiles;
        private string _selectedLeftImagePath;
        private string _selectedRightImagePath;

        private bool _isInCapture;
        private bool _isInCalibration;
        private bool _isBusy;
        private bool _canCapture;
        private bool _canCalibrate;

        private bool _imagesCaptured;

        private bool _userCancelled;

        private bool _isStereoCalibrated;

        public bool IsInCapture
        {
            get => _isInCapture;
            set
            {
                this.RaiseAndSetIfChanged(ref _isInCapture, value);
                IsBusy = value;
                CanCapture = value;
                CanCalibrate = value;
            }
        }

        public bool IsInCalibration
        {
            get => _isInCalibration;
            set
            {
                this.RaiseAndSetIfChanged(ref _isInCalibration, value);
                IsBusy = value;
                CanCapture = value;
                CanCalibrate = value;
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = _isInCapture || _isInCalibration;
                this.RaisePropertyChanged(nameof(IsBusy));
            }
        }

        public bool CanCapture
        {
            get => _canCapture;
            set
            {
                _canCapture = !_isInCapture && !_isInCalibration;
                this.RaisePropertyChanged(nameof(CanCapture));
            }
        }

        public bool CanCalibrate
        {
            get => _canCalibrate;
            set
            {
                _canCalibrate = _imagesCaptured && !_isInCalibration;
                this.RaisePropertyChanged(nameof(CanCalibrate));
            }
        }

        public bool IsStereoCalibrated
        {
            get => _isStereoCalibrated;
            set => this.RaiseAndSetIfChanged(ref _isStereoCalibrated, value);
        }

        public bool ImagesCaptured
        {
            get => _imagesCaptured;
            set => this.RaiseAndSetIfChanged(ref _imagesCaptured, value);
        }

        public ObservableCollection<string> StereoImagePairFiles
        {
            get => _stereoImagePairFiles;
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
                    return new Bitmap(_selectedLeftImagePath);
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
                    return new Bitmap(_selectedRightImagePath);
                }

                return null;
            }
        }


        public ScCalibrateStereoControlViewModel()
        {
            _leftImageFiles = new List<string>();
            _rightImageFiles = new List<string>();
            _stereoImagePairFiles = new ObservableCollection<string>();
            
            IsInCapture = false;
            IsInCalibration = false;
            ImagesCaptured = false;
            CanCapture = true;
            CanCalibrate = false;
        }

        public async Task CaptureStereoImages(MainWindowViewModel mainWindowVm)
        {
            await Task.Run(async () => 
            {
                IsInCapture = true;
                ImagesCaptured = false;

                _stereoImagePairFiles.Clear();

                string imageBaseDir = CalibConfig.StereoImagesDir;
                if (!Directory.Exists(imageBaseDir))
                {
                    Directory.CreateDirectory(imageBaseDir);
                }

                string leftImgDir = Path.Combine(imageBaseDir, "left");
                string rightImgDir = Path.Combine(imageBaseDir, "right");

                FsHelper.PurgeDirectory(leftImgDir);
                FsHelper.PurgeDirectory(rightImgDir);

                string filepath = Path.Combine(CalibConfig.MovementFileDir, CalibConfig.BodyStereoArmPositionFile);
                string[] positions = File.ReadAllText(filepath).Split("\n");
                for (int i = 1; i <= positions.Length; i++)
                {
                    if (_userCancelled)
                    {
                        break;
                    }

                    mainWindowVm.AddOperationLog($"将左臂移动至 {positions[0]}");
                    string moveLeftCmd = $"Scripts/move_leftarm.sh '{positions[0]}'";
                    moveLeftCmd.RunSync();


                    // TODO: Change to real script
                    "Mock/fake_cmd.sh".RunSync();

                    string curDir = System.AppDomain.CurrentDomain.BaseDirectory;
                    string leftSrc = Path.Combine(curDir, "Images", "left", $"Left{i}.jpg");
                    string leftDest = Path.Combine(curDir, leftImgDir, $"Left{i:D2}.jpg");
                    string rightSrc = Path.Combine(curDir, "Images", "right", $"Right{i}.jpg");
                    string rightDest = Path.Combine(curDir, rightImgDir, $"Right{i:D2}.jpg");
                    await SimulateSnapShotAsync(leftSrc, leftDest, rightSrc, rightDest);

                    FileInfo leftFi = new FileInfo(leftDest);
                    FileInfo rightFi = new FileInfo(rightDest);

                    mainWindowVm.AddOperationLog($"{i:D2} 左目图像：{leftFi.FullName}");
                    mainWindowVm.AddOperationLog($"{i:D2} 右目图像：{rightFi.FullName}");

                    string content = $"{i}: {leftFi.Name} <--> {rightFi.Name}";
                    _stereoImagePairFiles.Add(content);

                    _leftImageFiles.Add(leftFi.FullName);
                    _rightImageFiles.Add(rightFi.FullName);

                    SelectedImagePareIndex = i - 1;
                }

                ImagesCaptured = true;
                IsInCapture = false;

                mainWindowVm.AddOperationLog("完成双目图片获取");
            });
        }

        private static async Task SimulateSnapShotAsync(string leftSrc, string leftDest, string rightSrc, string rightDest)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                await Task.Run(() =>
                {
                    $"cp -r {leftSrc} {leftDest}".RunSync();
                    $"cp -r {rightSrc} {rightDest}".RunSync();
                });
            }
            else
            {
                FileInfo srcLeftFi = new FileInfo(leftSrc);
                FileInfo srcRightFi = new FileInfo(rightSrc);

                srcLeftFi.CopyTo(leftDest);
                srcRightFi.CopyTo(rightDest);
            }
        }

        public void CancelOperation()
        {
            this._userCancelled = true;

            Thread.Sleep(1000);

            this._userCancelled = false;
        }

        public void CalibrateStereo(MainWindowViewModel mainWindowVm)
        {
            Task.Run(() =>
            {
                IsInCalibration = true;
                IsStereoCalibrated = false;

                CameraCalibrator.SetLogCallback(mainWindowVm.AddOperationLog);

                var result = CameraCalibrator.CalibrateStereoCamera(_leftImageFiles, _rightImageFiles);
                string leftRms = $"左目图像 重投影误差：{result.LeftRms}";
                string rightRms = $"右目图像 重投影误差：{result.RightRms}";
                string stereoRms = $"双目标定完毕 重投影误差：{result.StereoRms}";

                mainWindowVm.AddOperationLog(leftRms);
                mainWindowVm.AddOperationLog(rightRms);
                mainWindowVm.AddOperationLog(stereoRms);

                string leftYamlFile = CameraCalibrator.GenerateYamlFile(CalibConfig.YamlFileDir, $"{CalibConfig.BodyStereoName}_left", result.ImageSize,
                    result.LeftCameraMatrix, result.LeftDistCoeffs, result.R1, result.P1);

                string rightYamlFile = CameraCalibrator.GenerateYamlFile(CalibConfig.YamlFileDir, $"{CalibConfig.BodyStereoName}_right", result.ImageSize,
                    result.RightCameraMatrix, result.RightDistCoeffs, result.R2, result.P2);

                string xmlFile = CameraCalibrator.GenerateXmlFile(CalibConfig.XmlFileDir, $"{CalibConfig.BodyStereoName}",
                    result.LeftCameraMatrix, result.LeftDistCoeffs, result.RightCameraMatrix, result.RightDistCoeffs,
                    result.Rotation, result.Translation, result.Essential, result.Fundamental);

                FileInfo leftYamlFi = new FileInfo(leftYamlFile);
                FileInfo rightYamlFi = new FileInfo(rightYamlFile);
                FileInfo xmlFi = new FileInfo(xmlFile);

                string leftYaml = $"左目内参Yaml：{leftYamlFi.FullName}";
                string rightYaml = $"右目内参Yaml：{rightYamlFi.FullName}";
                string xml = $"识别用Xml：{xmlFi.FullName}";

                mainWindowVm.AddOperationLog(leftYaml);
                mainWindowVm.AddOperationLog(rightYaml);
                mainWindowVm.AddOperationLog(xml);

                mainWindowVm.AddOperationLog("完成双目标定");

                IsInCalibration = false;
                IsStereoCalibrated = true;

                mainWindowVm.StereoCalibrated = true;
            });
        }

        public void ShowStereoCalibrationResult()
        {
            throw new System.NotImplementedException();
        }
    }
}