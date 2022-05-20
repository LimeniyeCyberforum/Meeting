using GrpcCommon;
using GrpsServer.Services;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Linq;

namespace GrpsServer.Model
{
    [Export]
    public class ChatService
    {
        [Import]
        private readonly ILogger<MeetingService> _logger;

        [Import]
        private IChatLogRepository repository = null;

        private event Action<MessageFromLobby> Added;

        public void Add(MessageFromLobby chatLog)
        {
            _logger?.LogInformation($"{chatLog.Username}: {chatLog.Message}\n{chatLog.Time}");

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
