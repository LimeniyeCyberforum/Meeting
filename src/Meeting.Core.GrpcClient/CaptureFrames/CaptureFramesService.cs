using System;
using Grpc.Core;
using System.Threading;
using Meeting.Core.Common;
using System.Collections.Generic;
using Meeting.Core.Common.DataTypes;
using Meeting.Core.Common.EventArgs;
using System.Collections.ObjectModel;
using CaptureFramesClient = MeetingProtobuf.Protos.CaptureFrames.CaptureFramesClient;

namespace Meeting.Core.GrpcClient.CaptureFrames
{
    public sealed partial class CaptureFramesService : ICaptureFramesService
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

        ~CaptureFramesService()
        {
            Dispose(disposing: false);
        }
    }
}
