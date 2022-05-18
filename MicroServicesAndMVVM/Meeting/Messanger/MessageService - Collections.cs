using Meeting.DataTypes.Messanger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Meeting.Messanger
{
    public partial class MessageService
    {
        private Dictionary<Guid, MessageDto> messages = new Dictionary<Guid, MessageDto>();

        public IReadOnlyDictionary<Guid, MessageDto> Messages { get; }

        public MessageService()
        {
            Messages = new ReadOnlyDictionary<Guid, MessageDto>(messages);
        }
    }
}
