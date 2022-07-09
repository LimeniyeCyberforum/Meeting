using System;

namespace Meeting.Core.Common.EventArgs
{
    public class CaptureFrameEventArgs : System.EventArgs
    {
        public Guid CaptureAreadGuid { get; }
        public byte[] Bytes { get; }
        public DateTime DateTime { get; }

        public CaptureFrameEventArgs(Guid captureAreadGuid, byte[] bytes, DateTime dateTime)
        {
            CaptureAreadGuid = captureAreadGuid;
            Bytes = bytes;
            DateTime = dateTime;
        }
    }
}
