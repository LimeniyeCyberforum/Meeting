using Meeting.Business.Common.Abstractions;
using Meeting.Wpf.UserControls.Chat;
using Meeting.Wpf.UserControls.Connect;
using MvvmCommon.WindowsDesktop;
using Meeting.Wpf.UserControls.CaptureFrames;
using System.Reactive.Disposables;
using System;

namespace Meeting.Wpf.Windows
{
    public class MeetingViewModel : BaseInpc
    {
        private readonly SerialDisposable eventSubscriptions = new SerialDisposable();

        private readonly IMeetingService _meetingService;

        private bool _isConnected = false;
        public bool IsConnected { get => _isConnected; private set => Set(ref _isConnected, value); }

        public ChatViewModel ChatVM { get; }
        public ConnectViewModel ConnectVM { get; }
        public CaptureFramesViewModel CaptureFramesVM { get; }

        public MeetingViewModel(IMeetingService meetingService)
        {
            _meetingService = meetingService;

            ChatVM = new ChatViewModel(_meetingService.Chat, _meetingService);
            ConnectVM = new ConnectViewModel(_meetingService);
            CaptureFramesVM = new CaptureFramesViewModel(_meetingService.CaptureFrames, _meetingService, _meetingService);

            Subscriptions();
        }

        private void Subscriptions()
        {
            eventSubscriptions.Disposable = null;
            CompositeDisposable disposable = new CompositeDisposable();
            _meetingService.AuthorizationStateChanged += OnConnectionStateChanged;
            disposable.Add(Disposable.Create(delegate
            {
                _meetingService.AuthorizationStateChanged -= OnConnectionStateChanged;
            }));
            eventSubscriptions.Disposable = disposable;
        }

        private void OnConnectionStateChanged(object? sender, UserConnectionState action)
        {
            IsConnected = action == UserConnectionState.Connected;
        }
    }
}
