using Meeting.Business.GrpcClient;
using Meeting.WPF.Windows;
using System.Windows;

namespace Meeting.WPF
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
