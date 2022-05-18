using Meeting.Abstractions.Interfaces.Messanger;
using Meeting.Grpc.Messanger;
using System;

namespace Meeting
{
    public enum DataInteractingType
    {
        Local,
        Wcf,
        Grpc
    }

    public sealed class MeetingService
    {
        private static MeetingService _instance = null;

        public IMessageService MeetingConnector { get; private set; }

        protected MeetingService() { }

        public static MeetingService Instance(DataInteractingType dataInteracting)
        {
            if (_instance == null)
            {
                _instance = new MeetingService();
                _instance.Initialize(dataInteracting);

                return _instance;
            }
            else
            {
                return null;
            }
        }

        private void Initialize(DataInteractingType dataInteracting)
        {
            switch (dataInteracting)
            {
                case DataInteractingType.Grpc:
                    MeetingConnector = new MessageServiceGrpc();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
