using MeetingRepository.Abstractions.Interfaces.Messanger;
using System;
using System.Threading.Tasks;

namespace MeetingRepository.Abstractions.Messanger
{
    public abstract partial class BaseMessageServiceAbstract : IMessageService
    {
        public abstract void SendMessage(Guid guid, string username, string message);

        public abstract Task SendMessageAsync(Guid guid, string username, string message);
    }
}
