using System;

namespace WPFView
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
        public bool IsEdited { get; }
        public MessageStatus Status { get; }
        public DateTime DateTime { get; }

        public Message(Guid id, string text, bool isEdited, MessageStatus status, DateTime dateTime)
        {
            Id = id;
            Text = text;
            IsEdited = isEdited;
            Status = status;
            DateTime = dateTime;
        }
    }
}
