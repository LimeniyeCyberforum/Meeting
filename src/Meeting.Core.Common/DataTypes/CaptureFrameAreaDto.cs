using Framework.DtoTypes;
using System;

namespace Meeting.Core.Common.DataTypes
{
    public class CaptureFrameAreaDto : GuidDto
    {
        public Guid OwnerGuid { get; }
        public bool IsActive { get; }

        public CaptureFrameAreaDto(Guid areaGuid, Guid ownerGuid, bool isActive)
            : base(areaGuid)
        {
            OwnerGuid = ownerGuid;
            IsActive = isActive;
        }

        protected override bool EqualsCore(GuidDto dto)
        {
            return dto is CaptureFrameAreaDto other &&
                OwnerGuid == other.OwnerGuid &&
                other.IsActive == IsActive;
        }
    }
}
