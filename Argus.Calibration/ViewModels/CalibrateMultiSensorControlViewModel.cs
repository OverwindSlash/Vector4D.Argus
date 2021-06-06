using Argus.Calibration.Helper;
using Argus.StereoCalibration;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Argus.Calibration.ViewModels
{
    public class CalibrateMultiSensorControlViewModel : ViewModelBase, IDisposable
    {
        private const string SnapshotsDir = "MultiSensorStereoSnapshots";
        private const string CalibrationResultDir = "CalibrationResults";

        private string result;

        public string Result
        {
            get => result;
            set => this.RaiseAndSetIfChanged(ref result, value);
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
                string moveLeftCmd = $"Scripts/move_leftarm.sh '{positions[0]}'";
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
            await CheckPositionAsync(mainWindowVm);
            // 3. Find stereo corner.
            string leftSnapshotDir = Path.Combine(SnapshotsDir, "left");
            string rightSnapshotDir = Path.Combine(SnapshotsDir, "right");
            var LeftImagePath = FsHelper.GetFirstFileByNameFromDirectory(leftSnapshotDir, "left");
            var RightImagePath = FsHelper.GetFirstFileByNameFromDirectory(rightSnapshotDir, "right");
            mainWindowVm.AddOperationLog($"左目抓拍图像保存至: {LeftImagePath}");
            mainWindowVm.AddOperationLog($"右目抓拍图像保存至: {RightImagePath}");

            mainWindowVm.AddOperationLog("图像角点识别中......");
            bool foundLeft= CameraCalibrator.CheckAndDrawConCorners(LeftImagePath);
            bool foundRight=CameraCalibrator.CheckAndDrawConCorners(RightImagePath);

            mainWindowVm.AddOperationLog("双目抓拍识别完成");
            if (!foundLeft || !foundRight)
            {
                mainWindowVm.AddOperationLog("存在未识别的角点，请移动标定板位置");
                return;
            }

            // copy image files to remote machine

            //4 find realsense corner
            await Task.Factory.StartNew(() =>
            {
                mainWindowVm.AddOperationLog("开始远程标定RealSense与双目相机...");
                string cmd = $"calibrate_rs_stereo_runsh.sh";
                cmd.InvokeRosMasterScript();
                mainWindowVm.AddOperationLog("标定结果将显示在界面上，请耐心等待...");

            });
        }


        public void Dispose()
        {
            string unInitArmCmd = $"kill_all.sh";
            unInitArmCmd.InvokeRosMasterScript();
        }
    }
}
