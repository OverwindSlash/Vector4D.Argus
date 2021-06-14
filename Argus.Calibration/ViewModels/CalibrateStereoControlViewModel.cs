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
using Argus.Calibration.Config;

namespace Argus.Calibration.ViewModels
{
    public class CalibrateStereoControlViewModel : ViewModelBase
    {
        private ObservableCollection<string> _stereoImagePairFiles;
        private int _selectedStereoImagePairIndex;
        private List<string> _leftImageFiles;
        private List<string> _rightImageFiles;
        private string _selectedLeftImagePath;
        private string _selectedRightImagePath;

        private StereoTypes _stereoType;

        private bool _isInCapture;
        private bool _isInCalibration;
        private bool _isBusy;
        private bool _canCapture;
        private bool _canCalibrate;

        private bool _imagesCaptured;

        private bool _userCancelled;

        private bool _isStereoCalibrated;
        private string _leftRms;
        private string _rightRms;
        private string _stereoRms;

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

        public CalibrateStereoControlViewModel(StereoTypes stereoType)
        {
            _leftImageFiles = new List<string>();
            _rightImageFiles = new List<string>();
            _stereoImagePairFiles = new ObservableCollection<string>();

            _stereoType = stereoType;

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

                // 0. Prepare robot arm movement environment.
                await Task.Run(() =>
                {
                    mainWindowVm.AddOperationLog($"启动Master上的机械臂及转台控制节点");
                    string initArmCmd = $"init_arm_turntable_move.sh";
                    initArmCmd.InvokeRosMasterScript();
                });

                // 1. Clean up
                _stereoImagePairFiles.Clear();

                string imageBaseDir = CalibConfig.StereoImagesDir;
                FsHelper.EnsureDirectoryExist(imageBaseDir);
                FsHelper.PurgeDirectory(imageBaseDir);

                string leftImgDir = Path.Combine(imageBaseDir, "left");
                string rightImgDir = Path.Combine(imageBaseDir, "right");

                // 2. Move arm to take snapshot
                bool isBodyStereo = (_stereoType == StereoTypes.BodyStereo);
                string filepath = isBodyStereo ? Path.Combine(CalibConfig.MovementFileDir, CalibConfig.BodyStereoArmPositionFile)   // Body Stereo
                        : Path.Combine(CalibConfig.MovementFileDir, CalibConfig.ArmToolsPositionFiles[(int)_stereoType]);           // Arm Tool Stereo
                string[] positions = File.ReadAllText(filepath).Split("\n");

                // 2.1 If not body stereo, then first move arm which attach tool.
                int startIndex = 0;

                bool isLeftArmTool = (int)_stereoType % 2 == 0;     // Is left or right arm tool
                string moveToolArmCmd = isLeftArmTool ? "move_leftarm.sh" : "move_rightarm.sh";
                string moveNonToolArmCmd = isLeftArmTool ? "move_rightarm.sh" : "move_leftarm.sh";

                if (!isBodyStereo)
                {
                    await Task.Run(() =>
                    {
                        string toolPrefix = isLeftArmTool ? "左" : "右";
                        mainWindowVm.AddOperationLog($"将{toolPrefix}臂移动至 {positions[0]}");
                        string moveToolArmTask = $"Scripts/{moveToolArmCmd} '{positions[0]}'";
                        moveToolArmTask.RunSync();
                    });

                    startIndex = 1;
                }

                // 2.2 Move arm to snapshot position.
                int imageNo = 1;
                for (int i = startIndex; i < positions.Length; i++, imageNo++)
                {
                    if (_userCancelled)
                    {
                        break;
                    }

                    // 1. Move robot arms to snapshot positions.
                    if (isBodyStereo)
                    {
                        // 1.1 Body stereo check only need move left arm to initial position
                        await Task.Run(() =>
                        {
                            mainWindowVm.AddOperationLog($"将左臂移动至位置{imageNo}：{positions[i]}");
                            string moveLeftCmd = $"Scripts/move_leftarm.sh '{positions[i]}'";
                            moveLeftCmd.RunSync();
                        });

                        // 2.2 take snap shot
                        // TODO: Temp solution for qc stereo
                        //string snapshotCmd = $"Scripts/snapshot_body.sh '{imageBaseDir}'";
                        string snapshotCmd = $"Scripts/snapshot_qc_body.sh '192.168.1.101' '192.168.1.102' '{imageBaseDir}'";
                        snapshotCmd.RunSync();
                    }
                    else
                    {
                        // Move left and right arm to initial position
                        await Task.Run(() =>
                        {
                            string nonToolPrefix = isLeftArmTool ? "右" : "左";
                            mainWindowVm.AddOperationLog($"将{nonToolPrefix}臂移动至位置{imageNo}： {positions[i]}");
                            string moveToolArmTask = $"Scripts/{moveNonToolArmCmd} '{positions[i]}'";
                            moveToolArmTask.RunSync();
                        });

                        // 2.2 take snap shot
                        string ip = CalibConfig.ArmToolsIps[(int)_stereoType];
                        string snapshotCmd = $"Scripts/snapshot_arm.sh '{ip}' '{imageBaseDir}'";
                        snapshotCmd.RunSync();
                    }                    

                    await SimulateSnapShotAsync(imageNo, leftImgDir, rightImgDir);

                    string leftDest = FsHelper.GetLastFileByNameFromDirectory(leftImgDir, "left");
                    string rightDest = FsHelper.GetLastFileByNameFromDirectory(rightImgDir, "right");
                    FileInfo leftFi = new FileInfo(leftDest);
                    FileInfo rightFi = new FileInfo(rightDest);

                    mainWindowVm.AddOperationLog($"{imageNo:D2} 左目图像：{leftFi.FullName}");
                    mainWindowVm.AddOperationLog($"{imageNo:D2} 右目图像：{rightFi.FullName}");

                    string content = $"{imageNo}: {leftFi.Name} <--> {rightFi.Name}";
                    _stereoImagePairFiles.Add(content);

                    _leftImageFiles.Add(leftFi.FullName);
                    _rightImageFiles.Add(rightFi.FullName);

                    SelectedImagePareIndex = _stereoImagePairFiles.Count - 1;
                }

                ImagesCaptured = true;
                IsInCapture = false;

                mainWindowVm.AddOperationLog("完成双目图片获取");

                // 3. Clean up robot arm movement environment.
                await Task.Run(() =>
                {
                    mainWindowVm.AddOperationLog($"关闭Master上的机械臂及转台控制节点");
                    string unInitArmCmd = $"kill_all.sh";
                    unInitArmCmd.InvokeRosMasterScript();
                });
            });
        }

        private static async Task SimulateSnapShotAsync(int imageNo, string leftImgDir, string rightImgDir)
        {
            string curDir = System.AppDomain.CurrentDomain.BaseDirectory;
            string leftSrc = Path.Combine(curDir, "Images", "left", $"Left{imageNo}.jpg");
            string leftDest = Path.Combine(curDir, leftImgDir, $"Left{imageNo:D2}.jpg");
            string rightSrc = Path.Combine(curDir, "Images", "right", $"Right{imageNo}.jpg");
            string rightDest = Path.Combine(curDir, rightImgDir, $"Right{imageNo:D2}.jpg");

            FsHelper.EnsureDirectoryExist(Path.Combine(curDir, leftImgDir));
            FsHelper.EnsureDirectoryExist(Path.Combine(curDir, rightImgDir));

            await Task.Run(() =>
            {
                FileInfo srcLeftFi = new FileInfo(leftSrc);
                FileInfo srcRightFi = new FileInfo(rightSrc);

                srcLeftFi.CopyTo(leftDest);
                srcRightFi.CopyTo(rightDest);
            });
        }

        public void CancelOperation()
        {
            this._userCancelled = true;

            Thread.Sleep(5000);

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
                LeftRms = $"左目图像 重投影误差：{result.LeftRms}";
                RightRms = $"右目图像 重投影误差：{result.RightRms}";
                StereoRms = $"双目标定完毕 重投影误差：{result.StereoRms}";

                mainWindowVm.AddOperationLog(LeftRms);
                mainWindowVm.AddOperationLog(RightRms);
                mainWindowVm.AddOperationLog(StereoRms);

                // Generate result files.
                FsHelper.EnsureDirectoryExist(CalibConfig.CalibrationResultDir);

                string stereoName = CalibConfig.BodyStereoName;
                if  (_stereoType != StereoTypes.BodyStereo)
                {
                    stereoName = CalibConfig.ArmToolsNames[(int)_stereoType];
                }

                string leftYamlFile = CameraCalibrator.GenerateYamlFile(CalibConfig.CalibrationResultDir, $"{stereoName}_left", result.ImageSize,
                    result.LeftCameraMatrix, result.LeftDistCoeffs, result.R1, result.P1);

                string rightYamlFile = CameraCalibrator.GenerateYamlFile(CalibConfig.CalibrationResultDir, $"{stereoName}_right", result.ImageSize,
                    result.RightCameraMatrix, result.RightDistCoeffs, result.R2, result.P2);

                string xmlFile = CameraCalibrator.GenerateXmlFile(CalibConfig.CalibrationResultDir, $"{stereoName}",
                    result.LeftCameraMatrix, result.LeftDistCoeffs, result.RightCameraMatrix, result.RightDistCoeffs,
                    result.Rotation, result.Translation, result.Essential, result.Fundamental);
                var f = CameraCalibrator.GenerateXmlFile(CalibConfig.CalibrationResultDir, $"{stereoName}_all",
                    result.LeftCameraMatrix, result.LeftDistCoeffs, result.RightCameraMatrix, result.RightDistCoeffs,
                    result.Rotation, result.Translation, result.Essential, result.Fundamental, result.R1, result.R2, result.P1, result.P2);

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
    }
}