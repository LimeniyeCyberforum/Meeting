using Meeting.Business.Common.EventArgs;
using System;

namespace Meeting.Business.Common.Abstractions.FrameCapture
{
    public abstract partial class CaptureFramesServiceAbstract
    {
        public event EventHandler<CaptureFrameEventArgs> CaptureFrameChanged;
        public event EventHandler<CaptureFrameStateEventArgs> CaptureFrameStateChanged;

        protected void RaiseCaptureFrameChangedAction(Guid frameAreaGuid, byte[] frameBytes, DateTime dateTime)
        {
            CaptureFrameChanged?.Invoke(this, new CaptureFrameEventArgs(frameAreaGuid, frameBytes, dateTime));
        }

        protected void RaiseCaptureFrameStateChangedAction(Guid frameAreaGuid, bool newStateIsOn)
        {
            CaptureFrameStateChanged?.Invoke(this, new CaptureFrameStateEventArgs(frameAreaGuid, newStateIsOn));
        }
    }
}
