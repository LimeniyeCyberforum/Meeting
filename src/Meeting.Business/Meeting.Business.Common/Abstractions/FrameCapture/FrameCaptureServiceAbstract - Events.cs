using Meeting.Business.Common.EventArgs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meeting.Business.Common.Abstractions.FrameCapture
{
    public abstract partial class FrameCaptureServiceAbstract
    {
        public event EventHandler<FrameCaptureEventArgs> FrameCaptureChanged;
        public event EventHandler<FrameCaptureStateEventArgs> FrameCaptureStateChanged;

        protected void RaiseFrameCaptureChangedAction(Guid frameAreaGuid, byte[] frameBytes, DateTime dateTime)
        {
            FrameCaptureChanged?.Invoke(this, new FrameCaptureEventArgs(frameAreaGuid, frameBytes, dateTime));
        }

        protected void RaiseFrameCaptureStateChangedAction(Guid frameAreaGuid, bool newStateIsOn)
        {
            FrameCaptureStateChanged?.Invoke(this, new FrameCaptureStateEventArgs(frameAreaGuid, newStateIsOn));
        }
    }
}
