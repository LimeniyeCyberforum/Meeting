using OpenCvSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meeting.Wpf.Camera
{
    public class CamStreaming
    {
        private Task? _previewTask;

        private CancellationTokenSource? _cancellationTokenSource;
        public int CameraDeviceId { get; private set; }
        public bool FlipHorizontally { get; set; }

        public event EventHandler<byte[]>? CaptureFrameChanged;

        public CamStreaming(int cameraDeviceId)
        {
            CameraDeviceId = cameraDeviceId;
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
                                CaptureFrameChanged?.Invoke(this, frame.ToBytes());

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
            if (_cancellationTokenSource is not null && _cancellationTokenSource.IsCancellationRequested)
                return;

            if (_previewTask is not null && !_previewTask.IsCompleted)
            {
                _cancellationTokenSource?.Cancel();

                // Wait for it, to avoid conflicts with read/write of _lastFrame
                await _previewTask;
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
