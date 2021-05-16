using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

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
            Result = "x:\ny:\nz:\nqw:\nqx:\nqy:\nqz:\n";
            CalibFile = "转台标定文件：";
        }
    }
}
