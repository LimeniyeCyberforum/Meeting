using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Meeting.Core.Common.DataTypes;

namespace Meeting.Core.Common.Abstractions.FrameCapture
{
    public abstract partial class CaptureFramesServiceAbstract : IDisposable
    {
        private bool disposed = false;

        protected Dictionary<Guid, CaptureFrameAreaDto> activeCaptureFrames = new Dictionary<Guid, CaptureFrameAreaDto>();

        public IReadOnlyDictionary<Guid, CaptureFrameAreaDto> ActiveCaptureFrames { get; }

        public CaptureFramesServiceAbstract()
        { 
            ActiveCaptureFrames = new ReadOnlyDictionary<Guid, CaptureFrameAreaDto>(activeCaptureFrames);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                CaptureFrameAreasUnsubscribe();
                CaptureFramesUnsubscribe();
            }
            disposed = true;
        }

        ~CaptureFramesServiceAbstract()
        {
            Dispose(disposing: false);
        }
    }
}
