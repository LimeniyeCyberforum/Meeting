using Grpc.Net.Client;
using GrpcCommon;
using MeetingGrpcClient;
using System.Net.Http;
using System.Windows;

namespace WPFView
{
    public partial class App : Application
    {
        private void OnApplicationLaunched(object sender, StartupEventArgs e)
        {
            //IocService.Initialize();

            var channel = GrpcChannel.ForAddress("https://localhost:7129/", new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler()
            });


            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(new MeetingService(new Meeting.MeetingClient(channel)))
            };
            MainWindow.Show();
        }
    }
}
