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
        private Task<Process> _task;

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
            _task = $"Scripts/init_roscore.sh".BashCancellable();
        }

        private void CloseRosCore_OnClick(object? sender, RoutedEventArgs e)
        {
            Process process = _task.Result;
            process.Kill();
        }
    }
}
