using Meeting.Business.Common.EventArgs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Meeting.Business.Common.Abstractions.FrameCapture
{
    public abstract class FrameCaptureServiceAbstract
    {
        public Guid CurrentUserGuid { get; }

        public event EventHandler<FrameCaptureEventArgs> FrameCaptureChanged;
        public event EventHandler<FrameCaptureStateEventArgs> FrameCaptureStateChanged;

        public FrameCaptureServiceAbstract(Guid currentUserGuid)
        {
            CurrentUserGuid = currentUserGuid;
        }

        protected void RaiseFrameCaptureChangedAction(Guid frameAreaGuid, byte[] frameBytes, DateTime dateTime)
        {
            FrameCaptureChanged?.Invoke(this, new FrameCaptureEventArgs(frameAreaGuid, frameBytes, dateTime));
        }

        protected void RaiseFrameCaptureStateChangedAction(Guid frameAreaGuid, bool newStateIsOn)
        {
            FrameCaptureStateChanged?.Invoke(this, new FrameCaptureStateEventArgs(frameAreaGuid, newStateIsOn));
        }

        public abstract Task SendOwnCameraFrameAsync(Stream stream);

        public abstract Task UsersCameraCaptureSubscribeAsync();

        public abstract Task UsersCameraCaptureUnsubscribeAsync();
    }
}
