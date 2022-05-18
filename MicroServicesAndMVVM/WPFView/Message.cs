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
        public bool IsOwner { get; }
        public MessageStatus Status { get; }
        public DateTime? DateTime { get; }

        public Message(Guid id, string text, bool isEdited, bool isOwner, MessageStatus status, DateTime? dateTime)
        {
            Id = id;
            Text = text;
            IsEdited = isEdited;
            IsOwner = isOwner;
            Status = status;
            DateTime = dateTime;
        }
    }
}
