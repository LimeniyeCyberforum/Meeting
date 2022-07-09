using System;
using Grpc.Core;
using Grpc.Net.Client;
using System.Threading.Tasks;
using Meeting.Core.Common.DataTypes;

using System.Net.Http;
using Xamarin.Essentials;
using Grpc.Net.Client.Web;
using Meeting.Core.Common;
using AuthorizationClient = MeetingProtobuf.Protos.Authorization.AuthorizationClient;

namespace Meeting.Core.GrpcClient
{
    public sealed partial class MeetingService : IMeetingService
    {
        private bool disposed = false;

        private AuthorizationClient _authorizationClient;

        public UserDto CurrentUser { get; private set; }

        public UserConnectionState CurrentConnectionState { get; private set; }

        public IChatService Chat { get; private set; }

        public ICaptureFramesService CaptureFrames { get; private set; }

        public IUsersService Users { get; private set; }

        public event EventHandler<UserConnectionState> AuthorizationStateChanged;

        public void JoinToLobby(string username)
        {
            var authReply = _authorizationClient.Connect(new MeetingProtobuf.Protos.ConnectRequest { Username = username });
            var metadata = new Metadata();
            metadata.Add("Authorization", $"Bearer {authReply.JwtToken}");
            UpdateMetadata(metadata);

            CurrentUser = new UserDto(Guid.Parse(authReply.UserGuid), username);
            CurrentConnectionState = UserConnectionState.Connected;
            RaiseAuthorizationStateChangedEvent(UserConnectionState.Connected);
        }

        public async Task JoinToLobbyAsync(string username)
        {
            var authReply = await _authorizationClient.ConnectAsync(new MeetingProtobuf.Protos.ConnectRequest { Username = username });
            var metadata = new Metadata();
            metadata.Add("Authorization", $"Bearer {authReply.JwtToken}");
            UpdateMetadata(metadata);

            CurrentUser = new UserDto(Guid.Parse(authReply.UserGuid), username);
            CurrentConnectionState = UserConnectionState.Connected;
            RaiseAuthorizationStateChangedEvent(UserConnectionState.Connected);
        }

        public bool IsNameExists(string username)
        {
            return _authorizationClient.IsNameExists(new MeetingProtobuf.Protos.CheckNameRequest { Username = username }).IsExists;
        }

        public async Task<bool> IsNameExistsAsync(string username)
        {
            var response = await _authorizationClient.IsNameExistsAsync(new MeetingProtobuf.Protos.CheckNameRequest { Username = username });
            return response.IsExists;
        }

        private void UpdateMetadata(Metadata metadata)
        {
            ((ChatService)Chat).UpdateMetadata(metadata);
            ((CaptureFramesService)CaptureFrames).UpdateMetadata(metadata);
        }

        private void RaiseAuthorizationStateChangedEvent(UserConnectionState newState)
        {
            AuthorizationStateChanged?.Invoke(this, newState);
        }

        private string GetServerAddress()
        {
            var address = "https://3.72.127.66:5010";

#if UseLocalConnect
            address = DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:5010" : "https://localhost:5010";
#endif
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


        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Chat.Dispose();
                CaptureFrames.Dispose();
                Users.Dispose();
            }
            disposed = true;
        }

        ~MeetingService()
        {
            Dispose(disposing: false);
        }
    }
}
