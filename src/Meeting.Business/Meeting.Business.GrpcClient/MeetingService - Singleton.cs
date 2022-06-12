using System.Threading;

using ChatClient = MeetingGrpc.Protos.Chat.ChatClient;
using UsersClient = MeetingGrpc.Protos.Users.UsersClient;
using CaptureFramesClient = MeetingGrpc.Protos.CaptureFrames.CaptureFramesClient;
using AuthorizationClient = MeetingGrpc.Protos.Authorization.AuthorizationClient;


namespace Meeting.Business.GrpcClient
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
