using System.Diagnostics;
using System.Threading.Tasks;
using Argus.Calibration.Helper;
using Argus.Calibration.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
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

        private void OpenRosCore_OnClick(object? sender, RoutedEventArgs e)
        {
            $"Scripts/init_roscore.sh".Bash();
        }

        private void CloseRosCore_OnClick(object? sender, RoutedEventArgs e)
        {
            $"Scripts/process_stopper.sh roscore".Bash();
        }
    }
}
