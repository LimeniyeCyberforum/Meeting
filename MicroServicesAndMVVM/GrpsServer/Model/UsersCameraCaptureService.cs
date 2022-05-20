using GrpcCommon;
using GrpsServer.Services;
using System.ComponentModel.Composition;
using System.Reactive.Linq;

namespace GrpsServer.Model
{
    [Export]
    public class UsersCameraCaptureService
    {
        private readonly ILogger<MeetingService> _logger;

        public UsersCameraCaptureService(ILogger<MeetingService> logger)
        {
            _logger = logger;
        }

        [Import]
        private IUsersCameraCaptureService repository = null;
        private event Action<CameraCapture> Changed;

        public void AddOrUpdate(CameraCapture cameraCapture)
        {
            //_logger.LogInformation($"Added capture from: {cameraCapture.UserGuid}");
            repository.AddOrUpdate(cameraCapture);
            Changed?.Invoke(cameraCapture);
        }

        public IObservable<CameraCapture> GetUserCameraCapturesAsObservable()
        {
            var oldLogs = repository.GetAll().ToObservable();
            var newLogs = Observable.FromEvent<CameraCapture>((x) => Changed += x, (x) => Changed -= x);

            return oldLogs.Concat(newLogs);
        }
    }
}
