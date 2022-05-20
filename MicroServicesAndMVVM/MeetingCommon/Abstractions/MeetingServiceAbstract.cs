using MeetingCommon.Abstractions.CameraCapture;
using MeetingCommon.Abstractions.Messanger;
using MeetingCommon.DataTypes;
using System.Threading.Tasks;

namespace MeetingCommon.Abstractions
{
    public abstract class MeetingServiceAbstract
    {
        public abstract MessageServiceAbstract MessageService { get; }

        public abstract CameraCaptureServiceAbstract CameraCaptureService { get; }

        public abstract UserDto Connect(string username);

        public abstract Task<UserDto> ConnectAsync(string username);
    }
}
