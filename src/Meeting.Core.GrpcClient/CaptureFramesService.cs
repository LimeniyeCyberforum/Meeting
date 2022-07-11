using System;
using Grpc.Core;
using System.Threading;
using Meeting.Core.Common;
using System.Collections.Generic;
using Meeting.Core.Common.DataTypes;
using Meeting.Core.Common.EventArgs;
using System.Collections.ObjectModel;
using MeetingProtobuf.Protos;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;
using System.Linq;
using Meeting.Core.GrpcClient.Util;
using CaptureFramesClient = MeetingProtobuf.Protos.CaptureFrames.CaptureFramesClient;

namespace Meeting.Core.GrpcClient
{
    internal sealed partial class CaptureFramesService : ICaptureFramesService
    {
        private Dictionary<Guid, CaptureFrameAreaDto> activeCaptureFrames = new Dictionary<Guid, CaptureFrameAreaDto>();
        private bool disposed = false;

        private CancellationTokenSource _captureFramesSubscriptionCancelationToken;
        private CancellationTokenSource _captureFramesSubscribeCancelationToken;

        private readonly CaptureFramesClient _client;
        private readonly IUsersService _usersService;
        private Metadata _metadata;

        public event EventHandler<CaptureFrameEventArgs> CaptureFrameChanged;
        public event EventHandler<CaptureFrameStateEventArgs> CaptureFrameStateChanged;

        public IReadOnlyDictionary<Guid, CaptureFrameAreaDto> ActiveCaptureFrames { get; }

        public CaptureFramesService(CaptureFramesClient client, IUsersService usersService)
            : base()
        {
            _client = client;
            _usersService = usersService;
            ActiveCaptureFrames = new ReadOnlyDictionary<Guid, CaptureFrameAreaDto>(activeCaptureFrames);
        }

        #region Methods

        public void UpdateMetadata(Metadata metadata)
        {
            _metadata = metadata;
        }

        public Guid CreateCaptureArea()
        {
            return Guid.Parse(_client.CreateCaptureArea(DateTime.UtcNow.ToTimestamp(), _metadata).AreaGuid);
        }

        public async Task<Guid> CreateCaptureAreaAsync()
        {
            var res = await _client.CreateCaptureAreaAsync(DateTime.UtcNow.ToTimestamp(), _metadata);
            return Guid.Parse(res.AreaGuid);
        }


        public void DestroyCaptureArea(Guid captureAreaGuid)
        {
            _client.DestroyCaptureArea(new CaptureAreaRequest
            {
                AreaGuid = captureAreaGuid.ToString(),
                Time = DateTime.UtcNow.ToTimestamp()
            }, _metadata);
        }

        public async Task DestroyCaptureAreaAsync(Guid captureAreaGuid)
        {
            await _client.DestroyCaptureAreaAsync(new CaptureAreaRequest
            {
                AreaGuid = captureAreaGuid.ToString(),
                Time = DateTime.UtcNow.ToTimestamp()
            }, _metadata);
        }


        public void TurnOnCaptureArea(Guid captureAreaGuid)
        {
            _client.TurnOnCaptureArea(new CaptureAreaRequest
            {
                AreaGuid = captureAreaGuid.ToString(),
                Time = DateTime.UtcNow.ToTimestamp()
            }, _metadata);
        }

        public async Task TurnOnCaptureAreaAsync(Guid captureAreaGuid)
        {
            await _client.TurnOnCaptureAreaAsync(new CaptureAreaRequest
            {
                AreaGuid = captureAreaGuid.ToString(),
                Time = DateTime.UtcNow.ToTimestamp()
            }, _metadata);
        }


        public void TurnOffCaptureArea(Guid captureAreaGuid)
        {
            _client.TurnOffCaptureArea(new CaptureAreaRequest
            {
                AreaGuid = captureAreaGuid.ToString(),
                Time = DateTime.UtcNow.ToTimestamp()
            }, _metadata);
        }

        public async Task TurnOffCaptureAreaAsync(Guid captureAreaGuid)
        {
            await _client.TurnOffCaptureAreaAsync(new CaptureAreaRequest
            {
                AreaGuid = captureAreaGuid.ToString(),
                Time = DateTime.UtcNow.ToTimestamp()
            }, _metadata);
        }


        public void SendFrame(byte[] bytes, Guid captureArea, DateTime dateTime)
        {
            _client.SendCaptureFrame(new CaptureFrame
            {
                CatureAreaGuid = captureArea.ToString(),
                Bytes = ByteString.CopyFrom(bytes),
                Time = dateTime.ToTimestamp()
            }, _metadata);
        }

