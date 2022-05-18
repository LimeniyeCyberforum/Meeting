using Meeting.Abstractions.Interfaces.Messanger;
using Meeting.Abstractions.Messanger;
using System;
using System.Threading.Tasks;

namespace Meeting.Grpc.Messanger
{
    internal class MessageServiceGrpc : MessageServiceAbstract, IMessageService
    {
        public override void SendMessage(Guid guid, string username, string message)
        {
            throw new NotImplementedException();
        }

        public override Task SendMessageAsync(Guid guid, string username, string message)
        {
            throw new NotImplementedException();
        }

        public MessageServiceGrpc()
            :base()
        {

        }
    }
}
