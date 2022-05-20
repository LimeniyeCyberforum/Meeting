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
        ConnectionAction CurrentConnectionState { get; }

        event EventHandler<ConnectionAction> ConnectionStateChanged;

        UserDto Connect(string username);

        Task<UserDto> ConnectAsync(string username);
    }

    public abstract class MeetingServiceAbstract : IMeetingConnectionService
    {
        public ConnectionAction CurrentConnectionState { get; protected set; }

        public event EventHandler<ConnectionAction> ConnectionStateChanged;

        public MessageServiceAbstract MessageService { get; protected set; }

        public CameraCaptureServiceAbstract CameraCaptureService { get; protected set; }

        public abstract UserDto Connect(string username);

        public abstract Task<UserDto> ConnectAsync(string username);

        protected void RaiseConnectionStateChangedAction(ConnectionAction action)
        {
            CurrentConnectionState = action;
            ConnectionStateChanged?.Invoke(this, action);
        }
    }
}
