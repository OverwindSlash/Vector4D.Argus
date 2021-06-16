using Argus.Calibration.Config;
using Argus.Calibration.Helper;
using Avalonia.Media.Imaging;
using OpenCvSharp;
using ReactiveUI;
using RosSharp;
using RosSharp.sensor_msgs;
using RosSharp.Topic;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Argus.Calibration.ViewModels
{
    public class HandEyeCalibrationControlViewModel : ViewModelBase, IDisposable
    {
        private Bitmap _stereoLeftImage;

        private Node? _node;
        private Subscriber<Image>? _subscriber;

        private RobotArms _operationArm;
        private StereoTypes _stereoType;

        private string _message;
        private bool _isInCalibration;

        private bool _isConnectedWithMaster;
        private bool _isTopicAccessable;

        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        public Bitmap LeftImage
        {
            get => _stereoLeftImage;
            set => this.RaiseAndSetIfChanged(ref _stereoLeftImage, value);
        }
        
        public bool IsTopicAccessable
        {
            get => _isTopicAccessable;
            set
            {
                this.RaiseAndSetIfChanged(ref _isTopicAccessable, value);
                this.RaisePropertyChanged(nameof(CanCalibrate));
            }
        }

        public bool IsInCalibration
        {
            get => _isInCalibration;
            set
            {
                this.RaiseAndSetIfChanged(ref _isInCalibration, value);
                this.RaisePropertyChanged(nameof(CanCalibrate));
            }
        }

        public bool CanCalibrate => (IsTopicAccessable && !IsInCalibration);

        public HandEyeCalibrationControlViewModel()
        {
        }

        public void Dispose()
        {
            if (_stereoLeftImage != null)
            {
                _stereoLeftImage.Dispose();
            }

            if (_node != null)
            {
                _node.Dispose();
            }

            // 4. Clean up
            string cleanUpCmd = $"kill_all.sh";
            cleanUpCmd.InvokeRosMasterScript();
        }

        public void SetArm(RobotArms arm)
        {
            _operationArm = arm;
        }

        public void SetStereoTypes(StereoTypes stereoType, MainWindowViewModel mainWindowViewModel)
        {
            _stereoType = stereoType;

            InitRosTopicConnection(mainWindowViewModel);
        }

        private void InitRosTopicConnection(MainWindowViewModel mainWindowViewModel)
        {
            mainWindowViewModel.AddOperationLog("开启双目视频流......");
            if (_stereoType == StereoTypes.BodyStereo)
            {
                string prepareStereoCmd = $"open_lucid_body_stereo.sh";
                //string prepareStereoCmd = $"open_qc_body_stereo.sh";
                prepareStereoCmd.InvokeRosMasterScript();
            }
            else
            {
                bool isLeftArmTool = (int) _stereoType % 2 == 0;
                string toolPrefix = isLeftArmTool ? "left" : "right";

                string ip = CalibConfig.ArmToolsIps[(int) _stereoType];
                //string prepareStereoCmd = $"open_arm_stereo.sh '{ip}' '{toolPrefix}'";
                // TODO: Add various arm tool stereo open scripts.
                string prepareStereoCmd = $"open_right_arm_stereo.sh";
                prepareStereoCmd.InvokeRosMasterScript();
            }

            Thread.Sleep(5000);

            mainWindowViewModel.AddOperationLog("订阅双目相机Topic......");
            try
            {
                IsTopicAccessable = false;
                
                Ros.MasterUri = new Uri(CalibConfig.RosMasterUri);
                Ros.HostName = CalibConfig.HostName;
                Ros.TopicTimeout = CalibConfig.TopicTimeout;
                Ros.XmlRpcTimeout = CalibConfig.XmlRpcTimeout;

                foreach (var node in Ros.GetNodes())
                {
                    node.Dispose();
                }

                _node = Ros.InitNodeAsync(CalibConfig.NodeName).Result;
                _subscriber = _node.SubscriberAsync<RosSharp.sensor_msgs.Image>(CalibConfig.LeftStereoTopic).Result;
                
                _subscriber.Subscribe(x =>
                {
                    if (x.data == null)
                    {
                        return;
                    }

                    int columns = (int) x.width;
                    int rows = (int) x.height;

                    // For image_raw topic.
                    // Mat image = new Mat(rows, columns, MatType.CV_8U, x.data.ToArray());
                    // Mat outImage = new Mat();
                    // Cv2.CvtColor(image, outImage, ColorConversionCodes.BayerRG2RGB);
                    // LeftImage = new Bitmap(outImage.ToMemoryStream());
                    // image.Dispose();
                    // outImage.Dispose();

                    // For tracking_result topic
                    Mat image = new Mat(rows, columns, MatType.CV_8UC3, x.data.ToArray());
                    Mat outImage = new Mat();
                    Cv2.CvtColor(image, outImage, ColorConversionCodes.BGR2RGB);
                    LeftImage = new Bitmap(outImage.ToMemoryStream());
                    image.Dispose();
                    outImage.Dispose();

                    IsTopicAccessable = true;
                });
                
                Message = "请等待左侧机载相机画面开始显示";
            }
            catch (Exception e)
            {
                mainWindowViewModel.AddOperationLog($"错误！{e.Message}");
                Message = $"错误！{e.Message}";
            }
        }

        public async Task CalibrateHandEye(MainWindowViewModel mainWindowVm)
        {
            string handEyeBaseDir = Path.Combine(FsHelper.GetHomeDirectory(), ".ros", "easyhandeye");
            string leftArmCalibFile = Path.Combine(handEyeBaseDir, "ur10_leftarm_eye_on_base.yaml");
            string rightArmCalibFile = Path.Combine(handEyeBaseDir, "ur10_rightarm_eye_on_base.yaml");

            string leftPrepareScript = "calibrate_body_stereo_left_arm_auto.sh";
            string rightPrepareScript = "calibrate_body_stereo_right_arm_auto.sh";
            if (_stereoType != StereoTypes.BodyStereo)
            {
                leftPrepareScript = "calibrate_body_stereo_left_arm_preset.sh";
                rightPrepareScript = "calibrate_body_stereo_right_arm_preset.sh";
            }

            // TODO: Change to real parameters.
            string leftCalibScriptParam = "ur10_leftarm_eye_on_base";
            string rightCalibScriptParam = "ur10_rightarm_eye_on_base";

            //bool isLeftArm = _operationArm == RobotArms.LeftArm;
            bool isLeftArm = (int)_stereoType % 2 == 0;
            string prepareScript = isLeftArm ? leftPrepareScript : rightPrepareScript;
            string calibScriptParam = isLeftArm ? leftCalibScriptParam : rightCalibScriptParam;
            string prefix = isLeftArm ? "左" : "右";
            string calibResultFile = isLeftArm ? leftArmCalibFile : rightArmCalibFile;

            await Task.Run(() =>
            {
                IsInCalibration = true;

                // 1. Prepare robot arm movement environment.
                mainWindowVm.AddOperationLog($"启动Master上的手眼标定配置环境");
                prepareScript.InvokeRosMasterScript();

                Thread.Sleep(30000);

                // 2. Calibrate handeye.
                Message = $"{prefix}臂自动手眼标定中......";

                // TODO: Temp solution for script param pass
                //string calibCmd = $"calibrate_body_stereo_handfree.sh '{calibScriptParam}'";
                string calibCmd = $"calibrate_body_stereo_handfree.sh";
                if (_stereoType != StereoTypes.BodyStereo)
                {
                    // 2.1 Load arm tool preset file.
                    string filepath = Path.Combine(CalibConfig.MovementFileDir,
                        CalibConfig.ArmToolsPresetFiles[(int) _stereoType]);
                    string[] positions = File.ReadAllText(filepath).Split("\n");

                    bool isLeftArmTool = (int) _stereoType % 2 == 0; // Is left or right arm tool
                    string moveToolArmCmd = isLeftArmTool ? "move_leftarm.sh" : "move_rightarm.sh";
                    string moveNonToolArmCmd = isLeftArmTool ? "move_rightarm.sh" : "move_leftarm.sh";

                    // 2.2 Move arm which NOT attach tool to init position.
                    string nonToolPrefix = !isLeftArmTool ? "左" : "右";
                    mainWindowVm.AddOperationLog($"将{nonToolPrefix}臂移动至 {positions[0]}");
                    string moveNonToolArmTask = $"Scripts/{moveNonToolArmCmd} '{positions[0]}'";
                    moveNonToolArmTask.RunSync();

                    // 2.3 Call script to perform preset postion handeye calibration.
                    // TODO: Temp solution for script param pass
                    //calibCmd = $"calibrate_body_stereo_preset_poses.sh {calibScriptParam} temp.txt";
                    calibCmd = $"calibrate_body_stereo_preset_poses.sh";
                }

                mainWindowVm.AddOperationLog(Message);
                mainWindowVm.AddOperationLog($"执行脚本 {calibCmd}");
                calibCmd.InvokeRosMasterScript();
                Message = $"请等待{prefix}臂自动手眼标定完成";
                mainWindowVm.AddOperationLog(Message);

                // 3. Copy calibration result to dest folder.
                string handEyeDestDir = CalibConfig.CalibrationResultDir;
                FsHelper.EnsureDirectoryExist(handEyeDestDir);

                string leftArmCalibDestFile = Path.Combine(handEyeDestDir, "ur10_leftarm_eye_on_base.yaml");
                string rightArmCalibDestFile = Path.Combine(handEyeDestDir, "ur10_rightarm_eye_on_base.yaml");
                string calibResultDestFile = isLeftArm ? leftArmCalibDestFile : rightArmCalibDestFile;

                SimulateGenerateHandeyeResultFile(handEyeBaseDir, leftArmCalibFile, rightArmCalibFile);

                FileInfo calibResultFi = new FileInfo(calibResultFile);
                calibResultFi.CopyTo(calibResultDestFile, true);

                string leftYaml = $"{prefix}臂手眼参数：{calibResultDestFile}";
                mainWindowVm.AddOperationLog(leftYaml);

                IsInCalibration = false;
            });
        }

        private static void SimulateGenerateHandeyeResultFile(string handEyeBaseDir, string leftArmCalibFile,
            string rightArmCalibFile)
        {
            FsHelper.EnsureDirectoryExist(handEyeBaseDir);

            if (!File.Exists(leftArmCalibFile))
            {
                FileInfo mockFi = new FileInfo("Handeye/ur10_leftarm_eye_on_base.yaml");
                mockFi.CopyTo(leftArmCalibFile);
            }

            if (!File.Exists(rightArmCalibFile))
            {
                FileInfo mockFi = new FileInfo("Handeye/ur10_rightarm_eye_on_base.yaml");
                mockFi.CopyTo(rightArmCalibFile);
            }
        }
    }
}