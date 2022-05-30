using System;

namespace Meeting.Business.Common.DataTypes
{
    public class UserDto : IEquatable<UserDto>
    {
        public Guid Guid { get; }

        public string UserName { get; }

        private readonly int hash;

        public UserDto(Guid guid, string userName)
        {
            Guid = guid;
            UserName = userName ?? string.Empty;

            hash = (Guid, UserName).GetHashCode();
        }

        public bool Equals(UserDto other)
        {
            return other.Guid == Guid &&
                Equals(other.UserName, UserName);
        }

        public override bool Equals(object obj)
        {
            return obj is UserDto currency && Equals(currency);
        }

        public override int GetHashCode() => hash;
    }
}
