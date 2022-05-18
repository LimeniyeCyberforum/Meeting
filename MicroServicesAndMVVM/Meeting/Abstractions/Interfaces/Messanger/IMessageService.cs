using Common.EventArgs;
using MeetingRepository.DataTypes.Messanger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingRepository.Abstractions.Interfaces.Messanger
{
    public interface IMessageService
    {
        event EventHandler<NotifyDictionaryChangedEventArgs<Guid, MessageDto>> MessagesChanged;

        IReadOnlyDictionary<Guid, MessageDto> Messages { get; }

        void SendMessage(Guid guid, string username, string message);
        Task SendMessageAsync(Guid guid, string username, string message);
    }
}
