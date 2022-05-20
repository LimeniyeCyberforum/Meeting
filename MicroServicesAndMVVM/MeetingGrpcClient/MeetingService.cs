using Grpc.Core;
using GrpcCommon;
using MeetingCommon.Abstractions;
using MeetingCommon.Abstractions.CameraCapture;
using MeetingCommon.Abstractions.Messanger;
using MeetingCommon.DataTypes;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MeetingGrpcClient
{
    public class MeetingService : MeetingServiceAbstract
    {
        private readonly Meeting.MeetingClient _client;

        public MeetingService()
        {
            var secure = false;

            if (secure)
            {
                //var httpHandler = new HttpClientHandler();

                // Here you can disable validation for server certificate for your easy local test
                // See https://docs.microsoft.com/en-us/aspnet/core/grpc/troubleshoot#call-a-grpc-service-with-an-untrustedinvalid-certificate
                //httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                //_client = new Meeting.MeetingClient(GrpcChannel.ForAddress("https://localhost:50052", new GrpcChannelOptions { HttpHandler = httpHandler }));

                //    var httpHandler = new HttpClientHandler();
                //    httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                //    using var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions { HttpHandler = httpHandler });
                //    _client = new GrpsServer.Messanger.MessangerClient(channel);


                var serverCACert = File.ReadAllText(@"C:\localhost_server.crt");
                var clientCert = File.ReadAllText(@"C:\localhost_client.crt");
                var clientKey = File.ReadAllText(@"C:\localhost_clientkey.pem");
                var keyPair = new KeyCertificatePair(clientCert, clientKey);
                //var credentials = new SslCredentials(serverCACert, keyPair);

                // Client authentication is an option. You can remove it as follows if you only need SSL.
                var credentials = new SslCredentials(serverCACert);

                _client = new Meeting.MeetingClient(
                    new Channel("localhost", 5001, credentials));


            }
            else
            {
                // create insecure channel
                _client = new Meeting.MeetingClient(
                    new Channel("localhost", 5001, ChannelCredentials.Insecure));
            }
        }

        public override UserDto Connect(string username)
        {
            throw new NotImplementedException();
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

            Initialize(userGuid);

            return new UserDto(userGuid, username);
        }

        private void Initialize(Guid currentUserGuid)
        {
            MessageService = new MessageService(_client);
            CameraCaptureService = new CameraCaptureService(_client, currentUserGuid);
            RaiseConnectionStateChangedAction(ConnectionAction.Connected);
        }
    }
}
