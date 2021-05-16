using Argus.Calibration.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Argus.Calibration.Views
{
    public partial class CalibrateTurntableControl : UserControl
    {
        public CalibrateTurntableControl()
        {
            InitializeComponent();
            DataContext = new CalibrateTurntableControlViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
