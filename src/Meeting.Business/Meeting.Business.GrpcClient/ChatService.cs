using Framework.EventArgs;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Meeting.Business.Common.Abstractions.Chat;
using Meeting.Business.Common.DataTypes;
using MeetingGrpc.Protos;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatClient = MeetingGrpc.Protos.Chat.ChatClient;

namespace Meeting.Business.GrpcClient
{
    public class ChatService : ChatServiceAbstract
    {
        private readonly CancellationTokenSource chatCancelationToken = new CancellationTokenSource();

        private readonly ChatClient _client;

        private Metadata _metadata;

        public ChatService(ChatClient client)
            : base()
        {
            _client = client;
        }

        public void UpdateMetadata(Metadata newMetadata)
        {
            _metadata = newMetadata;
        }

        public override Task ChatSubscribeAsync()
        {
            var call = _client.MessagesSubscribe(new Empty());

            return call.ResponseStream
                .ToAsyncEnumerable()
                .Finally(() => call.Dispose())
                .ForEachAsync((x) =>
                {
                    MessageDto message = new(Guid.Parse(x.LobbyMessage.MessageGuid), Guid.Parse(x.LobbyMessage.UserGuid), x.LobbyMessage.Username, x.LobbyMessage.MessageGuid, x.LobbyMessage.Time.ToDateTime());
                    switch (x.Action)
                    {
                        case MeetingGrpc.Protos.Action.Added:
                            SmartRaiseMessagesChangedEvent(NotifyDictionaryChangedAction.Added, message);
                            break;
                        case MeetingGrpc.Protos.Action.Removed:
                            SmartRaiseMessagesChangedEvent(NotifyDictionaryChangedAction.Removed, message);
                            break;
                        case MeetingGrpc.Protos.Action.Changed:
                            SmartRaiseMessagesChangedEvent(NotifyDictionaryChangedAction.Changed, message);
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                }, chatCancelationToken.Token);
        }

        public override Task ChatUnsubscribeAsync()
        {
            throw new NotImplementedException();
        }

        public override void SendMessage(Guid messageGuid, string message)
        {
            _client.SendMessage(new MessageRequest()
            {
                MessageGuid = messageGuid.ToString(),
                Message = message
            }, _metadata);
        }

        public override async Task SendMessageAsync(Guid messageGuid, string message)
        {
            await _client.SendMessageAsync(new MessageRequest() 
            {
                MessageGuid = messageGuid.ToString(),
                Message = message
            }, _metadata);
        }
    }
}
