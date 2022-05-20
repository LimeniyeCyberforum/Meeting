using Common.EventArgs;
using MeetingCommon.Abstractions.Messanger;
using Microsoft.Extensions.DependencyInjection;
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

        public ChatViewModel()
        {
            _messageService = IocService.ServiceProvider.GetService<MessageServiceAbstract>();
            _messageService.MessagesChanged += OnMessagesChanged;
        }

        private void OnMessagesChanged(object? sender, NotifyDictionaryChangedEventArgs<Guid, MeetingCommon.DataTypes.Messanger.MessageDto> e)
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
