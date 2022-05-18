using Common.EventArgs;
using Meeting.Abstractions.Interfaces.Messanger;
using Meeting.DataTypes.Messanger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meeting.Messanger
{
    public partial class MessageService : IMessageService
    {
        public void SendMessage(Guid guid, string username, string message)
        {
            throw new NotImplementedException();
        }

        public Task SendMessageAsync(Guid guid, string username, string message)
        {
            throw new NotImplementedException();
        }
    }
}
