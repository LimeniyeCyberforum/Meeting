using Meeting.Business.Common.DataTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Meeting.Business.Common.Abstractions.Messanger
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
