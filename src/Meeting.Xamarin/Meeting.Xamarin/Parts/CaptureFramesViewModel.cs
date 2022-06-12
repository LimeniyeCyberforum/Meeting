using Meeting.Business.Common.Abstractions;
using Meeting.Business.Common.Abstractions.FrameCapture;
using Meeting.Business.Common.DataTypes;
using System;
using System.Linq;
using System.Reactive.Disposables;
using Toolkit.Xamarin;
using Xamarin.Forms;

namespace Meeting.Xamarin.Parts
{
    public class CaptureFrameViewModel : BaseInpc
    {
        private Guid _areaGuid, _ownerGuid;
        private string _ownerName;
        private byte[] _data;

        public Guid AreaGuid { get => _areaGuid; set => Set(ref _areaGuid, value); }
        public Guid OwnerGuid { get => _ownerGuid; set => Set(ref _ownerGuid, value); }
        public string OwnerName { get => _ownerName; set => Set(ref _ownerName, value); }
        public byte[] Data { get => _data; set => Set(ref _data, value); }

        public CaptureFrameViewModel(Guid areaGuid, Guid ownerGuid, string ownerName, byte[] data)
        {
            AreaGuid = areaGuid;
            OwnerGuid = ownerGuid;
            OwnerName = ownerName;
            Data = data;
        }
    }

    public class CaptureFramesViewModel : BaseInpc
    {
        private readonly SerialDisposable eventSubscriptions = new SerialDisposable();
        private readonly CaptureFramesServiceAbstract _captureFramesService;
        private UserDto _currentUser;

        private readonly IMeetingAuthorization _authorizationService;
        private readonly IMeetingUsers _meetingUsers;

        private bool _isCameraOn;
        public bool IsCameraOn { get => _isCameraOn; set => Set(ref _isCameraOn, value); }

        public ObservableDictionary<Guid, CaptureFrameViewModel> CaptureFrameAreas { get; } = new ObservableDictionary<Guid, CaptureFrameViewModel>();

        public CaptureFramesViewModel(CaptureFramesServiceAbstract captureFramesService, IMeetingUsers users, IMeetingAuthorization authorizationService)
        {
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

            _meetingUsers.Users.UsersChanged += OnUsersChanged;
            _authorizationService.AuthorizationStateChanged += OnConnectionStateChanged;
            _captureFramesService.CaptureFrameStateChanged += OnCaptureFrameStateChanged;
            ProtectedPropertyChanged += OnProtectedPropertyChanged;

            disposable.Add(Disposable.Create(delegate
            {
                _meetingUsers.Users.UsersChanged -= OnUsersChanged;
                _authorizationService.AuthorizationStateChanged -= OnConnectionStateChanged;
                _captureFramesService.CaptureFrameStateChanged -= OnCaptureFrameStateChanged;
                ProtectedPropertyChanged -= OnProtectedPropertyChanged;
            }));
            eventSubscriptions.Disposable = disposable;
        }

        protected override void OnAppearing()
        {
            _captureFramesService.CaptureFrameChanged += OnCaptureFrameChanged;
        }

        protected override void OnDisappearing()
        {
            _captureFramesService.CaptureFrameChanged -= OnCaptureFrameChanged;
        }


        private void OnConnectionStateChanged(object sender, UserConnectionState action)
        {
            if (action == UserConnectionState.Connected)
            {
                _currentUser = _authorizationService.CurrentUser;
            }
            else if (action == UserConnectionState.Disconnected)
            {
                _currentUser = null;

                //_ = _cam.Stop();
                //_cam.Dispose();
            }
        }

        private void OnOwnCaptureFrameChanged(object sender, byte[] e)
        {
            _captureFramesService.SendFrameAsync(e, _currentUser.Guid, DateTime.UtcNow);
        }

        private void OnUsersChanged(object sender, Framework.EventArgs.NotifyDictionaryChangedEventArgs<Guid, Business.Common.DataTypes.UserDto> e)
        {
            var item = e.NewValue;
            var captureFrameArea = CaptureFrameAreas.Values.FirstOrDefault(x => x.OwnerGuid == item.Guid || x.AreaGuid == item.Guid);

            if (captureFrameArea == null)
                CaptureFrameAreas.Add(item.Guid, new CaptureFrameViewModel(item.Guid, item.Guid, item.UserName, null));
        }

        private void OnCaptureFrameStateChanged(object sender, Business.Common.EventArgs.CaptureFrameStateEventArgs e)
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
        }

        private void OnCaptureFrameChanged(object sender, Business.Common.EventArgs.CaptureFrameEventArgs e)
        {
            //Device.
            var area = CaptureFrameAreas[e.CaptureAreadGuid];
            area.Data = e.Bytes;
        }

        private void OnProtectedPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (string.Equals(nameof(IsCameraOn), propertyName))
            {
                if (IsCameraOn)
                {
                    //_cam.CaptureFrameChanged += OnOwnCaptureFrameChanged;
                    //_ = _cam.Start();
                }
                else
                {
                    //_cam.CaptureFrameChanged -= OnOwnCaptureFrameChanged;
                    //_ = _cam.Stop();
                }
            }
        }
    }
}
