﻿using Meeting.Business.Common.Abstractions;
using Meeting.Business.GrpcClient;
using Meeting.Wpf.ViewModels;
using System.Windows;

namespace Meeting.Wpf
{
    public partial class App : Application
    {
        private IMeetingService? _meetingService;

        private void OnApplicationLaunched(object sender, StartupEventArgs e)
        {
            _meetingService = MeetingService.Instance;
            _meetingService.Chat.ChatSubscribeAsync();
            _meetingService.Users.UsersSubscribeAsync();
            _meetingService.CaptureFrames.CaptureFrameAreasSubscribeAsync();
            _meetingService.CaptureFrames.CaptureFramesSubscribeAsync();

            MainWindow = new MeetingWindow()
            {
                DataContext = new MeetingViewModel(_meetingService)
            };

            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _meetingService?.Dispose();
            base.OnExit(e);
        }
    }
}
