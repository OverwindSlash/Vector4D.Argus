using Argus.Calibration.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Argus.Calibration.Views
{
    public partial class HandEyeCalibrationControl : UserControl
    {
        public HandEyeCalibrationControl()
        {
            InitializeComponent();
            DataContext = new HandEyeCalibrationControlViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
