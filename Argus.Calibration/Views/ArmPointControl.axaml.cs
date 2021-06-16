using Argus.Calibration.Config;
using Argus.Calibration.Helper;
using Argus.Calibration.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Argus.Calibration.Views
{
    public partial class ArmPointControl : UserControl
    {
        public ArmPointControl()
        {
            InitializeComponent();
            DataContext = new ArmPointControlViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        public void SetStereoTypes(StereoTypes stereoType)
        {
            var viewModel = (ArmPointControlViewModel)DataContext!;
            viewModel.SetStereoTypes(stereoType);
        }

        // public void SetLinks(string sourceLink, string destLink)
        // {
        //     var viewModel = (ArmPointControlViewModel)DataContext!;
        //     viewModel.SetLinks(sourceLink, destLink);
        // }
        
        public void SetArm(RobotArms robotArms)
        {
            var viewModel = (ArmPointControlViewModel)DataContext!;
            viewModel.SetArm(robotArms);
        }
        
        public void Init()
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            MainWindowViewModel windowDataContext = (MainWindowViewModel)window.DataContext!;
            
            var viewModel = (ArmPointControlViewModel)DataContext!;
            viewModel.Init(windowDataContext);
        }
        
        public void MoveTurntable(int x1, int y1)
        {
            var viewModel = (ArmPointControlViewModel)DataContext!;
            viewModel.MoveTurntable(x1, y1);
        }

        private void OpenStereoStream_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            MainWindowViewModel windowDataContext = (MainWindowViewModel)window.DataContext!;
            
            var viewModel = (ArmPointControlViewModel)DataContext!;
            viewModel.OpenStereoStream(windowDataContext);
        }

        private void Reconstruct3dCorner_OnClick(object? sender, RoutedEventArgs e)
        {
            var viewModel = (ArmPointControlViewModel)DataContext!;
            viewModel.Reconstruct3dCorner();
        }

        private void PointingCorner_OnClick(object? sender, RoutedEventArgs e)
        {
            var viewModel = (ArmPointControlViewModel)DataContext!;
            viewModel.PointingCorner(1);
        }
    }
}
