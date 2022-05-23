using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using GrpcCommon;
using MeetingCommon.Abstractions;
using MeetingCommon.DataTypes;
using MeetingGrpcClient;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MeetingXamarin
{
    public partial class MainPage : ContentPage
    {
        private readonly MeetingServiceAbstract _meetingServiceAbstract;
        private UserDto _currentUser;

        private string _message = "Kek";
        public string Message { get => _message; set => SetPropertyValue(ref _message, value); }

        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>()
        {
            new Message(Guid.NewGuid(), "Hello world!", DateTime.UtcNow)
        };

        public static string IPAddress = DeviceInfo.Platform == DevicePlatform.Android
        ? "10.0.2.2"
		: "localhost";

        public static string BaseUri = $"https://{IPAddress}:7129";

        public MainPage()
        {
            InitializeComponent();

            //android:networkSecurityConfig="@network_security_config"

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            GrpcChannel ToAuthChannel(HttpClient httpClient, string baseUri) =>
                GrpcChannel.ForAddress(baseUri,
                    new GrpcChannelOptions
                    {
                        HttpClient = httpClient,
                        Credentials = ChannelCredentials.SecureSsl
                    });


            var httpClientHandler = new HttpClientHandler();

            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };

            var grpcChannel = GrpcChannel.ForAddress(BaseUri, new GrpcChannelOptions { HttpHandler = httpClientHandler });


            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, httpClientHandler);
            var gpTest = new HttpClient(handler);


            _meetingServiceAbstract = new MeetingService(new Meeting.MeetingClient(ToAuthChannel(gpTest, BaseUri)));
            _currentUser = _meetingServiceAbstract.Connect("limeniye_mobile");
        }

        private void OnConnectionStateChanged(object sender, (ConnectionAction Action, MeetingCommon.DataTypes.UserDto User) e)
        {
            _meetingServiceAbstract.MessageService.MessagesChanged += OnMessagesChanged;
            _meetingServiceAbstract.MessageService.ChatSubscribeAsync();
        }

        private void OnMessagesChanged(object sender, Common.EventArgs.NotifyDictionaryChangedEventArgs<Guid, MeetingCommon.DataTypes.MessageDto> e)
        {

        }

        private async void OnSendClicked(object sender, EventArgs e)
        {
            var newMessage = new Message(Guid.NewGuid(), Message, null);
            Message = "";
            Messages.Add(newMessage);
            await _meetingServiceAbstract.MessageService.SendMessageAsync(newMessage.Id, _currentUser.Guid, newMessage.Text);
        }

        #region INPC
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetPropertyValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (value == null ? field != null : !value.Equals(field))
            {
                field = value;

                var handler = this.PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
                return true;
            }
            return false;
        }
        #endregion
    }

    public class Message
    {
        public Guid Id { get; }
        public string Text { get; }
        public DateTime? DateTime { get; }

        public Message(Guid id, string text, DateTime? dateTime)
        {
            Id = id;
            Text = text;
            DateTime = dateTime;
        }
    }

}
