using Grpc.Net.Client;
using GrpsServer;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;

namespace WPFView
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>()
        {
            new Message(Guid.NewGuid(), "Hello world", false, MessageStatus.Readed, DateTime.Now)
        };

        #region SendMessageCommand
        private RelayCommand _sendMessageCommand;
        public RelayCommand SendMessageCommand => _sendMessageCommand ?? (
            _sendMessageCommand = new RelayCommand(OnSendMessageExecute, CanSendMessageExecute));

        private void OnSendMessageExecute()
        {

        }

        private bool CanSendMessageExecute()
        {
            return true;
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;


            using var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions { HttpHandler = httpHandler });
            var client = new Greeter.GreeterClient(channel);

            var replay = client.SayHello(new HelloRequest() { Name = "Hello World!" });


            Debug.WriteLine(replay.Message);
        }
    }
}
