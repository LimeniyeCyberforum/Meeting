using Meeting.Business.Common.EventArgs;
using System;

namespace Meeting.Business.Common.Abstractions.FrameCapture
{
    public abstract partial class CaptureFrameServiceAbstract
    {
        public event EventHandler<CaptureFrameEventArgs> FrameCaptureChanged;
        public event EventHandler<CaptureFrameStateEventArgs> FrameCaptureStateChanged;

        protected void RaiseFrameCaptureChangedAction(Guid frameAreaGuid, byte[] frameBytes, DateTime dateTime)
        {
            FrameCaptureChanged?.Invoke(this, new CaptureFrameEventArgs(frameAreaGuid, frameBytes, dateTime));
        }

        protected void RaiseFrameCaptureStateChangedAction(Guid frameAreaGuid, bool newStateIsOn)
        {
            FrameCaptureStateChanged?.Invoke(this, new CaptureFrameStateEventArgs(frameAreaGuid, newStateIsOn));
        }
    }
}
