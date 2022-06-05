using Meeting.Business.Common.DataTypes;
using System;
using System.Collections.Generic;

namespace Meeting.Business.Common.Abstractions.FrameCapture
{
    public abstract partial class CaptureFramesServiceAbstract
    {
        protected List<CaptureFrameAreaDto> activeCaptureFrames = new List<CaptureFrameAreaDto>();

        public IReadOnlyList<CaptureFrameAreaDto> ActiveCaptureFrames { get; }

        public CaptureFramesServiceAbstract()
        { 
            ActiveCaptureFrames = new List<CaptureFrameAreaDto>(activeCaptureFrames);
        }
    }
}
