using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Meeting.Business.Common.DataTypes;

namespace Meeting.Business.Common.Abstractions.FrameCapture
{
    public abstract partial class CaptureFramesServiceAbstract
    {
        protected Dictionary<Guid, CaptureFrameAreaDto> activeCaptureFrames = new Dictionary<Guid, CaptureFrameAreaDto>();

        public IReadOnlyDictionary<Guid, CaptureFrameAreaDto> ActiveCaptureFrames { get; }

        public CaptureFramesServiceAbstract()
        { 
            ActiveCaptureFrames = new ReadOnlyDictionary<Guid, CaptureFrameAreaDto>(activeCaptureFrames);
        }
    }
}
