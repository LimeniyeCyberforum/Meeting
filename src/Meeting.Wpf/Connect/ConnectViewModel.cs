using Meeting.Business.Common.Abstractions;
using MvvmCommon.WindowsDesktop;

namespace Meeting.Wpf.Connect
{
    public class ConnectViewModel : BaseInpc
    {
        private readonly IMeetingAuthorization _meetingAuthorization;

        private string _name;
        private bool? _isValidName = null;

        public string Name { get => _name; set => Set(ref _name, value); }
        public bool? IsValidName { get => _isValidName; set => Set(ref _isValidName, value); }

        #region JoinCommand
        private RelayCommandAsync _joinCommand;
        public RelayCommandAsync JoinCommand => _joinCommand ?? (
            _joinCommand = new RelayCommandAsync(OnJoinExecute, CanJoinExecute));

        private async void OnJoinExecute()
        {
            await _meetingAuthorization.JoinToLobbyAsync(Name);
        }

        private bool CanJoinExecute()
        {
            return !(JoinCommand.IsBusy || string.IsNullOrWhiteSpace(Name));
        }
        #endregion

        public ConnectViewModel(IMeetingAuthorization meetingAuthorization)
        {
            _meetingAuthorization = meetingAuthorization;
            ProtectedPropertyChanged += OnProtectedPropertyChanged;
        }

        private void OnProtectedPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (string.Equals(propertyName, nameof(Name)))
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    IsValidName = null;
                    return;
                }

                IsValidName = !_meetingAuthorization.IsNameExists(Name);
            }
        }
    }
}
