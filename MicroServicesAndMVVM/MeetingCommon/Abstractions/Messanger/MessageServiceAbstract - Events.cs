using Common.EventArgs;
using Common.Extensions;
using MeetingRepository.Abstractions.Interfaces.Messanger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MeetingRepository.Abstractions.Messanger
{
    public abstract partial class BaseMessageServiceAbstract : IMessageService
    {
        public event EventHandler<byte[]> CameraCaptureChanged;
        public event EventHandler<NotifyDictionaryChangedEventArgs<Guid, MessageDto>> MessagesChanged;

        private int messagesChangedSyncNumber = 0;

        protected void RaiseCameraCaptureChanged(byte[] array)
        {
            CameraCaptureChanged?.Invoke(this, array);
        }

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
