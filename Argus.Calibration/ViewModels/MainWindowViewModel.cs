using System.Collections.ObjectModel;
using Argus.Calibration.Helper;
using ReactiveUI;

namespace Argus.Calibration.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        protected ObservableCollection<string> _logs;
        private int _newestLogIndex;

        public ObservableCollection<string> Logs
        {
            get => _logs;
        }

        public int NewestLogIndex
        {
            get => _newestLogIndex;
            set => this.RaiseAndSetIfChanged(ref _newestLogIndex, value);
        }

        private bool _stereoCalibrated;
        private bool _leftArmCalibrated;
        private bool _rightArmCalibrated;
        private int _selectedToolType;
        private bool _turntableCalibrated;

        public bool StereoCalibrated
        {
            get => _stereoCalibrated;
            set => this.RaiseAndSetIfChanged(ref _stereoCalibrated, value);
        }

        public bool LeftArmCalibrated
        {
            get => _leftArmCalibrated;
            set => this.RaiseAndSetIfChanged(ref _leftArmCalibrated, value);
        }

        public bool RightArmCalibrated
        {
            get => _rightArmCalibrated;
            set => this.RaiseAndSetIfChanged(ref _rightArmCalibrated, value);
        }

        public bool TurntableCalibrated
        {
            get => _turntableCalibrated;
            set => this.RaiseAndSetIfChanged(ref _turntableCalibrated, value);
        }

        public int SelectedToolType
        {
            get => _selectedToolType;
            set => this.RaiseAndSetIfChanged(ref _selectedToolType, value);
        }

        public MainWindowViewModel()
        {
            _logs = new ObservableCollection<string>();

            AddOperationLog("点击菜单栏或者快捷按钮开始操作");

            StereoCalibrated = true;
            LeftArmCalibrated = true;
            RightArmCalibrated = true;
            SelectedToolType = 0;

            //$"Scripts/init_roscore.sh".Bash();
        }

        public void AddOperationLog(string log)
        {
            _logs.Add(log);
            NewestLogIndex = _logs.Count - 1;
        }

        public void CleanUp()
        {
            string cleanUpCmd = $"kill_all.sh";
            cleanUpCmd.InvokeRosMasterScript();
        }      
    }
}