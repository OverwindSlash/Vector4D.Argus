using Argus.Calibration.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;

namespace Argus.Calibration.Views
{
    public class MainWindow : Window
    {
        [NotNull] private readonly StackPanel _workArea;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _workArea = this.FindControl<StackPanel>("WorkArea");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void ShowGetStereoImagesControl(object? sender, RoutedEventArgs e)
        {
            _workArea.Children.Clear();
            _workArea.Children.Add(new ScCheckPositionControl());
        }

        public void CloseWorkAreaControl()
        {
            _workArea.Children.Clear();
        }

        private void ShowCalibrateStereoControl(object? sender, RoutedEventArgs e)
        {
            _workArea.Children.Clear();
            _workArea.Children.Add(new ScCalibrateStereoControl());
        }
    }
}