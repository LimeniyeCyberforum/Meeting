using Framework.EventArgs;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Meeting.Business.Common.Abstractions;
using Meeting.Business.Common.DataTypes;
using Meeting.Business.GrpcClient;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using MeetingClient = MeetingGrpc.Protos.Meeting.MeetingClient;


namespace Meeting.Xamarin
{
    public partial class MainPage : ContentPage
    {
        public static MeetingServiceAbstract MeetingServiceAbstract;
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

        private bool permissionsGranted;
        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

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


            MeetingServiceAbstract = new MeetingService(new MeetingClient(ToAuthChannel(gpTest, BaseUri)));
            MeetingServiceAbstract.ConnectionStateChanged += OnConnectionStateChanged;
            _currentUser = MeetingServiceAbstract.Connect("limeniye_mobile");
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await VerifyPermissions();
        }

        private async Task<bool> VerifyPermissions()
        {
            try
            {
                PermissionStatus status = PermissionStatus.Unknown;

                // Camera permission - NOTE: Requires adding Permission to the Android Manifest and iOS pList 
                status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Camera Permission Required", "This app will proccess frames taken live from the device's camera", "OK");
                    status = await Permissions.RequestAsync<Permissions.Camera>();

                    if (status != PermissionStatus.Granted)
                        return await VerifyPermissions();
                }

                // All needed permissions granted 
                return true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "OK");
                return false;
            }
        }


        private void OnConnectionStateChanged(object sender, (ConnectionAction Action, Meeting.Business.Common.DataTypes.UserDto User) e)
        {
            MeetingServiceAbstract.MessageService.MessagesChanged += OnMessagesChanged;
            MeetingServiceAbstract.MessageService.ChatSubscribeAsync();
        }

        private void OnMessagesChanged(object sender, Framework.EventArgs.NotifyDictionaryChangedEventArgs<Guid, Meeting.Business.Common.DataTypes.MessageDto> e)
        {
            var newValue = e.NewValue;
            var oldValue = e.OldValue;

            switch (e.Action)
            {
                case NotifyDictionaryChangedAction.Added:
                    var index = Messages.IndexOf(Messages.FirstOrDefault(x => x.Id == newValue.Guid));
                    if (index > -1)
                    {
                        Messages[index] = new Message(newValue.Guid, newValue.Message, newValue.DateTime);
                    }
                    else
                    {
                        Messages.Add(new Message(newValue.Guid, newValue.Message, newValue.DateTime));
                    }
                    break;
                case NotifyDictionaryChangedAction.Initialized:
                    Messages.Clear();
                    throw new NotImplementedException();
            }
        }

        private async void OnSendClicked(object sender, EventArgs e)
        {
            var newMessage = new Message(Guid.NewGuid(), entryMessage.Text, null);
            entryMessage.Text = "";
            Messages.Add(newMessage);
            await MeetingServiceAbstract.MessageService.SendMessageAsync(newMessage.Id, _currentUser.Guid, newMessage.Text);
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
