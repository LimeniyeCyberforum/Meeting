using Meeting.Business.Common.Abstractions;
using MvvmCommon.WindowsDesktop;

namespace Meeting.WPF.Connect
{
    public class ConnectViewModel : BaseInpc
    {
        private readonly MeetingServiceAbstract _meetingConnectionService;

        private string _name;

        public string Name { get => _name; set => Set(ref _name, value); }

        #region JoinCommand
        private RelayCommandAsync _joinCommand;
        public RelayCommandAsync JoinCommand => _joinCommand ?? (
            _joinCommand = new RelayCommandAsync(OnJoinExecute, CanJoinExecute));

        private async void OnJoinExecute()
        {
            await _meetingConnectionService.JoinToLobbyAsync(Name);
        }

        private bool CanJoinExecute()
        {
            return !(JoinCommand.IsBusy || string.IsNullOrEmpty(Name) || Name == "");
        }
        #endregion

        public ConnectViewModel(MeetingServiceAbstract meetingConnectionService)
        {
            _meetingConnectionService = meetingConnectionService;
        }
    }
}
