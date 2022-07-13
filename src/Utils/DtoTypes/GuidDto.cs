using System;

namespace Utils.DtoTypes
{
    public abstract class GuidDto : IEquatable<GuidDto>
    {
        public Guid Guid { get; }
        private readonly int hash;
        public GuidDto(Guid guid)
        {
            Guid = guid;
            hash = HashCode.Combine(guid.GetHashCode(), GetType().GetHashCode());
        }

        public sealed override bool Equals(object obj)
        {
            return obj is GuidDto dto &&
                Equals(dto) &&
                EqualsCore(dto);
        }

        public bool Equals(GuidDto other)
        {
            return !(other is null) &&
                   Guid.Equals(other.Guid) &&
                   GetType().Equals(other.GetType());
        }

        protected abstract bool EqualsCore(GuidDto dto);
        public sealed override int GetHashCode() => hash;
    }
}
