using ImageProcessor;
using ImageProcessor.Imaging;
using Microsoft.Extensions.DependencyInjection;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WebcamWithOpenCV;
using WPFView.Chat;

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

    public sealed class WebcamStreaming0 : IDisposable
    {
        private System.Drawing.Bitmap _lastFrame;
        private Task _previewTask;

        private CancellationTokenSource _cancellationTokenSource;
        private readonly Image _imageControlForRendering;
        private readonly int _frameWidth;
        private readonly int _frameHeight;

        public int CameraDeviceId { get; private set; }
        public byte[] LastPngFrame { get; private set; }
        public bool FlipHorizontally { get; set; }

        public event EventHandler OnQRCodeRead;
        private readonly OpenCVQRCodeReader _qrCodeReader;

        private int _currentBarcodeReadFrameCount = 0;
        private const int _readBarcodeEveryNFrame = 10;

        public WebcamStreaming0(
            Image imageControlForRendering,
            int frameWidth,
            int frameHeight,
            int cameraDeviceId)
        {
            _imageControlForRendering = imageControlForRendering;
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            CameraDeviceId = cameraDeviceId;
            _qrCodeReader = new OpenCVQRCodeReader();
        }

        public async Task Start()
        {
            // Never run two parallel tasks for the webcam streaming
            if (_previewTask != null && !_previewTask.IsCompleted)
                return;

            var initializationSemaphore = new SemaphoreSlim(5, 5);

            _cancellationTokenSource = new CancellationTokenSource();
            _previewTask = Task.Run(async () =>
            {
                try
                {
                    // Creation and disposal of this object should be done in the same thread 
                    // because if not it throws disconnectedContext exception
                    var videoCapture = new VideoCapture();

                    if (!videoCapture.Open(CameraDeviceId))
                    {
                        throw new ApplicationException("Cannot connect to camera");
                    }

                    using (var frame = new Mat())
                    {
                        while (!_cancellationTokenSource.IsCancellationRequested)
                        {
                            videoCapture.Read(frame);

                            if (!frame.Empty())
                            {
                                //var messageService = IocService.ServiceProvider.GetService<BaseMessageServiceAbstract>();
                                //await messageService.SendCameraCaptureAsync(frame.ToMemoryStream());

                                if (OnQRCodeRead != null)
                                {
                                    // Try read the barcode every n frames to reduce latency
                                    if (_currentBarcodeReadFrameCount % _readBarcodeEveryNFrame == 0)
                                    {
                                        try
                                        {
                                            string qrCodeData = _qrCodeReader.DetectBarcode(frame);
                                            OnQRCodeRead.Invoke(
                                                this,
                                                new QRCodeReadEventArgs(qrCodeData));
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine(ex);
                                        }
                                    }

                                    _currentBarcodeReadFrameCount += 1 % _readBarcodeEveryNFrame;
                                }

                                // Releases the lock on first not empty frame
                                if (initializationSemaphore != null)
                                    initializationSemaphore.Release();

                                _lastFrame = FlipHorizontally
                                    ? BitmapConverter.ToBitmap(frame.Flip(FlipMode.Y))
                                    : BitmapConverter.ToBitmap(frame);

                                var lastFrameBitmapImage = _lastFrame.ToBitmapSource();
                                lastFrameBitmapImage.Freeze();
                                _imageControlForRendering.Dispatcher.Invoke(
                                    () => _imageControlForRendering.Source = lastFrameBitmapImage);
                            }

                            // 30 FPS
                            await Task.Delay(33);
                        }
                    }

                    videoCapture?.Dispose();
                }
                finally
                {
                    if (initializationSemaphore != null)
                        initializationSemaphore.Release();
                }

            }, _cancellationTokenSource.Token);

            // Async initialization to have the possibility to show an animated loader without freezing the GUI
            // The alternative was the long polling. (while !variable) await Task.Delay
            await initializationSemaphore.WaitAsync();
            initializationSemaphore.Dispose();
            initializationSemaphore = null;

            if (_previewTask.IsFaulted)
            {
                // To let the exceptions exit
                await _previewTask;
            }
        }

        public async Task Stop()
        {
            // If "Dispose" gets called before Stop
            if (_cancellationTokenSource.IsCancellationRequested)
                return;

            if (!_previewTask.IsCompleted)
            {
                _cancellationTokenSource.Cancel();

                // Wait for it, to avoid conflicts with read/write of _lastFrame
                await _previewTask;
            }

            if (_lastFrame != null)
            {
                using (var imageFactory = new ImageFactory())
                using (var stream = new MemoryStream())
                {
                    imageFactory
                        .Load(_lastFrame)
                        .Resize(new ResizeLayer(
                            size: new System.Drawing.Size(_frameWidth, _frameHeight),
                            resizeMode: ImageProcessor.Imaging.ResizeMode.Crop,
                            anchorPosition: AnchorPosition.Center))
                        .Save(stream);

                    LastPngFrame = stream.ToArray();
                }
            }
            else
            {
                LastPngFrame = null;
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _lastFrame?.Dispose();
        }

    }


















    public partial class MainWindow : System.Windows.Window
    {
        private WebcamStreaming0 _webcamStreaming;

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
 
        }

        private void OnCameraCaptureChanged(object? sender, byte[] e)
        {
            var lastFrameBitmapImage = ToImage(e);
            lastFrameBitmapImage.Freeze();

            Application.Current.Dispatcher.Invoke(
                () =>
                {
                    //outputCameraFromStream.Source = lastFrameBitmapImage;
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