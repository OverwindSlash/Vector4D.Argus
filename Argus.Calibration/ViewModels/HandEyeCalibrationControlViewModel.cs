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
using System.Linq;
using System.Threading.Tasks;

namespace Argus.Calibration.ViewModels
{
    public class HandEyeCalibrationControlViewModel : ViewModelBase, IDisposable
    {
        private Bitmap _stereoLeftImage;
        
        private Node? _node;
        private Subscriber<Image>? _subscriber;

        private RobotArms _operationArm;

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

        public HandEyeCalibrationControlViewModel()
        {
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

                Mat image = new Mat(rows, columns, MatType.CV_8UC3, x.data.Skip(4).ToArray());
                LeftImage = new Bitmap(image.ToMemoryStream());
                image.Dispose();
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
        }

        public void SetArm(RobotArms arm)
        {
            _operationArm = arm;
        }

        public async Task CalibrateHandEye(MainWindowViewModel mainWindowVm)
        {
            string handEyeBaseDir = @"~/.ros/easyhandeye";
            string handEyeDestDir = @"CalibResult";


            string leftArmCalibFile = Path.Combine(handEyeBaseDir, "ur10_leftarm_eye_on_base.yaml");
            string rightArmCalibFile = Path.Combine(handEyeBaseDir, "ur10_rightarm_eye_on_base.yaml");

            string leftPrepareScript = "calibrate_lucid_body_stereo_left_arm.sh";
            string rightPrepareScript = "calibrate_lucid_body_stereo_right_arm.sh";

            string leftCalibScriptParam = "left param";
            string rightCalibScriptParam = "right param";


            string prepareScript = leftPrepareScript;
            string calibScriptParam = leftCalibScriptParam;
            string prefix = "左";
            if (_operationArm == RobotArms.RightArm)
            {
                prepareScript = rightPrepareScript;
                calibScriptParam = rightCalibScriptParam;
                prefix = "右";
            }


            Message = "手眼标定环境配置中......";

            await Task.Run(() =>
            {
                IsInCalibration = true;

                // TODO: Change to real script
                // calibrate_lucid_body_stereo_left_arm.sh
                prepareScript.RunSync();
                Message = $"{prefix}臂自动手眼标定中......";
                mainWindowVm.AddOperationLog($"执行脚本 {prepareScript}");

                // TODO: Change to real script
                // calibrate_body_stereo_handfree.sh
                string calibCmd = $"Mock/fake_cmd.sh {calibScriptParam}";
                calibCmd.RunSync();
                Message = $"{prefix}臂自动手眼标定完成";
                mainWindowVm.AddOperationLog($"执行脚本 {calibCmd}");

                // TODO: Change to real script
                // Copy calibration result to dest folder.
                "Mock/fake_cmd.sh".RunSync();
                FileInfo leftArmCalibFi = new FileInfo(leftArmCalibFile);
                string leftYaml = $"{prefix}臂手眼参数：{leftArmCalibFi.FullName}";

                mainWindowVm.AddOperationLog(leftYaml);

                IsInCalibration = false;
            });
        }
    }
}
