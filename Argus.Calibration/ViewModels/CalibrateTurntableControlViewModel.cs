using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Argus.Calibration.Helper;
using RosSharp;

namespace Argus.Calibration.ViewModels
{
    public class CalibrateTurntableControlViewModel : ViewModelBase
    {
        private string _result;
        private string _calibFile;

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
            Result = "转台标定中……\n请等待标定计算完成";
            CalibFile = "转台标定文件：";
        }

        public void CalibrateTurntable(MainWindowViewModel mainWindowVm)
        {
            try
            {
                Ros.MasterUri = new Uri(CalibConfig.RosMasterUri);
                Ros.HostName = CalibConfig.HostName;
                Ros.TopicTimeout = CalibConfig.TopicTimeout;
                Ros.XmlRpcTimeout = CalibConfig.XmlRpcTimeout;

                var node = Ros.InitNodeAsync(CalibConfig.NodeName).Result;
                var subscriber = node.SubscriberAsync<RosSharp.std_msgs.String>(@"/qt_echo_topic").Result;

                subscriber.Subscribe(x =>
                {
                    Result = x.data;
                });
            
                mainWindowVm.AddOperationLog($"启动Master上的转台标定节点");
                string calibTurntableCmd = $"calibrate_turntable.sh";
                calibTurntableCmd.InvokeRosMasterScript();
            }
            catch (Exception e)
            {
                mainWindowVm.AddOperationLog($"错误！{e.Message}");
                Result = $"错误！{e.Message}";
            }
        }

        public void Dispose()
        {
            string cleanUpCmd = $"kill_all.sh";
            cleanUpCmd.InvokeRosMasterScript();
        }
    }
}
