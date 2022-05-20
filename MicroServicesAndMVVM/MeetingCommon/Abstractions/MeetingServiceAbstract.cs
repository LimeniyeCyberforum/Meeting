using MeetingCommon.Abstractions.CameraCapture;
using MeetingCommon.Abstractions.Messanger;
using MeetingCommon.DataTypes;
using System;
using System.Threading.Tasks;

namespace MeetingCommon.Abstractions
{
    public enum ConnectionAction
    {
        Connected,
        Disconnected
    }

    public interface IMeetingConnectionService
    {
        UserDto CurrentUser { get; }

        ConnectionAction CurrentConnectionState { get; }

        event EventHandler<(ConnectionAction Action, UserDto user)> ConnectionStateChanged;

        UserDto Connect(string username);

        Task<UserDto> ConnectAsync(string username);
    }

    public abstract class MeetingServiceAbstract : IMeetingConnectionService
    {
        public UserDto CurrentUser { get; private set; }

        public ConnectionAction CurrentConnectionState { get; private set; }

        public event EventHandler<(ConnectionAction Action, UserDto user)> ConnectionStateChanged;

        public MessageServiceAbstract MessageService { get; protected set; }

        public CameraCaptureServiceAbstract CameraCaptureService { get; protected set; }

        public abstract UserDto Connect(string username);

        public abstract Task<UserDto> ConnectAsync(string username);

        protected void RaiseConnectionStateChangedAction(ConnectionAction action, UserDto user)
        {
            CurrentConnectionState = action;
            CurrentUser = user;
            ConnectionStateChanged?.Invoke(this, (action, user));
        }
    }
}
