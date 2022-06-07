using Meeting.Business.Common.Abstractions;
using Meeting.Business.Common.Abstractions.FrameCapture;
using Meeting.Business.Common.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolkit.Xamarin;

namespace Meeting.Xamarin.Parts
{
    public class CaptureFrameViewModel : BaseInpc
    {
        private Guid _ownerGuid, _areaGuid;
        private string _ownerName;
        private byte[] _data;

        public Guid OwnerGuid { get => _ownerGuid; set => Set(ref _ownerGuid, value); }
        public Guid AreaGuid { get => _areaGuid; set => Set(ref _areaGuid, value); }
        public string OwnerName { get => _ownerName; set => Set(ref _ownerName, value); }
        public byte[] Data { get => _data; set => Set(ref _data, value); }

        public CaptureFrameViewModel(Guid ownerGuid, string ownerName, Guid areaGuid, byte[] data)
        {
            OwnerGuid = ownerGuid;
            AreaGuid = areaGuid;
            Data = data;
            OwnerName = ownerName;
        }
    }

    public class CaptureFramesViewModel : BaseInpc
    {
        private readonly CaptureFramesServiceAbstract _captureFramesService;
        //private readonly CamStreaming _cam;
        private UserDto _currentUser;

        private readonly IMeetingAuthorization _authorizationService;
        private readonly IMeetingUsers _meetingUsers;

        private bool _isCameraOn;
        public bool IsCameraOn { get => _isCameraOn; set => Set(ref _isCameraOn, value); }

        public ObservableDictionary<Guid, CaptureFrameViewModel> CaptureFrameAreas { get; } = new ObservableDictionary<Guid, CaptureFrameViewModel>();

        public CaptureFramesViewModel(CaptureFramesServiceAbstract captureFramesService, IMeetingUsers users, IMeetingAuthorization authorizationService)
        {
            //_cam = CamInitializeTest();
            //_cam.CaptureFrameChanged += OnOwnCaptureFrameChanged;


            _authorizationService = authorizationService;
            _authorizationService.AuthorizationStateChanged += OnConnectionStateChanged;

            _captureFramesService = captureFramesService;
            _captureFramesService.CaptureFrameStateChanged += OnCaptureFrameStateChanged;
            _captureFramesService.CaptureFrameChanged += OnCaptureFrameChanged;

            foreach (var item in _captureFramesService.ActiveCaptureFrames)
            {
                CaptureFrameAreas.Add(item.AreaGuid, new CaptureFrameViewModel(item.OwnerGuid, String.Empty, item.AreaGuid, null));
            }

            _meetingUsers = users;
            _meetingUsers.Users.UsersChanged += OnUsersChanged;

            foreach (var item in _meetingUsers.Users.Users)
            {
                var captureFrameArea = CaptureFrameAreas.Values.FirstOrDefault(x => x.OwnerGuid == item.Key || x.AreaGuid == item.Key);

                if (captureFrameArea == null)
                    CaptureFrameAreas.Add(item.Key, new CaptureFrameViewModel(item.Key, item.Value.UserName, item.Key, null));
            }

            ProtectedPropertyChanged += OnProtectedPropertyChanged;
        }


        private void OnConnectionStateChanged(object sender, UserConnectionState action)
        {
            if (action == UserConnectionState.Connected)
            {
                _currentUser = _authorizationService.CurrentUser;
            }
            else if (action == UserConnectionState.Disconnected)
            {
                //_ = _cam.Stop();
                //_cam.Dispose();
            }
        }

        //private CamStreaming? CamInitializeTest()
        //{
        //    CamStreaming result = null;
        //    var selectedCameraDeviceId = CameraDevicesEnumerator.GetAllConnectedCameras()[0].OpenCvId;
        //    if (_cam == null || _cam.CameraDeviceId != selectedCameraDeviceId)
        //    {
        //        _cam?.Dispose();
        //        result = new CamStreaming(frameWidth: 300, frameHeight: 300, selectedCameraDeviceId);
        //    }

        //    return result;
        //}

        private void OnOwnCaptureFrameChanged(object sender, byte[] e)
        {
            _captureFramesService.SendFrameAsync(e, _currentUser.Guid, DateTime.UtcNow);
        }


        private void OnUsersChanged(object sender, Framework.EventArgs.NotifyDictionaryChangedEventArgs<Guid, Business.Common.DataTypes.UserDto> e)
        {
            var item = e.NewValue;
            var captureFrameArea = CaptureFrameAreas.Values.FirstOrDefault(x => x.OwnerGuid == item.Guid || x.AreaGuid == item.Guid);

            if (captureFrameArea == null)
            {
                CaptureFrameAreas.Add(item.Guid, new CaptureFrameViewModel(item.Guid, item.UserName, item.Guid, null));
            }
        }

        private void OnCaptureFrameStateChanged(object sender, Business.Common.EventArgs.CaptureFrameStateEventArgs e)
        {
            if (e.IsOn)
            {
                CaptureFrameAreas.Add(e.CaptureAreadGuid, new CaptureFrameViewModel(e.OwnerGuid, null, e.CaptureAreadGuid, null));
            }
            else
            {
                CaptureFrameAreas.Remove(e.CaptureAreadGuid);
            }
        }

        private void OnCaptureFrameChanged(object sender, Business.Common.EventArgs.CaptureFrameEventArgs e)
        {
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
