using Meeting.Business.Common.Abstractions;
using Meeting.Business.Common.Abstractions.FrameCapture;
using Meeting.Business.Common.DataTypes;
using MvvmCommon.WindowsDesktop;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Threading;
using WebcamWithOpenCV;

namespace Meeting.Wpf.CaptureFrames
{
    public class CaptureFramesViewModel : BaseInpc
    {
        private readonly SerialDisposable eventSubscriptions = new SerialDisposable();
        private readonly Dispatcher dispatcher = Application.Current.Dispatcher;
        private readonly CaptureFramesServiceAbstract _captureFramesService;
        private readonly CamStreaming? _cam;
        private UserDto? _currentUser;

        private readonly IMeetingAuthorization _authorizationService;
        private readonly IMeetingUsers _meetingUsers;

        private bool _isCameraOn;
        public bool IsCameraOn { get => _isCameraOn; set => Set(ref _isCameraOn, value); }

        public ObservableDictionary<Guid, CaptureFrameViewModel> CaptureFrameAreas { get; } = new ObservableDictionary<Guid, CaptureFrameViewModel>();

        public CaptureFramesViewModel(CaptureFramesServiceAbstract captureFramesService, IMeetingUsers users, IMeetingAuthorization authorizationService)
        {
            _cam = CamInitializeTest();
            _authorizationService = authorizationService;
            _captureFramesService = captureFramesService;
            _meetingUsers = users;

            Initizalize();
            Subscriptions();
        }

        private void Initizalize()
        {
            foreach (var item in _captureFramesService.ActiveCaptureFrames)
                CaptureFrameAreas.Add(item.Key, new CaptureFrameViewModel(item.Key, item.Value.OwnerGuid, String.Empty, null));

            foreach (var item in _meetingUsers.Users.Users)
            {
                var captureFrameArea = CaptureFrameAreas.Values.FirstOrDefault(x => x.OwnerGuid == item.Key || x.AreaGuid == item.Key);

                if (captureFrameArea == null)
                    CaptureFrameAreas.Add(item.Key, new CaptureFrameViewModel(item.Key, item.Value.Guid, item.Value.UserName, null));
            }
        }

        private void Subscriptions()
        {
            eventSubscriptions.Disposable = null;
            CompositeDisposable disposable = new CompositeDisposable();

            if (_cam != null)
                _cam.CaptureFrameChanged += OnOwnCaptureFrameChanged;

            _meetingUsers.Users.UsersChanged += OnUsersChanged;
            _authorizationService.AuthorizationStateChanged += OnConnectionStateChanged;
            _captureFramesService.CaptureFrameStateChanged += OnCaptureFrameStateChanged;
            _captureFramesService.CaptureFrameChanged += OnCaptureFrameChanged;
            ProtectedPropertyChanged += OnProtectedPropertyChanged;

            disposable.Add(Disposable.Create(delegate
            {
                if (_cam != null)
                    _cam.CaptureFrameChanged -= OnOwnCaptureFrameChanged;

                _meetingUsers.Users.UsersChanged -= OnUsersChanged;
                _authorizationService.AuthorizationStateChanged -= OnConnectionStateChanged;
                _captureFramesService.CaptureFrameStateChanged -= OnCaptureFrameStateChanged;
                _captureFramesService.CaptureFrameChanged -= OnCaptureFrameChanged;
                ProtectedPropertyChanged -= OnProtectedPropertyChanged;
            }));
            eventSubscriptions.Disposable = disposable;
        }

        private void OnConnectionStateChanged(object? sender, UserConnectionState action)
        {
            if (action == UserConnectionState.Connected)
            {
                _currentUser = _authorizationService.CurrentUser;
            }
            else if (action == UserConnectionState.Disconnected)
            {
                _currentUser = null;
                _cam?.Stop();
                _cam?.Dispose();
            }
        }

        private CamStreaming? CamInitializeTest()
        {
            CamStreaming? result = null;
            var selectedCameraDeviceId = CameraDevicesEnumerator.GetAllConnectedCameras()[0].OpenCvId;

            if (_cam == null || _cam.CameraDeviceId != selectedCameraDeviceId)
            {
                _cam?.Dispose();
                result = new CamStreaming(frameWidth: 300, frameHeight: 300, selectedCameraDeviceId);
            }

            return result;
        }

        private void OnOwnCaptureFrameChanged(object? sender, byte[] e)
        {
            _captureFramesService.SendFrameAsync(e, _currentUser.Guid, DateTime.UtcNow);
        }

        private void OnUsersChanged(object? sender, Framework.EventArgs.NotifyDictionaryChangedEventArgs<Guid, Business.Common.DataTypes.UserDto> e)
        {
            var item = e.NewValue;
            var captureFrameArea = CaptureFrameAreas.Values.FirstOrDefault(x => x.OwnerGuid == item.Guid || x.AreaGuid == item.Guid);

            if (captureFrameArea == null)
            {
                dispatcher.BeginInvoke(() =>
                {
                    CaptureFrameAreas.Add(item.Guid, new CaptureFrameViewModel(item.Guid, item.Guid, item.UserName, null));
                });
            }
        }

        private void OnCaptureFrameStateChanged(object? sender, Business.Common.EventArgs.CaptureFrameStateEventArgs e)
        {
            dispatcher.BeginInvoke(() =>
            {
                switch (e.Action)
                {
                    case Business.Common.EventArgs.CaptureFrameState.Disabled:
                        var captureFrame = CaptureFrameAreas[e.CaptureAreadGuid];
                        captureFrame.Data = null;
                        break;
                    case Business.Common.EventArgs.CaptureFrameState.Created:
                        CaptureFrameAreas.Add(e.CaptureAreadGuid, new CaptureFrameViewModel(e.OwnerGuid, e.CaptureAreadGuid, "steve", null));
                        break;
                    case Business.Common.EventArgs.CaptureFrameState.Removed:
                        CaptureFrameAreas.Remove(e.CaptureAreadGuid);
                        break;
                }
            });
        }

        private void OnCaptureFrameChanged(object? sender, Business.Common.EventArgs.CaptureFrameEventArgs e)
        {
            dispatcher.BeginInvoke(() =>
            {
                var area = CaptureFrameAreas[e.CaptureAreadGuid];
                area.Data = e.Bytes;
            });
        }

        private void OnProtectedPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (string.Equals(nameof(IsCameraOn), propertyName))
            {
                if (IsCameraOn && _cam != null)
                {
                    _cam.CaptureFrameChanged += OnOwnCaptureFrameChanged;
                    _cam.Start();
                    _captureFramesService.TurnOnCaptureArea(_currentUser.Guid);
                }
                else if(_cam != null)
                {
                    _cam.CaptureFrameChanged -= OnOwnCaptureFrameChanged;
                    _cam.Stop();
                    _captureFramesService.TurnOffCaptureArea(_currentUser.Guid);
                }
            }
        }
    }
}
