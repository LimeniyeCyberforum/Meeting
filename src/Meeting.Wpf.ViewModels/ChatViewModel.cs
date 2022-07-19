using Utils.EventArgs;
using Meeting.Core.Common;
using Meeting.Core.Common.DataTypes;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Toolkit.WindowsDesktop;

namespace Meeting.Wpf.ViewModels
{
    public class ChatViewModel : ReactiveObject, IDisposable
    {
        private readonly SerialDisposable eventSubscriptions = new SerialDisposable();
        private readonly IChatService _messageService;
        private readonly IAuthorizationService _meetingAuthorization;

        [Reactive]
        public string? Message { get; set; }

        public ObservableCollection<MessageDto> Messages { get; } = new ObservableCollection<MessageDto>()
        {
            new MessageDto(Guid.NewGuid(), Guid.Empty, "Hello world", "Server", DateTime.UtcNow)
        };

        #region SendMessageCommand

        public ReactiveCommand<Unit, Unit> SendMessageCommand =>
            ReactiveCommand.Create(OnSendMessageExecute,
                this.WhenAnyValue(x => x.Message)
                .Select(x => !string.IsNullOrWhiteSpace(Message)), Scheduler.CurrentThread);

        private async void OnSendMessageExecute()
        {
            var message = Message;
            Message = string.Empty;
            var messageGuid = Guid.NewGuid();
            var newMessage = new OwnerMessageDto(messageGuid, _meetingAuthorization.CurrentUser.Guid, message, _meetingAuthorization.CurrentUser.UserName, null, MessageStatus.Sending);

            Messages.Add(newMessage);

            await _messageService.SendMessageAsync(messageGuid, message);
        }

        #endregion

        public ChatViewModel(IChatService messageService, IAuthorizationService meetingAuthorization)
        {
            Messages.EnableCollectionSynchronization();

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

            Subscribe();
        }

        private void Subscribe()
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

            switch (e.Action)
            {
                case NotifyDictionaryChangedAction.Added:
                    if (newValue.UserGuid == _meetingAuthorization.CurrentUser?.Guid)
                    {
                        MessageDto? message = Messages.FirstOrDefault(x => x.Guid == newValue.Guid);
                        int index = message is null ? -1 : Messages.IndexOf(message);
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
        }

        public void Dispose()
        {
            eventSubscriptions?.Dispose();
        }
    }
}
