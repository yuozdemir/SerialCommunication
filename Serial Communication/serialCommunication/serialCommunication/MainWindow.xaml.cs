using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Threading;
using System.Windows.Threading;
using System.IO;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace serialCommunication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool RequestStatus = false;
        public int data = 0;
        public int data_l = 4;
        public byte[] dataBytes;

        delegate void serialCalback(string val);

        CancellationTokenSource _tokenSource = null;

        public MainWindow()
        {
            InitializeComponent();

            COMPortSelecetor.Init();
            COMPortSelecetor.SetDataReceivedHandle(aDataReceivedHandler);

            /***********************************************************/
        }

        public void DoSomeWork(int milliseconds, CancellationToken token)
        {
            while (RequestStatus)   // Durumu sorgula.
            {
                if (COMPortSelecetor.port.ReadByte() == 10)   // Veri isteğini bekle.
                {
                    dataBytes = new byte[1];   // 1 byte'lık dizi tanımla.
                    dataBytes = BitConverter.GetBytes(data_l);  // Diziye paket uzunluğunu gir.
                    COMPortSelecetor.port.Write(dataBytes, 0, 1);   // Diziyi karşı tarafa gönder.


                    dataBytes = new byte[data_l];   // Paket uzunluğu kadar bir dizi tanımla.
                    Random random = new Random();   // Random fonksiyonunu çağır.

                    dataBytes[0] = Convert.ToByte(random.Next(0, 25));
                    dataBytes[1] = Convert.ToByte(random.Next(0, 25));
                    dataBytes[2] = Convert.ToByte(random.Next(0, 25));
                    dataBytes[3] = Convert.ToByte(random.Next(0, 25));

                    //data = random.Next(33554431, 2142432143);   // Random bir sayı oluştur.
                    //dataBytes = BitConverter.GetBytes(data);   // Sayıyı diziye yaz.
                    COMPortSelecetor.port.Write(dataBytes, 0, data_l);   // Diziyi karşı tarafa gönder.

                    Task.Delay(milliseconds).Wait();   // Bekle.
                }
            }
        }

        // ***************************************************************************************************************** //
        // ***************************************************************************************************************** //
        // ***************************************************************************************************************** //

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        // ***************************************************************************************************************** //
        // ***************************************************************************************************************** //
        // ***************************************************************************************************************** //

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (StopButton.IsEnabled == true)
            {
                MessageBox.Show("Please Press The Stop Button First", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            else
            {
                COMPortSelecetor.PushConnectButton();

                if (COMPortSelecetor.IsConnected())
                {
                    StartButton.IsEnabled = true;
                    StartButton.Foreground = Brushes.Green;

                    SettingsButton.IsEnabled = false;
                    SettingsButton.Foreground = Brushes.Gray;
                }

                else
                {
                    StartButton.IsEnabled = false;
                    StartButton.Foreground = Brushes.Gray;

                    SettingsButton.IsEnabled = true;
                    SettingsButton.Foreground = Brushes.Green;
                }
            }
        }

        private static void aDataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {

        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;

            int dataTime = Convert.ToInt32(this.DataTimeComboBox.Text);

            StartButton.IsEnabled = false;
            StartButton.Foreground = Brushes.Gray;

            StopButton.IsEnabled = true;
            StopButton.Foreground = Brushes.Green;

            RequestStatus = true;

            await Task.Run(() => DoSomeWork(dataTime, token)).ContinueWith((t) =>
            {
                System.Diagnostics.Debug.WriteLine($"Task completed. Status: {t.Status}");
            });
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = true;
            StartButton.Foreground = Brushes.Green;

            StopButton.IsEnabled = false;
            StopButton.Foreground = Brushes.Gray;

            RequestStatus = false;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(Settings_Menu, 10);
        }

        private void SettingsMenuSaveButton_Click(object sender, RoutedEventArgs e)
        {
            BaudRateComboBox.SelectedItem = BaudRateComboBox.SelectedIndex;
        }

        private void SettingsMenuCloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (Settings_Menu.Height == 0)
            {
                Panel.SetZIndex(Settings_Menu, 0);
            }
        }
    }
}
