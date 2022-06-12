using System;
using Grpc.Core;
using Grpc.Net.Client;
using System.Threading.Tasks;
using Meeting.Business.Common.DataTypes;

using Meeting.Business.Common.Abstractions;
using Meeting.Business.Common.Abstractions.Chat;
using Meeting.Business.Common.Abstractions.Users;
using Meeting.Business.Common.Abstractions.FrameCapture;

using ChatClient = MeetingGrpc.Protos.Chat.ChatClient;
using UsersClient = MeetingGrpc.Protos.Users.UsersClient;
using CaptureFramesClient = MeetingGrpc.Protos.CaptureFrames.CaptureFramesClient;
using AuthorizationClient = MeetingGrpc.Protos.Authorization.AuthorizationClient;
using System.Net.Http;
using Xamarin.Essentials;
using Grpc.Net.Client.Web;

namespace Meeting.Business.GrpcClient
{
    public class MeetingService : IMeetingService
    {
        private readonly AuthorizationClient _authorizationClient;

        public UserDto CurrentUser { get; private set; }

        public UserConnectionState CurrentConnectionState { get; private set; }

        public ChatServiceAbstract Chat { get; }

        public CaptureFramesServiceAbstract CaptureFrames { get; }

        public UsersServiceAbstract Users { get; }

        public event EventHandler<UserConnectionState> AuthorizationStateChanged;

        public MeetingService()
        {
            var channel = GetGrpcChannel();
            _authorizationClient = new AuthorizationClient(channel);
            Users = new UsersService(new UsersClient(channel));
            Chat = new ChatService(new ChatClient(channel));
            CaptureFrames = new CaptureFramesService(new CaptureFramesClient(channel), Users);
        }

        public void JoinToLobby(string username)
        {
            var authReply = _authorizationClient.Connect(new MeetingGrpc.Protos.ConnectRequest { Username = username });
            var metadata = new Metadata();
            metadata.Add("Authorization", $"Bearer {authReply.JwtToken}");
            UpdateMetadata(metadata);

            CurrentUser = new UserDto(Guid.Parse(authReply.UserGuid), username);
            CurrentConnectionState = UserConnectionState.Connected;
            RaiseAuthorizationStateChangedEvent(UserConnectionState.Connected);
        }

        public async Task JoinToLobbyAsync(string username)
        {
            var authReply = await _authorizationClient.ConnectAsync(new MeetingGrpc.Protos.ConnectRequest { Username = username });
            var metadata = new Metadata();
            metadata.Add("Authorization", $"Bearer {authReply.JwtToken}");
            UpdateMetadata(metadata);

            CurrentUser = new UserDto(Guid.Parse(authReply.UserGuid), username);
            CurrentConnectionState = UserConnectionState.Connected;
            RaiseAuthorizationStateChangedEvent(UserConnectionState.Connected);
        }

        public bool IsNameExists(string username)
        {
            return _authorizationClient.IsNameExists(new MeetingGrpc.Protos.CheckNameRequest { Username = username }).IsExists;
        }

        public async Task<bool> IsNameExistsAsync(string username)
        {
            var response = await _authorizationClient.IsNameExistsAsync(new MeetingGrpc.Protos.CheckNameRequest { Username = username });
            return response.IsExists;
        }

        protected void UpdateMetadata(Metadata metadata)
        {
            ((ChatService)Chat).UpdateMetadata(metadata);
            ((CaptureFramesService)CaptureFrames).UpdateMetadata(metadata);
        }

        protected void RaiseAuthorizationStateChangedEvent(UserConnectionState newState)
        {
            AuthorizationStateChanged?.Invoke(this, newState);
        }

        private string GetServerAddress()
        {
            var address = "https://3.72.127.66:5010";
            if (false) // Hot switcher
            {
                address = DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:5010" : "https://localhost:5010";
            }
            return address;
        }

        private GrpcChannel GetGrpcChannel()
        {
            string address = GetServerAddress();

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var httpClientHandler = new HttpClientHandler();

            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };

            HttpClient httpClient = null;

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                var grpcWebHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, httpClientHandler);
                httpClient = new HttpClient(grpcWebHandler);
            }
            else
            {
                httpClient = new(httpClientHandler);
            }

            return GrpcChannel.ForAddress(address, new GrpcChannelOptions
            {
                HttpClient = httpClient,
                Credentials = ChannelCredentials.SecureSsl
            });
        }
    }
}
