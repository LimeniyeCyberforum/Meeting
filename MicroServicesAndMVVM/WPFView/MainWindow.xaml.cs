using Grpc.Core;
using Grpc.Net.Client;
using GrpsServer;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WPFView
{
    public class MessageItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OwnerMessage { get; set; }
        public DataTemplate AnotherMessage { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is bool isOwner)
            {
                return SelectTemplateCore(isOwner);
            }

            throw new System.NotImplementedException($"Uknown DataTemplate parameter. \nType: {nameof(MessageItemDataTemplateSelector)}\nParametr: {item}");
        }

        private DataTemplate SelectTemplateCore(bool isOwner)
        {
            if (isOwner)
            {
                return OwnerMessage;
            }
            else
            {
                return AnotherMessage;
            }
        }
    }

    public class ChatViewModel : BaseInpc
    {
        private readonly Dispatcher dispatcher = Application.Current.Dispatcher;
        private AsyncDuplexStreamingCall<MessageRequest, MessageReplay> _call;
        private Messanger.MessangerClient? _client;


        private string _message;

        public string Message { get => _message; set => Set(ref _message, value); }

        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>()
        {
            new Message(Guid.NewGuid(), "Hello world", false, true, MessageStatus.Readed, DateTime.Now)
        };

        #region SendMessageCommand
        private RelayCommandAsync _sendMessageCommand;
        public RelayCommandAsync SendMessageCommand => _sendMessageCommand ?? (
            _sendMessageCommand = new RelayCommandAsync(OnSendMessageExecute, CanSendMessageExecute));

        private async void OnSendMessageExecute()
        {
            //await _call.RequestStream.WriteAsync(new MessageRequest() { Username = "limeniye", Message = Message });
            var newMessage = new Message(Guid.NewGuid(), Message, false, false, MessageStatus.Readed, null);
            Message = String.Empty;

            _ = dispatcher.BeginInvoke(() => Messages.Add(newMessage));

            var response = await _client.SendMessageAsync(new MessageRequest() { Username = "limeniye", Message = Message });

            var messageIndex = Messages.IndexOf(newMessage);
            if (messageIndex > -1)
            {
                _ = dispatcher.BeginInvoke(() => Messages[messageIndex] =
                       new Message(Guid.NewGuid(), Message, false, false, MessageStatus.Readed, response.Time.ToDateTime()));
            }
        }

        private void StreamComplete()
        {
            _call.RequestStream.CompleteAsync();
        }

        private bool CanSendMessageExecute()
        {
            return string.IsNullOrEmpty(Message) || Message == "" ? false : true;
        }
        #endregion

        public ChatViewModel()
        {
            InitializeStream();
        }

        private async Task InitializeStream()
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            using var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions { HttpHandler = httpHandler });
            _client = new Messanger.MessangerClient(channel);
            _call = _client.MessageStream();

            await Task.Run(async () =>
            {
                await foreach (var response in _call.ResponseStream.ReadAllAsync())
                {
                    _ = dispatcher.BeginInvoke(() => Messages.Add(new Message(Guid.NewGuid(), response.Message, false, false, MessageStatus.Readed, response.Time.ToDateTime())));
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
