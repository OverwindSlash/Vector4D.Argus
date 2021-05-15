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
        private MainWindowViewModel? _windowViewModel;

        public ScCheckPositionControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void StyledElement_OnInitialized(object? sender, EventArgs e)
        {
            var window = (MainWindow)this.Parent.Parent.Parent.Parent;
            _windowViewModel = (MainWindowViewModel)window.DataContext!;
        }

        public void InitDataContext(StereoTypes stereoType)
        {
            DataContext = new CheckPositionControlViewModel(stereoType, _windowViewModel);
        }

        public async Task CheckPositionAsync()
        {
            var viewModel = (CheckPositionControlViewModel) DataContext!;
            await viewModel.CheckPositionAsync();
        }
    }
}