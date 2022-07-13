using Utils.DtoTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meeting.Core.Common.DataTypes
{
    public enum MessageStatus
    {
        Sending,
        Unread,
        Readed
    }

    public class OwnerMessageDto : MessageDto
    {
        public MessageStatus Status { get; }

        public OwnerMessageDto(Guid guid, Guid userGuid, string message, string userName, DateTime? dateTime, MessageStatus status)
            : base(guid, userGuid, message, userName, dateTime)
        {
            Status = status;
        }

        protected override bool EqualsCore(GuidDto dto)
        {
            return base.EqualsCore(dto) &&
                dto is OwnerMessageDto other &&
                Status == other.Status;
        }
    }
}
