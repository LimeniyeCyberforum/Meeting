using Grpc.Core;
using Grpc.Net.Client;
using MeetingGrpc.Protos;
using System;
using System.Windows;
using AuthorizationClient = MeetingGrpc.Protos.Authorization.AuthorizationClient;
using ChatClient = MeetingGrpc.Protos.Chat.ChatClient;


namespace Meeting.WPF
{
    public partial class App : Application
    {
        private void OnApplicationLaunched(object sender, StartupEventArgs e)
        {
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            //var cert = X509Certificate.CreateFromCertFile("C:\\limeniye-certificate.crt");
            //var certificate = new X509Certificate2(cert);

            //var handler = new HttpClientHandler();
            //handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            //handler.ClientCertificates.Add(certificate);

            //HttpClient httpClient = new(handler);

            //var channelOptions = new GrpcChannelOptions
            //{
            //    HttpClient = httpClient
            //};
            //var channel = GrpcChannel.ForAddress("https://3.72.127.66:5010/", channelOptions);
            var channel = GrpcChannel.ForAddress("https://localhost:5010/"/*, channelOptions*/);

            var authClient = new AuthorizationClient(channel);
            var chatClient = new ChatClient(channel);
            var authReply = authClient.Connect(new MeetingGrpc.Protos.ConnectRequest { Username = "asf" });
            var metadata = new Metadata();
            metadata.Add("Authorization", $"Bearer {authReply.JwtToken}");

            var reply = chatClient.SendMessage(new MessageRequest { Message = "Hello world!", MessageGuid = Guid.NewGuid().ToString() }, metadata);

            MainWindow = new MainWindow()
            {
                //DataContext = new MainViewModel(new MeetingService(new AuthorizationClient(channel)))
            };
            MainWindow.Show();
        }
    }
}
