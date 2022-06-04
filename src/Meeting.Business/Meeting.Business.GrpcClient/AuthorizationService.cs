using Grpc.Core;
using Meeting.Business.Common.Abstractions.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AuthorizationClient = MeetingGrpc.Protos.Authorization.AuthorizationClient;


namespace Meeting.Business.GrpcClient
{
    public class AuthorizationService : AuthorizationServiceAbstract
    {
        private readonly AuthorizationClient _authorizationClient;

        public Metadata Metadata

        public AuthorizationService(AuthorizationClient auth)
            :base()
        {
            _authorizationClient = auth;
        }

        public override void JoinToLobby(string username)
        {
            var authReply = _authorizationClient.Connect(new MeetingGrpc.Protos.ConnectRequest { Username = username });
            var metadata = new Metadata();
            metadata.Add("Authorization", $"Bearer {authReply.JwtToken}");

            CurrentUser = new Common.DataTypes.UserDto(Guid.Parse(authReply.UserGuid), username);
            CurrentConnectionState = UserConnectionState.Connected;
            RaiseAuthorizationStateChangedEvent(UserConnectionState.Connected);
        }

        public override async Task JoinToLobbyAsync(string username)
        {
            var authReply = await _authorizationClient.ConnectAsync(new MeetingGrpc.Protos.ConnectRequest { Username = username });
            var metadata = new Metadata();
            metadata.Add("Authorization", $"Bearer {authReply.JwtToken}");

            CurrentUser = new Common.DataTypes.UserDto(Guid.Parse(authReply.UserGuid), username);
            CurrentConnectionState = UserConnectionState.Connected;
            RaiseAuthorizationStateChangedEvent(UserConnectionState.Connected);
        }
    }
}
