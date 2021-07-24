using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Argus.Calibration.Helper;
using RosSharp;
using RosSharp.Topic;
using System.Threading;

namespace Argus.Calibration.ViewModels
{
    public class CalibrateTurntableControlViewModel : ViewModelBase, IDisposable
    {
        private string _result;
        private string _calibFile;
        private Node? _node;
        private Subscriber<RosSharp.std_msgs.String>? _subscriber;

        public string Result
        {
            get => _result;
            set => this.RaiseAndSetIfChanged(ref _result, value);
        }

        public string CalibFile
        {
            get => _calibFile;
            set => this.RaiseAndSetIfChanged(ref _calibFile, value);
        }

        public CalibrateTurntableControlViewModel()
        {

        }
        public void initRos()
        {
            // Thread.Sleep(5000);
            Result = "转台标定中……\n请等待标定计算完成";
            CalibFile = "转台标定文件：";
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

        public void CalibrateTurntable(MainWindowViewModel mainWindowVm)
        {
            mainWindowVm.AddOperationLog("开启双目视频流......");
            string prepareStereoCmd = $"open_lucid_body_stereo_nodisp.sh";
            prepareStereoCmd.InvokeRosMasterScript();
            mainWindowVm.AddOperationLog($"启动Master上的转台标定节点");
            string calibTurntableCmd = $"calibrate_turntable.sh";
            calibTurntableCmd.InvokeRosMasterScript();
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

        public void SaveResult()
        {
            string cmd = $"Scripts/copy_turntable_param.sh";
            cmd.RunSync();
        }
    }
}
