using System.Windows;

namespace WPFView
{
    public partial class App : Application
    {
        private void OnApplicationLaunched(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow();
            MainWindow.Show();
        }
    }
}
