using Grpc.Net.Client;
using GrpcCommon;
using MeetingGrpcClient;
using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace WPFView
{
    public partial class App : Application
    {
        private void OnApplicationLaunched(object sender, StartupEventArgs e)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var cert = X509Certificate.CreateFromCertFile("C:\\limeniye-certificate.crt");
            var certificate = new X509Certificate2(cert);

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            handler.ClientCertificates.Add(certificate);

            HttpClient httpClient = new(handler);

            var channelOptions = new GrpcChannelOptions
            {
                HttpClient = httpClient
            };
            var channel = GrpcChannel.ForAddress("https://3.72.127.66:5010/", channelOptions);

            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(new MeetingService(new Meeting.MeetingClient(channel)))
            };
            MainWindow.Show();
        }
    }
}
