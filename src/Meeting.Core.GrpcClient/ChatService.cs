using Framework.EventArgs;
using Framework.Extensions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Meeting.Core.Common;
using Meeting.Core.Common.DataTypes;
using Meeting.Core.GrpcClient.Util;
using MeetingProtobuf.Protos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChatClient = MeetingProtobuf.Protos.Chat.ChatClient;

namespace Meeting.Core.GrpcClient
{
    internal partial class ChatService : IChatService
    {
        protected readonly Dictionary<Guid, MessageDto> messages = new Dictionary<Guid, MessageDto>();

        private bool disposed = false;

        private int messagesChangedSyncNumber = 0;
        
        private CancellationTokenSource _chatSubscribeCancellationToken;

        private readonly ChatClient _client;

        private Metadata _metadata;

        public IReadOnlyDictionary<Guid, MessageDto> Messages { get; }

        public event EventHandler<NotifyDictionaryChangedEventArgs<Guid, MessageDto>> MessagesChanged;

        public ChatService(ChatClient client)
            : base()
        {
            _client = client;
            Messages = new ReadOnlyDictionary<Guid, MessageDto>(messages);
        }

        public void UpdateMetadata(Metadata newMetadata)
        {
            _metadata = newMetadata;
        }

        public Task ChatSubscribeAsync()
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
                            messages.AddAndShout(this, MessagesChanged, message.Guid, message, ref messagesChangedSyncNumber);
                            break;
                        case MeetingProtobuf.Protos.Action.Removed:
                            messages.RemoveAndShout(this, MessagesChanged, message.Guid, ref messagesChangedSyncNumber);
                            break;
                        case MeetingProtobuf.Protos.Action.Changed:
                            messages.SetAndShout(this, MessagesChanged, message.Guid, message, ref messagesChangedSyncNumber);
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                }, _chatSubscribeCancellationToken.Token);
        }

        public void ChatUnsubscribe()
        {
            _chatSubscribeCancellationToken.Cancel();
            _chatSubscribeCancellationToken.Dispose();
        }

        public void SendMessage(Guid messageGuid, string message)
        {
            _client.SendMessage(new MessageRequest()
            {
                MessageGuid = messageGuid.ToString(),
                Message = message
            }, _metadata);
        }

        public async Task SendMessageAsync(Guid messageGuid, string message)
        {
            await _client.SendMessageAsync(new MessageRequest()
            {
                MessageGuid = messageGuid.ToString(),
                Message = message
            }, _metadata);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                ChatUnsubscribe();
            }
            disposed = true;
        }

        ~ChatService()
        {
            Dispose(disposing: false);
        }
    }
}
