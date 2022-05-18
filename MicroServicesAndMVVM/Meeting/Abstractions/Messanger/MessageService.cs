using Meeting.Abstractions.Interfaces.Messanger;
using System;
using System.Threading.Tasks;

namespace Meeting.Abstractions.Messanger
{
    public abstract partial class MessageService : IMessageService
    {
        public abstract void SendMessage(Guid guid, string username, string message);

        public abstract Task SendMessageAsync(Guid guid, string username, string message);
    }
}
