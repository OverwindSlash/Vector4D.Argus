using System;
using Argus.Calibration.Config;
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
        [NotNull] private readonly MenuItem _toolTypeMenu;
        [NotNull] private readonly ComboBox _toolTypeCombo;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

#if DEBUG
            this.AttachDevTools();
#endif

            _workArea = this.FindControl<StackPanel>("WorkArea");
            _toolTypeMenu = this.FindControl<MenuItem>("SelectToolType");
            _toolTypeCombo = this.FindControl<ComboBox>("ToolType");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void ShowGetStereoImagesControl(object? sender, RoutedEventArgs e)
        {
            _workArea.Children.Clear();

            ScCheckPositionControl control = new ScCheckPositionControl();
            control.InitDataContext(StereoTypes.BodyStereo);

            _workArea.Children.Add(control);
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

        private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_toolTypeCombo == null)
            {
                return;
            }

            int selectedIndex = _toolTypeCombo.SelectedIndex;

            var viewModel = (MainWindowViewModel)DataContext!;
            viewModel.SelectedToolType = selectedIndex;
        }

        private void SelectToolType(int toolType)
        {
            var viewModel = (MainWindowViewModel)DataContext!;
            viewModel.SelectedToolType = toolType;
        }

        private void SelectToolMenu_OnClick(object? sender, RoutedEventArgs e)
        {
            SelectToolType(_toolTypeMenu.SelectedIndex);
        }
    }
}