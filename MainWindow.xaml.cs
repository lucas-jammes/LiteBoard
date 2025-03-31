using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NAudio.Wave;

namespace LiteBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> audioInputs = [];

            // Browse input devices, add to audioInputs list
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(i);

                // Remove everything after the first parenthesis, space included
                string productName = Regex.Replace(deviceInfo.ProductName, @"\s*\(.*", "");

                audioInputs.Add(productName);
            }

            // Assign audioInputs to ComboBox
            AudioInput.ItemsSource = audioInputs;

            if (AudioInput.SelectedIndex == -1)
            {
                ComboBoxWatermark.Visibility = Visibility.Visible;
            }
        }

        // Drag window by clicking on the grid
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        // Close button
        private void CloseButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        // Volume to 0 when clicking on Volume Down image
        private void VolumeDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Slider.Value = 0;
        }

        // Volume to 100 when clicking on Volume Up image
        private void VolumeUp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Slider.Value = 100;
        }

        // Mute/Unmute when clicking on the Microphone image
        private void MicrophoneOn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MicrophoneOn.Visibility == Visibility.Visible)
            {
                MicrophoneOn.Visibility = Visibility.Hidden;
                MicrophoneOff.Visibility = Visibility.Visible;
            }
            else
            {
                MicrophoneOn.Visibility = Visibility.Visible;
                MicrophoneOff.Visibility = Visibility.Hidden;
            }
        }

        // Hide Watermark when a selection is made
        private void AudioInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxWatermark.Visibility = Visibility.Hidden;
        }
    }
}