        public async Task SendFrameAsync(byte[] bytes, Guid captureArea, DateTime dateTime)
        {
            await _client.SendCaptureFrameAsync(new CaptureFrame
            {
                CatureAreaGuid = captureArea.ToString(),
                Bytes = ByteString.CopyFrom(bytes),
                Time = dateTime.ToTimestamp()
            }, _metadata);
        }

        public Task CaptureFrameAreasSubscribeAsync()
        {
            if (_captureFramesSubscriptionCancelationToken is not null
                && !_captureFramesSubscriptionCancelationToken.IsCancellationRequested)
                return Task.CompletedTask;

            var call = _client.CaptureFrameAreasSubscribe(new Empty());
            _captureFramesSubscriptionCancelationToken = new CancellationTokenSource();

            return call.ResponseStream
                .ToAsyncEnumerable()
                .Finally(() => call.Dispose())
                .ForEachAsync((x) =>
                {
                    var areaGuid = Guid.Parse(x.CatureAreaGuid);
                    var ownerGuid = Guid.Parse(x.OwnerGuid);

                    SwitchCaptureFrameStateChanged(areaGuid, ownerGuid, x.Action, x.Time.ToDateTime());
                }, _captureFramesSubscriptionCancelationToken.Token);
        }

        private void SwitchCaptureFrameStateChanged(Guid areaGuid, Guid ownerGuid, CaptureStateAction action, DateTime dateTime)
        {
            switch (action)
            {
                case CaptureStateAction.Disabled:
                    var disabledCaptureFrame = new CaptureFrameAreaDto(areaGuid, ownerGuid, false);
                    activeCaptureFrames[areaGuid] = disabledCaptureFrame;
                    CaptureFrameStateChanged?.Invoke(this, new CaptureFrameStateEventArgs(ownerGuid, areaGuid, CaptureFrameState.Disabled, dateTime));
                    break;
                case CaptureStateAction.Enabled:
                    var enabledCaptureFrame = new CaptureFrameAreaDto(areaGuid, ownerGuid, true);
                    activeCaptureFrames[areaGuid] = enabledCaptureFrame;
                    CaptureFrameStateChanged?.Invoke(this, new CaptureFrameStateEventArgs(ownerGuid, areaGuid, CaptureFrameState.Enabled, dateTime));
                    break;
                case CaptureStateAction.Created:
                    var createdCaptureFrame = new CaptureFrameAreaDto(areaGuid, ownerGuid, true);
                    if (!_usersService.Users.ContainsKey(ownerGuid))
                    {
                        activeCaptureFrames.Add(areaGuid, createdCaptureFrame);
                        CaptureFrameStateChanged?.Invoke(this, new CaptureFrameStateEventArgs(ownerGuid, areaGuid, CaptureFrameState.Created, dateTime));
                    }
                    break;
                case CaptureStateAction.Removed:
                    var newCaptureFrame = new CaptureFrameAreaDto(areaGuid, ownerGuid, false);
                    if (!_usersService.Users.ContainsKey(ownerGuid))
                    {
                        activeCaptureFrames.Remove(areaGuid);
                        CaptureFrameStateChanged?.Invoke(this, new CaptureFrameStateEventArgs(ownerGuid, areaGuid, CaptureFrameState.Removed, dateTime));
                        return;
                    }
                    break;
            }
        }

        public void CaptureFrameAreasUnsubscribe()
        {
            _captureFramesSubscriptionCancelationToken.Cancel();
            _captureFramesSubscriptionCancelationToken.Dispose();
        }

        public Task CaptureFramesSubscribeAsync()
        {
            if (_captureFramesSubscribeCancelationToken is not null && !_captureFramesSubscribeCancelationToken.IsCancellationRequested)
                return Task.CompletedTask;

            var call = _client.CaptureFramesSubscribe(new Empty());
            _captureFramesSubscribeCancelationToken = new CancellationTokenSource();

            return call.ResponseStream
                .ToAsyncEnumerable()
                .Finally(() => call.Dispose())
                .ForEachAsync((x) =>
                {
                    var areaGuid = Guid.Parse(x.CatureAreaGuid);
                    if (activeCaptureFrames.TryGetValue(areaGuid, out var value))
                    {
                        if (value.IsActive)
                        {
                            CaptureFrameChanged?.Invoke(this, new CaptureFrameEventArgs(areaGuid, x.Bytes.ToByteArray(), x.Time.ToDateTime()));
                        }
                    }
                },
                _captureFramesSubscribeCancelationToken.Token);
        }

        public void CaptureFramesUnsubscribe()
        {
            _captureFramesSubscribeCancelationToken.Cancel();
            _captureFramesSubscribeCancelationToken.Dispose();
        }

        #endregion

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                CaptureFrameAreasUnsubscribe();
                CaptureFramesUnsubscribe();
            }
            disposed = true;
        }
    }
}
