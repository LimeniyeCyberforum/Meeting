using GrpcCommon;
using GrpsServer.Model;
using System.ComponentModel.Composition;

namespace GrpsServer.Persistence
{
    [Export(typeof(IChatLogRepository))]
    public class ChatRepository : IChatLogRepository
    {
        private readonly List<MessageFromLobby> localStorage = new List<MessageFromLobby>(); // dummy on memory storage

        public void Add(MessageFromLobby message)
        {
            localStorage.Add(message);
        }

        public IEnumerable<MessageFromLobby> GetAll()
        {
            return localStorage.AsReadOnly();
        }
    }
}
