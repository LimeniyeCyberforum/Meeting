using Meeting.Business.GrpcClient;
using Meeting.Wpf.Windows;
using System.Windows;

namespace Meeting.Wpf
{
    public partial class App : Application
    {
        private void OnApplicationLaunched(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(new MeetingService())
            };
            MainWindow.Show();
        }
    }
}
