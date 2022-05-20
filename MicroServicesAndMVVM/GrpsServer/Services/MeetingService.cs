using Grpc.Core;
using GrpsServer;
using Google.Protobuf.WellKnownTypes;
using GrpcCommon;
using GrpsServer.Model;
using System.Reactive.Linq;
using System.Linq;
using System.ComponentModel.Composition;

namespace GrpsServer.Services
{
    public class MeetingService : Meeting.MeetingBase
    {
        private readonly ILogger<MeetingService> _logger;
        public MeetingService(ILogger<MeetingService> logger)
        {
            _logger = logger;
        }

        [Import]
        private ChatService chatService = null;



        private readonly Dictionary<Guid, string> users = new Dictionary<Guid, string>();
        private const string TEST_LOBBY_PASSWORD = "limeniye";


        public override Task<ConnectResponse> Connect(ConnectRequest request, ServerCallContext context)
        {
            if (request.LobbyPassword != TEST_LOBBY_PASSWORD)
            {
                return Task.FromResult(new ConnectResponse
                {
                    IsSuccessfully = false,
                    ErrorMessage = "Incorrect password"
                });
            }

            if (!users.TryAdd(Guid.NewGuid(), request.Username))
            {
                return Task.FromResult(new ConnectResponse
                {
                    IsSuccessfully = false,
                    ErrorMessage = "Something whent wrong. Lets try again."
                });
            }

            return Task.FromResult(new ConnectResponse
            {
                IsSuccessfully = true,
                Guid = Guid.NewGuid().ToString()
            });
        }

        public override async Task MessagesSubscribe(Empty request, IServerStreamWriter<MessageFromLobby> responseStream, ServerCallContext context)
        {
            var peer = context.Peer;
            _logger.LogInformation($"{peer} subscribes.");

            context.CancellationToken.Register(() => _logger.LogInformation($"{peer} cancels subscription."));

            try
            {
                await chatService.GetChatLogsAsObservable()
                    .ToAsyncEnumerable()
                    .ForEachAwaitAsync(async (x) => await responseStream.WriteAsync(x), context.CancellationToken)
                    .ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation($"{peer} unsubscribed.");
            }
        }

        //public override Task<MessageResponse> SendMessage(MessageRequest request, ServerCallContext context)
        //{
        //    return Task.FromResult(new MessageResponse
        //    {
        //        Time = Timestamp.FromDateTime(DateTime.UtcNow),
        //        Username = request.Username,
        //    });
        //}

        //public override async Task MessageStream(IAsyncStreamReader<MessageRequest> requestStream, 
        //    IServerStreamWriter<MessageReplay> responseStream, ServerCallContext context)
        //{
        //    await foreach (var request in requestStream.ReadAllAsync())
        //    {
        //        await responseStream.WriteAsync(new MessageReplay()
        //        {
        //            Username = request.Username,
        //            Message = request.Message,
        //            Time = Timestamp.FromDateTime(DateTime.UtcNow)
        //        });
        //    }
        //}

        //public override async Task CameraCaptureStream(IAsyncStreamReader<CameraCaptureTest> requestStream,
        //    IServerStreamWriter<CameraCaptureTest> responseStream, ServerCallContext context)
        //{
        //    await foreach (var request in requestStream.ReadAllAsync())
        //    {
        //        await responseStream.WriteAsync(new CameraCaptureTest()
        //        {
        //            CaptureFrame = request.CaptureFrame
        //        });
        //    }
        //}
    }
}