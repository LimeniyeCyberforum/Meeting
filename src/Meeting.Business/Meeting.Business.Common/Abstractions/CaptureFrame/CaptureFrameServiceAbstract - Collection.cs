using Meeting.Business.Common.EventArgs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meeting.Business.Common.Abstractions.FrameCapture
{
    public abstract partial class CaptureFrameServiceAbstract
    {
        protected List<Guid> activeFrameCaptures = new List<Guid>();

        public IReadOnlyList<Guid> ActiveFrameCaptures { get; }

        public CaptureFrameServiceAbstract()
        { 
            ActiveFrameCaptures = new List<Guid>(activeFrameCaptures);
        }
    }
}
