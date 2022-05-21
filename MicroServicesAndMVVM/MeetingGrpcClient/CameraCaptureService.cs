using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using GrpcCommon;
using MeetingCommon.Abstractions.CameraCapture;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingGrpcClient
{
    public class CameraCaptureService : CameraCaptureServiceAbstract
    {
        private readonly CancellationTokenSource chatCancelationToken = new CancellationTokenSource();

        private readonly Meeting.MeetingClient _client;

        private readonly string _currentUserGuidString;

        public CameraCaptureService(Meeting.MeetingClient client, Guid currentUserGuid)
            : base(currentUserGuid)
        {
            _client = client;
            _currentUserGuidString = currentUserGuid.ToString();
        }

        public override async Task SendOwnCameraCaptureAsync(MemoryStream stream)
        {
            await _client.SendCameraFrameAsync(new CameraCapture()
            {
                UserGuid = _currentUserGuidString,
                CaptureFrame = ByteString.FromStream(stream)
            });
        }

        public override Task UsersCameraCaptureSubscribeAsync()
        {
            var call = _client.CameraCaptureSubscribe(new Empty());

            return call.ResponseStream
                .ToAsyncEnumerable()
                .Finally(() => call.Dispose())
                .ForEachAsync((x) =>
                { 
                    RaiseCameraFrameChangedAction(Guid.Parse(x.UserGuid), x.CaptureFrame.ToByteArray());
                }, chatCancelationToken.Token);
        }

        public override Task UsersCameraCaptureUnsubscribeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
