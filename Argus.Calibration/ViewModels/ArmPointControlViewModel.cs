using System;
using System.Collections.ObjectModel;
using System.Threading;
using Argus.Calibration.Config;
using Argus.Calibration.Helper;
using Argus.StereoCalibration;
using Avalonia.Media.Imaging;
using DynamicData;
using OpenCvSharp;
using ReactiveUI;
using RosSharp;
using RosSharp.sensor_msgs;
using RosSharp.Topic;

namespace Argus.Calibration.ViewModels
{
    public class ArmPointControlViewModel : ViewModelBase
    {
        private StereoTypes _stereoType;
        private string _stereoName;
        
        private Node? _node;
        private Subscriber<Image>? _leftStereoSubscriber;
        private Subscriber<Image>? _rightStereoSubscriber;
        
        private string _sourceLink;
        private string _destLink;
        
        private RobotArms _operationArm;
        
        private bool _isTopicAccessable;

        public bool LeftCornersFound { get; set; }
        public int LeftCornersCount { get; set; }

        public bool RightCornersFound { get; set; }
        public int RightCornersCount { get; set; }

        public ObservableCollection<int> CornerIndexes { get; set; }
        
        public bool IsTopicAccessable
        {
            get => _isTopicAccessable;
            set
            {
                this.RaiseAndSetIfChanged(ref _isTopicAccessable, value);
            }
        }
        
        private Bitmap _stereoLeftImage;
        public Bitmap LeftImage
        {
            get => _stereoLeftImage;
            set => this.RaiseAndSetIfChanged(ref _stereoLeftImage, value);
        }
        
        private Bitmap _stereoRightImage;
        public Bitmap RightImage
        {
            get => _stereoRightImage;
            set => this.RaiseAndSetIfChanged(ref _stereoRightImage, value);
        }
        
        public void SetStereoTypes(StereoTypes stereoType)
        {
            _stereoType = stereoType;
            
            _stereoName = CalibConfig.BodyStereoName;
            if  (_stereoType != StereoTypes.BodyStereo)
            {
                _stereoName = CalibConfig.ArmToolsNames[(int)_stereoType];
            }
        }

        // public void SetLinks(string sourceLink, string destLink)
        // {
        //     _sourceLink = sourceLink;
        //     _destLink = destLink;
        // }

        public void SetArm(RobotArms robotArms)
        {
            _operationArm = robotArms;
        }
        
        public void Init(MainWindowViewModel mainWindowViewModel)
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

            CornerIndexes = new  ObservableCollection<int>();
        }
        
        public void MoveTurntable(int x1, int y1)
        {
            string initTurntableCmd = $"init_arm_turntable_move.sh";
            initTurntableCmd.InvokeRosMasterScript();
        }
        
        public void OpenStereoStream(MainWindowViewModel mainWindowViewModel)
        {
            mainWindowViewModel.AddOperationLog("订阅双目相机Topic......");
            try
            {
                IsTopicAccessable = false;

                CloseAllStereoSteam();
                
                Ros.MasterUri = new Uri(CalibConfig.RosMasterUri);
                Ros.HostName = CalibConfig.HostName;
                Ros.TopicTimeout = CalibConfig.TopicTimeout;
                Ros.XmlRpcTimeout = CalibConfig.XmlRpcTimeout;

                _node = Ros.InitNodeAsync(CalibConfig.NodeName).Result;
            }
            catch (Exception e)
            {
                mainWindowViewModel.AddOperationLog($"错误！{e.Message}");
            }
            
            mainWindowViewModel.AddOperationLog("请等待双目相机画面开始显示......");
            
            _leftStereoSubscriber = _node.SubscriberAsync<Image>(@"/body_left/image_raw").Result;
            _rightStereoSubscriber = _node.SubscriberAsync<Image>(@"/body_right/image_raw").Result;
                
            _leftStereoSubscriber.Subscribe(x =>
            {
                if (x.data == null)
                {
                    return;
                }

                int columns = (int) x.width;
                int rows = (int) x.height;

                // For image_raw topic.
                Mat image = new Mat(rows, columns, MatType.CV_8U, x.data.ToArray());
                Mat outImage = new Mat();
                Cv2.CvtColor(image, outImage, ColorConversionCodes.BayerRG2RGB);

                (bool found, int cornersCount) p = CameraCalibrator.DrawCornersOnMat(outImage);
                LeftCornersFound = p.found;
                LeftCornersCount = p.cornersCount;

                // CornerIndexes.Clear();
                // this.RaisePropertyChanged(nameof(CornerIndexes));

                // if ((LeftCornersFound && RightCornersFound) && (LeftCornersCount == RightCornersCount))
                // {                    
                //     for (int i = 0; i < LeftCornersCount; i++)
                //     {
                //         CornerIndexes.Add(i);
                //     }
                //     this.RaisePropertyChanged(nameof(CornerIndexes));
                // }

                LeftImage = new Bitmap(outImage.ToMemoryStream());
                image.Dispose();
                outImage.Dispose();
            });
                
            _rightStereoSubscriber.Subscribe(x =>
            {
                if (x.data == null)
                {
                    return;
                }

                int columns = (int) x.width;
                int rows = (int) x.height;

                // For image_raw topic.
                Mat image = new Mat(rows, columns, MatType.CV_8U, x.data.ToArray());
                Mat outImage = new Mat();
                Cv2.CvtColor(image, outImage, ColorConversionCodes.BayerRG2RGB);

                (bool found, int cornersCount) p = CameraCalibrator.DrawCornersOnMat(outImage);
                RightCornersFound = p.found;
                RightCornersCount = p.cornersCount;

                RightImage = new Bitmap(outImage.ToMemoryStream());
                image.Dispose();
                outImage.Dispose();
            });
        }

        private void CloseAllStereoSteam()
        {
            foreach (var node in Ros.GetNodes())
            {
                node.Dispose();
            }
        }

        public void Reconstruct3dCorner()
        {
            CloseAllStereoSteam();
            
            // TODO: 3d construction
        }

        public void PointingCorner(int cornerIndex)
        {
            CalculateSrcDestLinks();

            // TODO_operationArm
            string cmd = $"pointing.sh {_destLink} {_sourceLink} {_operationArm} {cornerIndex} {_stereoName}";
            cmd.InvokeRosMasterScript();
        }
        
        private void CalculateSrcDestLinks()
        {
            // TODO
            if (_stereoType == StereoTypes.Realsense)
            {
                _sourceLink = "realsense_link";
            }
            else if (_stereoType == StereoTypes.BodyStereo)
            {
                _sourceLink = "body_link";
            }
            else
            {
                _sourceLink = $"{CalibConfig.ArmToolsNames[(int)_stereoType]}_link";
            }

            if (_operationArm == RobotArms.LeftArm)
            {
                _destLink = "left_arm_link";
            }
            else if (_operationArm == RobotArms.RightArm)
            {
                _destLink = "right_arm_link";
            }
            else
            {
                _destLink = "left_arm_link";
            }
        }
    }
}