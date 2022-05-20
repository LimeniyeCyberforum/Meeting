using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcCommon;
using MeetingCommon.Abstractions.Messanger;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MeetingGrpcClient
{
    public class MessageService : MessageServiceAbstract
    {
       private readonly Meeting.MeetingClient _client;


        public MessageService()
            : base()
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

        public override Task ChatSubscribeAsync()
        {
            throw new NotImplementedException();
        }

        public override Task ChatUnsubscribeAsync()
        {
            throw new NotImplementedException();
        }

        public override Task SendMessageAsync(Guid messageGuid, Guid userGuid, string message)
        {
            throw new NotImplementedException();
        }

        public override Task SendOwnCameraCaptureAsync(MemoryStream stream)
        {
            throw new NotImplementedException();
        }

        public override Task UsersCameraCaptureSubscribeAsync()
        {
            throw new NotImplementedException();
        }

        public override Task UsersCameraCaptureUnsubscribeAsync()
        {
            throw new NotImplementedException();
        }



        //private AsyncDuplexStreamingCall<MessageRequest, MessageReplay> _call;
        //private AsyncDuplexStreamingCall<CameraCaptureTest, CameraCaptureTest> _call2;
        //private GrpsServer.Messanger.MessangerClient _client;

        //public override void SendMessage(Guid guid, string username, string message)
        //{
        //    throw new NotImplementedException();
        //}

        //public override async Task SendMessageAsync(Guid guid, string username, string message)
        //{
        //    var response = await _client.SendMessageAsync(new MessageRequest() { Username = username, Message = message });
        //}

        //public override async Task SendCameraCaptureAsync(MemoryStream stream)
        //{
        //    try
        //    {
        //        await _call2.RequestStream.WriteAsync(new CameraCaptureTest() { CaptureFrame = ByteString.FromStream(stream) });
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    //await _call.RequestStream.WriteAsync(new MessageRequest() { Username = "limeniye", Message = "fasf" });
        //}

        //private void StreamComplete()
        //{
        //    _call.RequestStream.CompleteAsync();
        //}



        //private async void InitializeStream()
        //{
        //    var httpHandler = new HttpClientHandler();
        //    httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        //    using var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions { HttpHandler = httpHandler });
        //    _client = new GrpsServer.Messanger.MessangerClient(channel);
        //    _call = _client.MessageStream();
        //    _call2 = _client.CameraCaptureStream();

        //    await Task.Run(async () =>
        //    {
        //        await foreach (var response in _call2.ResponseStream.ReadAllAsync())
        //        {
        //            RaiseCameraCaptureChanged(response.CaptureFrame.ToByteArray());
        //            //System.Diagnostics.Debug.WriteLine(response.CaptureFrame);
        //        }

        //        //await foreach (var response in _call.ResponseStream.ReadAllAsync())
        //        //{
        //        //    RaiseMessagesChangedEvent(Common.EventArgs.NotifyDictionaryChangedAction.Added, new MessageDto(Guid.NewGuid(), response.Message, response.Username, response.Time.ToDateTime()));
        //        //}
        //    });
        //}
    }
}
