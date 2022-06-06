using Framework.EventArgs;
using Meeting.Business.Common.Abstractions;
using Meeting.Business.Common.Abstractions.Chat;
using Meeting.Business.Common.DataTypes;
using MvvmCommon.WindowsDesktop;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Meeting.WPF.Chat
{
    public class ChatViewModel : BaseInpc
    {
        private readonly Dispatcher dispatcher = Application.Current.Dispatcher;
        private readonly ChatServiceAbstract _messageService;
        private readonly IMeetingAuthorization _meetingAuthorization;

        private string _message;

        public string Message { get => _message; set => Set(ref _message, value); }

        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>()
        {
            new Message(Guid.NewGuid(), "Hello world", "Server", DateTime.Now)
        };

        #region SendMessageCommand
        private RelayCommandAsync _sendMessageCommand;
        public RelayCommandAsync SendMessageCommand => _sendMessageCommand ?? (
            _sendMessageCommand = new RelayCommandAsync(OnSendMessageExecute, CanSendMessageExecute));

        private async void OnSendMessageExecute()
        {
            var message = Message;
            Message = string.Empty;
            var messageGuid = Guid.NewGuid();
            var newMessage = new OwnMessage(messageGuid, message, _meetingAuthorization.CurrentUser.UserName, MessageStatus.Sending, null);

            _ = dispatcher.BeginInvoke(() => Messages.Add(newMessage));

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
                    Messages.Add(new OwnMessage(item.Guid, item.Message, item.UserName, MessageStatus.Readed, item.DateTime));
                }
                else
                {
                    Messages.Add(new Message(item.Guid, item.Message, item.UserName, item.DateTime));
                }
            }
        }

        private void OnMessagesChanged(object? sender, NotifyDictionaryChangedEventArgs<Guid, MessageDto> e)
        {
            var newValue = e.NewValue;
            var oldValue = e.OldValue;

            _ = dispatcher.BeginInvoke(() =>
            {
                switch (e.Action)
                {
                    case NotifyDictionaryChangedAction.Added:
                        if (newValue.UserGuid == _meetingAuthorization.CurrentUser?.Guid)
                        {
                            var index = Messages.IndexOf(Messages.FirstOrDefault(x => x.Id == newValue.Guid));
                            if (index > -1)
                            {
                                Messages[index] = new OwnMessage(newValue.Guid, newValue.Message, newValue.UserName, MessageStatus.Readed, newValue.DateTime);
                            }
                            else
                            {
                                Messages.Add(new OwnMessage(newValue.Guid, newValue.Message, newValue.UserName, MessageStatus.Readed, newValue.DateTime));
                            }
                        }
                        else
                        {
                            Messages.Add(new Message(newValue.Guid, newValue.Message, newValue.UserName, newValue.DateTime));
                        }

                        break;
                    case NotifyDictionaryChangedAction.Changed:
                        throw new NotImplementedException();
                        //var index = Messages.IndexOf(Messages.FirstOrDefault(x => x.Id == newValue.Guid));
                        //Messages[index] = new Message(newValue.Guid, newValue.Message, false, true, MessageStatus.Readed, DateTime.Now);
                        break;
                    case NotifyDictionaryChangedAction.Removed:
                        Messages.Remove(Messages.FirstOrDefault(x => x.Id == newValue.Guid));
                        break;
                    case NotifyDictionaryChangedAction.Cleared:
                        Messages.Clear();
                        break;
                    case NotifyDictionaryChangedAction.Initialized:
                        Messages.Clear();
                        throw new NotImplementedException();


                        //if (true)
                        //{
                        //    Messages.Add(new OwnMessage(newValue.Guid, newValue.Message, MessageStatus.Readed, newValue.DateTime));
                        //}
                        //else
                        //{
                        //    // TODO : Is not my message
                        //}
                        //foreach (var item in e.NewDictionary.Values.Select(x => new Message(newValue.Guid, newValue.Message, false, true, MessageStatus.Readed, DateTime.Now)))
                        //{
                        //    Messages.Add(item);
                        //}
                        break;
                }
            });
        }
    }
}
