using MeetingCommon.Abstractions;

namespace WPFView.Connect
{
    public class ConnectViewModel : BaseInpc
    {
        private readonly MeetingServiceAbstract _meetingService;

        private string _name;

        public string Name { get => _name; set => Set(ref _name, value); }

        #region JoinCommand
        private RelayCommandAsync _joinCommand;
        public RelayCommandAsync JoinCommand => _joinCommand ?? (
            _joinCommand = new RelayCommandAsync(OnJoinExecute, CanJoinExecute));

        private async void OnJoinExecute()
        {
            await _meetingService.ConnectAsync(Name);
        }

        private bool CanJoinExecute()
        {
            return JoinCommand.IsBusy || string.IsNullOrEmpty(Name) || Name == "" ? false : true;
        }
        #endregion

        public ConnectViewModel(MeetingServiceAbstract meetingSErvice)
        {
            _meetingService = meetingSErvice;
        }
    }
}
