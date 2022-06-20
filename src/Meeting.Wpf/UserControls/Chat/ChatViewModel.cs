using Framework.EventArgs;
using Meeting.Business.Common.Abstractions;
using Meeting.Business.Common.Abstractions.Chat;
using Meeting.Business.Common.DataTypes;
using MvvmCommon.WindowsDesktop;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Data;

namespace Meeting.Wpf.UserControls.Chat
{
    public class ChatViewModel : BaseInpc
    {
        private readonly SerialDisposable eventSubscriptions = new SerialDisposable();
        private readonly ChatServiceAbstract _messageService;
        private readonly IMeetingAuthorization _meetingAuthorization;

        private string _message;

        public string Message { get => _message; set => Set(ref _message, value); }

        public ObservableCollection<MessageDto> Messages { get; } = new ObservableCollection<MessageDto>()
        {
            new MessageDto(Guid.NewGuid(), Guid.Empty, "Hello world", "Server", DateTime.UtcNow)
        };

        #region SendMessageCommand
        private RelayCommand _sendMessageCommand;
        public RelayCommand SendMessageCommand => _sendMessageCommand ?? (
            _sendMessageCommand = new RelayCommand(OnSendMessageExecute, CanSendMessageExecute));

        private async void OnSendMessageExecute()
        {
            var message = Message;
            Message = string.Empty;
            var messageGuid = Guid.NewGuid();
            var newMessage = new OwnerMessageDto(messageGuid, _meetingAuthorization.CurrentUser.Guid, message, _meetingAuthorization.CurrentUser.UserName, null, MessageStatus.Sending);

            lock (((ICollection)Messages).SyncRoot)
            {
                Messages.Add(newMessage);
            }

            await _messageService.SendMessageAsync(messageGuid, message);
        }

        private bool CanSendMessageExecute()
        {
            return string.IsNullOrEmpty(Message) || Message == "" ? false : true;
        }
        #endregion

        public ChatViewModel(ChatServiceAbstract messageService, IMeetingAuthorization meetingAuthorization)
        {
            BindingOperations.EnableCollectionSynchronization(Messages, ((ICollection)Messages).SyncRoot);

            _meetingAuthorization = meetingAuthorization;
            _messageService = messageService;

            foreach (var item in _messageService.Messages.Values)
            {
                if (item.UserGuid == _meetingAuthorization.CurrentUser?.Guid)
                {
                    Messages.Add(new OwnerMessageDto(item.Guid, item.UserGuid, item.Message, item.UserName, item.DateTime, MessageStatus.Readed));
                }
                else
                {
                    Messages.Add(item);
                }
            }

            Subscriptions();
        }

        private void Subscriptions()
        {
            eventSubscriptions.Disposable = null;
            CompositeDisposable disposable = new CompositeDisposable();
            _messageService.MessagesChanged += OnMessagesChanged;

            disposable.Add(Disposable.Create(delegate
            {
                _messageService.MessagesChanged -= OnMessagesChanged;
            }));
            eventSubscriptions.Disposable = disposable;
        }

        private void OnMessagesChanged(object? sender, NotifyDictionaryChangedEventArgs<Guid, MessageDto> e)
        {
            var newValue = e.NewValue;
            var oldValue = e.OldValue;

            lock (((ICollection)Messages).SyncRoot)
            {
                switch (e.Action)
                {
                    case NotifyDictionaryChangedAction.Added:
                        if (newValue.UserGuid == _meetingAuthorization.CurrentUser?.Guid)
                        {
                            var index = Messages.IndexOf(Messages.FirstOrDefault(x => x.Guid == newValue.Guid));
                            if (index > -1)
                            {
                                Messages[index] = new OwnerMessageDto(newValue.Guid, newValue.UserGuid, newValue.Message, newValue.UserName, newValue.DateTime, MessageStatus.Readed);
                            }
                            else
                            {
                                Messages.Add(new OwnerMessageDto(newValue.Guid, newValue.UserGuid, newValue.Message, newValue.UserName, newValue.DateTime, MessageStatus.Readed));
                            }
                        }
                        else
                        {
                            Messages.Add(new MessageDto(newValue.Guid, newValue.UserGuid, newValue.Message, newValue.UserName, newValue.DateTime));
                        }
                        break;
                    case NotifyDictionaryChangedAction.Removed:
                        if (Messages.Contains(newValue))
                            Messages.Remove(newValue);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            };
        }
    }
}
