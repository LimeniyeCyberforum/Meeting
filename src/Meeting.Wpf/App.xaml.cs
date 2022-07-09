using Meeting.Core.Common;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Regions;
using System.Windows;

namespace Meeting.Wpf
{
    public partial class App : PrismApplication
    {
        private IMeetingService? _meetingService;

        protected override void OnExit(ExitEventArgs e)
        {
            _meetingService?.Dispose();
            base.OnExit(e);
        }

        public void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            _meetingService?.Dispose();
            MessageBox.Show("Unhandled exception occurred: \n" + e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MeetingWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.ConfigurationServices();
        }

        protected override void OnInitialized()
        {
            _meetingService = Container.Resolve<IMeetingService>();
            _meetingService.Chat.ChatSubscribeAsync();
            _meetingService.Users.UsersSubscribeAsync();
            _meetingService.CaptureFrames.CaptureFrameAreasSubscribeAsync();
            _meetingService.CaptureFrames.CaptureFramesSubscribeAsync();
            (App.Current.MainWindow.DataContext as INavigationAware)?.OnNavigatedTo(null);
            base.OnInitialized();
        }
    }
}
