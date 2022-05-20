using GrpcCommon;

namespace GrpsServer.Model
{
    public interface IUsersCameraCaptureService
    {
        void AddOrUpdate(CameraCapture cameraCapture);
        IEnumerable<CameraCapture> GetAll();
    }
}
