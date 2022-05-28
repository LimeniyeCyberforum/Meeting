using MeetingCommon.Abstractions;

namespace MeetingWPF.Connect
{
    public class ConnectViewModel : BaseInpc
    {
        private readonly IMeetingConnectionService _meetingConnectionService;

        private string _name;

        public string Name { get => _name; set => Set(ref _name, value); }

        #region JoinCommand
        private RelayCommandAsync _joinCommand;
        public RelayCommandAsync JoinCommand => _joinCommand ?? (
            _joinCommand = new RelayCommandAsync(OnJoinExecute, CanJoinExecute));

        private async void OnJoinExecute()
        {
            await _meetingConnectionService.ConnectAsync(Name);
        }

        private bool CanJoinExecute()
        {
            return JoinCommand.IsBusy || string.IsNullOrEmpty(Name) || Name == "" ? false : true;
        }
        #endregion

        public ConnectViewModel(IMeetingConnectionService meetingConnectionService)
        {
            _meetingConnectionService = meetingConnectionService;
        }
    }
}
