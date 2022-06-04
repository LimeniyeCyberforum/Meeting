using Meeting.Business.Common.DataTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Meeting.Business.Common.Abstractions.Chat
{
    public abstract partial class ChatServiceAbstract
    {
        protected readonly Dictionary<Guid, MessageDto> messages = new Dictionary<Guid, MessageDto>();

        public IReadOnlyDictionary<Guid, MessageDto> Messages { get; }

        public ChatServiceAbstract()
        {
            Messages = new ReadOnlyDictionary<Guid, MessageDto>(messages);
        }
    }
}
