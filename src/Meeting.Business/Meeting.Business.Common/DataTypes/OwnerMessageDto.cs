using System;
using System.Collections.Generic;
using System.Text;

namespace Meeting.Business.Common.DataTypes
{
    public enum MessageStatus
    {
        Sending,
        Unread,
        Readed
    }

    public class OwnerMessageDto : MessageDto, IEquatable<OwnerMessageDto>
    {
        public MessageStatus Status { get; }

        private readonly int hash;

        public OwnerMessageDto(Guid guid, Guid userGuid, string message, string userName, DateTime? dateTime, MessageStatus status)
            : base(guid, userGuid, message, userName, dateTime)
        {
            Status = status;

            hash = (base.GetHashCode(), Status).GetHashCode();
        }

        public bool Equals(OwnerMessageDto other)
        {
            return base.Equals(other) && other.Status == Status;
        }

        public override bool Equals(object obj)
        {
            return obj is MessageDto currency && Equals(currency);
        }

        public override int GetHashCode() => hash;
    }
}
