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

        private bool _isConnected = false;
        public bool IsConnected { get => _isConnected; private set => Set(ref _isConnected, value); }

        public ChatViewModel ChatVM { get; }
        public ConnectViewModel ConnectVM { get; }
        public CaptureFramesViewModel CaptureFramesVM { get; }

        public MainViewModel(IMeetingService meetingService)
        {
            _cam = CamInitializeTest();
            _meetingService = meetingService;
            _meetingService.Chat.ChatSubscribeAsync();
            _meetingService.Users.UsersSubscribeAsync();
            _meetingService.CaptureFrames.CaptureFrameAreasSubscribeAsync();
            ChatVM = new ChatViewModel(_meetingService.Chat, _meetingService);
            ConnectVM = new ConnectViewModel(_meetingService);
            CaptureFramesVM = new CaptureFramesViewModel(_meetingService.CaptureFrames, _meetingService, _cam);

            _meetingService.AuthorizationStateChanged += OnConnectionStateChanged;
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

            if (action == UserConnectionState.Disconnected)
            {
                _ = _cam.Stop();
                _cam.Dispose();
            }
        }
    }
}
