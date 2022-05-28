using System;
using System.Windows;

namespace MeetingWPF
{
    public partial class MainWindow : Window
    {
        //private WebcamStreaming0 _webcamStreaming;

        public MainWindow()
        {
            InitializeComponent();
            //cmbCameraDevices.ItemsSource = CameraDevicesEnumerator.GetAllConnectedCameras();
            //cmbCameraDevices.SelectedIndex = 0;
            //cameraLoading.Visibility = Visibility.Collapsed;
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
 
        }

        private void OnCameraCaptureChanged(object? sender, byte[] e)
        {
            //var lastFrameBitmapImage = ToImage(e);
            //lastFrameBitmapImage.Freeze();

            //Application.Current.Dispatcher.Invoke(
            //    () =>
            //    {
            //        //outputCameraFromStream.Source = lastFrameBitmapImage;
            //    });
        }

        //private BitmapImage ToImage(byte[] array)
        //{
        //    using (var ms = new System.IO.MemoryStream(array))
        //    {
        //        var image = new BitmapImage();
        //        image.BeginInit();
        //        image.CacheOption = BitmapCacheOption.OnLoad; // here
        //        image.StreamSource = ms;
        //        image.EndInit();
        //        return image;
        //    }
        //}



        private async void btnStop_Click(object sender, RoutedEventArgs e)
        {
            //chkQRCode.IsEnabled = false;
            //chkFlip.IsEnabled = false;

            //try
            //{
            //    await _webcamStreaming.Stop();
            //    btnStop.IsEnabled = false;
            //    btnStart.IsEnabled = true;

            //    // To save the screenshot
            //    // var screenshot = _webcamStreaming.LastPngFrame;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //_webcamStreaming?.Dispose();
        }

        private void chkQRCode_Checked(object sender, RoutedEventArgs e)
        {
            //if (_webcamStreaming != null)
            //{
            //    _webcamStreaming.OnQRCodeRead += _webcamStreaming_OnQRCodeRead;
            //}
        }

        private void _webcamStreaming_OnQRCodeRead(object sender, EventArgs e)
        {
            //txtQRCodeData.Dispatcher.Invoke(() =>
            //{
            //    var qrCodeData = (e as QRCodeReadEventArgs).QRCodeData;
            //    if (!string.IsNullOrWhiteSpace(qrCodeData))
            //    {
            //        txtQRCodeData.Document.Blocks.Clear();
            //        txtQRCodeData.Document.Blocks.Add(new Paragraph(new Run(qrCodeData)));
            //        txtQRCodeData.Foreground = new SolidColorBrush(Colors.Green);
            //    }
            //    else
            //    {
            //        txtQRCodeData.Foreground = new SolidColorBrush(Colors.Red);
            //    }
            //});
        }

        private void chkQRCode_Unchecked(object sender, RoutedEventArgs e)
        {
            //if (_webcamStreaming != null)
            //{
            //    _webcamStreaming.OnQRCodeRead -= _webcamStreaming_OnQRCodeRead;
            //}
        }

        private void btnClearQRCodeOutput_Click(object sender, RoutedEventArgs e)
        {
            //txtQRCodeData.Document.Blocks.Clear();
        }

        private void chkFlip_Checked(object sender, RoutedEventArgs e)
        {
            //if (_webcamStreaming != null)
            //{
            //    _webcamStreaming.FlipHorizontally = true;
            //}
        }

        private void chkFlip_Unchecked(object sender, RoutedEventArgs e)
        {
            //if (_webcamStreaming != null)
            //{
            //    _webcamStreaming.FlipHorizontally = false;
            //}
        }
    }
}