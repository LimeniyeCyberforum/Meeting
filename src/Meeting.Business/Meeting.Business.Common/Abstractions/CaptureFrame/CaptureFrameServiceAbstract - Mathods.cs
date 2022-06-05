using Meeting.Business.Common.EventArgs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meeting.Business.Common.Abstractions.FrameCapture
{
    public abstract partial class CaptureFrameServiceAbstract
    {
        public abstract Task SendFrameAsync(byte bytes);

        public abstract Task CaptureFramesSubscribeAsync();

        public abstract Task CaptureFramesUnsubscribeAsync();
    }
}
