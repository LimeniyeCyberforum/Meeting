﻿using Meeting.Business.Common.EventArgs;
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

        protected void RaiseCaptureFrameStateChangedAction(Guid ownerGuid, Guid frameAreaGuid, CaptureFrameState newCaptureState, DateTime dateTime)
        {
            CaptureFrameStateChanged?.Invoke(this, new CaptureFrameStateEventArgs(ownerGuid, frameAreaGuid, newCaptureState, dateTime));
        }
    }
}
