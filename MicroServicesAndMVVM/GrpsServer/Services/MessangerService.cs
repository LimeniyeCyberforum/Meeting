using Grpc.Core;
using GrpsServer;

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
                
            });
        }

        public override async Task MessageStream(IAsyncStreamReader<MessageRequest> requestStream, 
            IServerStreamWriter<MessageResponse> responseStream, ServerCallContext context)
        {
            await foreach (var request in requestStream.ReadAllAsync())
            {
                await responseStream.WriteAsync(new MessageResponse()
                {
                    //Message = "Hello " + request.Name + " " + DateTime.UtcNow
                });
            }
        }
    }
}