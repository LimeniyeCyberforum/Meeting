using GrpcCommon;
using GrpsServer.Model;
using System.ComponentModel.Composition;

namespace GrpsServer.Persistence
{
    public class UsersCameraCaptureRepository : IUsersCameraCaptureRepository
    {
        private readonly List<CameraCapture> localStorage = new List<CameraCapture>(); // dummy on memory storage

        public void AddFirstFrame(CameraCapture message)
        {
            localStorage.Add(message);
        }

        public void AddOrUpdateFrame(CameraCapture cameraCapture)
        {
            CameraCapture item = localStorage.FirstOrDefault(x => x.UserGuid == cameraCapture.UserGuid);
            if (item != null)
                item.CaptureFrame = cameraCapture.CaptureFrame;
            else
                localStorage.Add(cameraCapture);
        }

        public IEnumerable<CameraCapture> GetAll()
        {
            return localStorage.AsReadOnly();
        }
    }
}
