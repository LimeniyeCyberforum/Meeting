using Grpc.Core;
using Grpc.Net.Client;
using GrpsServer;
using MeetingRepository.Abstractions.Interfaces.Messanger;
using MeetingRepository.Abstractions.Messanger;
using MeetingRepository.DataTypes.Messanger;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MeetingRepository.Grpc.Messanger
{
    internal class MessageService : BaseMessageServiceAbstract, IMessageService
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

            await Task.Run(async () => 
            {
                await foreach (var response in _call.ResponseStream.ReadAllAsync())
                {
                    RaiseMessagesChangedEvent(Common.EventArgs.NotifyDictionaryChangedAction.Added, new MessageDto(Guid.NewGuid(), response.Message, response.Username, response.Time.ToDateTime()));
                }
            });
        }
    }
}
