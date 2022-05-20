using MeetingCommon.Abstractions.Messanger;
using WPFView.Connect;

namespace WPFView
{
    public class MainViewModel : BaseInpc
    {
        public ConnectViewModel ConnectVM { get; }

        public MainViewModel(MessageServiceAbstract messageService)
        {
            ConnectVM = new ConnectViewModel();

        }
    }
}
