using System;

namespace Meeting.Business.Common.EventArgs
{
    public class CaptureFrameStateEventArgs : System.EventArgs
    {
        public Guid OwnerGuid { get; }
        public Guid CaptureAreadGuid { get; }
        public bool IsOn { get; }
        public DateTime DateTime { get; }

        public CaptureFrameStateEventArgs(Guid ownerGuid, Guid captureAreadGuid, bool isOn, DateTime dateTime)
        {
            OwnerGuid = ownerGuid;
            CaptureAreadGuid = captureAreadGuid;
            IsOn = isOn;
            DateTime = dateTime;
        }
    }
}
