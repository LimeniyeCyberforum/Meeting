using Meeting.Core.Common.Abstractions.Chat;
using Meeting.Core.Common.Abstractions.FrameCapture;
using Meeting.Core.Common.Abstractions.Users;
using Meeting.Core.Common.DataTypes;
using System;
using System.Threading.Tasks;

namespace Meeting.Core.Common.Abstractions
{
    public enum UserConnectionState
    {
        Connected,
        Disconnected
    }

    public interface IMeetingAuthorization
    {
        UserDto CurrentUser { get; }

        UserConnectionState CurrentConnectionState { get; }

        event EventHandler<UserConnectionState> AuthorizationStateChanged;

        void JoinToLobby(string username);

        Task JoinToLobbyAsync(string username);

        Task<bool> IsNameExistsAsync(string username);

        bool IsNameExists(string username);
    }

    public interface IMeetingUsers
    {
        UsersServiceAbstract Users { get; }
    }

    public interface IMeetingChat
    {
        ChatServiceAbstract Chat { get; }
    }

    public interface IMeetingCaptureFrames
    {
        CaptureFramesServiceAbstract CaptureFrames { get; }
    }


    public interface IMeetingService : IMeetingAuthorization, IMeetingUsers, IMeetingChat, IMeetingCaptureFrames, IDisposable
    {
    }
}
