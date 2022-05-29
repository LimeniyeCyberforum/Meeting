using GrpcCommon;

namespace GrpsServer.Model
{
    public interface IChatLogRepository
    {
        void Add(MessageFromLobby chatLog);
        IEnumerable<MessageFromLobby> GetAll();
    }
}
