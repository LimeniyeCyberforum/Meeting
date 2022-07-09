using Meeting.Core.Common.Abstractions;
using Meeting.Core.GrpcClient;
using Meeting.Wpf.ViewModels;
using System.Windows;

namespace Meeting.Wpf
{
    public partial class App : Application
    {
        private IMeetingService? _meetingService;
        private MeetingViewModel? _meetingViewModel;

        private void OnApplicationLaunched(object sender, StartupEventArgs e)
        {
            _meetingService = MeetingService.Instance;
            _meetingService.Chat.ChatSubscribeAsync();
            _meetingService.Users.UsersSubscribeAsync();
            _meetingService.CaptureFrames.CaptureFrameAreasSubscribeAsync();
            _meetingService.CaptureFrames.CaptureFramesSubscribeAsync();

            _meetingViewModel = new MeetingViewModel(_meetingService);

            MainWindow = new MeetingWindow()
            {
                DataContext = _meetingViewModel
            };

            MainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _meetingService?.Dispose();
            _meetingViewModel?.Dispose();
            base.OnExit(e);
        }
    }
}
