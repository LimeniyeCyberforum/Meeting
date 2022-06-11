using System;

namespace Meeting.Business.Common.EventArgs
{
    public enum CaptureFrameState
    {
        Disabled,
        Enabled,
        Created,
        Removed
    }

    public class CaptureFrameStateEventArgs : System.EventArgs
    {
        public Guid OwnerGuid { get; }
        public Guid CaptureAreadGuid { get; }
        public CaptureFrameState Action { get; }
        public DateTime DateTime { get; }

        public CaptureFrameStateEventArgs(Guid ownerGuid, Guid captureAreadGuid, CaptureFrameState action, DateTime dateTime)
        {
            OwnerGuid = ownerGuid;
            CaptureAreadGuid = captureAreadGuid;
            Action = action;
            DateTime = dateTime;
        }
    }
}
