using System;
using System.Threading.Tasks;
using Grpc.Core;
using Meeting.Core.Common;
using Meeting.Core.Common.DataTypes;
using AuthorizationClient = MeetingProtobuf.Protos.Authorization.AuthorizationClient;

namespace Meeting.Core.GrpcClient
{
    internal sealed class AuthorizationService : IAuthorizationService
    {
        private readonly MetadataRepository _metadataRepos;

        private AuthorizationClient _authorizationClient;

        public UserConnectionState CurrentConnectionState { get; private set; }

        public UserDto CurrentUser { get; private set; }

        public event EventHandler<UserConnectionState> AuthorizationStateChanged;

        public AuthorizationService(AuthorizationClient authorizationClient, MetadataRepository metadataRepos)
        {
            _authorizationClient = authorizationClient;

            _metadataRepos = metadataRepos;
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

        public void JoinToLobby(string username)
        {
            var authReply = _authorizationClient.Connect(new MeetingProtobuf.Protos.ConnectRequest { Username = username });
            var metadata = new Metadata();
            metadata.Add("Authorization", $"Bearer {authReply.JwtToken}");
            _metadataRepos.SetMetadata(metadata);

            CurrentUser = new UserDto(Guid.Parse(authReply.UserGuid), username);
            CurrentConnectionState = UserConnectionState.Connected;
            AuthorizationStateChanged?.Invoke(this, UserConnectionState.Connected);
        }

        public async Task JoinToLobbyAsync(string username)
        {
            var authReply = await _authorizationClient.ConnectAsync(new MeetingProtobuf.Protos.ConnectRequest { Username = username });
            var metadata = new Metadata();
            metadata.Add("Authorization", $"Bearer {authReply.JwtToken}");
            _metadataRepos.SetMetadata(metadata);

            CurrentUser = new UserDto(Guid.Parse(authReply.UserGuid), username);
            CurrentConnectionState = UserConnectionState.Connected;
            AuthorizationStateChanged?.Invoke(this, UserConnectionState.Connected);
        }
    }
}
