using MeetingCommon.DataTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MeetingCommon.Abstractions.Messanger
{
    public abstract partial class MessageServiceAbstract
    {
        protected readonly Dictionary<Guid, MessageDto> messages = new Dictionary<Guid, MessageDto>();

        public IReadOnlyDictionary<Guid, MessageDto> Messages { get; }

        public MessageServiceAbstract()
        {
            Messages = new ReadOnlyDictionary<Guid, MessageDto>(messages);
        }
    }
}
