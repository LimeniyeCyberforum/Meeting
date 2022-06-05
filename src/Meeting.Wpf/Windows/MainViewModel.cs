using Meeting.Business.Common.Abstractions;
using WebcamWithOpenCV;
using Meeting.WPF.Chat;
using Meeting.WPF.Connect;
using MvvmCommon.WindowsDesktop;
using Meeting.Wpf.CaptureFrames;
using System.IO;

namespace Meeting.WPF.Windows
{
    public class MainViewModel : BaseInpc
    {
        private readonly IMeetingService _meetingService;
        private readonly CamStreaming? _cam;

        private bool _isConnected = false, _isCameraOn = false;
        public bool IsConnected { get => _isConnected; private set => Set(ref _isConnected, value); }
        public bool IsCameraOn { get => _isCameraOn; set => Set(ref _isCameraOn, value); }

        public ChatViewModel ChatVM { get; }
        public ConnectViewModel ConnectVM { get; }
        public CaptureFramesViewModel CaptureFramesVM { get; }

        public MainViewModel(IMeetingService meetingService)
        {
            _meetingService = meetingService;
            ChatVM = new ChatViewModel(_meetingService.Chat, _meetingService);
            ConnectVM = new ConnectViewModel(_meetingService);
            CaptureFramesVM = new CaptureFramesViewModel(_meetingService.CaptureFrames);

            ProtectedPropertyChanged += OnProtectedPropertyChanged;
            _meetingService.AuthorizationStateChanged += OnConnectionStateChanged;

            _cam = CamInitializeTest();
        }

        private CamStreaming? CamInitializeTest()
        {
            CamStreaming result = null;
            var selectedCameraDeviceId = CameraDevicesEnumerator.GetAllConnectedCameras()[0].OpenCvId;
            if (_cam == null || _cam.CameraDeviceId != selectedCameraDeviceId)
            {
                _cam?.Dispose();
                result = new CamStreaming( frameWidth: 300, frameHeight: 300, selectedCameraDeviceId);
            }

            return result;
        }

        private void OnConnectionStateChanged(object? sender, UserConnectionState action)
        {
            IsConnected = action == UserConnectionState.Connected;

            if (action == UserConnectionState.Connected)
            {
                _cam.CaptureFrameChanged += OnOwnCaptureFrameChanged;
            }
            else
            {
                _cam.CaptureFrameChanged -= OnOwnCaptureFrameChanged;
                _ = _cam.Stop();
            }
        }

        private void OnOwnCaptureFrameChanged(object? sender, Stream e)
        {

        }

        private void OnProtectedPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (string.Equals(nameof(IsCameraOn), propertyName))
            {
                if (IsCameraOn)
                {
                    _cam.CaptureFrameChanged += OnOwnCaptureFrameChanged;
                    _ = _cam.Start();
                }
                else
                {
                    _cam.CaptureFrameChanged -= OnOwnCaptureFrameChanged;
                    _ = _cam.Stop();
                }
            }
        }
    }
}
