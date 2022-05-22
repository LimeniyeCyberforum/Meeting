using Grpc.Core;
using GrpcCommon;
using MeetingCommon.Abstractions;
using MeetingCommon.Abstractions.CameraCapture;
using MeetingCommon.Abstractions.Messanger;
using MeetingCommon.DataTypes;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MeetingGrpcClient.Core
{
    public class MeetingService : MeetingServiceAbstract
    {
        private readonly Meeting.MeetingClient _client;

        public MeetingService()
        {
            // Locate required files and set true to enable SSL
            var secure = false;

            if (secure)
            {
                // create secure channel
                var serverCACert = File.ReadAllText(@"C:\localhost_server.crt");
                var clientCert = File.ReadAllText(@"C:\localhost_client.crt");
                var clientKey = File.ReadAllText(@"C:\localhost_clientkey.pem");
                var keyPair = new KeyCertificatePair(clientCert, clientKey);
                //var credentials = new SslCredentials(serverCACert, keyPair);

                // Client authentication is an option. You can remove it as follows if you only need SSL.
                var credentials = new SslCredentials(serverCACert);

                _client = new Meeting.MeetingClient(
                    new Channel("localhost", 7129, credentials));
            }
            else
            {
                // create insecure channel
                _client = new Meeting.MeetingClient(
                    new Channel("localhost", 7129, ChannelCredentials.Insecure));
            }
        }

        public override UserDto Connect(string username)
        {
            var result = _client.Connect(new ConnectRequest()
            {
                Username = username
            });

            if (!result.IsSuccessfully)
                throw new ArgumentException(result.ErrorMessage);

            Guid userGuid = Guid.Parse(result.Guid);
            var user = new UserDto(userGuid, username);

            Initialize(userGuid, user);

            return user;
        }

        public override async Task<UserDto> ConnectAsync(string username)
        {
            var result = await _client.ConnectAsync(new ConnectRequest()
            {
                Username = username
            });

            if (!result.IsSuccessfully)
                throw new ArgumentException(result.ErrorMessage);

            Guid userGuid = Guid.Parse(result.Guid);
            var user = new UserDto(userGuid, username);

            Initialize(userGuid, user);

            return user;
        }

        private void Initialize(Guid currentUserGuid, UserDto user)
        {
            MessageService = new MessageService(_client);
            CameraCaptureService = new CameraCaptureService(_client, currentUserGuid);
            RaiseConnectionStateChangedAction(ConnectionAction.Connected, user);
        }
    }
}
