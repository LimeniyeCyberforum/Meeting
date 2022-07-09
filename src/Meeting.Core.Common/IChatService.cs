using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.EventArgs;
using Meeting.Core.Common.DataTypes;

namespace Meeting.Core.Common
{
    public interface IChatService : IDisposable
    {
        IReadOnlyDictionary<Guid, MessageDto> Messages { get; }

        event EventHandler<NotifyDictionaryChangedEventArgs<Guid, MessageDto>> MessagesChanged;

        void SendMessage(Guid messageGuid, string message);

        Task SendMessageAsync(Guid messageGuid, string message);

        Task ChatSubscribeAsync();

        void ChatUnsubscribe();
    }
}
