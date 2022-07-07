using Meeting.Core.Common.Abstractions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Disposables;

namespace Meeting.Wpf.ViewModels
{
    public class MeetingViewModel : ReactiveObject, IDisposable
    {
        private readonly SerialDisposable eventSubscriptions = new SerialDisposable();

        private readonly IMeetingService _meetingService;

        [Reactive]
        public bool IsConnected { get; set; }

        public ChatViewModel ChatVM { get; }
        public ConnectViewModel ConnectVM { get; }
        public CaptureFramesViewModel CaptureFramesVM { get; }

        public MeetingViewModel(IMeetingService meetingService)
        {
            _meetingService = meetingService;

            ChatVM = new ChatViewModel(_meetingService.Chat, _meetingService);
            ConnectVM = new ConnectViewModel(_meetingService);
            CaptureFramesVM = new CaptureFramesViewModel(_meetingService.CaptureFrames, _meetingService, _meetingService);

            Subscribe();
        }

        private void Subscribe()
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

        public void Dispose()
        {
            ChatVM.Dispose();
            CaptureFramesVM.Dispose();
            eventSubscriptions?.Dispose();
        }
    }
}
