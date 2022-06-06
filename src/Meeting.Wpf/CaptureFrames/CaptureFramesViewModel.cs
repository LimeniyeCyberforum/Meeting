using Meeting.Business.Common.Abstractions;
using Meeting.Business.Common.Abstractions.FrameCapture;
using Meeting.WPF;
using MvvmCommon.WindowsDesktop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Meeting.Wpf.CaptureFrames
{
    public class CaptureFramesViewModel : BaseInpc
    {
        private readonly Dispatcher dispatcher = Application.Current.Dispatcher;
        private readonly CaptureFramesServiceAbstract _captureFramesService;
        private readonly CamStreaming? _cam;
        private readonly IMeetingUsers _meetingUsers;

        private bool _isCameraOn;
        public bool IsCameraOn { get => _isCameraOn; set => Set(ref _isCameraOn, value); }

        public ObservableDictionary<Guid, CaptureFrameViewModel> CaptureFrameAreas { get; } = new ObservableDictionary<Guid, CaptureFrameViewModel>();

        public CaptureFramesViewModel(CaptureFramesServiceAbstract captureFramesService, IMeetingUsers users, CamStreaming? cam)
        {
            _cam = cam;
            _cam.CaptureFrameChanged += OnOwnCaptureFrameChanged;


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

        private void OnOwnCaptureFrameChanged(object? sender, Stream e)
        {

        }


        private void OnUsersChanged(object? sender, Framework.EventArgs.NotifyDictionaryChangedEventArgs<Guid, Business.Common.DataTypes.UserDto> e)
        {
            var item = e.NewValue;
            var captureFrameArea = CaptureFrameAreas.Values.FirstOrDefault(x => x.OwnerGuid == item.Guid || x.AreaGuid == item.Guid);

            if (captureFrameArea == null)
            {
                dispatcher.BeginInvoke(() =>
                {
                    CaptureFrameAreas.Add(item.Guid, new CaptureFrameViewModel(item.Guid, item.UserName, item.Guid, null));
                });
            }
        }

        private void OnCaptureFrameStateChanged(object? sender, Business.Common.EventArgs.CaptureFrameStateEventArgs e)
        {
            dispatcher.BeginInvoke(() =>
            {
                if (e.IsOn)
                {
                    CaptureFrameAreas.Add(e.CaptureAreadGuid, null);
                }
                else
                {
                    CaptureFrameAreas.Remove(e.CaptureAreadGuid);
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
                if (IsCameraOn)
                {
                    _cam.CaptureFrameChanged += OnOwnCaptureFrameChanged;
                    _ = _cam.Start();
                }
                else
                {
                    _cam.CaptureFrameChanged -= OnOwnCaptureFrameChanged;
                    _ = _cam.Stop();
                }
            }
        }
    }
}
