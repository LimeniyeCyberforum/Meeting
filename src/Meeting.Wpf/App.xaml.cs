using Meeting.Business.Common.Abstractions;
using Meeting.Business.GrpcClient;
using Meeting.Wpf.Windows;
using System;
using System.Windows;

namespace Meeting.Wpf
{
    public partial class App : Application
    {
        private IMeetingService? _meetingService;

        private void OnApplicationLaunched(object sender, StartupEventArgs e)
        {
            _meetingService = new MeetingService();
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

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            if (_meetingService is not null)
            {
                _meetingService.Chat.ChatUnsubscribe();
                _meetingService.Users.UsersUnsubscribe();
                _meetingService.CaptureFrames.CaptureFrameAreasUnsubscribe();
                _meetingService.CaptureFrames.CaptureFramesUnsubscribe();
            }
            base.OnSessionEnding(e);
        }
    }
}
