using Argus.Calibration.Helper;
using Argus.StereoCalibration;
using ReactiveUI;
using RosSharp;
using RosSharp.Topic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Argus.Calibration.ViewModels
{
    public class CalibrateMultiSensorControlViewModel : ViewModelBase, IDisposable
    {
        private const string SnapshotsDir = "MultiSensorStereoSnapshots";
        private const string CalibrationResultDir = "CalibrationResults";

        private string result;
        private Node? _node;
        private Subscriber<RosSharp.std_msgs.String>? _subscriber;

        public string Result
        {
            get => result;
            set => this.RaiseAndSetIfChanged(ref result, value);
        }
        
        public string RobotName { get; set; }

        public CalibrateMultiSensorControlViewModel()
        {
        }

        public void initRos()
        {
            // Thread.Sleep(5000);
            Ros.MasterUri = new Uri(CalibConfig.RosMasterUri);
            Ros.HostName = CalibConfig.HostName;
            Ros.TopicTimeout = CalibConfig.TopicTimeout;
            Ros.XmlRpcTimeout = CalibConfig.XmlRpcTimeout;
            var nodes = Ros.GetNodes();
            foreach (var n in Ros.GetNodes())
            {
                n.Dispose();
            }
            _node = Ros.InitNodeAsync(CalibConfig.NodeName).Result;
            _subscriber = _node.SubscriberAsync<RosSharp.std_msgs.String>(@"/qt_echo_topic").Result;

            _subscriber.Subscribe(x =>
            {
                Result = x.data;
            });
        }
        
        public void Dispose()
        {
            if (_node != null)
            {
                _node.Dispose();
            }

            // 4. Clean up
            string cleanUpCmd = $"kill_all.sh";
            cleanUpCmd.InvokeRosMasterScript();
        }

        public async Task CheckPositionAsync(MainWindowViewModel mainWindowVm)
        {
            mainWindowVm.AddOperationLog("请等待机械臂移动至抓拍位置......");
            // 0. Prepare robot arm movement environment.
            await Task.Run(() =>
            {
                mainWindowVm.AddOperationLog($"启动Master上的机械臂控制节点");
                string initArmCmd = $"init_arm_move.sh";
                initArmCmd.InvokeRosMasterScript();
            });

            // 1. Move robot arms to snapshot positions.

            // 1.1 Body stereo check only need move left arm to initial position
            string filepath = Path.Combine(CalibConfig.MovementFileDir, CalibConfig.BodyStereoArmPositionFile);
            string[] positions = File.ReadAllText(filepath).Split("\n");

            await Task.Run(() =>
            {
                mainWindowVm.AddOperationLog($"将左臂移动至 {positions[0]}");
                //string moveLeftCmd = $"Scripts/move_leftarm.sh '{positions[0]}'";
                //TODO
                string moveLeftCmd = $"Scripts/move_leftarm.sh '-0.28851 -1.08640 0.14578 1.212 3.057 0.148'";
                moveLeftCmd.RunSync();
            });

            // 2. Take snapshot.
            FsHelper.PurgeDirectory(SnapshotsDir);

            mainWindowVm.AddOperationLog("请等待抓拍完成......");
            await Task.Run(() =>
            {
                string snapshotCmd = $"Scripts/snapshot_body.sh '{SnapshotsDir}'";
                snapshotCmd.RunSync();
            });
            mainWindowVm.AddOperationLog("抓拍完成......");

        }

        public async Task Calibrate(MainWindowViewModel mainWindowVm)
        {

             mainWindowVm.AddOperationLog("请等待机械臂移动至抓拍位置......");
            // 0. Prepare robot arm movement environment.
            await Task.Run(() =>
            {
                mainWindowVm.AddOperationLog($"启动Master上的机械臂控制节点");
                string initArmCmd = $"init_arm_move.sh";
                initArmCmd.InvokeRosMasterScript();
            });

            // 1. Move robot arms to snapshot positions.

            // 1.1 Body stereo check only need move left arm to initial position
            string filepath = Path.Combine(CalibConfig.MovementFileDir, CalibConfig.BodyStereoArmPositionFile);
            string[] positions = File.ReadAllText(filepath).Split("\n");

            await Task.Run(() =>
            {
                mainWindowVm.AddOperationLog($"将左臂移动至 {positions[0]}");
                //string moveLeftCmd = $"Scripts/move_leftarm.sh '{positions[0]}'";
                //TODO
                string moveLeftCmd = $"Scripts/move_leftarm.sh '-0.28851 -1.08640 0.14578 1.212 3.057 0.148'";
                moveLeftCmd.RunSync();
            });

            Thread.Sleep(10000);

            // await CheckPositionAsync(mainWindowVm);
            // 3. Find stereo corner.
            // string leftSnapshotDir = Path.Combine(SnapshotsDir, "left");
            // string rightSnapshotDir = Path.Combine(SnapshotsDir, "right");
            // var LeftImagePath = FsHelper.GetFirstFileByNameFromDirectory(leftSnapshotDir, "left");
            // var RightImagePath = FsHelper.GetFirstFileByNameFromDirectory(rightSnapshotDir, "right");
            // mainWindowVm.AddOperationLog($"左目抓拍图像保存至: {LeftImagePath}");
            // mainWindowVm.AddOperationLog($"右目抓拍图像保存至: {RightImagePath}");

            // mainWindowVm.AddOperationLog("图像角点识别中......");
            // bool foundLeft= CameraCalibrator.CheckAndDrawConCorners(LeftImagePath);
            // bool foundRight=CameraCalibrator.CheckAndDrawConCorners(RightImagePath);

            // mainWindowVm.AddOperationLog("双目抓拍识别完成");
            // if (!foundLeft || !foundRight)
            // {
            //     mainWindowVm.AddOperationLog("存在未识别的角点，请移动标定板位置");
            //     return;
            // }

            // copy image files to remote machine

            //4 find realsense corner
            // await Task.Factory.StartNew(() =>
            // {
            mainWindowVm.AddOperationLog("开始远程标定RealSense与双目相机...");
            string cmd = $"calibrate_rs_stereo_runsh.sh";
            cmd.InvokeRosMasterScript();
            mainWindowVm.AddOperationLog("标定结果将显示在界面上，请耐心等待...");

            // });
        }

        public void SaveResult()
        {
            string cmd = $"Scripts/copy_rs2stereo_param.sh {RobotName}";
            cmd.RunSync();
        }
    }
}
