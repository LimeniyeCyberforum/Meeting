using MvvmCommon.WindowsDesktop;
using System;

namespace Meeting.Wpf.CaptureFrames
{
    public class CaptureFrameViewModel : BaseInpc
    {
        private Guid _ownerGuid, _areaGuid;
        private string _ownerName;
        private byte[]? _data;

        public Guid OwnerGuid { get => _ownerGuid; set => Set(ref _ownerGuid, value); }
        public Guid AreaGuid { get => _areaGuid; set => Set(ref _areaGuid, value); }
        public string OwnerName { get => _ownerName; set => Set(ref _ownerName, value); }
        public byte[]? Data { get => _data; set => Set(ref _data, value); }

        public CaptureFrameViewModel(Guid ownerGuid, string ownerName, Guid areaGuid, byte[]? data)
        {
            OwnerGuid = ownerGuid;
            AreaGuid = areaGuid;
            Data = data;
            OwnerName = ownerName;
        }
    }
}
