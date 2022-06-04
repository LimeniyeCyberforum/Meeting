using System;
using System.Threading.Tasks;

namespace Meeting.Business.Common.Abstractions.Chat
{
    public abstract partial class ChatServiceAbstract
    {
        public abstract void SendMessage(Guid messageGuid, string message);

        public abstract Task SendMessageAsync(Guid messageGuid, string message);

        public abstract Task ChatSubscribeAsync();

        public abstract Task ChatUnsubscribeAsync();
    }
}
