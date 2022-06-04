using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Meeting.Business.Common.Abstractions.CameraCapture;
using System;
using MeetingGrpc.Protos;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FrameCaptureClient = MeetingGrpc.Protos.FrameCapture.FrameCaptureClient;
using Meeting.Business.Common.Abstractions.FrameCapture;

namespace Meeting.Business.GrpcClient
{
    public class FrameCaptureService : FrameCaptureServiceAbstract
    {
        private readonly CancellationTokenSource chatCancelationToken = new CancellationTokenSource();

        private readonly FrameCaptureClient _client;

        public FrameCaptureService(FrameCaptureClient client, Guid currentUserGuid)
            : base(currentUserGuid)
        {
            _client = client;
            _currentUserGuidString = currentUserGuid.ToString();
        }

        public override async Task SendOwnCameraCaptureAsync(Stream stream)
        {
            //await _client.SendCameraFrameAsync(new CameraCapture()
            //{
            //    UserGuid = _currentUserGuidString,
            //    CaptureFrame = ByteString.FromStream(stream)
            //});
        }

        public override Task UsersCameraCaptureSubscribeAsync()
        {
            //var call = _client.CameraCaptureSubscribe(new Empty());

            //return call.ResponseStream
            //    .ToAsyncEnumerable()
            //    .Finally(() => call.Dispose())
            //    .ForEachAsync((x) =>
            //    { 
            //        RaiseCameraFrameChangedAction(Guid.Parse(x.UserGuid), x.CaptureFrame.ToByteArray());
            //    }, chatCancelationToken.Token);

            return null;
        }

        public override Task UsersCameraCaptureUnsubscribeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
