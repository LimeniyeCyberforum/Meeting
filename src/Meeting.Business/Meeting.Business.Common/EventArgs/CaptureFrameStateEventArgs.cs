using System;

namespace Meeting.Business.Common.EventArgs
{
    public class CaptureFrameStateEventArgs : System.EventArgs
    {
        public Guid CaptureAreadGuid { get; }
        public bool IsOn { get; }

        public CaptureFrameStateEventArgs(Guid captureAreadGuid, bool isOn)
        {
            CaptureAreadGuid = captureAreadGuid;
            IsOn = isOn;
        }
    }
}
