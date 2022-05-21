using MeetingCommon.Abstractions;
using MeetingCommon.Abstractions.Messanger;
using MeetingCommon.DataTypes;
using System;
using System.Collections.ObjectModel;
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

        public ObservableDictionary<Guid, byte[]> InputVideoStreams { get; } = new ObservableDictionary<Guid, byte[]>();

        public MainViewModel(MeetingServiceAbstract meetingServiceAbstract)
        {
            ConnectVM = new ConnectViewModel(meetingServiceAbstract);

            _meetingServiceAbstract = meetingServiceAbstract;
            _meetingServiceAbstract.ConnectionStateChanged += OnConnectionStateChanged;
        }

        private void OnConnectionStateChanged(object? sender, (ConnectionAction Action, UserDto User) e)
        {
            IsConnected = e.Action == ConnectionAction.Connected ? true : false;
            ChatVM = new ChatViewModel(_meetingServiceAbstract.MessageService, _meetingServiceAbstract);
            _meetingServiceAbstract.CameraCaptureService.CameraFrameChanged += OnCameraFrameChanged;


        }

        private void OnCameraFrameChanged(object sender, Guid userGuid, byte[] frameBytes)
        {
            if (!InputVideoStreams.ContainsKey(userGuid))
            {
                InputVideoStreams.Add(userGuid, frameBytes);
            }
            else 
            {
                InputVideoStreams[userGuid] = frameBytes;
            }
        }
    }
}
