namespace Meeting.Business.Common.Abstractions.HibernateSessions
{
    public static class HibernateSessionAndroid
    {
        public static void OnCreate(this IMeetingService meetingService)
        {
            meetingService.Chat.ChatSubscribeAsync();
            meetingService.Users.UsersSubscribeAsync();
            meetingService.CaptureFrames.CaptureFrameAreasSubscribeAsync();
        }

        public static void OnStart(this IMeetingService meetingService) { }

        public static void OnResume(this IMeetingService meetingService)
        {
            meetingService.CaptureFrames.CaptureFramesSubscribeAsync();
        }

        public static void OnPause(this IMeetingService meetingService)
        {
            meetingService.CaptureFrames.CaptureFramesUnsubscribe();
        }

        public static void OnStop(this IMeetingService meetingService) { }

        public static void OnRestart(this IMeetingService meetingService) { }

        public static void OnDestroy(this IMeetingService meetingService)
        {
            meetingService.Chat.ChatUnsubscribe();
            meetingService.Users.UsersUnsubscribe();
            meetingService.CaptureFrames.CaptureFrameAreasUnsubscribe();
            meetingService.CaptureFrames.CaptureFramesUnsubscribe();
        }
    }
}
