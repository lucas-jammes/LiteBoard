using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        // Drag window by clicking on the grid
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void CloseButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void VolumeDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Slider.Value = 0;
        }

        private void VolumeUp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Slider.Value = 100;
        }

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

        private void AudioInput_Loaded(object sender, RoutedEventArgs e)
        {
            var audioInputs = new List<string>();

            // Browse input devices, add to audioInputs list
            for (int i = 0; i < WaveIn.DeviceCount; i++)
            {
                var deviceInfo = WaveIn.GetCapabilities(i);
                audioInputs.Add(deviceInfo.ProductName);
            }

            // Assign audioInputs to ComboBox
            AudioInput.ItemsSource = audioInputs;

            if (AudioInput.SelectedIndex == -1)
            {
                ComboBoxTemplate.Visibility = Visibility.Visible;
            }
        }

        private void AudioInput_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Hide Watermark when a selection is made
            ComboBoxTemplate.Visibility = Visibility.Hidden;
        }
    }
}