﻿using MeetingCommon.Abstractions;
using MeetingCommon.Abstractions.Messanger;
using WPFView.Chat;
using WPFView.Connect;

namespace WPFView
{
    public class MainViewModel : BaseInpc
    {
        private readonly MeetingServiceAbstract _meetingServiceAbstract;
        private bool _isConnected = false;

        public bool IsConnected { get => _isConnected; set => Set(ref _isConnected, value); }

        public ConnectViewModel ConnectVM { get; }
        public ChatViewModel ChatVM { get; }

        public MainViewModel(MeetingServiceAbstract meetingServiceAbstract)
        {
            ConnectVM = new ConnectViewModel(meetingServiceAbstract);
            ChatVM = new ChatViewModel(meetingServiceAbstract.MessageService);

            _meetingServiceAbstract = meetingServiceAbstract;
            _meetingServiceAbstract.ConnectionStateChanged += OnConnectionStateChanged;
        }

        private void OnConnectionStateChanged(object? sender, ConnectionAction e)
        {
            IsConnected = e == ConnectionAction.Connected ? true : false;
        }
    }
}
