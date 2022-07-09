using Meeting.Core.Common.DataTypes;
using Meeting.Core.Common.EventArgs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meeting.Core.Common
{
    public interface ICaptureFramesService : IDisposable
    {
        event EventHandler<CaptureFrameEventArgs> CaptureFrameChanged;
        event EventHandler<CaptureFrameStateEventArgs> CaptureFrameStateChanged;

        IReadOnlyDictionary<Guid, CaptureFrameAreaDto> ActiveCaptureFrames { get; }

        Guid CreateCaptureArea();
        Task<Guid> CreateCaptureAreaAsync();

        void DestroyCaptureArea(Guid captureAreaGuid);
        Task DestroyCaptureAreaAsync(Guid captureAreaGuid);

        void TurnOnCaptureArea(Guid captureAreaGuid);
        Task TurnOnCaptureAreaAsync(Guid captureAreaGuid);

        void TurnOffCaptureArea(Guid captureAreaGuid);
        Task TurnOffCaptureAreaAsync(Guid captureAreaGuid);

        Task CaptureFrameAreasSubscribeAsync();
        void CaptureFrameAreasUnsubscribe();

        void SendFrame(byte[] bytes, Guid captureArea, DateTime dateTime);
        Task SendFrameAsync(byte[] bytes, Guid captureArea, DateTime dateTime);

        Task CaptureFramesSubscribeAsync();
        void CaptureFramesUnsubscribe();
    }
}
