﻿using System;

namespace Meeting.Business.Common.DataTypes
{
    public abstract class GuidDto : IEquatable<GuidDto>
    {
        public Guid Guid { get; }
        private readonly int hash;
        public GuidDto(Guid guid)
        {
            Guid = guid;
            hash = guid.GetHashCode();
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
                   Guid.Equals(other.Guid);
        }
        protected abstract bool EqualsCore(GuidDto dto);
        public sealed override int GetHashCode() => hash;
    }
}
