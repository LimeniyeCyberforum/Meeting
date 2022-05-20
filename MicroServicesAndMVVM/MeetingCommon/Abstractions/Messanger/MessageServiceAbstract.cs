using System;
using System.IO;
using System.Threading.Tasks;

namespace MeetingCommon.Abstractions.Messanger
{
    public abstract partial class MessageServiceAbstract
    {
        //public abstract void SendMessage(Guid messageGuid, Guid userGuid, string message);

        public abstract Task SendMessageAsync(Guid messageGuid, Guid userGuid, string message);

        public abstract Task SendOwnCameraCaptureAsync(MemoryStream stream);


        public abstract Task ChatSubscribeAsync();

        public abstract Task ChatUnsubscribeAsync();

        public abstract Task UsersCameraCaptureSubscribeAsync();

        public abstract Task UsersCameraCaptureUnsubscribeAsync();

    }
}
