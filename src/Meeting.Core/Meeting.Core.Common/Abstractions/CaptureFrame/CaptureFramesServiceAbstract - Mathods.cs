using System;
using System.Threading.Tasks;

namespace Meeting.Core.Common.Abstractions.FrameCapture
{
    public abstract partial class CaptureFramesServiceAbstract
    {
        public abstract Guid CreateCaptureArea();
        public abstract Task<Guid> CreateCaptureAreaAsync();

        public abstract void DestroyCaptureArea(Guid captureAreaGuid);
        public abstract Task DestroyCaptureAreaAsync(Guid captureAreaGuid);

        public abstract void TurnOnCaptureArea(Guid captureAreaGuid);
        public abstract Task TurnOnCaptureAreaAsync(Guid captureAreaGuid);

        public abstract void TurnOffCaptureArea(Guid captureAreaGuid);
        public abstract Task TurnOffCaptureAreaAsync(Guid captureAreaGuid);

        public abstract Task CaptureFrameAreasSubscribeAsync();
        public abstract void CaptureFrameAreasUnsubscribe();

        public abstract void SendFrame(byte[] bytes, Guid captureArea, DateTime dateTime);
        public abstract Task SendFrameAsync(byte[] bytes, Guid captureArea, DateTime dateTime);

        public abstract Task CaptureFramesSubscribeAsync();

        public abstract void CaptureFramesUnsubscribe();
    }
}
