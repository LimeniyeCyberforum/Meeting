using System;

namespace Meeting.Business.Common.EventArgs
{
    public class FrameCaptureStateEventArgs : System.EventArgs
    {
        public Guid CaptureAreadGuid { get; }
        public bool IsOn { get; }

        public FrameCaptureStateEventArgs(Guid captureAreadGuid, bool isOn)
        {
            CaptureAreadGuid = captureAreadGuid;
            IsOn = isOn;
        }
    }
}
