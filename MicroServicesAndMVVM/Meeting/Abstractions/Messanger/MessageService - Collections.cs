using Meeting.DataTypes.Messanger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Meeting.Abstractions.Messanger
{
    internal abstract partial class MessageServiceAbstract
    {
        private Dictionary<Guid, MessageDto> messages = new Dictionary<Guid, MessageDto>();

        public IReadOnlyDictionary<Guid, MessageDto> Messages { get; }

        public MessageServiceAbstract()
        {
            Messages = new ReadOnlyDictionary<Guid, MessageDto>(messages);
        }
    }
}
