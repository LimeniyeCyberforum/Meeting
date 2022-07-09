using Meeting.Core.Common.DataTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Meeting.Core.Common.Abstractions.Chat
{
    public abstract partial class ChatServiceAbstract : IDisposable
    {
        private bool disposed = false;

        protected readonly Dictionary<Guid, MessageDto> messages = new Dictionary<Guid, MessageDto>();

        public IReadOnlyDictionary<Guid, MessageDto> Messages { get; }

        public ChatServiceAbstract()
        {
            Messages = new ReadOnlyDictionary<Guid, MessageDto>(messages);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                ChatUnsubscribe();
            }
            disposed = true;
        }

        ~ChatServiceAbstract()
        {
            Dispose(disposing: false);
        }
    }
}
