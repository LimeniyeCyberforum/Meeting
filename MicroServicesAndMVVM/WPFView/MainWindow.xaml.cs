using Grpc;
using Grpc.Net.Client;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;

namespace WPFView
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;


            using var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions { HttpHandler = httpHandler });
            var client = new Greeter.GreeterClient(channel);

            var replay = client.SayHello(new HelloRequest() { Name = "Hello World!" } );


            Debug.WriteLine(replay.Message);
        }
    }
}
