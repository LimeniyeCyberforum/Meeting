using System;
using Grpc.Core;
using System.IO;
using System.Linq;
using Google.Protobuf;
using MeetingGrpc.Protos;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Meeting.Business.Common.Abstractions.FrameCapture;
using CaptureFramesClient = MeetingGrpc.Protos.CaptureFrames.CaptureFramesClient;

namespace Meeting.Business.GrpcClient
{
    public class CaptureFramesService : CaptureFrameServiceAbstract
    {
        private readonly CancellationTokenSource chatCancelationToken = new CancellationTokenSource();

        private readonly CaptureFramesClient _client;
        private Metadata _metadata;

        public CaptureFramesService(CaptureFramesClient client)
            : base()
        {
            _client = client;
        }

        //public override async Task SendOwnCameraCaptureAsync(Stream stream)
        //{
        //    //await _client.SendCameraFrameAsync(new CameraCapture()
        //    //{
        //    //    UserGuid = _currentUserGuidString,
        //    //    CaptureFrame = ByteString.FromStream(stream)
        //    //});
        //}

        //public override Task UsersCameraCaptureSubscribeAsync()
        //{
        //    //var call = _client.CameraCaptureSubscribe(new Empty());

        //    //return call.ResponseStream
        //    //    .ToAsyncEnumerable()
        //    //    .Finally(() => call.Dispose())
        //    //    .ForEachAsync((x) =>
        //    //    { 
        //    //        RaiseCameraFrameChangedAction(Guid.Parse(x.UserGuid), x.CaptureFrame.ToByteArray());
        //    //    }, chatCancelationToken.Token);

        //    return null;
        //}

        //public override Task UsersCameraCaptureUnsubscribeAsync()
        //{
        //    throw new NotImplementedException();
        //}

        public void UpdateMetadata(Metadata metadata)
        {
            _metadata = metadata;
        }

        public override Task CaptureFrameAreasSubscribeAsync()
        {
            throw new NotImplementedException();
        }

        public override void CaptureFrameAreasUnsubscribe()
        {
            throw new NotImplementedException();
        }

        public override Task CaptureFramesSubscribeAsync()
        {
            throw new NotImplementedException();
        }

        public override void CaptureFramesUnsubscribe()
        {
            throw new NotImplementedException();
        }

        public override Guid CreateCaptureArea()
        {
            //_client.SwitchFrameCaptureState
            throw new NotImplementedException();

        }

        public override Task<Guid> CreateCaptureAreaAsync()
        {
            throw new NotImplementedException();
        }

        public override void DestroyCaptureArea(Guid captureAreaGuid)
        {
            throw new NotImplementedException();
        }

        public override Task DestroyCaptureAreaAsync(Guid captureAreaGuid)
        {
            throw new NotImplementedException();
        }

        public override void SendFrame(byte bytes, Guid captureArea)
        {
            throw new NotImplementedException();
        }

        public override Task SendFrameAsync(byte bytes, Guid captureArea)
        {
            throw new NotImplementedException();
        }
    }
}
