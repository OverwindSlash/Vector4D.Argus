using Argus.Calibration.Config;
using Argus.Calibration.Helper;
using Avalonia.Media.Imaging;
using OpenCvSharp;
using ReactiveUI;
using RosSharp;
using RosSharp.sensor_msgs;
using RosSharp.Topic;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private StereoTypes _stereoTypes;

        private string _message;
        private bool _isInCalibration;

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

        public bool IsInCalibration
        {
            get => _isInCalibration;
            set => this.RaiseAndSetIfChanged(ref _isInCalibration, value);
        }

        private Process _openLucidStereo;

        public HandEyeCalibrationControlViewModel()
        {
            // string openLucidStereo = $"Scripts/open_lucid_stereo.sh";
            // Task<Process> task = openLucidStereo.BashCancellable();

            // _openLucidStereo = task.Result;

            Ros.MasterUri = new Uri(CalibConfig.RosMasterUri);
            Ros.HostName = CalibConfig.HostName;
            Ros.TopicTimeout = CalibConfig.TopicTimeout;
            Ros.XmlRpcTimeout = CalibConfig.XmlRpcTimeout;

            _node = Ros.InitNodeAsync(CalibConfig.NodeName).Result;
            _subscriber = _node.SubscriberAsync<RosSharp.sensor_msgs.Image>(CalibConfig.LeftStereoTopic).Result;
            _subscriber.Subscribe(x =>
            {
                int columns = (int)x.width;
                int rows = (int)x.height;

                try
                {
                    Mat image = new Mat(rows, columns, MatType.CV_8U, x.data.ToArray());
                    Mat outImage = new Mat();
                    Cv2.CvtColor(image, outImage, ColorConversionCodes.BayerRG2RGB);
                    LeftImage = new Bitmap(outImage.ToMemoryStream());
                    image.Dispose();
                    outImage.Dispose();
                }
                catch (Exception e)
                {

                }
            });

            Message = "请等待左侧机载相机画面开始显示";
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

            if (_openLucidStereo != null)
            {
                _openLucidStereo.Kill();
            }
        }

        public void SetArm(RobotArms arm)
        {
            _operationArm = arm;
        }

        public void SetStereoTypes(StereoTypes stereoType)
        {
            _stereoTypes = stereoType;
        }

        public async Task CalibrateHandEye(MainWindowViewModel mainWindowVm)
        {
            string handEyeBaseDir = Path.Combine(FsHelper.GetHomeDirectory(), ".ros", "easyhandeye");
            string leftArmCalibFile = Path.Combine(handEyeBaseDir, "ur10_leftarm_eye_on_base.yaml");
            string rightArmCalibFile = Path.Combine(handEyeBaseDir, "ur10_rightarm_eye_on_base.yaml");

            string leftPrepareScript = "Scripts/calibrate_eob_leftarm.sh";
            string rightPrepareScript = "Scripts/calibrate_eob_rightarm.sh";

            // TODO: Change to real parameters.
            string leftCalibScriptParam = "namespace_prefix:=/ur10_leftarm_eye_on_base.yaml";
            string rightCalibScriptParam = "namespace_prefix:=/ur10_rightarm_eye_on_base.yaml";

            bool isLeftArm = _operationArm == RobotArms.LeftArm;
            string prepareScript = isLeftArm ? leftPrepareScript : rightPrepareScript;
            string calibScriptParam = isLeftArm ? leftCalibScriptParam : rightCalibScriptParam;
            string prefix = isLeftArm ? "左" : "右";
            string calibResultFile = isLeftArm ? leftArmCalibFile : rightArmCalibFile;

            await Task.Run(() =>
            {
                IsInCalibration = true;

                // 1. Prepare handeye infrastructure.
                Message = "手眼标定环境配置中......";
                mainWindowVm.AddOperationLog(Message);
                mainWindowVm.AddOperationLog($"执行脚本 {prepareScript}");
                prepareScript.RunSync();

                // 2. Calibrate handeye.
                Message = $"{prefix}臂自动手眼标定中......";
                string calibCmd = $"Scripts/calibrate_eob_handfree.sh {calibScriptParam}";
                mainWindowVm.AddOperationLog(Message);
                mainWindowVm.AddOperationLog($"执行脚本 {calibCmd}");
                calibCmd.RunSync();
                Message = $"{prefix}臂自动手眼标定完成";
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

        private static void SimulateGenerateHandeyeResultFile(string handEyeBaseDir, string leftArmCalibFile, string rightArmCalibFile)
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
