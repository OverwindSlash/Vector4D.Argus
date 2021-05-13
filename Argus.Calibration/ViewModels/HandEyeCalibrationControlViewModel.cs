using Argus.Calibration.Config;
using Argus.Calibration.Helper;
using Avalonia.Media.Imaging;
using OpenCvSharp;
using ReactiveUI;
using RosSharp;
using RosSharp.sensor_msgs;
using RosSharp.Topic;
using System;
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
            
            _node.Dispose();
        }

        public void SetArm(RobotArms arm)
        {
            _operationArm = arm;
        }

        public void CalibrateHandEye()
        {
            if (_operationArm == RobotArms.LeftArm)
            {
                Message = "左臂自动手眼标定中，请耐心等待完成";

                var moveLeftArmTask = Task.Run(() =>
                {
                    IsInCalibration = true;

                    // TODO: Change to real script
                    "Mock/fake_cmd.sh".Bash(() =>
                    {
                        IsInCalibration = false;
                        Message = "左臂自动手眼标定完成";
                    });
                });
            }
            else
            {
                Message = "右臂自动手眼标定中，请耐心等待完成";

                var moveRightArmTask = Task.Run(() =>
                {
                    IsInCalibration = true;

                    // TODO: Change to real script
                    "Mock/fake_cmd.sh".Bash(() =>
                    {
                        IsInCalibration = false;
                        Message = "右臂自动手眼标定完成";
                    });
                });
            }

            
        }
    }
}
