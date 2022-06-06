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
using System.Security.Cryptography.X509Certificates;
using System.IO;

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
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var channel = GrpcChannel.ForAddress("https://3.72.127.66:5010/", GetTemporaryGrpcChannelOptions());
            _authorizationClient = new AuthorizationClient(channel);

            Users = new UsersService(new UsersClient(channel));
            Chat = new ChatService(new ChatClient(channel));
            CaptureFrames = new CaptureFramesService(new CaptureFramesClient(channel));
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

        private GrpcChannelOptions GetTemporaryGrpcChannelOptions()
        {
            var path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"Resources\limeniye-certificate.crt");
            var cert = X509Certificate.CreateFromCertFile(path);
            var certificate = new X509Certificate2(cert);
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            handler.ClientCertificates.Add(certificate);
            HttpClient httpClient = new(handler);
            var channelOptions = new GrpcChannelOptions
            {
                HttpClient = httpClient
            };
            return channelOptions;
        }
    }
}
