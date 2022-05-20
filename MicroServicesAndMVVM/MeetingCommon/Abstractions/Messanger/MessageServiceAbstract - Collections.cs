using MeetingCommon.DataTypes.Messanger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MeetingCommon.Abstractions.Messanger
{
    public abstract partial class BaseMessageServiceAbstract
    {
        protected readonly Dictionary<Guid, MessageDto> messages = new Dictionary<Guid, MessageDto>();

        public IReadOnlyDictionary<Guid, MessageDto> Messages { get; }

        public BaseMessageServiceAbstract()
        {
            Messages = new ReadOnlyDictionary<Guid, MessageDto>(messages);
        }
    }
}
