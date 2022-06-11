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
using Meeting.Business.Common.Abstractions.Users;
using Meeting.Business.Common.DataTypes;

namespace Meeting.Business.GrpcClient
{
    public class CaptureFramesService : CaptureFramesServiceAbstract
    {
        private readonly Empty empty = new Empty();
        private readonly CancellationTokenSource chatCancelationToken = new CancellationTokenSource();

        private readonly CaptureFramesClient _client;
        private readonly UsersServiceAbstract _usersService;
        private Metadata _metadata;

        public CaptureFramesService(CaptureFramesClient client, UsersServiceAbstract usersService)
            : base()
        {
            _client = client;
            _usersService = usersService;
        }

        public void UpdateMetadata(Metadata metadata)
        {
            _metadata = metadata;
        }

        public override Task CaptureFrameAreasSubscribeAsync()
        {
            var call = _client.CaptureFrameAreasSubscribe(new Empty());

            return call.ResponseStream
                .ToAsyncEnumerable()
                .Finally(() => call.Dispose())
                .ForEachAsync((x) =>
                {
                    var areadGuid = Guid.Parse(x.CatureAreaGuid);
                    var ownerGuid = Guid.Parse(x.OwnerGuid);
                    if (x.IsOn)
                    {
                        var newCaptureFrame = new CaptureFrameAreaDto(areadGuid, ownerGuid, true);
                        if (!_usersService.Users.ContainsKey(ownerGuid))
                        {
                            activeCaptureFrames.Add(areadGuid, newCaptureFrame);
                            RaiseCaptureFrameStateChangedAction(ownerGuid, areadGuid, Common.EventArgs.CaptureFrameState.Created, x.Time.ToDateTime());
                        }
                        activeCaptureFrames[areadGuid] = newCaptureFrame;
                        RaiseCaptureFrameStateChangedAction(ownerGuid, areadGuid, Common.EventArgs.CaptureFrameState.Enabled, x.Time.ToDateTime());
                    }
                    else
                    {
                        var newCaptureFrame = new CaptureFrameAreaDto(areadGuid, ownerGuid, false);
                        if (!_usersService.Users.ContainsKey(ownerGuid))
                        {
                            activeCaptureFrames.Remove(areadGuid);
                            RaiseCaptureFrameStateChangedAction(ownerGuid, areadGuid, Common.EventArgs.CaptureFrameState.Removed, x.Time.ToDateTime());
                            return;
                        }
                        activeCaptureFrames[areadGuid] = newCaptureFrame;
                        RaiseCaptureFrameStateChangedAction(ownerGuid, areadGuid, Common.EventArgs.CaptureFrameState.Disabled, x.Time.ToDateTime());
                    }
                }, chatCancelationToken.Token);
        }

        public override void CaptureFrameAreasUnsubscribe()
        {
            throw new NotImplementedException();
        }

        public override Task CaptureFramesSubscribeAsync()
        {
            var call = _client.CaptureFramesSubscribe(new Empty());

            return call.ResponseStream
                .ToAsyncEnumerable()
                .Finally(() => call.Dispose())
                .ForEachAsync((x) =>
                {
                    //activeCaptureFrames.Add()
                    RaiseCaptureFrameChangedAction(Guid.Parse(x.CatureAreaGuid), x.Bytes.ToByteArray(), x.Time.ToDateTime());
                },
                chatCancelationToken.Token);
        }

        public override void CaptureFramesUnsubscribe()
        {
            throw new NotImplementedException();
        }

        public override Guid CreateCaptureArea()
        {
            return Guid.Parse(_client.CreateCaptureArea(DateTime.UtcNow.ToTimestamp(), _metadata).AreaGuid);
        }

        public override async Task<Guid> CreateCaptureAreaAsync()
        {
            var areaGuid = await _client.CreateCaptureAreaAsync(DateTime.UtcNow.ToTimestamp(), _metadata);
            return Guid.Parse(areaGuid.AreaGuid);
        }

        public override void DestroyCaptureArea(Guid captureAreaGuid)
        {
            _client.DestroyCaptureArea(new DestroyCaptureAreaRequest
            {
                AreaGuid = captureAreaGuid.ToString(),
                Time = DateTime.UtcNow.ToTimestamp()
            }, _metadata);
        }

        public override async Task DestroyCaptureAreaAsync(Guid captureAreaGuid)
        {
            await _client.DestroyCaptureAreaAsync(new DestroyCaptureAreaRequest
            {
                AreaGuid = captureAreaGuid.ToString(),
                Time = DateTime.UtcNow.ToTimestamp()
            }, _metadata);
        }

        public override void SendFrame(byte[] bytes, Guid captureArea, DateTime dateTime)
        {
            _client.SendCaptureFrame(new CaptureFrame
            {
                CatureAreaGuid = captureArea.ToString(),
                Bytes = ByteString.CopyFrom(bytes),
                Time = dateTime.ToTimestamp()
            }, _metadata);
        }

        public override async Task SendFrameAsync(byte[] bytes, Guid captureArea, DateTime dateTime)
        {
            await _client.SendCaptureFrameAsync(new CaptureFrame
            {
                CatureAreaGuid = captureArea.ToString(),
                Bytes = ByteString.CopyFrom(bytes),
                Time = dateTime.ToTimestamp()
            }, _metadata);
        }
    }
}
