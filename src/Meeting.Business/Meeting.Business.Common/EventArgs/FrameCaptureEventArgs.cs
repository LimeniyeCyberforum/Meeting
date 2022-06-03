using System;

namespace Meeting.Business.Common.EventArgs
{
    public class FrameCaptureEventArgs : System.EventArgs
    {
        public Guid CaptureAreadGuid { get; }
        public byte[] Bytes { get; }
        public DateTime DateTime { get; }

        public FrameCaptureEventArgs(Guid captureAreadGuid, byte[] bytes, DateTime dateTime)
        {
            CaptureAreadGuid = captureAreadGuid;
            Bytes = bytes;
            DateTime = dateTime;
        }
    }
}
