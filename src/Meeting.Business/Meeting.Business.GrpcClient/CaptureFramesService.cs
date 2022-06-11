using System;
using Grpc.Core;
using System.Linq;
using Google.Protobuf;
using MeetingGrpc.Protos;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Meeting.Business.Common.Abstractions.FrameCapture;
using Meeting.Business.Common.Abstractions.Users;
using Meeting.Business.Common.DataTypes;
using CaptureFramesClient = MeetingGrpc.Protos.CaptureFrames.CaptureFramesClient;

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

        #region Subscriptions

        public override Task CaptureFrameAreasSubscribeAsync()
        {
            var call = _client.CaptureFrameAreasSubscribe(new Empty());

            return call.ResponseStream
                .ToAsyncEnumerable()
                .Finally(() => call.Dispose())
                .ForEachAsync((x) =>
                {
                    var areaGuid = Guid.Parse(x.CatureAreaGuid);
                    var ownerGuid = Guid.Parse(x.OwnerGuid);

                    SwitchCaptureFrameStateChanged(areaGuid, ownerGuid, x.Action, x.Time.ToDateTime());

                }, chatCancelationToken.Token);
        }

        private void SwitchCaptureFrameStateChanged(Guid areaGuid, Guid ownerGuid, CaptureStateAction action, DateTime dateTime)
        {
            switch (action)
            {
                case CaptureStateAction.Disabled:
                    var disabledCaptureFrame = new CaptureFrameAreaDto(areaGuid, ownerGuid, false);
                    activeCaptureFrames[areaGuid] = disabledCaptureFrame;
                    RaiseCaptureFrameStateChangedAction(ownerGuid, areaGuid, Common.EventArgs.CaptureFrameState.Disabled, dateTime);
                    break;
                case CaptureStateAction.Enabled:
                    var enabledCaptureFrame = new CaptureFrameAreaDto(areaGuid, ownerGuid, true);
                    activeCaptureFrames[areaGuid] = enabledCaptureFrame;
                    RaiseCaptureFrameStateChangedAction(ownerGuid, areaGuid, Common.EventArgs.CaptureFrameState.Enabled, dateTime);
                    break;
                case CaptureStateAction.Created:
                    var createdCaptureFrame = new CaptureFrameAreaDto(areaGuid, ownerGuid, true);
                    if (!_usersService.Users.ContainsKey(ownerGuid))
                    {
                        activeCaptureFrames.Add(areaGuid, createdCaptureFrame);
                        RaiseCaptureFrameStateChangedAction(ownerGuid, areaGuid, Common.EventArgs.CaptureFrameState.Created, dateTime);
                    }
                    break;
                case CaptureStateAction.Removed:
                    var newCaptureFrame = new CaptureFrameAreaDto(areaGuid, ownerGuid, false);
                    if (!_usersService.Users.ContainsKey(ownerGuid))
                    {
                        activeCaptureFrames.Remove(areaGuid);
                        RaiseCaptureFrameStateChangedAction(ownerGuid, areaGuid, Common.EventArgs.CaptureFrameState.Removed, dateTime);
                        return;
                    }
                    break;
            }
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
                    var areaGuid = Guid.Parse(x.CatureAreaGuid);
                    if (activeCaptureFrames.TryGetValue(areaGuid, out CaptureFrameAreaDto value))
                    {
                        if (value.IsActive)
                        {
                            RaiseCaptureFrameChangedAction(areaGuid, x.Bytes.ToByteArray(), x.Time.ToDateTime());
                        }
                    }
                },
                chatCancelationToken.Token);
        }

        public override void CaptureFramesUnsubscribe()
        {
            throw new NotImplementedException();
        }

        #endregion

        public override Guid CreateCaptureArea()
        {
            return Guid.Parse(_client.CreateCaptureArea(DateTime.UtcNow.ToTimestamp(), _metadata).AreaGuid);
        }

        public override async Task<Guid> CreateCaptureAreaAsync()
        {
            var res = await _client.CreateCaptureAreaAsync(DateTime.UtcNow.ToTimestamp(), _metadata);
            return Guid.Parse(res.AreaGuid);
        }


        public override void DestroyCaptureArea(Guid captureAreaGuid)
        {
            _client.DestroyCaptureArea(new CaptureAreaRequest
            {
                AreaGuid = captureAreaGuid.ToString(),
                Time = DateTime.UtcNow.ToTimestamp()
            }, _metadata);
        }

        public override async Task DestroyCaptureAreaAsync(Guid captureAreaGuid)
        {
            await _client.DestroyCaptureAreaAsync(new CaptureAreaRequest
            {
                AreaGuid = captureAreaGuid.ToString(),
                Time = DateTime.UtcNow.ToTimestamp()
            }, _metadata);
        }


        public override void TurnOnCaptureArea(Guid captureAreaGuid)
        {
            _client.TurnOnCaptureArea(new CaptureAreaRequest
            {
                AreaGuid = captureAreaGuid.ToString(),
                Time = DateTime.UtcNow.ToTimestamp()
            }, _metadata);
        }

        public override async Task TurnOnCaptureAreaAsync(Guid captureAreaGuid)
        {
            await _client.TurnOnCaptureAreaAsync(new CaptureAreaRequest
            {
                AreaGuid = captureAreaGuid.ToString(),
                Time = DateTime.UtcNow.ToTimestamp()
            }, _metadata);
        }


        public override void TurnOffCaptureArea(Guid captureAreaGuid)
        {
            _client.TurnOffCaptureArea(new CaptureAreaRequest
            {
                AreaGuid = captureAreaGuid.ToString(),
                Time = DateTime.UtcNow.ToTimestamp()
            }, _metadata);
        }

        public override async Task TurnOffCaptureAreaAsync(Guid captureAreaGuid)
        {
            await _client.TurnOffCaptureAreaAsync(new CaptureAreaRequest
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
