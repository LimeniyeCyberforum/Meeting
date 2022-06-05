using Meeting.Business.Common.EventArgs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meeting.Business.Common.Abstractions.FrameCapture
{
    public abstract partial class CaptureFrameServiceAbstract
    {
        protected List<Guid> activeCaptureFrames = new List<Guid>();

        public IReadOnlyList<Guid> ActiveCaptureFrames { get; }

        public CaptureFrameServiceAbstract()
        {
            ActiveCaptureFrames = new List<Guid>(activeCaptureFrames);
        }
    }
}
