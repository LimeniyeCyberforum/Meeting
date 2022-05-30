using Framework.EventArgs;
using Framework.Extensions;
using Meeting.Business.Common.DataTypes;
using System;
using System.Collections.Generic;

namespace Meeting.Business.Common.Abstractions.Messanger
{
    public abstract partial class MessageServiceAbstract
    {
        public event EventHandler<NotifyDictionaryChangedEventArgs<Guid, MessageDto>> MessagesChanged;

        private int messagesChangedSyncNumber = 0;

        protected void RaiseMessagesChangedEvent(NotifyDictionaryChangedAction action, MessageDto newMessage = null, MessageDto oldMessage = null)
        {
            switch (action)
            {
                case NotifyDictionaryChangedAction.Added:
                    messages.AddAndShout(this, MessagesChanged, newMessage.Guid, newMessage, ref messagesChangedSyncNumber);
                    break;
                case NotifyDictionaryChangedAction.Removed:
                    messages.RemoveAndShout(this, MessagesChanged, newMessage.Guid, ref messagesChangedSyncNumber);
                    break;
                case NotifyDictionaryChangedAction.Changed:
                    messages.SetAndShout(this, MessagesChanged, newMessage.Guid, newMessage, ref messagesChangedSyncNumber);
                    break;
                case NotifyDictionaryChangedAction.Cleared:
                    messages.ClearAndShout(this, MessagesChanged, ref messagesChangedSyncNumber);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected void RaiseMessagesChangedEvent(IDictionary<Guid, MessageDto> newElements)
        {
            messages.NewElementsHandler(this, newElements, MessagesChanged, ref messagesChangedSyncNumber);
        }
    }
}
