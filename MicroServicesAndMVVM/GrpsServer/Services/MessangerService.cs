using Grpc.Core;
using GrpsServer;
using Google.Protobuf.WellKnownTypes;

namespace GrpsServer.Services
{
    public class MessangerService : Messanger.MessangerBase
    {
        private readonly ILogger<MessangerService> _logger;
        public MessangerService(ILogger<MessangerService> logger)
        {
            _logger = logger;
        }

        public override Task<MessageResponse> SendMessage(MessageRequest request, ServerCallContext context)
        {
            return Task.FromResult(new MessageResponse
            {
                Time = Timestamp.FromDateTime(DateTime.UtcNow),
                Username = request.Username,
            });
        }

        public override async Task MessageStream(IAsyncStreamReader<MessageRequest> requestStream, 
            IServerStreamWriter<MessageReplay> responseStream, ServerCallContext context)
        {
            await foreach (var request in requestStream.ReadAllAsync())
            {
                await responseStream.WriteAsync(new MessageReplay()
                {
                    Username = request.Username,
                    Message = request.Message,
                    Time = Timestamp.FromDateTime(DateTime.UtcNow)
                });
            }
        }

        public override async Task CameraCaptureStream(IAsyncStreamReader<CameraCaptureTest> requestStream,
            IServerStreamWriter<CameraCaptureTest> responseStream, ServerCallContext context)
        {
            await foreach (var request in requestStream.ReadAllAsync())
            {
                await responseStream.WriteAsync(new CameraCaptureTest()
                {
                    CaptureFrame = request.CaptureFrame
                });
            }
        }
    }
}