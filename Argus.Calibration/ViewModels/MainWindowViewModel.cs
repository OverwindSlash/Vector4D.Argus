using ReactiveUI;

namespace Argus.Calibration.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private bool _positionChecked;
        public bool PositionChecked
        {
            get => _positionChecked;
            set => this.RaiseAndSetIfChanged(ref _positionChecked, value);
        }
        
        public MainWindowViewModel()
        {
            PositionChecked = false;
        }
    }
}