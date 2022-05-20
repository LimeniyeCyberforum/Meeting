using Common.EventArgs;
using MeetingCommon.Abstractions;
using MeetingCommon.Abstractions.Messanger;
using MeetingCommon.DataTypes;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace WPFView.Chat
{
    public class ChatViewModel : BaseInpc
    {
        private readonly Dispatcher dispatcher = Application.Current.Dispatcher;
        private readonly MessageServiceAbstract _messageService;

        private UserDto _userDto;

        private string _message;

        public string Message { get => _message; set => Set(ref _message, value); }

        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>()
        {
            new Message(Guid.NewGuid(), "Hello world", false, true, MessageStatus.Readed, DateTime.Now)
        };

        #region SendMessageCommand
        private RelayCommandAsync _sendMessageCommand;
        public RelayCommandAsync SendMessageCommand => _sendMessageCommand ?? (
            _sendMessageCommand = new RelayCommandAsync(OnSendMessageExecute, CanSendMessageExecute));

        private async void OnSendMessageExecute()
        {
            //await _call.RequestStream.WriteAsync(new MessageRequest() { Username = "limeniye", Message = Message });
            var newMessage = new Message(Guid.NewGuid(), Message, false, false, MessageStatus.Readed, null);
            Message = string.Empty;

            _messageService.SendMessageAsync(Guid.NewGuid(), );

            _ = dispatcher.BeginInvoke(() => Messages.Add(newMessage));

            //await _messageService.SendMessageAsync(Guid.NewGuid(), "limeniye", Message);

            //var messageIndex = Messages.IndexOf(newMessage);
            //if (messageIndex > -1)
            //{
            //    _ = dispatcher.BeginInvoke(() => Messages[messageIndex] =
            //           new Message(Guid.NewGuid(), Message, false, false, MessageStatus.Readed, response.Time.ToDateTime()));
            //}
        }

        private bool CanSendMessageExecute()
        {
            return string.IsNullOrEmpty(Message) || Message == "" ? false : true;
        }
        #endregion

        public ChatViewModel(MessageServiceAbstract messageService, IMeetingConnectionService meetingConnection)
        {
            _messageService = messageService;
            meetingConnection.ConnectionStateChanged += OnConnectionStateChanged;

            foreach (var item in _messageService.Messages.Values)
                Messages.Add(new Message(item.Guid, item.Message, false, false, MessageStatus.Readed, item.DateTime));

            _messageService.MessagesChanged += OnMessagesChanged;
        }

        private void OnConnectionStateChanged(object? sender, (ConnectionAction Action, UserDto User) e)
        {
            if (e.Action == ConnectionAction.Connected)
            {
                _userDto = e.User;

                Messages.Clear();
                foreach (var item in _messageService.Messages.Values)
                    Messages.Add(new Message(item.Guid, item.Message, false, false, MessageStatus.Readed, item.DateTime));

                _messageService.MessagesChanged += OnMessagesChanged;
            }
            else
            {
                _userDto = null;
                _messageService.MessagesChanged -= OnMessagesChanged;
                Messages.Clear();
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
                        Messages.Add(new Message(newValue.Guid, newValue.Message, false, true, MessageStatus.Readed, DateTime.Now));

                        break;
                    case NotifyDictionaryChangedAction.Changed:
                        var index = Messages.IndexOf(Messages.FirstOrDefault(x => x.Id == newValue.Guid));
                        Messages[index] = new Message(newValue.Guid, newValue.Message, false, true, MessageStatus.Readed, DateTime.Now);
                        break;
                    case NotifyDictionaryChangedAction.Removed:
                        Messages.Remove(Messages.FirstOrDefault(x => x.Id == newValue.Guid));
                        break;
                    case NotifyDictionaryChangedAction.Cleared:
                        Messages.Clear();
                        break;
                    case NotifyDictionaryChangedAction.Initialized:
                        Messages.Clear();

                        foreach (var item in e.NewDictionary.Values.Select(x => new Message(newValue.Guid, newValue.Message, false, true, MessageStatus.Readed, DateTime.Now)))
                        {
                            Messages.Add(item);
                        }
                        break;
                }
            });
        }
    }
}
