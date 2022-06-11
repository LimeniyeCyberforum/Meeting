using Framework.Interfaces;
using Meeting.Business.Common.DataTypes;
using System;
using System.Collections.Generic;

namespace Meeting.Business.Common.Abstractions.FrameCapture
{
    public abstract partial class CaptureFramesServiceAbstract
    {
        protected HashSet<CaptureFrameAreaDto> activeCaptureFrames = new HashSet<CaptureFrameAreaDto>();

        public IReadonlyHashSet<CaptureFrameAreaDto> ActiveCaptureFrames { get; }

        public CaptureFramesServiceAbstract()
        { 
            ActiveCaptureFrames = new ReadonlyHashSet<CaptureFrameAreaDto>(activeCaptureFrames);
        }
    }
}
