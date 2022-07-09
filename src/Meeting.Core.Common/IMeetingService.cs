using Meeting.Core.Common.DataTypes;
using System;
using System.Threading.Tasks;

namespace Meeting.Core.Common
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
