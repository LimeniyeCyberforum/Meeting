using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Meeting.Business.Common.Abstractions.CameraCapture;
using System;
using MeetingGrpc.Protos;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MeetingClient = MeetingGrpc.Protos.Meeting.MeetingClient;


namespace Meeting.Business.GrpcClient
{
    public class CameraCaptureService : CameraCaptureServiceAbstract
    {
        private readonly CancellationTokenSource chatCancelationToken = new CancellationTokenSource();

        private readonly MeetingClient _client;

        private readonly string _currentUserGuidString;

        public CameraCaptureService(MeetingClient client, Guid currentUserGuid)
            : base(currentUserGuid)
        {
            _client = client;
            _currentUserGuidString = currentUserGuid.ToString();
        }

        public override async Task SendOwnCameraCaptureAsync(Stream stream)
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
