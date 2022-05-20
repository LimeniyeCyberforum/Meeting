using MeetingCommon.Abstractions;
using MeetingCommon.Abstractions.Messanger;
using WPFView.Chat;
using WPFView.Connect;

namespace WPFView
{
    public class MainViewModel : BaseInpc
    {
        private readonly MeetingServiceAbstract _meetingServiceAbstract;
        private bool _isConnected = false;
        private ChatViewModel _chatViewModel;

        public bool IsConnected { get => _isConnected; private set => Set(ref _isConnected, value); }
        public ChatViewModel ChatVM { get => _chatViewModel; private set => Set(ref _chatViewModel, value); }

        public ConnectViewModel ConnectVM { get; }

        public MainViewModel(MeetingServiceAbstract meetingServiceAbstract)
        {
            ConnectVM = new ConnectViewModel(meetingServiceAbstract);

            _meetingServiceAbstract = meetingServiceAbstract;
            _meetingServiceAbstract.ConnectionStateChanged += OnConnectionStateChanged;
        }

        private void OnConnectionStateChanged(object? sender, ConnectionAction e)
        {
            IsConnected = e == ConnectionAction.Connected ? true : false;
            ChatVM = new ChatViewModel(_meetingServiceAbstract.MessageService, _meetingServiceAbstract);
        }
    }
}
