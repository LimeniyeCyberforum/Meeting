using MeetingCommon.Abstractions;
using MeetingCommon.Abstractions.Messanger;
using MeetingCommon.DataTypes;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using WebcamWithOpenCV;
using MeetingWPF.Chat;
using MeetingWPF.Connect;

namespace MeetingWPF
{
    public class MainViewModel : BaseInpc
    {
        private readonly Dispatcher dispatcher = Application.Current.Dispatcher;
        private readonly MeetingServiceAbstract _meetingServiceAbstract;
        private readonly CamStreaming _cam;
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

            _cam = CamInitializeTest();
        }


        private CamStreaming CamInitializeTest()
        {
            CamStreaming result = null;
            var selectedCameraDeviceId = CameraDevicesEnumerator.GetAllConnectedCameras()[0].OpenCvId;
            if (_cam == null || _cam.CameraDeviceId != selectedCameraDeviceId)
            {
                _cam?.Dispose();
                result = new CamStreaming(
                    frameWidth: 300,
                    frameHeight: 300,
                    selectedCameraDeviceId);
            }

            return result;
        }



        private void OnConnectionStateChanged(object? sender, (ConnectionAction Action, UserDto User) e)
        {
            IsConnected = e.Action == ConnectionAction.Connected ? true : false;
            ChatVM = new ChatViewModel(_meetingServiceAbstract.MessageService, _meetingServiceAbstract);
            _meetingServiceAbstract.CameraCaptureService.CameraFrameChanged += OnCameraFrameChanged;
            _cam.CaptureFrameChanged += OnCaptureFrameChanged;
            _meetingServiceAbstract.CameraCaptureService.UsersCameraCaptureSubscribeAsync();
            //_ = _cam.Start();
        }

        private void OnCaptureFrameChanged(object? sender, System.IO.Stream e)
        {
            _meetingServiceAbstract.CameraCaptureService.SendOwnCameraCaptureAsync(e);
        }

        private void OnCameraFrameChanged(object sender, Guid userGuid, byte[] frameBytes)
        {
            if (!InputVideoStreams.ContainsKey(userGuid))
            {
                dispatcher.BeginInvoke(() => InputVideoStreams.Add(userGuid, frameBytes));
            }
            else 
            {
                dispatcher.BeginInvoke(() => InputVideoStreams[userGuid] = frameBytes);
            }
        }
    }
}
