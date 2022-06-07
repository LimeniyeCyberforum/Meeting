using Meeting.Business.Common.Abstractions;
using Meeting.Xamarin.Parts;
using Toolkit.Xamarin;
using Xamarin.Forms;

namespace Meeting.Xamarin.Pages
{
    public class MeetingViewModel : BaseInpc
    {
        private readonly IMeetingService _meetingService;

        private bool _isConnected = false;
        public bool IsConnected { get => _isConnected; private set => Set(ref _isConnected, value); }

        public ChatViewModel ChatVM { get; }
        public ConnectViewModel ConnectVM { get; }
        public CaptureFramesViewModel CaptureFramesVM { get; }

        public MeetingViewModel(IMeetingService meetingService)
        {
            _meetingService = meetingService;
            _meetingService.Chat.ChatSubscribeAsync();
            _meetingService.Users.UsersSubscribeAsync();
            _meetingService.CaptureFrames.CaptureFrameAreasSubscribeAsync();
            _meetingService.CaptureFrames.CaptureFramesSubscribeAsync();
            ChatVM = new ChatViewModel(_meetingService.Chat, _meetingService);
            ConnectVM = new ConnectViewModel(_meetingService);
            CaptureFramesVM = new CaptureFramesViewModel(_meetingService.CaptureFrames, _meetingService, _meetingService);
            _meetingService.AuthorizationStateChanged += OnConnectionStateChanged;
        }

        private void OnConnectionStateChanged(object sender, UserConnectionState action)
        {
            IsConnected = action == UserConnectionState.Connected;
        }
    }
}
