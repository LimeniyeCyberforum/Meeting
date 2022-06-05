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
using FrameCapture = MeetingGrpc.Protos.FrameCapture.FrameCaptureClient;
using AuthorizationClient = MeetingGrpc.Protos.Authorization.AuthorizationClient;

namespace Meeting.Business.GrpcClient
{
    public sealed class MeetingService : MeetingServiceAbstract
    {
        private readonly AuthorizationClient _authorizationClient;

        public new UsersServiceAbstract Users { get; }
        public new ChatServiceAbstract Chat { get; }
        public new CaptureFrameServiceAbstract FrameCaptures { get; }

        public MeetingService()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5010/"/*, channelOptions*/);
            _authorizationClient = new AuthorizationClient(channel);

            Users = new UsersService(new UsersClient(channel));
            Chat = new ChatService(new ChatClient(channel));
            FrameCaptures = new FrameCaptureService(new FrameCapture(channel));
        }

        public override void JoinToLobby(string username)
        {
            var authReply = _authorizationClient.Connect(new MeetingGrpc.Protos.ConnectRequest { Username = username });
            var metadata = new Metadata();
            metadata.Add("Authorization", $"Bearer {authReply.JwtToken}");
            UpdateMetadata(metadata);

            CurrentUser = new UserDto(Guid.Parse(authReply.UserGuid), username);
            CurrentConnectionState = UserConnectionState.Connected;
            RaiseAuthorizationStateChangedEvent(UserConnectionState.Connected);
        }

        public override async Task JoinToLobbyAsync(string username)
        {
            var authReply = await _authorizationClient.ConnectAsync(new MeetingGrpc.Protos.ConnectRequest { Username = username });
            var metadata = new Metadata();
            metadata.Add("Authorization", $"Bearer {authReply.JwtToken}");
            UpdateMetadata(metadata);

            CurrentUser = new UserDto(Guid.Parse(authReply.UserGuid), username);
            CurrentConnectionState = UserConnectionState.Connected;
            RaiseAuthorizationStateChangedEvent(UserConnectionState.Connected);
        }

        private void UpdateMetadata(Metadata metadata)
        {
            ((ChatService)Chat).UpdateMetadata(metadata);
            ((FrameCaptureService)FrameCaptures).UpdateMetadata(metadata);
        }
    }
}
