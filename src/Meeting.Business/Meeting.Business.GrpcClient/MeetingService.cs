using Grpc.Core;
using Meeting.Business.Common.Abstractions;
using Meeting.Business.Common.DataTypes;
using MeetingGrpc.Protos;
using System;
using System.IO;
using System.Threading.Tasks;
using AuthorizationClient = MeetingGrpc.Protos.Authorization.AuthorizationClient;

namespace Meeting.Business.GrpcClient
{
    public class MeetingService : MeetingServiceAbstract
    {
        private readonly AuthorizationClient _client;
        //private readonly GrpcChannel _channel;

        public MeetingService(AuthorizationClient client)
        {
            _client = client;
        }

        public override UserDto Connect(string username)
        {
            //var result = _client.Connect(new ConnectRequest()
            //{
            //    Username = username
            //});

            //if (!result.IsSuccessfully)
            //    throw new ArgumentException(result.ErrorMessage);

            //Guid userGuid = Guid.Parse(result.Guid);
            //var user = new UserDto(userGuid, username);

            //Initialize(userGuid, user);

            //return user;

            return null;
        }

        public override async Task<UserDto> ConnectAsync(string username)
        {
            //var result = await _client.ConnectAsync(new ConnectRequest()
            //{
            //    Username = username
            //});

            //if (!result.IsSuccessfully)
            //    throw new ArgumentException(result.ErrorMessage);

            //Guid userGuid = Guid.Parse(result.Guid);
            //var user = new UserDto(userGuid, username);

            //Initialize(userGuid, user);

            //return user;

            return null;
        }

        private void Initialize(Guid currentUserGuid, UserDto user)
        {
            //MessageService = new MessageService(_client);
            //CameraCaptureService = new CameraCaptureService(_client, currentUserGuid);
            RaiseConnectionStateChangedAction(ConnectionAction.Connected, user);
        }
    }
}
