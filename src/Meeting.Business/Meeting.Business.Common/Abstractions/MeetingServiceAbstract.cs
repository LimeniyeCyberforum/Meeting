using Meeting.Business.Common.Abstractions.Chat;
using Meeting.Business.Common.Abstractions.FrameCapture;
using Meeting.Business.Common.Abstractions.Users;
using Meeting.Business.Common.DataTypes;
using System;
using System.Threading.Tasks;

namespace Meeting.Business.Common.Abstractions
{
    public enum UserConnectionState
    {
        Connected,
        Disconnected
    }

    public abstract class MeetingServiceAbstract
    {
        public UsersServiceAbstract Users { get; }
        public ChatServiceAbstract Chat { get; }
        public CaptureFrameServiceAbstract FrameCaptures { get; }

        public UserDto CurrentUser { get; protected set; }

        public UserConnectionState CurrentConnectionState { get; protected set; }

        public event EventHandler<UserConnectionState> AuthorizationStateChanged;

        public abstract void JoinToLobby(string username);

        public abstract Task JoinToLobbyAsync(string username);

        protected void RaiseAuthorizationStateChangedEvent(UserConnectionState newState)
        {
            AuthorizationStateChanged?.Invoke(this, newState);
        }

    }
}
