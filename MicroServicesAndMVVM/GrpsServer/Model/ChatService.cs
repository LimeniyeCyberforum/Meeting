using GrpcCommon;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;

namespace GrpsServer.Model
{
    [Export]
    public class ChatService
    {
        private readonly ILogger<ChatService> _logger;

        public ChatService(ILogger<ChatService> logger)
        {
            _logger = logger;
        }

        [Import]
        private IChatLogRepository repository = null;
        private event Action<MessageFromLobby> Added;

        public void Add(MessageFromLobby chatLog)
        {
            _logger.LogInformation($"{chatLog}");

            repository.Add(chatLog);
            Added?.Invoke(chatLog);
        }

        public IObservable<MessageFromLobby> GetChatLogsAsObservable()
        {
            var oldLogs = repository.GetAll().ToObservable();
            var newLogs = Observable.FromEvent<MessageFromLobby>((x) => Added += x, (x) => Added -= x);

            return oldLogs.Concat(newLogs);
        }
    }
}
