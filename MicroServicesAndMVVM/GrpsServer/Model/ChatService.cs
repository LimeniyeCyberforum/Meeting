using GrpcCommon;
using System.Reactive.Linq;

namespace GrpsServer.Model
{
    public class ChatService
    {
        private readonly ILogger<ChatService> _logger;

        private readonly IChatLogRepository _repository;

        public ChatService(ILogger<ChatService> logger, IChatLogRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        private event Action<MessageFromLobby> Added;

        public void Add(MessageFromLobby chatLog)
        {
            _logger.LogInformation($"{chatLog.Username}: {chatLog.Message}\n{chatLog.Time}");

            _repository.Add(chatLog);
            Added?.Invoke(chatLog);
        }

        public IObservable<MessageFromLobby> GetChatLogsAsObservable()
        {
            var oldLogs = _repository.GetAll().ToObservable();
            var newLogs = Observable.FromEvent<MessageFromLobby>((x) => Added += x, (x) => Added -= x);

            return oldLogs.Concat(newLogs);
        }
    }
}
