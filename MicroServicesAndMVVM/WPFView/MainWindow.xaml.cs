using Grpc.Core;
using Grpc.Net.Client;
using GrpsServer;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WPFView
{
    public class ChatViewModel : BaseInpc
    {
        private string _message;

        public string Message { get => _message; set => Set(ref _message, value); }

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
            _call.RequestStream.WriteAsync(new HelloRequest() { Name = Message});

            //call.RequestStream.CompleteAsync();


        }

        private bool CanSendMessageExecute()
        {
            return true;
        }
        #endregion


        private AsyncDuplexStreamingCall<HelloRequest, HelloReply> _call;

        public ChatViewModel()
        {
            InitializeStream();
        }

        private async Task InitializeStream()
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;


            using var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions { HttpHandler = httpHandler });
            var client = new Greeter.GreeterClient(channel);

            var replay = client.SayHello(new HelloRequest() { Name = "limeniye" });


            _call = client.SayHelloStream();
            //= call;
            var dispatcher = Application.Current.Dispatcher;

            await Task.Run(async () =>
            {
                await foreach (var response in _call.ResponseStream.ReadAllAsync())
                {
                    if (dispatcher.CheckAccess())
                    {
                        Messages.Add(new Message(Guid.NewGuid(), response.Message, false, MessageStatus.Readed, DateTime.Now));
                    }
                    else
                    {
                        _ = dispatcher.BeginInvoke(() => Messages.Add(new Message(Guid.NewGuid(), response.Message, false, MessageStatus.Readed, DateTime.Now)));
                    }
                }
            });
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ChatViewModel();
        }
    }
}
