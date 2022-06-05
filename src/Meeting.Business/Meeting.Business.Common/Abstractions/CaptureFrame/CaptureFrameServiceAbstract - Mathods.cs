using Meeting.Business.Common.EventArgs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meeting.Business.Common.Abstractions.FrameCapture
{
    public abstract partial class CaptureFrameServiceAbstract
    {
        public abstract Guid CreateCaptureArea();
        public abstract Task<Guid> CreateCaptureAreaAsync();

        public abstract void DestroyCaptureArea(Guid captureAreaGuid);
        public abstract Task DestroyCaptureAreaAsync(Guid captureAreaGuid);

        public abstract Task CaptureFrameAreasSubscribeAsync();
        public abstract void CaptureFrameAreasUnsubscribe();

        public abstract void SendFrame(byte bytes, Guid captureArea);
        public abstract Task SendFrameAsync(byte bytes, Guid captureArea);

        public abstract Task CaptureFramesSubscribeAsync();

        public abstract void CaptureFramesUnsubscribe();
    }
}
