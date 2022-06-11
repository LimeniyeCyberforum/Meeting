using System;

namespace Meeting.Business.Common.DataTypes
{
    public class CaptureFrameAreaDto : GuidDto
    {
        public Guid OwnerGuid { get; }

        public CaptureFrameAreaDto(Guid areaGuid, Guid ownerGuid)
            : base(areaGuid)
        {
            OwnerGuid = ownerGuid;
        }

        protected override bool EqualsCore(GuidDto dto)
        {
            return dto is CaptureFrameAreaDto other &&
                Equals(other.OwnerGuid, OwnerGuid);
        }
    }
}
