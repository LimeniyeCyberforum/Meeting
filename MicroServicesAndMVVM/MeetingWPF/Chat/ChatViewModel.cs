using Common.EventArgs;
using MeetingCommon.Abstractions;
using MeetingCommon.Abstractions.Messanger;
using MeetingCommon.DataTypes;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace MeetingWPF.Chat
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
            new Message(Guid.NewGuid(), "Hello world", DateTime.Now)
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
            var newMessage = new OwnMessage(messageGuid, message, MessageStatus.Sending, null);

            _ = dispatcher.BeginInvoke(() => Messages.Add(newMessage));

            await _messageService.SendMessageAsync(messageGuid, _userDto.Guid, message);
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

            _userDto = meetingConnection.CurrentUser;

            foreach (var item in _messageService.Messages.Values)
            {
                if (item.UserGuid == _userDto.Guid)
                {
                    Messages.Add(new OwnMessage(item.Guid, item.Message, MessageStatus.Readed, item.DateTime));
                }
                else
                {
                    Messages.Add(new Message(item.Guid, item.Message, item.DateTime));
                }
            }

            _messageService.MessagesChanged += OnMessagesChanged;

            _messageService.ChatSubscribeAsync();
        }

        private void OnConnectionStateChanged(object? sender, (ConnectionAction Action, UserDto User) e)
        {
            if (e.Action == ConnectionAction.Connected)
            {
                _userDto = e.User;

                Messages.Clear();
                foreach (var item in _messageService.Messages.Values)
                {
                    if (item.UserGuid == _userDto.Guid)
                    {
                        Messages.Add(new OwnMessage(item.Guid, item.Message, MessageStatus.Readed, item.DateTime));
                    }
                    else
                    {
                        Messages.Add(new Message(item.Guid, item.Message, item.DateTime));
                    }
                }

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

                        if (newValue.UserGuid == _userDto.Guid)
                        {
                            var index = Messages.IndexOf(Messages.FirstOrDefault(x => x.Id == newValue.Guid));
                            if (index > -1)
                            {
                                Messages[index] = new OwnMessage(newValue.Guid, newValue.Message, MessageStatus.Readed, newValue.DateTime);
                            }
                            else
                            {
                                Messages.Add(new OwnMessage(newValue.Guid, newValue.Message, MessageStatus.Readed, newValue.DateTime));
                            }
                        }
                        else
                        {
                            Messages.Add(new Message(newValue.Guid, newValue.Message, newValue.DateTime));
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
