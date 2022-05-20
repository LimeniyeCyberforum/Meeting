using MeetingGrpcClient;
using System.Windows;

namespace WPFView
{
    public partial class App : Application
    {
        private void OnApplicationLaunched(object sender, StartupEventArgs e)
        {
            //IocService.Initialize();
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(new MeetingService())
            };
            MainWindow.Show();
        }
    }
}
