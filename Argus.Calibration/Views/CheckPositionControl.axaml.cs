using System;
using System.Threading.Tasks;
using Argus.Calibration.Config;
using Argus.Calibration.ViewModels;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Argus.Calibration.Views
{
    public class ScCheckPositionControl : UserControl
    {
        public ScCheckPositionControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void InitDataContext(StereoTypes stereoType)
        {
            DataContext = new CheckPositionControlViewModel(stereoType);
        }

        public async Task CheckPositionAsync()
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            MainWindowViewModel _windowViewModel = (MainWindowViewModel)window.DataContext!;

            var viewModel = (CheckPositionControlViewModel) DataContext!;
            await viewModel.CheckPositionAsync(_windowViewModel);
        }
    }
}