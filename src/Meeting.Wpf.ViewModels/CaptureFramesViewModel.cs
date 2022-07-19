using Meeting.Core.Common;
using Meeting.Core.Common.DataTypes;
using Meeting.Wpf.Camera;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Threading;
using Toolkit.WindowsDesktop;
using WebcamWithOpenCV;

namespace Meeting.Wpf.ViewModels
{
    public class CaptureFramesViewModel : ReactiveObject, IDisposable
    {
        private readonly SerialDisposable eventSubscriptions = new SerialDisposable();
        private readonly Dispatcher dispatcher = Application.Current.Dispatcher;
        private readonly ICaptureFramesService _captureFramesService;
        private readonly CamStreaming? _cam;
        private UserDto? _currentUser;

        private readonly IAuthorizationService _authorizationService;
        private readonly IUsersService _meetingUsers;

        [Reactive]
        public bool IsCameraOn { get; set; }

        public ObservableCollection<CaptureFrameViewModel> CaptureFrameAreas { get; } = new ObservableCollection<CaptureFrameViewModel>();

        public CaptureFramesViewModel(ICaptureFramesService captureFramesService, IUsersService usersService, IAuthorizationService authorizationService)
        {
            CaptureFrameAreas.EnableCollectionSynchronization();

             _cam = CamInitializeTest();
            _authorizationService = authorizationService;
            _captureFramesService = captureFramesService;
            _meetingUsers = usersService;

            this.WhenAnyValue(x => x.IsCameraOn).Subscribe(x => IsCameraOnChanged());

            Initizalize();
            Subscribe();
        }

        private void Initizalize()
        {
            lock (((ICollection)CaptureFrameAreas).SyncRoot)
            {
                foreach (var item in _captureFramesService.ActiveCaptureFrames)
                    CaptureFrameAreas.Add(new CaptureFrameViewModel(item.Key, item.Value.OwnerGuid, String.Empty, null));

                foreach (var item in _meetingUsers.Users)
                {
                    var captureFrameArea = CaptureFrameAreas.FirstOrDefault(x => x.OwnerGuid == item.Key || x.AreaGuid == item.Key);

                    if (captureFrameArea == null)
                        CaptureFrameAreas.Add(new CaptureFrameViewModel(item.Key, item.Value.Guid, item.Value.UserName, null));
                }
            }
        }

        private void Subscribe()
        {
            eventSubscriptions.Disposable = null;
            CompositeDisposable disposable = new CompositeDisposable();

            if (_cam != null)
                _cam.CaptureFrameChanged += OnOwnCaptureFrameChanged;

            _meetingUsers.UsersChanged += OnUsersChanged;
            _authorizationService.AuthorizationStateChanged += OnConnectionStateChanged;
            _captureFramesService.CaptureFrameStateChanged += OnCaptureFrameStateChanged;
            _captureFramesService.CaptureFrameChanged += OnCaptureFrameChanged;

            disposable.Add(Disposable.Create(delegate
            {
                if (_cam != null)
                    _cam.CaptureFrameChanged -= OnOwnCaptureFrameChanged;

                _meetingUsers.UsersChanged -= OnUsersChanged;
                _authorizationService.AuthorizationStateChanged -= OnConnectionStateChanged;
                _captureFramesService.CaptureFrameStateChanged -= OnCaptureFrameStateChanged;
                _captureFramesService.CaptureFrameChanged -= OnCaptureFrameChanged;
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
                result = new CamStreaming(selectedCameraDeviceId);
            }

            return result;
        }

        private void OnOwnCaptureFrameChanged(object? sender, byte[] e)
        {
            if (_currentUser is not null)
                _captureFramesService.SendFrameAsync(e, _currentUser.Guid, DateTime.UtcNow);
        }

        private void OnUsersChanged(object? sender, Utils.EventArgs.NotifyDictionaryChangedEventArgs<Guid, UserDto> e)
        {
            lock (((ICollection)CaptureFrameAreas).SyncRoot)
            {
                var item = e.NewValue;
                var captureFrameArea = CaptureFrameAreas.FirstOrDefault(x => x.OwnerGuid == item.Guid || x.AreaGuid == item.Guid);

                if (captureFrameArea == null)
                {
                    CaptureFrameAreas.Add(new CaptureFrameViewModel(item.Guid, item.Guid, item.UserName, null));
                }
            }
        }

        private void OnCaptureFrameStateChanged(object? sender, Core.Common.EventArgs.CaptureFrameStateEventArgs e)
        {
            lock (((ICollection)CaptureFrameAreas).SyncRoot)
            {
                UserDto? user = null;
                switch (e.Action)
                {
                    case Core.Common.EventArgs.CaptureFrameState.Disabled:
                        CaptureFrameViewModel? captureFrameArea = CaptureFrameAreas.FirstOrDefault(x => x.AreaGuid == e.CaptureAreadGuid);
                        int index = captureFrameArea is null ? -1 : CaptureFrameAreas.IndexOf(captureFrameArea);
                        if (index > -1)
                        {
                            CaptureFrameAreas[index].Data = null;
                        }
                        else
                        {
                            user = _meetingUsers.Users[e.OwnerGuid];
                            CaptureFrameAreas.Add(new CaptureFrameViewModel(e.OwnerGuid, e.CaptureAreadGuid, user.UserName, null));
                        }
                        break;
                    case Core.Common.EventArgs.CaptureFrameState.Created:
                        user = _meetingUsers.Users[e.OwnerGuid];
                        CaptureFrameAreas.Add(new CaptureFrameViewModel(e.OwnerGuid, e.CaptureAreadGuid, user.UserName, null));
                        break;
                    case Core.Common.EventArgs.CaptureFrameState.Removed:
                        CaptureFrameViewModel? captureFrameAreaForRemove = CaptureFrameAreas.FirstOrDefault(x => x.AreaGuid == e.CaptureAreadGuid);
                        int removeInxex = captureFrameAreaForRemove is null ? -1 : CaptureFrameAreas.IndexOf(captureFrameAreaForRemove);
                        if (removeInxex > -1)
                            CaptureFrameAreas.RemoveAt(removeInxex);
                        break;
                }
            }
        }

        private void OnCaptureFrameChanged(object? sender, Core.Common.EventArgs.CaptureFrameEventArgs e)
        {
            CaptureFrameViewModel? captureFrameArea = CaptureFrameAreas.FirstOrDefault(x => x.AreaGuid == e.CaptureAreadGuid);
            if (captureFrameArea is not null)
            {
                if (e.Bytes is not null)
                    dispatcher.Invoke(() => captureFrameArea.Data = e.Bytes);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"FrameArea not exitst {e}");
            }
        }

        private void IsCameraOnChanged()
        {
            if (IsCameraOn && _cam != null)
            {
                _cam.CaptureFrameChanged += OnOwnCaptureFrameChanged;
                _cam?.Start();
                if (_currentUser is not null)
                    _captureFramesService.TurnOnCaptureArea(_currentUser.Guid);
            }
            else if (_cam != null)
            {
                _cam.CaptureFrameChanged -= OnOwnCaptureFrameChanged;
                _cam?.Stop();
                if (_currentUser is not null)
                    _captureFramesService.TurnOffCaptureArea(_currentUser.Guid);
            }
        }

        public void Dispose()
        {
            eventSubscriptions?.Dispose();
        }
    }
}
