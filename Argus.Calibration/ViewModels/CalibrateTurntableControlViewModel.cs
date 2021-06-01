using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Argus.Calibration.Helper;

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
            mainWindowVm.AddOperationLog($"启动Master上的转台标定节点");
            string calibTurntableCmd = $"calibrate_turntable.sh";
            calibTurntableCmd.InvokeRosMasterScript();
        }

        public void Dispose()
        {
            string cleanUpCmd = $"kill_all.sh";
            cleanUpCmd.InvokeRosMasterScript();
        }
    }
}
