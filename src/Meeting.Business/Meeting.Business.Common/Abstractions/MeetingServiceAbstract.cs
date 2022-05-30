using Meeting.Business.Common.Abstractions.CameraCapture;
using Meeting.Business.Common.Abstractions.Messanger;
using Meeting.Business.Common.DataTypes;
using System;
using System.Threading.Tasks;

namespace Meeting.Business.Common.Abstractions
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

        event EventHandler<(ConnectionAction Action, UserDto User)> ConnectionStateChanged;

        UserDto Connect(string username);

        Task<UserDto> ConnectAsync(string username);
    }

    public abstract class MeetingServiceAbstract : IMeetingConnectionService
    {
        public UserDto CurrentUser { get; private set; }

        public ConnectionAction CurrentConnectionState { get; private set; }

        public event EventHandler<(ConnectionAction Action, UserDto User)> ConnectionStateChanged;

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
