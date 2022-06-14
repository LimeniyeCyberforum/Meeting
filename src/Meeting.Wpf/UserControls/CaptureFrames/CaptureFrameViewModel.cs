using MvvmCommon.WindowsDesktop;
using System;

namespace Meeting.Wpf.UserControls.CaptureFrames
{
    public class CaptureFrameViewModel : BaseInpc
    {
        private Guid _areaGuid, _ownerGuid;
        private string _ownerName;
        private byte[]? _data;

        public Guid AreaGuid { get => _areaGuid; set => Set(ref _areaGuid, value); }
        public Guid OwnerGuid { get => _ownerGuid; set => Set(ref _ownerGuid, value); }
        public string OwnerName { get => _ownerName; set => Set(ref _ownerName, value); }
        public byte[]? Data { get => _data; set => Set(ref _data, value); }

        public CaptureFrameViewModel(Guid areaGuid, Guid ownerGuid, string ownerName, byte[]? data)
        {
            AreaGuid = areaGuid;
            OwnerGuid = ownerGuid;
            OwnerName = ownerName;
            Data = data;
        }
    }
}
