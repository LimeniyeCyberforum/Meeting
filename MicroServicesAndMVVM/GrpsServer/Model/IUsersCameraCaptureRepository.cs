using GrpcCommon;

namespace GrpsServer.Model
{
    public interface IUsersCameraCaptureRepository
    {
        void AddFirstFrame(CameraCapture cameraCapture);
        // TODO : Temporary
        void AddOrUpdateFrame(CameraCapture cameraCapture);
        IEnumerable<CameraCapture> GetAll();
    }
}
