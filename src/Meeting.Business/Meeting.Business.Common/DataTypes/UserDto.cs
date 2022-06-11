using Framework.DtoTypes;
using System;

namespace Meeting.Business.Common.DataTypes
{
    public class UserDto : GuidDto
    {
        public string UserName { get; }

        public UserDto(Guid guid, string userName)
            : base(guid)
        {
            UserName = userName ?? string.Empty;
        }

        protected override bool EqualsCore(GuidDto dto)
        {
            return dto is UserDto other &&
                Equals(other.UserName, UserName);
        }
    }
}
