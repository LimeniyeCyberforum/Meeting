using Grpc.Core;
using Grpc.Net.Client;
using GrpsServer;
using Meeting.Abstractions.Interfaces.Messanger;
using Meeting.Abstractions.Messanger;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Meeting.Grpc.Messanger
{
    internal class MessageServiceGrpc : MessageServiceAbstract, IMessageService
    {
        private AsyncDuplexStreamingCall<MessageRequest, MessageReplay> _call;
        private GrpsServer.Messanger.MessangerClient _client;

        public override void SendMessage(Guid guid, string username, string message)
        {
            throw new NotImplementedException();
        }

        public override async Task SendMessageAsync(Guid guid, string username, string message)
        {
            var response = await _client.SendMessageAsync(new MessageRequest() { Username = username, Message = message });
        }

        private void StreamComplete()
        {
            _call.RequestStream.CompleteAsync();
        }

        public MessageServiceGrpc()
            :base()
        {

        }

        private async Task InitializeStream()
        {
            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            using var channel = GrpcChannel.ForAddress("https://localhost:5001", new GrpcChannelOptions { HttpHandler = httpHandler });
            _client = new GrpsServer.Messanger.MessangerClient(channel);
            _call = _client.MessageStream();

            await Task.Run(async () =>
            {
                await foreach (var response in _call.ResponseStream.ReadAllAsync())
                {
                    _ = dispatcher.BeginInvoke(() => Messages.Add(new Message(Guid.NewGuid(), response.Message, false, false, MessageStatus.Readed, response.Time.ToDateTime())));
                }
            });
        }
    }
}
