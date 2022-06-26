using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Meeting.Wpf.ViewModels
{
    public class CaptureFrameViewModel : ReactiveObject
    {
        [Reactive] public Guid AreaGuid { get; set;}
        [Reactive] public Guid OwnerGuid { get; set; }
        [Reactive] public string? OwnerName { get; set; }
        [Reactive] public byte[]? Data { get; set; }

        public CaptureFrameViewModel(Guid areaGuid, Guid ownerGuid, string ownerName, byte[]? data)
        {
            AreaGuid = areaGuid;
            OwnerGuid = ownerGuid;
            OwnerName = ownerName;
            Data = data;
        }
    }
}
