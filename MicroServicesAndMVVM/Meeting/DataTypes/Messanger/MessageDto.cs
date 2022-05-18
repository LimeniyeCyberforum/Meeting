using System;

namespace MeetingRepository.DataTypes.Messanger
{
    public class MessageDto : IEquatable<MessageDto>
    {
        public Guid Guid { get; }

        public string Message { get; }

        public string UserName { get; }

        public DateTime? DateTime { get; }

        private readonly int hash;

        public MessageDto(Guid guid, string message, string userName, DateTime? dateTime)
        {
            Guid = guid;
            Message = message ?? String.Empty;
            UserName = userName ?? String.Empty;
            DateTime = dateTime ?? new DateTime();

            hash = (Guid, Message, UserName, DateTime).GetHashCode();
        }

        public bool Equals(MessageDto other)
        {
            return other.Guid == Guid && 
                Equals(other.Message, Message) && 
                Equals(other.UserName, UserName) &&
                DateTime == DateTime;
        }

        public override bool Equals(object obj)
        {
            return obj is MessageDto currency && Equals(currency);
        }

        public override int GetHashCode() => hash;
    }
}
