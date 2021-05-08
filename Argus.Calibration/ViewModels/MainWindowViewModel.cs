using System.Collections.ObjectModel;
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

        public MainWindowViewModel()
        {
            _logs = new ObservableCollection<string>();

            AddOperationLog("点击菜单栏或者快捷按钮开始操作");
        }

        public void AddOperationLog(string log)
        {
            _logs.Add(log);
            NewestLogIndex = _logs.Count - 1;
        }
    }
}