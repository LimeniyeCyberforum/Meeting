using Framework.EventArgs;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Meeting.Business.Common.Abstractions.Chat;
using Meeting.Business.Common.DataTypes;
using MeetingProtobuf.Protos;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatClient = MeetingProtobuf.Protos.Chat.ChatClient;

namespace Meeting.Business.GrpcClient
{
    public class ChatService : ChatServiceAbstract
    {
        private CancellationTokenSource _chatSubscribeCancellationToken;

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
            if (_chatSubscribeCancellationToken is not null && !_chatSubscribeCancellationToken.IsCancellationRequested)
                return Task.CompletedTask;

            var call = _client.MessagesSubscribe(new Empty());
            _chatSubscribeCancellationToken = new CancellationTokenSource();

            return call.ResponseStream
                .ToAsyncEnumerable()
                .Finally(() => call.Dispose())
                .ForEachAsync((x) =>
                {
                    MessageDto message = new(Guid.Parse(x.LobbyMessage.MessageGuid), Guid.Parse(x.LobbyMessage.UserGuid), x.LobbyMessage.Message, x.LobbyMessage.Username, x.LobbyMessage.Time.ToDateTime());
                    switch (x.Action)
                    {
                        case MeetingProtobuf.Protos.Action.Added:
                            SmartRaiseMessagesChangedEvent(NotifyDictionaryChangedAction.Added, message);
                            break;
                        case MeetingProtobuf.Protos.Action.Removed:
                            SmartRaiseMessagesChangedEvent(NotifyDictionaryChangedAction.Removed, message);
                            break;
                        case MeetingProtobuf.Protos.Action.Changed:
                            SmartRaiseMessagesChangedEvent(NotifyDictionaryChangedAction.Changed, message);
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                }, _chatSubscribeCancellationToken.Token);
        }

        public override void ChatUnsubscribe()
        {
            _chatSubscribeCancellationToken.Cancel();
            _chatSubscribeCancellationToken.Dispose();
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
