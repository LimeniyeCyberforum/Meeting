using Meeting.Business.Common.DataTypes;
using System;
using System.Threading.Tasks;

namespace Meeting.Business.Common.Abstractions.Authorization
{
    public enum UserConnectionState
    {
        Connected,
        Disconnected
    }

    public abstract class AuthorizationServiceAbstract
    {
        public UserDto CurrentUser { get; protected set;  }

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
