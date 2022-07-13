using Utils.DtoTypes;
using System;

namespace Meeting.Core.Common.DataTypes
{
    public class MessageDto : GuidDto
    {
        public Guid UserGuid { get; }

        public string Message { get; }

        public string UserName { get; }

        public DateTime? DateTime { get; }

        public MessageDto(Guid guid, Guid userGuid, string message, string userName, DateTime? dateTime)
            : base(guid)
        {
            UserGuid = userGuid;
            Message = message ?? string.Empty;
            UserName = userName ?? string.Empty;
            DateTime = dateTime ?? new DateTime();
        }

        protected override bool EqualsCore(GuidDto dto)
        {
            return dto is MessageDto other &&
                other.UserGuid == UserGuid &&
                Equals(other.Message, Message) &&
                Equals(other.UserName, UserName) &&
                other.DateTime == DateTime;
        }
    }
}
