using MeetingRepository.Abstractions.Messanger;
using Microsoft.Extensions.DependencyInjection;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WebcamWithOpenCV;

namespace WPFView
{
    public class MessageItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OwnerMessage { get; set; }
        public DataTemplate AnotherMessage { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is bool isOwner)
            {
                return SelectTemplateCore(isOwner);
            }

            throw new System.NotImplementedException($"Uknown DataTemplate parameter. \nType: {nameof(MessageItemDataTemplateSelector)}\nParametr: {item}");
        }

        private DataTemplate SelectTemplateCore(bool isOwner)
        {
            if (isOwner)
            {
                return OwnerMessage;
            }
            else
            {
                return AnotherMessage;
            }
        }
    }

    public partial class MainWindow : System.Windows.Window
    {
        private WebcamStreaming _webcamStreaming;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ChatViewModel();
            cmbCameraDevices.ItemsSource = CameraDevicesEnumerator.GetAllConnectedCameras();
            cmbCameraDevices.SelectedIndex = 0;
            cameraLoading.Visibility = Visibility.Collapsed;


        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            chkQRCode.IsEnabled = true;
            chkFlip.IsEnabled = true;
            cameraLoading.Visibility = Visibility.Visible;
            webcamContainer.Visibility = Visibility.Hidden;
            btnStop.IsEnabled = false;
            btnStart.IsEnabled = false;

            var selectedCameraDeviceId = (cmbCameraDevices.SelectedItem as CameraDevice).OpenCvId;
            if (_webcamStreaming == null || _webcamStreaming.CameraDeviceId != selectedCameraDeviceId)
            {
                _webcamStreaming?.Dispose();
                _webcamStreaming = new WebcamStreaming(
                    imageControlForRendering: webcamPreview,
                    frameWidth: 300,
                    frameHeight: 300,
                    cameraDeviceId: cmbCameraDevices.SelectedIndex);
            }






            var videoCapture = new VideoCapture();

            if (!videoCapture.Open(_webcamStreaming.CameraDeviceId))
            {
                throw new ApplicationException("Cannot connect to camera");
            }

            bool test = false;

            _ = Task.Run(async () =>
            {
                using (var frame = new Mat())
                {
                    await Task.Delay(2000);
                    await Application.Current.Dispatcher.Invoke(async () =>
                    {
                        try
                        {

                            videoCapture.Read(frame);

                            if (!frame.Empty())
                            {
                                //frame.Flip(FlipMode.Y)
                                frame.ToMemoryStream();

                                var messageService = IocService.ServiceProvider.GetService<BaseMessageServiceAbstract>();
                                messageService.SendCameraCaptureAsync(frame.ToMemoryStream());

                                messageService.CameraCaptureChanged += OnCameraCaptureChanged;
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                    });
                }
            });













            try
            {
                await _webcamStreaming.Start();
                btnStop.IsEnabled = true;
                btnStart.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                btnStop.IsEnabled = false;
                btnStart.IsEnabled = true;
            }

            cameraLoading.Visibility = Visibility.Collapsed;
            webcamContainer.Visibility = Visibility.Visible;
        }

        private void OnCameraCaptureChanged(object? sender, byte[] e)
        {
            //_lastFrame = FlipHorizontally
            //                        ? BitmapConverter.ToBitmap(frame.Flip(FlipMode.Y))
            //                        : BitmapConverter.ToBitmap(frame);

            var lastFrameBitmapImage = ToImage(e);
            lastFrameBitmapImage.Freeze();
            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    outputCameraFromStream.Source = lastFrameBitmapImage;
                });
        }

        private BitmapImage ToImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }



        private async void btnStop_Click(object sender, RoutedEventArgs e)
        {
            chkQRCode.IsEnabled = false;
            chkFlip.IsEnabled = false;

            try
            {
                await _webcamStreaming.Stop();
                btnStop.IsEnabled = false;
                btnStart.IsEnabled = true;

                // To save the screenshot
                // var screenshot = _webcamStreaming.LastPngFrame;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _webcamStreaming?.Dispose();
        }

        private void chkQRCode_Checked(object sender, RoutedEventArgs e)
        {
            if (_webcamStreaming != null)
            {
                _webcamStreaming.OnQRCodeRead += _webcamStreaming_OnQRCodeRead;
            }
        }

        private void _webcamStreaming_OnQRCodeRead(object sender, EventArgs e)
        {
            txtQRCodeData.Dispatcher.Invoke(() =>
            {
                var qrCodeData = (e as QRCodeReadEventArgs).QRCodeData;
                if (!string.IsNullOrWhiteSpace(qrCodeData))
                {
                    txtQRCodeData.Document.Blocks.Clear();
                    txtQRCodeData.Document.Blocks.Add(new Paragraph(new Run(qrCodeData)));
                    txtQRCodeData.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    txtQRCodeData.Foreground = new SolidColorBrush(Colors.Red);
                }
            });
        }

        private void chkQRCode_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_webcamStreaming != null)
            {
                _webcamStreaming.OnQRCodeRead -= _webcamStreaming_OnQRCodeRead;
            }
        }

        private void btnClearQRCodeOutput_Click(object sender, RoutedEventArgs e)
        {
            txtQRCodeData.Document.Blocks.Clear();
        }

        private void chkFlip_Checked(object sender, RoutedEventArgs e)
        {
            if (_webcamStreaming != null)
            {
                _webcamStreaming.FlipHorizontally = true;
            }
        }

        private void chkFlip_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_webcamStreaming != null)
            {
                _webcamStreaming.FlipHorizontally = false;
            }
        }
    }
}