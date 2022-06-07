using Framework.EventArgs;
using Meeting.Business.Common.Abstractions;
using Meeting.Business.Common.Abstractions.Chat;
using Meeting.Business.Common.DataTypes;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Toolkit.Xamarin;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Meeting.Xamarin.Parts
{
    public class ChatViewModel : BaseInpc
    {
        private readonly ChatServiceAbstract _messageService;
        private readonly IMeetingAuthorization _meetingAuthorization;

        private string _message;

        public string Message { get => _message; set => Set(ref _message, value); }

        public ObservableCollection<MessageDto> Messages { get; } = new ObservableCollection<MessageDto>()
        {
            new MessageDto(Guid.NewGuid(), Guid.Empty, "Hello world", "Server", DateTime.Now)
        };

        #region SendMessageCommand
        private AsyncCommand _sendMessageCommand;
        public AsyncCommand SendMessageCommand => _sendMessageCommand ?? (
            _sendMessageCommand = new AsyncCommand(OnSendMessageExecute, CanSendMessageExecute));

        private async Task OnSendMessageExecute()
        {
            var message = Message;
            Message = string.Empty;
            var messageGuid = Guid.NewGuid();
            var newMessage = new OwnerMessageDto(messageGuid, _meetingAuthorization.CurrentUser.Guid, message, _meetingAuthorization.CurrentUser.UserName, null, MessageStatus.Sending);

            Messages.Add(newMessage);

            await _messageService.SendMessageAsync(messageGuid, message);
        }

        private bool CanSendMessageExecute()
        {
            return string.IsNullOrEmpty(Message) || Message == "" ? false : true;
        }
        #endregion

        public ChatViewModel(ChatServiceAbstract messageService, IMeetingAuthorization meetingAuthorization)
        {
            _meetingAuthorization = meetingAuthorization;

            _messageService = messageService;

            _messageService.MessagesChanged += OnMessagesChanged;

            _messageService.ChatSubscribeAsync();

            foreach (var item in _messageService.Messages.Values)
            {
                if (item.UserGuid == _meetingAuthorization.CurrentUser?.Guid)
                {
                    // TODO : Input type should be OwnerMessageDto
                    Messages.Add(new OwnerMessageDto(item.Guid, item.UserGuid, item.Message, item.UserName, item.DateTime, MessageStatus.Readed));
                }
                else
                {
                    Messages.Add(item);
                }
            }
        }

        private void OnMessagesChanged(object sender, NotifyDictionaryChangedEventArgs<Guid, MessageDto> e)
        {
            var newValue = e.NewValue;
            var oldValue = e.OldValue;

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
        }
    }
}
