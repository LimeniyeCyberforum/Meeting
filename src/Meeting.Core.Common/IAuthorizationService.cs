using System;
using System.Threading.Tasks;
using Meeting.Core.Common.DataTypes;

namespace Meeting.Core.Common
{
    public enum UserConnectionState
    {
        Connected,
        Disconnected
    }

    public interface IAuthorizationService
    {
        UserDto CurrentUser { get; }

        UserConnectionState CurrentConnectionState { get; }

        event EventHandler<UserConnectionState> AuthorizationStateChanged;

        void JoinToLobby(string username);

        Task JoinToLobbyAsync(string username);

        Task<bool> IsNameExistsAsync(string username);

        bool IsNameExists(string username);
    }
}
