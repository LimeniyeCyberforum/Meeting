using System;
using System.IO;
using System.Threading.Tasks;

namespace MeetingCommon.Abstractions.CameraCapture
{
    public delegate void CameraFrameChangedHandler(object sender, Guid userGuid, byte[] frameBytes);

    public abstract class CameraCaptureServiceAbstract
    {
        public Guid CurrentUserGuid { get; }

        public event CameraFrameChangedHandler CameraFrameChanged;

        public CameraCaptureServiceAbstract(Guid currentUserGuid)
        {
            CurrentUserGuid = currentUserGuid;
        }

        protected void RaiseCameraFrameChangedAction(Guid userGuid, byte[] frameBytes)
        {
            CameraFrameChanged?.Invoke(this, userGuid, frameBytes);
        }

        public abstract Task SendOwnCameraCaptureAsync(Stream stream);

        public abstract Task UsersCameraCaptureSubscribeAsync();

        public abstract Task UsersCameraCaptureUnsubscribeAsync();
    }
}
