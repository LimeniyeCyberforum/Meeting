using System;

namespace Meeting.Business.Common.EventArgs
{
    public class CaptureFrameStateEventArgs : System.EventArgs
    {
        public Guid OwnerGuid { get; }
        public Guid CaptureAreadGuid { get; }
        public bool IsOn { get; }

        public CaptureFrameStateEventArgs(Guid ownerGuid, Guid captureAreadGuid, bool isOn)
        {
            OwnerGuid = ownerGuid;
            CaptureAreadGuid = captureAreadGuid;
            IsOn = isOn;
        }
    }
}
