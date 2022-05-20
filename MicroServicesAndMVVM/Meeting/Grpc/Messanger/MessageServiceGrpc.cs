using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using GrpsServer;
using MeetingCommon.Abstractions.Interfaces.Messanger;
using MeetingCommon.Abstractions.Messanger;
using MeetingCommon.DataTypes.Messanger;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MeetingCommon.Grpc.Messanger
{
    public class MessageService : BaseMessageServiceAbstract, IMessageService
    {
        private AsyncDuplexStreamingCall<MessageRequest, MessageReplay> _call;
        private AsyncDuplexStreamingCall<CameraCaptureTest, CameraCaptureTest> _call2;
        private GrpsServer.Messanger.MessangerClient _client;

        public override void SendMessage(Guid guid, string username, string message)
        {
            throw new NotImplementedException();
        }

        public override async Task SendMessageAsync(Guid guid, string username, string message)
        {
            var response = await _client.SendMessageAsync(new MessageRequest() { Username = username, Message = message });
        }

        public override async Task SendCameraCaptureAsync(MemoryStream stream)
        {
            try
            {
                await _call2.RequestStream.WriteAsync(new CameraCaptureTest() { CaptureFrame = ByteString.FromStream(stream) });
            }
            catch (Exception ex)
            {

            }
            //await _call.RequestStream.WriteAsync(new MessageRequest() { Username = "limeniye", Message = "fasf" });
        }

        private void StreamComplete()
        {
            _call.RequestStream.CompleteAsync();
        }

        public MessageService()
            :base()
        {
            InitializeStream();
        }

        private async void InitializeStream()
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            using var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions { HttpHandler = httpHandler });
            _client = new GrpsServer.Messanger.MessangerClient(channel);
            _call = _client.MessageStream();
            _call2 = _client.CameraCaptureStream();

            await Task.Run(async () => 
            {
                await foreach (var response in _call2.ResponseStream.ReadAllAsync())
                {
                    RaiseCameraCaptureChanged(response.CaptureFrame.ToByteArray());
                    //System.Diagnostics.Debug.WriteLine(response.CaptureFrame);
                }

                //await foreach (var response in _call.ResponseStream.ReadAllAsync())
                //{
                //    RaiseMessagesChangedEvent(Common.EventArgs.NotifyDictionaryChangedAction.Added, new MessageDto(Guid.NewGuid(), response.Message, response.Username, response.Time.ToDateTime()));
                //}
            });
        }
    }
}
