using ChatClient = MeetingProtobuf.Protos.Chat.ChatClient;
using UsersClient = MeetingProtobuf.Protos.Users.UsersClient;
using CaptureFramesClient = MeetingProtobuf.Protos.CaptureFrames.CaptureFramesClient;
using AuthorizationClient = MeetingProtobuf.Protos.Authorization.AuthorizationClient;


namespace Meeting.Core.GrpcClient
{
    public partial class MeetingService
    {
        private static MeetingService _instance;
        private static readonly object _locker = new object();
        private static bool isInitialized;

        private MeetingService()
        {
            Init();
        }

        public static MeetingService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MeetingService();

                return _instance;
            }
        }

        private void Init()
        {
            lock (_locker)
            {
                if (isInitialized)
                    return;

                var channel = GetGrpcChannel();
                _authorizationClient = new AuthorizationClient(channel);
                Users = new UsersService(new UsersClient(channel));
                Chat = new ChatService(new ChatClient(channel));
                CaptureFrames = new CaptureFramesService(new CaptureFramesClient(channel), Users);

                isInitialized = true;
            }
        }
    }

}
