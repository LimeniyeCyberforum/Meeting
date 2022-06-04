using Grpc.Net.Client;
using Meeting.Business.Common.DataTypes;
using System;
using System.Threading.Tasks;

using Meeting.Business.Common.Abstractions.Authorization;
using Meeting.Business.Common.Abstractions.Users;
using Meeting.Business.Common.Abstractions.Chat;
using Meeting.Business.Common.Abstractions.FrameCapture;

using AuthorizationClient = MeetingGrpc.Protos.Authorization.AuthorizationClient;
using UsersClient = MeetingGrpc.Protos.Users.UsersClient;
using ChatClient = MeetingGrpc.Protos.Chat.ChatClient;
using FrameCapture = MeetingGrpc.Protos.FrameCapture.FrameCaptureClient;


namespace Meeting.Business.GrpcClient
{
    public class MeetingService //: MeetingServiceAbstract
    {
        //private readonly AuthorizationClient _client;
        //private readonly GrpcChannel _channel;

        public AuthorizationServiceAbstract Authorization { get; }
        public UsersServiceAbstract Users { get; }
        public ChatServiceAbstract Chat { get; }
        public FrameCaptureServiceAbstract FrameCaptures { get; }

        public MeetingService()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5010/"/*, channelOptions*/);

            var authClient = new AuthorizationClient(channel);
            var users = new UsersClient(channel);
            var chatClient = new ChatClient(channel);
            var frameCaptures = new FrameCapture(channel);


           

            var reply = chatClient.SendMessage(new MessageRequest { Message = "Hello world!", MessageGuid = Guid.NewGuid().ToString() }, metadata);
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
