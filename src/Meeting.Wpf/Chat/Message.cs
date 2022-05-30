using System;

namespace Meeting.WPF.Chat
{
    public enum MessageStatus
    {
        Sending,
        Unread,
        Readed
    }

    public class Message
    {
        public Guid Id { get; }
        public string Text { get; }
        public DateTime? DateTime { get; }

        public Message(Guid id, string text, DateTime? dateTime)
        {
            Id = id;
            Text = text;
            DateTime = dateTime;
        }
    }

    public class OwnMessage : Message
    {
        public MessageStatus Status { get; }

        public OwnMessage(Guid id, string text, MessageStatus status, DateTime? dateTime)
            : base(id, text, dateTime)
        {
            Status = status;
        }
    }
}
