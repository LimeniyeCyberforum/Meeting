using Meeting.Business.Common.EventArgs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meeting.Business.Common.Abstractions.FrameCapture
{
    public abstract partial class FrameCaptureServiceAbstract
    {
        public abstract Task SendOwnCameraFrameAsync(byte bytes);

        public abstract Task FrameCapturesSubscribeAsync();

        public abstract Task FrameCapturesUnsubscribeAsync();
    }
}
