using System;

namespace Meeting.Business.Common.DataTypes
{
    public class CaptureFrameAreaDto
    {
        public Guid OwnerGuid { get; }
        public Guid AreaGuid { get; }

        public CaptureFrameAreaDto(Guid ownerGuid, Guid areaGuid)
        {
            OwnerGuid = ownerGuid;
            AreaGuid = areaGuid;
        }
    }
}
