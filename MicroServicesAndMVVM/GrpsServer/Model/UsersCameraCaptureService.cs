using GrpcCommon;
using System.Reactive.Linq;

namespace GrpsServer.Model
{
    public class UsersCameraCaptureService
    {
        private readonly IUsersCameraCaptureRepository _repository;
        private event Action<CameraCapture> FrameCaptureStreamStartedChaned;
        private event Action<CameraCapture> FrameCaptureStreamStodepChaned;
        private event Action<CameraCapture> FrameCaptureUpdated;

        public UsersCameraCaptureService(IUsersCameraCaptureRepository repository)
        {
            _repository = repository;
        }

        public void AddFirstFrame(CameraCapture cameraCapture)
        {
            _repository.AddFirstFrame(cameraCapture);
            FrameCaptureStreamStartedChaned?.Invoke(cameraCapture);
        }

        public void UpdateFrameCapture(CameraCapture cameraCapture)
        {
            // TODO : Temporary
            _repository.AddOrUpdateFrame(cameraCapture);
            FrameCaptureUpdated?.Invoke(cameraCapture);
        }


        public IObservable<CameraCapture> GetUserCameraCaptureStreamStartAsObservable()
        {
            var oldLogs = _repository.GetAll().ToObservable();
            var updated = Observable.FromEvent<CameraCapture>((x) => FrameCaptureStreamStartedChaned += x, (x) => FrameCaptureStreamStartedChaned -= x);

            return oldLogs.Concat(updated);
        }

        public IObservable<CameraCapture> GetUserCameraCaptureFrameUpdateAsObservable()
        {
            var oldLogs = _repository.GetAll().ToObservable();
            var updated = Observable.FromEvent<CameraCapture>((x) => FrameCaptureUpdated += x, (x) => FrameCaptureUpdated -= x);

            return oldLogs.Concat(updated);
        }
    }
}
