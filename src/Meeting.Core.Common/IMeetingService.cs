using System;

namespace Meeting.Core.Common
{
    public interface IMeetingAuthorization
    {
        IAuthorizationService Authorization { get; }
    }

    public interface IMeetingUsers
    {
        IUsersService Users { get; }
    }

    public interface IMeetingChat
    {
        IChatService Chat { get; }
    }

    public interface IMeetingCaptureFrames
    {
        ICaptureFramesService CaptureFrames { get; }
    }

    public interface IMeetingService : IMeetingAuthorization, IMeetingUsers, IMeetingChat, IMeetingCaptureFrames, IDisposable
    {
    }
}
