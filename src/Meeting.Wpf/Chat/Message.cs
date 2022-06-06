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
        public string Name { get; }
        public DateTime? DateTime { get; }

        public Message(Guid id, string text, string name, DateTime? dateTime)
        {
            Id = id;
            Text = text;
            Name = name;
            DateTime = dateTime;
        }
    }

    public class OwnMessage : Message
    {
        public MessageStatus Status { get; }

        public OwnMessage(Guid id, string text, string name, MessageStatus status, DateTime? dateTime)
            : base(id, text, name, dateTime)
        {
            Status = status;
        }
    }
}
