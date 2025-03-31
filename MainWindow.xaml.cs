using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using NAudio.Wave;

namespace LiteBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WaveInEvent? waveIn;

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

        private void AudioInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected Index
            int selectedIndex = AudioInput.SelectedIndex;
            if (selectedIndex >= 0)
            {
                // Call StartRecording method with the selected Index
                StartRecording(selectedIndex);

                // Hide Watermark when a selection is made
                ComboBoxWatermark.Visibility = Visibility.Hidden;
            }
        }

        // Create a method to update the VolumeBar smoothly
        private void UpdateVolume(double newVolume)
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                From = VolumeBar.Value,
                To = newVolume,
                Duration = TimeSpan.FromMilliseconds(100),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            VolumeBar.BeginAnimation(ProgressBar.ValueProperty, animation);
        }

        // Use selected audio input device and start recording
        private void StartRecording(int deviceIndex)
        {
            // Stop and dispose the previous record
            waveIn?.StopRecording();
            waveIn?.Dispose();

            waveIn = new WaveInEvent
            {
                DeviceNumber = deviceIndex,
                WaveFormat = new WaveFormat(44100, 1) // 44.1 kHz, 16-bit, mono
            };

            waveIn.DataAvailable += WaveIn_DataAvailable!;
            waveIn.StartRecording();
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            double sum = 0;
            int bytesPerSample = 2; // 16-bit audio
            int sampleCount = e.BytesRecorded / bytesPerSample;

            for (int i = 0; i < e.BytesRecorded; i += bytesPerSample)
            {
                short sample = BitConverter.ToInt16(e.Buffer, i);
                double sampleFloat = sample / 32768.0; // to float
                sum += sampleFloat * sampleFloat;
            }

            double rms = Math.Sqrt(sum / sampleCount);
            double VolumePercent = rms * 100;

            // Dispatcher to update VolumeBar
            Dispatcher.Invoke(() => UpdateVolume(VolumePercent));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // On app close, stop recording and dispose the WaveInEvent
            waveIn?.StopRecording();
            waveIn?.Dispose();
        }
    }
}