using MeetingCommon.Abstractions.CameraCapture;
using MeetingCommon.Abstractions.Messanger;
using MeetingCommon.DataTypes;
using System.Threading.Tasks;

namespace MeetingCommon.Abstractions
{
    public abstract class MeetingServiceAbstract
    {
        public abstract MessageServiceAbstract MessageService { get; protected set; }

        public abstract CameraCaptureServiceAbstract CameraCaptureService { get; protected set; }

        public abstract UserDto Connect(string username);

        public abstract Task<UserDto> ConnectAsync(string username);
    }
}
