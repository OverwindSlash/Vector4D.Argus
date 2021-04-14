using Argus.Calibration.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Argus.Calibration.Views
{
    public class ScCalibrateStereoControl : UserControl
    {
        public ScCalibrateStereoControl()
        {
            InitializeComponent();
            DataContext = new ScCalibrateStereoControlViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}