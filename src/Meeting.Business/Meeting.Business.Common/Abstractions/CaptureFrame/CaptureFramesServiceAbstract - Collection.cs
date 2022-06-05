using System;
using System.Collections.Generic;

namespace Meeting.Business.Common.Abstractions.FrameCapture
{
    public abstract partial class CaptureFramesServiceAbstract
    {
        protected List<Guid> activeCaptureFrames = new List<Guid>();

        public IReadOnlyList<Guid> ActiveCaptureFrames { get; }

        public CaptureFramesServiceAbstract()
        { 
            ActiveCaptureFrames = new List<Guid>(activeCaptureFrames);
        }
    }
}
